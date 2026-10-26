namespace VotePreferentiel.Tests;

public sealed class VoteCounterTests
{
    private readonly VoteCounter _voteCounter = new();

    [Test]
    public async Task GetWinner_ReturnsChloeForExampleVotes()
    {
        var voteModel = CreateVoteModel(
            ["Alice", "Bilal", "Chloé"],
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Bilal", "Alice"],
            ["Chloé", "Alice", "Bilal"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_ReturnsOnlyCandidate()
    {
        var voteModel = CreateVoteModel(["Alice"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetWinner_TransfersPreferencesAfterEliminatingCandidates()
    {
        var voteModel = CreateVoteModel(
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Bilal", "Chloé", "Alice", "Diane"],
            ["Bilal", "Chloé", "Alice", "Diane"],
            ["Bilal", "Chloé", "Alice", "Diane"],
            ["Chloé", "Alice", "Bilal", "Diane"],
            ["Chloé", "Alice", "Bilal", "Diane"],
            ["Chloé", "Alice", "Bilal", "Diane"],
            ["Diane", "Chloé", "Bilal", "Alice"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_EliminatesAllCandidatesTiedForLastPlace()
    {
        var voteModel = CreateVoteModel(
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Alice", "Bilal"],
            ["Chloé", "Bilal", "Alice"]);

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_ThrowsForAnEmptyBallot()
    {
        var voteModel = CreateVoteModel([]);

        var argumentExceptionThrown = false;
        try
        {
            _voteCounter.GetWinner(voteModel);
        }
        catch (ArgumentException)
        {
            argumentExceptionThrown = true;
        }

        await Assert.That(argumentExceptionThrown).IsTrue();
    }

    private static VoteModel CreateVoteModel(params string[][] ballots)
    {
        return new VoteModel
        {
            Votes = ballots
                .Select(choices => new BallotModel { Choices = choices.ToList() })
                .ToList()
        };
    }
}

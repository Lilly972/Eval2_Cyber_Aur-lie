namespace VotePreferentiel;

public sealed class VoteModel
{
    public List<BallotModel> Votes { get; init; } = [];
}

public sealed class BallotModel
{
    public List<string> Choices { get; init; } = [];
}

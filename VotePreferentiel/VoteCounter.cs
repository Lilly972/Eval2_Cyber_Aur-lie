namespace VotePreferentiel;

public sealed class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        return GetBallots(voteModel)
            .SelectMany(ballot => ballot.Choices)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public string GetWinner(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        var ballots = GetBallots(voteModel);
        if (ballots.Count == 0)
        {
            throw new ArgumentException("The election must contain at least one ballot.", nameof(voteModel));
        }

        if (ballots.Any(ballot => ballot.Choices.Count == 0))
        {
            throw new ArgumentException("Ballots cannot be empty.", nameof(voteModel));
        }

        var candidates = GetCandidates(voteModel);
        if (candidates.Count == 0)
        {
            throw new ArgumentException("The election must contain at least one candidate.", nameof(voteModel));
        }

        var activeCandidates = new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase);

        while (activeCandidates.Count > 1)
        {
            var counts = CountFirstChoices(GetAllFirstChoices(voteModel, activeCandidates));
            foreach (var candidate in activeCandidates)
            {
                counts.TryAdd(candidate, 0);
            }

            var eliminatedCandidates = GetCandidatesWithLeastVotes(counts);
            if (eliminatedCandidates.Length == activeCandidates.Count)
            {
                throw new InvalidOperationException("The election has no unique winner.");
            }

            activeCandidates.ExceptWith(eliminatedCandidates);
        }

        return activeCandidates.Single();
    }

    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        var ballots = GetBallots(voteModel);
        if (ballots.Any(ballot => ballot.Choices.Count == 0))
        {
            throw new ArgumentException("Ballots cannot be empty.", nameof(voteModel));
        }

        return ballots.Select(ballot => ballot.Choices[0]).ToList();
    }

    public IDictionary<string, int> CountFirstChoices(List<string> firstChoices)
    {
        ArgumentNullException.ThrowIfNull(firstChoices);

        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var choice in firstChoices)
        {
            if (string.IsNullOrWhiteSpace(choice))
            {
                throw new ArgumentException("Candidate names cannot be empty.", nameof(firstChoices));
            }

            counts[choice] = counts.GetValueOrDefault(choice) + 1;
        }

        return counts;
    }

    public string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts)
    {
        ArgumentNullException.ThrowIfNull(firstChoiceCounts);
        if (firstChoiceCounts.Count == 0)
        {
            throw new ArgumentException("At least one candidate count is required.", nameof(firstChoiceCounts));
        }

        var leastVotes = firstChoiceCounts.Values.Min();
        return firstChoiceCounts
            .Where(candidate => candidate.Value == leastVotes)
            .Select(candidate => candidate.Key)
            .ToArray();
    }

    private static List<BallotModel> GetBallots(VoteModel voteModel)
    {
        if (voteModel.Votes is null || voteModel.Votes.Any(ballot => ballot is null || ballot.Choices is null))
        {
            throw new ArgumentException("The election contains an invalid ballot.", nameof(voteModel));
        }

        if (voteModel.Votes.SelectMany(ballot => ballot.Choices).Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Candidate names cannot be empty.", nameof(voteModel));
        }

        foreach (var ballot in voteModel.Votes)
        {
            if (ballot.Choices.Distinct(StringComparer.OrdinalIgnoreCase).Count() != ballot.Choices.Count)
            {
                throw new ArgumentException("A candidate can appear only once in a ballot.", nameof(voteModel));
            }
        }

        return voteModel.Votes;
    }

    private static List<string> GetAllFirstChoices(
        VoteModel voteModel,
        HashSet<string> activeCandidates)
    {
        return GetBallots(voteModel)
            .Select(ballot => ballot.Choices.FirstOrDefault(activeCandidates.Contains))
            .Where(choice => choice is not null)
            .Select(choice => choice!)
            .ToList();
    }
}

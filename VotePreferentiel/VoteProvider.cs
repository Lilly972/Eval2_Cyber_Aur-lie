using System.Net.Http.Json;
using System.Text.Json;

namespace VotePreferentiel;

public sealed class VoteProvider(HttpClient httpClient) : IVoteProvider
{
    public async Task<VoteModel> GetVotesAsync(CancellationToken cancellationToken = default)
    {
        var voteModel = await httpClient.GetFromJsonAsync<VoteModel>(
            string.Empty,
            cancellationToken);

        return voteModel ?? throw new JsonException("The votes JSON document was empty.");
    }
}

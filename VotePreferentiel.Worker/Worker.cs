using VotePreferentiel;

namespace VotePreferentiel.Worker;

public class Worker(
    ILogger<Worker> logger,
    IVoteCounter voteCounter,
    IVoteProvider voteProvider,
    IHostApplicationLifetime applicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        var votes = await voteProvider.GetVotesAsync(stoppingToken);
        var winner = voteCounter.GetWinner(votes);

        logger.LogInformation("The winner is {Winner}.", winner);
        applicationLifetime.StopApplication();
    }
}

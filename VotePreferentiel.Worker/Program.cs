using VotePreferentiel;
using VotePreferentiel.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IVoteCounter, VoteCounter>();
builder.Services.AddHttpClient<IVoteProvider, VoteProvider>((services, client) =>
{
    var jsonUrl = services.GetRequiredService<IConfiguration>()["VoteSettings:JsonUrl"];
    if (!Uri.TryCreate(jsonUrl, UriKind.Absolute, out var jsonUri))
    {
        throw new InvalidOperationException(
            "The setting 'VoteSettings:JsonUrl' must contain an absolute URL.");
    }

    client.BaseAddress = jsonUri;
});
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

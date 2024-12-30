using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SpotifyCli.Infrastructure;

class AppConfig
{
    public const string ClientId = "your-client-id-here";
    public string CredentialsPath { get; }
    public int Port { get; }
    public string CallbackUrl => $"http://localhost:{Port}/callback";

    public AppConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("spotifycli-settings.json", optional: false)
            .Build();

        Debug.Assert(config["Port"] is not null);
        Debug.Assert(config["CredentialsPath"] is not null);

        CredentialsPath = config["CredentialsPath"]!;
        Port = int.Parse(config["Port"]!);
    }

    public void Save() =>
        File.WriteAllText(
            "spotifycli-settings.json",
            JsonConvert.SerializeObject(this, Formatting.Indented)
        );

    public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
}

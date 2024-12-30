using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SpotifyCli.Infrastructure;

class AppConfig
{
    public string ClientId { get; }
    public string CredentialsPath { get; }
    public int Port { get; }
    public string CallbackUrl { get; }

    public AppConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("spotifycli-settings.json", optional: false)
            .Build();

        Debug.Assert(config["ClientId"] is not null);
        Debug.Assert(config["CredentialsPath"] is not null);
        Debug.Assert(config["Port"] is not null);
        Debug.Assert(int.TryParse(config["Port"], out _));
        Debug.Assert(config["CallbackUrl"] is not null);

        ClientId = config["ClientId"]!;
        CredentialsPath = config["CredentialsPath"]!;
        Port = int.Parse(config["Port"]!);
        CallbackUrl = config["CallbackUrl"]!;
    }

    public void Save() =>
        File.WriteAllText(
            "spotifycli-settings.json",
            JsonConvert.SerializeObject(this, Formatting.Indented)
        );

    public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
}

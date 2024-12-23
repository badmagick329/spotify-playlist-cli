using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SpotifyCli.Infrastructure;

class AppConfig
{
    public string CredentialsPath { get; }
    public string ClientId { get; }
    public string CallbackUrl { get; }
    public int Port { get; }
    public string PlaylistName { get; }
    public string SampleDeviceId { get; set; }

    public AppConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("spotifycli-settings.json", optional: false)
            .Build();

        Debug.Assert(config["CredentialsPath"] is not null);
        Debug.Assert(config["ClientId"] is not null);
        Debug.Assert(config["CallbackUrl"] is not null);
        Debug.Assert(config["Port"] is not null);
        Debug.Assert(config["PlaylistName"] is not null);

        CredentialsPath = config["CredentialsPath"]!;
        ClientId = config["ClientId"]!;
        CallbackUrl = config["CallbackUrl"]!;
        Port = int.Parse(config["Port"]!);
        PlaylistName = config["PlaylistName"]!;
        SampleDeviceId = config["SampleDeviceId"] ?? "";
    }

    public void Save() =>
        File.WriteAllText(
            "spotifycli-settings.json",
            JsonConvert.SerializeObject(this, Formatting.Indented)
        );

    public override string ToString() =>
        $"CredentialsPath: {CredentialsPath}\nClientId: {ClientId}\nCallbackUrl: {CallbackUrl}\nPort: {Port}\nPlaylistName: {PlaylistName}";
}

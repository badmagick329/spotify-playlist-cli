using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using static SpotifyAPI.Web.Scopes;

namespace SpotifyCli.Infrastructure;

class SpotifyAuthenticator(string clientId, string credentialsPath, string callbackUrl, int port)
{
    private readonly string _credentialsPath = credentialsPath;
    private readonly string _clientId = clientId;
    private readonly string _callbackUrl = callbackUrl;
    private readonly int _port = port;

    private EmbedIOAuthServer _server;
    public SpotifyClient Client { get; private set; }

    public async Task CreateClient()
    {
        // This is a bug in the SWAN Logging library, need this hack to bring back the cursor
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => Exiting();

        if (string.IsNullOrEmpty(_clientId))
        {
            throw new NullReferenceException("Please set ClientId via config");
        }

        if (File.Exists(_credentialsPath))
        {
            await Start();
        }
        else
        {
            await StartAuthentication();
        }
        Console.WriteLine($"Press enter when done");
        _ = Console.ReadKey();
        Console.WriteLine($"Done. Client is ready.\n{Client}");
    }

    private static void Exiting() => Console.CursorVisible = true;

    private async Task Start()
    {
        var json = await File.ReadAllTextAsync(_credentialsPath);
        var token =
            JsonConvert.DeserializeObject<PKCETokenResponse>(json)
            ?? throw new InvalidOperationException("Failed to deserialize token");
        var authenticator = new PKCEAuthenticator(_clientId, token);
        authenticator.TokenRefreshed += (sender, token) =>
            File.WriteAllText(_credentialsPath, JsonConvert.SerializeObject(token));

        var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);

        Client = new SpotifyClient(config);
        _server?.Dispose();
    }

    private async Task StartAuthentication()
    {
        _server = new(new Uri(_callbackUrl), _port);
        var (verifier, challenge) = PKCEUtil.GenerateCodes();

        await _server.Start();
        _server.AuthorizationCodeReceived += async (sender, response) =>
        {
            await _server.Stop();
            var token = await new OAuthClient().RequestToken(
                new PKCETokenRequest(_clientId, response.Code, _server.BaseUri, verifier)
            );

            await File.WriteAllTextAsync(_credentialsPath, JsonConvert.SerializeObject(token));
            await Start();
        };

        var request = new LoginRequest(_server.BaseUri, _clientId, LoginRequest.ResponseType.Code)
        {
            CodeChallenge = challenge,
            CodeChallengeMethod = "S256",
            Scope =
            [
                AppRemoteControl,
                PlaylistModifyPrivate,
                PlaylistReadCollaborative,
                PlaylistReadPrivate,
                UserLibraryModify,
                UserLibraryRead,
                UserModifyPlaybackState,
                UserReadCurrentlyPlaying,
                UserReadEmail,
                UserReadPlaybackPosition,
                UserReadPlaybackPosition,
                UserReadPlaybackState,
                UserReadPrivate,
                UserTopRead,
                PlaylistModifyPublic,
            ],
        };

        var uri = request.ToUri();
        try
        {
            BrowserUtil.Open(uri);
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to open URL, manually open: {0}", uri);
        }
    }
}

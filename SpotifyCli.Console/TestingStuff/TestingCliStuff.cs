using Spectre.Console;
using SpotifyAPI.Web;
using SpotifyCli.Infrastructure;

namespace SpotifyCli.TestingStuff;

class CliExamples
{
    private string _playlistName;
    private readonly SpotifyAuthenticator _spotifyAuthenticator;
    private SpotifyClient Client =>
        _spotifyAuthenticator.Client ?? throw new NullReferenceException("Client not initialized");

    public CliExamples(AppConfig config)
    {
        _spotifyAuthenticator = new SpotifyAuthenticator(
            config.ClientId,
            config.CredentialsPath,
            config.CallbackUrl,
            config.Port
        );
        _playlistName = config.PlaylistName;
    }

    public async Task Initialize() => await _spotifyAuthenticator.CreateClient();

    public async Task PromptSelection()
    {
        var allPlaylists = await FetchAllPlaylists();
        var selectedPlaylists = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[green]Select playlists to process[/]")
                .NotRequired()
                .PageSize(10)
                .MoreChoicesText("[grey]Move up and down to reveal more playlists[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to select, [green]<enter>[/] to accept, [red]<esc>[/] to cancel)[/]"
                )
                .UseConverter(playlistName => $"{playlistName}")
                .AddChoiceGroup(
                    "Playlists",
                    allPlaylists
                        .Select(p => $"{p.Name} ({p.Tracks?.Total ?? 0} tracks)")
                        .OrderBy(name => name)
                        .ToList()
                )
        );

        if (selectedPlaylists.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No playlists selected. Operation cancelled.[/]");
            return;
        }
    }

    private async Task<IList<FullPlaylist>> FetchAllPlaylists()
    {
        return await Client.PaginateAll(
            await Client.Playlists.CurrentUsers().ConfigureAwait(false)
        );
    }
}

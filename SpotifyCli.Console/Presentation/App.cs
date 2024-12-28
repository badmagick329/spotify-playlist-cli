using Spectre.Console;
using SpotifyCli.Application;
using SpotifyCli.Core;
using SpotifyCli.Infrastructure;

namespace SpotifyCli.Presentation;

class App
{
    public Client Client { get; private set; }

    public App()
    {
        var config = new AppConfig();
        Client = new Client(config);
    }

    public async Task Initialize() => await Client.Initialize();

    public async Task Run()
    {
        var newName = CreateFilteredPlaylistInputHandler.AskPlaylistName();
        var createFilteredPlaylist = new CreateFilteredPlaylist(Client, newName);
        await createFilteredPlaylist.Initialize();

        var selectedPlaylists = CreateFilteredPlaylistInputHandler.AskSelectedPlayistIds(
            createFilteredPlaylist.AllPlaylists
        );
        if (selectedPlaylists.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No playlists selected. Operation cancelled.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[yellow]Fetching tracks from playlists...[/]");
        await createFilteredPlaylist.SetSourcePlaylists(selectedPlaylists);

        switch (CreateFilteredPlaylistInputHandler.AskFilterType())
        {
            case FilterType.DateRange:
                AnsiConsole.MarkupLine("[yellow]Filtering by date range...[/]");
                await FilterByDateRange(createFilteredPlaylist);
                break;
            case FilterType.Artists:
                AnsiConsole.MarkupLine("[yellow]Filtering by artists...[/]");
                await FilterByArtists(createFilteredPlaylist);
                break;
        }
    }

    public static async Task FilterByDateRange(CreateFilteredPlaylist createFilteredPlaylist)
    {
        (var startDate, var endDate) = CreateFilteredPlaylistInputHandler.AskReleaseDate();
        AnsiConsole.MarkupLine(
            $"[yellow]Creating playlist with tracks from {startDate} to {endDate}...[/]"
        );
        await createFilteredPlaylist.SpotifyPlaylistFromDateRange(startDate, endDate);
    }

    public static async Task FilterByArtists(CreateFilteredPlaylist createFilteredPlaylist)
    {
        var artistsInPlaylists = createFilteredPlaylist
            .ArtistsInSourcePlaylists()
            .OrderBy(a => a)
            .ToList();

        var artists = CreateFilteredPlaylistInputHandler.AskArtists(artistsInPlaylists);
        AnsiConsole.MarkupLine("[yellow]Creating playlist with tracks by selected artists...[/]");
        await createFilteredPlaylist.SpotifyPlaylistFromArtists(artists);
    }
}

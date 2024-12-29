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

        foreach (var filterType in CreateFilteredPlaylistInputHandler.AskFilterTypes())
        {
            switch (filterType)
            {
                case FilterType.DateRange:
                    (var startDate, var endDate) =
                        CreateFilteredPlaylistInputHandler.AskReleaseDate();

                    createFilteredPlaylist.AddFilterFromDateRange(startDate, endDate);

                    break;
                case FilterType.Artists:
                    var artistsInPlaylists = createFilteredPlaylist
                        .ArtistsInSourcePlaylists()
                        .OrderBy(a => a)
                        .ToList();
                    var artists = CreateFilteredPlaylistInputHandler.AskArtists(artistsInPlaylists);

                    createFilteredPlaylist.AddFilterFromArtists(artists);
                    break;
            }
        }
        if (createFilteredPlaylist.FilteredPlaylist.Tracks.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No tracks match the criteria. Operation cancelled.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[yellow]Creating filtered playlist...[/]");
        await createFilteredPlaylist.CreateSpotifyPlaylist();
    }
}

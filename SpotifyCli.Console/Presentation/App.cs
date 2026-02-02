using SpotifyCli.Application;
using SpotifyCli.Core;
using SpotifyCli.Infrastructure;

namespace SpotifyCli.Presentation;

class App
{
    public Client Client { get; private set; }
    private readonly IUserInputHandler UserInputHandler;
    private readonly IUserNotifier UserNotifier;
    private readonly IUserOutput UserOutput;

    public App(
        IUserInputHandler userInputHandler,
        IUserNotifier userNotifier,
        IUserOutput userOutput
    )
    {
        var config = new AppConfig();
        Client = new Client(config);
        UserInputHandler = userInputHandler;
        UserNotifier = userNotifier;
        UserOutput = userOutput;
    }

    public async Task Initialize() => await Client.Initialize();

    public async Task RunPlaylistCreator()
    {
        var newName = UserInputHandler.AskNewPlaylistName();
        var createFilteredPlaylist = new CreateFilteredPlaylist(Client, newName);
        await createFilteredPlaylist.Initialize();

        var selectedPlaylists = UserInputHandler.AskSelectedPlayistIds(
            createFilteredPlaylist.AllPlaylists
        );
        if (selectedPlaylists.Count == 0)
        {
            UserNotifier.Notify(NotifyEvent.NoPlaylistsSelected);
            return;
        }

        UserNotifier.Notify(NotifyEvent.FetchingTracks);
        await createFilteredPlaylist.SetSourcesAndInitializeFilteredPlaylist(selectedPlaylists);

        foreach (var filterType in UserInputHandler.AskFilterTypes())
        {
            switch (filterType)
            {
                case FilterType.DateRange:
                    (var startDate, var endDate) = UserInputHandler.AskReleaseDate();

                    createFilteredPlaylist.AddFilterFromDateRange(startDate, endDate);
                    break;
                case FilterType.Artists:
                    var artistsInPlaylists = createFilteredPlaylist
                        .ArtistsInSourcePlaylists()
                        .OrderBy(a => a)
                        .ToList();
                    var artists = UserInputHandler.AskArtists(artistsInPlaylists);

                    createFilteredPlaylist.AddFilterFromArtists(artists);
                    break;
            }
        }
        if (createFilteredPlaylist.FilteredPlaylist.Tracks.Count == 0)
        {
            UserNotifier.Notify(NotifyEvent.NoTracksMatchedCriteria);
            return;
        }

        UserNotifier.Notify(NotifyEvent.CreatingPlaylist);
        await createFilteredPlaylist.CreateSpotifyPlaylist();
        UserNotifier.Notify(NotifyEvent.CreatedPlaylist);
    }

    public async Task RunListPlaylistSongs()
    {
        var allPlaylists = await Client.FetchAllPlaylists();
        var selectedPlaylists = UserInputHandler.AskSelectedPlayistIds(allPlaylists);
        if (selectedPlaylists.Count == 0)
        {
            UserNotifier.Notify(NotifyEvent.NoPlaylistsSelected);
            return;
        }

        List<Track> tracks = [];
        foreach (var playlistId in selectedPlaylists)
        {
            var playlistTracks = await Client.FetchPlaylistTracks(playlistId);
            tracks.AddRange(playlistTracks);
        }

        UserOutput.Write(
            tracks.Select(t => t.ToString()).Distinct().Aggregate((a, b) => a + "\n" + b)
        );
    }

    public async Task Menu()
    {
        var appMode = UserInputHandler.AskAppMode();
        if (appMode == AppMode.CreatePlaylist)
        {
            await RunPlaylistCreator();
            return;
        }
        else if (appMode == AppMode.ListPlaylistSongs)
        {
            await RunListPlaylistSongs();
            return;
        }

        return;
    }
}

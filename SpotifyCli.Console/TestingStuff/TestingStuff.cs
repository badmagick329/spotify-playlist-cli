using System.Diagnostics;
using SpotifyAPI.Web;
using SpotifyCli.Infrastructure;

namespace SpotifyCli.TestingStuff;

class Examples
{
    private static string SampleDeviceId { get; set; }
    private static string PlaylistName { get; set; }

    public static async Task PlaylistPlayExample(AppConfig config)
    {
        var auth = new SpotifyAuthenticator(
            config.ClientId,
            config.CredentialsPath,
            config.CallbackUrl,
            config.Port
        );
        SampleDeviceId = config.SampleDeviceId;
        PlaylistName = config.PlaylistName;

        await auth.CreateClient();
        var client = auth.Client;

        var allPlaylists = await FetchAllPlaylists(client);
        var playlist = SamplePlaylist(allPlaylists);
        Debug.Assert(playlist.Id is not null);

        try
        {
            var tracks = await FetchPlaylistTracks(client, playlist.Id);
            while (true)
            {
                Console.WriteLine("Enter search term");
                var searchTerm = Console.ReadLine() ?? "";
                Console.WriteLine($"Searching for {searchTerm}");
                var searchResults = FilterTracks(searchTerm, tracks);
                FullTrack track;
                if (searchResults.Count == 1)
                {
                    track = searchResults[0];
                }
                else if (searchResults.Count > 1)
                {
                    Console.WriteLine("Multiple tracks found, please select one:");
                    for (var i = 0; i < searchResults.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}: {searchResults[i].Name}");
                    }
                    var selection = int.Parse(Console.ReadLine() ?? "1");
                    track = searchResults[selection - 1];
                }
                else
                {
                    continue;
                }
                await PlayTrack(client, track);
            }
        }
        catch (APIException ex)
        {
            Console.WriteLine($"Error playing track: {ex.Message}");
        }
    }

    private static async Task<IList<FullPlaylist>> FetchAllPlaylists(SpotifyClient client)
    {
        return await client.PaginateAll(
            await client.Playlists.CurrentUsers().ConfigureAwait(false)
        );
    }

    private static FullPlaylist SamplePlaylist(IList<FullPlaylist> playlists)
    {
        Console.WriteLine($"Total Playlists in your Account: {playlists.Count}");
        // var playlist = playlists[0];
        FullPlaylist? playlist = null;
        foreach (var p in playlists)
        {
            if (p.Name == PlaylistName)
            {
                playlist = p;
                break;
            }
        }
        Debug.Assert(playlist is not null);
        Debug.Assert(playlist.Tracks is not null);
        return playlist;
    }

    private static async Task<IList<PlaylistTrack<IPlayableItem>>> FetchPlaylistTracks(
        SpotifyClient client,
        string playlistId
    )
    {
        var tracks =
            await client.PaginateAll(await client.Playlists.GetItems(playlistId))
            ?? throw new InvalidOperationException("Playlist is empty");
        return tracks;
    }

    private static List<FullTrack> FilterTracks(
        string searchTerm,
        IList<PlaylistTrack<IPlayableItem>> tracks
    )
    {
        return tracks
            .Select(t => t.Track)
            .OfType<FullTrack>()
            .Where(t => t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static async Task PlayTrack(SpotifyClient client, FullTrack track)
    {
        var playbackRequest = new PlayerResumePlaybackRequest
        {
            Uris = [track.Uri],
            DeviceId = SampleDeviceId,
        };
        await client.Player.ResumePlayback(playbackRequest);
        Console.WriteLine(
            $"Now playing: {track.Name} by {string.Join(", ", track.Artists.Select(a => a.Name))}"
        );
    }

    public static async Task<Device?> ActiveDevice(SpotifyClient client)
    {
        var devices = await client.Player.GetAvailableDevices();
        return devices.Devices.FirstOrDefault(d => d.IsActive);
    }

    public static async Task PlaylistCreateExample(AppConfig config, int year)
    {
        var auth = new SpotifyAuthenticator(
            config.ClientId,
            config.CredentialsPath,
            config.CallbackUrl,
            config.Port
        );
        SampleDeviceId = config.SampleDeviceId;
        PlaylistName = config.PlaylistName;

        await auth.CreateClient();
        var client = auth.Client;

        var allPlaylists = await FetchAllPlaylists(client);
        var playlist = SamplePlaylist(allPlaylists);
        Debug.Assert(playlist.Id is not null);
        var newPlaylist = await CreateOrFetchPlaylist(client, allPlaylists, year.ToString());
        Debug.Assert(newPlaylist.Id is not null);

        try
        {
            var tracks = (await FetchPlaylistTracks(client, playlist.Id))
                .Select(t => t.Track)
                .OfType<FullTrack>()
                .ToList();
            var tracksToAdd = new List<string>();
            foreach (var track in tracks)
            {
                var releaseDate = track.Album.ReleaseDate;
                if (releaseDate is null)
                {
                    continue;
                }
                if (int.Parse(releaseDate.Split('-')[0]) == year)
                {
                    if (tracksToAdd.Count > 99)
                    {
                        await client.Playlists.AddItems(
                            newPlaylist.Id,
                            new PlaylistAddItemsRequest(tracksToAdd)
                        );
                        Console.WriteLine("Added 100 tracks to playlist");
                        tracksToAdd.Clear();
                        Console.WriteLine("Reset tracksToAdd");
                        await Task.Delay(5000);
                        tracksToAdd.Add(track.Uri);
                    }
                    else
                    {
                        tracksToAdd.Add(track.Uri);
                    }
                }
            }
            Console.WriteLine("Finished loop with tracksToAdd count: " + tracksToAdd.Count);
            if (tracksToAdd.Count > 0)
            {
                await client.Playlists.AddItems(
                    newPlaylist.Id,
                    new PlaylistAddItemsRequest(tracksToAdd)
                );
            }
        }
        catch (APIException ex)
        {
            Console.WriteLine($"Error playing track: {ex.Message}");
        }
    }

    private static async Task<FullPlaylist> CreateOrFetchPlaylist(
        SpotifyClient client,
        IList<FullPlaylist> playlists,
        string newPlaylist
    )
    {
        foreach (var p in playlists)
        {
            if (p.Name == newPlaylist)
            {
                return p;
            }
        }
        var playlistCreateRequest = new PlaylistCreateRequest(newPlaylist);
        var me = await client.UserProfile.Current();
        return await client.Playlists.Create(me.Id, playlistCreateRequest);
    }
}

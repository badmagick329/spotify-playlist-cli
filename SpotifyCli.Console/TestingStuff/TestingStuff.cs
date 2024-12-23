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

        FullPlaylist playlist = await SamplePlaylist(client);
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

    private static async Task<FullPlaylist> SamplePlaylist(SpotifyClient client)
    {
        var playlists = await client.PaginateAll(
            await client.Playlists.CurrentUsers().ConfigureAwait(false)
        );
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
}

using SpotifyAPI.Web;
using SpotifyCli.Application;
using SpotifyCli.Core;
using SpotifyCli.Infrastructure.Mappers;

namespace SpotifyCli.Infrastructure;

class Client : IClient
{
    private readonly SpotifyAuthenticator _spotifyAuthenticator;
    private SpotifyClient SpotifyClient =>
        _spotifyAuthenticator.Client ?? throw new NullReferenceException("Client not initialized");
    private PrivateUser User { get; set; }

    public Client(AppConfig config)
    {
        _spotifyAuthenticator = new SpotifyAuthenticator(
            AppConfig.ClientId,
            config.CredentialsPath,
            config.CallbackUrl,
            config.Port
        );
    }

    public async Task Initialize()
    {
        await _spotifyAuthenticator.CreateClient();
        User = await SpotifyClient.UserProfile.Current();
    }

    public async Task<List<Playlist>> FetchAllPlaylists()
    {
        var fullPlaylists = await FetchAllFullPlaylists();
        return fullPlaylists
            .Select(FullPlaylistMapper.FromFullPlaylist)
            .OfType<Playlist>()
            .ToList();
    }

    private async Task<IList<FullPlaylist>> FetchAllFullPlaylists() =>
        await SpotifyClient.PaginateAll(
            await SpotifyClient.Playlists.CurrentUsers().ConfigureAwait(false)
        );

    public async Task CreateSpotifyPlaylist(FilteredPlaylist filteredPlaylist)
    {
        var createdPlaylist = await CreateOrFetchPlaylist(
            filteredPlaylist.SourcePlaylists,
            filteredPlaylist.Name
        );
        await SyncTracksWithSpotify(createdPlaylist, filteredPlaylist.Tracks);
    }

    // FIXME: Always creates new playlist instead of fetching old one
    private async Task<Playlist> CreateOrFetchPlaylist(List<Playlist> playlists, string newPlaylist)
    {
        foreach (var p in playlists)
        {
            if (p.Name == newPlaylist)
            {
                p.SavedTracks = await FetchPlaylistTracks(p.Id);
                return p;
            }
        }
        var playlistCreateRequest = new PlaylistCreateRequest(newPlaylist);
        var createdPlaylist =
            FullPlaylistMapper.FromFullPlaylist(
                await SpotifyClient.Playlists.Create(User.Id, playlistCreateRequest)
            ) ?? throw new NullReferenceException("Failed to create playlist");
        return createdPlaylist;
    }

    // TODO: Refactor?
    public async Task<FilteredPlaylist> FetchSourceTracksAndCreateFilteredPLaylist(
        List<Playlist> sourcePlaylists,
        string newName
    )
    {
        foreach (var playlist in sourcePlaylists)
        {
            playlist.SavedTracks = await FetchPlaylistTracks(playlist.Id);
        }
        return new FilteredPlaylist(sourcePlaylists, newName);
    }

    private async Task<List<Track>> FetchPlaylistTracks(string playlistId)
    {
        var tracks =
            await SpotifyClient.PaginateAll(await SpotifyClient.Playlists.GetItems(playlistId))
            ?? throw new InvalidOperationException("Playlist is empty");
        return tracks
            .Select(t => t.Track)
            .OfType<FullTrack>()
            .Where(t => t.Album.ReleaseDate is not null)
            .Select(FullTrackMapper.FromFullTrack)
            .Distinct()
            .ToList();
    }

    private async Task SyncTracksWithSpotify(Playlist playlist, List<Track> tracks)
    {
        var newUris = tracks.Select(t => t.Uri).ToList();
        var savedUris = playlist.SavedTracks?.Select(t => t.Uri).ToList() ?? [];

        var toAdd = newUris.ExceptBy(savedUris, u => u);
        var toRemove = savedUris.ExceptBy(newUris, u => u);

        await AddTracksToPlaylist(playlist.Id, toAdd);
        await RemoveTracksFromPlaylist(playlist.Id, toRemove);
    }

    private async Task AddTracksToPlaylist(string id, IEnumerable<string> toAdd)
    {
        try
        {
            var trackBatches = toAdd.Chunk(100);
            foreach (var batch in trackBatches)
            {
                await SpotifyClient.Playlists.AddItems(id, new PlaylistAddItemsRequest(batch));
            }
        }
        catch (APIException ex)
        {
            throw new InvalidOperationException("Failed to add tracks to playlist", ex);
        }
    }

    private async Task RemoveTracksFromPlaylist(string id, IEnumerable<string> toRemove)
    {
        try
        {
            var trackBatches = toRemove.Chunk(100);

            foreach (var batch in trackBatches)
            {
                await SpotifyClient.Playlists.RemoveItems(
                    id,
                    new PlaylistRemoveItemsRequest
                    {
                        Tracks = batch
                            .Select(uri => new PlaylistRemoveItemsRequest.Item { Uri = uri })
                            .ToList(),
                    }
                );
            }
        }
        catch (APIException ex)
        {
            throw new InvalidOperationException("Failed to remove tracks from playlist", ex);
        }
    }
}

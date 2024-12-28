using SpotifyCli.Core;

namespace SpotifyCli.Application;

class CreateFilteredPlaylist
{
    private readonly IClient _client;
    private readonly string _newName;
    private List<Playlist>? _allPlaylists = null;
    public List<Playlist> AllPlaylists
    {
        get
        {
            ArgumentNullException.ThrowIfNull(
                _allPlaylists,
                "AllPlaylists must be set before accessing"
            );
            return _allPlaylists;
        }
        private set { _allPlaylists = value; }
    }
    private List<Playlist>? _sourcePlaylists = null;
    public List<Playlist> SourcePlaylists
    {
        get
        {
            ArgumentNullException.ThrowIfNull(
                _sourcePlaylists,
                "SourcePlaylists must be set before accessing"
            );
            return _sourcePlaylists;
        }
        private set { _sourcePlaylists = value; }
    }
    public List<string> AllPlaylistIds => AllPlaylists.Select(p => p.Id).ToList();

    public CreateFilteredPlaylist(IClient client, string newName)
    {
        _client = client;
        _newName = newName;
    }

    public async Task Initialize()
    {
        AllPlaylists = await _client.FetchAllPlaylists();
    }

    public async Task SetSourcePlaylists(List<string> sourceIds)
    {
        SourcePlaylists = AllPlaylists.Where(p => sourceIds.Contains(p.Id)).ToList();

        foreach (var playlist in SourcePlaylists)
        {
            playlist.SavedTracks = await _client.FetchPlaylistTracks(playlist.Id);
        }
    }

    public async Task FilterByReleaseDate(ReleaseDate releaseDate)
    {
        var filteredPlaylist = new FilteredPlaylist(SourcePlaylists, _newName);
        filteredPlaylist.FilterByReleaseDate(releaseDate);
        await _client.CreateFilteredPlaylist(filteredPlaylist);
    }
}
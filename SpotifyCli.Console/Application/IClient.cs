using SpotifyCli.Core;

namespace SpotifyCli.Application;

interface IClient
{
    public Task Initialize();

    public Task<List<Playlist>> FetchAllPlaylists();
    public Task<FilteredPlaylist> FetchSourceTracksAndCreateFilteredPlaylist(
        List<Playlist> sourcePlaylists,
        string newName
    );
    public Task CreateSpotifyPlaylist(FilteredPlaylist filteredPlaylist);
}

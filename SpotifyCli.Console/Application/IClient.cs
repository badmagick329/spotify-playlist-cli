using SpotifyCli.Core;

namespace SpotifyCli.Application;

interface IClient
{
    public Task Initialize();

    public Task<List<Playlist>> FetchAllPlaylists();
    public Task CreateFilteredPlaylist(FilteredPlaylist filteredPlaylist);
    public Task<List<Track>> FetchPlaylistTracks(string playlistId);
}

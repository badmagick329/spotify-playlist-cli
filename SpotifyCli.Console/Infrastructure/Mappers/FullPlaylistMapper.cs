using SpotifyAPI.Web;
using SpotifyCli.Core;

namespace SpotifyCli.Infrastructure.Mappers;

public static class FullPlaylistMapper
{
    public static Playlist? FromFullPlaylist(FullPlaylist fullPlaylist) =>
        fullPlaylist switch
        {
            { Id: string id, Name: string name, Tracks.Total: int total } => new Playlist(
                id,
                name,
                total
            ),
            _ => null,
        };
}

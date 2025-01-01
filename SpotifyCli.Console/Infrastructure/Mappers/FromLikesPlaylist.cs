using SpotifyAPI.Web;
using SpotifyCli.Core;

namespace SpotifyCli.Infrastructure.Mappers;

public static class TracksToPlaylistMapper
{
    public static Playlist FromTracks(string id, string name, int trackCount) =>
        new(id, name, trackCount);
}

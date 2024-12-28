using SpotifyAPI.Web;
using SpotifyCli.Core;

namespace SpotifyCli.Infrastructure.Mappers;

public static class FullTrackMapper
{
    public static Track FromFullTrack(FullTrack fullTrack) =>
        new(
            fullTrack.Uri,
            fullTrack.Name,
            fullTrack.Artists.Select(artist => artist.Name).ToList(),
            fullTrack.Album.ReleaseDate
        );
}

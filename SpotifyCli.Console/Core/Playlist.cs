namespace SpotifyCli.Core;

public class Playlist
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int TrackCount { get; private set; }
    public List<Track>? SavedTracks { get; set; } = null;

    public Playlist(string id, string name, int trackCount)
    {
        Id = id;
        Name = name;
        TrackCount = trackCount;
    }

    public List<Track> FilterTracksByDateRange(ReleaseDate startDate, ReleaseDate endDate)
    {
        ArgumentNullException.ThrowIfNull(SavedTracks);

        return SavedTracks
            .Where(t =>
                t.ReleaseDate.IsAfterDateInclusive(startDate.Year, startDate.Month, startDate.Day)
                && t.ReleaseDate.IsBeforeDateInclusive(endDate.Year, endDate.Month, endDate.Day)
            )
            .ToList();
    }

    public IEnumerable<string> ArtistsInPlaylist()
    {
        ArgumentNullException.ThrowIfNull(SavedTracks);

        return SavedTracks.SelectMany(t => t.Artists).Distinct();
    }

    public List<Track> FilterTracksByArtists(List<string> artists)
    {
        ArgumentNullException.ThrowIfNull(SavedTracks);

        return SavedTracks.Where(t => t.Artists.Intersect(artists).Any()).ToList();
    }
}

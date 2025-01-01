namespace SpotifyCli.Core;

public class Playlist : BasePlaylist
{
    public Playlist(string id, string name, int trackCount)
        : base(id, name, trackCount) { }

    public override List<Track> FilterTracksByDateRange(ReleaseDate startDate, ReleaseDate endDate)
    {
        EnsureSavedTracksIsNotNull();

        return SavedTracks!
            .Where(t =>
                t.ReleaseDate.IsAfterDateInclusive(startDate.Year, startDate.Month, startDate.Day)
                && t.ReleaseDate.IsBeforeDateInclusive(endDate.Year, endDate.Month, endDate.Day)
            )
            .ToList();
    }

    public override IEnumerable<string> ArtistsInPlaylist()
    {
        EnsureSavedTracksIsNotNull();
        return SavedTracks!.SelectMany(t => t.Artists).Distinct();
    }

    public override List<Track> FilterTracksByArtists(List<string> artists)
    {
        EnsureSavedTracksIsNotNull();

        return SavedTracks!.Where(t => t.Artists.Intersect(artists).Any()).ToList();
    }

    // TODO: FetchTracks and FetchTracksFunc are terrible names and easily mixed up
    public override async Task FetchTracks()
    {
        ArgumentNullException.ThrowIfNull(
            FetchTracksFunc,
            "FetchTracksFunc is null. Ensure it has been set."
        );
        SavedTracks = await FetchTracksFunc();
    }

    private void EnsureSavedTracksIsNotNull()
    {
        ArgumentNullException.ThrowIfNull(
            SavedTracks,
            "SavedTracks is null. Ensure FetchTracks has been called."
        );
    }
}

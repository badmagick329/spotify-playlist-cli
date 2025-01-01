namespace SpotifyCli.Core;

public abstract class BasePlaylist
{
    public string Id { get; protected set; }
    public string Name { get; protected set; }
    public int TrackCount { get; protected set; }

    public List<Track>? SavedTracks { get; protected set; } = null;
    public Func<Task<List<Track>>>? FetchTracksFunc { get; set; } = null;

    public BasePlaylist(string id, string name, int trackCount)
    {
        Id = id;
        Name = name;
        TrackCount = trackCount;
    }

    public abstract List<Track> FilterTracksByDateRange(ReleaseDate startDate, ReleaseDate endDate);

    public abstract IEnumerable<string> ArtistsInPlaylist();

    public abstract List<Track> FilterTracksByArtists(List<string> artists);

    public abstract Task FetchTracks();
}

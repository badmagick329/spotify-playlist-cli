using System.Diagnostics;

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

    public List<Track> FilterTracksByDate(int year, int? month = null, int? day = null)
    {
        ArgumentNullException.ThrowIfNull(SavedTracks);
        if (day is not null)
        {
            Debug.Assert(month is not null, "day can only be specified if month is specified");
            Debug.Assert(day >= 1 && day <= 31, "day must be between 1 and 31");
        }
        if (month is not null)
        {
            Debug.Assert(month >= 1 && month <= 12, "month must be between 1 and 12");
        }

        return SavedTracks.Where(t => t.IsAtDate(year, month, day)).ToList();
    }
}
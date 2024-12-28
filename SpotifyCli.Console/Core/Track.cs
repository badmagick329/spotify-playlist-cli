using System.Diagnostics;

namespace SpotifyCli.Core;

public class Track
{
    public string Uri { get; init; }
    public string Name { get; init; }
    public List<string> Artists { get; init; }
    public ReleaseDate ReleaseDate { get; init; }

    public Track(string uri, string name, List<string> artists, string releaseDate)
    {
        Uri = uri;
        Name = name;
        Artists = artists;
        try
        {
            ReleaseDate = new ReleaseDate(releaseDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing release date for track {name} - {releaseDate}");
            throw new Exception(ex.Message);
        }
    }

    public bool IsAfterDateInclusive(int year, int? month = null, int? day = null) =>
        ReleaseDate.IsAfterDateInclusive(year, month, day);

    public bool IsBeforeDateInclusive(int year, int? month = null, int? day = null) =>
        ReleaseDate.IsBeforeDateInclusive(year, month, day);

    public bool IsAtDate(int year, int? month = null, int? day = null)
    {
        if (day is not null)
        {
            Debug.Assert(month is not null, "day can only be specified if month is specified");
            Debug.Assert(day >= 1 && day <= 31, "day must be between 1 and 31");
        }
        if (month is not null)
        {
            Debug.Assert(month >= 1 && month <= 12, "month must be between 1 and 12");
        }
        var result = ReleaseDate.IsAtDate(year, month, day);

        return ReleaseDate.IsAtDate(year, month, day);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Track track)
        {
            return false;
        }

        return Uri == track.Uri;
    }

    public override int GetHashCode() => Uri.GetHashCode();

    public override string ToString()
    {
        var artists = string.Join(", ", Artists);
        return $"{Name} by {artists} ({ReleaseDate})";
    }
}

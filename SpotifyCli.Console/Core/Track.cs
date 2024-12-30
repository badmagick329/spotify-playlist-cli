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

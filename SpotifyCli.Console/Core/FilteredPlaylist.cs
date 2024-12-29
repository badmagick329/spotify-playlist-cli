namespace SpotifyCli.Core;

class FilteredPlaylist
{
    public List<Playlist> SourcePlaylists { get; private set; }
    public List<Track> Tracks { get; private set; } = [];
    public string Name { get; init; }

    public FilteredPlaylist(List<Playlist> sourcePlaylists, string name)
    {
        SourcePlaylists = sourcePlaylists;
        Name = name;
    }

    public void FilterByReleaseDateRange(ReleaseDate startDate, ReleaseDate endDate)
    {
        if (Tracks.Count == 0)
        {
            Tracks = SourcePlaylists
                .SelectMany(sp => sp.FilterTracksByDateRange(startDate, endDate))
                .ToList();
            return;
        }

        var newTracks = SourcePlaylists
            .SelectMany(sp => sp.FilterTracksByDateRange(startDate, endDate))
            .ToList();
        Tracks = Tracks.Intersect(newTracks).ToList();
    }

    public void FilterByArtists(List<string> artists)
    {
        if (Tracks.Count == 0)
        {
            Tracks = SourcePlaylists.SelectMany(sp => sp.FilterTracksByArtists(artists)).ToList();
            return;
        }

        var newTracks = SourcePlaylists
            .SelectMany(sp => sp.FilterTracksByArtists(artists))
            .ToList();
        Tracks = Tracks.Intersect(newTracks).ToList();
    }
}

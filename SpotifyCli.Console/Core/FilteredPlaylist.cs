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

    public void FilterByReleaseDate(ReleaseDate releaseDate) =>
        Tracks = SourcePlaylists
            .SelectMany(sp =>
                sp.FilterTracksByDate(releaseDate.Year, releaseDate.Month, releaseDate.Day)
            )
            .ToList();

    public void FilterByReleaseDateRange(ReleaseDate startDate, ReleaseDate endDate) =>
        Tracks = SourcePlaylists
            .SelectMany(sp => sp.FilterTracksByDateRange(startDate, endDate))
            .ToList();

    public void FilterByArtists(List<string> artists)
    {
        Tracks = SourcePlaylists.SelectMany(sp => sp.FilterTracksByArtists(artists)).ToList();
    }
}

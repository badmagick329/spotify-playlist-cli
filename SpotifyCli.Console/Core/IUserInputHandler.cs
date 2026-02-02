namespace SpotifyCli.Core;

public interface IUserInputHandler
{
    public string AskNewPlaylistName();
    public List<string> AskSelectedPlayistIds(List<Playlist> playlists);
    public (ReleaseDate startDate, ReleaseDate endDate) AskReleaseDate();
    public FilterType AskFilterType();
    public List<FilterType> AskFilterTypes();
    public List<string> AskArtists(List<string> artists);
    public AppMode AskAppMode();
}

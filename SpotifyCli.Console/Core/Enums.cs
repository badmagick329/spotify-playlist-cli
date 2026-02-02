namespace SpotifyCli.Core;

public enum NotifyEvent
{
    NoPlaylistsSelected,
    FetchingTracks,
    NoTracksMatchedCriteria,
    CreatingPlaylist,
    CreatedPlaylist,
}

public enum FilterType
{
    DateRange,
    Artists,
}

public enum AppMode
{
    CreatePlaylist,
    ListPlaylistSongs,
}

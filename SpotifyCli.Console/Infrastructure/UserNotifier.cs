using Spectre.Console;
using SpotifyCli.Core;

namespace SpotifyCli.Infrastructure;

public class UserNotifier : IUserNotifier
{
    public void Notify(NotifyEvent notifyEvent)
    {
        string message;
        switch (notifyEvent)
        {
            case NotifyEvent.NoPlaylistsSelected:
                message = "[yellow]No playlists selected. Operation cancelled.[/]";
                break;
            case NotifyEvent.FetchingTracks:
                message = "[yellow]Fetching tracks from playlists...[/]";
                break;
            case NotifyEvent.NoTracksMatchedCriteria:
                message =
                    "[yellow]No tracks matched the specified criteria. Operation cancelled.[/]";
                break;
            case NotifyEvent.CreatingPlaylist:
                message = "[yellow]Creating filtered playlist...[/]";
                break;
            case NotifyEvent.CreatedPlaylist:
                message = "[green]Filtered playlist created successfully![/]";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(notifyEvent), notifyEvent, null);
        }

        AnsiConsole.MarkupLine(message);
    }
}

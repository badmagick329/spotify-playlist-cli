using Spectre.Console;
using SpotifyCli.Core;

namespace SpotifyCli.Presentation;

static class CreateFilteredPlaylistInputHandler
{
    const int PageSize = 15;

    public static string AskPlaylistName()
    {
        string newName = "";
        while (newName == "")
        {
            newName = AnsiConsole.Ask<string>("Enter the name of the new playlist:").Trim();
        }
        return newName;
    }

    public static List<string> AskSelectedPlayistIds(List<Playlist> playlists)
    {
        var numberToId = NumberToIdMapper(playlists);
        var selectionList = CreatePlaylistSelectionList(playlists);
        var selectedPlaylists = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[green]Select playlists to process[/]")
                .Required()
                .PageSize(PageSize)
                .MoreChoicesText("[grey]Move up and down to reveal more playlists[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to select, [green]<enter>[/] to accept, [red]<esc>[/] to cancel)[/]"
                )
                .AddChoices(selectionList)
        );
        var selectedNumbers = selectedPlaylists
            .Select(p => int.Parse(p.Split(' ')[0]) - 1)
            .ToList();
        return selectedNumbers.Select(n => numberToId[n]).ToList();
    }

    private static Dictionary<int, string> NumberToIdMapper(List<Playlist> playlists)
    {
        var numberToId = new Dictionary<int, string>();
        for (int i = 0; i < playlists.Count; i++)
        {
            numberToId[i] = playlists[i].Id;
        }
        return numberToId;
    }

    private static List<string> CreatePlaylistSelectionList(List<Playlist> playlists)
    {
        var selectionList = new List<string>();
        for (int i = 0; i < playlists.Count; i++)
        {
            selectionList.Add($"{i + 1} - {playlists[i].Name} ({playlists[i].TrackCount})");
        }
        return selectionList;
    }

    public static (ReleaseDate startDate, ReleaseDate endDate) AskReleaseDate()
    {
        ReleaseDate startDate;
        ReleaseDate endDate;
        while (true)
        {
            var startDateString = AnsiConsole.Ask<string>("Enter start of date range:");
            startDate = new ReleaseDate(startDateString);
            if (startDate.IsValid())
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]End date is not valid[/]");
                AnsiConsole.MarkupLine("Accepted formats are: yyyy, yyyy-mm, yyyy-mm-dd");
            }
        }
        while (true)
        {
            var endDateString = AnsiConsole.Ask<string>("Enter end of date range:");
            endDate = new ReleaseDate(endDateString);
            if (endDate.IsValid())
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]End date is not valid[/]");
                AnsiConsole.MarkupLine("Accepted formats are: yyyy, yyyy-mm, yyyy-mm-dd");
            }
        }
        return (startDate, endDate);
    }

    public static FilterType AskFilterType()
    {
        var filterChoices = new Dictionary<FilterType, string>
        {
            { FilterType.DateRange, "Filter by date range" },
            { FilterType.Artists, "Filter by specific artists" },
        };

        return AnsiConsole.Prompt(
            new SelectionPrompt<FilterType>()
                .Title("[green]Select filter type[/]")
                .PageSize(PageSize)
                .UseConverter(filter => filterChoices[filter])
                .MoreChoicesText("[grey]Move up and down to reveal more filter types[/]")
                .AddChoices(Enum.GetValues<FilterType>())
        );
    }

    public static List<FilterType> AskFilterTypes()
    {
        var filterChoices = new Dictionary<FilterType, string>
        {
            { FilterType.DateRange, "Filter by date range" },
            { FilterType.Artists, "Filter by specific artists" },
        };

        return AnsiConsole.Prompt(
            new MultiSelectionPrompt<FilterType>()
                .Title("[green]Select filter types[/]")
                .PageSize(PageSize)
                .MoreChoicesText("[grey]Move up and down to reveal more filter types[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to select, [green]<enter>[/] to accept, [red]<esc>[/] to cancel)[/]"
                )
                .UseConverter(filter => filterChoices[filter])
                .AddChoices(Enum.GetValues<FilterType>())
        );
    }

    public static List<string> AskArtists(List<string> artists)
    {
        return AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[green]Select artists to filter by[/]")
                .Required()
                .PageSize(PageSize)
                .MoreChoicesText("[grey]Move up and down to reveal more artists[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to select, [green]<enter>[/] to accept, [red]<esc>[/] to cancel)[/]"
                )
                .AddChoices(artists)
        );
    }
}

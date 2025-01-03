namespace SpotifyCli.Core;

public class ReleaseDate
{
    public string ReleaseDateAsString { get; init; }
    public int Year { get; init; }
    public int? Month { get; init; }
    public int? Day { get; init; }

    public ReleaseDate(int year, int? month = null, int? day = null)
    {
        Year = year;
        Month = month;
        Day = day;
        ReleaseDateAsString = $"{Year}";
        if (Month is not null)
        {
            ReleaseDateAsString += $"-{Month:D2}";
            if (Day is not null)
            {
                ReleaseDateAsString += $"-{Day:D2}";
            }
        }
    }

    public ReleaseDate(string releaseDate)
    {
        ReleaseDateAsString = releaseDate;
        Year = ReadYear(releaseDate);
        Month = ReadMonth(releaseDate);
        Day = ReadDay(releaseDate);
    }

    public bool IsAfterDateInclusive(int year, int? month = null, int? day = null) =>
        (month, day) switch
        {
            (int m, int d) => Year > year
                || (
                    Year == year
                    && (Month is null || Month > m || (Month == m && (Day is null || Day >= d)))
                ),

            (int m, null) => Year > year || (Year == year && (Month is null || Month >= m)),

            (null, _) => Year >= year,
        };

    public bool IsBeforeDateInclusive(int year, int? month = null, int? day = null) =>
        (month, day) switch
        {
            (int m, int d) => Year < year
                || (
                    Year == year
                    && (Month is null || Month < m || (Month == m && (Day is null || Day <= d)))
                ),

            (int m, null) => Year < year || (Year == year && (Month is null || Month <= m)),

            (null, _) => Year <= year,
        };

    public bool IsAtDate(int year, int? month = null, int? day = null) =>
        Year == year && (Month is null || Month == month) && (Day is null || Day == day);

    public bool IsValid()
    {
        var month = Month ?? 1;
        var day = Day ?? 1;
        try
        {
            _ = new DateTime(Year, month, day);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    private static int ReadYear(string releaseDate) =>
        releaseDate switch
        {
            null => throw new ArgumentNullException(nameof(releaseDate)),
            { Length: 4 } when int.TryParse(releaseDate, out int year) => year,
            { Length: > 4 }
                when releaseDate.Contains('-')
                    && int.TryParse(releaseDate.Split('-')[0], out int splitYear) => splitYear,
            _ => throw new ArgumentException(
                $"Invalid release date format: {releaseDate}. Expected YYYY, YYYY-MM or YYYY-MM-DD",
                nameof(releaseDate)
            ),
        };

    private static int? ReadMonth(string releaseDate) =>
        releaseDate switch
        {
            { Length: < 7 } => null,
            { Length: >= 7 }
                when releaseDate.Contains('-')
                    && int.TryParse(releaseDate.Split('-')[1], out int splitMonth) => splitMonth,
            _ => throw new ArgumentException(
                $"Invalid release date format: {releaseDate}. Expected YYYY, YYYY-MM or YYYY-MM-DD",
                nameof(releaseDate)
            ),
        };

    private static int? ReadDay(string releaseDate) =>
        releaseDate switch
        {
            { Length: < 10 } => null,
            { Length: 10 }
                when releaseDate.Contains('-')
                    && int.TryParse(releaseDate.Split('-')[2], out int splitDay) => splitDay,
            _ => throw new ArgumentException(
                $"Invalid release date format: {releaseDate}. Expected YYYY, YYYY-MM or YYYY-MM-DD",
                nameof(releaseDate)
            ),
        };

    public override string ToString() => ReleaseDateAsString;
}

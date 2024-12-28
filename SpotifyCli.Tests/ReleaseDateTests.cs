using SpotifyCli.Core;

namespace SpotifyCli.Tests;

public class ReleaseDateTests
{
    [Theory]
    [InlineData(2021, null, null, 2021, null, null)]
    [InlineData(2019, 12, null, 2019, 12, null)]
    [InlineData(2018, 3, 15, 2018, 3, 15)]
    public void GivenInts_WhenConstruced_ThenInitializationIsValid(
        int year,
        int? month,
        int? day,
        int expectedYear,
        int? expectedMonth,
        int? expectedDay
    )
    {
        var releaseDate = new ReleaseDate(year, month, day);

        Assert.Equal(expectedYear, releaseDate.Year);
        Assert.Equal(expectedMonth, releaseDate.Month);
        Assert.Equal(expectedDay, releaseDate.Day);
    }

    [Theory]
    [InlineData("2021-01", 2021, 1, null)]
    [InlineData("2019-12-15", 2019, 12, 15)]
    [InlineData("2018", 2018, null, null)]
    public void GivenString_WhenConstruced_ThenInitializationIsValid(
        string releaseDateAsString,
        int expectedYear,
        int? expectedMonth,
        int? expectedDay
    )
    {
        var releaseDate = new ReleaseDate(releaseDateAsString);

        Assert.Equal(expectedYear, releaseDate.Year);
        Assert.Equal(expectedMonth, releaseDate.Month);
        Assert.Equal(expectedDay, releaseDate.Day);
    }

    [Theory]
    [InlineData("2021", "2021-01-01", true)]
    [InlineData("2021", "2021-01", true)]
    [InlineData("2021", "2021", true)]
    [InlineData("2021", "2020-12-31", true)]
    [InlineData("2021", "2020-12", true)]
    [InlineData("2021", "2020", true)]
    [InlineData("2021", "2021-12-31", true)]
    [InlineData("2021", "2021-12", true)]
    [InlineData("2021", "2022-01-01", false)]
    [InlineData("2021", "2022-01", false)]
    [InlineData("2021", "2022", false)]
    public void GivenYearOnly_WhenBeforeComparison_ThenReturnExpectedResult(
        string year,
        string releaseDateAsString,
        bool expected
    )
    {
        var releaseDate = new ReleaseDate(releaseDateAsString);

        Assert.Equal(expected, releaseDate.IsBeforeDateInclusive(int.Parse(year)));
    }

    [Theory]
    [InlineData("2021", "2021-01-01", true)]
    [InlineData("2021", "2021-01", true)]
    [InlineData("2021", "2021", true)]
    [InlineData("2021", "2020-12-31", false)]
    [InlineData("2021", "2020-12", false)]
    [InlineData("2021", "2020", false)]
    [InlineData("2021", "2021-12-31", true)]
    [InlineData("2021", "2021-12", true)]
    [InlineData("2021", "2022-01-01", true)]
    [InlineData("2021", "2022-01", true)]
    [InlineData("2021", "2022", true)]
    public void GivenYearOnly_WhenAfterComparison_ThenReturnExpectedResult(
        string year,
        string releaseDateAsString,
        bool expected
    )
    {
        var releaseDate = new ReleaseDate(releaseDateAsString);

        Assert.Equal(expected, releaseDate.IsAfterDateInclusive(int.Parse(year)));
    }

    [Theory]
    [InlineData("2021-01", "2021-01-01", true)]
    [InlineData("2021-01", "2021-01", true)]
    [InlineData("2021-01", "2021", true)]
    [InlineData("2021-01", "2020-12-31", false)]
    [InlineData("2021-01", "2020-12", false)]
    [InlineData("2021-01", "2020", false)]
    [InlineData("2021-01", "2021-12-31", true)]
    [InlineData("2021-01", "2021-12", true)]
    [InlineData("2021-01", "2022-01-01", true)]
    [InlineData("2021-01", "2022-01", true)]
    [InlineData("2021-01", "2022", true)]
    public void GivenYearAndMonth_WhenAfterComparison_ThenReturnExpectedResult(
        string yearMonth,
        string releaseDateAsString,
        bool expected
    )
    {
        var releaseDate = new ReleaseDate(releaseDateAsString);

        var year = int.Parse(yearMonth.Split('-')[0]);
        var month = int.Parse(yearMonth.Split('-')[1]);

        Assert.Equal(expected, releaseDate.IsAfterDateInclusive(year, month));
    }

    [Theory]
    [InlineData("2021-01", "2021-01-01", true)]
    [InlineData("2021-01", "2021-01", true)]
    [InlineData("2021-01", "2021", true)]
    [InlineData("2021-01", "2020-12-31", true)]
    [InlineData("2021-01", "2020-12", true)]
    [InlineData("2021-01", "2020", true)]
    [InlineData("2021-01", "2021-12-31", false)]
    [InlineData("2021-01", "2021-12", false)]
    [InlineData("2021-01", "2022-01-01", false)]
    [InlineData("2021-01", "2022-01", false)]
    [InlineData("2021-01", "2022", false)]
    public void GivenYearAndMonth_WhenBeforeComparison_ThenReturnExpectedResult(
        string yearMonth,
        string releaseDateAsString,
        bool expected
    )
    {
        var releaseDate = new ReleaseDate(releaseDateAsString);

        var year = int.Parse(yearMonth.Split('-')[0]);
        var month = int.Parse(yearMonth.Split('-')[1]);

        Assert.Equal(expected, releaseDate.IsBeforeDateInclusive(year, month));
    }
}

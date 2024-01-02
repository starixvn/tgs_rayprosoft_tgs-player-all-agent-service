using TGS.Core.Infrastructures.Extensions;

namespace Tgs.Player.All.Agent.UnitTest.Extensions;

public class DateTimeOffsetTests
{
    [Test]
    public void Create_Player_Bucket_Event()
    {
        var now = DateTimeOffset.Parse("2024-01-01 03:00:00+08");
        var previousDay = now.AddDays(-1);
        var reportDate = new DateTimeOffset(previousDay.Year,
            previousDay.Month,
            previousDay.Day,
            0,
            0,
            0,
            previousDay.Offset);

        var weekFrom = reportDate.AddDays(-7).GetMonday();

        var weekTo = reportDate.AddDays(-7).GetSunday();

        Assert.That(DateTimeOffset.Parse("2023-12-25 00:00:00+8"), Is.EqualTo(weekFrom));
        Assert.That(DateTimeOffset.Parse("2023-12-31 00:00:00+8"), Is.EqualTo(weekTo));
    }
}
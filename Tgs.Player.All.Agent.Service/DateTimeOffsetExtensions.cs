namespace Tgs.Player.All.Agent.Service;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset GetFirstDayOfMonth(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.AddDays(1 - dateTimeOffset.Day);
    }

    public static DateTimeOffset GetLastDayOfMonth(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.GetFirstDayOfMonth().AddMonths(1).AddDays(-1);
    }

    public static DateTimeOffset GetMonday(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.Subtract(
            new TimeSpan(((int)dateTimeOffset.DayOfWeek == 0 ? 7 : (int)dateTimeOffset.DayOfWeek) - 1, 0, 0, 0));
    }

    public static DateTimeOffset GetSunday(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.Subtract(
            new TimeSpan(((int)dateTimeOffset.DayOfWeek == 0 ? 7 : (int)dateTimeOffset.DayOfWeek) - 7, 0, 0, 0));
    }

    public static DateTime GetFirstDayOfMonth(this DateTime dateTime)
    {
        return dateTime.AddDays(1 - dateTime.Day);
    }

    public static DateTime GetLastDayOfMonth(this DateTime dateTime)
    {
        return dateTime.GetFirstDayOfMonth().AddMonths(1).AddDays(-1);
    }

    public static DateTime GetMonday(this DateTime dateTime)
    {
        return dateTime.Subtract(
            new TimeSpan(((int)dateTime.DayOfWeek == 0 ? 7 : (int)dateTime.DayOfWeek) - 1, 0, 0, 0));
    }

    public static DateTime GetSunday(this DateTime dateTime)
    {
        return dateTime.Subtract(
            new TimeSpan(((int)dateTime.DayOfWeek == 0 ? 7 : (int)dateTime.DayOfWeek) - 7, 0, 0, 0));
    }
}
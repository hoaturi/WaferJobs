namespace JobBoard.Common.Extensions;

public static class DateTimeExtensions
{
    public static string ToRelativeTimeString(this DateTime date)
    {
        var now = DateTime.UtcNow;
        var timeDiff = now - date;

        if (timeDiff.TotalSeconds < 0)
            return "Just now";

        if (timeDiff.TotalDays > 365)
            return FormatTimeSpan(timeDiff.Days / 365, "year");
        if (timeDiff.TotalDays > 30)
            return FormatTimeSpan(timeDiff.Days / 30, "month");
        if (timeDiff.TotalDays >= 7)
            return FormatTimeSpan(timeDiff.Days / 7, "week");
        if (timeDiff.TotalDays >= 1)
            return FormatTimeSpan(timeDiff.Days, "day");
        if (timeDiff.TotalHours >= 1)
            return FormatTimeSpan(timeDiff.Hours, "hour");
        if (timeDiff.TotalMinutes >= 1)
            return FormatTimeSpan(timeDiff.Minutes, "minute");
        if (timeDiff.TotalSeconds >= 5)
            return FormatTimeSpan(timeDiff.Seconds, "second");

        return "Just now";
    }

    private static string FormatTimeSpan(int value, string unit)
    {
        var pluralSuffix = value == 1 ? "" : "s";
        return $"{value} {unit}{pluralSuffix} ago";
    }
}
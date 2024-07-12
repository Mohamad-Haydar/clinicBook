namespace api.Helper;

public static class CalcTime
{
    public static TimeOnly GetTime(DateTimeOffset date)
    {
        return new TimeOnly(date.Hour, date.Minute);
    }

    public static int GetlocalMinutes(DateTimeOffset date)
    {
        return date.Minute;
    }
    
}
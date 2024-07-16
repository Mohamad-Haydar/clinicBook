namespace api.Helper;

public static class CalcTime
{
    public static TimeSpan GetTime(TimeSpan date)
    {
        return new TimeSpan(date.Hours, date.Minutes, 0);
    }

    public static int GetlocalMinutes(TimeSpan date)
    {
        return date.Minutes;
    }
    
}
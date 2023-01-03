namespace Jem.CommonLibrary22;

public static class Clock
{
    private static DateTime? FrozenNow;

    public static DateTime Now => FrozenNow ?? DateTime.Now;

    public static DateTime NowUtc => Now.ToUniversalTime();

    public static string ToStringYMDHM() => Now.ToString("yyyy/mm/dd hh:nn");

    public static void FreezeAt(DateTime now)
    {
        FrozenNow = now;
    }

    public static void Freeze()
    {
        Clock.FrozenNow = DateTime.Now;
    }

    public static void FreezeAt(int year, int month, int day)
    {
        FreezeAt(new DateTime(year, month, day));
    }
}
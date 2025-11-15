using System;

namespace door.Droid
{
    public static class TimeUtils
    {
        public static string TimeString
        {
            get
            {
                return DateTime.UtcNow.ToString("hh:mm:ss.fff");
            }
        }

        public static bool EarlierOrSameAs(this DateTime time, DateTime other)
        {
            return time.CompareTo(other) <= 0;
        }

    }
}

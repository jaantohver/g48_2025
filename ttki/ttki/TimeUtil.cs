using System;

namespace ttki
{
	public static class TimeUtil
	{
		public static string GetHourMinuteSecondString(TimeSpan ts)
		{
			int sec = (int)ts.TotalSeconds;

			int hours = sec / 3600;
			int minutes = (sec % 3600) / 60;
			int seconds = sec % 60;

			return hours.ToString().PadLeft(2, '0') + ":" +
				minutes.ToString().PadLeft(2, '0') + ":" +
				seconds.ToString().PadLeft(2, '0');
        }
	}
}

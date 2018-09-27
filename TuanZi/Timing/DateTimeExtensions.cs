using System;
using System.Linq;


namespace TuanZi
{
    public static class DateTimeExtensions
    {
        public static bool IsWeekend(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weeks.Contains(dateTime.DayOfWeek);
        }

        public static bool IsWeekday(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            return weeks.Contains(dateTime.DayOfWeek);
        }

        public static string ToUniqueString(this DateTime dateTime, bool milsec = false)
        {
            int sedonds = dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second;
            string value = string.Format("{0}{1}{2}", dateTime.ToString("yy"), dateTime.DayOfYear, sedonds);
            return milsec ? value + dateTime.ToString("fff") : value;
        }

        public static string ToJsGetTime(this DateTime dateTime)
        {
            DateTime utc = dateTime.ToUniversalTime();
            return ((long)utc.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
        }

        public static readonly DateTime BeginOfEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a nullable date/time value to UTC.
        /// </summary>
        /// <param name="dateTime">The nullable date/time</param>
        /// <returns>The nullable date/time in UTC</returns>
        public static DateTime? ToUniversalTime(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToUniversalTime() : (DateTime?)null;
        }

        /// <summary>
        /// Converts a nullable UTC date/time value to local time.
        /// </summary>
        /// <param name="dateTime">The nullable UTC date/time</param>
        /// <returns>The nullable UTC date/time as local time</returns>
        public static DateTime? ToLocalTime(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToLocalTime() : (DateTime?)null;
        }


        /// <summary>
        /// Returns a date that is rounded to the next even hour above the given
        /// date.
        /// <p>
        /// For example an input date with a time of 08:13:54 would result in a date
        /// with the time of 09:00:00. If the date's time is in the 23rd hour, the
        /// date's 'day' will be promoted, and the time will be set to 00:00:00.
        /// </p>
        /// </summary>
        /// <param name="dateTime">the Date to round, if <see langword="null" /> the current time will
        /// be used</param>
        /// <returns>the new rounded date</returns>
        public static DateTime GetEvenHourDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                dateTime = DateTime.UtcNow;
            }
            DateTime d = dateTime.Value.AddHours(1);
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0);
        }

        /// <summary>
        /// Returns a date that is rounded to the next even minute above the given
        /// date.
        /// <p>
        /// For example an input date with a time of 08:13:54 would result in a date
        /// with the time of 08:14:00. If the date's time is in the 59th minute,
        /// then the hour (and possibly the day) will be promoted.
        /// </p>
        /// </summary>
        /// <param name="dateTime">The Date to round, if <see langword="null" /> the current time will  be used</param>
        /// <returns>The new rounded date</returns>
        public static DateTime GetEvenMinuteDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                dateTime = DateTime.UtcNow;
            }

            DateTime d = dateTime.Value;
            d = d.AddMinutes(1);
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
        }

        /// <summary>
        /// Returns a date that is rounded to the previous even minute below the
        /// given date.
        /// <p>
        /// For example an input date with a time of 08:13:54 would result in a date
        /// with the time of 08:13:00.
        /// </p>
        /// </summary>
        /// <param name="dateTime">the Date to round, if <see langword="null" /> the current time will
        /// be used</param>
        /// <returns>the new rounded date</returns>
        public static DateTime GetEvenMinuteDateBefore(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                dateTime = DateTime.UtcNow;
            }

            DateTime d = dateTime.Value;
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
        }

        public static long ToJavaScriptTicks(this DateTime dateTime)
        {
            DateTimeOffset utcDateTime = dateTime.ToUniversalTime();
            long javaScriptTicks = (utcDateTime.Ticks - BeginOfEpoch.Ticks) / (long)10000;
            return javaScriptTicks;
        }

        /// <summary>
        /// Get the first day of the month for
        /// any full date submitted
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {
            DateTime dtFrom = date;
            dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));
            return dtFrom;
        }

        /// <summary>
        /// Get the last day of the month for any
        /// full date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            DateTime dtTo = date;
            dtTo = dtTo.AddMonths(1);
            dtTo = dtTo.AddDays(-(dtTo.Day));
            return dtTo;
        }

        public static DateTime ToEndOfTheDay(this DateTime dt)
        {
            if (dt != null)
                return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
            return dt;
        }

        public static DateTime? ToEndOfTheDay(this DateTime? dt)
        {
            return (dt.HasValue ? dt.Value.ToEndOfTheDay() : dt);
        }

        public static DateTime ToStartOfTheDay(this DateTime dt)
        {
            if (dt != null)
                return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            return dt;
        }

        public static DateTime? ToStartOfTheDay(this DateTime? dt)
        {
            return (dt.HasValue ? dt.Value.ToStartOfTheDay() : dt);
        }

        /// <summary>Epoch time. Number of seconds since midnight (UTC) on 1st January 1970.</summary>
        public static long ToUnixTime(this DateTime date)
        {
            return Convert.ToInt64((date.ToUniversalTime() - BeginOfEpoch).TotalSeconds);
        }

        /// <summary>UTC date based on number of seconds since midnight (UTC) on 1st January 1970.</summary>
        public static DateTime FromUnixTime(this long unixTime)
        {
            return BeginOfEpoch.AddSeconds(unixTime);
        }

        public static string ToRelativeTime(this DateTime dateTime)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }


        public static DateTime AddWeekDays(DateTime date, int days)
        {
            if (days == 0) return date;
            int i = 0;
            while (i < days)
            {
                if (!(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)) i++;
                date = date.AddDays(1);
            }
            return date;
        }
    }
}
using System;


namespace TuanZi.Timing
{
    [Serializable]
    public class DateTimeRange
    {
        public DateTimeRange()
            : this(DateTime.MinValue, DateTime.MaxValue)
        { }

        public DateTimeRange(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public static DateTimeRange Yesterday
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(now.Date.AddDays(-1), now.Date.AddMilliseconds(-1));
            }
        }

        public static DateTimeRange Today
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(now.Date.Date, now.Date.AddDays(1).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange Tomorrow
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(now.Date.AddDays(1), now.Date.AddDays(2).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange LastWeek
        {
            get
            {
                DateTime now = DateTime.Now;
                DayOfWeek[] weeks =
                {
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                };
                int index = Array.IndexOf(weeks, now.DayOfWeek);
                return new DateTimeRange(now.Date.AddDays(-index - 7), now.Date.AddDays(-index).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange ThisWeek
        {
            get
            {
                DateTime now = DateTime.Now;
                DayOfWeek[] weeks =
                {
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                };
                int index = Array.IndexOf(weeks, now.DayOfWeek);
                return new DateTimeRange(now.Date.AddDays(-index), now.Date.AddDays(7 - index).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange NextWeek
        {
            get
            {
                DateTime now = DateTime.Now;
                DayOfWeek[] weeks =
                {
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                };
                int index = Array.IndexOf(weeks, now.DayOfWeek);
                return new DateTimeRange(now.Date.AddDays(-index + 7), now.Date.AddDays(14 - index).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange LastMonth
        {
            get
            {
                DateTime now = DateTime.Now;
                DateTime startTime = now.Date.AddDays(-now.Day + 1).AddMonths(-1);
                DateTime endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new DateTimeRange(startTime, endTime);
            }
        }

        public static DateTimeRange ThisMonth
        {
            get
            {
                DateTime now = DateTime.Now;
                DateTime startTime = now.Date.AddDays(-now.Day + 1);
                DateTime endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new DateTimeRange(startTime, endTime);
            }
        }

        public static DateTimeRange NextMonth
        {
            get
            {
                DateTime now = DateTime.Now;
                DateTime startTime = now.Date.AddDays(-now.Day + 1).AddMonths(1);
                DateTime endTime = startTime.AddMonths(1).AddMilliseconds(-1);
                return new DateTimeRange(startTime, endTime);
            }
        }

        public static DateTimeRange LastYear
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(new DateTime(now.Year - 1, 1, 1), new DateTime(now.Year, 1, 1).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange ThisYear
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(new DateTime(now.Year, 1, 1), new DateTime(now.Year + 1, 1, 1).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange NextYear
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(new DateTime(now.Year + 1, 1, 1), new DateTime(now.Year + 2, 1, 1).AddMilliseconds(-1));
            }
        }

        public static DateTimeRange Last30Days
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(now.AddDays(-30), now);
            }
        }

        public static DateTimeRange Last30DaysExceptToday
        {
            get
            {
                var now = DateTime.Now;
                return new DateTimeRange(now.Date.AddDays(-30), now.Date.AddMilliseconds(-1));
            }
        }

        public static DateTimeRange Last7Days
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTimeRange(now.AddDays(-7), now);
            }
        }

        public static DateTimeRange Last7DaysExceptToday
        {
            get
            {
                var now = DateTime.Now;
                return new DateTimeRange(now.Date.AddDays(-7), now.Date.AddMilliseconds(-1));
            }
        }

        public override string ToString()
        {
            return string.Format("[{0} - {1}]", StartTime, EndTime);
        }
    }
}


using System;
namespace TuanZi.Logging.RollingFile.Internal
{
    public class LogMessageEntry
    {
        public DateTimeOffset Timestamp { get; set; }

        public string Message { get; set; }
    }
}
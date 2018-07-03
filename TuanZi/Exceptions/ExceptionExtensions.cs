
using System;
using System.Runtime.ExceptionServices;
using System.Text;


namespace TuanZi.Exceptions
{
    public static class ExceptionExtensions
    {
        public static string FormatMessage(this Exception e, bool isHideStackTrace = false)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            string appString = string.Empty;
            while (e != null)
            {
                if (count > 0)
                {
                    appString += "  ";
                }
                sb.AppendLine(string.Format("{0} Message: {1}", appString, e.Message));
                sb.AppendLine(string.Format("{0} Type: {1}", appString, e.GetType().FullName));
                sb.AppendLine(string.Format("{0} Method: {1}", appString, (e.TargetSite == null ? null : e.TargetSite.Name)));
                sb.AppendLine(string.Format("{0} Source: {1}", appString, e.Source));
                if (!isHideStackTrace && e.StackTrace != null)
                {
                    sb.AppendLine(string.Format("{0} StackTrace: {1}", appString, e.StackTrace));
                }
                if (e.InnerException != null)
                {
                    sb.AppendLine(string.Format("{0} InnerException: ", appString));
                    count++;
                }
                e = e.InnerException;
            }
            return sb.ToString();
        }

        public static void ReThrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}
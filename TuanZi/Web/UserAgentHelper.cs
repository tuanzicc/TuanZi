//TuanZi.Me


namespace TuanZi.Web
{
    public static class UserAgentHelper
    {
        public static string GetOperatingSystemName(string userAgent)
        {
            if (userAgent.Contains("NT 6.1"))
            {
                return "Windows 7";
            }
            if (userAgent.Contains("NT 6.0"))
            {
                return "Windows Vista/Server 2008";
            }
            if (userAgent.Contains("NT 5.2"))
            {
                return "Windows Server 2003";
            }
            if (userAgent.Contains("NT 5.1"))
            {
                return "Windows XP";
            }
            if (userAgent.Contains("NT 5"))
            {
                return "Windows 2000";
            }
            if (userAgent.Contains("Mac"))
            {
                return "Mac";
            }
            if (userAgent.Contains("Unix"))
            {
                return "UNIX";
            }
            if (userAgent.Contains("Linux"))
            {
                return "Linux";
            }
            if (userAgent.Contains("SunOS"))
            {
                return "SunOS";
            }
            return "Other OperatingSystem";
        }

        public static string GetBrowserName(string userAgent)
        {
            if (userAgent.Contains("Maxthon"))
            {
                return "Maxthon";
            }
            if (userAgent.Contains("MetaSr"))
            {
                return "MetaSr";
            }
            if (userAgent.Contains("BIDUBrowser"))
            {
                return "BIDUBrowser";
            }
            if (userAgent.Contains("QQBrowser"))
            {
                return "QQBrowser";
            }
            if (userAgent.Contains("GreenBrowser"))
            {
                return "GreenBrowser";
            }
            if (userAgent.Contains("360se"))
            {
                return "360se";
            }
            if (userAgent.Contains("MSIE 6.0"))
            {
                return "Internet Explorer 6.0";
            }
            if (userAgent.Contains("MSIE 7.0"))
            {
                return "Internet Explorer 7.0";
            }
            if (userAgent.Contains("MSIE 8.0"))
            {
                return "Internet Explorer 8.0";
            }
            if (userAgent.Contains("MSIE 9.0"))
            {
                return "Internet Explorer 9.0";
            }
            if (userAgent.Contains("MSIE 10.0"))
            {
                return "Internet Explorer 10.0";
            }
            if (userAgent.Contains("Firefox"))
            {
                return "Firefox";
            }
            if (userAgent.Contains("Opera"))
            {
                return "Opera";
            }
            if (userAgent.Contains("Chrome"))
            {
                return "Chrome";
            }
            if (userAgent.Contains("Safari"))
            {
                return "Safari";
            }
            return "Other Browser";
        }
    }
}
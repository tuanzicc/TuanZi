namespace TuanZi.Data
{
    public class RegexPatterns
    {
        public const string Ip = @"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))";
        public const string SubstringFormat = "(?<=({0})).+(?=({1}))";
        public const string Email = @"[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+";
        public const string Unicode = @"[\u4E00-\u9FA5\uE815-\uFA29]+";
        public const string Url = @"(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*";
   
    }
}
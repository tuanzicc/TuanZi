using System;


namespace TuanZi
{
    public static class BooleanExtensions
    {
        public static string ToLower(this bool value)
        {
            return value.ToString().ToLower();
        }
    }
}
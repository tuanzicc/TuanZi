
using System;

namespace TuanZi.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsEmpty(this Guid? guid)
        {
            return (null == guid || guid == Guid.Empty);
        }
        public static bool IsEmpty(this Guid guid)
        {
            return (null == guid || guid == Guid.Empty);
        }
    }
}

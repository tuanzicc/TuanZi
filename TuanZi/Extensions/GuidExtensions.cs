
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

        public static string ToShort(this Guid guid, string split = "-")
        {
            long i = 1;
            foreach (byte b in guid.ToByteArray())
                i *= ((int)b + 1);

            string temp = string.Format("{0:x}", i - DateTime.Now.Ticks);
            while (temp.Length != 16)
            {
                foreach (byte b in guid.ToByteArray())
                    i *= ((int)b + 1);
                temp = string.Format("{0:x}", i - DateTime.Now.Ticks);
            }


            if (!split.IsNullOrEmpty() && temp.Length > 3)
            {
                int step = temp.Length / 4;
                for (int index = step; index < temp.Length; index += (step + 1))
                {
                    temp = temp.Insert(index, split);
                }
            }

            return temp.ToUpper();
        }
    }
}

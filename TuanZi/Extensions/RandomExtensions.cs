using System;
using System.Collections.Generic;
using System.Linq;


namespace TuanZi.Extensions
{
    public static class RandomExtensions
    {
        public static bool NextBoolean(this Random random)
        {
            return random.NextDouble() > 0.5;
        }

        public static T NextEnum<T>(this Random random) where T : struct
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }
            Array array = Enum.GetValues(type);
            int index = random.Next(array.GetLowerBound(0), array.GetUpperBound(0) + 1);
            return (T)array.GetValue(index);
        }

        public static byte[] NextBytes(this Random random, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            byte[] data = new byte[length];
            random.NextBytes(data);
            return data;
        }

        public static T NextItem<T>(this Random random, T[] items)
        {
            return items[random.Next(0, items.Length)];
        }

        public static DateTime NextDateTime(this Random random, DateTime minValue, DateTime maxValue)
        {
            long ticks = minValue.Ticks + (long)((maxValue.Ticks - minValue.Ticks) * random.NextDouble());
            return new DateTime(ticks);
        }

        public static DateTime NextDateTime(this Random random)
        {
            return NextDateTime(random, DateTime.MinValue, DateTime.MaxValue);
        }

        public static string NextNumberString(this Random random, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            char[] pattern = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string result = "";
            int n = pattern.Length;
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += pattern[rnd];
            }
            return result;
        }

        public static string NextLetterString(this Random random, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            char[] pattern = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
                'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = pattern.Length;
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += pattern[rnd];
            }
            return result;
        }

        public static string NextLetterAndNumberString(this Random random, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            char[] pattern = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = pattern.Length;
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += pattern[rnd];
            }
            return result;
        }


        public static List<T> NextItems<T>(this Random random, T[] source, int count, params T[] excepts)
        {
            if (source.Length <= count)
            {
                return source.ToList();
            }
            List<T> result = new List<T>();
            while (result.Count < count)
            {
                T item = random.NextItem(source);
                if (result.Contains(item) || excepts.Contains(item))
                {
                    continue;
                }
                result.Add(item);
            }
            return result;
        }

        public static List<int> NextItems(this Random random, int min, int max, int count, params int[] excepts)
        {
            List<int> source = new List<int>();
            for (int i = min; i <= max; i++)
            {
                source.Add(i);
            }
            return random.NextItems(source.ToArray(), count, excepts);
        }
    }
}

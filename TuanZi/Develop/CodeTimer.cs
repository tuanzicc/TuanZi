using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


namespace TuanZi.Develop
{
    public static class CodeTimer
    {
        #region Private Methods

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            NativeMethods.QueryThreadCycleTime(NativeMethods.GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        #endregion

        #region Public Methods

        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Time(string.Empty, 1, () => { });
        }

        public static void Time(string name, int iteration, Action action)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i < GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(1);
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            ulong cycleCount = GetCycleCount();
            for (int i = 0; i < iteration; i++)
            {
                action();
            }
            ulong cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tTime Elapsed:\t" + watch.Elapsed.TotalMilliseconds + "ms");
            Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));

            for (int i = 0; i < GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen" + i + "\t\t" + count);
            }

            Console.WriteLine();
        }

        #endregion
    }


    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentThread();

    }

}

using System;
using System.Diagnostics;
using System.Threading;


namespace TuanZi.Develop
{
    public static class CodeRamer
    {
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Ram(string.Empty, () => { });
        }

        public static void Ram(string name, Action action)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            GC.Collect();
            long start = GC.GetTotalMemory(true);
            action();
            GC.Collect();
            GC.WaitForFullGCComplete();
            long end = GC.GetTotalMemory(true);

            Console.ForegroundColor = currentForeColor;
            long result = end - start;
            Console.WriteLine("\tRam:\t" + result + " B");
            Console.WriteLine("\tRam:\t" + result / 1024 + " KB");
            Console.WriteLine("\tRam:\t" + result / 1024 / 1024 + " MB");
            Console.WriteLine();
        }
    }
}

using System;
using System.Threading;


namespace TuanZi.Utility.Reflection
{
    public static class SyncLocker
    {
        public static void MutexLock(string key, Action action, bool recursive = false, bool process = false)
        {
            string mutexKey = GetKey(key, process);
            using (Mutex mutex = new Mutex(false, mutexKey))
            {
                try
                {
                    mutex.WaitOne();
                    action();
                }
                catch (AbandonedMutexException)
                {
                    if (recursive)
                    {
                        throw;
                    }
                    MutexLock(key, action, true, process);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private static string GetKey(string key, bool process)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }
            key = key.ToBase64String();
            return process ? $@"Global\{key}" : $@"Local\{key}";
        }
    }
}
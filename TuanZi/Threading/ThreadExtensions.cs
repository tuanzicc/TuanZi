
using System;
using System.Threading;


namespace TuanZi.Threading
{
    public static class ThreadExtensions
    {
        public static void CancelSleep(this Thread thread)
        {
            if (thread.ThreadState != ThreadState.WaitSleepJoin)
            {
                return;
            }
            thread.Interrupt();
        }

        public static void StartAndIgnoreAbort(this Thread thread, Action<Exception> failAction = null)
        {
            try
            {
                thread.Start();
            }
            catch (ThreadAbortException)
            { }
            catch (Exception e)
            {
                if (failAction != null)
                {
                    failAction(e);
                }
            }
        }
    }
}
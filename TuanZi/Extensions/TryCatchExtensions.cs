using System;


namespace TuanZi.Extensions
{
    public static class TryCatchExtensions
    {
        public static bool TryCatch<T>(this T source, Action<T> action, Action<Exception> failureAction, Action<T> successAction) where T : class
        {
            bool result;
            try
            {
                action(source);
                successAction(source);
                result = true;
            }
            catch (Exception obj)
            {
                failureAction(obj);
                result = false;
            }
            return result;
        }

        public static bool TryCatch<T>(this T source, Action<T> action, Action<Exception> failureAction) where T : class
        {
            return source.TryCatch(action,
                failureAction,
                obj =>
                { });
        }

        public static TResult TryCatch<T, TResult>(this T source, Func<T, TResult> func, Action<Exception> failureAction, Action<T> successAction)
            where T : class
        {
            TResult result;
            try
            {
                TResult u = func(source);
                successAction(source);
                result = u;
            }
            catch (Exception obj)
            {
                failureAction(obj);
                result = default(TResult);
            }
            return result;
        }

        public static TResult TryCatch<T, TResult>(this T source, Func<T, TResult> func, Action<Exception> failureAction) where T : class
        {
            return source.TryCatch(func,
                failureAction,
                obj =>
                { });
        }

        public static void TryFinally<T>(this T source, Action<T> action, Action<T> finallyAction) where T : class
        {
            try
            {
                action(source);
            }
            finally
            {
                finallyAction(source);
            }
        }
    }
}
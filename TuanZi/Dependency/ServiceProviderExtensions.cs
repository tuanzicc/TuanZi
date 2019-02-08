using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace TuanZi.Dependency
{
    public static class ServiceProviderExtensions
    {
        public static void ExecuteScopedWork(this IServiceProvider provider, Action<IServiceProvider> action)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                action(scope.ServiceProvider);
            }
        }

        public static async Task ExecuteScopedWorkAsync(this IServiceProvider provider, Func<IServiceProvider, Task> action)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public static TResult ExecuteScopedWork<TResult>(this IServiceProvider provider, Func<IServiceProvider, TResult> func)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                return func(scope.ServiceProvider);
            }
        }

        public static async Task<TResult> ExecuteScopedWorkAsync<TResult>(this IServiceProvider provider, Func<IServiceProvider, Task<TResult>> func)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                return await func(scope.ServiceProvider);
            }
        }

        public static ClaimsPrincipal GetCurrentUser(this IServiceProvider provider)
        {
            try
            {
                IPrincipal user = provider.GetService<IPrincipal>();
                return user as ClaimsPrincipal;
            }
            catch
            {
                return null;
            }
        }
    }
}
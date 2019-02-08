using System;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.AspNetCore
{
    public static class ServiceProviderExtensions
    {
        public static HttpContext HttpContext(this IServiceProvider provider)
        {
            IHttpContextAccessor accessor = provider.GetService<IHttpContextAccessor>();
            return accessor?.HttpContext;
        }

        public static bool InHttpRequest(this IServiceProvider provider)
        {
            var context = provider.HttpContext();
            return context != null;
        }
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;


namespace TuanZi.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IHostingEnvironment GetHostingEnvironment(this IServiceCollection services)
        {
            return services.GetSingletonInstance<IHostingEnvironment>();
        }
    }
}
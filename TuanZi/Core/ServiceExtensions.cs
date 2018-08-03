using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TuanZi.Core.Builders;
using TuanZi.Core.Packs;
using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Reflection;
using TuanZi.Data;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTuan(this IServiceCollection services, Action<ITuanBuilder> builderAction = null)
        {
            Check.NotNull(services, nameof(services));

            ITuanBuilder builder = new TuanBuilder();
            if (builderAction != null)
            {
                builderAction(builder);
            }
            TuanPackManager manager = new TuanPackManager(builder, new AppDomainAllAssemblyFinder());
            manager.LoadPacks(services);
            services.AddSingleton(provider => manager);
            return services;
        }

        public static TuanOptions GetTuanOptions(this IServiceProvider provider)
        {
            return provider.GetService<IOptions<TuanOptions>>()?.Value;
        }
    }
}
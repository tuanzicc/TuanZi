using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TuanZi.Core.Builders;
using TuanZi.Core.Modules;
using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Reflection;


namespace TuanZi
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTuan(this IServiceCollection services, Action<ITuanBuilder> builderAction = null, AppServiceScanOptions scanOptions = null)
        {
            Check.NotNull(services, nameof(services));

            ITuanBuilder builder = new TuanBuilder();
            if (builderAction != null)
            {
                builderAction(builder);
            }
            TuanModuleManager manager = new TuanModuleManager(builder, new AppDomainAllAssemblyFinder());
            manager.LoadModules(services);
            services.AddSingleton(provider => manager);
            if (scanOptions == null)
            {
                scanOptions = new AppServiceScanOptions();
            }
            services = new AppServiceAdder(scanOptions).AddServices(services);
            if (builder.OptionsAction != null)
            {
                services.Configure(builder.OptionsAction);
            }
            return services;
        }

        public static TuanOptions GetTuanOptions(this IServiceProvider provider)
        {
            return provider.GetService<IOptionsMonitor<TuanOptions>>()?.CurrentValue;
        }
    }
}
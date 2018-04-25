using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TuanZi.Core.Options;


namespace TuanZi.Core.Modules
{
    public class TuanCoreModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Core;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<TuanOptions>, TuanOptionsSetup>();
            ServiceLocator.Instance.TrySetServiceCollection(services);

            return services;
        }

        public override void UseModule(IServiceProvider provider)
        {
            ServiceLocator.Instance.TrySetApplicationServiceProvider(provider);

            IsEnabled = true;
        }
    }
}
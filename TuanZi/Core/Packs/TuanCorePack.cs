using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TuanZi.Core.Options;
using TuanZi.Dependency;


namespace TuanZi.Core.Packs
{
    public class TuanCorePack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<TuanOptions>, TuanOptionsSetup>();
            ServiceLocator.Instance.TrySetServiceCollection(services);

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            ServiceLocator.Instance.TrySetApplicationServiceProvider(provider);

            IsEnabled = true;
        }
    }
}
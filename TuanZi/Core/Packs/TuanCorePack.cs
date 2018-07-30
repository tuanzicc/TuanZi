using System;
using Microsoft.AspNetCore.Builder;
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
            ServiceLocator.Instance.SetServiceCollection(services);

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            IServiceProvider provider = app.ApplicationServices;
            ServiceLocator.Instance.SetApplicationServiceProvider(provider);
            IsEnabled = true;
        }
    }
}
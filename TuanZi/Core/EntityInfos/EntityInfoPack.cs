using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;
using TuanZi.Entity;


namespace TuanZi.Core.EntityInfos
{
    public class EntityInfoPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IEntityTypeFinder, EntityTypeFinder>();
            services.AddSingleton<IEntityInfoHandler, EntityInfoHandler>();

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            IServiceProvider provider = app.ApplicationServices;
            IEntityInfoHandler handler = provider.GetService<IEntityInfoHandler>();
            handler.Initialize();
            IsEnabled = true;
        }
    }
}
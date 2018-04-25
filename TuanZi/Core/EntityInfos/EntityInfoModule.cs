using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;
using TuanZi.Entity;


namespace TuanZi.Core.EntityInfos
{
    public class EntityInfoModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IEntityTypeFinder, EntityTypeFinder>();
            services.AddSingleton<IEntityInfoHandler, EntityInfoHandler>();

            return services;
        }

        public override void UseModule(IServiceProvider provider)
        {
            IEntityInfoHandler handler = provider.GetService<IEntityInfoHandler>();
            handler.Initialize();
            IsEnabled = true;
        }
    }
}
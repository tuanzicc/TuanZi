using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TuanZi.Core.Functions;
using TuanZi.Core.Modules;
using TuanZi.Core.Packs;
using TuanZi.Dependency;

namespace TuanZi.AspNetCore.Mvc
{
    [DependsOnPacks(typeof(AspNetCorePack))]
    public class MvcFunctionPack : AspTuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.GetOrAddTypeFinder<IFunctionTypeFinder>(assemblyFinder => new MvcControllerTypeFinder(assemblyFinder));
            services.AddSingleton<IFunctionHandler, MvcFunctionHandler>();
            services.TryAddSingleton<IModuleInfoPicker, MvcModuleInfoPicker>();

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            IFunctionHandler functionHandler = app.ApplicationServices.GetServices<IFunctionHandler>().FirstOrDefault(m => m.GetType() == typeof(MvcFunctionHandler));
            if (functionHandler == null)
            {
                return;
            }
            functionHandler.Initialize();

            IsEnabled = true;
        }
    }
}
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Core.Functions;
using TuanZi.Core.Modules;
using TuanZi.Core.Packs;


namespace TuanZi.AspNetCore.Mvc
{
    public class MvcFunctionPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IFunctionHandler, MvcFunctionHandler>();
            services.AddSingleton<IModuleInfoPicker, MvcModuleInfoPicker>();

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
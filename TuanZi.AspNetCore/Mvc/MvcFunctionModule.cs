using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Core.Functions;
using TuanZi.Core.Modules;


namespace TuanZi.AspNetCore.Mvc
{
    public class MvcFunctionModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IFunctionHandler, MvcFunctionHandler>();
            return services;
        }

        public override void UseModule(IServiceProvider provider)
        {
            IFunctionHandler handler = provider.GetServices<IFunctionHandler>().FirstOrDefault(m => m.GetType() == typeof(MvcFunctionHandler));
            if (handler == null)
            {
                return;
            }
            handler.Initialize();
            IsEnabled = true;
        }
    }
}
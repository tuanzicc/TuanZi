using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;


namespace TuanZi.AspNetCore.Mvc
{
    public class MvcFunctionModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IMvcFunctionHandler, MvcFunctionHandler>();
            return services;
        }

        public override void UseModule(IServiceProvider provider)
        {
            IMvcFunctionHandler handler = provider.GetService<IMvcFunctionHandler>();
            handler.Initialize();
            IsEnabled = true;
        }
    }
}
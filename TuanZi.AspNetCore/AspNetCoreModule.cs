using Microsoft.Extensions.DependencyInjection;

using TuanZi.AspNetCore.Infrastructure;
using TuanZi.AspNetCore.Mvc.Filters;
using TuanZi.Core.Modules;
using TuanZi.Dependency;


namespace TuanZi.AspNetCore
{
    public class AspNetCoreModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Core;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IScopedServiceResolver, RequestScopedServiceResolver>();
            services.AddScoped<UnitOfWorkAttribute>();

            return services;
        }
    }
}
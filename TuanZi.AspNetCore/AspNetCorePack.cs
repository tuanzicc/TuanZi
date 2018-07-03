using Microsoft.Extensions.DependencyInjection;

using TuanZi.AspNetCore.Infrastructure;
using TuanZi.AspNetCore.Mvc.Filters;
using TuanZi.Core.Packs;
using TuanZi.Dependency;


namespace TuanZi.AspNetCore
{
    public class AspNetCorePack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IScopedServiceResolver, RequestScopedServiceResolver>();
            services.AddScoped<UnitOfWorkAttribute>();

            return services;
        }
    }
}
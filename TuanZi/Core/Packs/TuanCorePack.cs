using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Reflection;

namespace TuanZi.Core.Packs
{
    public class TuanCorePack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IAllAssemblyFinder, AppDomainAllAssemblyFinder>();
            services.AddSingleton<IConfigureOptions<TuanOptions>, TuanOptionsSetup>();

            return services;
        }
    }
}
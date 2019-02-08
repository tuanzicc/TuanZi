using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using TuanZi.Caching;
using TuanZi.Core.Options;
using TuanZi.Entity;
using TuanZi.Entity.Infrastructure;
using TuanZi.Filter;

namespace TuanZi.Core.Packs
{
    public class TuanCorePack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IConfigureOptions<TuanOptions>, TuanOptionsSetup>();
            services.TryAddSingleton<IEntityTypeFinder, EntityTypeFinder>();
            services.TryAddSingleton<IInputDtoTypeFinder, InputDtoTypeFinder>();
            services.TryAddSingleton<IOutputDtoTypeFinder, OutputDtoTypeFinder>();

            services.TryAddSingleton<ICacheService, CacheService>();
            services.TryAddScoped<IFilterService, FilterService>();

            return services;
        }
    }
}
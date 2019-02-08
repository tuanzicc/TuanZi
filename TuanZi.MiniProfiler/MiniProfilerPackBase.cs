using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.AspNetCore;
using TuanZi.Core.Packs;

using StackExchange.Profiling;


namespace TuanZi.MiniProfiler
{
    public abstract class MiniProfilerPackBase : AspTuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override int Order => 0;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            Action<MiniProfilerOptions> miniProfilerAction = GetMiniProfilerAction(services);

            services.AddMiniProfiler(miniProfilerAction).AddEntityFramework();

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            app.UseMiniProfiler();
            IsEnabled = true;
        }

        protected virtual Action<MiniProfilerOptions> GetMiniProfilerAction(IServiceCollection services)
        {
            return options =>
            {
                options.RouteBasePath = "/profiler";
            };
        }
    }
}
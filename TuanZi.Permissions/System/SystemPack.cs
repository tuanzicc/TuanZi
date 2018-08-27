using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;
using TuanZi.Core.Systems;

namespace TuanZi.Systems
{
    public class SystemPack : TuanPack
    {
        public override int Order => 0;

        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddScoped<SystemManager>();
            services.AddScoped<IKeyValueStore>(provider => provider.GetService<SystemManager>());

            return services;
        }
    }
}
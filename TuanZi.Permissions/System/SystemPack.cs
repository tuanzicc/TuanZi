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
            services.AddScoped<KeyValueStore>();
            services.AddScoped<IKeyValueStore>(provider => provider.GetService<KeyValueStore>());

            return services;
        }
    }
}
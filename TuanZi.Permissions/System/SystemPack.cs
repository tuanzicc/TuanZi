using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;


namespace TuanZi.System
{
    public class SystemPack : TuanPack
    {
        public override int Order => 0;

        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddScoped<SystemManager>();
            services.AddScoped<IKeyValueCoupleStore>(provider => provider.GetService<SystemManager>());

            return services;
        }
    }
}
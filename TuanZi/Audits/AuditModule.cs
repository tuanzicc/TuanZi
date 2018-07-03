using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core;
using TuanZi.Core.Packs;
using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    [DependsOnPacks(typeof(EventBusPack))]
    public class AuditPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddTransient<AuditEntityStoreEventHandler>();
            services.AddSingleton<IAuditStore, NullAuditStore>();

            return services;
        }
    }
}
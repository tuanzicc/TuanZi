using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;
using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    [DependsOnPacks(typeof(EventBusPack))]
    public abstract class AuditPackBase : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddTransient<AuditEntityEventHandler>();

            return services;
        }
    }
}
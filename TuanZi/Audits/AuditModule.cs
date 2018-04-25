using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core;
using TuanZi.Core.Modules;
using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    [DependsOnModules(typeof(EventBusModule))]
    public class AuditModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddTransient<AuditEntityStoreEventHandler>();
            services.AddSingleton<IAuditStore, NullAuditStore>();

            return services;
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core;
using TuanZi.Core.Modules;
using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    public class FileModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            
            //services.AddSingleton<IAuditStore, NullAuditStore>();

            return services;
        }
    }
}
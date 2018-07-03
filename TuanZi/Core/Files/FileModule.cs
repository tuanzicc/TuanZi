using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core;
using TuanZi.Core.Packs;
using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    public class FilePack : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            
            //services.AddSingleton<IAuditStore, NullAuditStore>();

            return services;
        }
    }
}
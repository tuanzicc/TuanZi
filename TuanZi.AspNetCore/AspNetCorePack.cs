using System.Security.Principal;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;

namespace TuanZi.AspNetCore
{
    public class AspNetCorePack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override int Order => 2;

        #region Overrides of TuanPack

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddTransient<IPrincipal>(provider =>
            {
                IHttpContextAccessor accessor = provider.GetService<IHttpContextAccessor>();
                return accessor?.HttpContext?.User;
            });

            return services;
        }

        #endregion
    }
}
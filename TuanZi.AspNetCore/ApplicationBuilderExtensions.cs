using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Core;
using TuanZi.Core.Packs;



namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTuan(this IApplicationBuilder app)
        {
            IServiceProvider provider = app.ApplicationServices;
            TuanPackManager packManager = provider.GetService<TuanPackManager>();
            packManager.UsePacks(app);

            return app;
        }

        public static IApplicationBuilder UseMvcWithAreaRoute(this IApplicationBuilder app, bool area = true)
        {
            return app.UseMvc(builder =>
            {
                if (area)
                {
                    builder.MapRoute("area", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                }
                builder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
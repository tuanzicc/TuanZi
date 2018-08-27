using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Core;
using TuanZi.Core.Packs;
using TuanZi.Exceptions;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTuan(this IApplicationBuilder app)
        {
            IServiceProvider provider = app.ApplicationServices;
            if (!(provider.GetService<ITuanPackManager>() is IAspUsePack aspPackManager))
            {
                throw new TuanException("The injection type of the interface ITuanPackManager is incorrect. This type should implement the interface IAspUsePack at the same time.");
            }
            aspPackManager.UsePack(app);

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
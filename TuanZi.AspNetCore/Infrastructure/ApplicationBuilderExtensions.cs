using System;
using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.EventBuses;
using TuanZi.Core;
using TuanZi.Core.Modules;


namespace TuanZi
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTuanMvc(this IApplicationBuilder app)
        {
            IServiceProvider serviceProvider = app.ApplicationServices;

            TuanModuleManager moduleManager = serviceProvider.GetService<TuanModuleManager>();
            moduleManager.UseModules(serviceProvider);

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
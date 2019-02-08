using System;

using Hangfire;
using Hangfire.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.AspNetCore;
using TuanZi.Core.Packs;
using TuanZi.Extensions;


namespace TuanZi.Hangfire
{
    public abstract class HangfirePackCore : AspTuanPack
    {
        public override PackLevel Level => PackLevel.Framework;

        public override int Order => 0;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            IConfiguration configuration = services.GetConfiguration();
            bool enabled = configuration["Tuan:Hangfire:Enabled"].CastTo(false);
            if (!enabled)
            {
                return services;
            }

            Action<IGlobalConfiguration> hangfireAction = GetHangfireAction(services);
            services.AddHangfire(hangfireAction);
            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            IServiceProvider serviceProvider = app.ApplicationServices;
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            bool enabled = configuration["Tuan:Hangfire:Enabled"].CastTo(false);
            if (!enabled)
            {
                return;
            }

            IGlobalConfiguration globalConfiguration = serviceProvider.GetService<IGlobalConfiguration>();
            globalConfiguration.UseLogProvider(new AspNetCoreLogProvider(serviceProvider.GetService<ILoggerFactory>()));

            BackgroundJobServerOptions serverOptions = GetBackgroundJobServerOptions(configuration);
            app.UseHangfireServer(serverOptions);

            string url = configuration["Tuan:Hangfire:DashboardUrl"].CastTo("/hangfire");
            DashboardOptions dashboardOptions = GetDashboardOptions(configuration);
            app.UseHangfireDashboard(url, dashboardOptions);

            IHangfireJobRunner jobRunner = serviceProvider.GetService<IHangfireJobRunner>();
            jobRunner?.Start();

            IsEnabled = true;
        }

        protected virtual Action<IGlobalConfiguration> GetHangfireAction(IServiceCollection services)
        {
            IConfiguration configuration = services.GetConfiguration();
            string storageConnectionString = configuration["Tuan:Hangfire:StorageConnectionString"].CastTo<string>();
            if (storageConnectionString != null)
            {
                return config => config.UseSqlServerStorage(storageConnectionString);
            }

            return config => { };
        }

        protected virtual BackgroundJobServerOptions GetBackgroundJobServerOptions(IConfiguration configuration)
        {
            BackgroundJobServerOptions serverOptions = new BackgroundJobServerOptions();
            int workerCount = configuration["Tuan:Hangfire:WorkerCount"].CastTo(0);
            if (workerCount > 0)
            {
                serverOptions.WorkerCount = workerCount;
            }
            return serverOptions;
        }

        protected virtual DashboardOptions GetDashboardOptions(IConfiguration configuration)
        {
            string[] roles = configuration["Tuan:Hangfire:Roles"].CastTo("").Split(",", true);
            DashboardOptions dashboardOptions = new DashboardOptions();
            if (roles.Length > 0)
            {
                dashboardOptions.Authorization = new[] { new RoleDashboardAuthorizationFilter(roles) };
            }
            return dashboardOptions;
        }
    }
}
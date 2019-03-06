using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Newtonsoft.Json.Serialization;

using TuanZi.Core.Packs;


namespace TuanZi.AspNetCore.SignalR
{
    [DependsOnPacks(typeof(AspNetCorePack))]
    public abstract class SignalRPackBase : AspTuanPack
    {
        public override PackLevel Level => PackLevel.Application;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IUserIdProvider, UserNameUserIdProvider>();
            services.TryAddSingleton<IConnectionUserCache, ConnectionUserCache>();

            ISignalRServerBuilder builder = services.AddSignalR();
            Action<ISignalRServerBuilder> buildAction = GetSignalRServerBuildAction(services);
            buildAction?.Invoke(builder);

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            Action<HubRouteBuilder> hubRouteBuildAction = GetHubRouteBuildAction(app.ApplicationServices);
            app.UseSignalR(hubRouteBuildAction);
        }

        protected virtual Action<ISignalRServerBuilder> GetSignalRServerBuildAction(IServiceCollection services)
        {
            return builder => builder.AddJsonProtocol(config => config.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        protected abstract Action<HubRouteBuilder> GetHubRouteBuildAction(IServiceProvider provider);
    }
}
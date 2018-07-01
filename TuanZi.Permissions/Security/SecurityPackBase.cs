

using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.EntityInfos;
using TuanZi.Core.Functions;
using TuanZi.Core.Modules;
using TuanZi.Core.Packs;
using TuanZi.Entity;
using TuanZi.Secutiry;

namespace TuanZi.Security
{
    public abstract class SecurityPackBase<TSecurityManager, TFunctionAuthorization, TFunctionAuthCache, TModuleHandler, TFunction, TFunctionInputDto, TEntityInfo,
      TEntityInfoInputDto, TModule, TModuleInputDto, TModuleKey, TModuleFunction, TModuleRole, TModuleUser, TRoleKey, TUserKey> : TuanPack
      where TSecurityManager : class, IFunctionStore<TFunction, TFunctionInputDto>,
      IEntityInfoStore<TEntityInfo, TEntityInfoInputDto>,
      IModuleStore<TModule, TModuleInputDto, TModuleKey>,
      IModuleFunctionStore<TModuleFunction, TModuleKey>,
      IModuleRoleStore<TModuleRole, TRoleKey, TModuleKey>,
      IModuleUserStore<TModuleUser, TUserKey, TModuleKey>
      where TFunctionAuthorization : IFunctionAuthorization
      where TFunctionAuthCache : IFunctionAuthCache
      where TModuleHandler : IModuleHandler
      where TFunction : IFunction
      where TFunctionInputDto : FunctionInputDtoBase
      where TEntityInfo : IEntityInfo, IEntity<Guid>
      where TEntityInfoInputDto : EntityInfoInputDtoBase
      where TModule : ModuleBase<TModuleKey>
      where TModuleInputDto : ModuleInputDtoBase<TModuleKey>
      where TModuleFunction : ModuleFunctionBase<TModuleKey>
      where TModuleRole : ModuleRoleBase<TModuleKey, TRoleKey>
      where TModuleUser : ModuleUserBase<TModuleKey, TUserKey>
      where TModuleKey : struct, IEquatable<TModuleKey>
      where TRoleKey : IEquatable<TRoleKey>
      where TUserKey : IEquatable<TUserKey>
    {
        public override PackLevel Level => PackLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddScoped<TSecurityManager>();

            services.AddSingleton(typeof(IFunctionAuthorization), typeof(TFunctionAuthorization));
            services.AddSingleton(typeof(IFunctionAuthCache), typeof(TFunctionAuthCache));
            services.AddSingleton(typeof(IModuleHandler), typeof(TModuleHandler));

            services.AddScoped(typeof(IFunctionStore<TFunction, TFunctionInputDto>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IEntityInfoStore<TEntityInfo, TEntityInfoInputDto>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleStore<TModule, TModuleInputDto, TModuleKey>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleFunctionStore<TModuleFunction, TModuleKey>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleRoleStore<TModuleRole, TRoleKey, TModuleKey>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleUserStore<TModuleUser, TUserKey, TModuleKey>), provider => provider.GetService<TSecurityManager>());

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            IModuleHandler moduleHandler = provider.GetService<IModuleHandler>();
            moduleHandler.Initialize();

            IFunctionHandler functionHandler = provider.GetService<IFunctionHandler>();
            functionHandler.RefreshCache();

            IEntityInfoHandler entityInfoHandler = provider.GetService<IEntityInfoHandler>();
            entityInfoHandler.RefreshCache();

            IFunctionAuthCache functionAuthCache = provider.GetService<IFunctionAuthCache>();
            functionAuthCache.BuildRoleCaches();

            base.UsePack(provider);
        }
    }
}
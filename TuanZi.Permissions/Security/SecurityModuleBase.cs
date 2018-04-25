

using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.EntityInfos;
using TuanZi.Core.Functions;
using TuanZi.Core.Modules;
using TuanZi.Entity;


namespace TuanZi.Security
{
    public abstract class SecurityModuleBase<TSecurityManager, TFunction, TFunctionInputDto, TEntityInfo, TEntityInfoInputDto,
        TModule, TModuleInputDto, TModuleKey, TModuleFunction, TModuleRole, TModuleUser, TRoleKey, TUserKey> : TuanModule
        where TSecurityManager : class, IFunctionStore<TFunction, TFunctionInputDto>,
        IEntityInfoStore<TEntityInfo, TEntityInfoInputDto>,
        IModuleStore<TModule, TModuleInputDto, TModuleKey>,
        IModuleFunctionStore<TModuleFunction>,
        IModuleRoleStore<TModuleRole>,
        IModuleUserStore<TModuleUser>
        where TFunction : IFunction, IEntity<Guid>
        where TFunctionInputDto : FunctionInputDtoBase
        where TEntityInfo : IEntityInfo, IEntity<Guid>
        where TEntityInfoInputDto : EntityInfoInputDtoBase
        where TModule : ModuleBase<TModuleKey>
        where TModuleInputDto : ModuleInputDtoBase<TModuleKey>
        where TModuleFunction : ModuleFunctionBase<TModuleKey>
        where TModuleRole : ModuleRoleBase<TModuleKey, TRoleKey>
        where TModuleUser : ModuleUserBase<TModuleKey, TUserKey>
        where TModuleKey : struct, IEquatable<TModuleKey>
    {
        public override ModuleLevel Level => ModuleLevel.Application;
        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddScoped<TSecurityManager>();

            services.AddScoped(typeof(IFunctionStore<TFunction, TFunctionInputDto>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IEntityInfoStore<TEntityInfo, TEntityInfoInputDto>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleStore<TModule, TModuleInputDto, TModuleKey>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleFunctionStore<TModuleFunction>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleRoleStore<TModuleRole>), provider => provider.GetService<TSecurityManager>());
            services.AddScoped(typeof(IModuleUserStore<TModuleUser>), provider => provider.GetService<TSecurityManager>());

            return services;
        }
    }
}
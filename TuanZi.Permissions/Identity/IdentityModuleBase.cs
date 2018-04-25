using System;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core;
using TuanZi.Core.Modules;


namespace TuanZi.Identity
{
    public abstract class IdentityModuleBase<TUserStore, TRoleStore, TUser, TRole, TUserKey, TRoleKey> : TuanModule
        where TUserStore : class, IUserStore<TUser>
        where TRoleStore : class, IRoleStore<TRole>
        where TUser : UserBase<TUserKey>
        where TRole : RoleBase<TRoleKey>
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        public override ModuleLevel Level => ModuleLevel.Application;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserStore<TUser>, TUserStore>();
            services.AddScoped<IRoleStore<TRole>, TRoleStore>();

            Action<IdentityOptions> setupAction = SetupAction();
            IdentityBuilder builder = services.AddIdentity<TUser, TRole>(setupAction);
            OnIdentityBuild(builder);

            return services;
        }

        protected virtual Action<IdentityOptions> SetupAction()
        {
            return null;
        }

        protected virtual IdentityBuilder OnIdentityBuild(IdentityBuilder builder)
        {
            return builder;
        }
    }
}
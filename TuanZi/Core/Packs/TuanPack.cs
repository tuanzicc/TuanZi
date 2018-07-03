using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Reflection;


namespace TuanZi.Core.Packs
{
    public abstract class TuanPack
    {
        public virtual PackLevel Level => PackLevel.Business;

        public virtual int Order => 0;

        public bool IsEnabled { get; protected set; }

        public virtual IServiceCollection AddServices(IServiceCollection services)
        {
            return services;
        }

        public virtual void UsePack(IServiceProvider provider)
        {
            IsEnabled = true;
        }

        internal Type[] GetDependModuleTypes()
        {
            DependsOnPacksAttribute depends = this.GetType().GetAttribute<DependsOnPacksAttribute>();
            if (depends == null)
            {
                return new Type[0];
            }
            return depends.DependedModuleTypes;
        }
    }
}
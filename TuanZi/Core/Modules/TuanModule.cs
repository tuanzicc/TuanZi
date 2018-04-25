using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Reflection;


namespace TuanZi.Core.Modules
{
    public abstract class TuanModule
    {
        public virtual ModuleLevel Level => ModuleLevel.Business;

        public virtual int Order => 0;

        public bool IsEnabled { get; protected set; }

        public virtual IServiceCollection AddServices(IServiceCollection services)
        {
            return services;
        }

        public virtual void UseModule(IServiceProvider provider)
        {
            IsEnabled = true;
        }

        internal Type[] GetDependModuleTypes()
        {
            DependsOnModulesAttribute depends = this.GetType().GetAttribute<DependsOnModulesAttribute>();
            if (depends == null)
            {
                return new Type[0];
            }
            return depends.DependedModuleTypes;
        }
    }
}
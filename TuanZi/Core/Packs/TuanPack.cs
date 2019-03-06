using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
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

        internal Type[] GetDependPackTypes(Type packType = null)
        {
            if (packType == null)
            {
                packType = GetType();
            }
            DependsOnPacksAttribute[] dependAttrs = packType.GetAttributes<DependsOnPacksAttribute>();
            if (dependAttrs.Length == 0)
            {
                return new Type[0];
            }
            List<Type> dependTypes = new List<Type>();
            foreach (DependsOnPacksAttribute dependAttr in dependAttrs)
            {
                Type[] packTypes = dependAttr.DependedPackTypes;
                if (packTypes.Length == 0)
                {
                    continue;
                }
                dependTypes.AddRange(packTypes);
                foreach (Type type in packTypes)
                {
                    dependTypes.AddRange(GetDependPackTypes(type));
                }
            }

            return dependTypes.Distinct().ToArray();
        }
    }
}
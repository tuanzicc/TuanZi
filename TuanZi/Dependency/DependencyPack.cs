using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TuanZi.Core.Packs;
using TuanZi.Reflection;


namespace TuanZi.Dependency
{
    public class DependencyPack : TuanPack
    {
        public DependencyPack()
        {
            ScanOptions = new ServiceScanOptions();
        }

        public override PackLevel Level => PackLevel.Core;

        public override int Order => 1;

        protected virtual ServiceScanOptions ScanOptions { get; }

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            ServiceLocator.Instance.SetServiceCollection(services);

            services.AddScoped<ScopedDictionary>();

            Type[] dependencyTypes = ScanOptions.TransientTypeFinder.FindAll();
            AddTypeWithInterfaces(services, dependencyTypes, ServiceLifetime.Transient);

            dependencyTypes = ScanOptions.ScopedTypeFinder.FindAll();
            AddTypeWithInterfaces(services, dependencyTypes, ServiceLifetime.Scoped);

            dependencyTypes = ScanOptions.SingletonTypeFinder.FindAll();
            AddTypeWithInterfaces(services, dependencyTypes, ServiceLifetime.Singleton);

            return services;
        }
        
        protected virtual IServiceCollection AddTypeWithInterfaces(IServiceCollection services, Type[] implementationTypes, ServiceLifetime lifetime)
        {
            foreach (Type implementationType in implementationTypes)
            {
                if (implementationType.IsAbstract || implementationType.IsInterface)
                {
                    continue;
                }
                Type[] interfaceTypes = GetImplementedInterfaces(implementationType);
                if (interfaceTypes.Length == 0)
                {
                    services.TryAdd(new ServiceDescriptor(implementationType, implementationType, lifetime));
                    continue;
                }
                for (int i = 0; i < interfaceTypes.Length; i++)
                {
                    Type interfaceType = interfaceTypes[i];
                    if (lifetime == ServiceLifetime.Transient)
                    {
                        services.TryAddEnumerable(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                        continue;
                    }
                    bool multiple = interfaceType.HasAttribute<MultipleDependencyAttribute>();
                    if (i == 0)
                    {
                        if (multiple)
                        {
                            services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                        }
                        else
                        {
                            services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                        }
                    }
                    else
                    {
                        Type firstInterfaceType = interfaceTypes[0];
                        if (multiple)
                        {
                            services.Add(new ServiceDescriptor(interfaceType, provider => provider.GetService(firstInterfaceType), lifetime));
                        }
                        else
                        {
                            services.TryAdd(new ServiceDescriptor(interfaceType, provider => provider.GetService(firstInterfaceType), lifetime));
                        }
                    }
                }
            }
            return services;
        }

        //Tuan
        private static Type[] GetImplementedInterfaces(Type type)
        {
            Type[] exceptInterfaces = { typeof(IDisposable) };
            TypeFilter theFilter = new TypeFilter(InterfaceFilter);
            Type[] interfaceTypes = type.FindInterfaces(theFilter, type.BaseType).Where(t => !exceptInterfaces.Contains(t) && !t.HasAttribute<IgnoreDependencyAttribute>()).ToArray();
            if (interfaceTypes.Length == 0)
                interfaceTypes = type.GetInterfaces().Where(t => !exceptInterfaces.Contains(t) && !t.HasAttribute<IgnoreDependencyAttribute>()).ToArray();
            for (int index = 0; index < interfaceTypes.Length; index++)
            {
                Type interfaceType = interfaceTypes[index];
                if (interfaceType.IsGenericType && !interfaceType.IsGenericTypeDefinition && interfaceType.FullName == null)
                {
                    interfaceTypes[index] = interfaceType.GetGenericTypeDefinition();
                }
            }
            return interfaceTypes;
        }
        public static bool InterfaceFilter(Type typeObj, Object criteriaObj)
        {
            Type baseClassType = (Type)criteriaObj;
            Type[] interfaces_array = baseClassType.GetInterfaces();
            for (int i = 0; i < interfaces_array.Length; i++)
            {
                if (typeObj.ToString() == interfaces_array[i].ToString())
                    return false;
            }
            return true;
        }


        public override void UsePack(IApplicationBuilder app)
        {
            ServiceLocator.Instance.SetApplicationServiceProvider(app.ApplicationServices);
            IsEnabled = true;
        }
    }
}
﻿using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TuanZi.Core.Options;
using TuanZi.Reflection;

namespace TuanZi.Dependency
{
    public class AppServiceAdder : IAppServiceAdder
    {
        private readonly AppServiceScanOptions _options;

        public AppServiceAdder()
            : this(new AppServiceScanOptions())
        { }

        public AppServiceAdder(AppServiceScanOptions options)
        {
            _options = options;
        }

        public IServiceCollection AddServices(IServiceCollection services)
        {
            Type[] dependencyTypes = _options.TransientTypeFinder.FindAll();
            AddTypeWithInterfaces(services, dependencyTypes, ServiceLifetime.Transient);

            dependencyTypes = _options.ScopedTypeFinder.FindAll();
            AddTypeWithInterfaces(services, dependencyTypes, ServiceLifetime.Scoped);

            dependencyTypes = _options.SingletonTypeFinder.FindAll();
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
                    if (i == 0)
                    {
                        services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                    }
                    else
                    {
                        Type firstInterfaceType = interfaceTypes[0];
                        services.TryAdd(new ServiceDescriptor(interfaceType, provider => provider.GetService(firstInterfaceType), lifetime));
                    }
                }
            }
            return services;
        }

        private static Type[] GetImplementedInterfaces(Type type)
        {
            Type[] exceptInterfaces = { typeof(IDisposable) };
            Type[] interfaceTypes = type.GetInterfaces().Where(t => !exceptInterfaces.Contains(t) && !t.HasAttribute<IgnoreDependencyAttribute>()).ToArray();
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
    }
}
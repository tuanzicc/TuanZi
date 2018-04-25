using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;


namespace TuanZi
{
    public sealed class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> InstanceLazy = new Lazy<ServiceLocator>(() => new ServiceLocator());
        private IServiceProvider _provider;

        private IServiceCollection _services;

        private ServiceLocator()
        { }

        public static ServiceLocator Instance => InstanceLazy.Value;

        internal void TrySetServiceCollection(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            if (_services == null)
            {
                _services = services;
            }
        }

        public void TrySetApplicationServiceProvider(IServiceProvider provider)
        {
            Check.NotNull(provider, nameof(provider));
            if (_provider == null)
            {
                _provider = provider;
            }
        }

        public IEnumerable<ServiceDescriptor> GetServiceDescriptors()
        {
            Check.NotNull(_services, nameof(_services));
            return _services;
        }

        public T GetService<T>()
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetService<T>();
            }
            return _provider.GetService<T>();
        }

        public object GetService(Type serviceType)
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetService(serviceType);
            }
            return _provider.GetService(serviceType);
        }

        public IEnumerable<T> GetServices<T>()
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetServices<T>();
            }
            return _provider.GetServices<T>();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            Check.NotNull(_services, nameof(_services));
            Check.NotNull(_provider, nameof(_provider));

            IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
            if (scopedResolver != null && scopedResolver.ResolveEnabled)
            {
                return scopedResolver.GetServices(serviceType);
            }
            return _provider.GetServices(serviceType);
        }
    }
}
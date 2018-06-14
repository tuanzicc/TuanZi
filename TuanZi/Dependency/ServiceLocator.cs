using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;
using TuanZi.Exceptions;

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

        public bool IsProviderEnabled => _provider != null;

        public IServiceProvider ScopedProvider
        {
            get
            {
                IScopedServiceResolver scopedResolver = _provider.GetService<IScopedServiceResolver>();
                return scopedResolver != null && scopedResolver.ResolveEnabled
                    ? scopedResolver.ScopedProvider
                    : null;
            }
        }

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

        public void ExcuteScopedWork(Action<IServiceProvider> action)
        {
            if (_provider == null)
            {
                throw new TuanException("Root-level IServiceProvider does not exist and cannot perform Scoped services");
            }
            IServiceProvider scopedProvider = ScopedProvider;
            IServiceScope newScope = null;
            if (scopedProvider == null)
            {
                newScope = _provider.CreateScope();
                scopedProvider = newScope.ServiceProvider;
            }
            try
            {
                action(scopedProvider);
            }
            finally
            {
                newScope?.Dispose();
            }
        }

        public TResult ExcuteScopedWork<TResult>(Func<IServiceProvider, TResult> func)
        {
            if (_provider == null)
            {
                throw new TuanException("Root-level IServiceProvider does not exist and cannot perform Scoped services");
            }
            IServiceProvider scopedProvider = ScopedProvider;
            IServiceScope newScope = null;
            if (scopedProvider == null)
            {
                newScope = _provider.CreateScope();
                scopedProvider = newScope.ServiceProvider;
            }
            try
            {
                return func(scopedProvider);
            }
            finally
            {
                newScope?.Dispose();
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
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Exceptions;

namespace TuanZi.Dependency
{
    public sealed class ServiceLocator : IDisposable
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

        public static bool InScoped()
        {
            return Instance.ScopedProvider != null;
        }

        internal void SetServiceCollection(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            _services = services;
        }

        internal void SetApplicationServiceProvider(IServiceProvider provider)
        {
            Check.NotNull(provider, nameof(provider));
            _provider = provider;
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

        public async Task ExcuteScopedWorkAsync(Func<IServiceProvider, Task> action)
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
                await action(scopedProvider);
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

        public async Task<TResult> ExcuteScopedWorkAsync<TResult>(Func<IServiceProvider, Task<TResult>> func)
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
                return await func(scopedProvider);
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

        public ILogger<T> GetLogger<T>()
        {
            ILoggerFactory factory = GetService<ILoggerFactory>();
            return factory.CreateLogger<T>();
        }

        public ILogger GetLogger(Type type)
        {
            ILoggerFactory factory = GetService<ILoggerFactory>();
            return factory.CreateLogger(type);
        }

        public ILogger GetLogger(string name)
        {
            ILoggerFactory factory = GetService<ILoggerFactory>();
            return factory.CreateLogger(name);
        }

        public ClaimsPrincipal GetCurrentUser()
        {
            try
            {
                IPrincipal user = GetService<IPrincipal>();
                return user as ClaimsPrincipal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetConfiguration(string path)
        {
            IConfiguration config = GetService<IConfiguration>() ?? Singleton<IConfiguration>.Instance;
            return config?[path];
        }

        public void Dispose()
        {
            _services = null;
            _provider = null;
        }
    }
}
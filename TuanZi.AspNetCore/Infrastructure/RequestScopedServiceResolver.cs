using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;


namespace TuanZi.AspNetCore.Infrastructure
{
    public class RequestScopedServiceResolver : IScopedServiceResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestScopedServiceResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool ResolveEnabled => _httpContextAccessor.HttpContext != null;

        public IServiceProvider ScopedProvider
        {
            get { return _httpContextAccessor.HttpContext.RequestServices; }
        }

        public T GetService<T>()
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetService<T>();
        }

        public object GetService(Type serviceType)
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetService(serviceType);
        }

        public IEnumerable<T> GetServices<T>()
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetServices<T>();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetServices(serviceType);
        }
    }
}
using System;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;


namespace TuanZi.AspNetCore
{
    [Dependency(ServiceLifetime.Singleton, ReplaceExisting = true)]
    public class HttpContextServiceScopeFactory : IHybridServiceScopeFactory
    {
        public HttpContextServiceScopeFactory(IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            ServiceScopeFactory = serviceScopeFactory;
            HttpContextAccessor = httpContextAccessor;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        protected IHttpContextAccessor HttpContextAccessor { get; }

        #region Implementation of IServiceScopeFactory

        public virtual IServiceScope CreateScope()
        {
            HttpContext httpContext = HttpContextAccessor?.HttpContext;
            if (httpContext == null)
            {
                return ServiceScopeFactory.CreateScope();
            }

            return new NonDisposedHttpContextServiceScope(httpContext.RequestServices);
        }

        #endregion

        protected class NonDisposedHttpContextServiceScope : IServiceScope
        {
            public NonDisposedHttpContextServiceScope(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get;  }

            public void Dispose()
            { }
        }
    }
}
using System;

using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.Dependency
{
    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    public class DefaultServiceScopeFactory : IHybridServiceScopeFactory
    {
        public DefaultServiceScopeFactory(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
        }
        
        protected IServiceScopeFactory ServiceScopeFactory { get; }

        #region Implementation of IServiceScopeFactory

        public IServiceScope CreateScope()
        {
            return ServiceScopeFactory.CreateScope();
        }

        #endregion
    }
}

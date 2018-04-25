using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.Dependency
{
    public interface IScopedServiceResolver
    {
        bool ResolveEnabled { get; }

        T GetService<T>();

        object GetService(Type serviceType);

        IEnumerable<T> GetServices<T>();

        IEnumerable<object> GetServices(Type serviceType);
    }
}
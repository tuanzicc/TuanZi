using System;

using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.Dependency
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyAttribute : Attribute
    {
        public DependencyAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        public ServiceLifetime Lifetime { get; }

        public bool TryAdd { get; set; }

        public bool ReplaceExisting { get; set; }

        public bool AddSelf { get; set; }
    }
}
using System;

using Microsoft.Extensions.DependencyInjection;


namespace TuanZi.Dependency
{
    internal class Lazier<T> : Lazy<T> where T : class
    {
        public Lazier(IServiceProvider provider)
            : base(provider.GetRequiredService<T>)
        { }
    }
}
using Microsoft.Extensions.DependencyInjection;
using System;

using TuanZi.Dependency;

namespace TuanZi.Core.EntityInfos
{

    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    public class EntityInfoHandler : EntityInfoHandlerBase<EntityInfo, EntityInfoHandler>
    {
        public EntityInfoHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
    }
}
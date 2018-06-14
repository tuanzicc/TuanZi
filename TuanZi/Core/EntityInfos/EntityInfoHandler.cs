using Microsoft.Extensions.Logging;
using System;


namespace TuanZi.Core.EntityInfos
{
    public class EntityInfoHandler : EntityInfoHandlerBase<EntityInfo, EntityInfoHandler>
    {
        public EntityInfoHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        { }
    }
}
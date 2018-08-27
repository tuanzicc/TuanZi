using System;
using System.Collections.Generic;
using TuanZi.Entity;

namespace TuanZi.Core.EntityInfos
{
    public interface IEntityInfo : IEntity<Guid>, IEntityHash
    {
        string Name { get; set; }

        string TypeName { get; set; }

        bool AuditEnabled { get; set; }

        string PropertyJson { get; set; }

        EntityProperty[] Properties { get; }

        void FromType(Type entityType);
    }
}
using System;
using System.Collections.Generic;


namespace TuanZi.Core.EntityInfos
{
    public interface IEntityInfo
    {
        Guid Id { get; set; }

        string Name { get; set; }

        string TypeName { get; set; }

        bool AuditEnabled { get; set; }

        string PropertyNamesJson { get; set; }

        IDictionary<string, string> PropertyNames { get; }

        void FromType(Type entityType);
    }
}
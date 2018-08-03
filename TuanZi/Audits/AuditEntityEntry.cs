using System.Collections.Generic;

using TuanZi.Entity;


namespace TuanZi.Audits
{
    public class AuditEntityEntry
    {
        public AuditEntityEntry()
            : this(null, null, OperateType.Query)
        { }

        public AuditEntityEntry(string name, string typeName, OperateType operateType)
        {
            Name = name;
            TypeName = typeName;
            OperateType = operateType;
            PropertyEntries = new List<AuditPropertyEntry>();
        }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public string EntityKey { get; set; }

        public OperateType OperateType { get; set; }

        public ICollection<AuditPropertyEntry> PropertyEntries { get; set; }

        public object Entity { get; set; }
    }
}
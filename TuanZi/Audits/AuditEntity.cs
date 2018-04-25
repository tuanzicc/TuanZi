using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TuanZi.Properties;


namespace TuanZi.Audits
{
    public class AuditEntity
    {
        public AuditEntity()
            : this(null, null, OperateType.Query)
        { }

        public AuditEntity(string name, string typeName, OperateType operateType)
        {
            Name = name;
            TypeName = typeName;
            OperateType = operateType;
            Properties = new List<AuditEntityProperty>();
        }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public string EntityKey { get; set; }

        public OperateType OperateType { get; set; }

        public ICollection<AuditEntityProperty> Properties { get; set; }
    }


    public enum OperateType
    {
        Query = 0,
        Insert = 1,
        Update = 2,
        Delete = 3
    }
}
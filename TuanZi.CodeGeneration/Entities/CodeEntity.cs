using System;
using System.Collections.Generic;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.CodeGeneration.Entities
{
    public class CodeEntity : EntityBase<Guid>
    {
        public string Name { get; set; }

        public string Display { get; set; }

        public string PrimaryKeyTypeFullName { get; set; }

        public bool IsDataAuth { get; set; }

        public Guid ModuleId { get; set; }

        public virtual CodeModule Module { get; set; }

        public virtual ICollection<CodeProperty> Properties { get; set; } = new List<CodeProperty>();
    }
}
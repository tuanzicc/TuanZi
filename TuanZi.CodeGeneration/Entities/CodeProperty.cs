using System;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.CodeGeneration.Entities
{
    public class CodeProperty : EntityBase<Guid>
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        public string Display { get; set; }

        public bool? IsRequired { get; set; }

        public int? MaxLength { get; set; }

        public int? MinLength { get; set; }

        public object[] Range { get; set; }

        public object Max { get; set; }

        public object Min { get; set; }

        public bool IsNullable { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsForeignKey { get; set; }

        public bool IsInputDto { get; set; } = true;

        public bool IsOutputDto { get; set; } = true;

        public Guid EntityId { get; set; }

        public virtual CodeEntity Entity { get; set; }
    }
}
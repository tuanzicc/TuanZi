using System;
using System.Collections.Generic;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.CodeGeneration.Entities
{
    public class CodeModule : EntityBase<Guid>
    {
        public string Name { get; set; }

        public string Display { get; set; }

        public Guid ProjectId { get; set; }

        public virtual CodeProject Project { get; set; }

        public virtual ICollection<CodeEntity> Entities { get; set; } = new List<CodeEntity>();

        public string Namespace => $"{(Project == null ? "" : Project.NamespacePrefix + ".")}{Name}";
    }
}
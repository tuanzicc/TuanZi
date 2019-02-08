using System;
using System.Collections.Generic;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.CodeGeneration.Entities
{
    public class CodeProject : EntityBase<Guid>
    {
        public string Name { get; set; }

        public string Company { get; set; }

        public string NamespacePrefix { get; set; }

        public string SiteUrl { get; set; }

        public string Creator { get; set; }

        public string Copyright { get; set; }

        public virtual ICollection<CodeModule> Modules { get; set; } = new List<CodeModule>();
    }
}
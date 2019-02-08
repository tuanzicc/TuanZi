using System.Collections.Generic;


namespace TuanZi.CodeGeneration.Schema
{
    public class ProjectMetadata
    {
        public string Name { get; set; }

        public string Company { get; set; }

        public string NamespacePrefix { get; set; }

        public string SiteUrl { get; set; }

        public string Creator { get; set; }

        public string Copyright { get; set; }

        public ICollection<ModuleMetadata> Modules { get; set; } = new List<ModuleMetadata>();
    }
}
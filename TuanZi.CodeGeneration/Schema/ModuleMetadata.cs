using System.Collections.Generic;

using TuanZi.Collections;


namespace TuanZi.CodeGeneration.Schema
{
    public class ModuleMetadata
    {
        private ProjectMetadata _project;

        public ProjectMetadata Project
        {
            get => _project;
            set
            {
                _project = value;
                value.Modules.AddIfNotExist(this);
            }
        }

        public string Name { get; set; }

        public string Display { get; set; }

        public string Namespace => $"{Project?.NamespacePrefix}.{Name}";

        public ICollection<EntityMetadata> Entities { get; set; } = new List<EntityMetadata>();
    }
}
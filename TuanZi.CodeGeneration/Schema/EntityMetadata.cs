using System;
using System.Collections.Generic;
using System.Linq;

using TuanZi.Collections;
using TuanZi.Exceptions;


namespace TuanZi.CodeGeneration.Schema
{
    public class EntityMetadata
    {
        private ModuleMetadata _module;

        public string Name { get; set; }

        public string Display { get; set; }

        public string PrimaryKeyTypeFullName { get; set; }

        public bool IsDataAuth { get; set; }

        public ModuleMetadata Module
        {
            get => _module;
            set
            {
                _module = value;
                value.Entities.AddIfNotExist(this);
            }
        }

        public ICollection<PropertyMetadata> Properties { get; set; } = new List<PropertyMetadata>();
    }
}
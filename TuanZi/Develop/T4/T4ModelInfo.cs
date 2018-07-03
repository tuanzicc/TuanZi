using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using TuanZi.Extensions;
using TuanZi.Reflection;

namespace TuanZi.Develop.T4
{
    public class T4ModelInfo
    {
        public T4ModelInfo(Type modelType, string moduleNamePattern = null)
        {
            modelType.CheckNotNull("modelType");
            string @namespace = modelType.Namespace;
            if (@namespace == null)
            {
                return;
            }
            Namespace = @namespace;
            if (moduleNamePattern != null)
            {
                PackName = @namespace.Match(moduleNamePattern);
            }
            Name = modelType.Name;
            Description = modelType.GetDescription();
            Properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo property = Properties.FirstOrDefault(m => m.HasAttribute<KeyAttribute>())
                ?? Properties.FirstOrDefault(m => m.Name.ToUpper() == "ID")
                    ?? Properties.FirstOrDefault(m => m.Name.ToUpper().EndsWith("ID"));
            if (property != null)
            {
                KeyType = property.PropertyType;
            }
        }

        public Type KeyType { get; private set; }
        
        public string PackName { get; private set; }

        public string Namespace { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IEnumerable<PropertyInfo> Properties { get; private set; }

        public string ProjectName { get; set; }
    }
}
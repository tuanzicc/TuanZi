using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

using TuanZi.Reflection;


namespace TuanZi.CodeGenerator
{
    public class TypeMetadata
    {
        public TypeMetadata()
        { }

        public TypeMetadata(Type type)
        {
            if (type == null)
            {
                return;
            }

            Name = type.Name;
            FullName = type.FullName;
            Namespace = type.Namespace;
            Display = type.GetDescription();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (property.HasAttribute<IgnoreGenPropertyAttribute>())
                {
                    continue;
                }
                if (property.GetMethod.IsVirtual && !property.GetMethod.IsFinal)
                {
                    continue;
                }
                if (PropertyMetadatas == null)
                {
                    PropertyMetadatas = new List<PropertyMetadata>();
                }
                PropertyMetadatas.Add(new PropertyMetadata(property));
            }
        }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Namespace { get; set; }

        public string Display { get; set; }

        public IList<PropertyMetadata> PropertyMetadatas { get; set; }
    }
}
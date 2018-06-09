using System;
using System.Collections.Generic;
using System.Reflection;

namespace TuanZi.Mapping
{
    /// <summary>
    /// Base class for mapping properties to other properties.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class MapPropertyAttribute : Attribute
    {
        public MapPropertyAttribute() {}

        public abstract IEnumerable<PropertyMapInfo> GetPropertyMapInfo(PropertyInfo targetProperty, Type sourceType = null);
    }
}
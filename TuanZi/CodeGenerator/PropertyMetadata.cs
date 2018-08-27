using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using TuanZi.Reflection;


namespace TuanZi.CodeGenerator
{
    public class PropertyMetadata
    {
        public PropertyMetadata()
        { }

        public PropertyMetadata(PropertyInfo property)
        {
            if (property == null)
            {
                return;
            }

            Name = property.Name;
            TypeName = property.PropertyType.FullName;
            Display = property.GetDescription();
            RequiredAttribute required = property.GetAttribute<RequiredAttribute>();
            if (required != null)
            {
                IsRequired = !required.AllowEmptyStrings;
            }
            StringLengthAttribute stringLength = property.GetAttribute<StringLengthAttribute>();
            if (stringLength != null)
            {
                MaxLength = stringLength.MaximumLength;
                MinLength = stringLength.MinimumLength;
            }
            else
            {
                MaxLength = property.GetAttribute<MaxLengthAttribute>()?.Length;
                MinLength = property.GetAttribute<MinLengthAttribute>()?.Length;
            }
            RangeAttribute range = property.GetAttribute<RangeAttribute>();
            if (range != null)
            {
                Range = new[] { range.Minimum, range.Maximum };
                Max = range.Maximum;
                Min = range.Minimum;
            }
            IsNullable = property.PropertyType.IsNullableType();
            if (IsNullable)
            {
                TypeName = property.PropertyType.GetUnNullableType().FullName;
            }
            if (property.PropertyType.IsEnum)
            {
                Type enumType = property.PropertyType;
                Array values = enumType.GetEnumValues();
                Enum[] enumItems = values.Cast<Enum>().ToArray();
                if (enumItems.Length > 0)
                {
                    EnumMetadatas = enumItems.Select(m => new EnumMetadata(m)).ToArray();
                }
            }
        }
        
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

        public EnumMetadata[] EnumMetadatas { get; set; }

        public bool HasValidateAttribute()
        {
            return IsRequired.HasValue || MaxLength.HasValue || MinLength.HasValue || Range != null || Max != null || Min != null;
        }
    }
}
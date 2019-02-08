using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using TuanZi.Collections;
using TuanZi.Reflection;


namespace TuanZi.CodeGeneration.Schema
{
    public class PropertyMetadata
    {
        private EntityMetadata _entity;

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

        public EnumMetadata[] EnumMetadatas { get; set; }

        public EntityMetadata Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                value.Properties.AddIfNotExist(this);
            }
        }

        public bool HasValidateAttribute()
        {
            return IsRequired.HasValue || MaxLength.HasValue || MinLength.HasValue || Range != null || Max != null || Min != null;
        }

        public string ToSingleTypeName()
        {
            return TypeHelper.ToSingleTypeName(TypeName, IsNullable);
        }

        public string ToJsTypeName()
        {
            PropertyMetadata prop = this;
            string name = "object";
            switch (prop.TypeName)
            {
                case "System.Byte":
                case "System.Int32":
                case "System.Int64":
                case "System.Decimal":
                case "System.Single":
                case "System.Double":
                    name = "number";
                    break;
                case "System.String":
                case "System.Guid":
                    name = "string";
                    break;
                case "System.Boolean":
                    name = "boolean";
                    break;
                case "System.DateTime":
                    name = "date";
                    break;
            }
            if (prop.EnumMetadatas != null)
            {
                name = "number";
            }
            return name;
        }
    }
}
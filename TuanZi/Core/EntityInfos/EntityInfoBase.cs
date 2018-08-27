using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using TuanZi.Data;
using TuanZi.Entity;
using TuanZi.Extensions;
using TuanZi.Reflection;


namespace TuanZi.Core.EntityInfos
{
    public abstract class EntityInfoBase : EntityBase<Guid>, IEntityInfo
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string TypeName { get; set; }

        public bool AuditEnabled { get; set; } = true;

        [Required]
        public string PropertyJson { get; set; }

        [NotMapped]
        public EntityProperty[] Properties
        {
            get
            {
                if (string.IsNullOrEmpty(PropertyJson) || !PropertyJson.StartsWith("["))
                {
                    return new EntityProperty[0];
                }
                return PropertyJson.FromJsonString<EntityProperty[]>();
            }
        }

        public virtual void FromType(Type entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            TypeName = entityType.GetFullNameWithModule();
            Name = entityType.GetDescription();
            AuditEnabled = true;

            PropertyInfo[] propertyInfos = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyJson = propertyInfos.Select(property =>
            {
                EntityProperty ep = new EntityProperty()
                {
                    Name = property.Name,
                    Display = property.GetDescription(),
                    TypeName = property.PropertyType.FullName
                };
                if (property.PropertyType.IsEnum)
                {
                    ep.TypeName = typeof(int).FullName;
                    Type enumType = property.PropertyType;
                    Array values = enumType.GetEnumValues();
                    int[] intValues = values.Cast<int>().ToArray();
                    string[] names = values.Cast<Enum>().Select(m => m.ToDescription()).ToArray();
                    for (int i = 0; i < intValues.Length; i++)
                    {
                        string value = intValues[i].ToString();
                        ep.ValueRange.Add(new { id = value, text = names[i] });
                    }
                }
                if (property.HasAttribute<UserFlagAttribute>())
                {
                    ep.IsUserFlag = true;
                }
                return ep;
            }).ToArray().ToJsonString();
        }
    }
}
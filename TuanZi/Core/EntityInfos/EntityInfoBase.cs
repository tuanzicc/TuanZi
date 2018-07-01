using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public string PropertyNamesJson { get; set; }

        public IDictionary<string, string> PropertyNames
        {
            get
            {
                if (PropertyNamesJson.IsNullOrEmpty())
                {
                    return new Dictionary<string, string>();
                }
                return PropertyNamesJson.FromJsonString<Dictionary<string, string>>();
            }
        }

        public virtual void FromType(Type entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            TypeName = entityType.FullName;
            Name = entityType.GetDescription();
            AuditEnabled = true;

            IDictionary<string, string> propertyDict = new Dictionary<string, string>();
            PropertyInfo[] propertyInfos = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                propertyDict.Add(propertyInfo.Name, propertyInfo.GetDescription());
            }
            PropertyNamesJson = propertyDict.ToJsonString();
        }
    }
}
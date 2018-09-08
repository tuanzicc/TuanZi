using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TuanZi.Core.Data;
using TuanZi.Entity;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Json;
using TuanZi.Reflection;


namespace TuanZi.Core.Systems
{
    public class KeyValue : EntityBase<Guid>, ILockable, IKeyValue
    {
        public KeyValue()
        { }

        public KeyValue(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string ValueJson { get; set; }

        public string ValueType { get; set; }

        [Required]
        public string Key { get; set; }

        [NotMapped]
        public object Value
        {
            get
            {
                if (ValueJson == null || ValueType == null)
                {
                    return null;
                }
                Type type = Type.GetType(ValueType);
                if (type == null)
                {
                    throw new TuanException($"Type '{ValueType}' cannot be obtained when getting the dictionary value with Key '{Key}'");
                }
                return ValueJson.FromJsonString(type);
            }
            set
            {
                ValueType = value?.GetType().GetFullNameWithModule();
                ValueJson = value?.ToJsonString();
            }
        }

        public bool IsLocked { get; set; }

        public T GetValue<T>()
        {
            object value = Value;
            if (Equals(value, default(T)))
            {
                return default(T);
            }
            if (value is T)
            {
                return (T)value;
            }
            try
            {
                return value.CastTo<T>();
            }
            catch (Exception)
            {
                throw new TuanException($"The incoming type '{typeof(T)}' does not match the actual data type '{ValueType}' when getting a strongly typed dictionary value");
            }
        }
    }
}
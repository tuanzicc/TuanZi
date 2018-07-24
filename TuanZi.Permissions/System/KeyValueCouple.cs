﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using TuanZi.Core.Data;
using TuanZi.Entity;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Json;


namespace TuanZi.System
{
    public class KeyValueCouple : EntityBase<Guid>, ILockable, IKeyValueCouple
    {
        public KeyValueCouple()
        { }

        public KeyValueCouple(string key, object value)
        {
            Key = key;
            Value = value;
        }

        [Required]
        public string Key { get; set; }
        
        public string ValueJson { get; set; }
        
        public string ValueType { get; set; }
        
        public bool IsLocked { get; set; }

        [NotMapped]
        [JsonIgnore]
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
                ValueType = value?.GetType().ToString();
                ValueJson = value?.ToJsonString();
            }
        }

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
            throw new TuanException($"The incoming type '{typeof(T)}' does not match the actual data type '{ValueType}' when getting a strongly typed dictionary value");
        }
    }
}
using System;
using System.ComponentModel;

using TuanZi.Core.Systems;
using TuanZi.Entity;
using TuanZi.Mapping;


namespace TuanZi.Systems
{
    [MapFrom(typeof(KeyValue))]
    public class KeyValueOutputDto : IOutputDto, IDataAuthEnabled
    {
        public Guid Id { get; set; }

        public string ValueJson { get; set; }

        public string ValueType { get; set; }

        public string Key { get; set; }

        public object Value { get; set; }

        public bool IsLocked { get; set; }

        public bool Updatable { get; set; }

        public bool Deletable { get; set; }
    }
}
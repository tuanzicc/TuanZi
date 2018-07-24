using System;
using System.Collections.Generic;


namespace TuanZi.Core.EntityInfos
{
    public class EntityProperty
    {
        public EntityProperty()
        {
            ValueRange = new List<object>();
        }

        public string Name { get; set; }

        public string Display { get; set; }

        public string TypeName { get; set; }

        public bool IsUserFlag { get; set; }

        public List<object> ValueRange { get; set; }
    }
}
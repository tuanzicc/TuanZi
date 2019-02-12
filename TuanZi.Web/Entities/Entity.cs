using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities
{
    public class Entity : EntityBase<Guid>, ICreatedTime
    {
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

  
}

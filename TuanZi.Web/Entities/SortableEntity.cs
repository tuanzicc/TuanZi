using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities
{
    public class SortableEntity : Entity
    {
        public long Sort { get; set; }
        public SortableEntity()
        {
            Sort = DateTime.Now.Ticks;
        }
    }
}

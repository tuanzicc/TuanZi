using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Entity
{
    public interface IRecyclable
    {
        bool IsDeleted { get; set; }
    }
    public enum RecycleOperation
    {
        LogicDelete,
        Restore,
        PhysicalDelete
    }
}

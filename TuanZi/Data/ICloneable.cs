using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Data
{
    public interface ICloneable<T> : ICloneable
    {
        new T Clone();
    }
}

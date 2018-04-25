using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Dependency
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
    public class IgnoreDependencyAttribute : Attribute
    {
    }
}

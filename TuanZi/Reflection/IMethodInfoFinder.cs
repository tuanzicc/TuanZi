using System;
using System.Reflection;

using TuanZi.Dependency;


namespace TuanZi.Reflection
{
    [IgnoreDependency]
    public interface IMethodInfoFinder
    {
        MethodInfo[] Find(Type type, Func<MethodInfo, bool> predicate);

        MethodInfo[] FindAll(Type type);
    }
}
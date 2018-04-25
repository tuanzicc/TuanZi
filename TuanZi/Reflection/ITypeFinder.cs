using System;
using TuanZi.Dependency;
using TuanZi.Finders;


namespace TuanZi.Reflection
{
    [IgnoreDependency]
    public interface ITypeFinder : IFinder<Type>
    { }
}
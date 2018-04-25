using System;

using TuanZi.Dependency;


namespace TuanZi.Finders
{
    [IgnoreDependency]
    public interface IFinder<out TItem>
    {
        TItem[] Find(Func<TItem, bool> predicate, bool fromCache = false);

        TItem[] FindAll(bool fromCache = false);
    }
}
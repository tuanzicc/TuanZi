using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Finders;
namespace TuanZi.Reflection
{
    [IgnoreDependency]
    public interface IAssemblyFinder : IFinder<Assembly>
    { }
}
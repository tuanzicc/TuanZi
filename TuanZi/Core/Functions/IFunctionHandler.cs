using TuanZi.Dependency;
using TuanZi.Reflection;

namespace TuanZi.Core.Functions
{
    public interface IFunctionHandler
    {
        IFunctionTypeFinder FunctionTypeFinder { get; }

        IMethodInfoFinder MethodInfoFinder { get; }

        void Initialize();

        IFunction GetFunction(string area, string controller, string action);

        void RefreshCache();

        void ClearCache();
    }
}
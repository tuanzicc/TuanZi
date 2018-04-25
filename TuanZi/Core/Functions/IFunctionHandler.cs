using TuanZi.Dependency;


namespace TuanZi.Core.Functions
{
    [IgnoreDependency]
    public interface IFunctionHandler
    {
        void Initialize();

        IFunction GetFunction(string area, string controller, string action);

        void RefreshCache();
    }
}
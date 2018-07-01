using System.Diagnostics;

using TuanZi.Core.Functions;


namespace TuanZi.Core.Modules
{
    [DebuggerDisplay("{ToDebugDisplay()}")]
    public class ModuleInfo
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public double Order { get; set; }

        public string Position { get; set; }

        public IFunction[] DependOnFunctions { get; set; } = new IFunction[0];

        private string ToDebugDisplay()
        {
            return $"{Name}[{Code}]({Position}),FunctionCount:{DependOnFunctions.Length}";
        }
    }
}
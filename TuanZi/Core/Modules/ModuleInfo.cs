using System.Diagnostics;

using TuanZi.Core.Functions;
using TuanZi.Entity;

namespace TuanZi.Core.Modules
{
    [DebuggerDisplay("{ToDebugDisplay()}")]
    public class ModuleInfo : IEntityHash
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public double Order { get; set; }

        public string Position { get; set; }

        public string PositionName { get; set; }

        public IFunction[] DependOnFunctions { get; set; } = new IFunction[0];

        private string ToDebugDisplay()
        {
            return $"{Name}[{Code}]({Position}),FunctionCount:{DependOnFunctions.Length}";
        }

        #region Overrides of Object

        public override bool Equals(object obj)
        {
            if (!(obj is ModuleInfo info))
            {
                return false;
            }
            return $"{info.Position}.{info.Code}" == $"{Position}.{Code}";
        }

        #endregion
    }
}
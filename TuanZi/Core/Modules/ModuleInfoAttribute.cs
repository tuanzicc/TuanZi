using System;


namespace TuanZi.Core.Modules
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ModuleInfoAttribute : Attribute
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public double Order { get; set; }

        public string Position { get; set; }
    }
}
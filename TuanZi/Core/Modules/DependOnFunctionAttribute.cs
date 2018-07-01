using System;


namespace TuanZi.Core.Modules
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class DependOnFunctionAttribute : Attribute
    {
        public DependOnFunctionAttribute(string action)
        {
            Action = action;
        }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; }
    }
}
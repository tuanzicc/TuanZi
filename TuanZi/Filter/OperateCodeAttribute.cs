
using System;


namespace TuanZi.Filter
{
    public class OperateCodeAttribute : Attribute
    {
        public OperateCodeAttribute(string code)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
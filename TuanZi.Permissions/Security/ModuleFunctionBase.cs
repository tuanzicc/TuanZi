using System;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.Security
{
    public abstract class ModuleFunctionBase<TModuleKey> : EntityBase<Guid>
    {
        [DisplayName("Module ID")]
        public TModuleKey ModuleId { get; set; }

        [DisplayName("Function ID")]
        public Guid FunctionId { get; set; }
    }
}
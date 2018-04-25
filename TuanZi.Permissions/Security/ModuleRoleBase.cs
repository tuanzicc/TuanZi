using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using TuanZi.Entity;


namespace TuanZi.Security
{
    public abstract class ModuleRoleBase<TModuleKey, TRoleKey> : EntityBase<Guid>
    {
        [DisplayName("Module ID")]
        public TModuleKey ModuleId { get; set; }

        [DisplayName("Role ID")]
        public TRoleKey RoleId { get; set; }
    }
}

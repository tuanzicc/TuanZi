using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using TuanZi.Entity;


namespace TuanZi.Security
{
    public abstract class ModuleUserBase<TModuleKey, TUserKey> : EntityBase<Guid>
    {
        [DisplayName("Module ID")]
        public TModuleKey ModuleId { get; set; }

        [DisplayName("User ID")]
        public TUserKey UserId { get; set; }

        public bool Disabled { get; set; }
    }
}

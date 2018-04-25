using System;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class UserLoginBase<TUserKey> : EntityBase<Guid>
    {
        [DisplayName("Login Provider")]
        public string LoginProvider { get; set; }

        [DisplayName("Provider Key")]
        public string ProviderKey { get; set; }

        [DisplayName("Provider Display Name")]
        public string ProviderDisplayName { get; set; }

        [DisplayName("User ID")]
        public TUserKey UserId { get; set; }
    }
}
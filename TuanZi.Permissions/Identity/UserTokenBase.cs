using System;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class UserTokenBase<TUserKey> : EntityBase<Guid>
        where TUserKey : IEquatable<TUserKey>
    {
        [DisplayName("User ID")]
        public TUserKey UserId { get; set; }

        [DisplayName("Login Provider")]
        public string LoginProvider { get; set; }

        [DisplayName("Token Name")]
        public string Name { get; set; }

        [DisplayName("Token Value")]
        public string Value { get; set; }
    }
}
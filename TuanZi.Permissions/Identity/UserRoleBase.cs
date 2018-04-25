using System;
using System.ComponentModel;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class UserRoleBase<TUserKey, TRoleKey> : EntityBase<Guid>,ICreatedTime,ILockable
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        protected UserRoleBase()
        {
            CreatedTime = DateTime.Now;
        }

        [DisplayName("User ID")]
        public TUserKey UserId { get; set; }

        [DisplayName("Role ID")]
        public TRoleKey RoleId { get; set; }

        [DisplayName("Created Time")]
        public DateTime CreatedTime { get; set; }

        [DisplayName("Is Locked")]
        public bool IsLocked { get; set; }
    }
}
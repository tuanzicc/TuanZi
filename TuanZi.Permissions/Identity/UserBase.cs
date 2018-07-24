using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class UserBase<TUserKey> : EntityBase<TUserKey>, ICreatedTime, ILockable
        where TUserKey : IEquatable<TUserKey>
    {
        protected UserBase()
        {
            CreatedTime = DateTime.Now;
        }

        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("Normalized UserName")]
        public string NormalizedUserName { get; set; }

        [DisplayName("Nick Name")]
        public string NickName { get; set; }

        [Required]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Normalize Email")]
        public string NormalizeEmail { get; set; }

        [DisplayName("Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [DisplayName("Password Hash")]
        public string PasswordHash { get; set; }

        public string HeadImg { get; set; }

        [DisplayName("Security Stamp")]
        public string SecurityStamp { get; set; }

        [DisplayName("Concurrency Stamp")]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Phone Number Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [DisplayName("Two Factor Enabled")]
        public bool TwoFactorEnabled { get; set; }

        [DisplayName("Lockout End")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [DisplayName("Lockout Enabled")]
        public bool LockoutEnabled { get; set; }

        [DisplayName("Access Failed Count")]
        public int AccessFailedCount { get; set; }

        public bool IsSystem { get; set; }

        public bool IsLocked { get; set; }

        [DisplayName("Created Time")]
        public DateTime CreatedTime { get; set; }

        public override string ToString()
        {
            return UserName;
        }

    }
}
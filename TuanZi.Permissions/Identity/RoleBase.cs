using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class RoleBase<TRoleKey> : EntityBase<TRoleKey>, ICreatedTime, ILockable
        where TRoleKey : IEquatable<TRoleKey>
    {
        protected RoleBase()
        {
            CreatedTime = DateTime.Now;
        }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Normalized Name")]
        public string NormalizedName { get; set; }

        [DisplayName("Concurrency Stamp")]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        [StringLength(512)]
        [DisplayName("Remark")]
        public string Remark { get; set; }

        [DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }

        [DisplayName("Is Default")]
        public bool IsDefault { get; set; }

        [DisplayName("Is System")]
        public bool IsSystem { get; set; }

        [DisplayName("Is Locked")]
        public bool IsLocked { get; set; }

        [DisplayName("Created Time")]
        public DateTime CreatedTime { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
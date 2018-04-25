using System.ComponentModel.DataAnnotations;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class RoleInputDtoBase<TRoleKey> : IInputDto<TRoleKey>
    {
        public TRoleKey Id { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(512)]
        public string Remark { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsDefault { get; set; }

        public bool IsLocked { get; set; }
    }
}
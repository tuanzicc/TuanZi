using System.ComponentModel.DataAnnotations;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class UserInputDtoBase<TUserKey> : IInputDto<TUserKey>
    {
        public TUserKey Id { get; set; }

        [Required]
        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }

        public bool IsLocked { get; set; }
    }
}


using System;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class UserRoleInputDtoBase<TUserKey, TRoleKey> : IInputDto<Guid>
    {
        public TUserKey UserId { get; set; }
        
        public TRoleKey RoleId { get; set; }
        
        public Guid Id { get; set; }
    }
}
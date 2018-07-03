using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;


namespace TuanZi.Identity
{
    public class UserNickNameValidator<TUser, TUserKey> : IUserValidator<TUser>
        where TUser : UserBase<TUserKey>
        where TUserKey : struct, IEquatable<TUserKey>
    {
        public virtual Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            IdentityResult result = IdentityResult.Success;
            TUser existUser = manager.Users.FirstOrDefault(m => m.NickName == user.NickName);
            if (existUser != null
                && (Equals(user.Id, default(TUserKey)) 
                || !Equals(user.Id, existUser.Id)))
            {
                result = new IdentityResult().Failed($"'{user.NickName}' alreay taken");
            }
            return Task.FromResult(result);
        }
    }
}
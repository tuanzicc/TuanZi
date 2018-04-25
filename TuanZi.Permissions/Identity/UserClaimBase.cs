using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class UserClaimBase<TUserKey> : EntityBase<int>
        where TUserKey : IEquatable<TUserKey>
    {
        [DisplayName("User ID")]
        public TUserKey UserId { get; set; }

        [Required]
        [DisplayName("Claim Type")]
        public string ClaimType { get; set; }

        [DisplayName("Claim Value")]
        public string ClaimValue { get; set; }

        public virtual Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }

        public virtual void InitializeFromClaim(Claim other)
        {
            ClaimType = other?.Type;
            ClaimValue = other?.Value;
        }
    }
}
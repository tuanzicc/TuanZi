using System;
using System.ComponentModel;
using System.Security.Claims;

using TuanZi.Entity;


namespace TuanZi.Identity
{
    public abstract class RoleClaimBase<TRoleKey> : EntityBase<int>
        where TRoleKey : IEquatable<TRoleKey>
    {
        [DisplayName("Role ID")]
        public TRoleKey RoleId { get; set; }

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
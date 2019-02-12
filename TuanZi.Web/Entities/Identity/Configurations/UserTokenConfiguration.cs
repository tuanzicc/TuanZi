using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class UserTokenConfiguration : EntityTypeConfigurationBase<UserToken, Guid>
    {
        public override void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.HasIndex(m => new { m.UserId, m.LoginProvider, m.Name }).HasName("UserTokenIndex").IsUnique();
        }
    }
}

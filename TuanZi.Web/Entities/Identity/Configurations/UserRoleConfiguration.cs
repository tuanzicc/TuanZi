using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class UserRoleConfiguration : EntityTypeConfigurationBase<UserRole, Guid>
    {
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasIndex(m => new { m.UserId, m.RoleId }).HasName("UserRoleIndex").IsUnique();
        }
    }
}

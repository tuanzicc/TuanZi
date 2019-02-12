using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class RoleConfiguration : EntityTypeConfigurationBase<Role, Guid>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasIndex(m => m.NormalizedName).HasName("RoleNameIndex").IsUnique();

            builder.Property(m => m.ConcurrencyStamp).IsConcurrencyToken();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class UserConfiguration : EntityTypeConfigurationBase<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(m => m.NormalizedUserName).HasName("UserNameIndex").IsUnique();
            //builder.HasIndex(m => m.NormalizeEmail).HasName("EmailIndex");
            builder.Property(m => m.ConcurrencyStamp).IsConcurrencyToken();

            builder.HasMany<File>().WithOne().HasForeignKey(m => m.UserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

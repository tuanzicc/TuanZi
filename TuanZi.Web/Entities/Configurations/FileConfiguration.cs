using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class FileConfiguration : EntityTypeConfigurationBase<File, Guid>
    {
        public override void Configure(EntityTypeBuilder<File> builder)
        {
            builder.HasMany<App>().WithOne().HasForeignKey(m => m.Logo).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany<Link>().WithOne().HasForeignKey(m => m.Image).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany<Site>().WithOne().HasForeignKey(m => m.Logo).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany<Theme>().WithOne().HasForeignKey(m => m.Photo).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

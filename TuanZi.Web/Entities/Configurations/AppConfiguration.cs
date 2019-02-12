using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class AppConfiguration : EntityTypeConfigurationBase<App, Guid>
    {
        public override void Configure(EntityTypeBuilder<App> builder)
        {
            //builder.HasMany<AnalyticLink>().WithOne().HasForeignKey(m => m.AppId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            //builder.HasMany<AnalyticRequest>().WithOne().HasForeignKey(m => m.AppId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany<File>().WithOne().HasForeignKey(m => m.AppId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany<Link>().WithOne().HasForeignKey(m => m.AppId).IsRequired().OnDelete(DeleteBehavior.Restrict);
           
            builder.HasMany<Inquiry>().WithOne().HasForeignKey(m => m.AppId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany<Theme>().WithOne().HasForeignKey(m => m.AppId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

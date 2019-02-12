using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class SiteConfiguration : EntityTypeConfigurationBase<Site, Guid>
    {
        public override void Configure(EntityTypeBuilder<Site> builder)
        {
            //builder.HasMany<AnalyticLink>().WithOne().HasForeignKey(m => m.SiteId).IsRequired();

        }
    } 
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class IpLocationConfiguration : EntityTypeConfigurationBase<IpLocation, int>
    {
        public override void Configure(EntityTypeBuilder<IpLocation> builder)
        {
            builder.HasIndex(m => m.Ip);
        }
    }
}

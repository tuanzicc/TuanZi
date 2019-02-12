using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class HitConfiguration : EntityTypeConfigurationBase<Hit, int>
    {
        public override void Configure(EntityTypeBuilder<Hit> builder)
        {
            builder.HasIndex(m => m.Ip);
            builder.HasIndex(m => m.Browser);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class VisitConfiguration : EntityTypeConfigurationBase<Visit, int>
    {
        public override void Configure(EntityTypeBuilder<Visit> builder)
        {
            builder.HasIndex(m => m.Ip);
            builder.HasIndex(m => m.Browser);
            builder.HasIndex(m => m.RefererHost);
            builder.HasIndex(m => m.Host);
            builder.HasIndex(m => m.Country);
            builder.HasIndex(m => m.Province);
            builder.HasIndex(m => m.City);
        }
    }
}

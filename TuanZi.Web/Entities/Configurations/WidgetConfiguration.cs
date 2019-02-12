using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class WidgetConfiguration : EntityTypeConfigurationBase<Widget, Guid>
    {
        public override void Configure(EntityTypeBuilder<Widget> builder)
        {
           // builder.HasMany<AnalyticLink>().WithOne().HasForeignKey(m => m.WidgetId).IsRequired();
            //builder.HasMany<WidgetLink>().WithOne().HasForeignKey(m => m.WidgetId).IsRequired();
        }
    } 
}

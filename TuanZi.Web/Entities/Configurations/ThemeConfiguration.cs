using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class ThemeConfiguration : EntityTypeConfigurationBase<Theme, Guid>
    {
        public override void Configure(EntityTypeBuilder<Theme> builder)
        {
        }
    } 
}

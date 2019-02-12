using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class TagConfiguration : EntityTypeConfigurationBase<Tag, Guid>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasIndex(m => m.Name);
        }
    }
}

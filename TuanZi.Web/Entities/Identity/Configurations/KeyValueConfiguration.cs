using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Core.Systems;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class KeyValueConfiguration : EntityTypeConfigurationBase<KeyValue, Guid>
    {
        public override void Configure(EntityTypeBuilder<KeyValue> builder)
        {
            builder.HasData(
                new KeyValue() { Key = SystemSettingKeys.SiteName, Value = "Tuan" },
                new KeyValue() { Key = SystemSettingKeys.SiteDescription, Value = "Tuan with .NetStandard2.0" }
            );
        }
    }
}

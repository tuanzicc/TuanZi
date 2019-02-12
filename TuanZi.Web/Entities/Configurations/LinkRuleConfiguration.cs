using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class LinkRuleConfiguration : EntityTypeConfigurationBase<LinkRule, Guid>
    {
        public override void Configure(EntityTypeBuilder<LinkRule> builder)
        {
        }
    } 
}

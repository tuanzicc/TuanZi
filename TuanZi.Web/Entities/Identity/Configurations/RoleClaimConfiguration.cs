using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class RoleClaimConfiguration : EntityTypeConfigurationBase<RoleClaim, int>
    {
        public override void Configure(EntityTypeBuilder<RoleClaim> builder)
        {

        }
    }
}

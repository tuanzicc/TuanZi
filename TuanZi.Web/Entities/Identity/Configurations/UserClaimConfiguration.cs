using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Entity;

namespace FuqLink.Entities.Configurations
{
    public class UserClaimConfiguration : EntityTypeConfigurationBase<UserClaim, int>
    {
        public override void Configure(EntityTypeBuilder<UserClaim> builder)
        {

        }
    }
}

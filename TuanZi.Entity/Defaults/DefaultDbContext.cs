﻿using Microsoft.EntityFrameworkCore;


namespace TuanZi.Entity
{
    public class DefaultDbContext : DbContextBase<DefaultDbContext>
    {
        public DefaultDbContext(DbContextOptions options, IEntityConfigurationTypeFinder typeFinder)
            : base(options, typeFinder)
        {
            
        }
    }
}
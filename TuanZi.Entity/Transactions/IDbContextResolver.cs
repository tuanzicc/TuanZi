using System;

using Microsoft.EntityFrameworkCore;

using TuanZi.Entity.Transactions;


namespace TuanZi.Entity
{
    public interface IDbContextResolver
    {
        IDbContext Resolve(DbContextResolveOptions resolveOptions);
    }
}
using System;
using System.Data.Common;

using Microsoft.EntityFrameworkCore;

using TuanZi.Entity.Transactions;


namespace TuanZi.Entity
{
    public interface IDbContextResolver
    {
        DbContext Resolve(DbContextResolveOptions resolveOptions);
    }
}
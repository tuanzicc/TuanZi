using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using TuanZi.Audits;
using TuanZi.Collections;
using TuanZi.Core;
using TuanZi.Core.EntityInfos;
using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Exceptions;

namespace TuanZi.Entity
{
    public static partial class DbContextExtensions
    {
      

        public static int ExecuteSqlCommand(this IDbContext dbContext,int? timeout, string sql, params object[] parameters)
        {
            if (!(dbContext is DbContext context))
            {
                throw new TuanException($"The parameter dbContext is type of '{dbContext.GetType()}' and cannot be converted to DbContext");
            }
            context.Database.SetCommandTimeout(timeout);
            return context.Database.ExecuteSqlCommand(new RawSqlString(sql), parameters);
        }

        public static Task<int> ExecuteSqlCommandAsync(this IDbContext dbContext, int? timeout, string sql, params object[] parameters)
        {
            if (!(dbContext is DbContext context))
            {
                throw new TuanException($"The parameter dbContext is type of '{dbContext.GetType()}' and cannot be converted to DbContext");
            }
            context.Database.SetCommandTimeout(timeout);
            return context.Database.ExecuteSqlCommandAsync(new RawSqlString(sql), parameters);
        }

     
    }
}
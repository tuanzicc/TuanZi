using Microsoft.EntityFrameworkCore;
using System;

using TuanZi.Reflection;


namespace TuanZi.Entity
{
    public static partial class EntityInterfaceExtensions
    {
       

        public static TEntity CheckIgnoreEntry<TEntity, TKey>(this TEntity entity, DbContext dbContext)
            where TEntity : class
            where TKey : IEquatable<TKey>
        {
            var properties = entity.GetType().GetProperties();

            if (entity is ICreatedTime)
            {
                dbContext.Entry(entity).Property(nameof(ICreatedTime.CreatedTime)).IsModified = false;
            }

            return entity;
        }

       
    }
}
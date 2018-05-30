﻿using System;

using TuanZi.Reflection;


namespace TuanZi.Entity
{
    public static partial class EntityInterfaceExtensions
    {
        public static TEntity CheckICreatedTime<TEntity, TKey>(this TEntity entity)
            where TEntity : IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (!(entity is ICreatedTime))
            {
                return entity;
            }
            ICreatedTime entity1 = (ICreatedTime)entity;
            entity1.CreatedTime = DateTime.Now;
            return (TEntity)entity1;
        }


        public static bool IsEntityType(this Type type)
        {
            Check.NotNull(type, nameof(type));
            return typeof(IEntity<>).IsGenericAssignableFrom(type) && !type.IsAbstract && !type.IsInterface;
        }

        public static bool IsExpired(this IExpirable entity)
        {
            Check.NotNull(entity, nameof(entity));
            DateTime now = DateTime.Now;
            return entity.BeginTime != null && entity.BeginTime.Value > now || entity.EndTime != null && entity.EndTime.Value < now;
        }

        public static TEntity CheckIRecycle<TEntity, TKey>(this TEntity entity, RecycleOperation operation)
            where TEntity : IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (!(entity is IRecyclable))
            {
                return entity;
            }
            IRecyclable entity1 = entity as IRecyclable;
            switch (operation)
            {
                case RecycleOperation.LogicDelete:
                    entity1.IsDeleted = true;
                    break;
                case RecycleOperation.Restore:
                    entity1.IsDeleted = false;
                    break;
                case RecycleOperation.PhysicalDelete:
                    if (!entity1.IsDeleted)
                    {
                        throw new InvalidOperationException("The data is not in the recycle (IsDeleted=true) state and cannot be permanently deleted");
                    }
                    break;
            }
            return (TEntity)entity1;
        }
    }
}
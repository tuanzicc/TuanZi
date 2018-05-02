using System;

using TuanZi.Reflection;


namespace TuanZi.Entity
{
    public static class EntityInterfaceExtensions
    {
        public static TEntity CheckICreatedTime<TEntity, TKey>(this TEntity entity)
            where TEntity : class, ICreatedTime
            where TKey : IEquatable<TKey>
        {
            Check.NotNull(entity, nameof(entity));
            
            entity.CreatedTime = DateTime.Now;
            return entity;
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
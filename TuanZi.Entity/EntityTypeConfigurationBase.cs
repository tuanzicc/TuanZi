using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace TuanZi.Entity
{
    public abstract class EntityTypeConfigurationBase<TEntity, TKey> : IEntityTypeConfiguration<TEntity>, IEntityRegister
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual Type DbContextType => null;

        public Type EntityType => typeof(TEntity);

        public void RegistTo(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }

        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
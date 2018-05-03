using System;
using System.Linq;
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
            //set default DeleteBehavior to Restrict
            //foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            //set default DeleteBehavior to Restrict
            //var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(m => !m.IsOwnership && m.DeleteBehavior == DeleteBehavior.Cascade);

            //foreach (var relationship in relationships)
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            modelBuilder.ApplyConfiguration(this);
        }

        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
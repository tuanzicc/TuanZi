using System;
using System.ComponentModel;

using TuanZi.Data;


namespace TuanZi.Entity
{
    public abstract class EntityBase<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected EntityBase()
        {
            if (typeof(TKey) == typeof(Guid))
            {
                Id = CombGuid.NewGuid().CastTo<TKey>();
            }
        }

        [DisplayName("ID")]
        public TKey Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            EntityBase<TKey> entity = obj as EntityBase<TKey>;
            if (entity == null)
            {
                return false;
            }
            return entity.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            if (Id == null)
            {
                return 0;
            }
            return Id.ToString().GetHashCode();
        }
    }
}
using System;
using System.ComponentModel;

using TuanZi.Data;
using TuanZi.Extensions;
using TuanZi.Reflection;

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
            if (!(obj is EntityBase<TKey> entity))
            {
                return false;
            }
            return IsKeyEqual(entity.Id, Id);
        }

        public static bool IsKeyEqual(TKey id1, TKey id2)
        {
            if (id1 == null && id2 == null)
            {
                return true;
            }
            if (id1 == null || id2 == null)
            {
                return false;
            }

            Type type = typeof(TKey);
            if (type.IsDeriveClassFrom(typeof(IEquatable<TKey>)))
            {
                return id1.Equals(id2);
            }
            return Equals(id1, id2);
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
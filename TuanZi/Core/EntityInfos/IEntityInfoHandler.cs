using System;

using TuanZi.Entity;


namespace TuanZi.Core.EntityInfos
{
    public interface IEntityInfoHandler
    {
        void Initialize();

        IEntityInfo GetEntityInfo(Type type);

        IEntityInfo GetEntityInfo<TEntity, TKey>() where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>;

        void RefreshCache();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TuanZi.Exceptions;
using TuanZi.Finders;
using TuanZi.Core;
using TuanZi.Core.EntityInfos;
using TuanZi.Core.Functions;
using TuanZi.Reflection;


namespace TuanZi.Entity
{
    
    public class EntityConfigurationTypeFinder : BaseTypeFinderBase<IEntityRegister>, IEntityConfigurationTypeFinder
    {
        private Dictionary<Type, IEntityRegister[]> _entityRegistersDict;

        public EntityConfigurationTypeFinder(IAllAssemblyFinder allAssemblyFinder)
            : base(allAssemblyFinder)
        { }

        protected Dictionary<Type, IEntityRegister[]> EntityRegistersDict
        {
            get
            {
                if (_entityRegistersDict == null)
                {
                    Type[] types = FindAll(true);
                    EntityRegistersInit(types);
                }

                return _entityRegistersDict;
            }
        }

        public IEntityRegister[] GetEntityRegisters(Type dbContextType)
        {
            return EntityRegistersDict.ContainsKey(dbContextType) ? EntityRegistersDict[dbContextType] : new IEntityRegister[0];
        }

        public Type GetDbContextTypeForEntity(Type entityType)
        {
            var dict = EntityRegistersDict;
            if (dict.Count == 0)
            {
                throw new TuanException($"No dbcontext entity mapping configuration was found. Please inherit the base class 'EntityTypeConfigurationBase<TEntity, TKey>' for each entity to load the entity into the context.");
            }
            foreach (var item in EntityRegistersDict)
            {
                if (item.Value.Any(m => m.EntityType == entityType))
                {
                    return item.Key;
                }
            }
            throw new TuanException($"Failed to get the context type of the entity class '{entityType}', please load it into the context by inheriting the base class 'EntityTypeConfigurationBase<TEntity, TKey>'");
        }

        private void EntityRegistersInit(Type[] types)
        {
            if (types.Length == 0)
            {
                if (_entityRegistersDict == null)
                {
                    _entityRegistersDict = new Dictionary<Type, IEntityRegister[]>();
                }
                return;
            }
            List<IEntityRegister> registers = types.Select(type => Activator.CreateInstance(type) as IEntityRegister).ToList();
            Dictionary<Type, IEntityRegister[]> dict = new Dictionary<Type, IEntityRegister[]>();
            List<IGrouping<Type, IEntityRegister>> groups = registers.GroupBy(m => m.DbContextType).ToList();
            Type key;
            foreach (IGrouping<Type, IEntityRegister> group in groups)
            {
                key = group.Key ?? typeof(DefaultDbContext);
                List<IEntityRegister> list = new List<IEntityRegister>();
                if (group.Key == null || group.Key == typeof(DefaultDbContext))
                {
                    list.AddRange(group);
                }
                else
                {
                    list = group.ToList();
                }
                if (list.Count > 0)
                {
                    dict[key] = list.ToArray();
                }
            }
            key = typeof(DefaultDbContext);
            if (dict.ContainsKey(key))
            {
                List<IEntityRegister> list = dict[key].ToList();
                if (!list.Any(m => m.EntityType.IsBaseOn<IEntityInfo>()))
                {
                    list.Add(new EntityInfoConfiguration());
                }
                if (!list.Any(m => m.EntityType.IsBaseOn<IFunction>()))
                {
                    list.Add(new FunctionConfiguration());
                }

                dict[key] = list.ToArray();
            }

            _entityRegistersDict = dict;
        }


        private class EntityInfoConfiguration : EntityTypeConfigurationBase<EntityInfo, Guid>
        {
            public override void Configure(EntityTypeBuilder<EntityInfo> builder)
            {
                builder.HasIndex(m => m.TypeName).HasName("ClassFullNameIndex").IsUnique();
            }
        }


        private class FunctionConfiguration : EntityTypeConfigurationBase<Function, Guid>
        {
            public override void Configure(EntityTypeBuilder<Function> builder)
            {
                builder.HasIndex(m => new { m.Area, m.Controller, m.Action }).HasName("AreaControllerActionIndex").IsUnique();
            }
        }
    }
}
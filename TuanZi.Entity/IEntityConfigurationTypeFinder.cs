using System;

using TuanZi.Dependency;
using TuanZi.Reflection;


namespace TuanZi.Entity
{
    public interface IEntityConfigurationTypeFinder : ITypeFinder
    {
        IEntityRegister[] GetEntityRegisters(Type dbContextType);

        Type GetDbContextTypeForEntity(Type entityType);
    }
}
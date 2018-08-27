using System;
using System.Linq;

using TuanZi.Entity;
using TuanZi.Reflection;


namespace TuanZi.CodeGenerator
{
    public class TypeMetadataHandler : ITypeMetadataHandler
    {
        private readonly IEntityTypeFinder _entityTypeFinder;
        private readonly IInputDtoTypeFinder _inputDtoTypeFinder;
        private readonly IOutputDtoTypeFinder _outputDtoTypeFinder;

        public TypeMetadataHandler(IEntityTypeFinder entityTypeFinder,
            IInputDtoTypeFinder inputDtoTypeFinder,
            IOutputDtoTypeFinder outputDtoTypeFinder)
        {
            _entityTypeFinder = entityTypeFinder;
            _inputDtoTypeFinder = inputDtoTypeFinder;
            _outputDtoTypeFinder = outputDtoTypeFinder;
        }

        public TypeMetadata[] GetEntityTypeMetadatas()
        {
            Type[] entityTypes = _entityTypeFinder.Find(m => !m.HasAttribute<IgnoreGenTypeAttribute>());
            return entityTypes.Select(m => new TypeMetadata(m)).ToArray();
        }

        public TypeMetadata[] GetInputDtoMetadatas()
        {
            Type[] inputDtoTypes = _inputDtoTypeFinder.Find(m => !m.HasAttribute<IgnoreGenTypeAttribute>());
            return inputDtoTypes.Select(m => new TypeMetadata(m)).ToArray();
        }

        public TypeMetadata[] GetOutputDtoMetadata()
        {
            Type[] outDtoTypes = _outputDtoTypeFinder.Find(m => !m.HasAttribute<IgnoreGenTypeAttribute>());
            return outDtoTypes.Select(m => new TypeMetadata(m)).ToArray();
        }

        public TypeMetadata GetTypeMetadata(Type type)
        {
            if (type == null)
            {
                return null;
            }
            return new TypeMetadata(type);
        }
    }
}
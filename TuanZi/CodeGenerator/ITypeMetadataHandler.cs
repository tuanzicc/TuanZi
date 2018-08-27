using System;


namespace TuanZi.CodeGenerator
{
    public interface ITypeMetadataHandler
    {
        TypeMetadata[] GetEntityTypeMetadatas();

        TypeMetadata[] GetInputDtoMetadatas();

        TypeMetadata[] GetOutputDtoMetadata();

        TypeMetadata GetTypeMetadata(Type type);
    }
}
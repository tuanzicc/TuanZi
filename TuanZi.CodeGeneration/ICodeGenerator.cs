using TuanZi.CodeGeneration.Schema;


namespace TuanZi.CodeGeneration
{
    public interface ICodeGenerator
    {
        CodeFile[] GenerateProjectCode(ProjectMetadata project);

        CodeFile GenerateEntityCode(EntityMetadata entity);

        CodeFile GenerateInputDtoCode(EntityMetadata entity);

        CodeFile GenerateOutputDtoCode(EntityMetadata entity);

        CodeFile GenerateServiceContract(ModuleMetadata module);

        CodeFile GenerateServiceMainImpl(ModuleMetadata module);

        CodeFile GenerateServiceEntityImpl(EntityMetadata entity);

        CodeFile GenerateEntityConfiguration(EntityMetadata entity);

        CodeFile GenerateAdminController(EntityMetadata entity);
    }
}
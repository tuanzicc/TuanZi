using TuanZi.CodeGeneration.Schema;


namespace TuanZi.CodeGeneration
{
    public class GenerateContext
    {
        public EntityMetadata[] EntityMetadatas { get; set; } = new EntityMetadata[0];

        public ProjectMetadata Project { get; set; }

        public ModuleMetadata Module { get; set; }

        public string Template { get; set; }

        public CodeType CodeType { get; set; } = CodeType.Entity;
    }
}
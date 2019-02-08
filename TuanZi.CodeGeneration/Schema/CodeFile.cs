using TuanZi.Extensions;


namespace TuanZi.CodeGeneration.Schema
{
    public class CodeFile
    {
        private string _sourceCode;

        public string SourceCode
        {
            get => _sourceCode;
            set => _sourceCode = value.ToHtmlDecode();
        }

        public string FileName { get; set; }
    }
}
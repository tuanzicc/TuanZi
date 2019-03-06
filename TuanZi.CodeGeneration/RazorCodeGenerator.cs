using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TuanZi.CodeGeneration.Schema;
using TuanZi.Data;
using TuanZi.Exceptions;
using TuanZi.Extensions;

using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;


namespace TuanZi.CodeGeneration
{
    public class RazorCodeGenerator : ICodeGenerator
    {
        private readonly IDictionary<CodeType, ITemplateKey> _keyDict = new ConcurrentDictionary<CodeType, ITemplateKey>();

        public virtual CodeFile[] GenerateProjectCode(ProjectMetadata project)
        {
            List<CodeFile> codeFiles = new List<CodeFile>();
            ModuleMetadata[] modules = project.Modules.ToArray();
            EntityMetadata[] entities = modules.SelectMany(m => m.Entities).ToArray();

            codeFiles.AddRange(entities.Select(GenerateEntityCode));
            codeFiles.AddRange(entities.Select(GenerateInputDtoCode));
            codeFiles.AddRange(entities.Select(GenerateOutputDtoCode));
            codeFiles.AddRange(entities.Select(GenerateServiceEntityImpl));
            codeFiles.AddRange(entities.Select(GenerateEntityConfiguration));
            codeFiles.AddRange(entities.Select(GenerateAdminController));

            codeFiles.AddRange(modules.Select(GenerateServiceContract));
            codeFiles.AddRange(modules.Select(GenerateServiceMainImpl));

            return codeFiles.OrderBy(m => m.FileName).ToArray();
        }

        public virtual CodeFile GenerateEntityCode(EntityMetadata entity)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.Entity, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.Entity);
                key = GetKey(CodeType.Entity, template);
                code = Engine.Razor.RunCompile(template, key, entity.GetType(), entity);
                _keyDict.Add(CodeType.Entity, key);
            }
            else
            {
                code = Engine.Razor.Run(key, entity.GetType(), entity);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{entity.Module.Project.NamespacePrefix}.Core/{entity.Module.Name}/Entities/{entity.Name}.generated.cs"
            };
        }

        public virtual CodeFile GenerateInputDtoCode(EntityMetadata entity)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.InputDto, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.InputDto);
                key = GetKey(CodeType.InputDto, template);
                code = Engine.Razor.RunCompile(template, key, entity.GetType(), entity);
                _keyDict.Add(CodeType.InputDto, key);
            }
            else
            {
                code = Engine.Razor.Run(key, entity.GetType(), entity);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{entity.Module.Project.NamespacePrefix}.Core/{entity.Module.Name}/Dtos/{entity.Name}InputDto.generated.cs"
            };
        }

        public virtual CodeFile GenerateOutputDtoCode(EntityMetadata entity)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.OutputDto, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.OutputDto);
                key = GetKey(CodeType.OutputDto, template);
                code = Engine.Razor.RunCompile(template, key, entity.GetType(), entity);
                _keyDict.Add(CodeType.OutputDto, key);
            }
            else
            {
                code = Engine.Razor.Run(key, entity.GetType(), entity);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{entity.Module.Project.NamespacePrefix}.Core/{entity.Module.Name}/Dtos/{entity.Name}OutputDto.generated.cs"
            };
        }

        public virtual CodeFile GenerateServiceContract(ModuleMetadata module)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.ServiceContract, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.ServiceContract);
                key = GetKey(CodeType.ServiceContract, template);
                code = Engine.Razor.RunCompile(template, key, module.GetType(), module);
                _keyDict.Add(CodeType.ServiceContract, key);
            }
            else
            {
                code = Engine.Razor.Run(key, module.GetType(), module);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{module.Project.NamespacePrefix}.Core/{module.Name}/I{module.Name}Contract.generated.cs"
            };
        }

        public virtual CodeFile GenerateServiceMainImpl(ModuleMetadata module)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.ServiceMainImpl, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.ServiceMainImpl);
                key = GetKey(CodeType.ServiceMainImpl, template);
                code = Engine.Razor.RunCompile(template, key, module.GetType(), module);
                _keyDict.Add(CodeType.ServiceMainImpl, key);
            }
            else
            {
                code = Engine.Razor.Run(key, module.GetType(), module);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{module.Project.NamespacePrefix}.Core/{module.Name}/{module.Name}Service.generated.cs"
            };
        }

        public virtual CodeFile GenerateServiceEntityImpl(EntityMetadata entity)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.ServiceEntityImpl, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.ServiceEntityImpl);
                key = GetKey(CodeType.ServiceEntityImpl, template);
                code = Engine.Razor.RunCompile(template, key, entity.GetType(), entity);
                _keyDict.Add(CodeType.ServiceEntityImpl, key);
            }
            else
            {
                code = Engine.Razor.Run(key, entity.GetType(), entity);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{entity.Module.Project.NamespacePrefix}.Core/{entity.Module.Name}/{entity.Module.Name}Service.{entity.Name}.generated.cs"
            };
        }

        public virtual CodeFile GenerateEntityConfiguration(EntityMetadata entity)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.EntityConfiguration, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.EntityConfiguration);
                key = GetKey(CodeType.EntityConfiguration, template);
                code = Engine.Razor.RunCompile(template, key, entity.GetType(), entity);
                _keyDict.Add(CodeType.EntityConfiguration, key);
            }
            else
            {
                code = Engine.Razor.Run(key, entity.GetType(), entity);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{entity.Module.Project.NamespacePrefix}.EntityConfiguration/{entity.Module.Name}/{entity.Name}Configuration.generated.cs"
            };
        }

        public virtual CodeFile GenerateAdminController(EntityMetadata entity)
        {
            string code;
            if (!_keyDict.TryGetValue(CodeType.AdminController, out ITemplateKey key))
            {
                string template = GetInternalTemplate(CodeType.AdminController);
                key = GetKey(CodeType.AdminController, template);
                code = Engine.Razor.RunCompile(template, key, entity.GetType(), entity);
                _keyDict.Add(CodeType.AdminController, key);
            }
            else
            {
                code = Engine.Razor.Run(key, entity.GetType(), entity);
            }
            return new CodeFile()
            {
                SourceCode = code,
                FileName = $"{entity.Module.Project.NamespacePrefix}.Web/Areas/Admin/Controllers/{entity.Module.Name}/{entity.Name}Controller.generated.cs"
            };
        }

        private ITemplateKey GetKey(CodeType codeType, string template)
        {
            Check.NotNull(template, nameof(template));

            string md5 = template.ToMd5Hash();
            string name = $"{codeType.ToString()}-{md5}";
            return Engine.Razor.GetKey(name);
        }

        private string GetInternalTemplate(CodeType type)
        {
            string resName = $"TuanZi.CodeGeneration.Templates.{type.ToString()}.cshtml";
            Stream stream = GetType().Assembly.GetManifestResourceStream(resName);
            if (stream == null)
            {
                throw new TuanException($"Could not find built-in code template for '{type.ToString()}'");
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
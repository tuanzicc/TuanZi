using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuanZi.Collections;
using TuanZi.Core.Modules;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Exceptions;


namespace TuanZi.Security
{
    public abstract class ModuleHandlerBase<TModule, TModuleInputDto, TModuleKey, TModuleFunction> : IModuleHandler
       where TModule : ModuleBase<TModuleKey>
       where TModuleInputDto : ModuleInputDtoBase<TModuleKey>, new()
       where TModuleKey : struct, IEquatable<TModuleKey>
       where TModuleFunction : ModuleFunctionBase<TModuleKey>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IModuleInfoPicker _moduleInfoPicker;

        protected ModuleHandlerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _moduleInfoPicker = serviceProvider.GetService<IModuleInfoPicker>();
            Logger = serviceProvider.GetLogger(GetType());
        }

        protected ILogger Logger { get; }

        public void Initialize()
        {
            ModuleInfo[] moduleInfos = _moduleInfoPicker.Pickup();
            if (moduleInfos.Length == 0)
            {
                return;
            }
            _serviceProvider.ExecuteScopedWork(provider =>
            {
                SyncToDatabase(provider, moduleInfos);
            });
        }

        protected virtual void SyncToDatabase(IServiceProvider provider, ModuleInfo[] moduleInfos)
        {
            Check.NotNull(moduleInfos, nameof(moduleInfos));
            if (moduleInfos.Length == 0)
            {
                return;
            }

            IModuleStore<TModule, TModuleInputDto, TModuleKey> moduleStore =
                provider.GetService<IModuleStore<TModule, TModuleInputDto, TModuleKey>>();
            if (moduleStore == null)
            {
                Logger.LogWarning("The service of IRepository<,> is not found, please initialize the Entity Pack module.");
                return;
            }

            IModuleFunctionStore<TModuleFunction, TModuleKey> moduleFunctionStore =
                provider.GetService<IModuleFunctionStore<TModuleFunction, TModuleKey>>();
            if (moduleFunctionStore == null)
            {
                Logger.LogWarning("The service of IRepository<,> is not found. Please initialize the EntityModule module.");
                return;
            }

            if (!moduleInfos.CheckSyncByHash(provider, Logger))
            {
                Logger.LogInformation("The module data signature is the same as last time, synchronization has been cancelled");
                return;
            }

            TModule[] modules = moduleStore.Modules.ToArray();
            var positionModules = modules.Select(m => new { m.Id, Position = GetModulePosition(modules, m) })
                .OrderByDescending(m => m.Position.Length).ToArray();
            string[] deletePositions = positionModules.Select(m => m.Position)
                .Except(moduleInfos.Select(n => $"{n.Position}.{n.Code}"))
                .Except(new[] { "Root" })
                .ToArray();
            TModuleKey[] deleteModuleIds = positionModules.Where(m => deletePositions.Contains(m.Position)).Select(m => m.Id).ToArray();
            OperationResult result;
            foreach (TModuleKey id in deleteModuleIds)
            {
                result = moduleStore.DeleteModule(id).Result;
                if (result.Errored)
                {
                    throw new TuanException(result.Message);
                }
            }

            foreach (ModuleInfo info in moduleInfos)
            {
                TModule parent = GetModule(moduleStore, info.Position);
                if (parent == null)
                {
                    int lastIndex = info.Position.LastIndexOf('.');
                    string parent1Position = info.Position.Substring(0, lastIndex);
                    TModule parent1 = GetModule(moduleStore, parent1Position);
                    if (parent1 == null)
                    {
                        throw new TuanException($"Module information with path '{parent1Position}' could not be found");
                    }
                    string parentCode = info.Position.Substring(lastIndex + 1, info.Position.Length - lastIndex - 1);
                    ModuleInfo parentInfo = new ModuleInfo() { Code = parentCode, Name = info.PositionName ?? parentCode, Position = parent1Position };
                    TModuleInputDto dto = GetDto(parentInfo, parent1, null);
                    result = moduleStore.CreateModule(dto).Result;
                    if (result.Errored)
                    {
                        throw new TuanException(result.Message);
                    }
                    parent = moduleStore.Modules.First(m => m.ParentId.Equals(parent1.Id) && m.Code == parentCode);
                }
                TModule module = moduleStore.Modules.FirstOrDefault(m => m.ParentId.Equals(parent.Id) && m.Code == info.Code);
                if (module == null)
                {
                    TModuleInputDto dto = GetDto(info, parent, null);
                    result = moduleStore.CreateModule(dto).Result;
                    if (result.Errored)
                    {
                        throw new TuanException(result.Message);
                    }
                    module = moduleStore.Modules.First(m => m.ParentId.Equals(parent.Id) && m.Code == info.Code);
                }
                else
                {
                    TModuleInputDto dto = GetDto(info, parent, module);
                    result = moduleStore.UpdateModule(dto).Result;
                    if (result.Errored)
                    {
                        throw new TuanException(result.Message);
                    }
                }
                if (info.DependOnFunctions.Length > 0)
                {
                    Guid[] functionIds = info.DependOnFunctions.Select(m => m.Id).ToArray();
                    result = moduleFunctionStore.SetModuleFunctions(module.Id, functionIds).Result;
                    if (result.Errored)
                    {
                        throw new TuanException(result.Message);
                    }
                }
            }

            IUnitOfWork unitOfWork = provider.GetUnitOfWork<TModule, TModuleKey>();
            unitOfWork.Commit();
        }

        private readonly IDictionary<string, TModule> _positionDictionary = new Dictionary<string, TModule>();
        private TModule GetModule(IModuleStore<TModule, TModuleInputDto, TModuleKey> moduleStore, string position)
        {
            if (_positionDictionary.ContainsKey(position))
            {
                return _positionDictionary[position];
            }
            string[] codes = position.Split('.');
            if (codes.Length == 0)
            {
                return null;
            }
            string code = codes[0];
            TModule module = moduleStore.Modules.FirstOrDefault(m => m.Code == code);
            if (module == null)
            {
                return null;
            }
            for (int i = 1; i < codes.Length; i++)
            {
                code = codes[i];
                module = moduleStore.Modules.FirstOrDefault(m => m.Code == code && m.ParentId.Equals(module.Id));
                if (module == null)
                {
                    return null;
                }
            }
            _positionDictionary.Add(position, module);
            return module;
        }

        private static TModuleInputDto GetDto(ModuleInfo info, TModule parent, TModule existsModule)
        {
            return new TModuleInputDto()
            {
                Id = existsModule?.Id ?? default(TModuleKey),
                Name = info.Name,
                Code = info.Code,
                OrderCode = info.Order,
                Remark = $"{parent.Name}-{info.Name}",
                ParentId = parent.Id
            };
        }

        private static string GetModulePosition(TModule[] source, TModule module)
        {
            string[] codes = module.TreePathIds.Select(id => source.First(n => n.Id.Equals(id)).Code).ToArray();
            return codes.ExpandAndToString(".");
        }
    }



}
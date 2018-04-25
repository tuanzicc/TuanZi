using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Core.EntityInfos;
using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Entity;
using TuanZi.Mapping;


namespace TuanZi.Security
{
    public abstract class SecurityManagerBase<TFunction, TFunctionInputDto, TEntityInfo, TEntityInfoInputDto, TModule, TModuleInputDto, TModuleKey,
        TModuleFunction, TModuleRole, TModuleUser, TRoleKey, TUserKey>
        : IFunctionStore<TFunction, TFunctionInputDto>,
        IEntityInfoStore<TEntityInfo, TEntityInfoInputDto>,
        IModuleStore<TModule, TModuleInputDto, TModuleKey>,
        IModuleFunctionStore<TModuleFunction>,
        IModuleRoleStore<TModuleRole>,
        IModuleUserStore<TModuleUser>
        where TFunction : IFunction, IEntity<Guid>
        where TFunctionInputDto : FunctionInputDtoBase
        where TEntityInfo : IEntityInfo, IEntity<Guid>
        where TEntityInfoInputDto : EntityInfoInputDtoBase
        where TModule : ModuleBase<TModuleKey>
        where TModuleInputDto : ModuleInputDtoBase<TModuleKey>
        where TModuleFunction : ModuleFunctionBase<TModuleKey>
        where TModuleRole : ModuleRoleBase<TModuleKey, TRoleKey>
        where TModuleUser : ModuleUserBase<TModuleKey, TUserKey>
        where TModuleKey : struct, IEquatable<TModuleKey>
    {
        private readonly IRepository<TFunction, Guid> _functionRepository;
        private readonly IRepository<TEntityInfo, Guid> _entityInfoRepository;
        private readonly IRepository<TModule, TModuleKey> _moduleRepository;
        private readonly IRepository<TModuleFunction, Guid> _moduleFunctionRepository;
        private readonly IRepository<TModuleRole, Guid> _moduleRoleRepository;
        private readonly IRepository<TModuleUser, Guid> _moduleUserRepository;

        protected SecurityManagerBase(
            IRepository<TFunction, Guid> functionRepository,
            IRepository<TEntityInfo, Guid> entityInfoRepository,
            IRepository<TModule, TModuleKey> moduleRepository,
            IRepository<TModuleFunction, Guid> moduleFunctionRepository,
            IRepository<TModuleRole, Guid> moduleRoleRepository,
            IRepository<TModuleUser, Guid> moduleUserRepository)
        {
            _functionRepository = functionRepository;
            _entityInfoRepository = entityInfoRepository;
            _moduleRepository = moduleRepository;
            _moduleFunctionRepository = moduleFunctionRepository;
            _moduleRoleRepository = moduleRoleRepository;
            _moduleUserRepository = moduleUserRepository;
        }

        #region Implementation of IFunctionStore<TFunction,in TFunctionInputDto>

        public IQueryable<TFunction> Functions
        {
            get { return _functionRepository.Query(); }
        }

        public Task<bool> CheckFunctionExists(Expression<Func<TFunction, bool>> predicate, Guid id = default(Guid))
        {
            return _functionRepository.CheckExistsAsync(predicate, id);
        }

        public Task<OperationResult> UpdateFunctions(params TFunctionInputDto[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            return _functionRepository.UpdateAsync(dtos,
                async (dto, entity) =>
                {
                    if (dto.AuditEntityEnabled && !dto.AuditOperationEnabled && !entity.AuditOperationEnabled && !entity.AuditEntityEnabled)
                    {
                        dto.AuditOperationEnabled = true;
                    }
                    else if (!dto.AuditOperationEnabled && dto.AuditEntityEnabled && entity.AuditOperationEnabled && entity.AuditEntityEnabled)
                    {
                        dto.AuditEntityEnabled = false;
                    }
                    if (dto.AccessType != entity.AccessType)
                    {
                        entity.IsAccessTypeChanged = true;
                    }
                });
        }

        #endregion

        #region Implementation of IEntityInfoStore<TEntityInfo,in TEntityInfoInputDto>

        public IQueryable<TEntityInfo> EntityInfos
        {
            get { return _entityInfoRepository.Query(); }
        }

        public Task<bool> CheckEntityInfoExists(Expression<Func<TEntityInfo, bool>> predicate, Guid id = default(Guid))
        {
            return _entityInfoRepository.CheckExistsAsync(predicate, id);
        }

        public Task<OperationResult> UpdateEntityInfos(params TEntityInfoInputDto[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            return _entityInfoRepository.UpdateAsync(dtos);
        }

        #endregion

        #region Implementation of IModuleStore<TModule,in TModuleInputDto,in TModuleKey>

        public IQueryable<TModule> Modules
        {
            get { return _moduleRepository.Query(); }
        }

        public Task<bool> CheckModuleExists(Expression<Func<TModule, bool>> predicate, TModuleKey id = default(TModuleKey))
        {
            return _moduleRepository.CheckExistsAsync(predicate, id);
        }

        public async Task<OperationResult> CreateModule(TModuleInputDto dto)
        {
            Check.NotNull(dto, nameof(dto));
            var exist = Modules.Where(m => m.Name == dto.Name && m.ParentId != null && m.ParentId.Equals(dto.ParentId))
                .SelectMany(m => Modules.Where(n => n.Id.Equals(m.ParentId)).Select(n => n.Name)).FirstOrDefault();
            if (exist != null)
            {
                return new OperationResult(OperationResultType.Error, $"A submodule named '{dto.Name}' already exists in module '{exist}'");
            }

            TModule entity = dto.MapTo<TModule>();
            var peerModules = Modules.Where(m => m.ParentId.Equals(dto.ParentId)).Select(m => new { m.OrderCode }).ToArray();
            if (peerModules.Length == 0)
            {
                entity.OrderCode = 1;
            }
            else
            {
                double maxCode = peerModules.Max(m => m.OrderCode);
                entity.OrderCode = maxCode + 1;
            }
            if (!dto.ParentId.Equals(default(TModuleKey)))
            {
                var parent = Modules.Where(m => m.Id.Equals(dto.ParentId)).Select(m => new { m.Id, m.TreePathString }).FirstOrDefault();
                if (parent == null)
                {
                    return new OperationResult(OperationResultType.Error, $"The parent module with ID '{dto.ParentId}' does not exist");
                }
                entity.ParentId = dto.ParentId;
                entity.TreePathString = GetModuleTreePath(parent.Id, parent.TreePathString);
            }
            else
            {
                entity.ParentId = null;
            }
            return await _moduleRepository.InsertAsync(entity) > 0
                ? new OperationResult(OperationResultType.Success, $"Module '{dto.Name}' created")
                : OperationResult.NoChanged;
        }

        public async Task<OperationResult> UpdateModule(TModuleInputDto dto)
        {
            Check.NotNull(dto, nameof(dto));
            var exist = Modules.Where(m => m.Name == dto.Name && m.ParentId != null && m.ParentId.Equals(dto.ParentId))
                .SelectMany(m => Modules.Where(n => n.Id.Equals(m.ParentId)).Select(n => new { n.Id, n.Name })).FirstOrDefault();
            if (exist != null)
            {
                return new OperationResult(OperationResultType.Error, $"A submodule named '{dto.Name}' already exists in module '{exist}'");
            }
            TModule entity = await _moduleRepository.GetAsync(dto.Id);
            if (entity == null)
            {
                return new OperationResult(OperationResultType.Error, $"Module with ID '{dto.Id}' does not exist.");
            }
            entity = dto.MapTo(entity);
            if (!dto.ParentId.Equals(default(TModuleKey)))
            {
                if (!entity.ParentId.Equals(dto.ParentId))
                {
                    var parent = Modules.Where(m => m.Id.Equals(dto.ParentId)).Select(m => new { m.Id, m.TreePathString }).FirstOrDefault();
                    if (parent == null)
                    {
                        return new OperationResult(OperationResultType.Error, $"The parent module with ID '{dto.ParentId}' does not exist");
                    }
                    entity.ParentId = dto.ParentId;
                    entity.TreePathString = GetModuleTreePath(parent.Id, parent.TreePathString);
                }
                else
                {
                    entity.ParentId = null;
                }
            }
            else
            {
                entity.ParentId = null;
            }
            return await _moduleRepository.UpdateAsync(entity) > 0
                ? new OperationResult(OperationResultType.Success, $"Module '{dto.Name}' updated")
                : OperationResult.NoChanged;
        }

        public async Task<OperationResult> DeleteModule(TModuleKey id)
        {
            TModule entity = await _moduleRepository.GetAsync(id);
            if (entity == null)
            {
                return OperationResult.Success;
            }
            if (await _moduleRepository.CheckExistsAsync(m => m.ParentId.Equals(id)))
            {
                return new OperationResult(OperationResultType.Error, $"Submodule of module '{entity.Name}' is not empty and cannot be deleted");
            }
            await _moduleFunctionRepository.DeleteBatchAsync(m => m.ModuleId.Equals(id));
            await _moduleRoleRepository.DeleteBatchAsync(m => m.ModuleId.Equals(id));
            await _moduleUserRepository.DeleteBatchAsync(m => m.ModuleId.Equals(id));

            return await _moduleRepository.DeleteAsync(entity) > 0
                ? new OperationResult(OperationResultType.Success, $"Module '{entity.Name}' deleted")
                : OperationResult.NoChanged;
        }

        private string GetModuleTreePath(TModuleKey parentId, string parentTreePath)
        {
            const string treePathItemFormat = "${0}$";
            if (parentTreePath == null)
            {
                return null;
            }
            return treePathItemFormat + treePathItemFormat.FormatWith(parentId);
        }

        #endregion

        #region Implementation of IModuleFunctionStore<TModuleFunction>

        public IQueryable<TModuleFunction> ModuleFunctions
        {
            get { return _moduleFunctionRepository.Query(); }
        }

        public Task<bool> CheckModuleFunctionExists(Expression<Func<TModuleFunction, bool>> predicate, Guid id = default(Guid))
        {
            return _moduleFunctionRepository.CheckExistsAsync(predicate, id);
        }

        #endregion

        #region Implementation of IModuleRoleStore<TModuleRole>

        public IQueryable<TModuleRole> ModuleRoles
        {
            get { return _moduleRoleRepository.Query(); }
        }

        public Task<bool> CheckModuleRoleExists(Expression<Func<TModuleRole, bool>> predicate, Guid id = default(Guid))
        {
            return _moduleRoleRepository.CheckExistsAsync(predicate, id);
        }

        #endregion

        #region Implementation of IModuleUserStore<TModuleUser>

        public IQueryable<TModuleUser> ModuleUsers
        {
            get { return _moduleUserRepository.Query(); }
        }

        public Task<bool> CheckModuleUserExists(Expression<Func<TModuleUser, bool>> predicate, Guid id = default(Guid))
        {
            return _moduleUserRepository.CheckExistsAsync(predicate, id);
        }

        #endregion
    }
}
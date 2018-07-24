using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TuanZi.Collections;
using TuanZi.Core.EntityInfos;
using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.EventBuses;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Filter;
using TuanZi.Identity;
using TuanZi.Mapping;
using TuanZi.Security.Events;
using TuanZi.Secutiry;

namespace TuanZi.Security
{

    public abstract class SecurityManagerBase<TFunction, TFunctionInputDto, TEntityInfo, TEntityInfoInputDto, TModule, TModuleInputDto, TModuleKey,
            TModuleFunction, TModuleRole, TModuleUser, TEntityRole, TEntityRoleInputDto, TUserRole, TRole, TRoleKey, TUser, TUserKey>
        : IFunctionStore<TFunction, TFunctionInputDto>,
          IEntityInfoStore<TEntityInfo, TEntityInfoInputDto>,
          IModuleStore<TModule, TModuleInputDto, TModuleKey>,
          IModuleFunctionStore<TModuleFunction, TModuleKey>,
          IModuleRoleStore<TModuleRole, TRoleKey, TModuleKey>,
          IModuleUserStore<TModuleUser, TUserKey, TModuleKey>,
          IEntityRoleStore<TEntityRole, TEntityRoleInputDto, TRoleKey>
        where TFunction : IFunction
        where TFunctionInputDto : FunctionInputDtoBase
        where TEntityInfo : IEntityInfo
        where TEntityInfoInputDto : EntityInfoInputDtoBase
        where TModule : ModuleBase<TModuleKey>
        where TModuleInputDto : ModuleInputDtoBase<TModuleKey>
        where TModuleFunction : ModuleFunctionBase<TModuleKey>, new()
        where TModuleRole : ModuleRoleBase<TModuleKey, TRoleKey>, new()
        where TModuleUser : ModuleUserBase<TModuleKey, TUserKey>, new()
        where TModuleKey : struct, IEquatable<TModuleKey>
        where TEntityRole : EntityRoleBase<TRoleKey>
        where TEntityRoleInputDto : EntityRoleInputDtoBase<TRoleKey>
        where TUserRole : UserRoleBase<TUserKey, TRoleKey>
        where TRole : RoleBase<TRoleKey>
        where TUser : UserBase<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
        where TUserKey : IEquatable<TUserKey>
    {
        private readonly IRepository<TEntityInfo, Guid> _entityInfoRepository;
        private readonly IEventBus _eventBus;
        private readonly IRepository<TFunction, Guid> _functionRepository;
        private readonly IRepository<TModuleFunction, Guid> _moduleFunctionRepository;
        private readonly IRepository<TModule, TModuleKey> _moduleRepository;
        private readonly IRepository<TModuleRole, Guid> _moduleRoleRepository;
        private readonly IRepository<TModuleUser, Guid> _moduleUserRepository;
        private readonly IRepository<TEntityRole, Guid> _entityRoleRepository;
        private readonly IRepository<TRole, TRoleKey> _roleRepository;
        private readonly IRepository<TUser, TUserKey> _userRepository;
        private readonly IRepository<TUserRole, Guid> _userRoleRepository;

        protected SecurityManagerBase(
            IEventBus eventBus,
            IRepository<TFunction, Guid> functionRepository,
            IRepository<TEntityInfo, Guid> entityInfoRepository,
            IRepository<TModule, TModuleKey> moduleRepository,
            IRepository<TModuleFunction, Guid> moduleFunctionRepository,
            IRepository<TModuleRole, Guid> moduleRoleRepository,
            IRepository<TModuleUser, Guid> moduleUserRepository,
            IRepository<TEntityRole, Guid> entityRoleRepository,
            IRepository<TUserRole, Guid> userRoleRepository,
            IRepository<TRole, TRoleKey> roleRepository,
            IRepository<TUser, TUserKey> userRepository
        )
        {
            _eventBus = eventBus;
            _functionRepository = functionRepository;
            _entityInfoRepository = entityInfoRepository;
            _moduleRepository = moduleRepository;
            _moduleFunctionRepository = moduleFunctionRepository;
            _moduleRoleRepository = moduleRoleRepository;
            _moduleUserRepository = moduleUserRepository;
            _entityRoleRepository = entityRoleRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        #region Implementation of IFunctionStore<TFunction,in TFunctionInputDto>

        public IQueryable<TFunction> Functions => _functionRepository.Query();

        public virtual Task<bool> CheckFunctionExists(Expression<Func<TFunction, bool>> predicate, Guid id = default(Guid))
        {
            return _functionRepository.CheckExistsAsync(predicate, id);
        }

        public virtual async Task<OperationResult> UpdateFunctions(params TFunctionInputDto[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            OperationResult result = await _functionRepository.UpdateAsync(dtos,
                async (dto, entity) =>
                {
                    if (dto.IsLocked && entity.Area == "Admin" && entity.Controller == "Function"
                        && (entity.Action == "Update" || entity.Action == "Read"))
                    {
                        throw new Exception($"Function '{entity.Name}' cannot be locked");
                    }
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
            if (result.Successed)
            {
                FunctionCacheRefreshEventData clearEventData = new FunctionCacheRefreshEventData();
                _eventBus.Publish(clearEventData);

                FunctionAuthCacheRefreshEventData removeEventData = new FunctionAuthCacheRefreshEventData()
                {
                    FunctionIds = dtos.Select(m => m.Id).ToArray()
                };
                _eventBus.Publish(removeEventData);
            }
            return result;
        }

        #endregion Implementation of IFunctionStore<TFunction,in TFunctionInputDto>

        #region Implementation of IEntityInfoStore<TEntityInfo,in TEntityInfoInputDto>

        public IQueryable<TEntityInfo> EntityInfos => _entityInfoRepository.Query();

        public virtual Task<bool> CheckEntityInfoExists(Expression<Func<TEntityInfo, bool>> predicate, Guid id = default(Guid))
        {
            return _entityInfoRepository.CheckExistsAsync(predicate, id);
        }

        public virtual Task<OperationResult> UpdateEntityInfos(params TEntityInfoInputDto[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            return _entityInfoRepository.UpdateAsync(dtos);
        }

        #endregion Implementation of IEntityInfoStore<TEntityInfo,in TEntityInfoInputDto>

        #region Implementation of IModuleStore<TModule,in TModuleInputDto,in TModuleKey>

        public IQueryable<TModule> Modules => _moduleRepository.Query();

        public virtual Task<bool> CheckModuleExists(Expression<Func<TModule, bool>> predicate, TModuleKey id = default(TModuleKey))
        {
            return _moduleRepository.CheckExistsAsync(predicate, id);
        }

        public virtual async Task<OperationResult> CreateModule(TModuleInputDto dto)
        {
            const string treePathItemFormat = "${0}$";
            Check.NotNull(dto, nameof(dto));
            if (dto.Name.Contains('.'))
            {
                return new OperationResult(OperationResultType.Error, $"Module '{dto.Name}' cannot contains '-'");
            }
            var exist = Modules.Where(m => m.Name == dto.Name && m.ParentId != null && m.ParentId.Equals(dto.ParentId))
                .SelectMany(m => Modules.Where(n => n.Id.Equals(m.ParentId)).Select(n => n.Name)).FirstOrDefault();
            if (exist != null)
            {
                return new OperationResult(OperationResultType.Error, $"A submodule named '{dto.Name}' already exists in module '{exist}'");
            }
            exist = Modules.Where(m => m.Code == dto.Code && m.ParentId != null && m.ParentId.Equals(dto.ParentId))
                .SelectMany(m => Modules.Where(n => n.Id.Equals(m.ParentId)).Select(n => n.Name)).FirstOrDefault();
            if (exist != null)
            {
                return new OperationResult(OperationResultType.Error, $"A submodule with code '{dto.Code}' already exists in module '{exist}'");
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
            string parentTreePathString = null;
            if (!Equals(dto.ParentId, default(TModuleKey)))
            {
                var parent = Modules.Where(m => m.Id.Equals(dto.ParentId)).Select(m => new { m.Id, m.TreePathString }).FirstOrDefault();
                if (parent == null)
                {
                    return new OperationResult(OperationResultType.Error, $"The parent module with ID '{dto.ParentId}' does not exist");
                }
                entity.ParentId = dto.ParentId;
                parentTreePathString = parent.TreePathString;
            }
            else
            {
                entity.ParentId = null;
            }
            if (await _moduleRepository.InsertAsync(entity) > 0)
            {
                entity.TreePathString = entity.ParentId == null
                    ? treePathItemFormat.FormatWith(entity.Id)
                    : GetModuleTreePath(entity.Id, parentTreePathString, treePathItemFormat);
                await _moduleRepository.UpdateAsync(entity);
                return new OperationResult(OperationResultType.Success, $"Module '{dto.Name}' created");
            }
            return OperationResult.NoChanges;
        }

        public virtual async Task<OperationResult> UpdateModule(TModuleInputDto dto)
        {
            const string treePathItemFormat = "${0}$";
            Check.NotNull(dto, nameof(dto));
            if (dto.Name.Contains('.'))
            {
                return new OperationResult(OperationResultType.Error, $"Module '{dto.Name}' cannot contains '-'");
            }
            var exist1 = Modules.Where(m => m.Name == dto.Name && m.ParentId != null && m.ParentId.Equals(dto.ParentId) && !m.Id.Equals(dto.Id))
                .SelectMany(m => Modules.Where(n => n.Id.Equals(m.ParentId)).Select(n => new { n.Id, n.Name })).FirstOrDefault();
            if (exist1 != null)
            {
                return new OperationResult(OperationResultType.Error, $"A submodule named '{dto.Name}' already exists in the module '{exist1.Name}'.");
            }
            var exist2 = Modules.Where(m => m.Code == dto.Code && m.ParentId != null && m.ParentId.Equals(dto.ParentId) && !m.Id.Equals(dto.Id))
                .SelectMany(m => Modules.Where(n => n.Id.Equals(m.ParentId)).Select(n => new { n.Id, n.Name })).FirstOrDefault();
            if (exist2 != null)
            {
                return new OperationResult(OperationResultType.Error, $"A submodule with code '{dto.Code}' already exists in module '{exist2.Name}'.");
            }
            TModule entity = await _moduleRepository.GetAsync(dto.Id);
            if (entity == null)
            {
                return new OperationResult(OperationResultType.Error, $"Module with ID '{dto.Id}' does not exist.");
            }
            entity = dto.MapTo(entity);
            if (!Equals(dto.ParentId, default(TModuleKey)))
            {
                if (!entity.ParentId.Equals(dto.ParentId))
                {
                    var parent = Modules.Where(m => m.Id.Equals(dto.ParentId)).Select(m => new { m.Id, m.TreePathString }).FirstOrDefault();
                    if (parent == null)
                    {
                        return new OperationResult(OperationResultType.Error, $"The parent module with ID '{dto.ParentId}' does not exist");
                    }
                    entity.ParentId = dto.ParentId;
                    entity.TreePathString = GetModuleTreePath(entity.Id, parent.TreePathString, treePathItemFormat);
                }
            }
            else
            {
                entity.ParentId = null;
                entity.TreePathString = treePathItemFormat.FormatWith(entity.Id);
            }
            return await _moduleRepository.UpdateAsync(entity) > 0
                 ? new OperationResult(OperationResultType.Success, $"Module '{dto.Name}' updated")
                : OperationResult.NoChanges;
        }

        public virtual async Task<OperationResult> DeleteModule(TModuleKey id)
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

            OperationResult result = await _moduleRepository.DeleteAsync(entity) > 0
                ? new OperationResult(OperationResultType.Success, $"Module '{entity.Name}' deleted")
                : OperationResult.NoChanges;
            if (result.Successed)
            {
                Guid[] functionIds = _moduleFunctionRepository.Query(m => m.Id.Equals(id)).Select(m => m.FunctionId).ToArray();
                FunctionAuthCacheRefreshEventData removeEventData = new FunctionAuthCacheRefreshEventData() { FunctionIds = functionIds };
                _eventBus.Publish(removeEventData);
            }
            return result;
        }

        public virtual TModuleKey[] GetModuleTreeIds(params TModuleKey[] rootIds)
        {
            return rootIds.SelectMany(m => _moduleRepository.Query(n => n.TreePathString.Contains($"${m}$")).Select(n => n.Id)).Distinct()
                .ToArray();
        }

        private static string GetModuleTreePath(TModuleKey currentId, string parentTreePath, string treePathItemFormat)
        {
            return $"{parentTreePath},{treePathItemFormat.FormatWith(currentId)}";
        }

        #endregion Implementation of IModuleStore<TModule,in TModuleInputDto,in TModuleKey>

        #region Implementation of IModuleFunctionStore<TModuleFunction>

        public IQueryable<TModuleFunction> ModuleFunctions => _moduleFunctionRepository.Query();

        public virtual Task<bool> CheckModuleFunctionExists(Expression<Func<TModuleFunction, bool>> predicate, Guid id = default(Guid))
        {
            return _moduleFunctionRepository.CheckExistsAsync(predicate, id);
        }

        public virtual async Task<OperationResult> SetModuleFunctions(TModuleKey moduleId, Guid[] functionIds)
        {
            TModule module = await _moduleRepository.GetAsync(moduleId);
            if (module == null)
            {
                return new OperationResult(OperationResultType.QueryNull, $"Module with ID '{moduleId}' does not exist.");
            }

            Guid[] existFunctionIds = _moduleFunctionRepository.Query(m => m.ModuleId.Equals(moduleId)).Select(m => m.FunctionId).ToArray();
            Guid[] addFunctionIds = functionIds.Except(existFunctionIds).ToArray();
            Guid[] removeFunctionIds = existFunctionIds.Except(functionIds).ToArray();
            List<string> addNames = new List<string>(), removeNames = new List<string>();
            int count = 0;

            foreach (Guid functionId in addFunctionIds)
            {
                TFunction function = await _functionRepository.GetAsync(functionId);
                if (function == null)
                {
                    continue;
                }
                TModuleFunction moduleFunction = new TModuleFunction() { ModuleId = moduleId, FunctionId = functionId };
                count = count + await _moduleFunctionRepository.InsertAsync(moduleFunction);
                addNames.Add(function.Name);
            }
            foreach (Guid functionId in removeFunctionIds)
            {
                TFunction function = await _functionRepository.GetAsync(functionId);
                if (function == null)
                {
                    continue;
                }
                TModuleFunction moduleFunction = _moduleFunctionRepository.Query()
                    .FirstOrDefault(m => m.ModuleId.Equals(moduleId) && m.FunctionId == functionId);
                if (moduleFunction == null)
                {
                    continue;
                }
                count = count + await _moduleFunctionRepository.DeleteAsync(moduleFunction);
                removeNames.Add(function.Name);
            }

            if (count > 0)
            {
                FunctionAuthCacheRefreshEventData removeEventData = new FunctionAuthCacheRefreshEventData()
                {
                    FunctionIds = addFunctionIds.Union(removeFunctionIds).Distinct().ToArray()
                };
                _eventBus.Publish(removeEventData);

                return new OperationResult(OperationResultType.Success,
                      $"Module '{module.Name}' added '{addNames.ExpandAndToString()}'，deleted '{removeNames.ExpandAndToString()}' done");
            }
            return OperationResult.NoChanges;
        }

        #endregion Implementation of IModuleFunctionStore<TModuleFunction>

        #region Implementation of IModuleRoleStore<TModuleRole>

        public IQueryable<TModuleRole> ModuleRoles => _moduleRoleRepository.Query();

        public virtual Task<bool> CheckModuleRoleExists(Expression<Func<TModuleRole, bool>> predicate, Guid id = default(Guid))
        {
            return _moduleRoleRepository.CheckExistsAsync(predicate, id);
        }

        public virtual async Task<OperationResult> SetRoleModules(TRoleKey roleId, TModuleKey[] moduleIds)
        {
            TRole role = await _roleRepository.GetAsync(roleId);
            if (role == null)
            {
                return new OperationResult(OperationResultType.QueryNull, $"Role with ID '{roleId}' does not exist.");
            }

            TModuleKey[] existModuleIds = _moduleRoleRepository.Query(m => m.RoleId.Equals(roleId)).Select(m => m.ModuleId).ToArray();
            TModuleKey[] addModuleIds = moduleIds.Except(existModuleIds).ToArray();
            TModuleKey[] removeModuleIds = existModuleIds.Except(moduleIds).ToArray();
            List<string> addNames = new List<string>(), removeNames = new List<string>();
            int count = 0;

            foreach (TModuleKey moduleId in addModuleIds)
            {
                TModule module = await _moduleRepository.GetAsync(moduleId);
                if (module == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, $"Module with ID '{moduleId}' does not exist.");
                }
                TModuleRole moduleRole = new TModuleRole() { ModuleId = moduleId, RoleId = roleId };
                count = count + await _moduleRoleRepository.InsertAsync(moduleRole);
                addNames.Add(module.Name);
            }
            foreach (TModuleKey moduleId in removeModuleIds)
            {
                TModule module = await _moduleRepository.GetAsync(moduleId);
                if (module == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, $"Module with ID '{moduleId}' does not exist.");
                }
                TModuleRole moduleRole = _moduleRoleRepository.Query().FirstOrDefault(m => m.RoleId.Equals(roleId) && m.ModuleId.Equals(moduleId));
                if (moduleRole == null)
                {
                    continue;
                }
                count = count + await _moduleRoleRepository.DeleteAsync(moduleRole);
                removeNames.Add(module.Name);
            }

            if (count > 0)
            {
                moduleIds = addModuleIds.Union(removeModuleIds).Distinct().ToArray();
                Guid[] functionIds = _moduleFunctionRepository.Query(m => moduleIds.Contains(m.ModuleId))
                    .Select(m => m.FunctionId).Distinct().ToArray();
                FunctionAuthCacheRefreshEventData removeEventData = new FunctionAuthCacheRefreshEventData() { FunctionIds = functionIds };
                _eventBus.Publish(removeEventData);

                if (addNames.Count > 0 && removeNames.Count == 0)
                {
                    return new OperationResult(OperationResultType.Success, $"Role '{role.Name}' added '{addNames.ExpandAndToString()}'");
                }
                if (addNames.Count == 0 && removeNames.Count > 0)
                {
                    return new OperationResult(OperationResultType.Success, $"Role '{role.Name}' deleted '{removeNames.ExpandAndToString()}'");
                }
                return new OperationResult(OperationResultType.Success,
                    $"Role '{role.Name}' added '{addNames.ExpandAndToString()}'，deleted '{removeNames.ExpandAndToString()}' done");
            }
            return OperationResult.NoChanges;
        }

        public virtual TModuleKey[] GetRoleModuleIds(TRoleKey roleId)
        {
            TModuleKey[] moduleIds = _moduleRoleRepository.Query(m => m.RoleId.Equals(roleId)).Select(m => m.ModuleId).Distinct().ToArray();
            return GetModuleTreeIds(moduleIds);
        }

        #endregion Implementation of IModuleRoleStore<TModuleRole>

        #region Implementation of IModuleUserStore<TModuleUser>

        public IQueryable<TModuleUser> ModuleUsers => _moduleUserRepository.Query();

        public virtual Task<bool> CheckModuleUserExists(Expression<Func<TModuleUser, bool>> predicate, Guid id = default(Guid))
        {
            return _moduleUserRepository.CheckExistsAsync(predicate, id);
        }

        public virtual async Task<OperationResult> SetUserModules(TUserKey userId, TModuleKey[] moduleIds)
        {
            TUser user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return new OperationResult(OperationResultType.QueryNull, $"User with ID '{userId}' does not exists.");
            }

            TModuleKey[] existModuleIds = _moduleUserRepository.Query(m => m.UserId.Equals(userId)).Select(m => m.ModuleId).ToArray();
            TModuleKey[] addModuleIds = moduleIds.Except(existModuleIds).ToArray();
            TModuleKey[] removeModuleIds = existModuleIds.Except(moduleIds).ToArray();
            List<string> addNames = new List<string>(), removeNames = new List<string>();
            int count = 0;

            foreach (TModuleKey moduleId in addModuleIds)
            {
                TModule module = await _moduleRepository.GetAsync(moduleId);
                if (module == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, $"Module with ID '{moduleId}' does not exist.");
                }
                TModuleUser moduleUser = new TModuleUser() { ModuleId = moduleId, UserId = userId };
                count += await _moduleUserRepository.InsertAsync(moduleUser);
                addNames.Add(module.Name);
            }
            foreach (TModuleKey moduleId in removeModuleIds)
            {
                TModule module = await _moduleRepository.GetAsync(moduleId);
                if (module == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, $"Module with ID '{moduleId}' does not exist.");
                }
                TModuleUser moduleUser = _moduleUserRepository.Query().FirstOrDefault(m => m.ModuleId.Equals(moduleId) && m.UserId.Equals(userId));
                if (moduleUser == null)
                {
                    continue;
                }
                count += await _moduleUserRepository.DeleteAsync(moduleUser);
                removeNames.Add(module.Name);
            }
            if (count > 0)
            {
                FunctionAuthCacheRefreshEventData removeEventData = new FunctionAuthCacheRefreshEventData() { UserNames = new[] { user.UserName } };
                _eventBus.Publish(removeEventData);

                if (addNames.Count > 0 && removeNames.Count == 0)
                {
                    return new OperationResult(OperationResultType.Success, $"User '{user.UserName}' added '{addNames.ExpandAndToString()}'");
                }
                if (addNames.Count == 0 && removeNames.Count > 0)
                {
                    return new OperationResult(OperationResultType.Success, $"User '{user.UserName}' deleted '{removeNames.ExpandAndToString()}'");
                }
                return new OperationResult(OperationResultType.Success,
                    $"User '{user.UserName}' added '{addNames.ExpandAndToString()}'，deleted '{removeNames.ExpandAndToString()}' done");
            }
            return OperationResult.NoChanges;
        }

        public virtual TModuleKey[] GetUserSelfModuleIds(TUserKey userId)
        {
            TModuleKey[] moduleIds = _moduleUserRepository.Query(m => m.UserId.Equals(userId)).Select(m => m.ModuleId).Distinct().ToArray();
            return GetModuleTreeIds(moduleIds);
        }

        public virtual TModuleKey[] GetUserWithRoleModuleIds(TUserKey userId)
        {
            TModuleKey[] selfModuleIds = GetUserSelfModuleIds(userId);

            TRoleKey[] roleIds = _userRoleRepository.Query(m => m.UserId.Equals(userId)).Select(m => m.RoleId).ToArray();
            TModuleKey[] roleModuleIds = roleIds
                .SelectMany(m => _moduleRoleRepository.Query(n => n.RoleId.Equals(m)).Select(n => n.ModuleId))
                .Distinct().ToArray();
            roleModuleIds = GetModuleTreeIds(roleModuleIds);

            return roleModuleIds.Union(selfModuleIds).Distinct().ToArray();
        }

        #endregion Implementation of IModuleUserStore<TModuleUser>

        #region Implementation of IEntityRoleStore<TEntityRole,in TEntityRoleInputDto,in TRoleKey>

        public virtual IQueryable<TEntityRole> EntityRoles => _entityRoleRepository.Query();

        public virtual Task<bool> CheckEntityRoleExists(Expression<Func<TEntityRole, bool>> predicate, Guid id = default(Guid))
        {
            return _entityRoleRepository.CheckExistsAsync(predicate, id);
        }

        public virtual FilterGroup[] GetEntityRoleFilterGroups(TRoleKey roleId, Guid entityId, DataAuthOperation operation)
        {
            return _entityRoleRepository.Query(m => m.RoleId.Equals(roleId) && m.EntityId == entityId && m.Operation == operation)
                .Select(m => m.FilterGroupJson).ToArray().Select(m => m.FromJsonString<FilterGroup>()).ToArray();
        }

        public virtual async Task<OperationResult> CreateEntityRoles(params TEntityRoleInputDto[] dtos)
        {
            List<DataAuthCacheItem> cacheItems = new List<DataAuthCacheItem>();
            OperationResult result = await _entityRoleRepository.InsertAsync(dtos,
                async dto =>
                {
                    TRole role = await _roleRepository.GetAsync(dto.RoleId);
                    if (role == null)
                    {
                        throw new TuanException($"Role ID with '{dto.RoleId}' does not exist.");
                    }
                    TEntityInfo entityInfo = await _entityInfoRepository.GetAsync(dto.EntityId);
                    if (entityInfo == null)
                    {
                        throw new TuanException($"Entity '{dto.EntityId}'  does not exist.");
                    }
                    if (await CheckEntityRoleExists(m => m.RoleId.Equals(dto.RoleId) && m.EntityId == dto.EntityId))
                    {
                        throw new TuanException($"The data permission rule for the role '{role.Name}' and the entity '{entityInfo.Name}' already exists.");
                    }
                    OperationResult checkResult = CheckFilterGroup(dto.FilterGroup, entityInfo);
                    if (!checkResult.Successed)
                    {
                        throw new TuanException($"Data rule validation failed:{checkResult.Message}");
                    }
                    cacheItems.Add(new DataAuthCacheItem()
                    {
                        RoleName = role.Name,
                        EntityTypeFullName = entityInfo.TypeName,
                        Operation = dto.Operation,
                        FilterGroup = dto.FilterGroup
                    });
                });
            if (result.Successed && cacheItems.Count > 0)
            {
                DataAuthCacheRefreshEventData eventData = new DataAuthCacheRefreshEventData() { CacheItems = cacheItems };
                _eventBus.Publish(eventData);
            }
            return result;
        }

        public virtual async Task<OperationResult> UpdateEntityRoles(params TEntityRoleInputDto[] dtos)
        {
            List<DataAuthCacheItem> cacheItems = new List<DataAuthCacheItem>();
            OperationResult result = await _entityRoleRepository.UpdateAsync(dtos,
                async (dto, entity) =>
                {
                    TRole role = await _roleRepository.GetAsync(dto.RoleId);
                    if (role == null)
                    {
                        throw new TuanException($"Role with ID '{dto.RoleId}' does not exist.");
                    }
                    TEntityInfo entityInfo = await _entityInfoRepository.GetAsync(dto.EntityId);
                    if (entityInfo == null)
                    {
                        throw new TuanException($"Entity'{dto.EntityId}' does not exist.");
                    }
                    if (await CheckEntityRoleExists(m => m.RoleId.Equals(dto.RoleId) && m.EntityId == dto.EntityId, dto.Id))
                    {
                        throw new TuanException($"The data permission rule for the role '{role.Name}' and the entity '{entityInfo.Name}' already exists.");
                    }
                    OperationResult checkResult = CheckFilterGroup(dto.FilterGroup, entityInfo);
                    if (!checkResult.Successed)
                    {
                        throw new TuanException($"Data rule validation failed:{checkResult.Message}");
                    }
                    cacheItems.Add(new DataAuthCacheItem()
                    {
                        RoleName = role.Name,
                        EntityTypeFullName = entityInfo.TypeName,
                        FilterGroup = dto.FilterGroup
                    });
                });

            if (result.Successed && cacheItems.Count > 0)
            {
                DataAuthCacheRefreshEventData eventData = new DataAuthCacheRefreshEventData() { CacheItems = cacheItems };
                _eventBus.Publish(eventData);
            }
            return result;
        }

        public virtual async Task<OperationResult> DeleteEntityRoles(params Guid[] ids)
        {
            List<(string, string)> list = new List<(string, string)>();
            OperationResult result = await _entityRoleRepository.DeleteAsync(ids,
                async entity =>
                {
                    TRole role = await _roleRepository.GetAsync(entity.RoleId);
                    TEntityInfo entityInfo = await _entityInfoRepository.GetAsync(entity.EntityId);
                    if (role != null && entityInfo != null)
                    {
                        list.Add((role.Name, entityInfo.TypeName));
                    }
                });
            if (result.Successed && list.Count > 0)
            {
                IDataAuthCache cache = ServiceLocator.Instance.GetService<IDataAuthCache>();
                foreach ((string roleName, string typeName) in list)
                {
                    cache.RemoveCache(roleName, typeName, DataAuthOperation.Delete);
                }
            }
            return result;
        }

        private static OperationResult CheckFilterGroup(FilterGroup group, TEntityInfo entityInfo)
        {
            EntityProperty[] properties = entityInfo.Properties;

            foreach (FilterRule rule in group.Rules)
            {
                EntityProperty property = properties.FirstOrDefault(m => m.Name == rule.Field);
                if (property == null)
                {
                    return new OperationResult(OperationResultType.Error, $"The property name '{rule.Field}' does not exist in the entity '{entityInfo.Name}'");
                }
                if (rule.Value == null || rule.Value.ToString().IsNullOrWhiteSpace())
                {
                    return new OperationResult(OperationResultType.Error, $"属性名“{property.Display}”操作“{rule.Operate.ToDescription()}”的值不能为空");
                }
            }
            if (group.Operate == FilterOperate.And)
            {
                List<IGrouping<string, FilterRule>> duplicate = group.Rules.GroupBy(m => m.Field + m.Operate).Where(m => m.Count() > 1).ToList();
                if (duplicate.Count > 0)
                {
                    FilterRule[] rules = duplicate.SelectMany(m => m.Select(n => n)).DistinctBy(m => m.Field + m.Operate).ToArray();
                    return new OperationResult(OperationResultType.Error,
                        $"The group operation is 'and' under the condition that the field and operation '{rules.ExpandAndToString(m => $"{properties.First(n => n.Name == m.Field).Display}-{m.Operate.ToDescription()}", ", ")}' There are duplicate rules, please remove duplicates");
                }
            }
            OperationResult result;
            if (group.Groups.Count > 0)
            {
                foreach (FilterGroup g in group.Groups)
                {
                    result = CheckFilterGroup(g, entityInfo);
                    if (!result.Successed)
                    {
                        return result;
                    }
                }
            }
            Type entityType = Type.GetType(entityInfo.TypeName);
            result = FilterHelper.CheckFilterGroup(group, entityType);
            if (!result.Successed)
            {
                return result;
            }

            return OperationResult.Success;
        }

        #endregion
    }
}
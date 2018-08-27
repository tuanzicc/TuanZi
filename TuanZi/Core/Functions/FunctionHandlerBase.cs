using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Exceptions;
using TuanZi.Reflection;


namespace TuanZi.Core.Functions
{

    public abstract class FunctionHandlerBase<TFunction> : IFunctionHandler
      where TFunction : class, IEntity<Guid>, IFunction, new()
    {
        private readonly List<TFunction> _functions = new List<TFunction>();

        protected FunctionHandlerBase()
        {
            Logger = ServiceLocator.Instance.GetLogger(GetType());
        }

        protected ILogger Logger { get; }

        public abstract IFunctionTypeFinder FunctionTypeFinder { get; }

        public abstract IMethodInfoFinder MethodInfoFinder { get; }

        public void Initialize()
        {
            Check.NotNull(FunctionTypeFinder, nameof(FunctionTypeFinder));

            Type[] functionTypes = FunctionTypeFinder.FindAll(true);
            TFunction[] functions = GetFunctions(functionTypes);
            Logger.LogInformation($"Function information is initialized, and a total of {functions.Length} function information is found.");

            ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                SyncToDatabase(provider, functions);
            });

            RefreshCache();
        }

        public IFunction GetFunction(string area, string controller, string action)
        {
            if (_functions.Count == 0)
            {
                RefreshCache();
            }
            return _functions.FirstOrDefault(m =>
                string.Equals(m.Area, area, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Controller, controller, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Action, action, StringComparison.OrdinalIgnoreCase));
        }

        public void RefreshCache()
        {
            ServiceLocator.Instance.ExcuteScopedWork(provider =>
            {
                _functions.Clear();
                _functions.AddRange(GetFromDatabase(provider));
            });
        }

        public void ClearCache()
        {
            _functions.Clear();
        }

        protected virtual TFunction[] GetFunctions(Type[] functionTypes)
        {
            List<TFunction> functions = new List<TFunction>();
            foreach (Type type in functionTypes.OrderBy(m => m.FullName))
            {
                TFunction controller = GetFunction(type);
                if (controller == null)
                {
                    continue;
                }
                if (!HasPickup(functions, controller))
                {
                    functions.Add(controller);
                }
                MethodInfo[] methods = MethodInfoFinder.FindAll(type);
                foreach (MethodInfo method in methods)
                {
                    TFunction action = GetFunction(controller, method);
                    if (action == null)
                    {
                        continue;
                    }
                    if (IsIgnoreMethod(action, method, functions))
                    {
                        continue;
                    }
                    functions.Add(action);
                }
            }
            return functions.ToArray();
        }

        protected abstract TFunction GetFunction(Type type);

        protected abstract TFunction GetFunction(TFunction typeFunction, MethodInfo method);

        protected virtual bool HasPickup(List<TFunction> functions, TFunction function)
        {
            return functions.Any(m =>
                string.Equals(m.Area, function.Area, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Controller, function.Controller, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Action, function.Action, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Name, function.Name, StringComparison.OrdinalIgnoreCase));
        }

        protected virtual TFunction GetFunction(IEnumerable<TFunction> functions, string area, string controller, string action, string name)
        {
            return functions.FirstOrDefault(m =>
                string.Equals(m.Area, area, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Controller, controller, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Action, action, StringComparison.OrdinalIgnoreCase)
                && string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        protected virtual bool IsIgnoreMethod(TFunction action, MethodInfo method, IEnumerable<TFunction> functions)
        {
            TFunction exist = GetFunction(functions, action.Area, action.Controller, action.Action, action.Name);
            return exist != null;
        }

        protected virtual void SyncToDatabase(IServiceProvider scopedProvider, TFunction[] functions)
        {

            Check.NotNull(functions, nameof(functions));
            if (functions.Length == 0)
            {
                return;
            }

            if (!functions.CheckSyncByHash(scopedProvider, Logger))
            {
                return;
            }

            IRepository<TFunction, Guid> repository = scopedProvider.GetService<IRepository<TFunction, Guid>>();
            if (repository == null)
            {
                throw new TuanException("The service of IRepository<,> is not found, please initialize the EntityPack module");
            }
            TFunction[] dbItems = repository.TrackQuery().ToArray();

            TFunction[] removeItems = dbItems.Except(functions,
                EqualityHelper<TFunction>.CreateComparer(m => m.Area + m.Controller + m.Action)).ToArray();
            int removeCount = removeItems.Length;
            repository.Delete(removeItems);

            TFunction[] addItems = functions.Except(dbItems,
                EqualityHelper<TFunction>.CreateComparer(m => m.Area + m.Controller + m.Action)).ToArray();
            int addCount = addItems.Length;
            repository.Insert(addItems);

            int updateCount = 0; return;
            foreach (TFunction item in dbItems.Except(removeItems))
            {
                bool isUpdate = false;
                TFunction function;
                try
                {
                    function = functions.Single(m =>
                        string.Equals(m.Area, item.Area, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(m.Controller, item.Controller, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(m.Action, item.Action, StringComparison.OrdinalIgnoreCase));
                }
                catch (InvalidOperationException)
                {
                    throw new TuanException($"Found more than one '{item.Area}-{item.Controller}-{item.Action}' function information that is not allowed");
                }
                if (function == null)
                {
                    continue;
                }
                if (!string.Equals(item.Name, function.Name, StringComparison.OrdinalIgnoreCase))
                {
                    item.Name = function.Name;
                    isUpdate = true;
                }
                if (item.IsAjax != function.IsAjax)
                {
                    item.IsAjax = function.IsAjax;
                    isUpdate = true;
                }
                if (!item.IsAccessTypeChanged && item.AccessType != function.AccessType)
                {
                    item.AccessType = function.AccessType;
                    isUpdate = true;
                }
                if (isUpdate)
                {
                    repository.Update(item);
                    updateCount++;
                    Logger.LogDebug($"Updated Function '{function.Name}({function.Area}/{function.Controller}/{function.Action})'");
                }
            }
            repository.UnitOfWork.Commit();
            if (removeCount + addCount + updateCount > 0)
            {
                string msg = "Function Changes";
                if (addCount > 0)
                {
                    foreach (TFunction function in addItems)
                    {
                        Logger.LogDebug($"Added Function '{function.Name}({function.Area}/{function.Controller}/{function.Action})'");
                    }
                    msg += "，Added " + addCount;
                }
                if (updateCount > 0)
                {
                    msg += "，Updated " + updateCount;
                }
                if (removeCount > 0)
                {
                    foreach (TFunction function in removeItems)
                    {
                        Logger.LogDebug($"Deleted Function '{function.Name}({function.Area}/{function.Controller}/{function.Action})'");
                    }
                    msg += "，Deleted " + removeCount;
                }
                Logger.LogInformation(msg);
            }
        }

        protected virtual TFunction[] GetFromDatabase(IServiceProvider scopedProvider)
        {
            IRepository<TFunction, Guid> repository = scopedProvider.GetService<IRepository<TFunction, Guid>>();
            return repository.Query().ToArray();
        }

    }



}
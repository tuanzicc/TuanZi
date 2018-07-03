

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;


namespace TuanZi.Security
{
    [IgnoreDependency]
    public interface IFunctionStore<TFunction, in TFunctionInputDto>
       where TFunction : IFunction
       where TFunctionInputDto : FunctionInputDtoBase
    {
        IQueryable<TFunction> Functions { get; }

        Task<bool> CheckFunctionExists(Expression<Func<TFunction, bool>> predicate, Guid id = default(Guid));

        Task<OperationResult> UpdateFunctions(params TFunctionInputDto[] dtos);

    }
}
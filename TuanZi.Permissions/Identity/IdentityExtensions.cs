using System.Linq;

using Microsoft.AspNetCore.Identity;

using TuanZi.Collections;
using TuanZi.Data;


namespace TuanZi.Identity
{
    public static class IdentityExtensions
    {
        public static OperationResult ToOperationResult(this IdentityResult identityResult)
        {
            return identityResult.Succeeded
                ? new OperationResult(OperationResultType.Success)
                : new OperationResult(OperationResultType.Error, identityResult.Errors.Select(m => $"{m.Code}:{m.Description}").ExpandAndToString());
        }

        public static IdentityResult Failed(this IdentityResult identityResult, params string[] errors)
        {
            var identityErrors = identityResult.Errors;
            identityErrors = identityErrors.Union(errors.Select(m => new IdentityError() { Description = m }));
            return IdentityResult.Failed(identityErrors.ToArray());
        }
    }
}
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

        public static OperationResult<TUser> ToOperationResult<TUser>(this IdentityResult identityResult, TUser user)
        {
            return identityResult.Succeeded
                ? new OperationResult<TUser>(OperationResultType.Success, "Success", user)
                : new OperationResult<TUser>(OperationResultType.Error,
                    identityResult.Errors.Select(m => m.Code.IsMissing() ? m.Description : $"{m.Code}:{m.Description}").ExpandAndToString());
        }


        public static IdentityResult Failed(this IdentityResult identityResult, params string[] errors)
        {
            var identityErrors = identityResult.Errors;
            identityErrors = identityErrors.Union(errors.Select(m => new IdentityError() { Description = m }));
            return IdentityResult.Failed(identityErrors.ToArray());
        }
    }
}
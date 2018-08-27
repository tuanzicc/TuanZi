using System.Security.Principal;

using TuanZi.Core.Functions;


namespace TuanZi.Secutiry
{
    public interface IFunctionAuthorization
    {
        AuthorizationResult Authorize(IFunction function, IPrincipal principal);

        string[] GetOkRoles(IFunction function, IPrincipal principal);
    }
}
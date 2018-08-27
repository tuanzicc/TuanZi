using TuanZi.Core.Functions;


namespace TuanZi.Secutiry
{
    public class FunctionAuthorization : FunctionAuthorizationBase
    {
        public FunctionAuthorization(IFunctionAuthCache functionAuthCache)
            : base(functionAuthCache)
        { }
    }
}
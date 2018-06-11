using System;


namespace TuanZi.Secutiry
{
    public interface IFunctionAuthCache
    {
        void BuildRoleCaches();

        void RemoveFunctionCaches(params Guid[] functionIds);

        void RemoveUserCaches(params string[] userNames);

        string[] GetFunctionRoles(Guid functionId);

        Guid[] GetUserFunctions(string userName);
    }
}
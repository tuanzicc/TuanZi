using TuanZi.Filter;


namespace TuanZi.Secutiry
{
    public interface IDataAuthCache
    {
        void BuildCaches();

        void SetCache(DataAuthCacheItem item);

        void RemoveCache(string roleName, string entityTypeFullName, DataAuthOperation operation);

        FilterGroup GetFilterGroup(string roleName, string entityTypeFullName, DataAuthOperation operation);
    }
}
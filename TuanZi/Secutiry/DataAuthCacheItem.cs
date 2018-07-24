using TuanZi.Filter;


namespace TuanZi.Secutiry
{
    public class DataAuthCacheItem
    {
        public string RoleName { get; set; }

        public string EntityTypeFullName { get; set; }

        public DataAuthOperation Operation { get; set; }

        public FilterGroup FilterGroup { get; set; }
    }
}
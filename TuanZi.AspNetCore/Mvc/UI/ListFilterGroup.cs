using Microsoft.AspNetCore.Http;

using TuanZi.AspNetCore.Http;
using TuanZi.Extensions;
using TuanZi.Filter;
using TuanZi.Json;


namespace TuanZi.AspNetCore.UI
{
    public class ListFilterGroup : FilterGroup
    {
        public ListFilterGroup(HttpRequest request)
        {
            string jsonGroup = request.Params("filter_group");
            if (jsonGroup.IsNullOrEmpty())
            {
                return;
            }
            FilterGroup group = JsonHelper.FromJson<FilterGroup>(jsonGroup);
            Rules = group.Rules;
            Groups = group.Groups;
            Operate = group.Operate;
        }
    }
}
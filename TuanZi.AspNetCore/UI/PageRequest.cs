using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.AspNetCore.Http;

using TuanZi.AspNetCore.Http;
using TuanZi.Filter;
using TuanZi.Json;


namespace TuanZi.AspNetCore.UI
{
    public class PageRequest
    {
        public PageRequest(HttpRequest request)
        {
            string jsonGroup = request.Params("filter_group");
            FilterGroup = !jsonGroup.IsNullOrEmpty() ? JsonHelper.FromJson<FilterGroup>(jsonGroup) : new FilterGroup();

            int pageIndex = request.Params("pageIndex").CastTo(1);
            int pageSize = request.Params("pageSize").CastTo(20);
            PageCondition = new PageCondition(pageIndex, pageSize);
            string sortField = request.Params("sortField");
            string sortOrder = request.Params("sortOrder");
            if (!sortField.IsNullOrEmpty() && !sortOrder.IsNullOrEmpty())
            {
                string[] files = sortField.Split(",", true);
                string[] orders = sortOrder.Split(",", true);
                if (files.Length != orders.Length)
                {
                    throw new ArgumentException("The number of column names and direction parameters in the query list is inconsistent");
                }
                List<SortCondition> sortConditions = new List<SortCondition>();
                for (int i = 0; i < files.Length; i++)
                {
                    ListSortDirection direction = orders[i].ToLower() == "desc" ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    sortConditions.Add(new SortCondition(files[i], direction));
                }
                PageCondition.SortConditions = sortConditions.ToArray();
            }
            else
            {
                PageCondition.SortConditions = new SortCondition[0];
            }
        }

        public FilterGroup FilterGroup { get; }

        public PageCondition PageCondition { get; }

        public void AddDefaultSortCondition(params SortCondition[] conditions)
        {
            Check.NotNullOrEmpty(conditions, nameof(conditions));

            if (PageCondition.SortConditions.Length == 0)
            {
                PageCondition.SortConditions = conditions;
            }
        }
    }
}
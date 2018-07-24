using TuanZi.Data;


namespace TuanZi.Filter
{
    public class PageRequest
    {
        public PageRequest()
        {
            PageCondition = new PageCondition(1, 20);
            FilterGroup = new FilterGroup();
        }

        public PageCondition PageCondition { get; set; }

        public FilterGroup FilterGroup { get; set; }

        public void AddDefaultSortCondition(params SortCondition[] sortConditions)
        {
            Check.NotNullOrEmpty(sortConditions, nameof(sortConditions));
            if (PageCondition.SortConditions.Length == 0)
            {
                PageCondition.SortConditions = sortConditions;
            }
        }
    }
}
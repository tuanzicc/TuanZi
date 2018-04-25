namespace TuanZi.Filter
{
    public class PageCondition
    {
        public PageCondition()
            : this(1, 20)
        { }

        public PageCondition(int pageIndex, int pageSize)
        {
            pageIndex.CheckGreaterThan("pageIndex", 0);
            pageSize.CheckGreaterThan("pageSize", 0);
            PageIndex = pageIndex;
            PageSize = pageSize;
            SortConditions = new SortCondition[] { };
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public SortCondition[] SortConditions { get; set; }
    }
}
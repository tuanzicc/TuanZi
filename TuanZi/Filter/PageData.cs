using System.Collections.Generic;


namespace TuanZi.Filter
{
    public class PageData<T>
    {
        public PageData()
            : this(new List<T>(), 0)
        { }

        public PageData(IEnumerable<T>rows, int total)
        {
            Rows = rows;
            Total = total;
        }

        public IEnumerable<T> Rows { get; set; }

        public int Total { get; set; }
    }
}
namespace TuanZi.Filter
{
    public class PageResult<T>
    {
        public PageResult()
            : this(new T[0], 0)
        { }

        public PageResult(T[] data, int total)
        {
            Data = data;
            Total = total;
        }

        public T[] Data { get; set; }

        public int Total { get; set; }

        public PageData<T> ToPageData()
        {
            return new PageData<T>(Data, Total);
        }
    }
}
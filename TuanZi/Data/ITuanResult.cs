namespace TuanZi.Data
{
    public interface ITuanResult<TResultType> : ITuanResult<TResultType, object>
    { }


    public interface ITuanResult<TResultType, TData>
    {
        TResultType ResultType { get; set; }

        string Message { get; set; }

        TData Data { get; set; }
    }
}
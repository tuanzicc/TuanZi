using TuanZi.Reflection;

namespace TuanZi.Data
{
    public class OperationResult : OperationResult<object>
    {
        static OperationResult()
        {
            Success = new OperationResult(OperationResultType.Success);
            NoChanged = new OperationResult(OperationResultType.NoChanged);
        }

        public OperationResult()
            : this(OperationResultType.NoChanged)
        { }

        public OperationResult(OperationResultType resultType)
            : this(resultType, null, null)
        { }

        public OperationResult(OperationResultType resultType, string message)
            : this(resultType, message, null)
        { }

        public OperationResult(OperationResultType resultType, string message, object data)
            : base(resultType, message, data)
        { }

        public static OperationResult Success { get; private set; }

        public new static OperationResult NoChanged { get; private set; }
    }


    public class OperationResult<TData> : TuanResult<OperationResultType, TData>
    {
        static OperationResult()
        {
            NoChanged = new OperationResult<TData>(OperationResultType.NoChanged);
        }

        public OperationResult()
            : this(OperationResultType.NoChanged)
        { }

        public OperationResult(OperationResultType resultType)
            : this(resultType, null, default(TData))
        { }

        public OperationResult(OperationResultType resultType, string message)
            : this(resultType, message, default(TData))
        { }

        public OperationResult(OperationResultType resultType, string message, TData data)
            : base(resultType, message, data)
        { }

        public override string Message
        {
            get { return _message ?? ResultType.ToDescription(); }
            set { _message = value; }
        }

        public static OperationResult<TData> NoChanged { get; private set; }

        public bool Successed
        {
            get { return ResultType == OperationResultType.Success; }
        }
    }
}
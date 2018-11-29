using System;
using TuanZi.Extensions;

namespace TuanZi.Data
{
    public abstract class TuanResult<TResultType> : TuanResult<TResultType, object>, ITuanResult<TResultType>
    {
        protected TuanResult()
            : this(default(TResultType))
        { }

        protected TuanResult(TResultType type)
            : this(type, null, null)
        { }

        protected TuanResult(TResultType type, string message)
            : this(type, message, null)
        { }

        protected TuanResult(TResultType type, string message, object data)
            : base(type, message, data)
        { }
    }


    public abstract class TuanResult<TResultType, TData> : ITuanResult<TResultType, TData>
    {
        protected string _message;

        protected TuanResult()
            : this(default(TResultType))
        { }

        protected TuanResult(TResultType type)
            : this(type, null, default(TData))
        { }

        protected TuanResult(TResultType type, string message)
            : this(type, message, default(TData))
        { }

        protected TuanResult(TResultType type, string message, TData data)
        {
            if (message == null && typeof(TResultType).IsEnum)
            {
                message = (type as Enum)?.ToDescription();
            }
            ResultType = type;
            _message = message;
            Data = data;
        }

        public TResultType ResultType { get; set; }

        public virtual string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public TData Data { get; set; }
    }
}
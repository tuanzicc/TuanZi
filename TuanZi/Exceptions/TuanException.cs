using System;
using System.Runtime.Serialization;


namespace TuanZi.Exceptions
{
    [Serializable]
    public class TuanException : Exception
    {
        public TuanException()
        { }
        
        public TuanException(string message)
            : base(message)
        { }
        
        public TuanException(string message, Exception inner)
            : base(message, inner)
        { }
        
        protected TuanException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
using System;
using System.Runtime.Serialization;


namespace TuanZi.Exceptions
{
    [Serializable]
    public class SessionExpiredException : Exception
    {
        public SessionExpiredException()
        { }
        
        public SessionExpiredException(string message)
            : base(message)
        { }
        
        public SessionExpiredException(string message, Exception inner)
            : base(message, inner)
        { }
        
        protected SessionExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
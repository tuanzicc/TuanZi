using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TuanZi.Web
{
    [Serializable]
    public class SessionExpiredException : WarningException
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

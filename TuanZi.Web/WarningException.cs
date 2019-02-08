using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TuanZi.Web
{
    [Serializable]
    public class WarningException : Exception
    {
        public WarningException()
        { }

        public WarningException(string message)
            : base(message)
        { }

        public WarningException(string message, Exception inner)
            : base(message, inner)
        { }

        protected WarningException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

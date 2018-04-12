namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class InternalException : Exception
    {
        public InternalException() : this(string.Empty)
        {
        }

        public InternalException(string message) : base(message + Environment.NewLine + "LiveLinq internal exception.")
        {
        }

        protected InternalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InternalException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}


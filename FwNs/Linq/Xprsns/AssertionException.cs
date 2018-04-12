namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class AssertionException : InternalException
    {
        public AssertionException()
        {
        }

        public AssertionException(string message) : base(message)
        {
        }

        protected AssertionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}


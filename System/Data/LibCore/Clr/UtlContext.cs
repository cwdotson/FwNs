namespace System.Data.LibCore.Clr
{
    using FwNs.Core.LC.cEngine;
    using System;

    public class UtlContext
    {
        [ThreadStatic]
        public static Session session;
        [ThreadStatic]
        public static UtlPipe pipe;

        public static bool IsAvailable
        {
            get
            {
                return (session > null);
            }
        }

        public static UtlPipe Pipe
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException();
                }
                if ((pipe == null) || (pipe.GetSession() != session))
                {
                    pipe = new UtlPipe(session);
                }
                return pipe;
            }
        }
    }
}


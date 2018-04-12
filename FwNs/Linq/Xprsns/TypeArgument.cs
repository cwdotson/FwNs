namespace FwNs.Linq.Xprsns
{
    using FwNs.Core.Typs;
    using System;
    using System.Diagnostics;

    public class TypeArgument
    {
        public static void IsDelegate<T>(string argName)
        {
            if (!TypsFw.IsDelegate(typeof(T)))
            {
                throw new ArgumentException("EMCommon.TypeArgument_NotDelegate", argName);
            }
        }

        [DebuggerStepThrough]
        public static void ThrowIf(bool condition, string argName)
        {
            if (condition)
            {
                throw new ArgumentException(argName);
            }
        }

        [DebuggerStepThrough]
        public static void ThrowIf(bool condition, string argName, string message)
        {
            if (condition)
            {
                throw new ArgumentException(message, argName);
            }
        }
    }
}


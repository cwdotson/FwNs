namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    internal class Assert
    {
        [Conditional("DEBUG"), DebuggerStepThrough]
        public static void Fail(string message)
        {
            Trayss.FlushBuffer();
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw new AssertionException();
                }
                throw new AssertionException(message);
            }
        }

        public static void IsNullOrIs<T>(LambdaExpression lambdaExpression)
        {
            throw new NotImplementedException();
        }

        public static void IsValidEnum<T>(T enumValue) where T: struct
        {
        }
    }
}


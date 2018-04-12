namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class Argument
    {
        [Extension, DebuggerStepThrough]
        public static T ArgNotNull<T>(T arg, string argName)
        {
            NotNull<T>(arg, argName);
            return arg;
        }

        [DebuggerStepThrough]
        public static void ArrayNotEmpty(Array array, string argName)
        {
            NotNull<Array>(array, argName);
            if (array.Length == 0)
            {
                throw new ArgumentException(Errorz.Argument_ArrayIsEmpty, argName);
            }
        }

        [DebuggerStepThrough]
        public static void Between(int value, int min, int max, string argName)
        {
            NotLess(value, min, argName);
            NotGreater(value, max, argName);
        }

        [DebuggerStepThrough]
        public static void Between<T>(T value, T min, T max, string argName) where T: IComparable<T>
        {
            NotLess<T>(value, min, argName);
            NotGreater<T>(value, max, argName);
        }

        [DebuggerStepThrough]
        public static void CollectionNotEmpty(ICollection coll, string argName)
        {
            NotNull<ICollection>(coll, argName);
            if (coll.Count == 0)
            {
                throw new ArgumentException(Errorz.Argument_ArrayIsEmpty, argName);
            }
        }

        [DebuggerStepThrough]
        public static void NotGreater(int arg, int max, string argName)
        {
            if (arg > max)
            {
                throw new ArgumentOutOfRangeException(Xtnz.InvariantFormat(Errorz.Argument_NotGreater, argName, max));
            }
        }

        [DebuggerStepThrough]
        public static void NotGreater<T>(T arg, T max, string argName) where T: IComparable<T>
        {
            if (arg.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(Xtnz.InvariantFormat(Errorz.Argument_NotGreater, argName, max));
            }
        }

        [DebuggerStepThrough]
        public static void NotLess(int arg, int min, string argName)
        {
            if (arg < min)
            {
                throw new ArgumentOutOfRangeException(Xtnz.InvariantFormat(Errorz.Argument_NotLess, argName, min));
            }
        }

        [DebuggerStepThrough]
        public static void NotLess<T>(T arg, T min, string argName) where T: IComparable<T>
        {
            if (arg.CompareTo(min) < 0)
            {
                throw new ArgumentOutOfRangeException(Xtnz.InvariantFormat(Errorz.Argument_NotLess, argName, min));
            }
        }

        [DebuggerStepThrough]
        public static void NotNull<T>(T arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string arg, string argName)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentNullException(argName);
            }
        }

        [DebuggerStepThrough]
        public static void NotNullOrEmpty<T>(T[] array, string argName)
        {
            NotNull<T[]>(array, argName);
            if (array.Length == 0)
            {
                throw new ArgumentException(Errorz.Argument_ArrayIsEmpty, argName);
            }
        }

        [DebuggerStepThrough]
        public static void OfType<T>(Expression expr, string argName)
        {
            OfType(expr, typeof(T), argName);
        }

        [DebuggerStepThrough]
        public static void OfType(Expression expr, Type exprType, string argName)
        {
            NotNull<Expression>(expr, argName);
            if (expr.Type != exprType)
            {
                throw new ArgumentException("EMExpressions.Func_DifferentTypeValues.InvariantFormat(expr)");
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

        [DebuggerStepThrough]
        public static void ThrowIf(bool condition, string argName, string messageFormat, object formatArg1)
        {
            if (condition)
            {
                throw new ArgumentException(Xtnz.InvariantFormat(messageFormat, formatArg1), argName);
            }
        }

        [DebuggerStepThrough]
        public static void ThrowIf(bool condition, string argName, string messageFormat, params object[] formatArgs)
        {
            if (condition)
            {
                throw new ArgumentException(Xtnz.InvariantFormat(messageFormat, formatArgs), argName);
            }
        }

        [DebuggerStepThrough]
        public static void ThrowIf(bool condition, string argName, string messageFormat, object formatArg1, object formatArg2)
        {
            if (condition)
            {
                throw new ArgumentException(Xtnz.InvariantFormat(messageFormat, formatArg1, formatArg2), argName);
            }
        }
    }
}


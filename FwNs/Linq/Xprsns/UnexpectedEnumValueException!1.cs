namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class UnexpectedEnumValueException<T> : Exception where T: struct, IComparable
    {
        public UnexpectedEnumValueException(T value) : base(UnexpectedEnumValueException<T>.GetMessage(value))
        {
            Argument.ThrowIf(typeof(T).IsEnum, "value", "Value is not enum");
            this.Value = value;
        }

        private UnexpectedEnumValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Value = (T) info.GetValue("Value", typeof(T));
        }

        private static string GetMessage(T value)
        {
            return Xtnz.InvariantFormat(Errorz.Errors_UnexpectedEnumValue, string.Format("{0}.{1}", typeof(T).FullName, value));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", this.Value);
        }

        public T Value { get; private set; }
    }
}


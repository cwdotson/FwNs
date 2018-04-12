namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class SourceChangeEventArgs<T> : EventArgs, IEquatable<SourceChangeEventArgs<T>>
    {
        internal static readonly SourceChangeEventArgs<T> Reset;

        static SourceChangeEventArgs()
        {
            SourceChangeEventArgs<T>.Reset = new SourceChangeEventArgs<T>(default(T), SourceChangeType.Reset, -1);
        }

        public SourceChangeEventArgs(T item, SourceChangeType changeType, int ordinal)
        {
            this.Item = item;
            this.ChangeType = changeType;
            this.Ordinal = ordinal;
        }

        public bool Equals(SourceChangeEventArgs<T> args)
        {
            if (args == null)
            {
                return false;
            }
            return (((args.ChangeType == SourceChangeType.Reset) && (this.ChangeType == SourceChangeType.Reset)) || ((args.ChangeType == this.ChangeType) && EqualityComparer<T>.Default.Equals(args.Item, this.Item)));
        }

        public override bool Equals(object obj)
        {
            SourceChangeEventArgs<T> args = obj as SourceChangeEventArgs<T>;
            return ((args != null) && this.Equals(args));
        }

        public override int GetHashCode()
        {
            if (this.ChangeType == SourceChangeType.Reset)
            {
                return 0;
            }
            int hashCode = this.ChangeType.GetHashCode();
            if (this.Item != null)
            {
                hashCode ^= this.Item.GetHashCode();
            }
            return hashCode;
        }

        public override string ToString()
        {
            return string.Format("{0} Item = {1}, ChangeType = {2}, Ordinal = {3} {4}", new object[] { '{', this.Item, this.ChangeType, this.Ordinal, '}' });
        }

        internal virtual object Details { get; set; }

        public T Item { get; private set; }

        public SourceChangeType ChangeType { get; private set; }

        public int Ordinal { get; private set; }
    }
}


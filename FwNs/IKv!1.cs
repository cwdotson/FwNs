namespace FwNs
{
    using System;

    public interface IKv<TK> : IKv<TK, object>, IKv<TK, object, IComparable>, IKeyVal<TK, object, IComparable>, IEquatable<KeyValuePair<TK, object>>, IComparable<KeyValuePair<TK, object>>, IEquatable<IKeyVal<TK, object, IComparable>>, IComparable<IKeyVal<TK, object, IComparable>>, IEquatable<Tuple<TK, object, IComparable>>, IComparable<Tuple<TK, object, IComparable>>, IComparable
    {
    }
}


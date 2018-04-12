namespace FwNs
{
    using System;

    public interface IKeyVal<TK> : IKeyVal<TK, object>, IKeyVal<TK, object, IComparable>, IEquatable<KeyValuePair<TK, object>>, IComparable<KeyValuePair<TK, object>>, IEquatable<IKeyVal<TK, object, IComparable>>, IComparable<IKeyVal<TK, object, IComparable>>, IEquatable<Tuple<TK, object, IComparable>>, IComparable<Tuple<TK, object, IComparable>>, IComparable
    {
    }
}


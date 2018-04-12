namespace FwNs
{
    using System;

    public interface IKeyVal<TK, TV> : IKeyVal<TK, TV, IComparable>, IEquatable<KeyValuePair<TK, TV>>, IComparable<KeyValuePair<TK, TV>>, IEquatable<IKeyVal<TK, TV, IComparable>>, IComparable<IKeyVal<TK, TV, IComparable>>, IEquatable<Tuple<TK, TV, IComparable>>, IComparable<Tuple<TK, TV, IComparable>>, IComparable
    {
    }
}


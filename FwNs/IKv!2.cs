namespace FwNs
{
    using System;

    public interface IKv<K, V> : IKv<K, V, IComparable>, IKeyVal<K, V, IComparable>, IEquatable<KeyValuePair<K, V>>, IComparable<KeyValuePair<K, V>>, IEquatable<IKeyVal<K, V, IComparable>>, IComparable<IKeyVal<K, V, IComparable>>, IEquatable<Tuple<K, V, IComparable>>, IComparable<Tuple<K, V, IComparable>>, IComparable
    {
    }
}


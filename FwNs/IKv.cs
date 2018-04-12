namespace FwNs
{
    using System;

    public interface IKv : IKv<IComparable>, IKv<IComparable, object>, IKv<IComparable, object, IComparable>, IKeyVal<IComparable, object, IComparable>, IEquatable<KeyValuePair<IComparable, object>>, IComparable<KeyValuePair<IComparable, object>>, IEquatable<IKeyVal<IComparable, object, IComparable>>, IComparable<IKeyVal<IComparable, object, IComparable>>, IEquatable<Tuple<IComparable, object, IComparable>>, IComparable<Tuple<IComparable, object, IComparable>>, IComparable
    {
    }
}


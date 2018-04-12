namespace FwNs
{
    using System;

    public interface IKeyVal : IKeyVal<object>, IKeyVal<object, object>, IKeyVal<object, object, IComparable>, IEquatable<KeyValuePair<object, object>>, IComparable<KeyValuePair<object, object>>, IEquatable<IKeyVal<object, object, IComparable>>, IComparable<IKeyVal<object, object, IComparable>>, IEquatable<Tuple<object, object, IComparable>>, IComparable<Tuple<object, object, IComparable>>, IComparable
    {
    }
}


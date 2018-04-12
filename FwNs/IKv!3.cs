namespace FwNs
{
    using System;

    public interface IKv<TK, TV, TC> : IKeyVal<TK, TV, TC>, IEquatable<KeyValuePair<TK, TV>>, IComparable<KeyValuePair<TK, TV>>, IEquatable<IKeyVal<TK, TV, TC>>, IComparable<IKeyVal<TK, TV, TC>>, IEquatable<Tuple<TK, TV, TC>>, IComparable<Tuple<TK, TV, TC>>, IComparable where TC: IComparable
    {
        TK Kee { get; set; }

        TV Val { get; set; }
    }
}


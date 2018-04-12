namespace FwNs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;

    public interface IKeyVal<TK, TV, TC> : IEquatable<KeyValuePair<TK, TV>>, IComparable<KeyValuePair<TK, TV>>, IEquatable<IKeyVal<TK, TV, TC>>, IComparable<IKeyVal<TK, TV, TC>>, IEquatable<Tuple<TK, TV, TC>>, IComparable<Tuple<TK, TV, TC>>, IComparable where TC: IComparable
    {
        event EventHandler ValChnged;

        Dictionary<string, object> AsDictionary();
        KeyValuePair<TC, Dictionary<string, object>> AsKvpDict();
        KeyValuePair<TC, KeyValuePair<TK, TV>> AsKvpKvp();
        XElement AsXElement();
        int CompareTo(object obj);
        int CompareTo(TC other);
        string ToString();
        void vChngedHndlr();

        TK Key { get; }

        Func<TK, TC> Key2CmprbleFunc { get; set; }

        Func<DataTable, DataRow> Kv2Drf { get; }

        Stack<Func<IKeyVal<TK, TV, TC>, string>> ToStrngStck { get; set; }

        Stack<Func<TV, string>> tV2Stck { get; set; }

        TV Value { get; set; }
    }
}


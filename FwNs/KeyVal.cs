namespace FwNs
{
    using System;
    using System.Collections.Generic;

    public class KeyVal : KeyVal<string, object, IComparable>
    {
        public KeyVal(KeyValuePair<string, object> kvp) : base(kvp)
        {
        }

        public KeyVal(KeyValuePair<string, object> kvp, Func<string, IComparable> key2CmprbleFunc) : base(kvp, key2CmprbleFunc)
        {
        }

        public KeyVal(KeyValuePair<string, object> kvp, IComparable tC) : base(kvp, tC)
        {
        }

        public KeyVal(string tK, object tV) : base(tK, tV)
        {
        }

        public KeyVal(string tK, object tV, Func<string, IComparable> key2CmprbleFunc) : base(tK, tV, key2CmprbleFunc)
        {
        }

        public KeyVal(string tK, object tV, IComparable tC) : base(tK, tV, tC)
        {
        }
    }
}


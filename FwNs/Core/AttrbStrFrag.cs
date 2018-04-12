namespace FwNs.Core
{
    using System;

    public class AttrbStrFrag : AttrbStr
    {
        public AttrbStrFrag(string s) : base("{0}{1}", "", s)
        {
        }
    }
}


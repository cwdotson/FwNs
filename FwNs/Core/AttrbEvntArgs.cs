namespace FwNs.Core
{
    using System;
    using System.Runtime.CompilerServices;

    public class AttrbEvntArgs : EventArgs
    {
        public AttrbEvntArgs(string nm)
        {
            this.EvntNm = nm;
        }

        public string EvntNm { get; set; }
    }
}


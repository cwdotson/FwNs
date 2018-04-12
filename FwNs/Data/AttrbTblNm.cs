namespace FwNs.Data
{
    using FwNs.Core;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class AttrbTblNm : AttrbStr
    {
        private IEnumerable<EnmTblNm> _kydTyps;

        public AttrbTblNm(string s, int prntTypId) : base("{0}{1}", "", s)
        {
            this.PrntTyp = (EnmTblNm) prntTypId;
        }

        public AttrbTblNm(string s, int prntTypId, string kydTyps) : this(s, prntTypId)
        {
            this.KydTypsStrng = kydTyps;
        }

        public EnmTblNm PrntTyp { get; set; }

        public string KydTypsStrng { get; set; }
    }
}


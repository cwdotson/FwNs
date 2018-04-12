namespace FwNs.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class DbRootCtxt : IDbRootCtxt
    {
        private StringBuilder _ctxtSb;
        private ICollection<Action> _actionCollection;

        public static void AddPropNms()
        {
        }

        public static string[,] FtypRsrcs()
        {
            throw new Exception();
        }

        public void LoadActionsTest000(WythPrnt wythPrnt)
        {
            throw new Exception();
        }

        public void Maintnce_FTyp_Ef1()
        {
            throw new Exception();
        }

        public void SetDbRootsCtxts()
        {
            throw new Exception();
        }

        public int AktnLmt { get; set; }

        public StringBuilder CtxtSb
        {
            get
            {
                if (this._ctxtSb == null)
                {
                    this._ctxtSb = new StringBuilder();
                }
                return this._ctxtSb;
            }
            set
            {
                this._ctxtSb = value;
            }
        }

        public ICollection<Action> ActionCollection
        {
            get
            {
                if (this._actionCollection == null)
                {
                    this._actionCollection = new Collection<Action>();
                }
                return this._actionCollection;
            }
            set
            {
                this._actionCollection = value;
            }
        }

        public EnmCtxtMode CtxtMode { get; set; }
    }
}


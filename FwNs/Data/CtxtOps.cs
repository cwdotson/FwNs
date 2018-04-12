namespace FwNs.Data
{
    using System;
    using System.Data.Linq;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class CtxtOps
    {
        public static Func<DbRootCtxt, bool> CntnuFunc;

        static CtxtOps()
        {
            CntnuFunc = new Func<DbRootCtxt, bool>(<>c.<>9, this.<.cctor>b__4_0);
        }

        [Extension]
        public static string GetConStrng(EnmCtxtMode mode)
        {
            if (mode != EnmCtxtMode.Test000)
            {
                if (mode != EnmCtxtMode.Test001)
                {
                    throw new Exception();
                }
            }
            else
            {
                return @"Data Source=XPS8300\SQL2K12;Initial Catalog=DbRootsV0IV;Integrated Security=True";
            }
            return @"Data Source=XPS8300\SQL2K12;Initial Catalog=DbRootsV0IV;Integrated Security=True";
        }

        [Extension]
        public static DataLoadOptions GetDataLoadOptns(EnmCtxtMode mode)
        {
            if (mode != EnmCtxtMode.Test000)
            {
                if (mode != EnmCtxtMode.Test001)
                {
                    throw new Exception();
                }
            }
            else
            {
                return GetDataLoadOptns(EnmDataLoadOptns.Test000);
            }
            return GetDataLoadOptns(EnmDataLoadOptns.Test001);
        }

        [Extension]
        public static DataLoadOptions GetDataLoadOptns(EnmDataLoadOptns mode)
        {
            // This item is obfuscated and can not be translated.
            throw new Exception();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly CtxtOps.<>c <>9 = new CtxtOps.<>c();

            internal bool <.cctor>b__4_0(DbRootCtxt ctxt)
            {
                if ((0 < ctxt.AktnLmt) && (ctxt.AktnLmt < ctxt.ActionCollection.Count))
                {
                    return false;
                }
                return true;
            }
        }
    }
}


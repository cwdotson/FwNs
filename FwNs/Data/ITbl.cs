namespace FwNs.Data
{
    using FwNs;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class ITbl : DataTable
    {
        private HashSet<IKv<string, Type>> colz;

        public ITbl(IKv<string, Type>[] colz)
        {
            this.colz = new HashSet<IKv<string, Type>>(colz);
            if (this.Colz.Count != base.Columns.Count)
            {
                throw new Exception();
            }
        }

        public HashSet<IKv<string, Type>> Colz
        {
            get
            {
                if (base.Columns.Count < this.colz.Count)
                {
                    base.Columns.AddRange(Enumerable.ToArray<DataColumn>(Enumerable.Select<IKv<string, Type>, DataColumn>(Enumerable.Where<IKv<string, Type>>(this.colz, new Func<IKv<string, Type>, bool>(this, this.<get_Colz>b__2_0)), <>c.<>9__2_1 ?? (<>c.<>9__2_1 = new Func<IKv<string, Type>, DataColumn>(<>c.<>9, this.<get_Colz>b__2_1)))));
                }
                return this.colz;
            }
            set
            {
                this.colz = value;
                if (!Enumerable.All<IKv<string, Type>>(this.colz, new Func<IKv<string, Type>, bool>(this, this.<set_Colz>b__3_0)))
                {
                    base.Columns.AddRange(Enumerable.ToArray<DataColumn>(Enumerable.Select<IKv<string, Type>, DataColumn>(Enumerable.Where<IKv<string, Type>>(this.colz, new Func<IKv<string, Type>, bool>(this, this.<set_Colz>b__3_1)), <>c.<>9__3_2 ?? (<>c.<>9__3_2 = new Func<IKv<string, Type>, DataColumn>(<>c.<>9, this.<set_Colz>b__3_2)))));
                }
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly ITbl.<>c <>9 = new ITbl.<>c();
            public static Func<IKv<string, Type>, DataColumn> <>9__2_1;
            public static Func<IKv<string, Type>, DataColumn> <>9__3_2;

            internal DataColumn <get_Colz>b__2_1(IKv<string, Type> u)
            {
                return new DataColumn(u.Kee, u.Val);
            }

            internal DataColumn <set_Colz>b__3_2(IKv<string, Type> u)
            {
                return new DataColumn(u.Kee, u.Val);
            }
        }
    }
}


namespace FwNs.Data
{
    using FwNs.Data.Mdls;
    using System;
    using System.Linq;

    public static class Xmpls
    {
        public static bool DataTestOvrd = false;
        public static object[,] DataTestSwtchs = new object[0, 0];

        public static object Test000()
        {
            using (DClsszDataContext context = new DClsszDataContext())
            {
                DbRoot root = Queryable.First<DbRoot>(context.DbRoots);
                for (int i = 0; i < root.CTyps.Count; i++)
                {
                    CTyp typ = Enumerable.First<CTyp>(Enumerable.Skip<CTyp>(root.CTyps, i));
                    for (int j = 0; j < typ.FTyps.Count; j++)
                    {
                        FTyp typ2 = Enumerable.First<FTyp>(Enumerable.Skip<FTyp>(typ.FTyps, j));
                        for (int k = 0; k < typ2.Itms.Count; k++)
                        {
                            Itm itm = Enumerable.First<Itm>(Enumerable.Skip<Itm>(typ2.Itms, k));
                            for (int m = 0; m < itm.Parts.Count; m++)
                            {
                                Enumerable.First<Part>(Enumerable.Skip<Part>(itm.Parts, m));
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static object Test000(object obj)
        {
            return null;
        }
    }
}


namespace FwNs
{
    using FwNs.DbRootsVII0004DataSetTableAdapters;
    using System;

    public class dsGettr
    {
        public static DbRootsVII0004DataSet getDbrDs()
        {
            DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
            new TblIdsTableAdapter().Fill(set.TblIds);
            new DbRootsTableAdapter().Fill(set.DbRoots);
            new MchnsTableAdapter().Fill(set.Mchns);
            new UsrsTableAdapter().Fill(set.Usrs);
            new CTypsTableAdapter().Fill(set.CTyps);
            new FTypsTableAdapter().Fill(set.FTyps);
            new WrdsTableAdapter().Fill(set.Wrds);
            new DrvsTableAdapter().Fill(set.Drvs);
            new CtnrsTableAdapter().Fill(set.Ctnrs);
            new ItmsTableAdapter().Fill(set.Itms);
            new PartsTableAdapter().Fill(set.Parts);
            return set;
        }
    }
}


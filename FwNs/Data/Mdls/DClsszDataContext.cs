namespace FwNs.Data.Mdls
{
    using System;
    using System.Data;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    [Database(Name="DbRootsV0IV")]
    public class DClsszDataContext : DataContext
    {
        private static MappingSource mappingSource = new AttributeMappingSource();

        public DClsszDataContext() : base(Settings.Default.DbRootsV0IVConnectionString, mappingSource)
        {
        }

        public DClsszDataContext(IDbConnection connection) : base(connection, mappingSource)
        {
        }

        public DClsszDataContext(string connection) : base(connection, mappingSource)
        {
        }

        public DClsszDataContext(IDbConnection connection, MappingSource mappingSource) : base(connection, mappingSource)
        {
        }

        public DClsszDataContext(string connection, MappingSource mappingSource) : base(connection, mappingSource)
        {
        }

        public Table<DbRoot> DbRoots
        {
            get
            {
                return base.GetTable<DbRoot>();
            }
        }

        public Table<Drv> Drvs
        {
            get
            {
                return base.GetTable<Drv>();
            }
        }

        public Table<CTyp> CTyps
        {
            get
            {
                return base.GetTable<CTyp>();
            }
        }

        public Table<FTyp> FTyps
        {
            get
            {
                return base.GetTable<FTyp>();
            }
        }

        public Table<Ctnr> Ctnrs
        {
            get
            {
                return base.GetTable<Ctnr>();
            }
        }

        public Table<Itm> Itms
        {
            get
            {
                return base.GetTable<Itm>();
            }
        }

        public Table<Part> Parts
        {
            get
            {
                return base.GetTable<Part>();
            }
        }

        public Table<Wrd> Wrds
        {
            get
            {
                return base.GetTable<Wrd>();
            }
        }

        public Table<Mchn> Mchns
        {
            get
            {
                return base.GetTable<Mchn>();
            }
        }

        public Table<RowRef> RowRefs
        {
            get
            {
                return base.GetTable<RowRef>();
            }
        }

        public Table<RowTag> RowTags
        {
            get
            {
                return base.GetTable<RowTag>();
            }
        }

        public Table<Tag> Tags
        {
            get
            {
                return base.GetTable<Tag>();
            }
        }

        public Table<TblId> TblIds
        {
            get
            {
                return base.GetTable<TblId>();
            }
        }

        public Table<Usr> Usrs
        {
            get
            {
                return base.GetTable<Usr>();
            }
        }
    }
}


namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.DbRoots")]
    public class DbRoot : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private DateTime _D8t;
        private bool _Actv;
        private EntitySet<Drv> _Drvs;
        private EntitySet<CTyp> _CTyps;
        private EntitySet<Wrd> _Wrds;
        private EntitySet<Mchn> _Mchns;
        private EntitySet<RowRef> _RowRefs;
        private EntitySet<RowTag> _RowTags;
        private EntitySet<Tag> _Tags;
        private EntitySet<TblId> _TblIds;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public DbRoot()
        {
            this._Drvs = new EntitySet<Drv>(new Action<Drv>(this.attach_Drvs), new Action<Drv>(this.detach_Drvs));
            this._CTyps = new EntitySet<CTyp>(new Action<CTyp>(this.attach_CTyps), new Action<CTyp>(this.detach_CTyps));
            this._Wrds = new EntitySet<Wrd>(new Action<Wrd>(this.attach_Wrds), new Action<Wrd>(this.detach_Wrds));
            this._Mchns = new EntitySet<Mchn>(new Action<Mchn>(this.attach_Mchns), new Action<Mchn>(this.detach_Mchns));
            this._RowRefs = new EntitySet<RowRef>(new Action<RowRef>(this.attach_RowRefs), new Action<RowRef>(this.detach_RowRefs));
            this._RowTags = new EntitySet<RowTag>(new Action<RowTag>(this.attach_RowTags), new Action<RowTag>(this.detach_RowTags));
            this._Tags = new EntitySet<Tag>(new Action<Tag>(this.attach_Tags), new Action<Tag>(this.detach_Tags));
            this._TblIds = new EntitySet<TblId>(new Action<TblId>(this.attach_TblIds), new Action<TblId>(this.detach_TblIds));
        }

        private void attach_CTyps(CTyp entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void attach_Drvs(Drv entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void attach_Mchns(Mchn entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void attach_RowRefs(RowRef entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void attach_RowTags(RowTag entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void attach_Tags(Tag entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void attach_TblIds(TblId entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void attach_Wrds(Wrd entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = this;
        }

        private void detach_CTyps(CTyp entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        private void detach_Drvs(Drv entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        private void detach_Mchns(Mchn entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        private void detach_RowRefs(RowRef entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        private void detach_RowTags(RowTag entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        private void detach_Tags(Tag entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        private void detach_TblIds(TblId entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        private void detach_Wrds(Wrd entity)
        {
            this.SendPropertyChanging();
            entity.DbRoot = null;
        }

        protected virtual void SendPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void SendPropertyChanging()
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        [Column(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if (this._Id != value)
                {
                    this.SendPropertyChanging();
                    this._Id = value;
                    this.SendPropertyChanged("Id");
                }
            }
        }

        [Column(Storage="_D8t", DbType="DateTime NOT NULL")]
        public DateTime D8t
        {
            get
            {
                return this._D8t;
            }
            set
            {
                if (this._D8t != value)
                {
                    this.SendPropertyChanging();
                    this._D8t = value;
                    this.SendPropertyChanged("D8t");
                }
            }
        }

        [Column(Storage="_Actv", DbType="Bit NOT NULL")]
        public bool Actv
        {
            get
            {
                return this._Actv;
            }
            set
            {
                if (this._Actv != value)
                {
                    this.SendPropertyChanging();
                    this._Actv = value;
                    this.SendPropertyChanged("Actv");
                }
            }
        }

        [Association(Name="DbRoot_Drv", Storage="_Drvs", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<Drv> Drvs
        {
            get
            {
                return this._Drvs;
            }
            set
            {
                this._Drvs.Assign(value);
            }
        }

        [Association(Name="DbRoot_CTyp", Storage="_CTyps", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<CTyp> CTyps
        {
            get
            {
                return this._CTyps;
            }
            set
            {
                this._CTyps.Assign(value);
            }
        }

        [Association(Name="DbRoot_Wrd", Storage="_Wrds", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<Wrd> Wrds
        {
            get
            {
                return this._Wrds;
            }
            set
            {
                this._Wrds.Assign(value);
            }
        }

        [Association(Name="DbRoot_Mchn", Storage="_Mchns", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<Mchn> Mchns
        {
            get
            {
                return this._Mchns;
            }
            set
            {
                this._Mchns.Assign(value);
            }
        }

        [Association(Name="DbRoot_RowRef", Storage="_RowRefs", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<RowRef> RowRefs
        {
            get
            {
                return this._RowRefs;
            }
            set
            {
                this._RowRefs.Assign(value);
            }
        }

        [Association(Name="DbRoot_RowTag", Storage="_RowTags", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<RowTag> RowTags
        {
            get
            {
                return this._RowTags;
            }
            set
            {
                this._RowTags.Assign(value);
            }
        }

        [Association(Name="DbRoot_Tag", Storage="_Tags", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<Tag> Tags
        {
            get
            {
                return this._Tags;
            }
            set
            {
                this._Tags.Assign(value);
            }
        }

        [Association(Name="DbRoot_TblId", Storage="_TblIds", ThisKey="Id", OtherKey="IdDbRoot")]
        public EntitySet<TblId> TblIds
        {
            get
            {
                return this._TblIds;
            }
            set
            {
                this._TblIds.Assign(value);
            }
        }
    }
}


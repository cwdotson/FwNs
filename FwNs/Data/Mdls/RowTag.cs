namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.RowTags")]
    public class RowTag : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private int _IdTblId;
        private int _IdRow;
        private int _IdTag;
        private bool _Neg8d;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();
        private EntityRef<FwNs.Data.Mdls.Tag> _Tag = new EntityRef<FwNs.Data.Mdls.Tag>();
        private EntityRef<FwNs.Data.Mdls.TblId> _TblId = new EntityRef<FwNs.Data.Mdls.TblId>();

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

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

        [Column(Storage="_IdDbRoot", DbType="Int NOT NULL")]
        public int IdDbRoot
        {
            get
            {
                return this._IdDbRoot;
            }
            set
            {
                if (this._IdDbRoot != value)
                {
                    if (this._DbRoot.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdDbRoot = value;
                    this.SendPropertyChanged("IdDbRoot");
                }
            }
        }

        [Column(Storage="_IdTblId", DbType="Int NOT NULL")]
        public int IdTblId
        {
            get
            {
                return this._IdTblId;
            }
            set
            {
                if (this._IdTblId != value)
                {
                    if (this._TblId.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdTblId = value;
                    this.SendPropertyChanged("IdTblId");
                }
            }
        }

        [Column(Storage="_IdRow", DbType="Int NOT NULL")]
        public int IdRow
        {
            get
            {
                return this._IdRow;
            }
            set
            {
                if (this._IdRow != value)
                {
                    this.SendPropertyChanging();
                    this._IdRow = value;
                    this.SendPropertyChanged("IdRow");
                }
            }
        }

        [Column(Storage="_IdTag", DbType="Int NOT NULL")]
        public int IdTag
        {
            get
            {
                return this._IdTag;
            }
            set
            {
                if (this._IdTag != value)
                {
                    if (this._Tag.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdTag = value;
                    this.SendPropertyChanged("IdTag");
                }
            }
        }

        [Column(Storage="_Neg8d", DbType="Bit NOT NULL")]
        public bool Neg8d
        {
            get
            {
                return this._Neg8d;
            }
            set
            {
                if (this._Neg8d != value)
                {
                    this.SendPropertyChanging();
                    this._Neg8d = value;
                    this.SendPropertyChanged("Neg8d");
                }
            }
        }

        [Association(Name="DbRoot_RowTag", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.DbRoot DbRoot
        {
            get
            {
                return this._DbRoot.Entity;
            }
            set
            {
                FwNs.Data.Mdls.DbRoot entity = this._DbRoot.Entity;
                if ((entity != value) || !this._DbRoot.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._DbRoot.Entity = null;
                        entity.RowTags.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.RowTags.Add(this);
                        this._IdDbRoot = value.Id;
                    }
                    else
                    {
                        this._IdDbRoot = 0;
                    }
                    this.SendPropertyChanged("DbRoot");
                }
            }
        }

        [Association(Name="Tag_RowTag", Storage="_Tag", ThisKey="IdTag", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.Tag Tag
        {
            get
            {
                return this._Tag.Entity;
            }
            set
            {
                FwNs.Data.Mdls.Tag entity = this._Tag.Entity;
                if ((entity != value) || !this._Tag.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._Tag.Entity = null;
                        entity.RowTags.Remove(this);
                    }
                    this._Tag.Entity = value;
                    if (value != null)
                    {
                        value.RowTags.Add(this);
                        this._IdTag = value.Id;
                    }
                    else
                    {
                        this._IdTag = 0;
                    }
                    this.SendPropertyChanged("Tag");
                }
            }
        }

        [Association(Name="TblId_RowTag", Storage="_TblId", ThisKey="IdTblId", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.TblId TblId
        {
            get
            {
                return this._TblId.Entity;
            }
            set
            {
                FwNs.Data.Mdls.TblId entity = this._TblId.Entity;
                if ((entity != value) || !this._TblId.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._TblId.Entity = null;
                        entity.RowTags.Remove(this);
                    }
                    this._TblId.Entity = value;
                    if (value != null)
                    {
                        value.RowTags.Add(this);
                        this._IdTblId = value.Id;
                    }
                    else
                    {
                        this._IdTblId = 0;
                    }
                    this.SendPropertyChanged("TblId");
                }
            }
        }
    }
}


namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.TblIds")]
    public class TblId : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private string _Nm;
        private string _FK;
        private bool _Actv;
        private EntitySet<RowTag> _RowTags;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public TblId()
        {
            this._RowTags = new EntitySet<RowTag>(new Action<RowTag>(this.attach_RowTags), new Action<RowTag>(this.detach_RowTags));
            this._DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();
        }

        private void attach_RowTags(RowTag entity)
        {
            this.SendPropertyChanging();
            entity.TblId = this;
        }

        private void detach_RowTags(RowTag entity)
        {
            this.SendPropertyChanging();
            entity.TblId = null;
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

        [Column(Storage="_Nm", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
        public string Nm
        {
            get
            {
                return this._Nm;
            }
            set
            {
                if (this._Nm != value)
                {
                    this.SendPropertyChanging();
                    this._Nm = value;
                    this.SendPropertyChanged("Nm");
                }
            }
        }

        [Column(Storage="_FK", DbType="VarChar(50)")]
        public string FK
        {
            get
            {
                return this._FK;
            }
            set
            {
                if (this._FK != value)
                {
                    this.SendPropertyChanging();
                    this._FK = value;
                    this.SendPropertyChanged("FK");
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

        [Association(Name="TblId_RowTag", Storage="_RowTags", ThisKey="Id", OtherKey="IdTblId")]
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

        [Association(Name="DbRoot_TblId", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
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
                        entity.TblIds.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.TblIds.Add(this);
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
    }
}


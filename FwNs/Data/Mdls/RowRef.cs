namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.RowRefs")]
    public class RowRef : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private int _IdTbl1;
        private int _IdRow1;
        private int _IdTbl2;
        private int _IdRow2;
        private string _Nfo;
        private DateTime? _Cr8D8;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();

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

        [Column(Storage="_IdTbl1", DbType="Int NOT NULL")]
        public int IdTbl1
        {
            get
            {
                return this._IdTbl1;
            }
            set
            {
                if (this._IdTbl1 != value)
                {
                    this.SendPropertyChanging();
                    this._IdTbl1 = value;
                    this.SendPropertyChanged("IdTbl1");
                }
            }
        }

        [Column(Storage="_IdRow1", DbType="Int NOT NULL")]
        public int IdRow1
        {
            get
            {
                return this._IdRow1;
            }
            set
            {
                if (this._IdRow1 != value)
                {
                    this.SendPropertyChanging();
                    this._IdRow1 = value;
                    this.SendPropertyChanged("IdRow1");
                }
            }
        }

        [Column(Storage="_IdTbl2", DbType="Int NOT NULL")]
        public int IdTbl2
        {
            get
            {
                return this._IdTbl2;
            }
            set
            {
                if (this._IdTbl2 != value)
                {
                    this.SendPropertyChanging();
                    this._IdTbl2 = value;
                    this.SendPropertyChanged("IdTbl2");
                }
            }
        }

        [Column(Storage="_IdRow2", DbType="Int NOT NULL")]
        public int IdRow2
        {
            get
            {
                return this._IdRow2;
            }
            set
            {
                if (this._IdRow2 != value)
                {
                    this.SendPropertyChanging();
                    this._IdRow2 = value;
                    this.SendPropertyChanged("IdRow2");
                }
            }
        }

        [Column(Storage="_Nfo", DbType="NVarChar(255)")]
        public string Nfo
        {
            get
            {
                return this._Nfo;
            }
            set
            {
                if (this._Nfo != value)
                {
                    this.SendPropertyChanging();
                    this._Nfo = value;
                    this.SendPropertyChanged("Nfo");
                }
            }
        }

        [Column(Storage="_Cr8D8", DbType="DateTime")]
        public DateTime? Cr8D8
        {
            get
            {
                return this._Cr8D8;
            }
            set
            {
                DateTime? nullable = this._Cr8D8;
                DateTime? nullable2 = value;
                if ((nullable.HasValue == nullable2.HasValue) ? (nullable.HasValue ? (nullable.GetValueOrDefault() != nullable2.GetValueOrDefault()) : false) : true)
                {
                    this.SendPropertyChanging();
                    this._Cr8D8 = value;
                    this.SendPropertyChanged("Cr8D8");
                }
            }
        }

        [Association(Name="DbRoot_RowRef", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
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
                        entity.RowRefs.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.RowRefs.Add(this);
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


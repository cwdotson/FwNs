namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.CTyps")]
    public class CTyp : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private string _Nm;
        private string _Fldr;
        private EntitySet<FTyp> _FTyps;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public CTyp()
        {
            this._FTyps = new EntitySet<FTyp>(new Action<FTyp>(this.attach_FTyps), new Action<FTyp>(this.detach_FTyps));
            this._DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();
        }

        private void attach_FTyps(FTyp entity)
        {
            this.SendPropertyChanging();
            entity.CTyp = this;
        }

        private void detach_FTyps(FTyp entity)
        {
            this.SendPropertyChanging();
            entity.CTyp = null;
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

        [Column(Storage="_Nm", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
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

        [Column(Storage="_Fldr", DbType="NVarChar(255)")]
        public string Fldr
        {
            get
            {
                return this._Fldr;
            }
            set
            {
                if (this._Fldr != value)
                {
                    this.SendPropertyChanging();
                    this._Fldr = value;
                    this.SendPropertyChanged("Fldr");
                }
            }
        }

        [Association(Name="CTyp_FTyp", Storage="_FTyps", ThisKey="Id", OtherKey="IdCTyp")]
        public EntitySet<FTyp> FTyps
        {
            get
            {
                return this._FTyps;
            }
            set
            {
                this._FTyps.Assign(value);
            }
        }

        [Association(Name="DbRoot_CTyp", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
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
                        entity.CTyps.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.CTyps.Add(this);
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


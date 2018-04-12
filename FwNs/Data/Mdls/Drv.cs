namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Drvs")]
    public class Drv : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private int _IdMchn;
        private int? _IdUsr;
        private string _Lttr;
        private string _Nfo;
        private EntitySet<Ctnr> _Ctnrs;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot;
        private EntityRef<FwNs.Data.Mdls.Mchn> _Mchn;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public Drv()
        {
            this._Ctnrs = new EntitySet<Ctnr>(new Action<Ctnr>(this.attach_Ctnrs), new Action<Ctnr>(this.detach_Ctnrs));
            this._DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();
            this._Mchn = new EntityRef<FwNs.Data.Mdls.Mchn>();
        }

        private void attach_Ctnrs(Ctnr entity)
        {
            this.SendPropertyChanging();
            entity.Drv = this;
        }

        private void detach_Ctnrs(Ctnr entity)
        {
            this.SendPropertyChanging();
            entity.Drv = null;
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

        [Column(Storage="_IdMchn", DbType="Int NOT NULL")]
        public int IdMchn
        {
            get
            {
                return this._IdMchn;
            }
            set
            {
                if (this._IdMchn != value)
                {
                    if (this._Mchn.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdMchn = value;
                    this.SendPropertyChanged("IdMchn");
                }
            }
        }

        [Column(Storage="_IdUsr", DbType="Int")]
        public int? IdUsr
        {
            get
            {
                return this._IdUsr;
            }
            set
            {
                int? nullable = this._IdUsr;
                int? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._IdUsr = value;
                    this.SendPropertyChanged("IdUsr");
                }
            }
        }

        [Column(Storage="_Lttr", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
        public string Lttr
        {
            get
            {
                return this._Lttr;
            }
            set
            {
                if (this._Lttr != value)
                {
                    this.SendPropertyChanging();
                    this._Lttr = value;
                    this.SendPropertyChanged("Lttr");
                }
            }
        }

        [Column(Storage="_Nfo", DbType="NVarChar(MAX)")]
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

        [Association(Name="Drv_Ctnr", Storage="_Ctnrs", ThisKey="Id", OtherKey="IdDrv")]
        public EntitySet<Ctnr> Ctnrs
        {
            get
            {
                return this._Ctnrs;
            }
            set
            {
                this._Ctnrs.Assign(value);
            }
        }

        [Association(Name="DbRoot_Drv", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
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
                        entity.Drvs.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.Drvs.Add(this);
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

        [Association(Name="Mchn_Drv", Storage="_Mchn", ThisKey="IdMchn", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.Mchn Mchn
        {
            get
            {
                return this._Mchn.Entity;
            }
            set
            {
                FwNs.Data.Mdls.Mchn entity = this._Mchn.Entity;
                if ((entity != value) || !this._Mchn.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._Mchn.Entity = null;
                        entity.Drvs.Remove(this);
                    }
                    this._Mchn.Entity = value;
                    if (value != null)
                    {
                        value.Drvs.Add(this);
                        this._IdMchn = value.Id;
                    }
                    else
                    {
                        this._IdMchn = 0;
                    }
                    this.SendPropertyChanged("Mchn");
                }
            }
        }
    }
}


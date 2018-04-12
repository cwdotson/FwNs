namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Mchns")]
    public class Mchn : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private string _Nm;
        private EntitySet<Drv> _Drvs;
        private EntitySet<Usr> _Usrs;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public Mchn()
        {
            this._Drvs = new EntitySet<Drv>(new Action<Drv>(this.attach_Drvs), new Action<Drv>(this.detach_Drvs));
            this._Usrs = new EntitySet<Usr>(new Action<Usr>(this.attach_Usrs), new Action<Usr>(this.detach_Usrs));
            this._DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();
        }

        private void attach_Drvs(Drv entity)
        {
            this.SendPropertyChanging();
            entity.Mchn = this;
        }

        private void attach_Usrs(Usr entity)
        {
            this.SendPropertyChanging();
            entity.Mchn = this;
        }

        private void detach_Drvs(Drv entity)
        {
            this.SendPropertyChanging();
            entity.Mchn = null;
        }

        private void detach_Usrs(Usr entity)
        {
            this.SendPropertyChanging();
            entity.Mchn = null;
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

        [Column(Storage="_Nm", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
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

        [Association(Name="Mchn_Drv", Storage="_Drvs", ThisKey="Id", OtherKey="IdMchn")]
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

        [Association(Name="Mchn_Usr", Storage="_Usrs", ThisKey="Id", OtherKey="IdMchn")]
        public EntitySet<Usr> Usrs
        {
            get
            {
                return this._Usrs;
            }
            set
            {
                this._Usrs.Assign(value);
            }
        }

        [Association(Name="DbRoot_Mchn", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
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
                        entity.Mchns.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.Mchns.Add(this);
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


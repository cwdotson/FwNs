namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Wrds")]
    public class Wrd : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private string _Val;
        private EntitySet<Ctnr> _Ctnrs;
        private EntitySet<Itm> _Itms;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public Wrd()
        {
            this._Ctnrs = new EntitySet<Ctnr>(new Action<Ctnr>(this.attach_Ctnrs), new Action<Ctnr>(this.detach_Ctnrs));
            this._Itms = new EntitySet<Itm>(new Action<Itm>(this.attach_Itms), new Action<Itm>(this.detach_Itms));
            this._DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();
        }

        private void attach_Ctnrs(Ctnr entity)
        {
            this.SendPropertyChanging();
            entity.Wrd = this;
        }

        private void attach_Itms(Itm entity)
        {
            this.SendPropertyChanging();
            entity.Wrd = this;
        }

        private void detach_Ctnrs(Ctnr entity)
        {
            this.SendPropertyChanging();
            entity.Wrd = null;
        }

        private void detach_Itms(Itm entity)
        {
            this.SendPropertyChanging();
            entity.Wrd = null;
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

        [Column(Storage="_Val", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
        public string Val
        {
            get
            {
                return this._Val;
            }
            set
            {
                if (this._Val != value)
                {
                    this.SendPropertyChanging();
                    this._Val = value;
                    this.SendPropertyChanged("Val");
                }
            }
        }

        [Association(Name="Wrd_Ctnr", Storage="_Ctnrs", ThisKey="Id", OtherKey="IdWrd")]
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

        [Association(Name="Wrd_Itm", Storage="_Itms", ThisKey="Id", OtherKey="IdWrd")]
        public EntitySet<Itm> Itms
        {
            get
            {
                return this._Itms;
            }
            set
            {
                this._Itms.Assign(value);
            }
        }

        [Association(Name="DbRoot_Wrd", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
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
                        entity.Wrds.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.Wrds.Add(this);
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


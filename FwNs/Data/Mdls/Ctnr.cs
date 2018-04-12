namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Ctnrs")]
    public class Ctnr : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDrv;
        private int _IdCtnr;
        private int _IdWrd;
        private DateTime _Cr8D8;
        private DateTime _ChkD8;
        private string _Nfo;
        private EntitySet<Itm> _Itms;
        private EntityRef<FwNs.Data.Mdls.Drv> _Drv;
        private EntityRef<FwNs.Data.Mdls.Wrd> _Wrd;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public Ctnr()
        {
            this._Itms = new EntitySet<Itm>(new Action<Itm>(this.attach_Itms), new Action<Itm>(this.detach_Itms));
            this._Drv = new EntityRef<FwNs.Data.Mdls.Drv>();
            this._Wrd = new EntityRef<FwNs.Data.Mdls.Wrd>();
        }

        private void attach_Itms(Itm entity)
        {
            this.SendPropertyChanging();
            entity.Ctnr = this;
        }

        private void detach_Itms(Itm entity)
        {
            this.SendPropertyChanging();
            entity.Ctnr = null;
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

        [Column(Storage="_IdDrv", DbType="Int NOT NULL")]
        public int IdDrv
        {
            get
            {
                return this._IdDrv;
            }
            set
            {
                if (this._IdDrv != value)
                {
                    if (this._Drv.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdDrv = value;
                    this.SendPropertyChanged("IdDrv");
                }
            }
        }

        [Column(Storage="_IdCtnr", DbType="Int NOT NULL")]
        public int IdCtnr
        {
            get
            {
                return this._IdCtnr;
            }
            set
            {
                if (this._IdCtnr != value)
                {
                    this.SendPropertyChanging();
                    this._IdCtnr = value;
                    this.SendPropertyChanged("IdCtnr");
                }
            }
        }

        [Column(Storage="_IdWrd", DbType="Int NOT NULL")]
        public int IdWrd
        {
            get
            {
                return this._IdWrd;
            }
            set
            {
                if (this._IdWrd != value)
                {
                    if (this._Wrd.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdWrd = value;
                    this.SendPropertyChanged("IdWrd");
                }
            }
        }

        [Column(Storage="_Cr8D8", DbType="DateTime NOT NULL")]
        public DateTime Cr8D8
        {
            get
            {
                return this._Cr8D8;
            }
            set
            {
                if (this._Cr8D8 != value)
                {
                    this.SendPropertyChanging();
                    this._Cr8D8 = value;
                    this.SendPropertyChanged("Cr8D8");
                }
            }
        }

        [Column(Storage="_ChkD8", DbType="DateTime NOT NULL")]
        public DateTime ChkD8
        {
            get
            {
                return this._ChkD8;
            }
            set
            {
                if (this._ChkD8 != value)
                {
                    this.SendPropertyChanging();
                    this._ChkD8 = value;
                    this.SendPropertyChanged("ChkD8");
                }
            }
        }

        [Column(Storage="_Nfo", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
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

        [Association(Name="Ctnr_Itm", Storage="_Itms", ThisKey="Id", OtherKey="IdCtnr")]
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

        [Association(Name="Drv_Ctnr", Storage="_Drv", ThisKey="IdDrv", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.Drv Drv
        {
            get
            {
                return this._Drv.Entity;
            }
            set
            {
                FwNs.Data.Mdls.Drv entity = this._Drv.Entity;
                if ((entity != value) || !this._Drv.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._Drv.Entity = null;
                        entity.Ctnrs.Remove(this);
                    }
                    this._Drv.Entity = value;
                    if (value != null)
                    {
                        value.Ctnrs.Add(this);
                        this._IdDrv = value.Id;
                    }
                    else
                    {
                        this._IdDrv = 0;
                    }
                    this.SendPropertyChanged("Drv");
                }
            }
        }

        [Association(Name="Wrd_Ctnr", Storage="_Wrd", ThisKey="IdWrd", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.Wrd Wrd
        {
            get
            {
                return this._Wrd.Entity;
            }
            set
            {
                FwNs.Data.Mdls.Wrd entity = this._Wrd.Entity;
                if ((entity != value) || !this._Wrd.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._Wrd.Entity = null;
                        entity.Ctnrs.Remove(this);
                    }
                    this._Wrd.Entity = value;
                    if (value != null)
                    {
                        value.Ctnrs.Add(this);
                        this._IdWrd = value.Id;
                    }
                    else
                    {
                        this._IdWrd = 0;
                    }
                    this.SendPropertyChanged("Wrd");
                }
            }
        }
    }
}


namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Itms")]
    public class Itm : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdCtnr;
        private int _IdWrd;
        private int _IdFTyp;
        private DateTime _Cr8D8;
        private DateTime _ChkD8;
        private string _Nfo;
        private EntitySet<Part> _Parts;
        private EntityRef<FwNs.Data.Mdls.Ctnr> _Ctnr;
        private EntityRef<FwNs.Data.Mdls.FTyp> _FTyp;
        private EntityRef<FwNs.Data.Mdls.Wrd> _Wrd;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public Itm()
        {
            this._Parts = new EntitySet<Part>(new Action<Part>(this.attach_Parts), new Action<Part>(this.detach_Parts));
            this._Ctnr = new EntityRef<FwNs.Data.Mdls.Ctnr>();
            this._FTyp = new EntityRef<FwNs.Data.Mdls.FTyp>();
            this._Wrd = new EntityRef<FwNs.Data.Mdls.Wrd>();
        }

        private void attach_Parts(Part entity)
        {
            this.SendPropertyChanging();
            entity.Itm = this;
        }

        private void detach_Parts(Part entity)
        {
            this.SendPropertyChanging();
            entity.Itm = null;
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
                    if (this._Ctnr.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
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

        [Column(Storage="_IdFTyp", DbType="Int NOT NULL")]
        public int IdFTyp
        {
            get
            {
                return this._IdFTyp;
            }
            set
            {
                if (this._IdFTyp != value)
                {
                    if (this._FTyp.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdFTyp = value;
                    this.SendPropertyChanged("IdFTyp");
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

        [Association(Name="Itm_Part", Storage="_Parts", ThisKey="Id", OtherKey="IdItm")]
        public EntitySet<Part> Parts
        {
            get
            {
                return this._Parts;
            }
            set
            {
                this._Parts.Assign(value);
            }
        }

        [Association(Name="Ctnr_Itm", Storage="_Ctnr", ThisKey="IdCtnr", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.Ctnr Ctnr
        {
            get
            {
                return this._Ctnr.Entity;
            }
            set
            {
                FwNs.Data.Mdls.Ctnr entity = this._Ctnr.Entity;
                if ((entity != value) || !this._Ctnr.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._Ctnr.Entity = null;
                        entity.Itms.Remove(this);
                    }
                    this._Ctnr.Entity = value;
                    if (value != null)
                    {
                        value.Itms.Add(this);
                        this._IdCtnr = value.Id;
                    }
                    else
                    {
                        this._IdCtnr = 0;
                    }
                    this.SendPropertyChanged("Ctnr");
                }
            }
        }

        [Association(Name="FTyp_Itm", Storage="_FTyp", ThisKey="IdFTyp", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.FTyp FTyp
        {
            get
            {
                return this._FTyp.Entity;
            }
            set
            {
                FwNs.Data.Mdls.FTyp entity = this._FTyp.Entity;
                if ((entity != value) || !this._FTyp.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._FTyp.Entity = null;
                        entity.Itms.Remove(this);
                    }
                    this._FTyp.Entity = value;
                    if (value != null)
                    {
                        value.Itms.Add(this);
                        this._IdFTyp = value.Id;
                    }
                    else
                    {
                        this._IdFTyp = 0;
                    }
                    this.SendPropertyChanged("FTyp");
                }
            }
        }

        [Association(Name="Wrd_Itm", Storage="_Wrd", ThisKey="IdWrd", OtherKey="Id", IsForeignKey=true)]
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
                        entity.Itms.Remove(this);
                    }
                    this._Wrd.Entity = value;
                    if (value != null)
                    {
                        value.Itms.Add(this);
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


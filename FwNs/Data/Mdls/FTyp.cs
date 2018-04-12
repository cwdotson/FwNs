namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.FTyps")]
    public class FTyp : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdCTyp;
        private string _Extn;
        private string _Applctn;
        private string _Dscrptn;
        private bool _Ef0;
        private bool _Ef1;
        private EntitySet<Itm> _Itms;
        private EntityRef<FwNs.Data.Mdls.CTyp> _CTyp;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public FTyp()
        {
            this._Itms = new EntitySet<Itm>(new Action<Itm>(this.attach_Itms), new Action<Itm>(this.detach_Itms));
            this._CTyp = new EntityRef<FwNs.Data.Mdls.CTyp>();
        }

        private void attach_Itms(Itm entity)
        {
            this.SendPropertyChanging();
            entity.FTyp = this;
        }

        private void detach_Itms(Itm entity)
        {
            this.SendPropertyChanging();
            entity.FTyp = null;
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

        [Column(Storage="_IdCTyp", DbType="Int NOT NULL")]
        public int IdCTyp
        {
            get
            {
                return this._IdCTyp;
            }
            set
            {
                if (this._IdCTyp != value)
                {
                    if (this._CTyp.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdCTyp = value;
                    this.SendPropertyChanged("IdCTyp");
                }
            }
        }

        [Column(Storage="_Extn", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
        public string Extn
        {
            get
            {
                return this._Extn;
            }
            set
            {
                if (this._Extn != value)
                {
                    this.SendPropertyChanging();
                    this._Extn = value;
                    this.SendPropertyChanged("Extn");
                }
            }
        }

        [Column(Storage="_Applctn", DbType="NVarChar(MAX)")]
        public string Applctn
        {
            get
            {
                return this._Applctn;
            }
            set
            {
                if (this._Applctn != value)
                {
                    this.SendPropertyChanging();
                    this._Applctn = value;
                    this.SendPropertyChanged("Applctn");
                }
            }
        }

        [Column(Storage="_Dscrptn", DbType="NVarChar(MAX)")]
        public string Dscrptn
        {
            get
            {
                return this._Dscrptn;
            }
            set
            {
                if (this._Dscrptn != value)
                {
                    this.SendPropertyChanging();
                    this._Dscrptn = value;
                    this.SendPropertyChanged("Dscrptn");
                }
            }
        }

        [Column(Storage="_Ef0", DbType="Bit NOT NULL")]
        public bool Ef0
        {
            get
            {
                return this._Ef0;
            }
            set
            {
                if (this._Ef0 != value)
                {
                    this.SendPropertyChanging();
                    this._Ef0 = value;
                    this.SendPropertyChanged("Ef0");
                }
            }
        }

        [Column(Storage="_Ef1", DbType="Bit NOT NULL")]
        public bool Ef1
        {
            get
            {
                return this._Ef1;
            }
            set
            {
                if (this._Ef1 != value)
                {
                    this.SendPropertyChanging();
                    this._Ef1 = value;
                    this.SendPropertyChanged("Ef1");
                }
            }
        }

        [Association(Name="FTyp_Itm", Storage="_Itms", ThisKey="Id", OtherKey="IdFTyp")]
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

        [Association(Name="CTyp_FTyp", Storage="_CTyp", ThisKey="IdCTyp", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.CTyp CTyp
        {
            get
            {
                return this._CTyp.Entity;
            }
            set
            {
                FwNs.Data.Mdls.CTyp entity = this._CTyp.Entity;
                if ((entity != value) || !this._CTyp.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._CTyp.Entity = null;
                        entity.FTyps.Remove(this);
                    }
                    this._CTyp.Entity = value;
                    if (value != null)
                    {
                        value.FTyps.Add(this);
                        this._IdCTyp = value.Id;
                    }
                    else
                    {
                        this._IdCTyp = 0;
                    }
                    this.SendPropertyChanged("CTyp");
                }
            }
        }
    }
}


namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Parts")]
    public class Part : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdItm;
        private int? _Typ;
        private int _Ndx;
        private string _Val;
        private string _Nfo;
        private EntityRef<FwNs.Data.Mdls.Itm> _Itm = new EntityRef<FwNs.Data.Mdls.Itm>();

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

        [Column(Storage="_IdItm", DbType="Int NOT NULL")]
        public int IdItm
        {
            get
            {
                return this._IdItm;
            }
            set
            {
                if (this._IdItm != value)
                {
                    if (this._Itm.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._IdItm = value;
                    this.SendPropertyChanged("IdItm");
                }
            }
        }

        [Column(Storage="_Typ", DbType="Int")]
        public int? Typ
        {
            get
            {
                return this._Typ;
            }
            set
            {
                int? nullable = this._Typ;
                int? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._Typ = value;
                    this.SendPropertyChanged("Typ");
                }
            }
        }

        [Column(Storage="_Ndx", DbType="Int NOT NULL")]
        public int Ndx
        {
            get
            {
                return this._Ndx;
            }
            set
            {
                if (this._Ndx != value)
                {
                    this.SendPropertyChanging();
                    this._Ndx = value;
                    this.SendPropertyChanged("Ndx");
                }
            }
        }

        [Column(Storage="_Val", DbType="NText", UpdateCheck=UpdateCheck.Never)]
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

        [Column(Storage="_Nfo", DbType="VarChar(MAX)")]
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

        [Association(Name="Itm_Part", Storage="_Itm", ThisKey="IdItm", OtherKey="Id", IsForeignKey=true)]
        public FwNs.Data.Mdls.Itm Itm
        {
            get
            {
                return this._Itm.Entity;
            }
            set
            {
                FwNs.Data.Mdls.Itm entity = this._Itm.Entity;
                if ((entity != value) || !this._Itm.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._Itm.Entity = null;
                        entity.Parts.Remove(this);
                    }
                    this._Itm.Entity = value;
                    if (value != null)
                    {
                        value.Parts.Add(this);
                        this._IdItm = value.Id;
                    }
                    else
                    {
                        this._IdItm = 0;
                    }
                    this.SendPropertyChanged("Itm");
                }
            }
        }
    }
}


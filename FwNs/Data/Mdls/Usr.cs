namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Usrs")]
    public class Usr : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdMchn;
        private string _Nm;
        private EntityRef<FwNs.Data.Mdls.Mchn> _Mchn = new EntityRef<FwNs.Data.Mdls.Mchn>();

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

        [Association(Name="Mchn_Usr", Storage="_Mchn", ThisKey="IdMchn", OtherKey="Id", IsForeignKey=true)]
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
                        entity.Usrs.Remove(this);
                    }
                    this._Mchn.Entity = value;
                    if (value != null)
                    {
                        value.Usrs.Add(this);
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


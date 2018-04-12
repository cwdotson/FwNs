namespace FwNs.Data.Mdls
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Tags")]
    public class Tag : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        private int _Id;
        private int _IdDbRoot;
        private int? _IdEnmTyp;
        private int? _IdPrnt;
        private string _Txt;
        private EntitySet<RowTag> _RowTags;
        private EntityRef<FwNs.Data.Mdls.DbRoot> _DbRoot;

        [field: CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated]
        public event PropertyChangingEventHandler PropertyChanging;

        public Tag()
        {
            this._RowTags = new EntitySet<RowTag>(new Action<RowTag>(this.attach_RowTags), new Action<RowTag>(this.detach_RowTags));
            this._DbRoot = new EntityRef<FwNs.Data.Mdls.DbRoot>();
        }

        private void attach_RowTags(RowTag entity)
        {
            this.SendPropertyChanging();
            entity.Tag = this;
        }

        private void detach_RowTags(RowTag entity)
        {
            this.SendPropertyChanging();
            entity.Tag = null;
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

        [Column(Storage="_IdEnmTyp", DbType="Int")]
        public int? IdEnmTyp
        {
            get
            {
                return this._IdEnmTyp;
            }
            set
            {
                int? nullable = this._IdEnmTyp;
                int? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._IdEnmTyp = value;
                    this.SendPropertyChanged("IdEnmTyp");
                }
            }
        }

        [Column(Storage="_IdPrnt", DbType="Int")]
        public int? IdPrnt
        {
            get
            {
                return this._IdPrnt;
            }
            set
            {
                int? nullable = this._IdPrnt;
                int? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._IdPrnt = value;
                    this.SendPropertyChanged("IdPrnt");
                }
            }
        }

        [Column(Storage="_Txt", DbType="VarChar(MAX) NOT NULL", CanBeNull=false)]
        public string Txt
        {
            get
            {
                return this._Txt;
            }
            set
            {
                if (this._Txt != value)
                {
                    this.SendPropertyChanging();
                    this._Txt = value;
                    this.SendPropertyChanged("Txt");
                }
            }
        }

        [Association(Name="Tag_RowTag", Storage="_RowTags", ThisKey="Id", OtherKey="IdTag")]
        public EntitySet<RowTag> RowTags
        {
            get
            {
                return this._RowTags;
            }
            set
            {
                this._RowTags.Assign(value);
            }
        }

        [Association(Name="DbRoot_Tag", Storage="_DbRoot", ThisKey="IdDbRoot", OtherKey="Id", IsForeignKey=true)]
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
                        entity.Tags.Remove(this);
                    }
                    this._DbRoot.Entity = value;
                    if (value != null)
                    {
                        value.Tags.Add(this);
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


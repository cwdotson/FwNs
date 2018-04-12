namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Runtime.CompilerServices;

    public class Field
    {
        public Field(string name, DataTypeEnum type) : this(name, type, -1)
        {
        }

        public Field(string name, DataTypeEnum type, int ordinal)
        {
            this.Name = name;
            this.DataType = type;
            this.AutoIncStart = -1;
            this.Ordinal = ordinal;
        }

        public Field Clone()
        {
            return new Field(this.Name, this.DataType, this.Ordinal) { 
                AutoIncStart = this.AutoIncStart,
                IsArray = this.IsArray,
                IsPrimaryKey = this.IsPrimaryKey,
                CurAutoIncVal = this.CurAutoIncVal,
                Comment = this.Comment
            };
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string Name { get; set; }

        public DataTypeEnum DataType { get; set; }

        public int Ordinal { get; internal set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsArray { get; set; }

        public int AutoIncStart { get; set; }

        internal int CurAutoIncVal { get; set; }

        public bool IsAutoInc
        {
            get
            {
                return (this.AutoIncStart > -1);
            }
        }

        public string Comment { get; set; }

        public object Tag { get; set; }
    }
}


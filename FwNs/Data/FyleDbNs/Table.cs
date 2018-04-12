namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections.Generic;

    public class Table : Records
    {
        private const string Index = "index";
        private FwNs.Data.FyleDbNs.Fields _fields;

        public Table(FwNs.Data.FyleDbNs.Fields fields)
        {
            this.Create(fields, null, true);
        }

        public Table(FwNs.Data.FyleDbNs.Fields fields, bool copyFields)
        {
            this.Create(fields, null, copyFields);
        }

        public Table(FwNs.Data.FyleDbNs.Fields fields, object[][] records, bool copyFields)
        {
            this.Create(fields, records, copyFields);
        }

        public Table(FwNs.Data.FyleDbNs.Fields fields, Records records, bool copyFields) : base(records.Count)
        {
            this.initFields(fields, copyFields);
            if (records != null)
            {
                foreach (Record record in records)
                {
                    base.Add(record);
                }
            }
        }

        internal void Create(FwNs.Data.FyleDbNs.Fields fields, object[][] records, bool copyFields)
        {
            base.Clear();
            this.initFields(fields, copyFields);
            if (records != null)
            {
                for (int i = 0; i < records.Length; i++)
                {
                    object[] values = records[i];
                    Record item = new Record(this._fields, values);
                    base.Add(item);
                }
            }
        }

        private void initFields(FwNs.Data.FyleDbNs.Fields fields, bool copyFields)
        {
            if (copyFields)
            {
                this._fields = new FwNs.Data.FyleDbNs.Fields(fields.Count);
                int num = 0;
                using (List<Field>.Enumerator enumerator = fields.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Field field = enumerator.Current.Clone();
                        field.Ordinal = num++;
                        this._fields.Add(field);
                    }
                    return;
                }
            }
            this._fields = fields;
        }

        public Record NewRow()
        {
            return new Record(this._fields, null);
        }

        private void orderBy(Records records, string[] fieldList, string[] orderByList)
        {
            List<Field> sortFields = new List<Field>(orderByList.Length);
            List<bool> sortDirLst = new List<bool>(orderByList.Length);
            List<bool> caseLst = new List<bool>(orderByList.Length);
            FileDbEngine.GetOrderByLists(this._fields, fieldList, orderByList, sortFields, sortDirLst, caseLst);
            records.Sort(new RecordComparer(sortFields, sortDirLst, caseLst));
        }

        public FileDb SaveToDb(string dbName)
        {
            FileDb db1 = new FileDb();
            db1.Create(this, dbName);
            return db1;
        }

        public Table SelectRecords(FilterExpression filter)
        {
            return this.SelectRecords(filter, null, null);
        }

        public Table SelectRecords(FilterExpressionGroup filter)
        {
            return this.SelectRecords(filter, null, null);
        }

        public Table SelectRecords(string filter)
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords(group);
        }

        public Table SelectRecords(FilterExpression filter, string[] fieldList)
        {
            return this.SelectRecords(filter, fieldList, null);
        }

        public Table SelectRecords(FilterExpressionGroup filter, string[] fieldList)
        {
            return this.SelectRecords(filter, fieldList, null);
        }

        public Table SelectRecords(string filter, string[] fieldList)
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords(group, fieldList, null);
        }

        public Table SelectRecords(FilterExpression filter, string[] fieldList, string[] orderByList)
        {
            FilterExpressionGroup group = new FilterExpressionGroup();
            group.Add(BoolOpEnum.And, filter);
            return this.SelectRecords(group, fieldList, orderByList);
        }

        public Table SelectRecords(FilterExpressionGroup filter, string[] fieldList, string[] orderByList)
        {
            int length;
            FwNs.Data.FyleDbNs.Fields fields = new FwNs.Data.FyleDbNs.Fields(this._fields.Count);
            if (fieldList != null)
            {
                length = fieldList.Length;
                for (int i = 0; i < length; i++)
                {
                    string fieldName = fieldList[i];
                    if (!this._fields.ContainsKey(fieldName))
                    {
                        throw new Exception(string.Format("Invalid field name - {0}", fieldName));
                    }
                    Field field = this._fields[fieldName];
                    fields.Add(new Field(fieldName, field.DataType, i));
                }
            }
            else
            {
                length = this._fields.Count;
                for (int i = 0; i < length; i++)
                {
                    Field field2 = this._fields[i];
                    fields.Add(new Field(field2.Name, field2.DataType, i));
                }
            }
            Records records = new Records(Math.Min(10, base.Count));
            foreach (Record record in this)
            {
                if (FileDbEngine.evaluate(filter, record.Values.ToArray(), this._fields))
                {
                    object[] values = new object[length];
                    if (fieldList != null)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            string str2 = fieldList[i];
                            values[i] = record[str2];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++)
                        {
                            values[i] = record[i];
                        }
                    }
                    Record item = new Record(fields, values);
                    records.Add(item);
                }
            }
            if (orderByList != null)
            {
                this.orderBy(records, fieldList, orderByList);
            }
            return new Table(fields, records, false);
        }

        public Table SelectRecords(string filter, string[] fieldList, string[] orderByList)
        {
            FilterExpressionGroup group = FilterExpressionGroup.Parse(filter);
            return this.SelectRecords(group, fieldList, orderByList);
        }

        public override string ToString()
        {
            return string.Format("NumRecords = {0}", base.Count);
        }

        public FwNs.Data.FyleDbNs.Fields Fields
        {
            get
            {
                return this._fields;
            }
        }
    }
}


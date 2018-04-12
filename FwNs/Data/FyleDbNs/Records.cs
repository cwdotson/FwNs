namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections.Generic;

    public class Records : List<Record>
    {
        public Records()
        {
        }

        public Records(int capacity) : base(capacity)
        {
        }

        internal Records(Fields fields, object[][] records) : base(records.Length)
        {
            for (int i = 0; i < records.Length; i++)
            {
                object[] values = records[i];
                Record item = new Record(fields, values);
                base.Add(item);
            }
        }

        internal Records(Fields fields, object[] record) : base(1)
        {
            Record item = new Record(fields, record);
            base.Add(item);
        }
    }
}


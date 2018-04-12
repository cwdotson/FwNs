namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections;

    public class ObjectEnumerator : IEnumerator
    {
        private object[] _row;
        private int position = -1;

        public ObjectEnumerator(object[] list)
        {
            this._row = list;
        }

        public bool MoveNext()
        {
            this.position++;
            return (this.position < this._row.Length);
        }

        public void Reset()
        {
            this.position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public object Current
        {
            get
            {
                object obj2;
                try
                {
                    obj2 = this._row[this.position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
                return obj2;
            }
        }
    }
}


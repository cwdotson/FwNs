namespace FwNs.Linq.Xprsns
{
    using System;
    using System.IO;
    using System.Text;

    internal class IndentedTextWriter : TextWriter
    {
        public const string DefaultTabString = "    ";
        private int _indentLevel;
        private bool _tabsPending;
        private readonly string _tabString;
        private readonly TextWriter _writer;

        public IndentedTextWriter(TextWriter writer) : this(writer, "    ")
        {
        }

        public IndentedTextWriter(TextWriter writer, string tabString) : base(CultureInfo.InvariantCulture)
        {
            this._writer = writer;
            this._tabString = tabString;
            this._indentLevel = 0;
            this._tabsPending = false;
        }

        public override void Close()
        {
            this._writer.Close();
        }

        public override void Flush()
        {
            this._writer.Flush();
        }

        internal void InternalOutputTabs()
        {
            for (int i = 0; i < this._indentLevel; i++)
            {
                this._writer.Write(this._tabString);
            }
        }

        protected virtual void OutputTabs()
        {
            if (this._tabsPending)
            {
                for (int i = 0; i < this._indentLevel; i++)
                {
                    this._writer.Write(this._tabString);
                }
                this._tabsPending = false;
            }
        }

        public override void Write(bool value)
        {
            this.OutputTabs();
            this._writer.Write(value);
        }

        public override void Write(char value)
        {
            this.OutputTabs();
            this._writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            this.OutputTabs();
            this._writer.Write(buffer);
        }

        public override void Write(double value)
        {
            this.OutputTabs();
            this._writer.Write(value);
        }

        public override void Write(int value)
        {
            this.OutputTabs();
            this._writer.Write(value);
        }

        public override void Write(long value)
        {
            this.OutputTabs();
            this._writer.Write(value);
        }

        public override void Write(object value)
        {
            this.OutputTabs();
            this._writer.Write(value);
        }

        public override void Write(float value)
        {
            this.OutputTabs();
            this._writer.Write(value);
        }

        public override void Write(string s)
        {
            this.OutputTabs();
            this._writer.Write(s);
        }

        public override void Write(string format, object arg0)
        {
            this.OutputTabs();
            this._writer.Write(format, arg0);
        }

        public override void Write(string format, params object[] arg)
        {
            this.OutputTabs();
            this._writer.Write(format, arg);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            this.OutputTabs();
            this._writer.Write(buffer, index, count);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            this.OutputTabs();
            this._writer.Write(format, arg0, arg1);
        }

        public override void WriteLine()
        {
            this.OutputTabs();
            this._writer.WriteLine();
            this._tabsPending = true;
        }

        public override void WriteLine(bool value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(char value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(double value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(char[] buffer)
        {
            this.OutputTabs();
            this._writer.WriteLine(buffer);
            this._tabsPending = true;
        }

        public override void WriteLine(int value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(long value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(object value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(float value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(string s)
        {
            this.OutputTabs();
            this._writer.WriteLine(s);
            this._tabsPending = true;
        }

        public override void WriteLine(uint value)
        {
            this.OutputTabs();
            this._writer.WriteLine(value);
            this._tabsPending = true;
        }

        public override void WriteLine(string format, object arg0)
        {
            this.OutputTabs();
            this._writer.WriteLine(format, arg0);
            this._tabsPending = true;
        }

        public override void WriteLine(string format, params object[] arg)
        {
            this.OutputTabs();
            this._writer.WriteLine(format, arg);
            this._tabsPending = true;
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            this.OutputTabs();
            this._writer.WriteLine(buffer, index, count);
            this._tabsPending = true;
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            this.OutputTabs();
            this._writer.WriteLine(format, arg0, arg1);
            this._tabsPending = true;
        }

        public void WriteLineNoTabs(string s)
        {
            this._writer.WriteLine(s);
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return this._writer.Encoding;
            }
        }

        public int Indent
        {
            get
            {
                return this._indentLevel;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this._indentLevel = value;
            }
        }

        public TextWriter InnerWriter
        {
            get
            {
                return this._writer;
            }
        }

        public override string NewLine
        {
            get
            {
                return this._writer.NewLine;
            }
            set
            {
                this._writer.NewLine = value;
            }
        }

        internal string TabString
        {
            get
            {
                return this._tabString;
            }
        }
    }
}


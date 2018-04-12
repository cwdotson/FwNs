namespace FwNs.Core.LC.cLib
{
    using System;
    using System.IO;

    public sealed class SimpleLog : IDisposable
    {
        public const int LogNone = 0;
        public const int LogError = 1;
        public const int LogNormal = 2;
        private readonly UtlFileAccess _fileAccess;
        private readonly string _filePath;
        private bool _isSystem;
        private int _level;
        private TextWriter _writer;

        public SimpleLog(string path, int level, UtlFileAccess fileAccess)
        {
            this._isSystem = path == null;
            this._filePath = path;
            this._fileAccess = fileAccess;
            this.SetLevel(level);
        }

        public void Close()
        {
            if ((this._writer != null) && !this._isSystem)
            {
                this._writer.Close();
            }
            this._writer = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && (this._writer == Console.Out))
            {
                this._writer.Dispose();
            }
        }

        public void Flush()
        {
            if (this._writer != null)
            {
                this._writer.Flush();
            }
        }

        public int GetLevel()
        {
            return this._level;
        }

        public TextWriter GetPrintWriter()
        {
            return this._writer;
        }

        public void LogContext(Exception t, string message)
        {
            lock (this)
            {
                if (this._level != 0)
                {
                    string str = DateTime.Now.ToString();
                    if (message == null)
                    {
                        message = "";
                    }
                    try
                    {
                        this._writer.WriteLine(string.Concat(new object[] { str, " ", t, " ", message }));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void LogContext(int atLevel, string message)
        {
            lock (this)
            {
                if (this._level >= atLevel)
                {
                    string str = DateTime.Now.ToString();
                    try
                    {
                        this._writer.WriteLine(str + " " + message);
                        this._writer.Flush();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void LogContext(Exception t, string message, string sql)
        {
            lock (this)
            {
                if (this._level != 0)
                {
                    string str = DateTime.Now.ToString();
                    if (sql == null)
                    {
                        sql = "";
                    }
                    if (message == null)
                    {
                        message = "";
                    }
                    try
                    {
                        this._writer.WriteLine(str + " " + (sql + "\n " + t.Message + "\n " + t.StackTrace + ((t.InnerException != null) ? (t.InnerException.Message + "\n " + t.InnerException.StackTrace) : "")));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void SetLevel(int level)
        {
            this._level = level;
            this.SetupWriter();
        }

        private void SetupLog(string file)
        {
            try
            {
                this._fileAccess.CreateParentDirs(file);
                this._writer = new StreamWriter(this._fileAccess.OpenOutputStreamElement(file, FileMode.Append, FileAccess.Write));
            }
            catch (Exception)
            {
                this._isSystem = true;
                this._writer = Console.Out;
            }
        }

        private void SetupWriter()
        {
            if (this._level == 0)
            {
                this.Close();
            }
            else if (this._writer == null)
            {
                if (this._isSystem)
                {
                    this._writer = Console.Out;
                }
                else
                {
                    this.SetupLog(this._filePath);
                }
            }
        }
    }
}


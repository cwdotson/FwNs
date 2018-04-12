namespace FwNs.Core.LC.cScriptIO
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cPersist;
    using System;
    using System.IO;
    using System.IO.Compression;

    public sealed class ScriptWriterEncode : ScriptWriterText
    {
        private readonly Crypto _crypto;
        private readonly MemoryStream _memOut;
        private readonly BinaryWriter _byteOut;

        public ScriptWriterEncode(Database db, string file, Crypto crypto) : base(db, file, false, false, false)
        {
            this._crypto = crypto;
            this._memOut = new MemoryStream();
            this._byteOut = new BinaryWriter(this._memOut);
        }

        public ScriptWriterEncode(Database db, string file, bool includeCached, Crypto crypto) : base(db, file, includeCached, true, false)
        {
            try
            {
                base.FileStreamOut = crypto.GetOutputStream(base.FileStreamOut);
                base.FileStreamOut = new GZipStream(base.FileStreamOut, CompressionMode.Compress);
            }
            catch (IOException exception)
            {
                object[] add = new object[] { exception.Message, base.OutFile };
                throw Error.GetError(exception, 0x1c4, 0x1a, add);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this._memOut != null))
            {
                this._byteOut.Close();
                this._memOut.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void FinishStream()
        {
            base.FileStreamOut.Flush();
        }

        public override void WriteRowOutToFile()
        {
            lock (base.FileStreamOutLock)
            {
                if (this._byteOut == null)
                {
                    base.WriteRowOutToFile();
                }
                else
                {
                    byte[] buffer = this._crypto.Encode(base.RowOut.GetBuffer(), 0, base.RowOut.Size());
                    int length = buffer.Length;
                    this._byteOut.Seek(0, SeekOrigin.Begin);
                    this._byteOut.Write(length);
                    this._byteOut.Write(buffer, 0, length);
                    this._byteOut.Flush();
                    base.FileStreamOut.Write(this._memOut.GetBuffer(), 0, length + 4);
                }
            }
        }
    }
}


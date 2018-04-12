namespace FwNs.Core.LC.cScriptIO
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRowIO;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    public class ScriptReaderDecode : ScriptReaderText
    {
        private readonly Stream _dataInput;
        private readonly Crypto _crypto;
        private byte[] _buffer;
        private readonly Stream _cStream;

        public ScriptReaderDecode(Database db, string fileName, Crypto crypto) : base(db)
        {
            this._buffer = new byte[0x100];
            Stream input = base.database.logger.GetFileAccess().OpenInputStreamElement(fileName);
            this._cStream = crypto.GetInputStream(input);
            input = new GZipStream(this._cStream, CompressionMode.Decompress);
            base.DataStreamIn = new StreamReader(input);
            base.RowIn = new RowInputTextLog();
        }

        public ScriptReaderDecode(Database db, string fileName, Crypto crypto, bool forLog) : base(db)
        {
            this._buffer = new byte[0x100];
            this._crypto = crypto;
            Stream stream = base.database.logger.GetFileAccess().OpenInputStreamElement(fileName);
            this._dataInput = stream;
            base.RowIn = new RowInputTextLog();
        }

        public override void Close()
        {
            try
            {
                if (this._cStream != null)
                {
                    this._cStream.Close();
                }
                if (base.DataStreamIn != null)
                {
                    base.DataStreamIn.Close();
                }
                if (this._dataInput != null)
                {
                    this._dataInput.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public override bool ReadLoggedStatement(Session session)
        {
            int length;
            if (this._dataInput == null)
            {
                return base.ReadLoggedStatement(session);
            }
            BinaryReader reader = new BinaryReader(this._dataInput);
            try
            {
                length = reader.ReadInt32();
                if ((length * 2) > this._buffer.Length)
                {
                    this._buffer = new byte[length * 2];
                }
                reader.Read(this._buffer, 0, length);
            }
            catch (Exception)
            {
                return false;
            }
            this._buffer = this._crypto.Decode(this._buffer, 0, length);
            length = this._buffer.Length;
            string s = Encoding.UTF8.GetString(this._buffer, 0, length);
            base.LineCount++;
            base.statement = StringConverter.UnicodeStringToString(s);
            if (base.statement == null)
            {
                return false;
            }
            base.ProcessStatement(session);
            return true;
        }
    }
}


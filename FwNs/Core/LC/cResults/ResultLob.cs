namespace FwNs.Core.LC.cResults
{
    using System;
    using System.IO;

    public class ResultLob : Result
    {
        private long _blockLength;
        private long _blockOffset;
        private byte[] _byteBlock;
        private char[] _charBlock;
        private long _lobId;
        private TextReader _reader;
        private Stream _stream;
        private int _subType;

        private ResultLob() : base(0x12)
        {
        }

        public long GetBlockLength()
        {
            return this._blockLength;
        }

        public byte[] GetByteArray()
        {
            return this._byteBlock;
        }

        public char[] GetCharArray()
        {
            return this._charBlock;
        }

        public Stream GetInputStream()
        {
            return this._stream;
        }

        public long GetLobId()
        {
            return this._lobId;
        }

        public long GetOffset()
        {
            return this._blockOffset;
        }

        public TextReader GetReader()
        {
            return this._reader;
        }

        public int GetSubType()
        {
            return this._subType;
        }

        public static ResultLob NewLobCreateBlobRequest(long sessionId, long lobId, Stream stream, long length)
        {
            return new ResultLob { 
                _lobId = lobId,
                _subType = 7,
                _blockLength = length,
                _stream = stream
            };
        }

        public static ResultLob NewLobCreateBlobResponse(long id)
        {
            return new ResultLob { 
                _subType = 0x1b,
                _lobId = id
            };
        }

        public static ResultLob NewLobCreateClobRequest(long sessionId, long lobId, TextReader reader, long length)
        {
            return new ResultLob { 
                _lobId = lobId,
                _subType = 8,
                _blockLength = length,
                _reader = reader
            };
        }

        public static ResultLob NewLobCreateClobResponse(long id)
        {
            return new ResultLob { 
                _subType = 0x1c,
                _lobId = id
            };
        }

        public static ResultLob NewLobGetBytePatternPositionRequest(long id, byte[] pattern, long offset)
        {
            return new ResultLob { 
                _subType = 5,
                _lobId = id,
                _blockOffset = offset,
                _byteBlock = pattern,
                _blockLength = pattern.Length
            };
        }

        public static ResultLob NewLobGetBytesRequest(long id, long offset, int length)
        {
            return new ResultLob { 
                _subType = 1,
                _lobId = id,
                _blockOffset = offset,
                _blockLength = length
            };
        }

        public static ResultLob NewLobGetBytesResponse(long id, long offset, byte[] block)
        {
            return new ResultLob { 
                _subType = 0x15,
                _lobId = id,
                _blockOffset = offset,
                _byteBlock = block,
                _blockLength = block.Length
            };
        }

        public static ResultLob NewLobGetCharPatternPositionRequest(long id, char[] pattern, long offset)
        {
            return new ResultLob { 
                _subType = 6,
                _lobId = id,
                _blockOffset = offset,
                _charBlock = pattern,
                _blockLength = pattern.Length
            };
        }

        public static ResultLob NewLobGetCharsRequest(long id, long offset, int length)
        {
            return new ResultLob { 
                _subType = 3,
                _lobId = id,
                _blockOffset = offset,
                _blockLength = length
            };
        }

        public static ResultLob NewLobGetCharsResponse(long id, long offset, char[] chars)
        {
            return new ResultLob { 
                _subType = 0x17,
                _lobId = id,
                _blockOffset = offset,
                _charBlock = chars,
                _blockLength = chars.Length
            };
        }

        public static ResultLob NewLobGetLengthRequest(long id)
        {
            return new ResultLob { 
                _subType = 10,
                _lobId = id
            };
        }

        public static ResultLob NewLobGetRequest(long id, long offset, long length)
        {
            return new ResultLob { 
                _subType = 11,
                _lobId = id,
                _blockOffset = offset,
                _blockLength = length
            };
        }

        public static ResultLob NewLobSetBytesRequest(long id, long offset, byte[] block)
        {
            return new ResultLob { 
                _subType = 2,
                _lobId = id,
                _blockOffset = offset,
                _byteBlock = block,
                _blockLength = block.Length
            };
        }

        public static ResultLob NewLobSetCharsRequest(long id, long offset, char[] chars)
        {
            return new ResultLob { 
                _subType = 4,
                _lobId = id,
                _blockOffset = offset,
                _charBlock = chars,
                _blockLength = chars.Length
            };
        }

        public static ResultLob NewLobSetResponse(long id, long length)
        {
            return new ResultLob { 
                _subType = 0x16,
                _lobId = id,
                _blockLength = length
            };
        }

        public static ResultLob NewLobTruncateRequest(long id, long offset)
        {
            return new ResultLob { 
                _subType = 9,
                _lobId = id,
                _blockOffset = offset
            };
        }

        public static ResultLob NewLobTruncateResponse(long id)
        {
            return new ResultLob { 
                _subType = 0x1d,
                _lobId = id
            };
        }
    }
}


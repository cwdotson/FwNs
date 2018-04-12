namespace System.Data.LibCore
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cResults;
    using System;
    using System.Data.Common;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public sealed class UtlException : DbException
    {
        private readonly string _sqlState;
        private readonly int _errorCode;

        public UtlException()
        {
        }

        public UtlException(string message) : base(message)
        {
        }

        private UtlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UtlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UtlException(string message, string sqlState, int errorCode) : base(message)
        {
            this._sqlState = sqlState;
        }

        public UtlException(string message, string sqlState, int errorCode, Exception innerException) : base(message, innerException)
        {
            this._sqlState = sqlState;
            this._errorCode = errorCode;
        }

        public static UtlException GetException(CoreException e)
        {
            return new UtlException(e.GetMessage(), e.GetSqlState(), e.GetErrorCode(), e);
        }

        public static UtlException GetException(int id)
        {
            return GetException(Error.GetError(id));
        }

        public static UtlException GetException(int id, string message)
        {
            return GetException(Error.GetError(id, message));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public static UtlException SqlWarning(Result r)
        {
            return new UtlException(r.GetMainString(), r.GetSubString(), r.GetErrorCode());
        }

        public static void ThrowError(CoreException e)
        {
            throw new UtlException(e.GetMessage(), e.GetSqlState(), e.GetErrorCode(), e);
        }

        public static void ThrowError(Result r)
        {
            throw new UtlException(r.GetMainString(), r.GetSubString(), r.GetErrorCode(), r.GetException());
        }

        public override int ErrorCode
        {
            get
            {
                return this._errorCode;
            }
        }

        public string SqlState
        {
            get
            {
                return this._sqlState;
            }
        }
    }
}


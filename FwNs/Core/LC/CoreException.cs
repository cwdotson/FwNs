namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cResults;
    using System;
    using System.Runtime.Serialization;
    using System.Security;

    [Serializable]
    public class CoreException : Exception, IEquatable<CoreException>
    {
        public static CoreException[] EmptyArray = new CoreException[0];
        public static CoreException NoDataCondition = Error.GetError(0x44c);
        private readonly string _msg;
        private readonly string _state;
        private readonly int _code;
        private int _level;
        private int _statementCode;

        public CoreException()
        {
        }

        public CoreException(Result r)
        {
            this._msg = r.GetMainString();
            this._state = r.GetSubString();
            this._code = (int) r.GetStatementId();
        }

        public CoreException(string message) : base(message)
        {
            this._msg = message;
        }

        protected CoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._code = info.GetInt32("_code");
            this._level = info.GetInt32("_level");
            this._msg = info.GetString("_msg");
            this._state = info.GetString("_state");
            this._statementCode = info.GetInt32("_statementCode");
        }

        public CoreException(string message, Exception t) : base(message, t)
        {
            this._msg = message;
        }

        public CoreException(Exception t, string errorState, int errorCode)
        {
            this._msg = t.ToString();
            this._state = errorState;
            this._code = errorCode;
        }

        public CoreException(string message, string state, int code)
        {
            this._msg = message;
            this._state = state;
            this._code = code;
        }

        public CoreException(Exception t, string message, string state, int code) : base(message, t)
        {
            this._msg = message;
            this._state = state;
            this._code = code;
        }

        public bool Equals(CoreException otherException)
        {
            return ((((otherException != null) && (this._code == otherException._code)) && Equals(this._state, otherException._state)) && Equals(this._msg, otherException._msg));
        }

        public override bool Equals(object other)
        {
            CoreException otherException = other as CoreException;
            return ((otherException != null) && this.Equals(otherException));
        }

        public static bool Equals(object a, object b)
        {
            return ((a == b) || (((a != null) && (b != null)) && a.Equals(b)));
        }

        public int GetErrorCode()
        {
            return this._code;
        }

        public override int GetHashCode()
        {
            return this._code;
        }

        public int GetLevel()
        {
            return this._level;
        }

        public string GetMessage()
        {
            return this._msg;
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public string GetSqlState()
        {
            return this._state;
        }

        public int GetStatementCode()
        {
            return this._statementCode;
        }

        public void SetLevel(int level)
        {
            this._level = level;
        }

        public void SetStatementType(int group, int code)
        {
            this._statementCode = code;
        }
    }
}


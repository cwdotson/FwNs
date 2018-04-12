namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cSchemas;
    using System;

    public sealed class Condition
    {
        public QNameManager.QName Name;
        public string SqlState;
        public Expression ErrNo;
        public bool IsSqlState;

        public Condition(QNameManager.QName name, Expression errNo)
        {
            this.Name = name;
            this.ErrNo = errNo;
            this.IsSqlState = false;
        }

        public Condition(QNameManager.QName name, string sqlState)
        {
            this.Name = name;
            this.SqlState = sqlState;
            this.IsSqlState = true;
        }
    }
}


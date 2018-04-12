namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Text;

    public sealed class ExpressionOrderBy : Expression
    {
        private bool _isDescending;
        private bool _isNullsLast;

        public ExpressionOrderBy(Expression e) : base(0x5e)
        {
            base.nodes = new Expression[] { e };
        }

        public override string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            builder.Append(base.GetLeftNode().Describe(session, blanks));
            if (this._isDescending)
            {
                builder.Append("DESC").Append(' ');
            }
            return builder.ToString();
        }

        public override string GetSql()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ORDER").Append(' ').Append("BY").Append(' ');
            if (base.nodes[0].Alias != null)
            {
                builder.Append(base.nodes[0].Alias.Name);
            }
            else
            {
                builder.Append(base.nodes[0].GetSql());
            }
            if (this._isDescending)
            {
                builder.Append(' ').Append("DESC");
            }
            return builder.ToString();
        }

        public override object GetValue(Session session)
        {
            return base.nodes[0].GetValue(session);
        }

        public bool IsDescending()
        {
            return this._isDescending;
        }

        public bool IsNullsLast()
        {
            return this._isNullsLast;
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            base.nodes[0].ResolveTypes(session, parent);
            if (base.nodes[0].IsUnresolvedParam())
            {
                throw Error.GetError(0x15bf);
            }
            base.DataType = base.nodes[0].DataType;
        }

        public void SetDescending()
        {
            this._isDescending = true;
        }

        public void SetNullsLast()
        {
            this._isNullsLast = true;
        }
    }
}


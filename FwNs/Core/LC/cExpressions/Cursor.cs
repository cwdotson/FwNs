namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cStatements;
    using System;

    public sealed class Cursor : Expression
    {
        private readonly StatementQuery _queryStatement;
        private RowSetNavigator _navigator;
        private Result _result;
        private CursorState _state;

        public Cursor(StatementQuery queryStatement) : base(100)
        {
            base.nodes = Expression.emptyArray;
            this._queryStatement = queryStatement;
            base.DataType = SqlType.SqlAllTypes;
            this._state = CursorState.Closed;
            this._result = null;
            this._navigator = null;
        }

        private void CheckClosed(string cursorName)
        {
            if (this._state != CursorState.Closed)
            {
                throw Error.GetError(0xe12, cursorName);
            }
        }

        private void CheckOpen()
        {
            if (this._state != CursorState.Open)
            {
                throw Error.GetError(0xe11);
            }
        }

        public Cursor Clone()
        {
            return new Cursor(this._queryStatement) { 
                nodes = Expression.emptyArray,
                DataType = base.DataType,
                _state = this._state,
                _result = this._result,
                _navigator = this._navigator
            };
        }

        public void Close(Session session)
        {
            this.CheckOpen();
            this._result = null;
            this._navigator.Close();
            this._state = CursorState.Closed;
        }

        public void CollectObjectNames(OrderedHashSet<QNameManager.QName> set)
        {
            this._queryStatement.CollectTableNamesForRead(set);
        }

        public object[] Fetch(Session session)
        {
            this.CheckOpen();
            if (!this._navigator.Next())
            {
                throw Error.GetError(0x44c);
            }
            return this._navigator.GetCurrent();
        }

        public SqlType[] GetResultDataTypes()
        {
            this.CheckOpen();
            return this._result.MetaData.ColumnTypes;
        }

        public void Open(Session session, string cursorName)
        {
            this.CheckClosed(cursorName);
            this._result = this._queryStatement.Execute(session);
            if (this._result.IsError())
            {
                throw this._result.GetException();
            }
            if (!this._result.IsData())
            {
                throw Error.GetError(0xe10);
            }
            this._navigator = this._result.GetNavigator();
            this._state = CursorState.Open;
        }
    }
}


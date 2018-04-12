namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class StatementHandler : Statement
    {
        public const int None = 0;
        public const int SqlException = 1;
        public const int SqlWarning = 2;
        public const int SqlNotFound = 3;
        public const int SqlState = 4;
        public const int ErrorCode = 5;
        public const int Condition = 6;
        public const int Continue = 5;
        public const int Exit = 6;
        public const int Undo = 7;
        public static readonly StatementHandler[] EmptyExceptionHandlerArray = new StatementHandler[0];
        private readonly OrderedHashSet<int> _conditionErroCodes;
        private readonly OrderedIntHashSet _conditionGroups;
        private readonly OrderedHashSet<string> _conditionNames;
        private readonly OrderedHashSet<string> _conditionStates;
        public int HandlerType;
        private Statement _statement;

        public StatementHandler(int handlerType) : base(0x44e, 0x7d7)
        {
            this._conditionErroCodes = new OrderedHashSet<int>();
            this._conditionGroups = new OrderedIntHashSet();
            this._conditionNames = new OrderedHashSet<string>();
            this._conditionStates = new OrderedHashSet<string>();
            this.HandlerType = handlerType;
        }

        public void AddConditionErrorCode(int errorCode)
        {
            if (!(((this._conditionErroCodes.Add(errorCode) & this._conditionGroups.IsEmpty()) & this._conditionStates.IsEmpty()) & this._conditionNames.IsEmpty()))
            {
                throw Error.GetError(0x15e4);
            }
        }

        public void AddConditionName(QNameManager.QName conditionName)
        {
            if (!(((this._conditionNames.Add(conditionName.Name) & this._conditionGroups.IsEmpty()) & this._conditionStates.IsEmpty()) & this._conditionErroCodes.IsEmpty()))
            {
                throw Error.GetError(0x15e4);
            }
        }

        public void AddConditionState(string sqlState)
        {
            if (!(((this._conditionStates.Add(sqlState) & this._conditionGroups.IsEmpty()) & this._conditionErroCodes.IsEmpty()) & this._conditionNames.IsEmpty()))
            {
                throw Error.GetError(0x15e4);
            }
        }

        public void AddConditionType(int conditionType)
        {
            if (!((this._conditionGroups.Add(conditionType) & this._conditionStates.IsEmpty()) & this._conditionErroCodes.IsEmpty()))
            {
                throw Error.GetError(0x15e4);
            }
        }

        public void AddStatement(Statement s)
        {
            this._statement = s;
        }

        public override string Describe(Session session)
        {
            return "";
        }

        public override Result Execute(Session session)
        {
            if (this._statement != null)
            {
                return this._statement.Execute(session);
            }
            return Result.UpdateZeroResult;
        }

        public string[] GetConditionStates()
        {
            return this._conditionStates.ToArray(new string[this._conditionStates.Size()]);
        }

        public int[] GetConditionTypes()
        {
            return this._conditionGroups.ToArray();
        }

        public override OrderedHashSet<QNameManager.QName> GetReferences()
        {
            if (this._statement == null)
            {
                return new OrderedHashSet<QNameManager.QName>();
            }
            return this._statement.GetReferences();
        }

        public override string GetSql()
        {
            return base.Sql;
        }

        public bool HandlesCondition(string sqlState, int errorCode)
        {
            if (this._conditionStates.Contains(sqlState))
            {
                return true;
            }
            if (this._conditionErroCodes.Contains(-1 * errorCode))
            {
                return true;
            }
            string key = sqlState.Substring(0, 2);
            if (this._conditionStates.Contains(key))
            {
                return true;
            }
            if (key.Equals("01"))
            {
                return this._conditionGroups.Contains(2);
            }
            if (key.Equals("02"))
            {
                return this._conditionGroups.Contains(3);
            }
            return this._conditionGroups.Contains(1);
        }

        public override bool IsCatalogChange()
        {
            return false;
        }

        public override void Resolve(Session session, RangeVariable[] rangeVars)
        {
            base.Resolve(session, rangeVars);
            if (this._statement != null)
            {
                this._statement.Resolve(session, rangeVars);
                base.ReadTableNames = this._statement.GetTableNamesForRead();
                base.WriteTableNames = this._statement.GetTableNamesForWrite();
            }
            if (!this._conditionNames.IsEmpty())
            {
                int[] indexes = new int[this._conditionNames.Size()];
                ColumnSchema[] variables = new ColumnSchema[this._conditionNames.Size()];
                StatementSet.SetVariables(rangeVars, this._conditionNames, indexes, variables);
                for (int i = 0; i < this._conditionNames.Size(); i++)
                {
                    ColumnSchema schema1 = variables[i];
                    if (schema1 == null)
                    {
                        throw Error.GetError(0x19e0, this._conditionNames.Get(i));
                    }
                    FwNs.Core.LC.cStatements.Condition condition = (FwNs.Core.LC.cStatements.Condition) schema1.GetDefaultExpression().GetValue(session);
                    if (condition.IsSqlState)
                    {
                        this._conditionStates.Add(condition.SqlState);
                    }
                    else
                    {
                        this._conditionErroCodes.Add((int) condition.ErrNo.GetValue(session, SqlType.SqlInteger));
                    }
                }
            }
        }
    }
}


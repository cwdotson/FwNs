namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Text;

    public sealed class TriggerDefSQL : TriggerDef
    {
        public TriggerDefSQL(QNameManager.QName name, int when, int operation, bool forEachRow, Table table, Table[] transitions, RangeVariable[] rangeVars, Expression condition, string conditionSql, int[] updateColumns, Routine routine) : base(name, when, operation, forEachRow, table, transitions, rangeVars, condition, conditionSql, updateColumns)
        {
            base.routine = routine;
        }

        public override void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public override string GetClassName()
        {
            return null;
        }

        public override OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public override string GetProcedureSql()
        {
            return base.routine.statement.GetSql();
        }

        public override OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return base.routine.GetReferences();
        }

        public override string GetSql()
        {
            StringBuilder sqlMain = base.GetSqlMain();
            sqlMain.Append(base.routine.statement.GetSql());
            return sqlMain.ToString();
        }

        public override bool HasNewTable()
        {
            return (base.Transitions[3] > null);
        }

        public override bool HasOldTable()
        {
            return (base.Transitions[2] > null);
        }

        public override void PushPair(Session session, object[] oldData, object[] newData)
        {
            lock (this)
            {
                Result updateZeroResult = Result.UpdateZeroResult;
                if (session.sessionContext.Depth > 0x80)
                {
                    throw Error.GetError(0x1ca);
                }
                session.sessionContext.Push(true);
                if (base.Transitions[0] != null)
                {
                    base.RangeVars[0].GetIterator(session).SetCurrent(oldData);
                }
                if (base.Transitions[1] != null)
                {
                    base.RangeVars[1].GetIterator(session).SetCurrent(newData);
                }
                session.sessionContext.triggerOldData = oldData;
                session.sessionContext.triggerNewData = newData;
                session.sessionContext.RestoreTriggerIterators = new SessionContext.RestoreTriggerRangeIterators(this.RetoreTriggerIterators);
                if (base.Condition.TestCondition(session))
                {
                    session.sessionContext.RoutineVariables = new object[base.routine.VariableCount];
                    updateZeroResult = base.routine.statement.Execute(session);
                }
                session.sessionContext.triggerOldData = null;
                session.sessionContext.triggerNewData = null;
                session.sessionContext.RestoreTriggerIterators = null;
                session.sessionContext.Pop(true);
                if (updateZeroResult.IsError())
                {
                    throw updateZeroResult.GetException();
                }
            }
        }

        public void RetoreTriggerIterators(Session session, object[] oldData, object[] newData)
        {
            if (base.Transitions[0] != null)
            {
                base.RangeVars[0].GetIterator(session).SetCurrent(oldData);
            }
            if (base.Transitions[1] != null)
            {
                base.RangeVars[1].GetIterator(session).SetCurrent(newData);
            }
        }
    }
}


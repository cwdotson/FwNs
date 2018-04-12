namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public class TriggerDef : ISchemaObject
    {
        public const int OldRow = 0;
        public const int NewRow = 1;
        public const int OldTable = 2;
        public const int NewTable = 3;
        public const int Before = 4;
        public const int After = 5;
        public const int Instead = 6;
        public const int NumTriggerOps = 3;
        public const int NumTrigs = 9;
        private const long ChangeTimestamp = 0L;
        public static readonly TriggerDef[] EmptyArray = new TriggerDef[0];
        public Table[] Transitions;
        public RangeVariable[] RangeVars;
        public Expression Condition;
        private readonly bool _hasTransitionTables;
        private readonly bool _hasTransitionRanges;
        private readonly string _conditionSql;
        public string ProcedureSql;
        public Statement[] Statements;
        public Routine routine;
        private readonly int[] _updateColumns;
        public QNameManager.QName Name;
        public int ActionTiming;
        public int OperationType;
        private readonly bool _forEachRow;
        private readonly int _maxRowsQueued;
        public Table table;
        private readonly ITrigger _trigger;
        private readonly string _triggerClassName;
        public int TriggerType;
        protected Queue<TriggerData> PendingQueue;
        protected int RowsQueued;
        protected bool Valid;
        protected bool KeepGoing;

        public TriggerDef()
        {
            this.Statements = Statement.EmptyArray;
            this.Valid = true;
            this.KeepGoing = true;
        }

        public TriggerDef(QNameManager.QName name, int when, int operation, bool forEachRow, Table table, Table[] transitions, RangeVariable[] rangeVars, Expression condition, string conditionSql, int[] updateColumns)
        {
            this.Statements = Statement.EmptyArray;
            this.Valid = true;
            this.KeepGoing = true;
            this.Name = name;
            this.ActionTiming = when;
            this.OperationType = operation;
            this._forEachRow = forEachRow;
            this.table = table;
            this.Transitions = transitions;
            this.RangeVars = rangeVars;
            this.Condition = condition ?? new ExpressionLogical(true);
            this._updateColumns = updateColumns;
            this._conditionSql = conditionSql;
            this._hasTransitionRanges = (transitions[0] != null) || (transitions[1] > null);
            this._hasTransitionTables = (transitions[2] != null) || (transitions[3] > null);
            this.SetUpIndexesAndTypes();
        }

        public TriggerDef(QNameManager.QName name, int when, int operation, bool forEach, Table table, Table[] transitions, RangeVariable[] rangeVars, Expression condition, string conditionSql, int[] updateColumns, string triggerClassName, bool noWait, int queueSize) : this(name, when, operation, forEach, table, transitions, rangeVars, condition, conditionSql, updateColumns)
        {
            this._triggerClassName = triggerClassName;
            this._maxRowsQueued = queueSize;
            this.RowsQueued = 0;
            this.PendingQueue = new Queue<TriggerData>();
            Type type = null;
            try
            {
                int index = triggerClassName.IndexOf(":");
                if (index != -1)
                {
                    triggerClassName = triggerClassName.Substring(index + 1);
                    type = Assembly.Load(triggerClassName.Substring(0, index)).GetType(triggerClassName);
                }
                else
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        type = assemblies[i].GetType(triggerClassName);
                        if (type != null)
                        {
                            goto Label_00AD;
                        }
                    }
                }
            }
            catch (Exception)
            {
                type = typeof(DefaultTrigger);
            }
        Label_00AD:
            if (type == null)
            {
                this.Valid = false;
                this._trigger = new DefaultTrigger();
            }
            else
            {
                try
                {
                    this._trigger = (ITrigger) Activator.CreateInstance(type);
                }
                catch (Exception)
                {
                    this.Valid = false;
                    this._trigger = new DefaultTrigger();
                }
            }
        }

        public virtual void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public string GetActionOrientationString()
        {
            if (!this._forEachRow)
            {
                return "STATEMENT";
            }
            return "ROW";
        }

        public string GetActionTimingString()
        {
            switch (this.ActionTiming)
            {
                case 4:
                    return "BEFORE";

                case 5:
                    return "AFTER";

                case 6:
                    return "INSTEAD OF";
            }
            throw Error.RuntimeError(0xc9, "TriggerDef");
        }

        public virtual QNameManager.QName GetCatalogName()
        {
            return this.Name.schema.schema;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public virtual string GetClassName()
        {
            return this._trigger.GetType().FullName;
        }

        public virtual OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public string GetConditionSql()
        {
            return this._conditionSql;
        }

        public string GetEventTypeString()
        {
            int operationType = this.OperationType;
            switch (operationType)
            {
                case 0x13:
                    return "DELETE";

                case 50:
                    return "INSERT";
            }
            if (operationType != 0x52)
            {
                throw Error.RuntimeError(0xc9, "TriggerDef");
            }
            return "UPDATE";
        }

        public virtual QNameManager.QName GetName()
        {
            return this.Name;
        }

        public string GetNewTransitionRowName()
        {
            if (this.Transitions[1] != null)
            {
                return this.Transitions[1].GetName().Name;
            }
            return null;
        }

        public string GetNewTransitionTableName()
        {
            if (this.Transitions[3] != null)
            {
                return this.Transitions[3].GetName().Name;
            }
            return null;
        }

        public string GetOldTransitionRowName()
        {
            if (this.Transitions[0] != null)
            {
                return this.Transitions[0].GetName().Name;
            }
            return null;
        }

        public string GetOldTransitionTableName()
        {
            if (this.Transitions[2] != null)
            {
                return this.Transitions[2].GetName().Name;
            }
            return null;
        }

        public static int GetOperationType(int token)
        {
            if (token == 0x4e)
            {
                return 0x13;
            }
            if (token == 0x85)
            {
                return 50;
            }
            if (token != 0x12d)
            {
                throw Error.RuntimeError(0xc9, "TriggerDef");
            }
            return 0x52;
        }

        public virtual Grantee GetOwner()
        {
            return this.Name.schema.Owner;
        }

        public virtual string GetProcedureSql()
        {
            return this.ProcedureSql;
        }

        public virtual OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return new OrderedHashSet<QNameManager.QName>();
        }

        public virtual QNameManager.QName GetSchemaName()
        {
            return this.Name.schema;
        }

        public virtual int GetSchemaObjectType()
        {
            return 8;
        }

        public virtual string GetSql()
        {
            StringBuilder sqlMain = this.GetSqlMain();
            sqlMain.Append("CALL").Append(' ');
            sqlMain.Append(StringConverter.ToQuotedString(this._triggerClassName, '"', false));
            return sqlMain.ToString();
        }

        public StringBuilder GetSqlMain()
        {
            StringBuilder builder = new StringBuilder(0x100);
            builder.Append("CREATE").Append(' ');
            builder.Append("TRIGGER").Append(' ');
            builder.Append(this.Name.GetSchemaQualifiedStatementName()).Append(' ');
            builder.Append(this.GetActionTimingString()).Append(' ');
            builder.Append(this.GetEventTypeString()).Append(' ');
            if (this._updateColumns != null)
            {
                builder.Append("OF").Append(' ');
                for (int i = 0; i < this._updateColumns.Length; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(',');
                    }
                    QNameManager.QName name = this.table.GetColumn(this._updateColumns[i]).GetName();
                    builder.Append(name.StatementName);
                }
                builder.Append(' ');
            }
            builder.Append("ON").Append(' ');
            builder.Append(this.table.GetName().GetSchemaQualifiedStatementName()).Append(' ');
            if (this._hasTransitionRanges || this._hasTransitionTables)
            {
                builder.Append("REFERENCING").Append(' ');
                string str = "";
                if (this.Transitions[0] != null)
                {
                    builder.Append("OLD").Append(' ').Append("ROW");
                    builder.Append(' ').Append("AS").Append(' ');
                    builder.Append(this.Transitions[0].GetName().StatementName);
                    str = " ";
                }
                if (this.Transitions[1] != null)
                {
                    builder.Append(str);
                    builder.Append("NEW").Append(' ').Append("ROW");
                    builder.Append(' ').Append("AS").Append(' ');
                    builder.Append(this.Transitions[1].GetName().StatementName);
                    str = " ";
                }
                if (this.Transitions[2] != null)
                {
                    builder.Append(str);
                    builder.Append("OLD").Append(' ').Append("TABLE");
                    builder.Append(' ').Append("AS").Append(' ');
                    builder.Append(this.Transitions[2].GetName().StatementName);
                    str = " ";
                }
                if (this.Transitions[3] != null)
                {
                    builder.Append(str);
                    builder.Append("OLD").Append(' ').Append("TABLE");
                    builder.Append(' ').Append("AS").Append(' ');
                    builder.Append(this.Transitions[3].GetName().StatementName);
                }
                builder.Append(' ');
            }
            if (this._forEachRow)
            {
                builder.Append("FOR").Append(' ');
                builder.Append("EACH").Append(' ');
                builder.Append("ROW").Append(' ');
            }
            bool flag = true;
            if (!flag.Equals(this.Condition.ValueData))
            {
                builder.Append("WHEN").Append(' ');
                builder.Append("(").Append(this._conditionSql);
                builder.Append(")").Append(' ');
            }
            return builder;
        }

        public int GetStatementType()
        {
            return this.OperationType;
        }

        public Table GetTable()
        {
            return this.table;
        }

        public static int GetTiming(int token)
        {
            if (token == 0x150)
            {
                return 5;
            }
            if (token == 0x157)
            {
                return 4;
            }
            if (token != 0x1a6)
            {
                throw Error.RuntimeError(0xc9, "TriggerDef");
            }
            return 6;
        }

        public int[] GetUpdateColumnIndexes()
        {
            return this._updateColumns;
        }

        public string GetUpdateColumnsSql()
        {
            StringBuilder builder = new StringBuilder();
            if (this._updateColumns != null)
            {
                builder.Append(' ');
                for (int i = 0; i < this._updateColumns.Length; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(',');
                    }
                    QNameManager.QName name = this.table.GetColumn(this._updateColumns[i]).GetName();
                    builder.Append(name.StatementName);
                }
                builder.Append(' ');
            }
            return builder.ToString();
        }

        public virtual bool HasNewTable()
        {
            return false;
        }

        public virtual bool HasOldTable()
        {
            return false;
        }

        public bool IsForEachRow()
        {
            return this._forEachRow;
        }

        public bool IsValid()
        {
            return this.Valid;
        }

        public virtual void PushPair(Session session, object[] row1, object[] row2)
        {
            lock (this)
            {
                if (this._maxRowsQueued == 0)
                {
                    this._trigger.Fire(this.TriggerType, this.Name.Name, this.table.GetName().Name, row1, row2);
                }
            }
        }

        private void SetUpIndexesAndTypes()
        {
            this.TriggerType = 0;
            int operationType = this.OperationType;
            switch (operationType)
            {
                case 0x13:
                    this.TriggerType = 1;
                    break;

                case 50:
                    this.TriggerType = 0;
                    break;

                default:
                    if (operationType != 0x52)
                    {
                        throw Error.RuntimeError(0xc9, "TriggerDef");
                    }
                    this.TriggerType = 2;
                    break;
            }
            if (this._forEachRow)
            {
                this.TriggerType += 3;
            }
            if ((this.ActionTiming == 4) || (this.ActionTiming == 6))
            {
                this.TriggerType += 3;
            }
        }

        public void Terminate()
        {
            lock (this)
            {
                this.KeepGoing = false;
                Monitor.Pulse(this);
            }
        }

        public class DefaultTrigger : ITrigger
        {
            public void Fire(int i, string name, string table, object[] row1, object[] row2)
            {
            }
        }

        public class TriggerData
        {
            public object[] OldRow;
            public object[] NewRow;
            public string Username;
        }
    }
}


namespace FwNs.Core.LC.cEngine
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public sealed class SessionContext
    {
        public Session session;
        public bool IsAutoCommit;
        public bool IsReadOnly;
        public bool NoSql;
        public int CurrentMaxRows;
        public bool AllowLocalTemporaryTables = true;
        public HashMappedList<string, ColumnSchema> SessionVariables;
        public RangeVariable[] SessionVariablesRange;
        public object[] SessionVariableValues = new object[0];
        private Stack<bool> _BoolStack = new Stack<bool>();
        private Stack<object[]> _ObjectArrayStack = new Stack<object[]>();
        private Stack<LongDeque> _LongDequeStack = new Stack<LongDeque>();
        private Stack<HashMappedList<string, int>> _HashMappedListStack = new Stack<HashMappedList<string, int>>();
        private Stack<IRangeIterator[]> _IRangeIteratorArrayStack = new Stack<IRangeIterator[]>();
        private Stack<RestoreTriggerRangeIterators> _DelegateStack = new Stack<RestoreTriggerRangeIterators>();
        private Stack<int> _IntStack = new Stack<int>();
        public object[] RoutineArguments = new object[0];
        public object[] RoutineVariables = new object[0];
        public object[] DynamicArguments = new object[0];
        public int Depth;
        private readonly Stack<Result> _handlerContextStack = new Stack<Result>();
        public object[] triggerOldData;
        public object[] triggerNewData;
        public RestoreTriggerRangeIterators RestoreTriggerIterators;
        public HashMappedList<string, int> Savepoints;
        public LongDeque SavepointTimestamps;
        private Pool<LongDeque> longDequePool = new Pool<LongDeque>();
        private Pool<HashMappedList<string, int>> hashMappedListPool = new Pool<HashMappedList<string, int>>();
        public IRangeIterator[] RangeIterators;
        public Statement CurrentStatement;
        private HashSet<object> _constraintPath;
        public StatementResultUpdate RowUpdateStatement;
        private HashMappedList<string, Table> SessionTables;
        private Stack<HashMappedList<string, Table>> PopSessionTables = new Stack<HashMappedList<string, Table>>();

        public SessionContext(Session session)
        {
            this.session = session;
            this.RangeIterators = new IRangeIterator[4];
            this.Savepoints = new HashMappedList<string, int>(4);
            this.SavepointTimestamps = new LongDeque();
            this.SessionVariables = new HashMappedList<string, ColumnSchema>();
            this.SessionVariablesRange = new RangeVariable[] { new RangeVariable(this.SessionVariables, true) };
            this.IsAutoCommit = this.IsReadOnly = this.NoSql = false;
        }

        public bool ActiveHandlerContextPresent()
        {
            return (this._handlerContextStack.Count > 0);
        }

        public void AddSessionTable(Table table)
        {
            if (this.SessionTables == null)
            {
                this.SessionTables = new HashMappedList<string, Table>();
            }
            if (this.SessionTables.ContainsKey(table.GetName().Name))
            {
                throw Error.GetError(0x1580);
            }
            this.SessionTables.Add(table.GetName().Name, table);
        }

        public void AddSessionVariable(ColumnSchema variable)
        {
            int index = this.SessionVariables.Size();
            if (!this.SessionVariables.Add(variable.GetName().Name, variable))
            {
                throw Error.GetError(0x1580);
            }
            object[] destinationArray = new object[this.SessionVariables.Size()];
            Array.Copy(this.RoutineVariables, destinationArray, this.RoutineVariables.Length);
            this.RoutineVariables = destinationArray;
            this.RoutineVariables[index] = variable.GetDefaultValue(this.session);
        }

        public void ClearStructures(StatementDMQL cs)
        {
            int rangeIteratorCount = cs.RangeIteratorCount;
            if (rangeIteratorCount > this.RangeIterators.Length)
            {
                rangeIteratorCount = this.RangeIterators.Length;
            }
            for (int i = 0; i < rangeIteratorCount; i++)
            {
                if (this.RangeIterators[i] != null)
                {
                    this.RangeIterators[i].Reset();
                    this.RangeIterators[i] = null;
                }
            }
            if (this.RestoreTriggerIterators != null)
            {
                this.RestoreTriggerIterators(this.session, this.triggerOldData, this.triggerNewData);
            }
        }

        public void DropSessionTable(string name)
        {
            if (this.SessionTables != null)
            {
                this.SessionTables.Remove(name);
            }
        }

        public Table FindSessionTable(string name)
        {
            Table dynamicTable = null;
            if (this.SessionTables != null)
            {
                dynamicTable = this.SessionTables.Get(name);
            }
            if ((dynamicTable == null) && (this.Depth == 0))
            {
                dynamicTable = this.GetDynamicTable(name);
            }
            return dynamicTable;
        }

        public RangeVariable.RangeIteratorBase GetCheckIterator(RangeVariable rangeVariable)
        {
            IRangeIterator iterator = this.RangeIterators[1];
            if (iterator == null)
            {
                iterator = rangeVariable.GetIterator(this.session);
                this.RangeIterators[1] = iterator;
            }
            return (RangeVariable.RangeIteratorBase) iterator;
        }

        public HashSet<object> GetConstraintPath()
        {
            if (this._constraintPath == null)
            {
                this._constraintPath = new HashSet<object>();
            }
            else
            {
                this._constraintPath.Clear();
            }
            return this._constraintPath;
        }

        public Table GetDynamicTable(string name)
        {
            if (this.DynamicArguments != null)
            {
                for (int i = 0; i < this.DynamicArguments.Length; i++)
                {
                    Table table2 = this.DynamicArguments[i] as Table;
                    if ((table2 != null) && (table2.GetName().Name == name))
                    {
                        return table2;
                    }
                }
            }
            return null;
        }

        public Result PeekHandlerContext()
        {
            return this._handlerContextStack.Peek();
        }

        public void Pop(bool isInvoke)
        {
            if (isInvoke)
            {
                this.session.sessionData.persistentStoreCollection.Pop();
            }
            this.longDequePool.Store(this.SavepointTimestamps);
            this.hashMappedListPool.Store(this.Savepoints);
            this.triggerOldData = this._ObjectArrayStack.Pop();
            this.triggerNewData = this._ObjectArrayStack.Pop();
            this.RestoreTriggerIterators = this._DelegateStack.Pop();
            this.CurrentMaxRows = this._IntStack.Pop();
            this.NoSql = this._BoolStack.Pop();
            this.IsReadOnly = this._BoolStack.Pop();
            this.IsAutoCommit = this._BoolStack.Pop();
            this.SavepointTimestamps = this._LongDequeStack.Pop();
            this.Savepoints = this._HashMappedListStack.Pop();
            this.RangeIterators = this._IRangeIteratorArrayStack.Pop();
            this.RoutineVariables = this._ObjectArrayStack.Pop();
            this.RoutineArguments = this._ObjectArrayStack.Pop();
            this.DynamicArguments = this._ObjectArrayStack.Pop();
            this.Depth--;
        }

        public Result PopHandlerContext()
        {
            return this._handlerContextStack.Pop();
        }

        public void PopRoutineTables()
        {
            this.SessionTables = this.PopSessionTables.Pop();
        }

        public void Push(bool isInvoke)
        {
            if (isInvoke)
            {
                this.session.sessionData.persistentStoreCollection.Push();
            }
            this._ObjectArrayStack.Push(this.DynamicArguments);
            this._ObjectArrayStack.Push(this.RoutineArguments);
            this._ObjectArrayStack.Push(this.RoutineVariables);
            this._IRangeIteratorArrayStack.Push(this.RangeIterators);
            this._HashMappedListStack.Push(this.Savepoints);
            this._LongDequeStack.Push(this.SavepointTimestamps);
            this._BoolStack.Push(this.IsAutoCommit);
            this._BoolStack.Push(this.IsReadOnly);
            this._BoolStack.Push(this.NoSql);
            this._IntStack.Push(this.CurrentMaxRows);
            this._DelegateStack.Push(this.RestoreTriggerIterators);
            this._ObjectArrayStack.Push(this.triggerNewData);
            this._ObjectArrayStack.Push(this.triggerOldData);
            this.RangeIterators = new IRangeIterator[4];
            this.SavepointTimestamps = this.longDequePool.Fetch();
            this.SavepointTimestamps.Clear();
            this.Savepoints = this.hashMappedListPool.Fetch();
            this.Savepoints.Clear();
            this.IsAutoCommit = false;
            this.CurrentMaxRows = 0;
            this.Depth++;
        }

        public void PushDynamicArguments(object[] args)
        {
            this.Push(false);
            this.DynamicArguments = args;
        }

        public void PushHandlerContext(Result r)
        {
            this._handlerContextStack.Push(r);
        }

        public void PushRoutineTables(HashMappedList<string, Table> map)
        {
            this.PopSessionTables.Push(this.SessionTables);
            this.SessionTables = map;
        }

        public void SetDynamicArguments(object[] args)
        {
            this.DynamicArguments = args;
        }

        public void SetOrAddSessionVariable(ColumnSchema variable, object value)
        {
            int index = this.SessionVariables.Size();
            if (this.SessionVariables.ContainsKey(variable.GetName().Name))
            {
                index = this.SessionVariables.GetIndex(variable.GetName().Name);
            }
            else
            {
                this.SessionVariables.Add(variable.GetName().Name, variable);
                object[] destinationArray = new object[this.SessionVariables.Size()];
                Array.Copy(this.SessionVariableValues, destinationArray, this.SessionVariableValues.Length);
                this.SessionVariableValues = destinationArray;
            }
            this.SessionVariableValues[index] = value;
        }

        public void SetRangeIterator(IRangeIterator iterator)
        {
            int rangePosition = iterator.GetRangePosition();
            if (rangePosition >= this.RangeIterators.Length)
            {
                this.RangeIterators = ArrayUtil.ResizeArray<IRangeIterator>(this.RangeIterators, rangePosition + 1);
            }
            this.RangeIterators[iterator.GetRangePosition()] = iterator;
        }

        public void SetSessionTables(Table[] tables)
        {
        }

        public void SetVariable(ColumnSchema variable, object value)
        {
            int index = this.SessionVariables.GetIndex(variable.GetName().Name);
            if (index == -1)
            {
                throw Error.GetError(0x157d, variable.GetName().Name);
            }
            this.RoutineVariables[index] = value;
        }

        public delegate void RestoreTriggerRangeIterators(Session session, object[] oldData, object[] newData);
    }
}


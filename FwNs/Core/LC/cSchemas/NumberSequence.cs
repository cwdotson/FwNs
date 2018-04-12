namespace FwNs.Core.LC.cSchemas
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Text;

    public sealed class NumberSequence : ISchemaObject
    {
        public static NumberSequence[] EmptyArray = new NumberSequence[0];
        private long _currValue;
        private SqlType _dataType;
        private long _increment;
        private bool _isAlways;
        private bool _isCycle;
        private long _lastValue;
        private bool _limitReached;
        private long _maxValue;
        private long _minValue;
        private QNameManager.QName _name;
        private bool _restartValueDefault;
        private long _startValue;

        public NumberSequence()
        {
            try
            {
                this.SetDefaults(null, SqlType.SqlBigint);
            }
            catch (CoreException)
            {
            }
        }

        public NumberSequence(QNameManager.QName name, SqlType type)
        {
            this.SetDefaults(name, type);
        }

        public NumberSequence(QNameManager.QName name, long value, long increment, SqlType type) : this(name, type)
        {
            this.SetStartValue(value);
            this.SetIncrement(increment);
        }

        private void CheckInTypeRange(long value)
        {
            long num2;
            long num3;
            int typeCode = this._dataType.TypeCode;
            switch (typeCode)
            {
                case 2:
                case 3:
                    num2 = 0x7fffffffffffffffL;
                    num3 = -9223372036854775808L;
                    break;

                case 4:
                    num2 = 0x7fffffffL;
                    num3 = -2147483648L;
                    break;

                case 5:
                    num2 = 0x7fffL;
                    num3 = -32768L;
                    break;

                case -6:
                    num2 = 0xffL;
                    num3 = 0L;
                    break;

                default:
                    if (typeCode != 0x19)
                    {
                        throw Error.RuntimeError(0xc9, "NumberSequence");
                    }
                    num2 = 0x7fffffffffffffffL;
                    num3 = -9223372036854775808L;
                    break;
            }
            if ((value < num3) || (value > num2))
            {
                throw Error.GetError(0x15dd);
            }
        }

        public void CheckValues()
        {
            lock (this)
            {
                if (this._restartValueDefault)
                {
                    this._currValue = this._lastValue = this._startValue;
                    this._restartValueDefault = false;
                }
                if (((this._minValue >= this._maxValue) || (this._startValue < this._minValue)) || (((this._startValue > this._maxValue) || (this._currValue < this._minValue)) || (this._currValue > this._maxValue)))
                {
                    throw Error.GetError(0x15dd);
                }
            }
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public NumberSequence Duplicate()
        {
            lock (this)
            {
                return new NumberSequence { 
                    _name = this._name,
                    _startValue = this._startValue,
                    _currValue = this._currValue,
                    _lastValue = this._lastValue,
                    _increment = this._increment,
                    _dataType = this._dataType,
                    _minValue = this._minValue,
                    _maxValue = this._maxValue,
                    _isCycle = this._isCycle,
                    _isAlways = this._isAlways
                };
            }
        }

        public QNameManager.QName GetCatalogName()
        {
            return this._name.schema.schema;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public SqlType GetDataType()
        {
            return this._dataType;
        }

        private long GetDefaultMinOrMax(bool isMax)
        {
            long num2;
            long num3;
            int typeCode = this._dataType.TypeCode;
            switch (typeCode)
            {
                case 2:
                case 3:
                    num2 = 0x7fffffffffffffffL;
                    num3 = -9223372036854775808L;
                    break;

                case 4:
                    num2 = 0x7fffffffL;
                    num3 = -2147483648L;
                    break;

                case 5:
                    num2 = 0x7fffL;
                    num3 = -32768L;
                    break;

                case -6:
                    num2 = 0xffL;
                    num3 = 0L;
                    break;

                default:
                    if (typeCode != 0x19)
                    {
                        throw Error.RuntimeError(0xc9, "NumberSequence");
                    }
                    num2 = 0x7fffffffffffffffL;
                    num3 = -9223372036854775808L;
                    break;
            }
            if (!isMax)
            {
                return num3;
            }
            return num2;
        }

        public long GetIncrement()
        {
            return this._increment;
        }

        public long GetMaxValue()
        {
            lock (this)
            {
                return this._maxValue;
            }
        }

        public long GetMinValue()
        {
            lock (this)
            {
                return this._minValue;
            }
        }

        public QNameManager.QName GetName()
        {
            return this._name;
        }

        public Grantee GetOwner()
        {
            return this._name.schema.Owner;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return new OrderedHashSet<QNameManager.QName>();
        }

        public string GetRestartSql()
        {
            StringBuilder builder1 = new StringBuilder(0x80);
            builder1.Append("ALTER").Append(' ');
            builder1.Append("SEQUENCE");
            builder1.Append(' ').Append(this._name.GetSchemaQualifiedStatementName());
            builder1.Append(' ').Append("RESTART");
            builder1.Append(' ').Append("WITH").Append(' ').Append(this.Peek());
            return builder1.ToString();
        }

        public static string GetRestartSql(Table t)
        {
            string statementName = t.GetColumn(t.IdentityColumn).GetName().StatementName;
            NumberSequence identitySequence = t.IdentitySequence;
            StringBuilder builder1 = new StringBuilder(0x80);
            builder1.Append("ALTER").Append(' ').Append("TABLE");
            builder1.Append(' ').Append(t.GetName().GetSchemaQualifiedStatementName());
            builder1.Append(' ').Append("ALTER").Append(' ');
            builder1.Append("COLUMN");
            builder1.Append(' ').Append(statementName);
            builder1.Append(' ').Append("RESTART");
            builder1.Append(' ').Append("WITH").Append(' ').Append(identitySequence.Peek());
            return builder1.ToString();
        }

        public QNameManager.QName GetSchemaName()
        {
            return this._name.schema;
        }

        public int GetSchemaObjectType()
        {
            return 7;
        }

        public string GetSql()
        {
            StringBuilder builder = new StringBuilder(0x80);
            if (this._name == null)
            {
                builder.Append("GENERATED").Append(' ');
                if (this.IsAlways())
                {
                    builder.Append("ALWAYS");
                }
                else
                {
                    builder.Append("BY").Append(' ').Append("DEFAULT");
                }
                builder.Append(' ').Append("AS").Append(' ').Append("IDENTITY").Append("(");
            }
            else
            {
                builder.Append("CREATE").Append(' ');
                builder.Append("SEQUENCE").Append(' ');
                builder.Append(this.GetName().GetSchemaQualifiedStatementName()).Append(' ');
                builder.Append("AS").Append(' ');
                builder.Append(this.GetDataType().GetNameString()).Append(' ');
            }
            builder.Append("START").Append(' ');
            builder.Append("WITH").Append(' ');
            builder.Append(this._startValue);
            if (this.GetIncrement() != 1L)
            {
                builder.Append(' ').Append("INCREMENT").Append(' ');
                builder.Append("BY").Append(' ');
                builder.Append(this.GetIncrement());
            }
            if (!this.HasDefaultMinMax())
            {
                builder.Append(' ').Append("MINVALUE").Append(' ');
                builder.Append(this.GetMinValue());
                builder.Append(' ').Append("MAXVALUE").Append(' ');
                builder.Append(this.GetMaxValue());
            }
            if (this.IsCycle())
            {
                builder.Append(' ').Append("CYCLE");
            }
            if (this._name == null)
            {
                builder.Append(")");
            }
            return builder.ToString();
        }

        public long GetStartValue()
        {
            lock (this)
            {
                return this._startValue;
            }
        }

        public long GetValue()
        {
            lock (this)
            {
                long num2;
                if (this._limitReached)
                {
                    throw Error.GetError(0xd58);
                }
                if (this._increment > 0L)
                {
                    if (this._currValue > (this._maxValue - this._increment))
                    {
                        if (this._isCycle)
                        {
                            num2 = this._minValue;
                        }
                        else
                        {
                            this._limitReached = true;
                            num2 = this._minValue;
                        }
                    }
                    else
                    {
                        num2 = this._currValue + this._increment;
                    }
                }
                else if (this._currValue < (this._minValue - this._increment))
                {
                    if (this._isCycle)
                    {
                        num2 = this._maxValue;
                    }
                    else
                    {
                        this._limitReached = true;
                        num2 = this._minValue;
                    }
                }
                else
                {
                    num2 = this._currValue + this._increment;
                }
                this._currValue = num2;
                return this._currValue;
            }
        }

        public object GetValueObject()
        {
            lock (this)
            {
                object obj3;
                long num = this.GetValue();
                int typeCode = this._dataType.TypeCode;
                if ((typeCode - 2) <= 1)
                {
                    obj3 = num;
                }
                else if (typeCode != 0x19)
                {
                    obj3 = (int) num;
                }
                else
                {
                    obj3 = num;
                }
                return obj3;
            }
        }

        public bool HasDefaultMinMax()
        {
            lock (this)
            {
                long num2;
                long num3;
                int typeCode = this._dataType.TypeCode;
                switch (typeCode)
                {
                    case 2:
                    case 3:
                        num2 = 0x7fffffffffffffffL;
                        num3 = -9223372036854775808L;
                        break;

                    case 4:
                        num2 = 0x7fffffffL;
                        num3 = -2147483648L;
                        break;

                    case 5:
                        num2 = 0x7fffL;
                        num3 = -32768L;
                        break;

                    case -6:
                        num2 = 0xffL;
                        num3 = 0L;
                        break;

                    default:
                        if (typeCode != 0x19)
                        {
                            throw Error.RuntimeError(0xc9, "NumberSequence");
                        }
                        num2 = 0x7fffffffffffffffL;
                        num3 = -9223372036854775808L;
                        break;
                }
                return ((this._minValue == num3) && (this._maxValue == num2));
            }
        }

        public bool IsAlways()
        {
            lock (this)
            {
                return this._isAlways;
            }
        }

        public bool IsCycle()
        {
            lock (this)
            {
                return this._isCycle;
            }
        }

        public long Peek()
        {
            lock (this)
            {
                return this._currValue;
            }
        }

        public void Reset()
        {
            lock (this)
            {
                this._lastValue = this._currValue = this._startValue;
            }
        }

        public void Reset(NumberSequence other)
        {
            lock (this)
            {
                this._name = other._name;
                this._startValue = other._startValue;
                this._currValue = other._currValue;
                this._lastValue = other._lastValue;
                this._increment = other._increment;
                this._dataType = other._dataType;
                this._minValue = other._minValue;
                this._maxValue = other._maxValue;
                this._isCycle = other._isCycle;
                this._isAlways = other._isAlways;
            }
        }

        public void SetAlways(bool value)
        {
            lock (this)
            {
                this._isAlways = value;
            }
        }

        public void SetCurrentValueNoCheck(long value)
        {
            lock (this)
            {
                this.CheckInTypeRange(value);
                this._lastValue = value;
                this._currValue = value;
            }
        }

        public void SetCycle(bool value)
        {
            lock (this)
            {
                this._isCycle = value;
            }
        }

        public void SetDefaultMaxValue()
        {
            lock (this)
            {
                this._maxValue = this.GetDefaultMinOrMax(true);
            }
        }

        public void SetDefaultMinValue()
        {
            lock (this)
            {
                this._minValue = this.GetDefaultMinOrMax(false);
            }
        }

        public void SetDefaults(QNameManager.QName name, SqlType type)
        {
            long num2;
            long num3;
            this._name = name;
            this._dataType = type;
            switch (this._dataType.TypeCode)
            {
                case 2:
                case 3:
                    if (type.Scale != 0)
                    {
                        break;
                    }
                    num2 = 0x7fffffffffffffffL;
                    num3 = -9223372036854775808L;
                    goto Label_00A7;

                case 4:
                    num2 = 0x7fffffffL;
                    num3 = -2147483648L;
                    goto Label_00A7;

                case 5:
                    num2 = 0x7fffL;
                    num3 = -32768L;
                    goto Label_00A7;

                case -6:
                    num2 = 0xffL;
                    num3 = 0L;
                    goto Label_00A7;

                case 0x19:
                    num2 = 0x7fffffffffffffffL;
                    num3 = -9223372036854775808L;
                    goto Label_00A7;
            }
            throw Error.GetError(0x15bb);
        Label_00A7:
            this._minValue = num3;
            this._maxValue = num2;
            this._increment = 1L;
        }

        public void SetIncrement(long value)
        {
            lock (this)
            {
                if ((value < -16384L) || (value > 0x3fffL))
                {
                    throw Error.GetError(0x15dd);
                }
                this._increment = value;
            }
        }

        public void SetMaxValueNoCheck(long value)
        {
            lock (this)
            {
                this.CheckInTypeRange(value);
                this._maxValue = value;
            }
        }

        public void SetMinValueNoCheck(long value)
        {
            lock (this)
            {
                this.CheckInTypeRange(value);
                this._minValue = value;
            }
        }

        public void SetStartValue(long value)
        {
            lock (this)
            {
                if ((value < this._minValue) || (value > this._maxValue))
                {
                    throw Error.GetError(0x15dd);
                }
                this._startValue = value;
                this._currValue = this._lastValue = this._startValue;
            }
        }

        public void SetStartValueDefault()
        {
            lock (this)
            {
                this._restartValueDefault = true;
            }
        }

        public void SetStartValueNoCheck(long value)
        {
            lock (this)
            {
                this.CheckInTypeRange(value);
                this._startValue = value;
                this._currValue = this._lastValue = this._startValue;
            }
        }

        public long SystemUpdate(long value)
        {
            lock (this)
            {
                if (value == this._currValue)
                {
                    this._currValue += this._increment;
                    return value;
                }
                if (this._increment > 0L)
                {
                    if (value > this._currValue)
                    {
                        this._currValue = value + this._increment;
                    }
                }
                else if (value < this._currValue)
                {
                    this._currValue = value + this._increment;
                }
                return value;
            }
        }

        public long UserUpdate(long value)
        {
            lock (this)
            {
                if (value == this._currValue)
                {
                    this._currValue += this._increment;
                    return value;
                }
                if (this._increment > 0L)
                {
                    if (value > this._currValue)
                    {
                        this._currValue += (((value - this._currValue) + this._increment) / this._increment) * this._increment;
                    }
                }
                else if (value < this._currValue)
                {
                    this._currValue += (((value - this._currValue) + this._increment) / this._increment) * this._increment;
                }
                return value;
            }
        }
    }
}


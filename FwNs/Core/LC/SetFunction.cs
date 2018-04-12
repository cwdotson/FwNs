namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Collections.Generic;

    public sealed class SetFunction
    {
        private readonly HashSet<object> _distinctValues;
        private readonly bool _isDistinct;
        private readonly int _setType;
        private readonly int _dataType;
        private readonly SqlType _type;
        private int _count;
        private bool _hasNull;
        private bool _every = true;
        private bool _some;
        private long _currentLong;
        private double _currentDouble;
        private decimal _currentBigDecimal;
        private object _currentValue;
        private double _sk;
        private double _vk;
        private long _n;
        private bool _initialized;
        private readonly bool _sample;

        public SetFunction(int setType, SqlType type, bool isDistinct)
        {
            this._setType = setType;
            this._type = type;
            if (isDistinct)
            {
                this._isDistinct = true;
                this._distinctValues = new HashSet<object>();
            }
            if ((setType == 0x51) || (setType == 0x4f))
            {
                this._sample = true;
            }
            if (type != null)
            {
                this._dataType = type.TypeCode;
                if (type.IsIntervalType())
                {
                    this._dataType = 10;
                }
            }
        }

        public void Add(Session session, object item)
        {
            if (item == null)
            {
                this._hasNull = true;
            }
            else if (!this._isDistinct || this._distinctValues.Add(item))
            {
                this._count++;
                switch (this._setType)
                {
                    case 0x47:
                        return;

                    case 0x48:
                    case 0x4b:
                        switch (this._dataType)
                        {
                            case -6:
                            case 4:
                            case 5:
                                int num3;
                                try
                                {
                                    num3 = (int) item;
                                }
                                catch (Exception)
                                {
                                    num3 = Convert.ToInt32(item);
                                }
                                this._currentLong += num3;
                                return;

                            case 2:
                            case 3:
                                decimal num4;
                                try
                                {
                                    num4 = (decimal) item;
                                }
                                catch (Exception)
                                {
                                    num4 = Convert.ToDecimal(item);
                                }
                                this._currentBigDecimal += num4;
                                return;

                            case 6:
                            case 7:
                            case 8:
                                double num5;
                                try
                                {
                                    num5 = (double) item;
                                }
                                catch (Exception)
                                {
                                    num5 = Convert.ToDouble(item);
                                }
                                this._currentDouble += num5;
                                return;

                            case 10:
                            {
                                IntervalSecondData data = item as IntervalSecondData;
                                if (data != null)
                                {
                                    this._currentLong += data.Units;
                                    this._currentLong += data.Nanos;
                                    if (Math.Abs(this._currentLong) >= DTIType.NanoScaleFactors[0])
                                    {
                                        this._currentLong += this._currentLong / ((long) DTIType.NanoScaleFactors[0]);
                                        this._currentLong = this._currentLong % ((long) DTIType.NanoScaleFactors[0]);
                                        return;
                                    }
                                }
                                else
                                {
                                    IntervalMonthData data2 = item as IntervalMonthData;
                                    if (data2 != null)
                                    {
                                        this._currentLong += data2.Units;
                                    }
                                }
                                return;
                            }
                            case 0x19:
                                long num6;
                                try
                                {
                                    num6 = (long) item;
                                }
                                catch (Exception)
                                {
                                    num6 = Convert.ToInt64(item);
                                }
                                this._currentLong += num6;
                                return;

                            case 0x68:
                                this._currentValue = item;
                                return;
                        }
                        break;

                    case 0x49:
                        if (this._currentValue != null)
                        {
                            if (this._type.Compare(session, this._currentValue, item, this._type, false) > 0)
                            {
                                this._currentValue = item;
                            }
                            return;
                        }
                        this._currentValue = item;
                        return;

                    case 0x4a:
                        if (this._currentValue != null)
                        {
                            if (this._type.Compare(session, this._currentValue, item, this._type, false) < 0)
                            {
                                this._currentValue = item;
                            }
                            return;
                        }
                        this._currentValue = item;
                        return;

                    case 0x4c:
                        if (!(item is bool))
                        {
                            throw Error.GetError(0x15bd);
                        }
                        this._every = this._every && ((bool) item);
                        return;

                    case 0x4d:
                        if (!(item is bool))
                        {
                            throw Error.GetError(0x15bd);
                        }
                        this._some = this._some || ((bool) item);
                        return;

                    case 0x4e:
                    case 0x4f:
                    case 80:
                    case 0x51:
                        this.AddDataPoint(item);
                        return;

                    default:
                        throw Error.RuntimeError(0xc9, "SetFunction");
                }
                throw Error.GetError(0x15bd);
            }
        }

        private void AddDataPoint(object x)
        {
            if (x != null)
            {
                double num;
                try
                {
                    num = (double) x;
                }
                catch (Exception)
                {
                    num = Convert.ToDouble(x);
                }
                if (!this._initialized)
                {
                    this._n = 1L;
                    this._sk = num;
                    this._vk = 0.0;
                    this._initialized = true;
                }
                else
                {
                    this._n += 1L;
                    long num2 = this._n - 1L;
                    double num3 = this._sk - (num * num2);
                    this._vk += ((num3 * num3) / ((double) this._n)) / ((double) num2);
                    this._sk += num;
                }
            }
        }

        private object GetStdDev()
        {
            if (this._initialized)
            {
                if (!this._sample)
                {
                    return Math.Sqrt(this._vk / ((double) this._n));
                }
                if (this._n != 1L)
                {
                    return Math.Sqrt(this._vk / ((double) (this._n - 1L)));
                }
            }
            return null;
        }

        public static SqlType GetType(int setType, SqlType type)
        {
            if (setType == 0x47)
            {
                return SqlType.SqlInteger;
            }
            int num = type.IsIntervalType() ? 10 : type.TypeCode;
            switch (setType)
            {
                case 0x48:
                {
                    int num2 = num;
                    switch (num2)
                    {
                        case -5:
                        case -4:
                        case -3:
                        case -2:
                        case -1:
                        case 0:
                        case 1:
                        case 9:
                            goto Label_00E4;

                        case 2:
                        case 3:
                            return SqlType.GetDataType(type.TypeCode, 0, type.Precision * 2L, type.Scale);

                        case 6:
                        case 7:
                        case 8:
                            return SqlType.SqlDouble;

                        case 10:
                            return IntervalType.NewIntervalType(type.TypeCode, 9L, type.Scale);
                    }
                    if (num2 != 0x19)
                    {
                        goto Label_00E4;
                    }
                    break;
                }
                case 0x49:
                case 0x4a:
                    if (type.IsArrayType() || type.IsLobType())
                    {
                        throw Error.GetError(0x15bd);
                    }
                    return type;

                case 0x4b:
                {
                    int num3 = num;
                    switch (num3)
                    {
                        case -6:
                        case 4:
                        case 5:
                            goto Label_0164;

                        case -5:
                        case -4:
                        case -3:
                        case -2:
                        case -1:
                        case 0:
                        case 1:
                        case 9:
                            goto Label_016A;

                        case 2:
                        case 3:
                        case 6:
                        case 7:
                        case 8:
                        case 10:
                            return type;
                    }
                    if (num3 != 0x19)
                    {
                        goto Label_016A;
                    }
                    goto Label_0164;
                }
                case 0x4c:
                case 0x4d:
                    if (!type.IsBooleanType())
                    {
                        goto Label_01A8;
                    }
                    return SqlType.SqlBoolean;

                case 0x4e:
                case 0x4f:
                case 80:
                case 0x51:
                    if (!type.IsNumberType())
                    {
                        goto Label_01A8;
                    }
                    return SqlType.SqlDouble;

                default:
                    if (setType != 0x68)
                    {
                        throw Error.RuntimeError(0xc9, "SetFunction");
                    }
                    return type;
            }
            return SqlType.SqlBigint;
        Label_00E4:
            throw Error.GetError(0x15bd);
        Label_0164:
            return SqlType.SqlDecimal;
        Label_016A:
            throw Error.GetError(0x15bd);
        Label_01A8:
            throw Error.GetError(0x15bd);
        }

        public object GetValue(Session session)
        {
            if (this._hasNull)
            {
                session.AddWarning(Error.GetError(0x3eb));
            }
            if (this._setType == 0x47)
            {
                return this._count;
            }
            if (this._count == 0)
            {
                return null;
            }
            int num = this._setType;
            switch (num)
            {
                case 0x48:
                {
                    int num2 = this._dataType;
                    switch (num2)
                    {
                        case -5:
                        case -4:
                        case -3:
                        case -2:
                        case -1:
                        case 0:
                        case 1:
                        case 9:
                            goto Label_0162;

                        case 2:
                        case 3:
                            return this._currentBigDecimal;

                        case 6:
                        case 7:
                        case 8:
                            return this._currentDouble;

                        case 10:
                        {
                            long result = this._currentLong;
                            if (!NumberType.IsInLongLimits(result))
                            {
                                throw Error.GetError(0xd6b);
                            }
                            if (((IntervalType) this._type).IsDaySecondIntervalType())
                            {
                                return new IntervalSecondData(result, this._currentLong, (IntervalType) this._type, true);
                            }
                            return IntervalMonthData.NewIntervalMonth(result, (IntervalType) this._type);
                        }
                    }
                    if (num2 != 0x19)
                    {
                        goto Label_0162;
                    }
                    break;
                }
                case 0x49:
                case 0x4a:
                    return this._currentValue;

                case 0x4b:
                {
                    int num4 = this._dataType;
                    switch (num4)
                    {
                        case -6:
                        case 4:
                        case 5:
                            goto Label_027A;

                        case -5:
                        case -4:
                        case -3:
                        case -2:
                        case -1:
                        case 0:
                        case 1:
                        case 9:
                            goto Label_0290;

                        case 2:
                        case 3:
                            return (this._currentBigDecimal / this._count);

                        case 6:
                        case 7:
                        case 8:
                            return (this._currentDouble / ((double) this._count));

                        case 10:
                        {
                            long result = this._currentLong / ((long) this._count);
                            if (!NumberType.IsInLongLimits(result))
                            {
                                throw Error.GetError(0xd6b);
                            }
                            if (((IntervalType) this._type).IsDaySecondIntervalType())
                            {
                                return new IntervalSecondData(result, this._currentLong, (IntervalType) this._type, true);
                            }
                            return IntervalMonthData.NewIntervalMonth(result, (IntervalType) this._type);
                        }
                    }
                    if (num4 != 0x19)
                    {
                        goto Label_0290;
                    }
                    goto Label_027A;
                }
                case 0x4c:
                    return this._every;

                case 0x4d:
                    return this._some;

                case 0x4e:
                case 0x4f:
                    return this.GetStdDev();

                case 80:
                case 0x51:
                    return this.GetVariance();

                default:
                    if (num != 0x68)
                    {
                        throw Error.RuntimeError(0xc9, "SetFunction");
                    }
                    return this._currentValue;
            }
            return this._currentLong;
        Label_0162:
            throw Error.RuntimeError(0xc9, "SetFunction");
        Label_027A:
            return (this._currentLong / ((long) this._count));
        Label_0290:
            throw Error.RuntimeError(0xc9, "SetFunction");
        }

        private object GetVariance()
        {
            if (this._initialized)
            {
                if (!this._sample)
                {
                    return (this._vk / ((double) this._n));
                }
                if (this._n != 1L)
                {
                    return (this._vk / ((double) (this._n - 1L)));
                }
            }
            return null;
        }
    }
}


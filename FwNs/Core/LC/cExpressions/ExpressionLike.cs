namespace FwNs.Core.LC.cExpressions
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class ExpressionLike : ExpressionLogical
    {
        private const int Escape = 2;
        private const int Ternary = 3;
        private Like _likeObject;

        public ExpressionLike(Expression left, Expression right, Expression escape, bool noOptimisation) : base(0x35)
        {
            base.nodes = new Expression[] { left, right, escape };
            this._likeObject = new Like();
            base.NoOptimisation = noOptimisation;
        }

        public override string Describe(Session session, int blanks)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('\n');
            for (int i = 0; i < blanks; i++)
            {
                builder.Append(' ');
            }
            builder.Append("LIKE ");
            builder.Append(this._likeObject.Describe(session));
            return builder.ToString();
        }

        public override string GetSql()
        {
            if (this._likeObject == null)
            {
                return base.GetSql();
            }
            string contextSql = Expression.GetContextSql(base.nodes[0]);
            string str3 = Expression.GetContextSql(base.nodes[1]);
            StringBuilder builder = new StringBuilder();
            builder.Append(contextSql).Append(' ').Append("LIKE").Append(' ');
            builder.Append(str3);
            if (base.nodes[2] != null)
            {
                builder.Append(' ').Append("ESCAPE").Append(' ');
                builder.Append(base.nodes[2].GetSql());
                builder.Append(' ');
            }
            return builder.ToString();
        }

        public override object GetValue(Session session)
        {
            if (base.OpType != 0x35)
            {
                return base.GetValue(session);
            }
            object o = base.nodes[0].GetValue(session);
            if (this._likeObject.IsVariable)
            {
                object rightValue = base.nodes[1].GetValue(session);
                object valueCore = this.GetValueCore(session, ref rightValue);
                lock (this._likeObject)
                {
                    this._likeObject.SetPattern(session, rightValue, valueCore, base.nodes[2] > null);
                    return this._likeObject.Compare(session, o);
                }
            }
            return this._likeObject.Compare(session, o);
        }

        private object GetValueCore(Session session, ref object rightValue)
        {
            if (base.nodes[1].DataType.IsCharacterType())
            {
                if (base.nodes[1].DataType.IsLobType())
                {
                    rightValue = SqlType.SqlVarcharDefault.ConvertToType(session, rightValue, base.nodes[1].DataType);
                }
            }
            else if (base.nodes[1].DataType.IsLobType())
            {
                rightValue = SqlType.SqlVarbinaryDefault.ConvertToType(session, rightValue, base.nodes[1].DataType);
            }
            object a = null;
            if (base.nodes[2] != null)
            {
                a = base.nodes[2].GetValue(session);
                if (base.nodes[2].DataType.IsCharacterType())
                {
                    if (base.nodes[2].DataType.IsLobType())
                    {
                        a = SqlType.SqlVarcharDefault.ConvertToType(session, a, base.nodes[2].DataType);
                    }
                    return a;
                }
                if (base.nodes[2].DataType.IsLobType())
                {
                    a = SqlType.SqlVarbinaryDefault.ConvertToType(session, a, base.nodes[2].DataType);
                }
            }
            return a;
        }

        public override List<Expression> ResolveColumnReferences(RangeVariable[] rangeVarArray, int rangeCount, List<Expression> unresolvedSet, bool acceptsSequences)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    unresolvedSet = base.nodes[i].ResolveColumnReferences(rangeVarArray, rangeCount, unresolvedSet, acceptsSequences);
                }
            }
            return unresolvedSet;
        }

        public override void ResolveTypes(Session session, Expression parent)
        {
            for (int i = 0; i < base.nodes.Length; i++)
            {
                if (base.nodes[i] != null)
                {
                    base.nodes[i].ResolveTypes(session, this);
                }
            }
            bool flag = true;
            if (base.nodes[2] != null)
            {
                if (base.nodes[2].IsUnresolvedParam())
                {
                    throw Error.GetError(0x15bf);
                }
                base.nodes[2].ResolveTypes(session, this);
                flag = base.nodes[2].OpType == 1;
                if (flag)
                {
                    base.nodes[2].SetAsConstantValue(session);
                    if (base.nodes[2].DataType == null)
                    {
                        throw Error.GetError(0x15bf);
                    }
                    if (base.nodes[2].ValueData != null)
                    {
                        long length;
                        int typeCode = base.nodes[2].DataType.TypeCode;
                        switch (typeCode)
                        {
                            case 1:
                            case 12:
                                length = ((string) base.nodes[2].ValueData).Length;
                                break;

                            default:
                                if ((typeCode - 60) <= 1)
                                {
                                    length = ((BinaryData) base.nodes[2].ValueData).Length(session);
                                    break;
                                }
                                throw Error.GetError(0x15bd);
                        }
                        if (length != 1L)
                        {
                            throw Error.GetError(0xd6f);
                        }
                    }
                }
            }
            if ((base.nodes[0].DataType == null) && (base.nodes[1].DataType == null))
            {
                throw Error.GetError(0x15bf);
            }
            if (base.nodes[0].IsUnresolvedParam())
            {
                base.nodes[0].DataType = base.nodes[1].DataType.IsBinaryType() ? ((SqlType) SqlType.SqlVarbinaryDefault) : ((SqlType) SqlType.SqlVarcharDefault);
            }
            else if (base.nodes[1].IsUnresolvedParam())
            {
                base.nodes[1].DataType = base.nodes[0].DataType.IsBinaryType() ? ((SqlType) SqlType.SqlVarbinaryDefault) : ((SqlType) SqlType.SqlVarcharDefault);
            }
            if ((base.nodes[0].DataType.IsCharacterType() && base.nodes[1].DataType.IsCharacterType()) && ((base.nodes[2] == null) || base.nodes[2].DataType.IsCharacterType()))
            {
                bool flag3 = (base.nodes[0].DataType.TypeCode == 100) || (base.nodes[1].DataType.TypeCode == 100);
                this._likeObject.SetIgnoreCase(flag3);
            }
            else
            {
                if ((!base.nodes[0].DataType.IsBinaryType() || !base.nodes[1].DataType.IsBinaryType()) || ((base.nodes[2] != null) && !base.nodes[2].DataType.IsBinaryType()))
                {
                    throw Error.GetError(0x15bd);
                }
                this._likeObject.IsBinary = true;
            }
            this._likeObject.DataType = base.nodes[0].DataType;
            bool flag2 = base.nodes[1].OpType == 1;
            if ((flag2 & flag) && (base.nodes[0].OpType == 1))
            {
                base.SetAsConstantValue(session);
                this._likeObject = null;
            }
            else if (flag2 & flag)
            {
                this._likeObject.IsVariable = false;
                object a = null;
                if (flag2)
                {
                    a = base.nodes[1].GetValue(session);
                    if (base.nodes[1].DataType.IsCharacterType())
                    {
                        if (base.nodes[1].DataType.IsLobType())
                        {
                            a = SqlType.SqlVarcharDefault.ConvertToType(session, a, base.nodes[1].DataType);
                        }
                    }
                    else if (base.nodes[1].DataType.IsLobType())
                    {
                        a = SqlType.SqlVarbinaryDefault.ConvertToType(session, a, base.nodes[1].DataType);
                    }
                }
                object obj3 = null;
                if (flag && (base.nodes[2] > null))
                {
                    obj3 = base.nodes[2].GetValue(session);
                    if (base.nodes[2].DataType.IsCharacterType())
                    {
                        if (base.nodes[2].DataType.IsLobType())
                        {
                            obj3 = SqlType.SqlVarcharDefault.ConvertToType(session, obj3, base.nodes[2].DataType);
                        }
                    }
                    else if (base.nodes[2].DataType.IsLobType())
                    {
                        obj3 = SqlType.SqlVarbinaryDefault.ConvertToType(session, obj3, base.nodes[2].DataType);
                    }
                }
                this._likeObject.SetPattern(session, a, obj3, base.nodes[2] > null);
                base.NoOptimisation = true;
            }
        }

        public override bool TestCondition(Session session)
        {
            if (base.OpType != 0x35)
            {
                return base.TestCondition(session);
            }
            object o = base.nodes[0].GetValue(session);
            if (this._likeObject.IsVariable)
            {
                object rightValue = base.nodes[1].GetValue(session);
                object valueCore = this.GetValueCore(session, ref rightValue);
                lock (this._likeObject)
                {
                    this._likeObject.SetPattern(session, rightValue, valueCore, base.nodes[2] > null);
                    return this._likeObject.CompareBool(session, o);
                }
            }
            return this._likeObject.CompareBool(session, o);
        }
    }
}


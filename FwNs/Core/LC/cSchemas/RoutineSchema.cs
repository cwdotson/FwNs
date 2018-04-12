namespace FwNs.Core.LC.cSchemas
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using System;
    using System.Text;

    public sealed class RoutineSchema : ISchemaObject
    {
        private readonly QNameManager.QName _name;
        private readonly int _routineType;
        public Routine[] Routines = Routine.EmptyArray;

        public RoutineSchema(int type, QNameManager.QName name)
        {
            this._routineType = type;
            this._name = name;
        }

        public void AddSpecificRoutine(Database database, Routine routine)
        {
            SqlType[] parameterTypes = routine.GetParameterTypes();
            for (int i = 0; i < this.Routines.Length; i++)
            {
                if (this.Routines[i].ParameterTypes.Length != parameterTypes.Length)
                {
                    continue;
                }
                if (this._routineType == 0x11)
                {
                    throw Error.GetError(0x15e5);
                }
                if (this.Routines[i].IsAggregate() != routine.IsAggregate())
                {
                    throw Error.GetError(0x15e5);
                }
                bool flag = true;
                for (int j = 0; j < parameterTypes.Length; j++)
                {
                    if (!this.Routines[i].ParameterTypes[j].Equals(parameterTypes[j]))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    throw Error.GetError(0x15e5);
                }
            }
            if (routine.GetSpecificName() == null)
            {
                QNameManager.QName name = database.NameManager.NewSpecificRoutineName(this._name);
                routine.SetSpecificName(name);
            }
            else
            {
                routine.GetSpecificName().Parent = this._name;
                routine.GetSpecificName().schema = this._name.schema;
            }
            routine.SetName(this._name);
            routine.routineSchema = this;
            this.Routines = ArrayUtil.ResizeArray<Routine>(this.Routines, this.Routines.Length + 1);
            this.Routines[this.Routines.Length - 1] = routine;
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
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
            OrderedHashSet<ISchemaObject> set1 = new OrderedHashSet<ISchemaObject>();
            set1.AddAll(this.Routines);
            return set1;
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
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            for (int i = 0; i < this.Routines.Length; i++)
            {
                set.AddAll(this.Routines[i].GetReferences());
            }
            return set;
        }

        public QNameManager.QName GetSchemaName()
        {
            return this._name.schema;
        }

        public int GetSchemaObjectType()
        {
            return this._routineType;
        }

        public Routine GetSpecificRoutine(SqlType[] types)
        {
            int index = -1;
            int num2 = 0;
            while (num2 < this.Routines.Length)
            {
                int num3 = 0;
                if (!this.Routines[num2].IsAggregate())
                {
                    goto Label_00CF;
                }
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] != null)
                    {
                        int num1 = types[i].PrecedenceDegree(this.Routines[num2].ParameterTypes[i]);
                        if ((num1 == 0) && (num3 == i))
                        {
                            num3 = i + 1;
                        }
                        if (num1 < -64)
                        {
                            if (index == -1)
                            {
                                goto Label_00C6;
                            }
                            int num5 = types[i].PrecedenceDegree(this.Routines[index].ParameterTypes[i]);
                            int num6 = types[i].PrecedenceDegree(this.Routines[num2].ParameterTypes[i]);
                            if (num5 != num6)
                            {
                                if (num6 < num5)
                                {
                                    index = num2;
                                }
                                goto Label_00C6;
                            }
                        }
                    }
                }
                if (num3 == types.Length)
                {
                    return this.Routines[num2];
                }
                if (index != -1)
                {
                    goto Label_00CF;
                }
                index = num2;
            Label_00C6:
                num2++;
                continue;
            Label_00CF:
                if (this.Routines[num2].ParameterTypes.Length == types.Length)
                {
                    if (types.Length == 0)
                    {
                        return this.Routines[num2];
                    }
                    for (int j = 0; j < types.Length; j++)
                    {
                        if (types[j] != null)
                        {
                            int num8 = types[j].PrecedenceDegree(this.Routines[num2].ParameterTypes[j]);
                            if (num8 < -64)
                            {
                                goto Label_00C6;
                            }
                            if ((num8 == 0) && (num3 == j))
                            {
                                num3 = j + 1;
                            }
                        }
                    }
                    if (num3 == types.Length)
                    {
                        return this.Routines[num2];
                    }
                    if (index == -1)
                    {
                        index = num2;
                    }
                    else
                    {
                        for (int k = 0; k < types.Length; k++)
                        {
                            if (types[k] != null)
                            {
                                int num10 = types[k].PrecedenceDegree(this.Routines[index].ParameterTypes[k]);
                                int num11 = types[k].PrecedenceDegree(this.Routines[num2].ParameterTypes[k]);
                                if (num10 != num11)
                                {
                                    if (num11 < num10)
                                    {
                                        index = num2;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                goto Label_00C6;
            }
            if (index < 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(this._name.GetSchemaQualifiedStatementName());
                builder.Append("(");
                for (int i = 0; i < types.Length; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(types[i].GetNameString());
                }
                builder.Append(")");
                throw Error.GetError(0x15e9, builder.ToString());
            }
            return this.Routines[index];
        }

        public Routine GetSpecificRoutine(int paramCount)
        {
            for (int i = 0; i < this.Routines.Length; i++)
            {
                if (this.Routines[i].ParameterTypes.Length == paramCount)
                {
                    return this.Routines[i];
                }
            }
            throw Error.GetError(0x157d, this._name.GetStatementName());
        }

        public Routine[] GetSpecificRoutines()
        {
            return this.Routines;
        }

        public string GetSql()
        {
            return null;
        }

        public bool IsAggregate()
        {
            return this.Routines[0].isAggregate;
        }

        public void RemoveSpecificRoutine(Routine routine)
        {
            for (int i = 0; i < this.Routines.Length; i++)
            {
                if (this.Routines[i] == routine)
                {
                    this.Routines = ArrayUtil.ToAdjustedArray<Routine>(this.Routines, null, i, -1);
                    return;
                }
            }
        }
    }
}


namespace FwNs.Core.LC.cSchemas
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class SchemaObjectSet
    {
        public HashMap<string, object> Map;
        private readonly int _type;

        public SchemaObjectSet(int type)
        {
            this._type = type;
            switch (type)
            {
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 12:
                case 13:
                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 0x18:
                case 0x1b:
                    this.Map = new HashMappedList<string, object>();
                    return;

                case 5:
                case 9:
                case 20:
                    this.Map = new HashMap<string, object>();
                    break;

                case 10:
                case 11:
                case 0x12:
                case 0x13:
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x19:
                case 0x1a:
                    break;

                default:
                    return;
            }
        }

        public void Add(ISchemaObject obj)
        {
            QNameManager.QName specificName = obj.GetName();
            if (this._type == 0x18)
            {
                specificName = ((Routine) obj).GetSpecificName();
            }
            if (this.Map.ContainsKey(specificName.Name))
            {
                throw Error.GetError(GetAddErrorCode(specificName.type), specificName.Name);
            }
            object obj2 = obj;
            switch (specificName.type)
            {
                case 5:
                case 9:
                case 20:
                    obj2 = specificName;
                    break;
            }
            this.Map.Put(specificName.Name, obj2);
        }

        public static void AddAllSql(OrderedHashSet<object> resolved, OrderedHashSet<object> unresolved, List<string> list, Iterator<object> it, OrderedHashSet<object> newResolved)
        {
            while (it.HasNext())
            {
                ISchemaObject key = (ISchemaObject) it.Next();
                OrderedHashSet<QNameManager.QName> references = key.GetReferences();
                bool flag = true;
                for (int i = 0; i < references.Size(); i++)
                {
                    QNameManager.QName name = references.Get(i);
                    if (SqlInvariants.IsSystemSchemaName(name) || (name.schema == SqlInvariants.ModuleQname))
                    {
                        continue;
                    }
                    int type = name.type;
                    switch (type)
                    {
                        case 3:
                        {
                            if (!resolved.Contains(name))
                            {
                                flag = false;
                            }
                            continue;
                        }
                        case 4:
                        case 6:
                        case 7:
                        case 8:
                        case 10:
                        case 11:
                        case 15:
                        {
                            continue;
                        }
                        case 5:
                        {
                            if (name.Parent == key.GetName())
                            {
                                Constraint constraint = ((Table) key).GetConstraint(name.Name);
                                if ((constraint.GetConstraintType() == 3) && !IsChildObjectResolved(constraint, resolved))
                                {
                                    flag = false;
                                }
                            }
                            continue;
                        }
                        case 9:
                        {
                            if (key.GetSchemaObjectType() != 3)
                            {
                                break;
                            }
                            Table table1 = (Table) key;
                            if (!IsChildObjectResolved(table1.GetColumn(table1.FindColumn(name.Name)), resolved))
                            {
                                flag = false;
                            }
                            continue;
                        }
                        case 12:
                        case 13:
                        case 0x10:
                        case 0x11:
                            goto Label_0147;

                        case 14:
                        {
                            if (name.schema != null)
                            {
                                goto Label_0147;
                            }
                            continue;
                        }
                        default:
                            goto Label_013B;
                    }
                    if (!resolved.Contains(name.Parent))
                    {
                        flag = false;
                    }
                    continue;
                Label_013B:
                    if ((type != 0x18) && (type != 0x1b))
                    {
                        continue;
                    }
                Label_0147:
                    if (!resolved.Contains(name))
                    {
                        flag = false;
                    }
                }
                if (!flag)
                {
                    unresolved.Add(key);
                }
                else
                {
                    QNameManager.QName specificName;
                    if (((key.GetSchemaObjectType() == 0x10) || (key.GetSchemaObjectType() == 0x1b)) || (key.GetSchemaObjectType() == 0x11))
                    {
                        specificName = ((Routine) key).GetSpecificName();
                    }
                    else
                    {
                        specificName = key.GetName();
                    }
                    resolved.Add(specificName);
                    if (newResolved != null)
                    {
                        newResolved.Add(key);
                    }
                    if (key.GetSchemaObjectType() == 3)
                    {
                        list.AddRange(((Table) key).GetSql(resolved, unresolved));
                    }
                    else
                    {
                        if (((key.GetSchemaObjectType() - 0x10) <= 1) && ((Routine) key).IsRecursive)
                        {
                            list.Add(((Routine) key).GetSqlDeclaration());
                            list.Add(((Routine) key).GetSqlAlter());
                            continue;
                        }
                        list.Add(key.GetSql());
                    }
                }
            }
        }

        public void CheckAdd(QNameManager.QName name)
        {
            if (this.Map.ContainsKey(name.Name))
            {
                throw Error.GetError(GetAddErrorCode(name.type), name.Name);
            }
        }

        public void CheckExists(string name)
        {
            if (!this.Map.ContainsKey(name))
            {
                throw Error.GetError(GetGetErrorCode(this._type), name);
            }
        }

        private static int GetAddErrorCode(int type)
        {
            switch (type)
            {
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 12:
                case 13:
                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 20:
                case 0x18:
                case 0x1b:
                    return 0x1580;
            }
            throw Error.RuntimeError(0xc9, "SchemaObjectSet");
        }

        public static int GetGetErrorCode(int type)
        {
            switch (type)
            {
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 12:
                case 13:
                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 20:
                case 0x18:
                case 0x1b:
                    return 0x157d;
            }
            throw Error.RuntimeError(0xc9, "SchemaObjectSet");
        }

        public static string GetName(int type)
        {
            switch (type)
            {
                case 3:
                    return "TABLE";

                case 4:
                    return "VIEW";

                case 5:
                    return "CONSTRAINT";

                case 6:
                    return "ASSERTION";

                case 7:
                    return "SEQUENCE";

                case 8:
                    return "TRIGGER";

                case 9:
                    return "COLUMN";

                case 12:
                    return "TYPE";

                case 13:
                    return "DOMAIN";

                case 14:
                    return "CHARACTER SET";

                case 15:
                    return "COLLATION";

                case 0x10:
                    return "FUNCTION";

                case 0x11:
                    return "PROCEDURE";

                case 20:
                    return "INDEX";

                case 0x1b:
                    return "AGGREGATE";
            }
            throw Error.RuntimeError(0xc9, "SchemaObjectSet");
        }

        public QNameManager.QName GetName(string name)
        {
            switch (this._type)
            {
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 12:
                case 13:
                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 0x18:
                case 0x1b:
                {
                    ISchemaObject obj2 = (ISchemaObject) this.Map.Get(name);
                    if (obj2 == null)
                    {
                        return null;
                    }
                    return obj2.GetName();
                }
                case 5:
                case 9:
                case 20:
                    return (QNameManager.QName) this.Map.Get(name);
            }
            return (QNameManager.QName) this.Map.Get(name);
        }

        public ISchemaObject GetObject(string name)
        {
            switch (this._type)
            {
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 9:
                case 12:
                case 13:
                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 0x18:
                case 0x1b:
                    return (ISchemaObject) this.Map.Get(name);
            }
            throw Error.RuntimeError(0xc9, "SchemaObjectSet");
        }

        public string[] GetSql(OrderedHashSet<object> resolved, OrderedHashSet<object> unresolved)
        {
            List<string> list = new List<string>();
            if (!(this.Map is HashMappedList<string, object>))
            {
                return null;
            }
            if (this.Map.IsEmpty())
            {
                return new string[0];
            }
            Iterator<object> it = this.Map.GetValues().GetIterator();
            if (((this._type == 0x10) || (this._type == 0x11)) || (this._type == 0x1b))
            {
                OrderedHashSet<object> set = new OrderedHashSet<object>();
                while (it.HasNext())
                {
                    RoutineSchema schema = (RoutineSchema) it.Next();
                    for (int i = 0; i < schema.Routines.Length; i++)
                    {
                        Routine key = schema.Routines[i];
                        if ((key.DataImpact != 1) && (key.DataImpact != 2))
                        {
                            set.Add(key);
                        }
                    }
                }
                it = set.GetIterator();
            }
            AddAllSql(resolved, unresolved, list, it, null);
            return list.ToArray();
        }

        private static bool IsChildObjectResolved(ISchemaObject obj, OrderedHashSet<object> resolved)
        {
            OrderedHashSet<QNameManager.QName> references = obj.GetReferences();
            for (int i = 0; i < references.Size(); i++)
            {
                QNameManager.QName name = references.Get(i);
                if (!SqlInvariants.IsSystemSchemaName(name) && !resolved.Contains(name))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsEmpty()
        {
            return this.Map.IsEmpty();
        }

        public void Remove(string name)
        {
            this.Map.Remove(name);
        }

        public void RemoveParent(QNameManager.QName parent)
        {
            Iterator<object> iterator = this.Map.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                if ((this._type == 8) || (this._type == 0x18))
                {
                    if (((ISchemaObject) iterator.Next()).GetName().Parent == parent)
                    {
                        iterator.Remove();
                    }
                }
                else if (((QNameManager.QName) iterator.Next()).Parent == parent)
                {
                    iterator.Remove();
                }
            }
        }

        public void Rename(QNameManager.QName name, QNameManager.QName newName)
        {
            if (this.Map.ContainsKey(newName.Name))
            {
                throw Error.GetError(GetAddErrorCode(name.type), newName.Name);
            }
            switch (newName.type)
            {
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 12:
                case 13:
                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 0x1b:
                {
                    int index = ((HashMappedList<string, object>) this.Map).GetIndex(name.Name);
                    if (index == -1)
                    {
                        throw Error.GetError(GetGetErrorCode(name.type), name.Name);
                    }
                    ((ISchemaObject) ((HashMappedList<string, object>) this.Map).Get(index)).GetName().Rename(newName);
                    ((HashMappedList<string, object>) this.Map).SetKey(index, name.Name);
                    return;
                }
                case 5:
                case 9:
                case 20:
                    this.Map.Remove(name.Name);
                    name.Rename(newName);
                    this.Map.Put(name.Name, name);
                    break;

                case 10:
                case 11:
                case 0x12:
                case 0x13:
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x19:
                case 0x1a:
                    break;

                default:
                    return;
            }
        }
    }
}


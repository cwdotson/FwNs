namespace FwNs.Core.LC.cRights
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Text;

    public sealed class Right
    {
        public bool IsFull = false;
        public bool IsFullSelect;
        public bool IsFullInsert;
        public bool IsFullUpdate;
        public bool IsFullReferences;
        public bool IsFullTrigger;
        public bool IsFullDelete;
        private OrderedHashSet<string> _selectColumnSet;
        private OrderedHashSet<string> _insertColumnSet;
        private OrderedHashSet<string> _updateColumnSet;
        private OrderedHashSet<string> _referencesColumnSet;
        private OrderedHashSet<string> _triggerColumnSet;
        public Right GrantableRights;
        public Grantee Grantor;
        public Grantee grantee;
        public static OrderedHashSet<string> EmptySet = new OrderedHashSet<string>();
        public static OrderedHashSet<Right> EmptySetRight = new OrderedHashSet<Right>();
        public static Right FullRights = GetFullRights();
        public static Right NoRights = new Right();
        public static string[] PrivilegeNames = new string[] { "SELECT", "INSERT", "UPDATE", "DELETE", "REFERENCES", "TRIGGER" };
        public static int[] PrivilegeTypes = new int[] { 1, 4, 8, 2, 0x40, 0x80 };

        public void Add(Right right)
        {
            if (!this.IsFull)
            {
                if (right.IsFull)
                {
                    this.Clear();
                    this.IsFull = true;
                }
                else
                {
                    this.IsFullSelect |= right.IsFullSelect;
                    this.IsFullInsert |= right.IsFullInsert;
                    this.IsFullUpdate |= right.IsFullUpdate;
                    this.IsFullReferences |= right.IsFullReferences;
                    this.IsFullDelete |= right.IsFullDelete;
                    if (this.IsFullSelect)
                    {
                        this._selectColumnSet = null;
                    }
                    else if (right._selectColumnSet != null)
                    {
                        if (this._selectColumnSet == null)
                        {
                            this._selectColumnSet = new OrderedHashSet<string>();
                        }
                        this._selectColumnSet.AddAll(right._selectColumnSet);
                    }
                    if (this.IsFullInsert)
                    {
                        this._insertColumnSet = null;
                    }
                    else if (right._insertColumnSet != null)
                    {
                        if (this._insertColumnSet == null)
                        {
                            this._insertColumnSet = new OrderedHashSet<string>();
                        }
                        this._insertColumnSet.AddAll(right._insertColumnSet);
                    }
                    if (this.IsFullUpdate)
                    {
                        this._updateColumnSet = null;
                    }
                    else if (right._updateColumnSet != null)
                    {
                        if (this._updateColumnSet == null)
                        {
                            this._updateColumnSet = new OrderedHashSet<string>();
                        }
                        this._updateColumnSet.AddAll(right._updateColumnSet);
                    }
                    if (this.IsFullReferences)
                    {
                        this._referencesColumnSet = null;
                    }
                    else if (right._referencesColumnSet != null)
                    {
                        if (this._referencesColumnSet == null)
                        {
                            this._referencesColumnSet = new OrderedHashSet<string>();
                        }
                        this._referencesColumnSet.AddAll(right._referencesColumnSet);
                    }
                    if (this.IsFullTrigger)
                    {
                        this._triggerColumnSet = null;
                    }
                    else if (right._triggerColumnSet != null)
                    {
                        if (this._triggerColumnSet == null)
                        {
                            this._triggerColumnSet = new OrderedHashSet<string>();
                        }
                        this._triggerColumnSet.AddAll(right._triggerColumnSet);
                    }
                }
            }
        }

        public bool CanAccessFully(int privilegeType)
        {
            if (this.IsFull)
            {
                return true;
            }
            if (privilegeType <= 8)
            {
                switch (privilegeType)
                {
                    case 1:
                        return this.IsFullSelect;

                    case 2:
                        return this.IsFullDelete;

                    case 4:
                        return this.IsFullInsert;

                    case 8:
                        return this.IsFullUpdate;
                }
            }
            else
            {
                if (privilegeType == 0x40)
                {
                    return this.IsFullReferences;
                }
                if (privilegeType == 0x80)
                {
                    return this.IsFullTrigger;
                }
            }
            throw Error.RuntimeError(0xc9, "Right");
        }

        public bool CanDelete()
        {
            if (!this.IsFull)
            {
                return this.IsFullDelete;
            }
            return true;
        }

        public bool CanInsert(Table table, bool[] columnCheckList)
        {
            if (!this.IsFull && !this.IsFullInsert)
            {
                return ContainsAllColumns(this._insertColumnSet, table, columnCheckList);
            }
            return true;
        }

        public bool CanReference(Table table, bool[] columnCheckList)
        {
            if (!this.IsFull && !this.IsFullReferences)
            {
                return ContainsAllColumns(this._referencesColumnSet, table, columnCheckList);
            }
            return true;
        }

        public bool CanSelect(Table table, bool[] columnCheckList)
        {
            if (!this.IsFull && !this.IsFullSelect)
            {
                return ContainsAllColumns(this._selectColumnSet, table, columnCheckList);
            }
            return true;
        }

        public bool CanUpdate(Table table, bool[] columnCheckList)
        {
            if (!this.IsFull && !this.IsFullUpdate)
            {
                return ContainsAllColumns(this._updateColumnSet, table, columnCheckList);
            }
            return true;
        }

        private void Clear()
        {
            this.IsFull = this.IsFullSelect = this.IsFullInsert = this.IsFullUpdate = this.IsFullReferences = this.IsFullDelete = false;
            this._selectColumnSet = this._insertColumnSet = this._updateColumnSet = this._referencesColumnSet = (OrderedHashSet<string>) (this._triggerColumnSet = null);
        }

        public bool Contains(Right right)
        {
            if (!this.IsFull)
            {
                if (((right.IsFull || !ContainsRights(this.IsFullSelect, this._selectColumnSet, right._selectColumnSet, right.IsFullSelect)) || (!ContainsRights(this.IsFullInsert, this._insertColumnSet, right._insertColumnSet, right.IsFullInsert) || !ContainsRights(this.IsFullUpdate, this._updateColumnSet, right._updateColumnSet, right.IsFullUpdate))) || (!ContainsRights(this.IsFullReferences, this._referencesColumnSet, right._referencesColumnSet, right.IsFullReferences) || !ContainsRights(this.IsFullTrigger, this._triggerColumnSet, right._triggerColumnSet, right.IsFullTrigger)))
                {
                    return false;
                }
                if (!this.IsFullDelete)
                {
                    return !right.IsFullDelete;
                }
            }
            return true;
        }

        public static bool ContainsAllColumns(OrderedHashSet<string> columnSet, Table table, bool[] columnCheckList)
        {
            for (int i = 0; i < columnCheckList.Length; i++)
            {
                if (columnCheckList[i])
                {
                    if (columnSet == null)
                    {
                        return false;
                    }
                    if (!columnSet.Contains(table.GetColumn(i).GetName().Name))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ContainsRights(bool isFull, OrderedHashSet<string> columnSet, OrderedHashSet<string> otherColumnSet, bool otherIsFull)
        {
            if (isFull)
            {
                return true;
            }
            if (otherIsFull)
            {
                return false;
            }
            return ((otherColumnSet == null) || ((columnSet != null) && columnSet.ContainsAll(otherColumnSet)));
        }

        public Right Duplicate()
        {
            Right right1 = new Right();
            right1.Add(this);
            return right1;
        }

        private static void GetColumnList(Table t, OrderedHashSet<string> set, StringBuilder buf)
        {
            int num = 0;
            bool[] newColumnCheckList = t.GetNewColumnCheckList();
            for (int i = 0; i < set.Size(); i++)
            {
                string name = set.Get(i);
                int num5 = t.FindColumn(name);
                if (num5 != -1)
                {
                    newColumnCheckList[num5] = true;
                    num++;
                }
            }
            if (num == 0)
            {
                throw Error.RuntimeError(0xc9, "Right");
            }
            buf.Append('(');
            int index = 0;
            int num3 = 0;
            while (index < newColumnCheckList.Length)
            {
                if (newColumnCheckList[index])
                {
                    num3++;
                    buf.Append(t.GetColumn(index).GetName().StatementName);
                    if (num3 < num)
                    {
                        buf.Append(',');
                    }
                }
                index++;
            }
            buf.Append(')');
        }

        public OrderedHashSet<string> GetColumnsForAllRights(Table table)
        {
            if (this.IsFull)
            {
                return table.GetColumnNameSet();
            }
            if ((this.IsFullSelect || this.IsFullInsert) || (this.IsFullUpdate || this.IsFullReferences))
            {
                return table.GetColumnNameSet();
            }
            OrderedHashSet<string> set2 = new OrderedHashSet<string>();
            if (this._selectColumnSet != null)
            {
                set2.AddAll(this._selectColumnSet);
            }
            if (this._insertColumnSet != null)
            {
                set2.AddAll(this._insertColumnSet);
            }
            if (this._updateColumnSet != null)
            {
                set2.AddAll(this._updateColumnSet);
            }
            if (this._referencesColumnSet != null)
            {
                set2.AddAll(this._referencesColumnSet);
            }
            return set2;
        }

        public OrderedHashSet<string> GetColumnsForPrivilege(Table table, int type)
        {
            if (this.IsFull)
            {
                return table.GetColumnNameSet();
            }
            if (type <= 4)
            {
                if (type == 1)
                {
                    if (!this.IsFullSelect)
                    {
                        OrderedHashSet<string> set2 = this._selectColumnSet;
                        if (set2 == null)
                        {
                            return EmptySet;
                        }
                        return set2;
                    }
                    return table.GetColumnNameSet();
                }
                if (type == 4)
                {
                    if (!this.IsFullInsert)
                    {
                        OrderedHashSet<string> set3 = this._insertColumnSet;
                        if (set3 == null)
                        {
                            return EmptySet;
                        }
                        return set3;
                    }
                    return table.GetColumnNameSet();
                }
            }
            else
            {
                if (type == 8)
                {
                    if (!this.IsFullUpdate)
                    {
                        OrderedHashSet<string> set4 = this._updateColumnSet;
                        if (set4 == null)
                        {
                            return EmptySet;
                        }
                        return set4;
                    }
                    return table.GetColumnNameSet();
                }
                if (type == 0x40)
                {
                    if (!this.IsFullReferences)
                    {
                        OrderedHashSet<string> set5 = this._referencesColumnSet;
                        if (set5 == null)
                        {
                            return EmptySet;
                        }
                        return set5;
                    }
                    return table.GetColumnNameSet();
                }
                if (type == 0x80)
                {
                    if (!this.IsFullTrigger)
                    {
                        OrderedHashSet<string> set6 = this._triggerColumnSet;
                        if (set6 == null)
                        {
                            return EmptySet;
                        }
                        return set6;
                    }
                    return table.GetColumnNameSet();
                }
            }
            return EmptySet;
        }

        private static Right GetFullRights()
        {
            return new Right { 
                Grantor = GranteeManager.SystemAuthorisation,
                IsFull = true
            };
        }

        public Right GetGrantableRights()
        {
            return (this.GrantableRights ?? NoRights);
        }

        public Grantee GetGrantee()
        {
            return this.grantee;
        }

        public Grantee GetGrantor()
        {
            return this.Grantor;
        }

        public string GetTableRightsSQL(Table table)
        {
            StringBuilder buf = new StringBuilder();
            if (this.IsFull)
            {
                return "ALL";
            }
            if (this.IsFullSelect)
            {
                buf.Append("SELECT");
                buf.Append(',');
            }
            else if (this._selectColumnSet != null)
            {
                buf.Append("SELECT");
                GetColumnList(table, this._selectColumnSet, buf);
                buf.Append(',');
            }
            if (this.IsFullInsert)
            {
                buf.Append("INSERT");
                buf.Append(',');
            }
            else if (this._insertColumnSet != null)
            {
                buf.Append("INSERT");
                GetColumnList(table, this._insertColumnSet, buf);
                buf.Append(',');
            }
            if (this.IsFullUpdate)
            {
                buf.Append("UPDATE");
                buf.Append(',');
            }
            else if (this._updateColumnSet != null)
            {
                buf.Append("UPDATE");
                GetColumnList(table, this._updateColumnSet, buf);
                buf.Append(',');
            }
            if (this.IsFullDelete)
            {
                buf.Append("DELETE");
                buf.Append(',');
            }
            if (this.IsFullReferences)
            {
                buf.Append("REFERENCES");
                buf.Append(',');
            }
            else if (this._referencesColumnSet != null)
            {
                buf.Append("REFERENCES");
                buf.Append(',');
            }
            if (this.IsFullTrigger)
            {
                buf.Append("TRIGGER");
                buf.Append(',');
            }
            else if (this._triggerColumnSet != null)
            {
                buf.Append("TRIGGER");
                buf.Append(',');
            }
            return buf.ToString().Substring(0, buf.Length - 1);
        }

        public bool IsEmpty()
        {
            if (((((!this.IsFull && !this.IsFullSelect) && (!this.IsFullInsert && !this.IsFullUpdate)) && ((!this.IsFullReferences && !this.IsFullDelete) && ((this._selectColumnSet == null) || this._selectColumnSet.IsEmpty()))) && ((this._insertColumnSet == null) || this._insertColumnSet.IsEmpty())) && (((this._updateColumnSet == null) || this._updateColumnSet.IsEmpty()) && ((this._referencesColumnSet == null) || this._referencesColumnSet.IsEmpty())))
            {
                if (this._triggerColumnSet != null)
                {
                    return this._triggerColumnSet.IsEmpty();
                }
                return true;
            }
            return false;
        }

        public void Remove(ISchemaObject obj, Right right)
        {
            if (right.IsFull)
            {
                this.Clear();
            }
            else
            {
                if (this.IsFull)
                {
                    this.IsFull = false;
                    this.IsFullSelect = this.IsFullInsert = this.IsFullUpdate = this.IsFullReferences = this.IsFullDelete = true;
                }
                if (right.IsFullDelete)
                {
                    this.IsFullDelete = false;
                }
                Table table = (Table) obj;
                if (this.IsFullSelect || (this._selectColumnSet != null))
                {
                    if (right.IsFullSelect)
                    {
                        this.IsFullSelect = false;
                        this._selectColumnSet = null;
                    }
                    else if (right._selectColumnSet != null)
                    {
                        if (this.IsFullSelect)
                        {
                            this.IsFullSelect = false;
                            this._selectColumnSet = table.GetColumnNameSet();
                        }
                        this._selectColumnSet.RemoveAll(right._selectColumnSet);
                        if (this._selectColumnSet.IsEmpty())
                        {
                            this._selectColumnSet = null;
                        }
                    }
                }
                if (this.IsFullInsert || (this._insertColumnSet != null))
                {
                    if (right.IsFullInsert)
                    {
                        this.IsFullInsert = false;
                        this._insertColumnSet = null;
                    }
                    else if (right._insertColumnSet != null)
                    {
                        if (this.IsFullInsert)
                        {
                            this.IsFullInsert = false;
                            this._insertColumnSet = table.GetColumnNameSet();
                        }
                        this._insertColumnSet.RemoveAll(right._insertColumnSet);
                        if (this._insertColumnSet.IsEmpty())
                        {
                            this._insertColumnSet = null;
                        }
                    }
                }
                if (this.IsFullUpdate || (this._updateColumnSet != null))
                {
                    if (right.IsFullUpdate)
                    {
                        this.IsFullUpdate = false;
                        this._updateColumnSet = null;
                    }
                    else if (right._updateColumnSet != null)
                    {
                        if (this.IsFullUpdate)
                        {
                            this.IsFullUpdate = false;
                            this._updateColumnSet = table.GetColumnNameSet();
                        }
                        this._updateColumnSet.RemoveAll(right._updateColumnSet);
                        if (this._updateColumnSet.IsEmpty())
                        {
                            this._updateColumnSet = null;
                        }
                    }
                }
                if (this.IsFullReferences || (this._referencesColumnSet != null))
                {
                    if (right.IsFullReferences)
                    {
                        this.IsFullReferences = false;
                        this._referencesColumnSet = null;
                    }
                    else if (right._referencesColumnSet != null)
                    {
                        if (this.IsFullReferences)
                        {
                            this.IsFullReferences = false;
                            this._referencesColumnSet = table.GetColumnNameSet();
                        }
                        this._referencesColumnSet.RemoveAll(right._referencesColumnSet);
                        if (this._referencesColumnSet.IsEmpty())
                        {
                            this._referencesColumnSet = null;
                        }
                    }
                }
                if (this.IsFullTrigger || (this._triggerColumnSet != null))
                {
                    if (right.IsFullTrigger)
                    {
                        this.IsFullTrigger = false;
                        this._triggerColumnSet = null;
                    }
                    else if (right._triggerColumnSet != null)
                    {
                        if (this.IsFullTrigger)
                        {
                            this.IsFullTrigger = false;
                            this._triggerColumnSet = table.GetColumnNameSet();
                        }
                        this._triggerColumnSet.RemoveAll(right._triggerColumnSet);
                        if (this._triggerColumnSet.IsEmpty())
                        {
                            this._triggerColumnSet = null;
                        }
                    }
                }
            }
        }

        public void Set(int type, OrderedHashSet<string> set)
        {
            if (type <= 8)
            {
                switch (type)
                {
                    case 1:
                        if (set == null)
                        {
                            this.IsFullSelect = true;
                        }
                        this._selectColumnSet = set;
                        return;

                    case 2:
                        if (set != null)
                        {
                            break;
                        }
                        this.IsFullDelete = true;
                        return;

                    case 4:
                        if (set == null)
                        {
                            this.IsFullInsert = true;
                        }
                        this._insertColumnSet = set;
                        return;

                    case 8:
                        if (set == null)
                        {
                            this.IsFullUpdate = true;
                        }
                        this._updateColumnSet = set;
                        return;
                }
            }
            else if (type == 0x40)
            {
                if (set == null)
                {
                    this.IsFullReferences = true;
                }
                this._referencesColumnSet = set;
            }
            else if (type == 0x80)
            {
                if (set == null)
                {
                    this.IsFullTrigger = true;
                }
                this._triggerColumnSet = set;
            }
        }

        public void SetColumns(Table table)
        {
            if (this._selectColumnSet != null)
            {
                SetColumns(table, this._selectColumnSet);
            }
            if (this._insertColumnSet != null)
            {
                SetColumns(table, this._insertColumnSet);
            }
            if (this._updateColumnSet != null)
            {
                SetColumns(table, this._updateColumnSet);
            }
            if (this._referencesColumnSet != null)
            {
                SetColumns(table, this._referencesColumnSet);
            }
            if (this._triggerColumnSet != null)
            {
                SetColumns(table, this._triggerColumnSet);
            }
        }

        private static void SetColumns(Table t, OrderedHashSet<string> set)
        {
            int num = 0;
            bool[] newColumnCheckList = t.GetNewColumnCheckList();
            for (int i = 0; i < set.Size(); i++)
            {
                string name = set.Get(i);
                int index = t.FindColumn(name);
                if (index == -1)
                {
                    throw Error.GetError(0x157d, name);
                }
                newColumnCheckList[index] = true;
                num++;
            }
            if (num == 0)
            {
                throw Error.GetError(0x157d);
            }
            set.Clear();
            for (int j = 0; j < newColumnCheckList.Length; j++)
            {
                if (newColumnCheckList[j])
                {
                    set.Add(t.GetColumn(j).GetName().Name);
                }
            }
        }
    }
}


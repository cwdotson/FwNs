namespace FwNs.Core.LC.cRights
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Grantee : ISchemaObject
    {
        private bool _isRole;
        private bool _isAdminDirect;
        private bool _isAdmin;
        private bool _isPublic;
        private bool _isSystem;
        protected QNameManager.QName GranteeName;
        private readonly MultiValueHashMap<QNameManager.QName, Right> _directRightsMap = new MultiValueHashMap<QNameManager.QName, Right>();
        private readonly HashMap<QNameManager.QName, Right> _fullRightsMap = new HashMap<QNameManager.QName, Right>();
        public OrderedHashSet<Grantee> Roles;
        private readonly MultiValueHashMap<QNameManager.QName, Right> _grantedRightsMap = new MultiValueHashMap<QNameManager.QName, Right>();
        protected GranteeManager granteeManager;
        protected Right OwnerRights;

        public Grantee(QNameManager.QName name, GranteeManager man)
        {
            this.GranteeName = name;
            this.granteeManager = man;
            this.Roles = new OrderedHashSet<Grantee>();
            Right right1 = new Right {
                IsFull = true,
                Grantor = GranteeManager.SystemAuthorisation,
                grantee = this
            };
            this.OwnerRights = right1;
        }

        private void AddGranteeAndRoles(OrderedHashSet<Grantee> set)
        {
            set.Add(this);
            for (int i = 0; i < this.Roles.Size(); i++)
            {
                Grantee key = this.Roles.Get(i);
                if (!set.Contains(key))
                {
                    key.AddGranteeAndRoles(set);
                }
            }
        }

        private void AddToFullRights(HashMap<QNameManager.QName, Right> map)
        {
            Iterator<QNameManager.QName> iterator = map.GetKeySet().GetIterator();
            while (iterator.HasNext())
            {
                QNameManager.QName key = iterator.Next();
                Right right = map.Get(key);
                Right right2 = this._fullRightsMap.Get(key);
                if (right2 == null)
                {
                    right2 = right.Duplicate();
                    this._fullRightsMap.Put(key, right2);
                }
                else
                {
                    right2.Add(right);
                }
                if (right.GrantableRights != null)
                {
                    if (right2.GrantableRights == null)
                    {
                        right2.GrantableRights = right.GrantableRights.Duplicate();
                    }
                    else
                    {
                        right2.GrantableRights.Add(right.GrantableRights);
                    }
                }
            }
        }

        private void AddToFullRights(MultiValueHashMap<QNameManager.QName, Right> map)
        {
            Iterator<QNameManager.QName> iterator = map.GetKeySet().GetIterator();
            while (iterator.HasNext())
            {
                QNameManager.QName key = iterator.Next();
                Iterator<Right> iterator2 = map.Get(key);
                Right right = this._fullRightsMap.Get(key);
                while (iterator2.HasNext())
                {
                    Right right2 = iterator2.Next();
                    if (right == null)
                    {
                        right = right2.Duplicate();
                        this._fullRightsMap.Put(key, right);
                    }
                    else
                    {
                        right.Add(right2);
                    }
                    if (right2.GrantableRights != null)
                    {
                        if (right.GrantableRights == null)
                        {
                            right.GrantableRights = right2.GrantableRights.Duplicate();
                        }
                        else
                        {
                            right.GrantableRights.Add(right2.GrantableRights);
                        }
                    }
                }
            }
        }

        public bool CanChangeAuthorisation()
        {
            if (!this._isAdmin)
            {
                return this.HasRole(this.granteeManager.ChangeAuthRole);
            }
            return true;
        }

        public void CheckAccess(ISchemaObject obj)
        {
            if (!this.IsFullyAccessibleByRole(obj.GetName()))
            {
                QNameManager.QName key = obj.GetName();
                Routine routine = obj as Routine;
                if (routine != null)
                {
                    key = routine.GetSpecificName();
                }
                Right right = this._fullRightsMap.Get(key);
                if ((right == null) || right.IsEmpty())
                {
                    throw Error.GetError(0x157d, obj.GetName().Name);
                }
            }
        }

        public void CheckAdmin()
        {
            if (!this.IsAdmin())
            {
                throw Error.GetError(0x1583);
            }
        }

        public void CheckDelete(Table table)
        {
            if (!this.IsFullyAccessibleByRole(table.GetName()))
            {
                Right right = this._fullRightsMap.Get(table.GetName());
                if ((right == null) || !right.CanDelete())
                {
                    throw Error.GetError(0x157d, table.GetName().Name);
                }
            }
        }

        public void CheckInsert(Table table, bool[] checkList)
        {
            if (!this.IsFullyAccessibleByRole(table.GetName()))
            {
                Right right = this._fullRightsMap.Get(table.GetName());
                if ((right == null) || !right.CanInsert(table, checkList))
                {
                    throw Error.GetError(0x157d, table.GetName().Name);
                }
            }
        }

        public void CheckReferences(Table table, bool[] checkList)
        {
            if (!this.IsFullyAccessibleByRole(table.GetName()))
            {
                Right right = this._fullRightsMap.Get(table.GetName());
                if ((right == null) || !right.CanReference(table, checkList))
                {
                    throw Error.GetError(0x157d, table.GetName().Name);
                }
            }
        }

        public void CheckSchemaUpdateOrGrantRights(string schemaName)
        {
            if (!this.HasSchemaUpdateOrGrantRights(schemaName))
            {
                throw Error.GetError(0x157d, schemaName);
            }
        }

        public void CheckSelect(Table table, bool[] checkList)
        {
            if (!this.IsFullyAccessibleByRole(table.GetName()))
            {
                Right right = this._fullRightsMap.Get(table.GetName());
                if ((right == null) || !right.CanSelect(table, checkList))
                {
                    throw Error.GetError(0x157d, table.GetName().Name);
                }
            }
        }

        public void CheckUpdate(Table table, bool[] checkList)
        {
            if (!this.IsFullyAccessibleByRole(table.GetName()))
            {
                Right right = this._fullRightsMap.Get(table.GetName());
                if ((right == null) || !right.CanUpdate(table, checkList))
                {
                    throw Error.GetError(0x157d, table.GetName().Name);
                }
            }
        }

        public void ClearPrivileges()
        {
            this.Roles.Clear();
            this._directRightsMap.Clear();
            this._grantedRightsMap.Clear();
            this._fullRightsMap.Clear();
            this._isAdmin = false;
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public OrderedHashSet<Right> GetAllDirectPrivileges(ISchemaObject obj)
        {
            if (obj.GetOwner() == this)
            {
                OrderedHashSet<Right> set1 = new OrderedHashSet<Right>();
                set1.Add(this.OwnerRights);
                return set1;
            }
            Iterator<Right> iterator = this._directRightsMap.Get(obj.GetName());
            if (iterator.HasNext())
            {
                OrderedHashSet<Right> set2 = new OrderedHashSet<Right>();
                while (iterator.HasNext())
                {
                    set2.Add(iterator.Next());
                }
                return set2;
            }
            return Right.EmptySetRight;
        }

        public Right GetAllGrantableRights(QNameManager.QName name)
        {
            if (this._isAdmin)
            {
                return name.schema.Owner.OwnerRights;
            }
            if (name.schema.Owner == this)
            {
                return this.OwnerRights;
            }
            if (this.Roles.Contains(name.schema.Owner))
            {
                return name.schema.Owner.OwnerRights;
            }
            OrderedHashSet<Grantee> allRoles = this.GetAllRoles();
            for (int i = 0; i < allRoles.Size(); i++)
            {
                Grantee grantee = allRoles.Get(i);
                if (name.schema.Owner == grantee)
                {
                    return grantee.OwnerRights;
                }
            }
            Right right2 = this._fullRightsMap.Get(name);
            if ((right2 != null) && (right2.GrantableRights != null))
            {
                return right2.GrantableRights;
            }
            return Right.NoRights;
        }

        public OrderedHashSet<Right> GetAllGrantedPrivileges(ISchemaObject obj)
        {
            Iterator<Right> iterator = this._grantedRightsMap.Get(obj.GetName());
            if (iterator.HasNext())
            {
                OrderedHashSet<Right> set2 = new OrderedHashSet<Right>();
                while (iterator.HasNext())
                {
                    set2.Add(iterator.Next());
                }
                return set2;
            }
            return Right.EmptySetRight;
        }

        public OrderedHashSet<Grantee> GetAllRoles()
        {
            OrderedHashSet<Grantee> granteeAndAllRoles = this.GetGranteeAndAllRoles();
            granteeAndAllRoles.Remove(this);
            return granteeAndAllRoles;
        }

        public QNameManager.QName GetCatalogName()
        {
            return null;
        }

        public virtual long GetChangeTimestamp()
        {
            return 0L;
        }

        public OrderedHashSet<string> GetColumnsForAllPrivileges(Table table)
        {
            if (this.IsFullyAccessibleByRole(table.GetName()))
            {
                return table.GetColumnNameSet();
            }
            Right right = this._fullRightsMap.Get(table.GetName());
            if (right != null)
            {
                return right.GetColumnsForAllRights(table);
            }
            return Right.EmptySet;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public OrderedHashSet<Grantee> GetDirectRoles()
        {
            return this.Roles;
        }

        public string GetDirectRolesAsString()
        {
            return RoleMapToString(this.Roles);
        }

        public OrderedHashSet<Grantee> GetGranteeAndAllRoles()
        {
            OrderedHashSet<Grantee> set = new OrderedHashSet<Grantee>();
            this.AddGranteeAndRoles(set);
            return set;
        }

        public OrderedHashSet<Grantee> GetGranteeAndAllRolesWithPublic()
        {
            OrderedHashSet<Grantee> set = new OrderedHashSet<Grantee>();
            this.AddGranteeAndRoles(set);
            set.Add(this.granteeManager.PublicRole);
            return set;
        }

        public QNameManager.QName GetName()
        {
            return this.GranteeName;
        }

        public string GetNameString()
        {
            return this.GranteeName.Name;
        }

        public Grantee GetOwner()
        {
            return null;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return new OrderedHashSet<QNameManager.QName>();
        }

        public MultiValueHashMap<QNameManager.QName, Right> GetRights()
        {
            return this._directRightsMap;
        }

        public List<string> GetRightsSQL()
        {
            List<string> list = new List<string>();
            string directRolesAsString = this.GetDirectRolesAsString();
            if (directRolesAsString.Length != 0)
            {
                StringBuilder builder = new StringBuilder(0x80);
                builder.Append("GRANT").Append(' ').Append(directRolesAsString);
                builder.Append(' ').Append("TO").Append(' ');
                builder.Append(this.GetStatementName());
                list.Add(builder.ToString());
            }
            MultiValueHashMap<QNameManager.QName, Right> rights = this.GetRights();
            Iterator<QNameManager.QName> iterator = rights.GetKeySet().GetIterator();
            while (iterator.HasNext())
            {
                QNameManager.QName key = iterator.Next();
                Iterator<Right> iterator2 = rights.Get(key);
                while (iterator2.HasNext())
                {
                    Right right = iterator2.Next();
                    StringBuilder builder2 = new StringBuilder(0x80);
                    QNameManager.QName name2 = key;
                    switch (name2.type)
                    {
                        case 3:
                        case 4:
                        {
                            Table table = this.granteeManager.database.schemaManager.FindUserTable(null, name2.Name, name2.schema.Name);
                            if (table != null)
                            {
                                builder2.Append("GRANT").Append(' ');
                                builder2.Append(right.GetTableRightsSQL(table));
                                builder2.Append(' ').Append("ON").Append(' ');
                                builder2.Append("TABLE").Append(' ');
                                builder2.Append(name2.GetSchemaQualifiedStatementName());
                            }
                            break;
                        }
                        case 7:
                            if (((NumberSequence) this.granteeManager.database.schemaManager.FindSchemaObject(name2.Name, name2.schema.Name, 7)) != null)
                            {
                                builder2.Append("GRANT").Append(' ');
                                builder2.Append("USAGE");
                                builder2.Append(' ').Append("ON").Append(' ');
                                builder2.Append("SEQUENCE").Append(' ');
                                builder2.Append(name2.GetSchemaQualifiedStatementName());
                            }
                            break;

                        case 12:
                            if (((SqlType) this.granteeManager.database.schemaManager.FindSchemaObject(name2.Name, name2.schema.Name, 13)) != null)
                            {
                                builder2.Append("GRANT").Append(' ');
                                builder2.Append("USAGE");
                                builder2.Append(' ').Append("ON").Append(' ');
                                builder2.Append("TYPE").Append(' ');
                                builder2.Append(name2.GetSchemaQualifiedStatementName());
                            }
                            break;

                        case 13:
                            if (((SqlType) this.granteeManager.database.schemaManager.FindSchemaObject(name2.Name, name2.schema.Name, 13)) != null)
                            {
                                builder2.Append("GRANT").Append(' ');
                                builder2.Append("USAGE");
                                builder2.Append(' ').Append("ON").Append(' ');
                                builder2.Append("DOMAIN").Append(' ');
                                builder2.Append(name2.GetSchemaQualifiedStatementName());
                            }
                            break;

                        case 0x10:
                        case 0x11:
                        case 0x18:
                        case 0x1b:
                        {
                            ISchemaObject obj2 = this.granteeManager.database.schemaManager.FindSchemaObject(name2.Name, name2.schema.Name, name2.type);
                            if (obj2 != null)
                            {
                                builder2.Append("GRANT").Append(' ');
                                builder2.Append("EXECUTE").Append(' ');
                                builder2.Append("ON").Append(' ');
                                builder2.Append("SPECIFIC").Append(' ');
                                if (obj2.GetSchemaObjectType() == 0x11)
                                {
                                    builder2.Append("PROCEDURE");
                                }
                                else if (obj2.GetSchemaObjectType() == 0x1b)
                                {
                                    builder2.Append("AGGREGATE");
                                }
                                else
                                {
                                    builder2.Append("FUNCTION");
                                }
                                builder2.Append(' ');
                                builder2.Append(name2.GetSchemaQualifiedStatementName());
                            }
                            break;
                        }
                    }
                    if (builder2.Length != 0)
                    {
                        builder2.Append(' ').Append("TO").Append(' ');
                        builder2.Append(this.GetStatementName());
                        list.Add(builder2.ToString());
                    }
                }
            }
            return list;
        }

        public QNameManager.QName GetSchemaName()
        {
            return null;
        }

        public int GetSchemaObjectType()
        {
            return 11;
        }

        public virtual string GetSql()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("CREATE").Append(' ').Append("ROLE");
            builder1.Append(' ').Append(this.GranteeName.StatementName);
            return builder1.ToString();
        }

        public string GetStatementName()
        {
            return this.GranteeName.StatementName;
        }

        public void Grant(Grantee role)
        {
            this.Roles.Add(role);
        }

        public void Grant(QNameManager.QName name, Right right, Grantee grantor, bool withGrant)
        {
            Right allGrantableRights = grantor.GetAllGrantableRights(name);
            Right right3 = null;
            if (right == Right.FullRights)
            {
                if (allGrantableRights.IsEmpty())
                {
                    return;
                }
                right = allGrantableRights;
            }
            else if (!allGrantableRights.Contains(right))
            {
                throw Error.GetError(0x7d0);
            }
            Iterator<Right> iterator = this._directRightsMap.Get(name);
            while (iterator.HasNext())
            {
                Right right4 = iterator.Next();
                if (right4.Grantor == grantor)
                {
                    right3 = right4;
                    right3.Add(right);
                    break;
                }
            }
            if (right3 == null)
            {
                right3 = right.Duplicate();
                right3.Grantor = grantor;
                right3.grantee = this;
                this._directRightsMap.Put(name, right3);
            }
            if (withGrant)
            {
                if (right3.GrantableRights == null)
                {
                    right3.GrantableRights = right.Duplicate();
                }
                else
                {
                    right3.GrantableRights.Add(right);
                }
            }
            if (!grantor._isSystem)
            {
                grantor._grantedRightsMap.Put(name, right3);
            }
            this.UpdateAllRights();
        }

        public bool HasNonSelectTableRight(Table table)
        {
            if (this.IsFullyAccessibleByRole(table.GetName()))
            {
                return true;
            }
            Right right = this._fullRightsMap.Get(table.GetName());
            return ((right != null) && ((((right.IsFull || right.IsFullDelete) || (right.IsFullInsert || right.IsFullUpdate)) || right.IsFullReferences) || right.IsFullTrigger));
        }

        public bool HasRole(Grantee role)
        {
            return this.GetAllRoles().Contains(role);
        }

        public bool HasRoleDirect(Grantee role)
        {
            return this.Roles.Contains(role);
        }

        public bool HasSchemaUpdateOrGrantRights(string schemaName)
        {
            if (this.IsAdmin())
            {
                return true;
            }
            Grantee role = this.granteeManager.database.schemaManager.ToSchemaOwner(schemaName);
            return ((role == this) || this.HasRole(role));
        }

        public bool IsAccessible(ISchemaObject obj)
        {
            return this.IsAccessible(obj.GetName());
        }

        public bool IsAccessible(QNameManager.QName name)
        {
            if (this.IsFullyAccessibleByRole(name))
            {
                return true;
            }
            Right right = this._fullRightsMap.Get(name);
            return (((right != null) && !right.IsEmpty()) || (!this._isPublic && this.granteeManager.PublicRole.IsAccessible(name)));
        }

        public bool IsAdmin()
        {
            return this._isAdmin;
        }

        public bool IsFullyAccessibleByRole(QNameManager.QName name)
        {
            if (this._isAdmin)
            {
                return true;
            }
            if (name.schema == null)
            {
                return false;
            }
            Grantee owner = name.schema.Owner;
            return ((owner == this) || this.HasRole(owner));
        }

        public bool IsGrantable(Grantee role)
        {
            return this._isAdmin;
        }

        public bool IsGrantable(ISchemaObject obj, Right right)
        {
            return (this.IsFullyAccessibleByRole(obj.GetName()) || this.GetAllGrantableRights(obj.GetName()).Contains(right));
        }

        public bool IsSchemaCreator()
        {
            if (!this._isAdmin)
            {
                return this.HasRole(this.granteeManager.SchemaRole);
            }
            return true;
        }

        public void Revoke(Grantee role)
        {
            if (!this.HasRoleDirect(role))
            {
                throw Error.GetError(0x8cd, role.GetNameString());
            }
            this.Roles.Remove(role);
        }

        public void Revoke(ISchemaObject obj, Right right, Grantee grantor, bool grantOption)
        {
            QNameManager.QName key = obj.GetName();
            Routine routine = obj as Routine;
            if (routine != null)
            {
                key = routine.GetSpecificName();
            }
            Iterator<Right> iterator = this._directRightsMap.Get(key);
            Right right2 = null;
            while (iterator.HasNext())
            {
                right2 = iterator.Next();
                if (right2.Grantor == grantor)
                {
                    break;
                }
            }
            if (right2 != null)
            {
                if (right2.GrantableRights != null)
                {
                    right2.GrantableRights.Remove(obj, right);
                }
                if (!grantOption)
                {
                    if (right.IsFull)
                    {
                        this._directRightsMap.Remove(key, right2);
                        grantor._grantedRightsMap.Remove(key, right2);
                        this.UpdateAllRights();
                    }
                    else
                    {
                        right2.Remove(obj, right);
                        if (right2.IsEmpty())
                        {
                            this._directRightsMap.Remove(key, right2);
                            grantor._grantedRightsMap.Remove(key, right2);
                        }
                        this.UpdateAllRights();
                    }
                }
            }
        }

        public void RevokeDbObject(QNameManager.QName name)
        {
            this._directRightsMap.Remove(name);
            this._grantedRightsMap.Remove(name);
            this._fullRightsMap.Remove(name);
        }

        public static string RoleMapToString(OrderedHashSet<Grantee> roles)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < roles.Size(); i++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(',');
                }
                builder.Append(roles.Get(i).GetStatementName());
            }
            return builder.ToString();
        }

        public void SetAdminDirect()
        {
            this._isAdmin = this._isAdminDirect = true;
        }

        public void UpdateAllRights()
        {
            this._fullRightsMap.Clear();
            this._isAdmin = this._isAdminDirect;
            for (int i = 0; i < this.Roles.Size(); i++)
            {
                Grantee grantee = this.Roles.Get(i);
                this.AddToFullRights(grantee._fullRightsMap);
                this._isAdmin |= grantee.IsAdmin();
            }
            this.AddToFullRights(this._directRightsMap);
            if ((!this._isRole && !this._isPublic) && !this._isSystem)
            {
                this.AddToFullRights(this.granteeManager.PublicRole._fullRightsMap);
            }
        }

        public bool UpdateNestedRoles(Grantee role)
        {
            bool flag = false;
            if (role != this)
            {
                for (int i = 0; i < this.Roles.Size(); i++)
                {
                    Grantee grantee = this.Roles.Get(i);
                    flag |= grantee.UpdateNestedRoles(role);
                }
            }
            if (flag)
            {
                this.UpdateAllRights();
            }
            if (!flag)
            {
                return (role == this);
            }
            return true;
        }

        public HashSet<Grantee> VisibleGrantees()
        {
            HashSet<Grantee> set = new HashSet<Grantee>();
            GranteeManager granteeManager = this.granteeManager;
            if (this.IsAdmin())
            {
                Iterator<Grantee> iterator = granteeManager.GetGrantees().GetIterator();
                while (iterator.HasNext())
                {
                    set.Add(iterator.Next());
                }
                return set;
            }
            set.Add(this);
            Iterator<Grantee> iterator2 = this.GetAllRoles().GetIterator();
            while (iterator2.HasNext())
            {
                set.Add(iterator2.Next());
            }
            return set;
        }

        public bool IsRole
        {
            get
            {
                return this._isRole;
            }
            set
            {
                this._isRole = value;
            }
        }

        public bool IsSystem
        {
            get
            {
                return this._isSystem;
            }
            set
            {
                this._isSystem = value;
            }
        }

        public bool IsPublic
        {
            get
            {
                return this._isPublic;
            }
            set
            {
                this._isPublic = value;
            }
        }
    }
}


namespace FwNs.Core.LC.cRights
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;

    public sealed class GranteeManager : GrantConstants
    {
        public static User SystemAuthorisation = GetSystemAuthorisation();
        private readonly HashMappedList<string, Grantee> _map = new HashMappedList<string, Grantee>();
        private readonly HashMappedList<string, Grantee> _roleMap = new HashMappedList<string, Grantee>();
        public Database database;
        public Grantee PublicRole;
        public Grantee DbaRole;
        public Grantee SchemaRole;
        public Grantee ChangeAuthRole;
        private static readonly Dictionary<string, int> RightsStringLookup = GetRightsStringLookup();

        public GranteeManager(Database database)
        {
            this.database = database;
            this.AddRole(this.database.NameManager.NewQName("PUBLIC", false, 11));
            this.PublicRole = this.GetRole("PUBLIC");
            this.PublicRole.IsPublic = true;
            this.AddRole(this.database.NameManager.NewQName("DBA", false, 11));
            this.DbaRole = this.GetRole("DBA");
            this.DbaRole.SetAdminDirect();
            this.AddRole(this.database.NameManager.NewQName("CREATE_SCHEMA", false, 11));
            this.SchemaRole = this.GetRole("CREATE_SCHEMA");
            this.AddRole(this.database.NameManager.NewQName("CHANGE_AUTHORIZATION", false, 11));
            this.ChangeAuthRole = this.GetRole("CHANGE_AUTHORIZATION");
        }

        public Grantee AddRole(QNameManager.QName name)
        {
            if (this._map.ContainsKey(name.Name))
            {
                throw Error.GetError(0xfa3, name.Name);
            }
            Grantee grantee = new Grantee(name, this) {
                IsRole = true
            };
            this._map.Put(name.Name, grantee);
            this._roleMap.Add(name.Name, grantee);
            return grantee;
        }

        public User AddUser(QNameManager.QName name)
        {
            if (this._map.ContainsKey(name.Name))
            {
                throw Error.GetError(0xfa3, name.Name);
            }
            User user = new User(name, this);
            this._map.Put(name.Name, user);
            return user;
        }

        public void CheckGranteeList(OrderedHashSet<string> granteeList)
        {
            for (int i = 0; i < granteeList.Size(); i++)
            {
                string name = granteeList.Get(i);
                if (this.Get(name) == null)
                {
                    throw Error.GetError(0xfa1, name);
                }
                if (IsImmutable(name))
                {
                    throw Error.GetError(0xfa2, name);
                }
            }
        }

        public void CheckRoleList(string granteeName, OrderedHashSet<string> roleList, Grantee grantor, bool grant)
        {
            Grantee grantee = this.Get(granteeName);
            for (int i = 0; i < roleList.Size(); i++)
            {
                string name = roleList.Get(i);
                Grantee role = this.GetRole(name);
                if (role == null)
                {
                    throw Error.GetError(0x898, name);
                }
                if (name.Equals("SYS") || name.Equals("PUBLIC"))
                {
                    throw Error.GetError(0xfa2, name);
                }
                if (grant)
                {
                    if (grantee.GetDirectRoles().Contains(role))
                    {
                        throw Error.GetError(0x898, granteeName);
                    }
                }
                else if (!grantee.GetDirectRoles().Contains(role))
                {
                    throw Error.GetError(0x898, name);
                }
                if (!grantor.IsAdmin())
                {
                    throw Error.GetError(0x7d0, grantor.GetNameString());
                }
            }
        }

        public void DropRole(string name)
        {
            if (!this.IsRole(name))
            {
                throw Error.GetError(0x898, name);
            }
            if (IsReserved(name))
            {
                throw Error.GetError(0x1583);
            }
            this.RemoveGrantee(name);
        }

        public Grantee Get(string name)
        {
            return this._map.Get(name);
        }

        public static int GetCheckSingleRight(string right)
        {
            int num = GetRight(right);
            if (num == 0)
            {
                throw Error.GetError(0x15cd, right);
            }
            return num;
        }

        public Grantee GetDBARole()
        {
            return this.DbaRole;
        }

        public FwNs.Core.LC.cLib.ICollection<Grantee> GetGrantees()
        {
            return this._map.GetValues();
        }

        public static int GetRight(string right)
        {
            int num;
            if (RightsStringLookup.TryGetValue(right, out num))
            {
                return num;
            }
            return 0;
        }

        private static Dictionary<string, int> GetRightsStringLookup()
        {
            return new Dictionary<string, int>(7) { 
                { 
                    "ALL",
                    0x3f
                },
                { 
                    "SELECT",
                    1
                },
                { 
                    "UPDATE",
                    8
                },
                { 
                    "DELETE",
                    2
                },
                { 
                    "INSERT",
                    4
                },
                { 
                    "EXECUTE",
                    0x20
                },
                { 
                    "USAGE",
                    0x10
                },
                { 
                    "REFERENCES",
                    0x40
                },
                { 
                    "TRIGGER",
                    0x80
                }
            };
        }

        public string[] GetRightstSQL()
        {
            List<string> list = new List<string>();
            Iterator<Grantee> iterator = this.GetGrantees().GetIterator();
            while (iterator.HasNext())
            {
                Grantee grantee = iterator.Next();
                if (!IsImmutable(grantee.GetNameString()))
                {
                    List<string> rightsSQL = grantee.GetRightsSQL();
                    list.AddRange(rightsSQL);
                }
            }
            return list.ToArray();
        }

        public Grantee GetRole(string name)
        {
            Grantee local1 = this._roleMap.Get(name);
            if (local1 == null)
            {
                throw Error.GetError(0x898, name);
            }
            return local1;
        }

        public FwNs.Core.LC.cLib.ISet<string> GetRoleNames()
        {
            return this._roleMap.GetKeySet();
        }

        public FwNs.Core.LC.cLib.ICollection<Grantee> GetRoles()
        {
            return this._roleMap.GetValues();
        }

        public string[] GetSQL()
        {
            List<string> list = new List<string>();
            Iterator<Grantee> iterator = this.GetRoles().GetIterator();
            while (iterator.HasNext())
            {
                Grantee grantee = iterator.Next();
                if (!IsReserved(grantee.GetNameString()))
                {
                    list.Add(grantee.GetSql());
                }
            }
            iterator = this.GetGrantees().GetIterator();
            while (iterator.HasNext())
            {
                Grantee grantee2 = iterator.Next();
                if (grantee2 is User)
                {
                    list.Add(grantee2.GetSql());
                }
            }
            return list.ToArray();
        }

        private static User GetSystemAuthorisation()
        {
            User user = new User(QNameManager.NewSystemObjectName("SYS", 11), null) {
                IsSystem = true
            };
            user.SetAdminDirect();
            user.SetInitialSchema(SqlInvariants.SystemSchemaQname);
            SqlInvariants.InformationSchemaQname.Owner = user;
            SqlInvariants.SystemSchemaQname.Owner = user;
            SqlInvariants.LobsSchemaQname.Owner = user;
            SqlInvariants.SqljSchemaQname.Owner = user;
            return user;
        }

        public static Grantee GetSystemRole()
        {
            return SystemAuthorisation;
        }

        public void Grant(string granteeName, string roleName, Grantee grantor)
        {
            Grantee role = this.Get(granteeName);
            if (role == null)
            {
                throw Error.GetError(0xfa1, granteeName);
            }
            if (IsImmutable(granteeName))
            {
                throw Error.GetError(0xfa2, granteeName);
            }
            Grantee grantee2 = this.GetRole(roleName);
            if (grantee2 == null)
            {
                throw Error.GetError(0x898, roleName);
            }
            if (grantee2 == role)
            {
                throw Error.GetError(0x8cb, granteeName);
            }
            if (grantee2.HasRole(role))
            {
                throw Error.GetError(0x8cb, roleName);
            }
            if (!grantor.IsGrantable(grantee2))
            {
                throw Error.GetError(0x7d0, grantor.GetNameString());
            }
            role.Grant(grantee2);
            role.UpdateAllRights();
            if (role.IsRole)
            {
                this.UpdateAllRights(role);
            }
        }

        public void Grant(OrderedHashSet<string> granteeList, ISchemaObject dbObject, Right right, Grantee grantor, bool withGrantOption)
        {
            RoutineSchema schema = dbObject as RoutineSchema;
            if (schema != null)
            {
                ISchemaObject[] specificRoutines = schema.GetSpecificRoutines();
                this.Grant(granteeList, specificRoutines, right, grantor, withGrantOption);
            }
            else
            {
                QNameManager.QName specificName = dbObject.GetName();
                Routine routine = dbObject as Routine;
                if (routine != null)
                {
                    specificName = routine.GetSpecificName();
                }
                if (!grantor.IsGrantable(dbObject, right))
                {
                    throw Error.GetError(0x7d0, grantor.GetNameString());
                }
                if (grantor.IsAdmin())
                {
                    grantor = dbObject.GetOwner();
                }
                this.CheckGranteeList(granteeList);
                for (int i = 0; i < granteeList.Size(); i++)
                {
                    Grantee role = this.Get(granteeList.Get(i));
                    role.Grant(specificName, right, grantor, withGrantOption);
                    if (role.IsRole)
                    {
                        this.UpdateAllRights(role);
                    }
                }
            }
        }

        public void Grant(OrderedHashSet<string> granteeList, ISchemaObject[] routines, Right right, Grantee grantor, bool withGrantOption)
        {
            bool flag = false;
            for (int i = 0; i < routines.Length; i++)
            {
                if (grantor.IsGrantable(routines[i], right))
                {
                    this.Grant(granteeList, routines[i], right, grantor, withGrantOption);
                    flag = true;
                }
            }
            if (!flag)
            {
                throw Error.GetError(0x7d0, grantor.GetNameString());
            }
        }

        public void GrantSystemToPublic(ISchemaObject obj, Right right)
        {
            this.PublicRole.Grant(obj.GetName(), right, SystemAuthorisation, true);
        }

        public static bool IsImmutable(string name)
        {
            if ((!name.Equals("SYS") && !name.Equals("DBA")) && !name.Equals("CREATE_SCHEMA"))
            {
                return name.Equals("CHANGE_AUTHORIZATION");
            }
            return true;
        }

        public static bool IsReserved(string name)
        {
            if ((!name.Equals("SYS") && !name.Equals("DBA")) && (!name.Equals("CREATE_SCHEMA") && !name.Equals("CHANGE_AUTHORIZATION")))
            {
                return name.Equals("PUBLIC");
            }
            return true;
        }

        public bool IsRole(string name)
        {
            return this._roleMap.ContainsKey(name);
        }

        public void RemoveDbObject(QNameManager.QName name)
        {
            for (int i = 0; i < this._map.Size(); i++)
            {
                this._map.Get(i).RevokeDbObject(name);
            }
        }

        private void RemoveEmptyRole(Grantee role)
        {
            for (int i = 0; i < this._map.Size(); i++)
            {
                this._map.Get(i).Roles.Remove(role);
            }
        }

        public bool RemoveGrantee(string name)
        {
            if (IsReserved(name))
            {
                return false;
            }
            Grantee role = this._map.Remove(name);
            if (role == null)
            {
                return false;
            }
            role.ClearPrivileges();
            this.UpdateAllRights(role);
            if (role.IsRole)
            {
                this._roleMap.Remove(name);
                this.RemoveEmptyRole(role);
            }
            return true;
        }

        public void Revoke(string granteeName, string roleName, Grantee grantor)
        {
            if (!grantor.IsAdmin())
            {
                throw Error.GetError(0x1583);
            }
            Grantee role = this.Get(granteeName);
            if (role == null)
            {
                throw Error.GetError(0xfa0, granteeName);
            }
            Grantee grantee2 = this._roleMap.Get(roleName);
            role.Revoke(grantee2);
            role.UpdateAllRights();
            if (role.IsRole)
            {
                this.UpdateAllRights(role);
            }
        }

        public void Revoke(OrderedHashSet<string> granteeList, ISchemaObject dbObject, Right rights, Grantee grantor, bool grantOption, bool cascade)
        {
            RoutineSchema schema = dbObject as RoutineSchema;
            if (schema != null)
            {
                ISchemaObject[] specificRoutines = schema.GetSpecificRoutines();
                this.Revoke(granteeList, specificRoutines, rights, grantor, grantOption, cascade);
            }
            else
            {
                QNameManager.QName specificName = dbObject.GetName();
                Routine routine = dbObject as Routine;
                if (routine != null)
                {
                    specificName = routine.GetSpecificName();
                }
                if (!grantor.IsFullyAccessibleByRole(specificName))
                {
                    throw Error.GetError(0x157d, dbObject.GetName().Name);
                }
                if (grantor.IsAdmin())
                {
                    grantor = dbObject.GetOwner();
                }
                for (int i = 0; i < granteeList.Size(); i++)
                {
                    string str = granteeList.Get(i);
                    if (this.Get(str) == null)
                    {
                        throw Error.GetError(0xfa1, str);
                    }
                    if (IsImmutable(str))
                    {
                        throw Error.GetError(0xfa2, str);
                    }
                }
                for (int j = 0; j < granteeList.Size(); j++)
                {
                    string str2 = granteeList.Get(j);
                    Grantee role = this.Get(str2);
                    role.Revoke(dbObject, rights, grantor, grantOption);
                    role.UpdateAllRights();
                    if (role.IsRole)
                    {
                        this.UpdateAllRights(role);
                    }
                }
            }
        }

        public void Revoke(OrderedHashSet<string> granteeList, ISchemaObject[] routines, Right rights, Grantee grantor, bool grantOption, bool cascade)
        {
            for (int i = 0; i < routines.Length; i++)
            {
                this.Revoke(granteeList, routines[i], rights, grantor, grantOption, cascade);
            }
        }

        private void UpdateAllRights(Grantee role)
        {
            for (int i = 0; i < this._map.Size(); i++)
            {
                Grantee grantee = this._map.Get(i);
                if (grantee.IsRole)
                {
                    grantee.UpdateNestedRoles(role);
                }
            }
            for (int j = 0; j < this._map.Size(); j++)
            {
                Grantee grantee2 = this._map.Get(j);
                if (!grantee2.IsRole)
                {
                    grantee2.UpdateAllRights();
                }
            }
        }

        public static bool ValidRightString(string rightString)
        {
            return (GetRight(rightString) > 0);
        }
    }
}


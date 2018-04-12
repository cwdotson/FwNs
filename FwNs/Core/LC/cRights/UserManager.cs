namespace FwNs.Core.LC.cRights
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;

    public sealed class UserManager
    {
        private readonly HashMappedList<string, User> _userList;
        private readonly GranteeManager _granteeManager;

        public UserManager(Database database)
        {
            this._granteeManager = database.GetGranteeManager();
            this._userList = new HashMappedList<string, User>();
        }

        public void CreateFirstUser(string username, string password)
        {
            bool isquoted = true;
            if (username.Equals("SA", StringComparison.OrdinalIgnoreCase))
            {
                username = "SA";
                isquoted = false;
            }
            QNameManager.QName name = this._granteeManager.database.NameManager.NewQName(username, isquoted, 11);
            this.CreateUser(name, password);
            this._granteeManager.Grant(name.Name, "DBA", this._granteeManager.GetDBARole());
        }

        public User CreateUser(QNameManager.QName name, string password)
        {
            User user = this._granteeManager.AddUser(name);
            user.SetPassword(password);
            if (!this._userList.Add(name.Name, user))
            {
                throw Error.GetError(0xfa3, name.StatementName);
            }
            return user;
        }

        public void DropUser(string name)
        {
            if (GranteeManager.IsReserved(name))
            {
                throw Error.GetError(0xfa2, name);
            }
            if (!this._granteeManager.RemoveGrantee(name))
            {
                throw Error.GetError(0xfa1, name);
            }
            if (this._userList.Remove(name) == null)
            {
                throw Error.GetError(0xfa1, name);
            }
        }

        public bool Exists(string name)
        {
            return (this._userList.Get(name) > null);
        }

        public User Get(string name)
        {
            User local1 = this._userList.Get(name);
            if (local1 == null)
            {
                throw Error.GetError(0xfa1, name);
            }
            return local1;
        }

        public string[] GetInitialSchemaSQL()
        {
            List<string> list = new List<string>(this._userList.Size());
            for (int i = 0; i < this._userList.Size(); i++)
            {
                User user = this._userList.Get(i);
                if (!user.IsSystem && (user.GetInitialSchema() != null))
                {
                    list.Add(user.GetInitialSchemaSQL());
                }
            }
            return list.ToArray();
        }

        public static User GetSysUser()
        {
            return GranteeManager.SystemAuthorisation;
        }

        public User GetUser(string name, string password)
        {
            if (name == null)
            {
                name = "";
            }
            if (password == null)
            {
                password = "";
            }
            User user1 = this.Get(name);
            user1.CheckPassword(password);
            return user1;
        }

        public HashMappedList<string, User> GetUsers()
        {
            return this._userList;
        }

        public List<User> ListVisibleUsers(Session session)
        {
            List<User> list = new List<User>();
            bool flag = session.IsAdmin();
            string username = session.GetUsername();
            if ((this._userList != null) && (this._userList.Size() != 0))
            {
                for (int i = 0; i < this._userList.Size(); i++)
                {
                    User item = this._userList.Get(i);
                    if (item != null)
                    {
                        string nameString = item.GetNameString();
                        if (flag)
                        {
                            list.Add(item);
                        }
                        else if (username.Equals(nameString))
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public void RemoveSchemaReference(string schemaName)
        {
            lock (this)
            {
                for (int i = 0; i < this._userList.Size(); i++)
                {
                    User user = this._userList.Get(i);
                    QNameManager.QName initialSchema = user.GetInitialSchema();
                    if ((initialSchema != null) && schemaName.Equals(initialSchema.Name))
                    {
                        user.SetInitialSchema(null);
                    }
                }
            }
        }
    }
}


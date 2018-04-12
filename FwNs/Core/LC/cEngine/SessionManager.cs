namespace FwNs.Core.LC.cEngine
{
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;

    public sealed class SessionManager : IDisposable
    {
        private int _sessionIdCount = 1;
        private readonly Dictionary<long, Session> _sessionMap = new Dictionary<long, Session>();
        private readonly Session _sysSession;
        private readonly Session _sysLobSession;

        public SessionManager(Database db)
        {
            User sysUser = UserManager.GetSysUser();
            int num = this._sessionIdCount;
            this._sessionIdCount = num + 1;
            this._sysSession = new Session(db, sysUser, false, false, (long) num, null, 0);
            num = this._sessionIdCount;
            this._sessionIdCount = num + 1;
            this._sysLobSession = new Session(db, sysUser, true, false, (long) num, null, 0);
        }

        public void ClearAll()
        {
            lock (this)
            {
                this._sessionMap.Clear();
            }
        }

        public void CloseAllSessions()
        {
            lock (this)
            {
                Session[] allSessions = this.GetAllSessions();
                for (int i = 0; i < allSessions.Length; i++)
                {
                    allSessions[i].Close();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._sysLobSession.Dispose();
                this._sysSession.Dispose();
                using (Dictionary<long, Session>.ValueCollection.Enumerator enumerator = this._sessionMap.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Dispose();
                    }
                }
            }
        }

        public Session[] GetAllSessions()
        {
            lock (this)
            {
                Session[] sessionArray2 = new Session[this._sessionMap.Count];
                int num = 0;
                foreach (Session session in this._sessionMap.Values)
                {
                    sessionArray2[num++] = session;
                }
                return sessionArray2;
            }
        }

        public Session GetSession(long id)
        {
            lock (this)
            {
                Session session2;
                if (this._sessionMap.TryGetValue(id, out session2))
                {
                    return session2;
                }
                return null;
            }
        }

        public Session GetSysLobSession()
        {
            return this._sysLobSession;
        }

        public Session GetSysSession()
        {
            this._sysSession.CurrentSchema = this._sysSession.database.schemaManager.GetDefaultSchemaQName();
            this._sysSession.SetProcessingScript(false);
            this._sysSession.SetProcessingLog(false);
            this._sysSession.SetUser(UserManager.GetSysUser());
            return this._sysSession;
        }

        public Session GetSysSession(string schema, User user)
        {
            this._sysSession.CurrentSchema = this._sysSession.database.schemaManager.GetSchemaQName(schema);
            this._sysSession.SetProcessingScript(false);
            this._sysSession.SetProcessingLog(false);
            this._sysSession.SetUser(user);
            return this._sysSession;
        }

        public Session GetSysSession(string schema, bool forScript)
        {
            this._sysSession.CurrentSchema = this._sysSession.database.schemaManager.GetDefaultSchemaQName();
            this._sysSession.SetProcessingScript(forScript);
            this._sysSession.SetProcessingLog(false);
            this._sysSession.SetUser(UserManager.GetSysUser());
            return this._sysSession;
        }

        public static Session GetSysSessionForScript(Database db)
        {
            Session session1 = new Session(db, UserManager.GetSysUser(), false, false, 0L, null, 0);
            session1.SetProcessingScript(true);
            return session1;
        }

        public Session[] GetVisibleSessions(Session session)
        {
            lock (this)
            {
                return (session.IsAdmin() ? this.GetAllSessions() : new Session[] { session });
            }
        }

        public bool IsEmpty()
        {
            lock (this)
            {
                return (this._sessionMap.Count == 0);
            }
        }

        public bool IsUserActive(string userName)
        {
            lock (this)
            {
                foreach (Session session in this._sessionMap.Values)
                {
                    if (userName.Equals(session.GetUser().GetNameString()))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Session NewSession(Database db, User user, bool rdy, bool forLog, string zoneString, int timeZoneSeconds)
        {
            Session session = new Session(db, user, !forLog, rdy, (long) this._sessionIdCount, zoneString, timeZoneSeconds);
            session.SetProcessingLog(forLog);
            this._sessionMap.Add((long) this._sessionIdCount, session);
            this._sessionIdCount++;
            return session;
        }

        public Session NewSysSession(QNameManager.QName schema, User user)
        {
            return new Session(this._sysSession.database, user, false, false, 0L, null, 0) { CurrentSchema = schema };
        }

        public void RemoveSchemaReference(Schema schema)
        {
            lock (this)
            {
                foreach (Session session in this._sessionMap.Values)
                {
                    if (session.GetCurrentSchemaQName() == schema.GetName())
                    {
                        session.ResetSchema();
                    }
                }
            }
        }

        public void RemoveSession(Session session)
        {
            lock (this)
            {
                this._sessionMap.Remove(session.GetId());
            }
        }

        public void ResetLoggedSchemas()
        {
            lock (this)
            {
                using (Dictionary<long, Session>.ValueCollection.Enumerator enumerator = this._sessionMap.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.LoggedSchema = null;
                    }
                }
                this._sysLobSession.LoggedSchema = null;
            }
        }
    }
}


namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Collections.Generic;

    public sealed class StatementManager
    {
        private readonly Dictionary<long, Statement> _csidMap;
        private readonly Database _database;
        private readonly Dictionary<string, long> _nameLookup;
        private readonly Dictionary<int, Dictionary<string, long>> _schemaMap;
        private readonly Dictionary<long, string> _sqlLookup;
        private long _nextCsId;

        public StatementManager(Database database)
        {
            this._database = database;
            this._schemaMap = new Dictionary<int, Dictionary<string, long>>();
            this._sqlLookup = new Dictionary<long, string>();
            this._csidMap = new Dictionary<long, Statement>();
            this._nameLookup = new Dictionary<string, long>();
            this._nextCsId = 0L;
        }

        public Statement Compile(Session session, Result cmd)
        {
            lock (this)
            {
                Statement statement2;
                string mainString = cmd.GetMainString();
                long statementId = this.GetStatementId(session.CurrentSchema, mainString);
                if ((!this._csidMap.TryGetValue(statementId, out statement2) || !statement2.IsValid()) || (statement2.GetCompileTimestamp() < this._database.schemaManager.GetSchemaChangeTimestamp()))
                {
                    int executeProperties = cmd.GetExecuteProperties();
                    statement2 = session.CompileStatement(mainString, executeProperties);
                    statementId = this.RegisterStatement(statementId, statement2);
                }
                statement2.SetGeneratedColumnInfo(cmd.GetGeneratedResultType(), cmd.GetGeneratedResultMetaData());
                return statement2;
            }
        }

        public Statement Compile(Session session, string name, string sql, int props)
        {
            lock (this)
            {
                long statementId;
                Statement statement2;
                if (this._nameLookup.TryGetValue(name, out statementId))
                {
                    this.FreeStatement(statementId);
                    this._nameLookup.Remove(name);
                }
                statementId = this.GetStatementId(session.CurrentSchema, sql);
                if ((!this._csidMap.TryGetValue(statementId, out statement2) || !statement2.IsValid()) || (statement2.GetCompileTimestamp() < this._database.schemaManager.GetSchemaChangeTimestamp()))
                {
                    statement2 = session.CompileStatement(sql, props);
                    statementId = this.RegisterStatement(statementId, statement2);
                }
                this._nameLookup[name] = statementId;
                return statement2;
            }
        }

        public void FreeStatement(long csid)
        {
            lock (this)
            {
                Statement statement;
                if ((csid != -1L) && this._csidMap.TryGetValue(csid, out statement))
                {
                    this._csidMap.Remove(csid);
                    int hashCode = statement.GetSchemaName().GetHashCode();
                    string key = this._sqlLookup[csid];
                    this._sqlLookup.Remove(csid);
                    this._schemaMap[hashCode].Remove(key);
                }
            }
        }

        public void FreeStatement(string name)
        {
            lock (this)
            {
                long num;
                if (this._nameLookup.TryGetValue(name, out num))
                {
                    this.FreeStatement(num);
                    this._nameLookup.Remove(name);
                }
            }
        }

        public Statement GetStatement(Session session, long csid)
        {
            lock (this)
            {
                Statement statement2;
                if (!this._csidMap.TryGetValue(csid, out statement2))
                {
                    return null;
                }
                if (statement2.GetCompileTimestamp() < this._database.schemaManager.GetSchemaChangeTimestamp())
                {
                    string sql = this._sqlLookup[csid];
                    QNameManager.QName currentSchemaQName = session.GetCurrentSchemaQName();
                    try
                    {
                        QNameManager.QName schemaName = statement2.GetSchemaName();
                        session.SetSchema(schemaName.Name);
                        StatementInsert insert = null;
                        if (statement2.GeneratedResultMetaData() != null)
                        {
                            insert = (StatementInsert) statement2;
                        }
                        statement2 = session.CompileStatement(sql, statement2.GetResultProperties());
                        statement2.SetId(csid);
                        statement2.SetCompileTimestamp(this._database.TxManager.GetGlobalChangeTimestamp());
                        if (insert != null)
                        {
                            statement2.SetGeneratedColumnInfo(insert.GeneratedType, insert.GeneratedInputMetaData);
                        }
                        this._csidMap[csid] = statement2;
                    }
                    catch (Exception)
                    {
                        this.FreeStatement(csid);
                        return null;
                    }
                    finally
                    {
                        session.SetSchema(currentSchemaQName.Name);
                    }
                }
                return statement2;
            }
        }

        public Statement GetStatement(Session session, string name)
        {
            lock (this)
            {
                long num;
                Statement statement = null;
                if (this._nameLookup.TryGetValue(name, out num))
                {
                    statement = this.GetStatement(session, num);
                    if (statement == null)
                    {
                        this._nameLookup.Remove(name);
                    }
                }
                return statement;
            }
        }

        private long GetStatementId(QNameManager.QName schema, string sql)
        {
            Dictionary<string, long> dictionary;
            long num2;
            if (this._schemaMap.TryGetValue(schema.GetHashCode(), out dictionary) && dictionary.TryGetValue(sql, out num2))
            {
                return num2;
            }
            return -1L;
        }

        private long NextId()
        {
            this._nextCsId += 1L;
            return this._nextCsId;
        }

        private long RegisterStatement(long csid, Statement cs)
        {
            if (csid < 0L)
            {
                Dictionary<string, long> dictionary;
                csid = this.NextId();
                int hashCode = cs.GetSchemaName().GetHashCode();
                if (!this._schemaMap.TryGetValue(hashCode, out dictionary))
                {
                    dictionary = new Dictionary<string, long>();
                    this._schemaMap.Add(hashCode, dictionary);
                }
                dictionary[cs.GetSql()] = csid;
                this._sqlLookup.Add(csid, cs.GetSql());
            }
            cs.SetId(csid);
            cs.SetCompileTimestamp(this._database.TxManager.GetGlobalChangeTimestamp());
            this._csidMap[csid] = cs;
            return csid;
        }

        public void Reset()
        {
            lock (this)
            {
                this._schemaMap.Clear();
                this._sqlLookup.Clear();
                this._csidMap.Clear();
                this._nextCsId = 0L;
            }
        }
    }
}


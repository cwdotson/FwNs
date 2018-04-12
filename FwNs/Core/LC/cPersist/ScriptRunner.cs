namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cResources;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cScriptIO;
    using FwNs.Core.LC.cStatements;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class ScriptRunner
    {
        public static void RunScript(Database database, string logFilename)
        {
            Dictionary<int, Session> dictionary = new Dictionary<int, Session>();
            Session session = null;
            int key = 0;
            database.SetReferentialIntegrity(false);
            ScriptReaderBase base2 = null;
            Statement cs = new StatementDML(0x51, 0x7d4, null);
            try
            {
                Crypto crypto = database.logger.GetCrypto();
                if (crypto == null)
                {
                    base2 = new ScriptReaderText(database, logFilename);
                }
                else
                {
                    base2 = new ScriptReaderDecode(database, logFilename, crypto, true);
                }
                while (base2.ReadLoggedStatement(session))
                {
                    int sessionNumber = base2.GetSessionNumber();
                    if ((session == null) || (key != sessionNumber))
                    {
                        key = sessionNumber;
                        if (!dictionary.TryGetValue(key, out session))
                        {
                            session = database.GetSessionManager().NewSession(database, UserManager.GetSysUser(), false, true, null, 0);
                            dictionary.Add(key, session);
                        }
                    }
                    if (session.IsClosed())
                    {
                        dictionary.Remove(key);
                        continue;
                    }
                    switch (base2.GetStatementType())
                    {
                        case 1:
                        {
                            string loggedStatement = base2.GetLoggedStatement();
                            Result result = session.ExecuteDirectStatement(loggedStatement, ResultProperties.DefaultPropsValue);
                            if ((result != null) && result.IsError())
                            {
                                if (result.GetException() != null)
                                {
                                    throw result.GetException();
                                }
                                throw Error.GetError(result);
                            }
                            break;
                        }
                        case 2:
                        {
                            session.sessionContext.CurrentStatement = cs;
                            session.BeginAction(cs);
                            object[] data = base2.GetData();
                            base2.GetCurrentTable().DeleteNoCheckFromLog(session, data);
                            session.EndAction(Result.UpdateOneResult);
                            break;
                        }
                        case 3:
                        {
                            session.sessionContext.CurrentStatement = cs;
                            session.BeginAction(cs);
                            object[] data = base2.GetData();
                            base2.GetCurrentTable().InsertNoCheckFromLog(session, data);
                            session.EndAction(Result.UpdateOneResult);
                            break;
                        }
                        case 4:
                            session.Commit(false);
                            break;

                        case 6:
                        {
                            QNameManager.QName name = database.schemaManager.FindSchemaQName(base2.GetCurrentSchema());
                            session.SetCurrentSchemaQName(name);
                            break;
                        }
                    }
                    if (session.IsClosed())
                    {
                        dictionary.Remove(key);
                    }
                }
            }
            catch (Exception exception)
            {
                if (!(exception is EndOfStreamException))
                {
                    if (exception is OutOfMemoryException)
                    {
                        database.logger.LogSevereEvent(string.Concat(new object[] { FwNs.Core.LC.cResources.SR.ScriptRunner_RunScript_out_of_memory_processing_, logFilename, FwNs.Core.LC.cResources.SR.ScriptRunner_RunScript__line__, base2.GetLineNumber() }), exception);
                        throw Error.GetError(460);
                    }
                    database.logger.LogSevereEvent(logFilename + FwNs.Core.LC.cResources.SR.ScriptRunner_RunScript__line__ + base2.GetLineNumber(), exception);
                }
            }
            finally
            {
                if (base2 != null)
                {
                    base2.Close();
                }
                database.GetSessionManager().CloseAllSessions();
                database.SetReferentialIntegrity(true);
            }
        }
    }
}


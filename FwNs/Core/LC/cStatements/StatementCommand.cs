namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cScriptIO;
    using System;
    using System.Globalization;

    public sealed class StatementCommand : Statement
    {
        public Expression[] Expressions;
        public object[] Parameters;

        public StatementCommand(int type, object[] args) : this(type, args, null, null)
        {
        }

        public StatementCommand(int type, object[] args, QNameManager.QName readName, QNameManager.QName writeName) : base(type)
        {
            base.isTransactionStatement = true;
            this.Parameters = args;
            if ((readName != null) && (readName != writeName))
            {
                base.ReadTableNames = new QNameManager.QName[] { readName };
            }
            if (writeName != null)
            {
                base.WriteTableNames = new QNameManager.QName[] { writeName };
            }
            switch (type)
            {
                case 0x3e9:
                case 0x3ec:
                    base.Group = 0x7e0;
                    base.isLogged = false;
                    return;

                case 0x3ea:
                    base.Group = 0x7e0;
                    base.isLogged = false;
                    return;

                case 0x3eb:
                    base.isLogged = false;
                    base.Group = 0x7e0;
                    base.isTransactionStatement = false;
                    return;

                case 0x3f3:
                case 0x3f4:
                case 0x3f5:
                case 0x3f6:
                case 0x3f7:
                case 0x3f8:
                case 0x3f9:
                case 0x3fa:
                case 0x3fb:
                case 0x3fc:
                case 0x3ff:
                case 0x400:
                case 0x403:
                case 0x404:
                case 0x407:
                case 0x409:
                case 0x40b:
                case 0x40c:
                case 0x40d:
                case 0x7fc:
                case 0x7fe:
                case 0x7ff:
                case 0x801:
                    base.Group = 0x7dc;
                    base.isTransactionStatement = true;
                    return;

                case 0x402:
                case 0x800:
                    base.isTransactionStatement = false;
                    base.Group = 0x7dc;
                    return;

                case 0x412:
                case 0x413:
                    base.Group = 0x7dc;
                    base.isTransactionStatement = false;
                    return;

                case 0x42b:
                    base.Group = 0x7dc;
                    base.isTransactionStatement = false;
                    base.isLogged = false;
                    return;

                case 0x42c:
                    base.MetaDataImpact = 1;
                    base.Group = 0x7e1;
                    base.isTransactionStatement = true;
                    return;

                case 0x42f:
                    base.Group = 0x7e1;
                    base.isTransactionStatement = true;
                    return;
            }
            throw Error.RuntimeError(0xc9, "StatementCommand");
        }

        public override string Describe(Session session)
        {
            return base.Sql;
        }

        public override Result Execute(Session session)
        {
            Result result;
            try
            {
                result = this.GetResult(session);
            }
            catch (Exception exception1)
            {
                result = Result.NewErrorResult(exception1, null);
            }
            if (result.IsError())
            {
                result.GetException().SetStatementType(base.Group, base.type);
                return result;
            }
            try
            {
                if (base.isLogged)
                {
                    session.database.logger.WriteToLog(session, base.Sql);
                }
            }
            catch (Exception exception2)
            {
                return Result.NewErrorResult(exception2, base.Sql);
            }
            return result;
        }

        public Result GetResult(Session session)
        {
            bool flag;
            QNameManager.QName name;
            bool flag3;
            if (base.IsExplain)
            {
                return Result.NewSingleColumnStringResult("OPERATION", this.Describe(session));
            }
            int type = base.type;
            switch (type)
            {
                case 0x3e9:
                    return this.ProcessBackup(session);

                case 0x3ea:
                {
                    bool mode = Convert.ToBoolean(this.Parameters[0]);
                    try
                    {
                        session.database.logger.Checkpoint(mode);
                        return Result.UpdateZeroResult;
                    }
                    catch (CoreException exception1)
                    {
                        return Result.NewErrorResult(exception1, base.Sql);
                    }
                    break;
                }
                case 0x3eb:
                    break;

                case 0x3ec:
                {
                    string file = (string) this.Parameters[0];
                    if (file != null)
                    {
                        try
                        {
                            using (ScriptWriterText text = new ScriptWriterText(session.database, file, true, true, true))
                            {
                                text.WriteAll();
                            }
                        }
                        catch (CoreException exception9)
                        {
                            return Result.NewErrorResult(exception9, base.Sql);
                        }
                        return Result.UpdateZeroResult;
                    }
                    return session.database.GetScript(false);
                }
                case 0x3ed:
                case 0x3ee:
                case 0x3ef:
                case 0x3f0:
                case 0x3f1:
                case 0x3f2:
                case 0x3fd:
                case 0x3fe:
                case 0x400:
                case 0x401:
                case 0x405:
                case 0x406:
                case 0x408:
                case 0x40a:
                case 0x40e:
                case 0x40f:
                case 0x410:
                case 0x411:
                    goto Label_04D2;

                case 0x3f3:
                    goto Label_0160;

                case 0x3f4:
                {
                    int num4 = Convert.ToInt32(this.Parameters[0]);
                    session.database.schemaManager.SetDefaultTableType(num4);
                    return Result.UpdateZeroResult;
                }
                case 0x3f5:
                case 0x3f6:
                case 0x3f7:
                case 0x3f8:
                case 0x3f9:
                case 0x3fa:
                case 0x3fc:
                case 0x3ff:
                case 0x402:
                case 0x403:
                case 0x404:
                case 0x407:
                case 0x409:
                    goto Label_0400;

                case 0x3fb:
                    try
                    {
                        int megas = Convert.ToInt32(this.Parameters[0]);
                        session.CheckAdmin();
                        session.CheckDdlWrite();
                        session.database.logger.SetLogSize(megas);
                        return Result.UpdateZeroResult;
                    }
                    catch (CoreException exception3)
                    {
                        return Result.NewErrorResult(exception3, base.Sql);
                    }
                    goto Label_020A;

                case 0x40b:
                    goto Label_0451;

                case 0x40c:
                    goto Label_020A;

                case 0x40d:
                {
                    bool mode = Convert.ToBoolean(this.Parameters[0], CultureInfo.CurrentCulture);
                    session.database.SetStrictColumnSize(mode);
                    return Result.UpdateZeroResult;
                }
                case 0x412:
                    return this.ProcessSetUserInitialSchema(session);

                case 0x413:
                    try
                    {
                        string password = (string) this.Parameters[1];
                        session.CheckDdlWrite();
                        session.SetScripting(true);
                        ((this.Parameters[0] == null) ? session.GetUser() : ((User) this.Parameters[0])).SetPassword(password);
                        return Result.UpdateZeroResult;
                    }
                    catch (CoreException exception4)
                    {
                        return Result.NewErrorResult(exception4, base.Sql);
                    }
                    goto Label_02C1;

                default:
                    goto Label_02C1;
            }
            try
            {
                int closemode = Convert.ToInt32(this.Parameters[0]);
                session.database.Close(closemode);
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception2)
            {
                return Result.NewErrorResult(exception2, base.Sql);
            }
        Label_0160:
            name = (QNameManager.QName) this.Parameters[0];
            session.database.schemaManager.SetDefaultSchemaQName(name);
            session.database.schemaManager.SetSchemaChangeTimestamp();
            return Result.UpdateZeroResult;
        Label_020A:
            flag3 = Convert.ToBoolean(this.Parameters[0], CultureInfo.CurrentCulture);
            session.database.SetStrictNames(flag3);
            return Result.UpdateZeroResult;
        Label_02C1:
            switch (type)
            {
                case 0x42b:
                case 0x42c:
                case 0x42f:
                    goto Label_0400;

                case 0x42d:
                case 0x42e:
                    goto Label_04D2;

                default:
                    switch (type)
                    {
                        case 0x7fc:
                        {
                            bool mode = Convert.ToBoolean(this.Parameters[0], CultureInfo.CurrentCulture);
                            session.database.SetStrictReferences(mode);
                            return Result.UpdateZeroResult;
                        }
                        case 0x7fe:
                            try
                            {
                                int mode = Convert.ToInt32(this.Parameters[0]);
                                session.CheckAdmin();
                                session.CheckDdlWrite();
                                session.database.TxManager.SetTransactionControl(session, mode);
                                return Result.UpdateZeroResult;
                            }
                            catch (CoreException exception5)
                            {
                                return Result.NewErrorResult(exception5, base.Sql);
                            }
                            goto Label_037E;

                        case 0x7ff:
                        case 0x801:
                            goto Label_03B9;
                    }
                    goto Label_04D2;
            }
        Label_037E:;
            try
            {
                string str3 = (string) this.Parameters[0];
                session.database.SetUniqueName(str3);
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception6)
            {
                return Result.NewErrorResult(exception6, base.Sql);
            }
        Label_03B9:;
            try
            {
                int num7 = Convert.ToInt32(this.Parameters[0]);
                session.CheckAdmin();
                session.CheckDdlWrite();
                session.database.DefaultIsolationLevel = num7;
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception7)
            {
                return Result.NewErrorResult(exception7, base.Sql);
            }
        Label_0400:;
            try
            {
                bool val = Convert.ToBoolean(this.Parameters[0], CultureInfo.CurrentCulture);
                session.CheckAdmin();
                session.CheckDdlWrite();
                session.database.logger.SetIncrementBackup(val);
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception8)
            {
                return Result.NewErrorResult(exception8, base.Sql);
            }
        Label_0451:
            flag = Convert.ToBoolean(this.Parameters[0], CultureInfo.CurrentCulture);
            session.database.SetReferentialIntegrity(flag);
            return Result.UpdateZeroResult;
        Label_04D2:
            throw Error.RuntimeError(0xc9, "StatemntCommand");
        }

        public override bool IsAutoCommitStatement()
        {
            return base.isTransactionStatement;
        }

        private Result ProcessBackup(Session session)
        {
            string destPath = (string) this.Parameters[0];
            bool blocking = Convert.ToBoolean(this.Parameters[1], CultureInfo.CurrentCulture);
            bool script = Convert.ToBoolean(this.Parameters[2], CultureInfo.CurrentCulture);
            bool compressed = Convert.ToBoolean(this.Parameters[3], CultureInfo.CurrentCulture);
            try
            {
                session.CheckAdmin();
                if (!session.database.GetDatabaseType().Equals("file:"))
                {
                    return Result.NewErrorResult(Error.GetError(0x1d0));
                }
                if (session.database.IsReadOnly())
                {
                    return Result.NewErrorResult(Error.GetError(0x1cb), null);
                }
                if (!session.database.logger.IsStoredFileAccess())
                {
                    return Result.NewErrorResult(Error.GetError(0x1d0), null);
                }
                session.database.logger.Backup(destPath, session.database.GetPath(), script, blocking, compressed);
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception1)
            {
                return Result.NewErrorResult(exception1, base.Sql);
            }
        }

        private Result ProcessSetUserInitialSchema(Session session)
        {
            try
            {
                User user = (User) this.Parameters[0];
                QNameManager.QName schema = (QNameManager.QName) this.Parameters[1];
                session.CheckDdlWrite();
                if (user == null)
                {
                    user = session.GetUser();
                }
                else
                {
                    session.CheckAdmin();
                    session.CheckDdlWrite();
                    user = session.database.userManager.Get(user.GetNameString());
                }
                if (schema != null)
                {
                    schema = session.database.schemaManager.GetSchemaQName(schema.Name);
                }
                user.SetInitialSchema(schema);
                session.database.schemaManager.SetSchemaChangeTimestamp();
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception1)
            {
                return Result.NewErrorResult(exception1, base.Sql);
            }
        }
    }
}


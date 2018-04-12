namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public sealed class StatementSession : Statement
    {
        private readonly Expression[] _expressions;
        private readonly object[] _parameters;

        public StatementSession(int type, Expression[] args) : base(type)
        {
            this._expressions = args;
            base.isTransactionStatement = false;
            switch (type)
            {
                case 0x47:
                case 0x49:
                case 0x4a:
                case 0x4c:
                case 0x42:
                    base.Group = 0x7d8;
                    return;
            }
            throw Error.RuntimeError(0xc9, "StatementSession");
        }

        public StatementSession(int type, object[] args) : base(type)
        {
            this._parameters = args;
            base.isTransactionStatement = false;
            base.isLogged = false;
            if (type > 0x3f)
            {
                if (type <= 0x6f)
                {
                    if (type != 0x4a)
                    {
                        if (type == 0x4b)
                        {
                            goto Label_0165;
                        }
                    }
                    else
                    {
                        base.Group = 0x7d8;
                        base.isLogged = true;
                        return;
                    }
                    if ((type - 0x62) <= 1)
                    {
                        goto Label_0159;
                    }
                    switch (type)
                    {
                        case 0x6d:
                            goto Label_014D;

                        case 0x6f:
                            goto Label_0165;
                    }
                }
                else if (type <= 0x418)
                {
                    if ((type == 0x40a) || ((type - 0x415) <= 2))
                    {
                        goto Label_014D;
                    }
                    if (type == 0x418)
                    {
                        goto Label_0165;
                    }
                }
                else
                {
                    if (type == 0x420)
                    {
                        base.Group = 0x7db;
                        base.isLogged = true;
                        return;
                    }
                    if (type == 0x814)
                    {
                        goto Label_0171;
                    }
                }
            }
            else
            {
                if (type <= 0x16)
                {
                    if (type != 11)
                    {
                        if (type != 0x10)
                        {
                            if (type == 0x16)
                            {
                                base.Group = 0x7d6;
                                return;
                            }
                            goto Label_017D;
                        }
                        goto Label_00B3;
                    }
                    goto Label_0165;
                }
                if (type <= 0x2f)
                {
                    switch (type)
                    {
                        case 0x2b:
                        case 0x2c:
                            goto Label_00B3;

                        case 0x2d:
                        case 0x2f:
                            goto Label_0159;

                        case 0x20:
                            goto Label_0171;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 0x35:
                            goto Label_0159;

                        case 0x36:
                        case 0x37:
                            goto Label_017D;

                        case 0x38:
                            base.Group = 0x7df;
                            return;

                        case 0x39:
                            goto Label_0165;
                    }
                    if ((type - 0x3e) <= 1)
                    {
                        goto Label_0165;
                    }
                }
            }
            goto Label_017D;
        Label_00B3:
            base.Group = 0x7df;
            return;
        Label_014D:
            base.Group = 0x7db;
            return;
        Label_0159:
            base.Group = 0x7d3;
            return;
        Label_0165:
            base.Group = 0x7d5;
            return;
        Label_0171:
            base.Group = 0x7d8;
            return;
        Label_017D:
            throw Error.RuntimeError(0xc9, "StatementSession");
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
            Table table;
            bool flag;
            ColumnSchema[] schemaArray;
            int num2;
            object obj2;
            Grantee role;
            if (base.IsExplain)
            {
                return Result.NewSingleColumnStringResult("OPERATION", this.Describe(session));
            }
            int type = base.type;
            if (type <= 0x4c)
            {
                if (type > 0x16)
                {
                    switch (type)
                    {
                        case 0x20:
                        {
                            QNameManager.QName name = (QNameManager.QName) this._parameters[0];
                            bool flag6 = (bool) this._parameters[1];
                            if (session.sessionContext.FindSessionTable(name.Name) == null)
                            {
                                if (!flag6)
                                {
                                    throw Error.GetError(0x157d, name.Name);
                                }
                                return Result.UpdateZeroResult;
                            }
                            session.sessionContext.DropSessionTable(name.Name);
                            return Result.UpdateZeroResult;
                        }
                        case 0x21:
                        case 0x22:
                        case 0x23:
                        case 0x24:
                        case 0x29:
                        case 0x2a:
                        case 0x2e:
                            goto Label_094C;

                        case 0x25:
                        case 0x26:
                        case 0x27:
                        case 40:
                        case 0x2d:
                        case 0x2f:
                        case 0x38:
                            goto Label_0944;

                        case 0x2b:
                            return this.ProcessExecuteImmediate(session);

                        case 0x2c:
                            return this.ProcessExecute(session);
                    }
                    if (type == 0x39)
                    {
                        string str3 = (string) this._parameters[0];
                        try
                        {
                            session.ReleaseSavepoint(str3);
                            return Result.UpdateZeroResult;
                        }
                        catch (CoreException exception9)
                        {
                            return Result.NewErrorResult(exception9, base.Sql);
                        }
                    }
                    switch (type)
                    {
                        case 0x3e:
                        {
                            bool chain = Convert.ToBoolean(this._parameters[0], CultureInfo.CurrentCulture);
                            session.Rollback(chain);
                            return Result.UpdateZeroResult;
                        }
                        case 0x3f:
                        {
                            string str4 = (string) this._parameters[0];
                            session.Savepoint(str4);
                            return Result.UpdateZeroResult;
                        }
                        case 0x42:
                            try
                            {
                                string data = (string) this._expressions[0].GetValue(session);
                                data = (string) SqlType.SqlVarchar.Trim(session, data, " ", true, true);
                                if (session.database.GetCatalogName().Name.Equals(data))
                                {
                                    return Result.UpdateZeroResult;
                                }
                                return Result.NewErrorResult(Error.GetError(0x12e8), base.Sql);
                            }
                            catch (CoreException exception10)
                            {
                                return Result.NewErrorResult(exception10, base.Sql);
                            }
                            goto Label_056E;

                        case 0x47:
                        case 0x4b:
                            goto Label_07EA;

                        case 0x49:
                            goto Label_056E;

                        case 0x4a:
                            try
                            {
                                string str7;
                                if (this._expressions == null)
                                {
                                    str7 = ((QNameManager.QName) this._parameters[0]).Name;
                                }
                                else
                                {
                                    str7 = (string) this._expressions[0].GetValue(session);
                                }
                                str7 = (string) SqlType.SqlVarchar.Trim(session, str7, " ", true, true);
                                QNameManager.QName schemaQName = session.database.schemaManager.GetSchemaQName(str7);
                                session.SetSchema(schemaQName.Name);
                                return Result.UpdateZeroResult;
                            }
                            catch (CoreException exception12)
                            {
                                return Result.NewErrorResult(exception12, base.Sql);
                            }
                            goto Label_06C8;

                        case 0x4c:
                            goto Label_06C8;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 11:
                            try
                            {
                                bool chain = this._parameters > null;
                                session.Commit(chain);
                                return Result.UpdateZeroResult;
                            }
                            catch (CoreException exception8)
                            {
                                return Result.NewErrorResult(exception8, base.Sql);
                            }
                            goto Label_0374;

                        case 0x10:
                        {
                            string name = (string) this._parameters[0];
                            try
                            {
                                session.statementManager.FreeStatement(name);
                                return Result.UpdateZeroResult;
                            }
                            catch (CoreException exception7)
                            {
                                return Result.NewErrorResult(exception7, base.Sql);
                            }
                            break;
                        }
                        case 0x16:
                            goto Label_0374;
                    }
                }
            }
            else
            {
                if (type <= 0x40a)
                {
                    if ((type - 0x62) <= 1)
                    {
                        goto Label_0944;
                    }
                    switch (type)
                    {
                        case 0x6d:
                            goto Label_0184;

                        case 110:
                            goto Label_094C;

                        case 0x6f:
                        {
                            bool flag2 = true;
                            try
                            {
                                if (this._parameters[0] != null)
                                {
                                    bool rdy = Convert.ToBoolean(this._parameters[0], CultureInfo.CurrentCulture);
                                    session.SetReadOnly(rdy);
                                }
                                if (this._parameters[1] != null)
                                {
                                    int level = Convert.ToInt32(this._parameters[1]);
                                    session.SetIsolation(level);
                                }
                                if (flag2)
                                {
                                    session.StartTransaction();
                                }
                                return Result.UpdateZeroResult;
                            }
                            catch (CoreException exception1)
                            {
                                return Result.NewErrorResult(exception1, base.Sql);
                            }
                            break;
                        }
                    }
                    if (type != 0x40a)
                    {
                        goto Label_094C;
                    }
                    try
                    {
                        bool mode = Convert.ToBoolean(this._parameters[0], CultureInfo.CurrentCulture);
                        session.SetIgnoreCase(mode);
                        return Result.UpdateZeroResult;
                    }
                    catch (CoreException exception2)
                    {
                        return Result.NewErrorResult(exception2, base.Sql);
                    }
                }
                switch (type)
                {
                    case 0x415:
                        goto Label_0245;

                    case 0x416:
                        goto Label_02CF;

                    case 0x417:
                    {
                        int count = Convert.ToInt32(this._parameters[0]);
                        session.SetResultMemoryRowCount(count);
                        return Result.UpdateZeroResult;
                    }
                    case 0x418:
                    {
                        string name = (string) this._parameters[0];
                        try
                        {
                            session.RollbackToSavepoint(name);
                            return Result.UpdateZeroResult;
                        }
                        catch (CoreException exception3)
                        {
                            return Result.NewErrorResult(exception3, base.Sql);
                        }
                        break;
                    }
                    case 0x420:
                        goto Label_027F;

                    case 0x814:
                        goto Label_0184;
                }
            }
            goto Label_094C;
        Label_0184:
            table = (Table) this._parameters[0];
            List<Constraint> tempConstraints = (List<Constraint>) this._parameters[1];
            StatementDMQL tdmql = (StatementDMQL) this._parameters[2];
            ColumnSchema variable = null;
            if (this._parameters.Length > 3)
            {
                variable = (ColumnSchema) this._parameters[3];
            }
            try
            {
                if (tempConstraints != null)
                {
                    table = ParserDDL.AddTableConstraintDefinitions(session, table, tempConstraints, null, false);
                }
                table.Compile(session, null);
                session.sessionContext.AddSessionTable(table);
                if (table.hasLobColumn)
                {
                    throw Error.GetError(0x4b0);
                }
                if (tdmql != null)
                {
                    Result result = tdmql.Execute(session);
                    table.InsertIntoTable(session, result);
                }
                if (variable != null)
                {
                    session.sessionContext.AddSessionVariable(variable);
                }
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception4)
            {
                return Result.NewErrorResult(exception4, base.Sql);
            }
        Label_0245:
            flag = Convert.ToBoolean(this._parameters[0], CultureInfo.CurrentCulture);
            try
            {
                session.SetAutoCommit(flag);
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception5)
            {
                return Result.NewErrorResult(exception5, base.Sql);
            }
        Label_027F:
            schemaArray = (ColumnSchema[]) this._parameters[0];
            try
            {
                for (int i = 0; i < schemaArray.Length; i++)
                {
                    session.sessionContext.AddSessionVariable(schemaArray[i]);
                }
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception6)
            {
                return Result.NewErrorResult(exception6, base.Sql);
            }
        Label_02CF:
            num2 = Convert.ToInt32(this._parameters[0]);
            session.SetSqlMaxRows(num2);
            return Result.UpdateZeroResult;
        Label_0374:
            session.Close();
            return Result.UpdateZeroResult;
        Label_056E:
            role = null;
            try
            {
                string data = (string) this._expressions[0].GetValue(session);
                if (data != null)
                {
                    data = (string) SqlType.SqlVarchar.Trim(session, data, " ", true, true);
                    role = session.database.granteeManager.GetRole(data);
                }
            }
            catch (CoreException)
            {
                return Result.NewErrorResult(Error.GetError(0x898), base.Sql);
            }
            if (session.IsInMidTransaction())
            {
                return Result.NewErrorResult(Error.GetError(0xe75), base.Sql);
            }
            if (role == null)
            {
                session.SetRole(null);
                return Result.UpdateZeroResult;
            }
            if (session.GetGrantee().HasRole(role))
            {
                session.SetRole(role);
                return Result.UpdateZeroResult;
            }
            return Result.NewErrorResult(Error.GetError(0x898), base.Sql);
        Label_06C8:
            if (session.IsInMidTransaction())
            {
                return Result.NewErrorResult(Error.GetError(0xe75), base.Sql);
            }
            try
            {
                User user;
                string password = null;
                string data = (string) this._expressions[0].GetValue(session);
                data = (string) SqlType.SqlVarchar.Trim(session, data, " ", true, true);
                if (this._expressions[1] != null)
                {
                    password = (string) this._expressions[1].GetValue(session);
                }
                if (password == null)
                {
                    user = session.database.userManager.Get(data);
                }
                else
                {
                    user = session.database.GetUserManager().GetUser(data, password);
                }
                if (user == null)
                {
                    throw Error.GetError(0xfa1);
                }
                base.Sql = user.GetConnectUserSQL();
                if (user != session.GetGrantee())
                {
                    if ((password == null) && !session.GetGrantee().CanChangeAuthorisation())
                    {
                        throw Error.GetError(0xfa0);
                    }
                    session.SetUser(user);
                    session.SetRole(null);
                    session.ResetSchema();
                }
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception13)
            {
                return Result.NewErrorResult(exception13, base.Sql);
            }
        Label_07EA:
            obj2 = null;
            if ((this._expressions[0].GetExprType() == 1) && (this._expressions[0].GetConstantValueNoCheck(session) == null))
            {
                session.SetZoneSeconds(session.SessionTimeZoneSeconds);
                return Result.UpdateZeroResult;
            }
            try
            {
                obj2 = this._expressions[0].GetValue(session);
            }
            catch (CoreException)
            {
            }
            Result result3 = obj2 as Result;
            if (result3 != null)
            {
                if (!result3.IsData())
                {
                    return Result.NewErrorResult(Error.GetError(0xd51), base.Sql);
                }
                object[] next = result3.GetNavigator().GetNext();
                if ((result3.GetNavigator().Next() || (next == null)) || (next[0] == null))
                {
                    result3.GetNavigator().Close();
                    return Result.NewErrorResult(Error.GetError(0xd51), base.Sql);
                }
                obj2 = next[0];
                result3.GetNavigator().Close();
            }
            else if (obj2 == null)
            {
                return Result.NewErrorResult(Error.GetError(0xd51), base.Sql);
            }
            long seconds = ((IntervalSecondData) obj2).GetSeconds();
            if ((-((long) DTIType.TimezoneSecondsLimit) <= seconds) && (seconds <= DTIType.TimezoneSecondsLimit))
            {
                session.SetZoneSeconds((int) seconds);
                return Result.UpdateZeroResult;
            }
            return Result.NewErrorResult(Error.GetError(0xd51), base.Sql);
        Label_0944:
            return this.ProcessPrepare(session);
        Label_094C:
            throw Error.RuntimeError(0xc9, "StatementSession");
        }

        public override bool IsAutoCommitStatement()
        {
            return false;
        }

        public override bool IsCatalogChange()
        {
            return false;
        }

        private Result ProcessExecute(Session session)
        {
            string name = (string) this._parameters[0];
            OrderedHashSet<Expression> set = (OrderedHashSet<Expression>) this._parameters[1];
            try
            {
                object[] parameterValues = new object[set.Size()];
                for (int i = 0; i < set.Size(); i++)
                {
                    parameterValues[i] = set.Get(i).GetValue(session);
                }
                Statement statement = session.statementManager.GetStatement(session, name);
                Result cmd = Result.NewPreparedExecuteRequest(statement.GetParametersMetaData().GetParameterTypes(), statement.GetId());
                cmd.SetStatement(statement);
                cmd.SetPreparedExecuteProperties(parameterValues, 0, 0, ResultProperties.DefaultPropsValue);
                return session.Execute(cmd);
            }
            catch (CoreException exception1)
            {
                return Result.NewErrorResult(exception1, base.Sql);
            }
        }

        private Result ProcessExecuteImmediate(Session session)
        {
            Expression expression = (Expression) this._parameters[0];
            try
            {
                string sql = (string) expression.GetValue(session);
                Result cmd = Result.NewExecuteDirectRequest();
                cmd.SetMainString(sql);
                return session.Execute(cmd);
            }
            catch (CoreException exception1)
            {
                return Result.NewErrorResult(exception1, base.Sql);
            }
        }

        private Result ProcessPrepare(Session session)
        {
            string name = (string) this._parameters[0];
            Expression expression = (Expression) this._parameters[1];
            try
            {
                string sql = (string) expression.GetValue(session);
                session.statementManager.Compile(session, name, sql, ResultProperties.DefaultPropsValue);
                return Result.UpdateZeroResult;
            }
            catch (CoreException exception1)
            {
                return Result.NewErrorResult(exception1, base.Sql);
            }
        }
    }
}


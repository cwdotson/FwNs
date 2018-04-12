namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class StatementSchemaDefinition : StatementSchema
    {
        private readonly StatementSchema[] _statements;

        public StatementSchemaDefinition(StatementSchema[] statements)
        {
            this._statements = statements;
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
            }
            return result;
        }

        public static string GetDropSchemaStatement(QNameManager.QName schema)
        {
            return ("DROP SCHEMA " + schema.StatementName + " CASCADE");
        }

        public Result GetResult(Session session)
        {
            QNameManager.QName schemaName = this._statements[0].GetSchemaName();
            if (base.IsExplain)
            {
                return Result.NewSingleColumnStringResult("OPERATION", this.Describe(session));
            }
            Result result2 = this._statements[0].Execute(session);
            List<Constraint> list = new List<Constraint>();
            StatementSchema schema = new StatementSchema(null, 0x430, null);
            if ((this._statements.Length != 1) && !result2.IsError())
            {
                QNameManager.QName currentSchemaQName = session.GetCurrentSchemaQName();
                for (int i = 1; i < this._statements.Length; i++)
                {
                    try
                    {
                        session.SetSchema(schemaName.Name);
                    }
                    catch (CoreException)
                    {
                    }
                    this._statements[i].SetSchemaQName(schemaName);
                    session.Parser.Reset(this._statements[i].GetSql());
                    try
                    {
                        StatementSchema schema2;
                        session.Parser.Read();
                        int statementType = this._statements[i].GetStatementType();
                        if (statementType <= 0x3d)
                        {
                            switch (statementType)
                            {
                                case 0x30:
                                case 0x31:
                                    result2 = this._statements[i].Execute(session);
                                    goto Label_02D0;

                                case 0x34:
                                case 6:
                                    goto Label_02B4;

                                case 0x17:
                                case 14:
                                    goto Label_026F;

                                case 0x3d:
                                case 8:
                                case 10:
                                    goto Label_025D;
                            }
                            goto Label_02C0;
                        }
                        if (statementType <= 0x72)
                        {
                            StatementSchema schema1;
                            switch (statementType)
                            {
                                case 0x4d:
                                    schema1 = session.Parser.CompileCreate();
                                    schema1.IsSchemaDefinition = true;
                                    schema1.SetSchemaQName(schemaName);
                                    if (session.Parser.token.TokenType != 0x2ec)
                                    {
                                        throw session.Parser.UnexpectedToken();
                                    }
                                    break;

                                case 0x4e:
                                case 0x51:
                                case 0x52:
                                    goto Label_02C0;

                                case 0x4f:
                                    goto Label_02B4;

                                case 80:
                                case 0x54:
                                    goto Label_026F;

                                case 0x53:
                                    goto Label_025D;

                                default:
                                    if (statementType == 0x72)
                                    {
                                        goto Label_02B4;
                                    }
                                    goto Label_02C0;
                            }
                            schema1.SetIsLogged(false);
                            result2 = schema1.Execute(session);
                            QNameManager.QName name = ((Table) schema1.Arguments[0]).GetName();
                            Table schemaObject = (Table) session.database.schemaManager.GetSchemaObject(name);
                            List<Constraint> collection = (List<Constraint>) schema1.Arguments[1];
                            list.AddRange(collection);
                            collection.Clear();
                            schema.Sql = schemaObject.GetSql();
                            schema.Execute(session);
                            goto Label_02D0;
                        }
                        switch (statementType)
                        {
                            case 0x75:
                                goto Label_02B4;

                            case 0x85:
                                break;

                            case 0x41c:
                                goto Label_026F;

                            default:
                                goto Label_02C0;
                        }
                    Label_025D:
                        result2 = this._statements[i].Execute(session);
                        goto Label_02D0;
                    Label_026F:
                        schema2 = session.Parser.CompileCreate();
                        schema2.IsSchemaDefinition = true;
                        schema2.SetSchemaQName(schemaName);
                        if (session.Parser.token.TokenType != 0x2ec)
                        {
                            throw session.Parser.UnexpectedToken();
                        }
                        result2 = schema2.Execute(session);
                        goto Label_02D0;
                    Label_02B4:
                        throw session.Parser.UnsupportedFeature();
                    Label_02C0:
                        throw Error.RuntimeError(0xc9, "");
                    Label_02D0:
                        if (result2.IsError())
                        {
                            break;
                        }
                    }
                    catch (CoreException exception2)
                    {
                        result2 = Result.NewErrorResult(exception2, this._statements[i].GetSql());
                        break;
                    }
                }
                if (!result2.IsError())
                {
                    try
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            Constraint c = list[j];
                            Table userTable = session.database.schemaManager.GetUserTable(session, c.Core.RefTableName);
                            ParserDDL.AddForeignKey(session, userTable, c, null);
                            schema.Sql = c.GetSql();
                            schema.Execute(session);
                        }
                    }
                    catch (CoreException exception3)
                    {
                        result2 = Result.NewErrorResult(exception3, base.Sql);
                    }
                }
                if (result2.IsError())
                {
                    try
                    {
                        session.database.schemaManager.DropSchema(session, schemaName.Name, true);
                        session.database.logger.WriteToLog(session, GetDropSchemaStatement(schemaName));
                    }
                    catch (CoreException)
                    {
                    }
                }
                try
                {
                    session.SetCurrentSchemaQName(currentSchemaQName);
                }
                catch (Exception)
                {
                }
            }
            return result2;
        }
    }
}


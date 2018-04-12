namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class StatementSchema : Statement
    {
        public object[] Arguments;
        public bool IsSchemaDefinition;
        public int Order;
        public Token[] StatementTokens;

        public StatementSchema() : base(0x40, 0x7d1)
        {
            this.Arguments = new object[0];
            base.isTransactionStatement = true;
        }

        public StatementSchema(string sql, int type, object[] args) : this(sql, type, args, null, null)
        {
        }

        public StatementSchema(string sql, int type, QNameManager.QName readName, QNameManager.QName writeName) : this(sql, type, null, readName, writeName)
        {
        }

        public StatementSchema(string sql, int type, object[] args, QNameManager.QName readName, QNameManager.QName writeName) : base(type)
        {
            this.Arguments = new object[0];
            base.isTransactionStatement = true;
            base.Sql = sql;
            if (args != null)
            {
                this.Arguments = args;
            }
            if ((readName != null) && (readName != writeName))
            {
                base.ReadTableNames = new QNameManager.QName[] { readName };
            }
            if (writeName != null)
            {
                base.WriteTableNames = new QNameManager.QName[] { writeName };
            }
            if (type <= 0x75)
            {
                if (type <= 0x34)
                {
                    switch (type)
                    {
                        case 3:
                        case 4:
                        case 0x11:
                            goto Label_03FF;

                        case 6:
                            base.Group = 0x7d1;
                            this.Order = 9;
                            return;

                        case 8:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;

                        case 10:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;

                        case 14:
                            base.Group = 0x7d1;
                            this.Order = 7;
                            return;

                        case 0x17:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;

                        case 0x18:
                        case 0x19:
                        case 0x1a:
                        case 0x1b:
                        case 0x1d:
                        case 30:
                        case 0x1f:
                        case 0x20:
                        case 0x21:
                        case 0x22:
                        case 0x23:
                        case 0x24:
                            goto Label_040B;

                        case 0x30:
                            base.Group = 0x7d2;
                            this.Order = 10;
                            return;

                        case 0x31:
                            base.Group = 0x7d2;
                            this.Order = 10;
                            return;

                        case 0x34:
                            base.Group = 0x7d1;
                            this.Order = 2;
                            return;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 0x3b:
                            goto Label_0417;

                        case 60:
                            goto Label_03FF;

                        case 0x3d:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;

                        case 0x40:
                            base.Group = 0x7d1;
                            return;

                        case 0x4d:
                            base.Group = 0x7d1;
                            this.Order = 2;
                            return;

                        case 0x4e:
                        case 0x73:
                        case 0x74:
                            goto Label_040B;

                        case 0x4f:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;

                        case 80:
                            base.Group = 0x7d1;
                            this.Order = 7;
                            return;

                        case 0x53:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;

                        case 0x54:
                            base.Group = 0x7d1;
                            this.Order = 5;
                            return;

                        case 0x72:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;

                        case 0x75:
                            base.Group = 0x7d1;
                            this.Order = 1;
                            return;
                    }
                }
                goto Label_0423;
            }
            if (type <= 0x428)
            {
                switch (type)
                {
                    case 0x7f:
                    case 0x86:
                        goto Label_03FF;

                    case 0x81:
                        goto Label_0417;

                    case 0x85:
                        base.Group = 0x7d1;
                        this.Order = 1;
                        return;

                    case 0x87:
                    case 0x421:
                    case 0x422:
                    case 0x423:
                    case 0x426:
                        goto Label_040B;

                    case 0x41b:
                        base.Group = 0x7d1;
                        this.Order = 8;
                        return;

                    case 0x41c:
                        base.Group = 0x7d2;
                        this.Order = 4;
                        return;

                    case 0x41d:
                        base.Group = 0x7d1;
                        this.Order = 1;
                        return;

                    case 0x428:
                        base.Group = 0x7d2;
                        return;
                }
                goto Label_0423;
            }
            if (type == 0x430)
            {
                base.Group = 0x7d2;
                return;
            }
            if (type == 0x450)
            {
                base.Group = 0x7d2;
                this.StatementTokens = (Token[]) args[0];
                return;
            }
            if (type != 0x817)
            {
                if (type == 0x818)
                {
                    base.Group = 0x7d2;
                    this.Order = 11;
                    return;
                }
                goto Label_0423;
            }
        Label_03FF:
            base.Group = 0x7d2;
            return;
        Label_040B:
            base.Group = 0x7d2;
            return;
        Label_0417:
            base.Group = 0x7d2;
            return;
        Label_0423:
            throw Error.RuntimeError(0xc9, "StatemntSchema");
        }

        public static void CheckSchemaUpdateAuthorisation(Session session, QNameManager.QName schema)
        {
            if (!session.IsProcessingLog())
            {
                if (SqlInvariants.IsSystemSchemaName(schema.Name))
                {
                    throw Error.GetError(0x157f);
                }
                if (session.Parser.IsSchemaDefinition)
                {
                    if (schema == session.GetCurrentSchemaQName())
                    {
                        return;
                    }
                    Error.GetError(0x1581, schema.Name);
                }
                session.GetGrantee().CheckSchemaUpdateOrGrantRights(schema.Name);
                session.CheckDdlWrite();
            }
        }

        public override string Describe(Session session)
        {
            return base.Sql;
        }

        private static void DropDomain(Session session, QNameManager.QName name, bool cascade)
        {
            SqlType schemaObject = (SqlType) session.database.schemaManager.GetSchemaObject(name);
            OrderedHashSet<QNameManager.QName> referencingObjectNames = session.database.schemaManager.GetReferencingObjectNames(schemaObject.GetName());
            if (!cascade && (referencingObjectNames.Size() > 0))
            {
                QNameManager.QName name2 = referencingObjectNames.Get(0);
                throw Error.GetError(0x157e, name2.GetSchemaQualifiedStatementName());
            }
            Constraint[] constraints = schemaObject.userTypeModifier.GetConstraints();
            referencingObjectNames.Clear();
            for (int i = 0; i < constraints.Length; i++)
            {
                referencingObjectNames.Add(constraints[i].GetName());
            }
            session.database.schemaManager.RemoveSchemaObjects(referencingObjectNames);
            session.database.schemaManager.RemoveSchemaObject(schemaObject.GetName(), cascade);
            schemaObject.userTypeModifier = null;
        }

        private static void DropObject(Session session, QNameManager.QName name, bool cascade)
        {
            name = session.database.schemaManager.GetSchemaObjectName(name.schema, name.Name, name.type, true);
            session.database.schemaManager.RemoveSchemaObject(name, cascade);
        }

        private static void DropRole(Session session, QNameManager.QName name, bool cascade)
        {
            Grantee role = session.database.GetGranteeManager().GetRole(name.Name);
            if (!cascade && session.database.schemaManager.HasSchemas(role))
            {
                Schema schema = session.database.schemaManager.GetSchemas(role)[0];
                throw Error.GetError(0x157e, schema.GetName().StatementName);
            }
            session.database.schemaManager.DropSchemas(session, role, cascade);
            session.database.GetGranteeManager().DropRole(name.Name);
        }

        private static void DropRoutine(Session session, QNameManager.QName name, bool cascade)
        {
            CheckSchemaUpdateAuthorisation(session, name.schema);
            session.database.schemaManager.RemoveSchemaObject(name, cascade);
        }

        private static void DropSchema(Session session, QNameManager.QName name, bool cascade)
        {
            QNameManager.QName userSchemaQName = session.database.schemaManager.GetUserSchemaQName(name.Name);
            CheckSchemaUpdateAuthorisation(session, userSchemaQName);
            session.database.schemaManager.DropSchema(session, name.Name, cascade);
        }

        private static void DropTable(Session session, QNameManager.QName name, bool cascade)
        {
            Table table = session.database.schemaManager.FindUserTable(session, name.Name, name.schema.Name);
            session.database.schemaManager.DropTableOrView(session, table, cascade);
        }

        private static void DropType(Session session, QNameManager.QName name, bool cascade)
        {
            CheckSchemaUpdateAuthorisation(session, name.schema);
            session.database.schemaManager.RemoveSchemaObject(name, cascade);
            ((SqlType) session.database.schemaManager.GetSchemaObject(name)).userTypeModifier = null;
        }

        private static void DropUser(Session session, QNameManager.QName name, bool cascade)
        {
            Grantee grantee = session.database.GetUserManager().Get(name.Name);
            if (session.database.GetSessionManager().IsUserActive(name.Name))
            {
                throw Error.GetError(0x15a3);
            }
            if (!cascade && session.database.schemaManager.HasSchemas(grantee))
            {
                Schema schema = session.database.schemaManager.GetSchemas(grantee)[0];
                throw Error.GetError(0x157e, schema.GetName().StatementName);
            }
            session.database.schemaManager.DropSchemas(session, grantee, cascade);
            session.database.GetUserManager().DropUser(name.Name);
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
            session.database.schemaManager.SetSchemaChangeTimestamp();
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

        private Result GetResult(Session session)
        {
            Routine routine;
            View view;
            SchemaManager schemaManager = session.database.schemaManager;
            if (base.IsExplain)
            {
                return Result.NewSingleColumnStringResult("OPERATION", this.Describe(session));
            }
            int type = base.type;
            if (type > 0x54)
            {
                if (type <= 0x87)
                {
                    switch (type)
                    {
                        case 0x72:
                        case 0x73:
                        case 0x74:
                        case 0x7f:
                        case 0x81:
                        case 0x85:
                        case 0x86:
                        case 0x87:
                            goto Label_05DC;

                        case 0x75:
                            return Result.UpdateZeroResult;
                    }
                    goto Label_0684;
                }
                switch (type)
                {
                    case 0x41b:
                    case 0x41c:
                    case 0x41d:
                    case 0x421:
                    case 0x422:
                    case 0x423:
                    case 0x426:
                        goto Label_05DC;

                    case 0x41e:
                    case 0x41f:
                    case 0x420:
                    case 0x424:
                    case 0x425:
                    case 0x427:
                        goto Label_0684;

                    case 0x428:
                    {
                        QNameManager.QName schema = (QNameManager.QName) this.Arguments[0];
                        QNameManager.QName name = (QNameManager.QName) this.Arguments[1];
                        if (schema.type == 1)
                        {
                            try
                            {
                                session.CheckAdmin();
                                session.CheckDdlWrite();
                                schema.Rename(name);
                                goto Label_0694;
                            }
                            catch (CoreException exception8)
                            {
                                return Result.NewErrorResult(exception8, base.Sql);
                            }
                        }
                        if (schema.type == 2)
                        {
                            CheckSchemaUpdateAuthorisation(session, schema);
                            schemaManager.CheckSchemaNameCanChange(schema);
                            schemaManager.RenameSchema(schema, name);
                            goto Label_0694;
                        }
                        try
                        {
                            ISchemaObject column;
                            schema.SetSchemaIfNull(session.GetCurrentSchemaQName());
                            if (schema.type == 9)
                            {
                                Table userTable = schemaManager.GetUserTable(session, schema.Parent);
                                column = userTable.GetColumn(userTable.GetColumnIndex(schema.Name));
                            }
                            else
                            {
                                column = schemaManager.GetSchemaObject(schema);
                                if (column == null)
                                {
                                    throw Error.GetError(0x157d, schema.Name);
                                }
                                schema = column.GetName();
                            }
                            CheckSchemaUpdateAuthorisation(session, schema.schema);
                            name.SetSchemaIfNull(schema.schema);
                            if (schema.schema != name.schema)
                            {
                                return Result.NewErrorResult(Error.GetError(0x1581), base.Sql);
                            }
                            name.Parent = schema.Parent;
                            if (column.GetSchemaObjectType() == 9)
                            {
                                QNameManager.QName parent = column.GetName().Parent;
                                schemaManager.CheckColumnIsReferenced(parent, column.GetName());
                                schemaManager.GetUserTable(session, parent).RenameColumn((ColumnSchema) column, name);
                            }
                            else
                            {
                                schemaManager.RenameSchemaObject(schema, name);
                            }
                            goto Label_0694;
                        }
                        catch (CoreException exception9)
                        {
                            return Result.NewErrorResult(exception9, base.Sql);
                        }
                        break;
                    }
                }
                if (type != 0x430)
                {
                    if (type == 0x817)
                    {
                        goto Label_0649;
                    }
                    if (type != 0x818)
                    {
                        goto Label_0684;
                    }
                    this.ProcessComment(session, schemaManager);
                }
                goto Label_0694;
            }
            if (type > 0x34)
            {
                switch (type)
                {
                    case 0x3b:
                    case 60:
                    case 0x40:
                        goto Label_0364;

                    case 0x3d:
                        try
                        {
                            session.CheckAdmin();
                            session.CheckDdlWrite();
                            QNameManager.QName name = (QNameManager.QName) this.Arguments[0];
                            session.database.GetGranteeManager().AddRole(name);
                            goto Label_0694;
                        }
                        catch (CoreException exception2)
                        {
                            return Result.NewErrorResult(exception2, base.Sql);
                        }
                        break;

                    case 0x3e:
                    case 0x3f:
                        goto Label_0684;
                }
                switch (type)
                {
                    case 0x4d:
                    {
                        Table table = (Table) this.Arguments[0];
                        List<Constraint> tempConstraints = (List<Constraint>) this.Arguments[1];
                        StatementDMQL tdmql = (StatementDMQL) this.Arguments[2];
                        List<Constraint> constraintList = null;
                        try
                        {
                            this.SetOrCheckObjectName(session, null, table.GetName(), true);
                        }
                        catch (CoreException exception3)
                        {
                            return Result.NewErrorResult(exception3, base.Sql);
                        }
                        try
                        {
                            if (this.IsSchemaDefinition)
                            {
                                constraintList = new List<Constraint>();
                            }
                            if (tempConstraints != null)
                            {
                                table = ParserDDL.AddTableConstraintDefinitions(session, table, tempConstraints, constraintList, true);
                                this.Arguments[1] = constraintList;
                            }
                            table.Compile(session, null);
                            schemaManager.AddSchemaObject(table);
                            if (tdmql != null)
                            {
                                Result result = tdmql.Execute(session);
                                table.InsertIntoTable(session, result);
                            }
                            if (table.hasLobColumn)
                            {
                                IRowIterator rowIterator = table.GetRowIterator(session);
                                while (rowIterator.HasNext())
                                {
                                    object[] rowData = rowIterator.GetNextRow().RowData;
                                    session.sessionData.AdjustLobUsageCount(table, rowData, 1);
                                }
                            }
                            return Result.UpdateZeroResult;
                        }
                        catch (CoreException exception4)
                        {
                            schemaManager.RemoveExportedKeys(table);
                            schemaManager.RemoveDependentObjects(table.GetName());
                            return Result.NewErrorResult(exception4, base.Sql);
                        }
                        break;
                    }
                    case 0x4e:
                        goto Label_0364;

                    case 0x4f:
                        break;

                    case 80:
                        try
                        {
                            this.ProcessCreateTrigger(session, schemaManager);
                            goto Label_0694;
                        }
                        catch (CoreException exception5)
                        {
                            return Result.NewErrorResult(exception5, base.Sql);
                        }
                        goto Label_0320;

                    case 0x51:
                    case 0x52:
                        goto Label_0684;

                    case 0x53:
                        goto Label_0320;

                    case 0x54:
                        goto Label_0342;

                    default:
                        goto Label_0684;
                }
                return Result.UpdateZeroResult;
            }
            switch (type)
            {
                case 3:
                case 4:
                case 10:
                case 14:
                case 0x17:
                case 0x18:
                case 0x19:
                case 0x1a:
                case 0x1b:
                case 0x1d:
                case 30:
                case 0x1f:
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                    goto Label_0136;

                case 5:
                case 7:
                case 9:
                case 11:
                case 12:
                case 13:
                case 15:
                case 0x10:
                case 0x12:
                case 0x13:
                case 20:
                case 0x15:
                case 0x16:
                case 0x1c:
                    goto Label_0684;

                case 6:
                    goto Label_0674;

                case 8:
                {
                    Charset charset = (Charset) this.Arguments[0];
                    try
                    {
                        this.SetOrCheckObjectName(session, null, charset.GetName(), true);
                        schemaManager.AddSchemaObject(charset);
                        goto Label_0694;
                    }
                    catch (CoreException exception1)
                    {
                        return Result.NewErrorResult(exception1, base.Sql);
                    }
                    break;
                }
                case 0x11:
                    goto Label_05FD;
            }
            switch (type)
            {
                case 0x30:
                case 0x31:
                    break;

                case 0x34:
                    goto Label_0364;

                default:
                    goto Label_0684;
            }
        Label_0136:
            return Result.UpdateZeroResult;
        Label_0320:;
            try
            {
                this.ProcessCreateType(session, schemaManager);
                goto Label_0694;
            }
            catch (CoreException exception6)
            {
                return Result.NewErrorResult(exception6, base.Sql);
            }
        Label_0342:;
            try
            {
                this.ProcessCreateView(session, schemaManager);
                goto Label_0694;
            }
            catch (CoreException exception7)
            {
                return Result.NewErrorResult(exception7, base.Sql);
            }
        Label_0364:
            return Result.UpdateZeroResult;
        Label_05DC:;
            try
            {
                this.ProcessAlterSequence(session);
                goto Label_0694;
            }
            catch (CoreException exception10)
            {
                return Result.NewErrorResult(exception10, base.Sql);
            }
        Label_05FD:
            routine = (Routine) this.Arguments[0];
            try
            {
                routine.ResolveReferences(session);
                Routine schemaObject = (Routine) schemaManager.GetSchemaObject(routine.GetSpecificName());
                schemaManager.ReplaceReferences(schemaObject, routine);
                schemaObject.SetAsAlteredRoutine(routine);
                goto Label_0694;
            }
            catch (CoreException exception11)
            {
                return Result.NewErrorResult(exception11, base.Sql);
            }
        Label_0649:
            view = (View) this.Arguments[0];
            try
            {
                ProcessAlterView(session, schemaManager, view);
                goto Label_0694;
            }
            catch (CoreException exception12)
            {
                return Result.NewErrorResult(exception12, base.Sql);
            }
        Label_0674:
            return Result.UpdateZeroResult;
        Label_0684:
            throw Error.RuntimeError(0xc9, "CompiledStateemntSchema");
        Label_0694:
            return Result.UpdateZeroResult;
        }

        public override bool IsAutoCommitStatement()
        {
            return true;
        }

        private void ProcessAlterSequence(Session session)
        {
            NumberSequence sequence = (NumberSequence) this.Arguments[0];
            NumberSequence other = (NumberSequence) this.Arguments[1];
            CheckSchemaUpdateAuthorisation(session, sequence.GetSchemaName());
            sequence.Reset(other);
        }

        private static void ProcessAlterView(Session session, SchemaManager schemaManager, View view)
        {
            CheckSchemaUpdateAuthorisation(session, view.GetSchemaName());
            View schemaObject = (View) schemaManager.GetSchemaObject(view.GetName());
            if (schemaObject == null)
            {
                throw Error.GetError(0x157d, view.GetName().Name);
            }
            view.SetName(schemaObject.GetName());
            view.Compile(session, null);
            if (schemaManager.GetReferencingObjectNames(schemaObject.GetName()).GetCommonElementCount(view.GetReferences()) > 0)
            {
                throw Error.GetError(0x157e);
            }
            int tableIndex = schemaManager.GetTableIndex(schemaObject);
            schemaManager.SetTable(tableIndex, view);
            OrderedHashSet<Table> tableSet = new OrderedHashSet<Table>();
            tableSet.Add(view);
            try
            {
                schemaManager.RecompileDependentObjects(tableSet);
            }
            catch (CoreException)
            {
                schemaManager.SetTable(tableIndex, schemaObject);
                schemaManager.RecompileDependentObjects(tableSet);
            }
        }

        private void ProcessComment(Session session, SchemaManager schemaManager)
        {
            QNameManager.QName name = (QNameManager.QName) this.Arguments[0];
            string str = (string) this.Arguments[1];
            int type = name.type;
            switch (type)
            {
                case 3:
                {
                    Table table2 = (Table) schemaManager.GetSchemaObject(name.Name, name.schema.Name, 3);
                    if (!session.GetGrantee().IsFullyAccessibleByRole(table2.GetName()))
                    {
                        throw Error.GetError(0x157d);
                    }
                    table2.GetName().Comment = str;
                    break;
                }
                case 9:
                {
                    Table table = (Table) schemaManager.GetSchemaObject(name.Parent.Name, name.Parent.schema.Name, 3);
                    if (!session.GetGrantee().IsFullyAccessibleByRole(table.GetName()))
                    {
                        throw Error.GetError(0x157d);
                    }
                    int columnIndex = table.GetColumnIndex(name.Name);
                    if (columnIndex < 0)
                    {
                        throw Error.GetError(0x157d);
                    }
                    table.GetColumn(columnIndex).GetName().Comment = str;
                    return;
                }
                default:
                {
                    if (type != 0x12)
                    {
                        break;
                    }
                    RoutineSchema schema = (RoutineSchema) schemaManager.GetSchemaObject(name.Name, name.schema.Name, 0x12);
                    if (!session.GetGrantee().IsFullyAccessibleByRole(schema.GetName()))
                    {
                        throw Error.GetError(0x157d);
                    }
                    schema.GetName().Comment = str;
                    return;
                }
            }
        }

        private void ProcessCreateAlias(Session session, SchemaManager schemaManager)
        {
            QNameManager.QName name = (QNameManager.QName) this.Arguments[0];
            Routine[] routineArray = (Routine[]) this.Arguments[1];
            session.CheckAdmin();
            session.CheckDdlWrite();
            if (name != null)
            {
                for (int i = 0; i < routineArray.Length; i++)
                {
                    routineArray[i].SetName(name);
                    schemaManager.AddSchemaObject(routineArray[i]);
                }
            }
        }

        private void ProcessCreateDomain(Session session, SchemaManager schemaManager)
        {
            SqlType type = (SqlType) this.Arguments[0];
            Constraint[] constraints = type.userTypeModifier.GetConstraints();
            this.SetOrCheckObjectName(session, null, type.GetName(), true);
            for (int i = 0; i < constraints.Length; i++)
            {
                Constraint constraint = constraints[i];
                this.SetOrCheckObjectName(session, type.GetName(), constraint.GetName(), true);
                schemaManager.AddSchemaObject(constraint);
            }
            schemaManager.AddSchemaObject(type);
        }

        private void ProcessCreateIndex(Session session)
        {
            Table table = (Table) this.Arguments[0];
            int[] col = (int[]) this.Arguments[1];
            QNameManager.QName name = (QNameManager.QName) this.Arguments[2];
            bool unique = Convert.ToBoolean(this.Arguments[3], CultureInfo.CurrentCulture);
            this.SetOrCheckObjectName(session, table.GetName(), name, true);
            new TableWorks(session, table).AddIndex(col, name, unique);
        }

        private void ProcessCreateRoutine(Session session, SchemaManager schemaManager)
        {
            Routine routine = (Routine) this.Arguments[0];
            routine.Resolve(session);
            this.SetOrCheckObjectName(session, null, routine.GetName(), false);
            schemaManager.AddSchemaObject(routine);
        }

        private void ProcessCreateSchema(Session session, SchemaManager schemaManager)
        {
            QNameManager.QName name = (QNameManager.QName) this.Arguments[0];
            Grantee owner = (Grantee) this.Arguments[1];
            session.CheckDdlWrite();
            if (!schemaManager.SchemaExists(name.Name))
            {
                schemaManager.CreateSchema(name, owner);
                base.Sql = schemaManager.FindSchema(name.Name).GetSql();
            }
            else if (!session.IsProcessingScript() || !"PUBLIC".Equals(name.Name))
            {
                throw Error.GetError(0x1580, name.Name);
            }
        }

        private void ProcessCreateSequence(Session session, SchemaManager schemaManager)
        {
            NumberSequence sequence = (NumberSequence) this.Arguments[0];
            this.SetOrCheckObjectName(session, null, sequence.GetName(), true);
            schemaManager.AddSchemaObject(sequence);
        }

        private void ProcessCreateTrigger(Session session, SchemaManager schemaManager)
        {
            TriggerDef td = (TriggerDef) this.Arguments[0];
            QNameManager.QName name = (QNameManager.QName) this.Arguments[1];
            CheckSchemaUpdateAuthorisation(session, td.GetSchemaName());
            schemaManager.CheckSchemaObjectNotExists(td.GetName());
            if ((name != null) && (schemaManager.GetSchemaObject(name) == null))
            {
                throw Error.GetError(0x157d, name.Name);
            }
            td.table.AddTrigger(td, name);
            schemaManager.AddSchemaObject(td);
        }

        private void ProcessCreateType(Session session, SchemaManager schemaManager)
        {
            SqlType type = (SqlType) this.Arguments[0];
            this.SetOrCheckObjectName(session, null, type.GetName(), true);
            schemaManager.AddSchemaObject(type);
        }

        private void ProcessCreateUser(Session session)
        {
            QNameManager.QName name = (QNameManager.QName) this.Arguments[0];
            string password = (string) this.Arguments[1];
            Grantee grantor = (Grantee) this.Arguments[2];
            session.CheckAdmin();
            session.CheckDdlWrite();
            session.database.GetUserManager().CreateUser(name, password);
            if (Convert.ToBoolean(this.Arguments[3], CultureInfo.CurrentCulture))
            {
                session.database.GetGranteeManager().Grant(name.Name, "DBA", grantor);
            }
        }

        private void ProcessCreateView(Session session, SchemaManager schemaManager)
        {
            View view = (View) this.Arguments[0];
            CheckSchemaUpdateAuthorisation(session, view.GetSchemaName());
            schemaManager.CheckSchemaObjectNotExists(view.GetName());
            view.Compile(session, null);
            schemaManager.AddSchemaObject(view);
        }

        private void ProcessDropColumn(Session session, SchemaManager schemaManager)
        {
            QNameManager.QName name = (QNameManager.QName) this.Arguments[0];
            bool cascade = Convert.ToBoolean(this.Arguments[2], CultureInfo.CurrentCulture);
            Table userTable = schemaManager.GetUserTable(session, name.Parent);
            int columnIndex = userTable.GetColumnIndex(name.Name);
            if (userTable.GetColumnCount() == 1)
            {
                throw Error.GetError(0x15d7);
            }
            CheckSchemaUpdateAuthorisation(session, userTable.GetSchemaName());
            session.Commit(false);
            new TableWorks(session, userTable).DropColumn(columnIndex, cascade);
        }

        private void ProcessGrantRevokeRole(Session session)
        {
            bool grant = base.type == 0x31;
            OrderedHashSet<string> granteeList = (OrderedHashSet<string>) this.Arguments[0];
            OrderedHashSet<string> roleList = (OrderedHashSet<string>) this.Arguments[1];
            Grantee grantor = (Grantee) this.Arguments[2];
            GranteeManager granteeManager = session.database.granteeManager;
            granteeManager.CheckGranteeList(granteeList);
            for (int i = 0; i < granteeList.Size(); i++)
            {
                string granteeName = granteeList.Get(i);
                granteeManager.CheckRoleList(granteeName, roleList, grantor, grant);
            }
            if (grant)
            {
                for (int j = 0; j < granteeList.Size(); j++)
                {
                    string granteeName = granteeList.Get(j);
                    for (int k = 0; k < roleList.Size(); k++)
                    {
                        string roleName = roleList.Get(k);
                        granteeManager.Grant(granteeName, roleName, grantor);
                    }
                }
            }
            else
            {
                for (int j = 0; j < granteeList.Size(); j++)
                {
                    string granteeName = granteeList.Get(j);
                    for (int k = 0; k < roleList.Size(); k++)
                    {
                        granteeManager.Revoke(granteeName, roleList.Get(k), grantor);
                    }
                }
            }
        }

        private void SetOrCheckObjectName(Session session, QNameManager.QName parent, QNameManager.QName name, bool check)
        {
            if (name.schema == null)
            {
                name.schema = base.SchemaName ?? session.GetCurrentSchemaQName();
            }
            else
            {
                name.schema = session.GetSchemaQName(name.schema.Name);
                if (name.schema == null)
                {
                    throw Error.GetError(0x1581);
                }
                if (this.IsSchemaDefinition && (base.SchemaName != name.schema))
                {
                    throw Error.GetError(0x1581);
                }
            }
            name.Parent = parent;
            if (!this.IsSchemaDefinition)
            {
                CheckSchemaUpdateAuthorisation(session, name.schema);
            }
            if (check)
            {
                session.database.schemaManager.CheckSchemaObjectNotExists(name);
            }
        }

        public void SetSchemaName(Session session, QNameManager.QName parent, QNameManager.QName name)
        {
            if (name.schema == null)
            {
                name.schema = base.SchemaName ?? session.GetCurrentSchemaQName();
            }
            else
            {
                name.schema = session.GetSchemaQName(name.schema.Name);
                if (name.schema == null)
                {
                    throw Error.GetError(0x1581);
                }
                if (this.IsSchemaDefinition && (base.SchemaName != name.schema))
                {
                    throw Error.GetError(0x1581);
                }
            }
        }
    }
}


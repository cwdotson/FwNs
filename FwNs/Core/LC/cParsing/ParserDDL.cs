namespace FwNs.Core.LC.cParsing
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public class ParserDDL : ParserRoutine
    {
        public static int[] SchemaCommands = new int[] { 0x36, 120 };
        public static short[] StartStatementTokens = new short[] { 0x36, 120, 4, 0x57 };
        public static short[] StartStatementTokensSchema = new short[] { 0x36, 120 };

        public ParserDDL(Session session, Scanner scanner) : base(session, scanner)
        {
        }

        public static void AddForeignKey(Session session, Table table, Constraint c, List<Constraint> constraintList)
        {
            QNameManager.QName mainTableName = c.GetMainTableName();
            if (mainTableName == table.GetName())
            {
                c.Core.MainTable = table;
            }
            else
            {
                Table table2 = session.database.schemaManager.FindUserTable(session, mainTableName.Name, mainTableName.schema.Name);
                if (table2 == null)
                {
                    if (constraintList == null)
                    {
                        throw Error.GetError(0x157d, mainTableName.Name);
                    }
                    constraintList.Add(c);
                    return;
                }
                c.Core.MainTable = table2;
            }
            c.SetColumnsIndexes(table);
            Constraint uniqueConstraintForColumns = c.Core.MainTable.GetUniqueConstraintForColumns(c.Core.MainCols, c.Core.RefCols);
            if (uniqueConstraintForColumns == null)
            {
                throw Error.GetError(0x1593);
            }
            Index mainIndex = uniqueConstraintForColumns.GetMainIndex();
            new TableWorks(session, table).CheckCreateForeignKey(c);
            bool forward = c.Core.MainTable.GetSchemaName() != table.GetSchemaName();
            int tableIndex = session.database.schemaManager.GetTableIndex(table);
            if ((tableIndex != -1) && (tableIndex < session.database.schemaManager.GetTableIndex(c.Core.MainTable)))
            {
                forward = true;
            }
            QNameManager.QName name = session.database.NameManager.NewAutoName("IDX", table.GetSchemaName(), table.GetName(), 20);
            Index index2 = table.CreateAndAddIndexStructure(session, name, c.Core.RefCols, null, null, false, true, forward);
            QNameManager.QName name3 = session.database.NameManager.NewAutoName("REF", c.GetName().Name, table.GetSchemaName(), table.GetName(), 20);
            c.Core.UniqueName = uniqueConstraintForColumns.GetName();
            c.Core.MainName = name3;
            c.Core.MainIndex = mainIndex;
            c.Core.RefTable = table;
            c.Core.RefName = c.GetName();
            c.Core.RefIndex = index2;
            c.IsForward = forward;
            table.AddConstraint(c);
            c.Core.MainTable.AddConstraint(new Constraint(name3, c));
            session.database.schemaManager.AddSchemaObject(c);
        }

        public static Table AddTableConstraintDefinitions(Session session, Table table, List<Constraint> tempConstraints, List<Constraint> constraintList, bool addToSchema)
        {
            Constraint c = tempConstraints[0];
            string namepart = (c.GetName() == null) ? null : c.GetName().Name;
            QNameManager.QName indexName = session.database.NameManager.NewAutoName("IDX", namepart, table.GetSchemaName(), table.GetName(), 20);
            c.SetColumnsIndexes(table);
            table.CreatePrimaryKey(indexName, c.Core.MainCols, true);
            if (c.Core.MainCols != null)
            {
                Constraint constraint2 = new Constraint(c.GetName(), table, table.GetPrimaryIndex(), 4);
                table.AddConstraint(constraint2);
                if (addToSchema)
                {
                    session.database.schemaManager.AddSchemaObject(constraint2);
                }
            }
            for (int i = 1; i < tempConstraints.Count; i++)
            {
                c = tempConstraints[i];
                switch (c.ConstType)
                {
                    case 0:
                    {
                        AddForeignKey(session, table, c, constraintList);
                        continue;
                    }
                    case 1:
                    {
                        continue;
                    }
                    case 2:
                        c.SetColumnsIndexes(table);
                        if (table.GetUniqueConstraintForColumns(c.Core.MainCols) != null)
                        {
                            throw Error.GetError(0x1592);
                        }
                        break;

                    case 3:
                    {
                        try
                        {
                            c.PrepareCheckConstraint(session, table, false);
                        }
                        catch (CoreException)
                        {
                            if (!session.IsProcessingScript())
                            {
                                throw;
                            }
                            continue;
                        }
                        table.AddConstraint(c);
                        if (c.IsNotNull())
                        {
                            table.GetColumn(c.NotNullColumnIndex).SetNullable(false);
                            table.SetColumnTypeVars(c.NotNullColumnIndex);
                        }
                        if (addToSchema)
                        {
                            session.database.schemaManager.AddSchemaObject(c);
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                indexName = session.database.NameManager.NewAutoName("IDX", c.GetName().Name, table.GetSchemaName(), table.GetName(), 20);
                Index index = table.CreateAndAddIndexStructure(session, indexName, c.Core.MainCols, null, null, true, true, false);
                Constraint constraint3 = new Constraint(c.GetName(), table, index, 2);
                table.AddConstraint(constraint3);
                if (addToSchema)
                {
                    session.database.schemaManager.AddSchemaObject(constraint3);
                }
            }
            return table;
        }

        public void CheckDatabaseUpdateAuthorisation()
        {
            base.session.CheckAdmin();
            base.session.CheckDdlWrite();
        }

        public void CheckSchemaUpdateAuthorisation(QNameManager.QName schema)
        {
            if (!base.session.IsProcessingLog())
            {
                SqlInvariants.CheckSchemaNameNotSystem(schema.Name);
                if (base.IsSchemaDefinition)
                {
                    if (schema != base.session.GetCurrentSchemaQName())
                    {
                        throw Error.GetError(0x1581);
                    }
                }
                else
                {
                    base.session.GetGrantee().CheckSchemaUpdateOrGrantRights(schema.Name);
                }
                base.session.CheckDdlWrite();
            }
        }

        public Statement CompileAlter()
        {
            base.Read();
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x15c)
            {
                if (tokenType > 0x114)
                {
                    if (tokenType == 0x12f)
                    {
                        return this.CompileAlterUser();
                    }
                    if (tokenType == 0x15c)
                    {
                        base.Read();
                        base.CheckIsSimpleName();
                        string tokenString = base.token.TokenString;
                        base.CheckValidCatalogName(tokenString);
                        base.Read();
                        base.ReadThis(0x24f);
                        base.ReadThis(0x11b);
                        return this.CompileRenameObject(base.database.GetCatalogName(), 1);
                    }
                }
                else
                {
                    switch (tokenType)
                    {
                        case 0x101:
                            return base.CompileAlterSpecificRoutine();

                        case 0x114:
                            return this.CompileAlterTable();
                    }
                }
            }
            else
            {
                switch (tokenType)
                {
                    case 0x189:
                        return this.CompileAlterDomain();

                    case 0x1f0:
                    {
                        base.Read();
                        QNameManager.QName name = base.ReadSchemaName();
                        base.ReadThis(0x24f);
                        base.ReadThis(0x11b);
                        return this.CompileRenameObject(name, 2);
                    }
                    case 0x1f8:
                        return this.CompileAlterSequence();

                    case 0x21f:
                        return this.CompileCreateView(true);

                    case 0x240:
                    {
                        base.Read();
                        QNameManager.QName name2 = base.ReadNewSchemaObjectName(20, true);
                        base.ReadThis(0x24f);
                        base.ReadThis(0x11b);
                        return this.CompileRenameObject(name2, 20);
                    }
                }
            }
            throw base.UnexpectedToken();
        }

        public Statement CompileAlterColumn(Table table, ColumnSchema column, int columnIndex)
        {
            int position = base.GetPosition();
            switch (base.token.TokenType)
            {
                case 0x57:
                    base.Read();
                    if (base.token.TokenType == 0x4d)
                    {
                        base.Read();
                        return this.CompileAlterColumnDropDefault(table, column, columnIndex);
                    }
                    if (base.token.TokenType != 0x197)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    return this.CompileAlterColumnDropGenerated(table, column, columnIndex);

                case 0xfc:
                {
                    base.Read();
                    int tokenType = base.token.TokenType;
                    if (tokenType > 0xb5)
                    {
                        switch (tokenType)
                        {
                            case 0xb8:
                                base.Read();
                                return this.CompileAlterColumnSetNullability(table, column, true);

                            case 0x17a:
                                base.Read();
                                base.ReadThis(0x215);
                                return this.CompileAlterColumnDataType(table, column);
                        }
                        break;
                    }
                    if (tokenType == 0x4d)
                    {
                        base.Read();
                        SqlType dataType = column.GetDataType();
                        Expression expr = base.ReadDefaultClause(dataType);
                        return this.CompileAlterColumnSetDefault(table, column, expr);
                    }
                    if (tokenType != 0xb5)
                    {
                        break;
                    }
                    base.Read();
                    base.ReadThis(0xb8);
                    return this.CompileAlterColumnSetNullability(table, column, false);
                }
                case 0x24f:
                    base.Read();
                    base.ReadThis(0x11b);
                    return this.CompileAlterColumnRename(table, column);

                default:
                    goto Label_00FE;
            }
            this.Rewind(position);
            base.Read();
        Label_00FE:
            if ((base.token.TokenType != 0xfc) && (base.token.TokenType != 0x1e3))
            {
                return this.CompileAlterColumnType(table, column);
            }
            if (!column.IsIdentity())
            {
                throw Error.GetError(0x159f);
            }
            return this.CompileAlterColumnSequenceOptions(table, column);
        }

        private Statement CompileAlterColumnDataType(Table table, ColumnSchema column)
        {
            base.ReadTypeDefinition(false, false);
            return new StatementSchema(base.GetLastPart(), 4, null, null, table.GetName());
        }

        private Statement CompileAlterColumnDropDefault(Table table, ColumnSchema column, int columnIndex)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 4, null, table.GetName());
        }

        private Statement CompileAlterColumnDropGenerated(Table table, ColumnSchema column, int columnIndex)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 4, null, table.GetName());
        }

        private Statement CompileAlterColumnRename(Table table, ColumnSchema column)
        {
            base.CheckIsSimpleName();
            QNameManager.QName name = base.ReadNewSchemaObjectName(9, true);
            if (table.FindColumn(name.Name) > -1)
            {
                throw Error.GetError(0x1580, name.Name);
            }
            base.database.schemaManager.CheckColumnIsReferenced(table.GetName(), column.GetName());
            string lastPart = base.GetLastPart();
            object[] args = new object[] { column.GetName(), name };
            return new StatementSchema(lastPart, 0x428, args);
        }

        private Statement CompileAlterColumnSequenceOptions(Table table, ColumnSchema column)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 4, null, table.GetName());
        }

        private Statement CompileAlterColumnSetDefault(Table table, ColumnSchema column, Expression expr)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 4, null, table.GetName());
        }

        private Statement CompileAlterColumnSetNullability(Table table, ColumnSchema column, bool b)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 4, null, table.GetName());
        }

        private Statement CompileAlterColumnType(Table table, ColumnSchema column)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 4, null, table.GetName());
        }

        public Statement CompileAlterDomain()
        {
            base.Read();
            QNameManager.QName schemaQName = base.session.GetSchemaQName(base.token.NamePrefix);
            SqlType domain = base.database.schemaManager.GetDomain(base.token.TokenString, schemaQName.Name, true);
            base.Read();
            switch (base.token.TokenType)
            {
                case 0x57:
                {
                    base.Read();
                    if (base.token.TokenType == 0x4d)
                    {
                        base.Read();
                        return this.CompileAlterDomainDropDefault(domain);
                    }
                    if (base.token.TokenType != 0x2f)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.CheckIsSchemaObjectName();
                    QNameManager.QName name = base.database.schemaManager.GetSchemaObjectName(domain.GetSchemaName(), base.token.TokenString, 5, true);
                    base.Read();
                    return this.CompileAlterDomainDropConstraint(domain, name);
                }
                case 0xfc:
                {
                    base.Read();
                    base.ReadThis(0x4d);
                    Expression e = base.ReadDefaultClause(domain);
                    return this.CompileAlterDomainSetDefault(domain, e);
                }
                case 0x14e:
                    base.Read();
                    if ((base.token.TokenType == 0x2f) || (base.token.TokenType == 0x24))
                    {
                        List<Constraint> constraintList = new List<Constraint>();
                        base.compileContext.CurrentDomain = domain;
                        try
                        {
                            this.ReadConstraint(domain, constraintList);
                        }
                        finally
                        {
                            base.compileContext.CurrentDomain = null;
                        }
                        Constraint c = constraintList[0];
                        return this.CompileAlterDomainAddConstraint(domain, c);
                    }
                    break;

                case 0x24f:
                    base.Read();
                    base.ReadThis(0x11b);
                    return this.CompileRenameObject(domain.GetName(), 13);
            }
            throw base.UnexpectedToken();
        }

        private Statement CompileAlterDomainAddConstraint(SqlType domain, Constraint c)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 3, null, null);
        }

        private Statement CompileAlterDomainDropConstraint(SqlType domain, QNameManager.QName name)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 3, null, null);
        }

        private Statement CompileAlterDomainDropDefault(SqlType domain)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 3, null, null);
        }

        private Statement CompileAlterDomainSetDefault(SqlType domain, Expression e)
        {
            return new StatementSchema(base.GetStatement(base.GetParsePosition(), StartStatementTokens), 3, null, null);
        }

        public Statement CompileAlterSequence()
        {
            base.Read();
            QNameManager.QName schemaQName = base.session.GetSchemaQName(base.token.NamePrefix);
            NumberSequence sequence = base.database.schemaManager.GetSequence(base.token.TokenString, schemaQName.Name, true);
            base.Read();
            if (base.token.TokenType == 0x24f)
            {
                base.Read();
                base.ReadThis(0x11b);
                return this.CompileRenameObject(sequence.GetName(), 7);
            }
            NumberSequence sequence2 = sequence.Duplicate();
            this.ReadSequenceOptions(sequence2, false, true);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { sequence, sequence2 };
            return new StatementSchema(lastPart, 0x86, args);
        }

        private Statement CompileAlterTable()
        {
            base.Read();
            string tokenString = base.token.TokenString;
            QNameManager.QName schemaQName = base.session.GetSchemaQName(base.token.NamePrefix);
            Table table = base.database.schemaManager.GetUserTable(base.session, tokenString, schemaQName.Name);
            base.Read();
            int tokenType = base.token.TokenType;
            if (tokenType > 0x57)
            {
                if (tokenType == 0x14e)
                {
                    base.Read();
                    QNameManager.QName name = null;
                    if (base.token.TokenType == 0x2f)
                    {
                        base.Read();
                        name = base.ReadNewDependentSchemaObjectName(table.GetName(), 5);
                    }
                    switch (base.token.TokenType)
                    {
                        case 0x24:
                            base.Read();
                            return this.CompileAlterTableAddCheckConstraint(table, name);

                        case 0x2a:
                            if (name != null)
                            {
                                throw base.UnexpectedToken();
                            }
                            base.Read();
                            base.CheckIsSimpleName();
                            return this.CompileAlterTableAddColumn(table);

                        case 0x70:
                            base.Read();
                            base.ReadThis(0x1ab);
                            return this.CompileAlterTableAddForeignKeyConstraint(table, name);

                        case 0xd4:
                            base.Read();
                            base.ReadThis(0x1ab);
                            return this.CompileAlterTableAddPrimaryKey(table, name);

                        case 0x129:
                            base.Read();
                            return this.CompileAlterTableAddUniqueConstraint(table, name);
                    }
                    if (name != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.CheckIsSimpleName();
                    return this.CompileAlterTableAddColumn(table);
                }
                if (tokenType == 0x24f)
                {
                    base.Read();
                    base.ReadThis(0x11b);
                    return this.CompileRenameObject(table.GetName(), 3);
                }
            }
            else
            {
                if (tokenType == 4)
                {
                    base.Read();
                    if (base.token.TokenType == 0x2a)
                    {
                        base.Read();
                    }
                    int columnIndex = table.GetColumnIndex(base.token.TokenString);
                    ColumnSchema column = table.GetColumn(columnIndex);
                    base.Read();
                    return this.CompileAlterColumn(table, column, columnIndex);
                }
                if (tokenType == 0x57)
                {
                    base.Read();
                    int num3 = base.token.TokenType;
                    switch (num3)
                    {
                        case 0x2a:
                            base.Read();
                            break;

                        case 0x2f:
                            base.Read();
                            return this.CompileAlterTableDropConstraint(table);

                        default:
                            if (num3 != 0xd4)
                            {
                                break;
                            }
                            base.Read();
                            base.ReadThis(0x1ab);
                            return this.CompileAlterTableDropPrimaryKey(table);
                    }
                    base.CheckIsSimpleName();
                    string colName = base.token.TokenString;
                    bool cascade = false;
                    base.Read();
                    if (base.token.TokenType == 0x1e4)
                    {
                        base.Read();
                    }
                    else if (base.token.TokenType == 0x15b)
                    {
                        base.Read();
                        cascade = true;
                    }
                    return CompileAlterTableDropColumn(table, colName, cascade);
                }
            }
            throw base.UnexpectedToken();
        }

        public Statement CompileAlterTableAddCheckConstraint(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.database.NameManager.NewAutoName("CT", table.GetSchemaName(), table.GetName(), 5);
            }
            Constraint c = new Constraint(name, null, 3);
            this.ReadCheckConstraintCondition(c);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { c };
            return new StatementSchema(lastPart, 4, args, null, table.GetName());
        }

        public Statement CompileAlterTableAddColumn(Table table)
        {
            int columnCount = table.GetColumnCount();
            List<Constraint> constraintList = new List<Constraint>();
            Constraint item = new Constraint(null, null, 5);
            constraintList.Add(item);
            base.CheckIsSchemaObjectName();
            QNameManager.QName qName = base.database.NameManager.NewColumnQName(table.GetName(), base.token.TokenString, base.IsDelimitedIdentifier());
            base.Read();
            ColumnSchema schema = this.ReadColumnDefinitionOrNull(table, qName, constraintList);
            if (schema == null)
            {
                throw Error.GetError(0x1388);
            }
            if (base.token.TokenType == 0x157)
            {
                base.Read();
                columnCount = table.GetColumnIndex(base.token.TokenString);
                base.Read();
            }
            string lastPart = base.GetLastPart();
            object[] args = new object[] { schema, columnCount, constraintList };
            return new StatementSchema(lastPart, 4, args, null, table.GetName());
        }

        public Statement CompileAlterTableAddForeignKeyConstraint(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.database.NameManager.NewAutoName("FK", table.GetSchemaName(), table.GetName(), 5);
            }
            OrderedHashSet<string> refColSet = base.ReadColumnNames(false);
            Constraint constraint = this.ReadFKReferences(table, name, refColSet);
            QNameManager.QName mainTableName = constraint.GetMainTableName();
            constraint.Core.MainTable = base.database.schemaManager.GetTable(base.session, mainTableName.Name, mainTableName.schema.Name);
            constraint.SetColumnsIndexes(table);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { constraint };
            return new StatementSchema(lastPart, 4, args, constraint.Core.MainTableName, table.GetName());
        }

        public Statement CompileAlterTableAddPrimaryKey(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.session.database.NameManager.NewAutoName("PK", table.GetSchemaName(), table.GetName(), 5);
            }
            OrderedHashSet<string> mainCols = base.ReadColumnNames(false);
            Constraint constraint = new Constraint(name, mainCols, 4);
            constraint.SetColumnsIndexes(table);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { constraint };
            return new StatementSchema(lastPart, 4, args, null, table.GetName());
        }

        public Statement CompileAlterTableAddUniqueConstraint(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.database.NameManager.NewAutoName("CT", table.GetSchemaName(), table.GetName(), 5);
            }
            int[] numArray = this.ReadColumnList(table, false);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { numArray, name };
            return new StatementSchema(lastPart, 4, args, null, table.GetName());
        }

        public static Statement CompileAlterTableDropColumn(Table table, string colName, bool cascade)
        {
            QNameManager.QName writeName = null;
            int columnIndex = table.GetColumnIndex(colName);
            if (table.GetColumnCount() == 1)
            {
                throw Error.GetError(0x15d7);
            }
            object[] args = new object[] { table.GetColumn(columnIndex).GetName(), 5, cascade, false };
            if (!table.IsTemp())
            {
                writeName = table.GetName();
            }
            return new StatementSchema(null, 0x421, args, null, writeName);
        }

        private Statement CompileAlterTableDropConstraint(Table t)
        {
            bool cascade = false;
            ISchemaObject obj2 = base.ReadSchemaObjectName(t.GetSchemaName(), 5);
            if (base.token.TokenType == 0x1e4)
            {
                base.Read();
            }
            else if (base.token.TokenType == 0x15b)
            {
                base.Read();
                cascade = true;
            }
            object[] args = new object[] { obj2.GetName(), 5, cascade, false };
            return new StatementSchema(base.GetLastPart(), 0x423, args) { WriteTableNames = this.GetReferenceArray(t.GetName(), cascade) };
        }

        private Statement CompileAlterTableDropPrimaryKey(Table t)
        {
            bool cascade = false;
            if (base.token.TokenType == 0x1e4)
            {
                base.Read();
            }
            else if (base.token.TokenType == 0x15b)
            {
                base.Read();
                cascade = true;
            }
            if (!t.HasPrimaryKey())
            {
                throw Error.GetError(0x157d);
            }
            ISchemaObject primaryConstraint = t.GetPrimaryConstraint();
            object[] args = new object[] { primaryConstraint.GetName(), 5, cascade, false };
            return new StatementSchema(base.GetLastPart(), 0x423, args) { WriteTableNames = this.GetReferenceArray(t.GetName(), cascade) };
        }

        public Statement CompileAlterUser()
        {
            QNameManager.QName schemaQName;
            base.Read();
            QNameManager.QName name = this.ReadNewUserIdentifier();
            User user = base.database.GetUserManager().Get(name.Name);
            if (name.Name.Equals("PUBLIC"))
            {
                throw Error.GetError(0x157f);
            }
            if (base.token.TokenType == 0x256)
            {
                base.Read();
                base.ReadThis(0x17);
                string str = this.ReadPassword();
                object[] objArray1 = new object[] { user, str };
                return new StatementCommand(0x413, objArray1);
            }
            if (base.token.TokenType != 0xfc)
            {
                throw base.UnexpectedToken();
            }
            base.Read();
            base.ReadThis(0x241);
            base.ReadThis(0x1f0);
            if (base.token.TokenType == 0x4d)
            {
                schemaQName = null;
            }
            else
            {
                schemaQName = base.database.schemaManager.GetSchemaQName(base.token.TokenString);
            }
            base.Read();
            object[] args = new object[] { user, schemaQName };
            return new StatementCommand(0x412, args);
        }

        public StatementSchema CompileComment()
        {
            QNameManager.QName name;
            base.ReadThis(0x335);
            base.ReadThis(0xc0);
            int tokenType = base.token.TokenType;
            if (tokenType != 0x2a)
            {
                if ((tokenType != 0x114) && (tokenType != 490))
                {
                    throw base.UnexpectedToken();
                }
                int type = (base.token.TokenType == 490) ? 0x12 : 3;
                base.Read();
                base.CheckIsSchemaObjectName();
                name = base.database.NameManager.NewQName(base.token.TokenString, base.token.IsDelimitedIdentifier, type);
                if (base.token.NamePrefix == null)
                {
                    name.schema = base.session.GetCurrentSchemaQName();
                }
                else
                {
                    name.schema = base.database.NameManager.NewQName(base.token.NamePrefix, base.token.IsDelimitedPrefix, 2);
                }
                base.Read();
            }
            else
            {
                base.Read();
                base.CheckIsSchemaObjectName();
                name = base.database.NameManager.NewQName(base.token.TokenString, base.token.IsDelimitedIdentifier, 9);
                if (base.token.NamePrefix == null)
                {
                    throw Error.GetError(0x157d);
                }
                name.Parent = base.database.NameManager.NewQName(base.token.NamePrefix, base.token.IsDelimitedPrefix, 3);
                if (base.token.NamePrePrefix == null)
                {
                    name.Parent.schema = base.session.GetCurrentSchemaQName();
                }
                else
                {
                    name.Parent.schema = base.database.NameManager.NewQName(base.token.NamePrePrefix, base.token.IsDelimitedPrePrefix, 3);
                }
                base.Read();
            }
            base.ReadThis(140);
            string str = base.ReadQuotedString();
            object[] args = new object[] { name, str };
            return new StatementSchema(null, 0x818, args);
        }

        public override StatementSchema CompileCreate()
        {
            int type = 4;
            bool flag = false;
            base.Read();
            bool flag2 = false;
            if (base.token.TokenType == 0xc3)
            {
                base.Read();
                base.ReadThis(0x28d);
                flag2 = true;
            }
            switch (base.token.TokenType)
            {
                case 0x77:
                    base.Read();
                    base.ReadThis(0x209);
                    base.ReadIfThis(0x246);
                    base.ReadThis(0x114);
                    flag = true;
                    type = 3;
                    break;

                case 0x114:
                    base.Read();
                    flag = true;
                    type = base.database.schemaManager.GetDefaultTableType();
                    break;

                case 0x209:
                    base.Read();
                    base.ReadThis(0x114);
                    flag = true;
                    type = 3;
                    break;

                case 0x22d:
                    base.Read();
                    base.ReadThis(0x114);
                    flag = true;
                    type = 5;
                    break;

                case 0x246:
                    base.Read();
                    base.ReadThis(0x114);
                    flag = true;
                    break;

                case 0x253:
                    base.Read();
                    base.ReadThis(0x114);
                    flag = true;
                    type = 3;
                    break;
            }
            if (flag)
            {
                return this.CompileCreateTableBody(type);
            }
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x1e9)
            {
                switch (tokenType)
                {
                    case 0x74:
                    case 0xd5:
                        goto Label_02B3;

                    case 0x121:
                        return this.CompileCreateTrigger();

                    case 0x129:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.CheckIsThis(0x240);
                        return this.CompileCreateIndex(true);

                    case 0x12f:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateUser();

                    case 0x189:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateDomain();

                    case 0x1e9:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateRole();
                }
                goto Label_02C4;
            }
            if (tokenType <= 0x215)
            {
                switch (tokenType)
                {
                    case 0x1f0:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateSchema();

                    case 0x1f8:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateSequence();

                    case 0x215:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateType();
                }
                goto Label_02C4;
            }
            if (tokenType <= 0x227)
            {
                switch (tokenType)
                {
                    case 0x21f:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateView(false);

                    case 0x227:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        return this.CompileCreateAlias();
                }
                goto Label_02C4;
            }
            if (tokenType == 0x240)
            {
                if (flag2)
                {
                    throw base.UnexpectedToken();
                }
                return this.CompileCreateIndex(false);
            }
            if (tokenType != 0x332)
            {
                goto Label_02C4;
            }
        Label_02B3:
            if (flag2)
            {
                throw base.UnexpectedToken();
            }
            return base.CompileCreateProcedureOrFunction();
        Label_02C4:
            throw base.UnexpectedToken();
        }

        public StatementSchema CompileCreateAlias()
        {
            throw base.UnsupportedFeature();
        }

        public StatementSchema CompileCreateDomain()
        {
            bool flag;
            base.Read();
            base.ReadIfThis(9);
            SqlType dataType = base.ReadTypeDefinition(false, false).Duplicate();
            Expression defaultExpression = null;
            if (base.ReadIfThis(0x4d))
            {
                defaultExpression = base.ReadDefaultClause(dataType);
            }
            UserTypeModifier modifier = new UserTypeModifier(base.ReadNewSchemaObjectName(13, false), 13, dataType);
            modifier.SetDefaultClause(defaultExpression);
            dataType.userTypeModifier = modifier;
            List<Constraint> constraintList = new List<Constraint>();
            base.compileContext.CurrentDomain = dataType;
            do
            {
                flag = false;
                switch (base.token.TokenType)
                {
                    case 0x24:
                    case 0x2f:
                        this.ReadConstraint(dataType, constraintList);
                        break;

                    default:
                        flag = true;
                        break;
                }
            }
            while (!flag);
            base.compileContext.CurrentDomain = null;
            for (int i = 0; i < constraintList.Count; i++)
            {
                Constraint c = constraintList[i];
                c.PrepareCheckConstraint(base.session, null, false);
                modifier.AddConstraint(c);
            }
            string lastPart = base.GetLastPart();
            object[] args = new object[] { dataType };
            return new StatementSchema(lastPart, 0x17, args);
        }

        private StatementSchema CompileCreateIndex(bool unique)
        {
            base.Read();
            QNameManager.QName name = base.ReadNewSchemaObjectName(20, true);
            base.ReadThis(0xc0);
            Table table = base.ReadTableName();
            QNameManager.QName schemaName = table.GetSchemaName();
            name.SetSchemaIfNull(schemaName);
            name.Parent = table.GetName();
            if (name.schema != schemaName)
            {
                throw Error.GetError(0x1581);
            }
            name.schema = table.GetSchemaName();
            int[] numArray = this.ReadColumnList(table, true);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { table, numArray, name, unique };
            return new StatementSchema(lastPart, 0x41c, args, null, table.GetName());
        }

        public StatementSchema CompileCreateRole()
        {
            base.Read();
            QNameManager.QName name = this.ReadNewUserIdentifier();
            string lastPart = base.GetLastPart();
            object[] args = new object[] { name };
            return new StatementSchema(lastPart, 0x3d, args);
        }

        private StatementSchema CompileCreateSchema()
        {
            QNameManager.QName lobsSchemaQname = null;
            string tokenString = null;
            bool flag;
            base.Read();
            if (base.token.TokenType != 14)
            {
                lobsSchemaQname = base.ReadNewSchemaName();
            }
            if (base.token.TokenType == 14)
            {
                base.Read();
                base.CheckIsSimpleName();
                tokenString = base.token.TokenString;
                base.Read();
                if (lobsSchemaQname == null)
                {
                    Grantee grantee2 = base.database.GetGranteeManager().Get(tokenString);
                    if (grantee2 == null)
                    {
                        throw Error.GetError(0xfa1, tokenString);
                    }
                    lobsSchemaQname = base.database.NameManager.NewQName(grantee2.GetName().Name, base.IsDelimitedIdentifier(), 2);
                    SqlInvariants.CheckSchemaNameNotSystem(base.token.TokenString);
                }
            }
            if ("PUBLIC".Equals(tokenString))
            {
                throw Error.GetError(0xfa2, tokenString);
            }
            Grantee owner = (tokenString == null) ? base.session.GetGrantee() : base.database.GetGranteeManager().Get(tokenString);
            if (owner == null)
            {
                throw Error.GetError(0xfa1, tokenString);
            }
            if (!base.session.GetGrantee().IsSchemaCreator())
            {
                throw Error.GetError(0x7d0, base.session.GetGrantee().GetNameString());
            }
            if (base.database.schemaManager.SchemaExists(lobsSchemaQname.Name))
            {
                throw Error.GetError(0x1580, lobsSchemaQname.Name);
            }
            if (lobsSchemaQname.Name.Equals("SYSTEM_LOBS"))
            {
                lobsSchemaQname = SqlInvariants.LobsSchemaQname;
                owner = lobsSchemaQname.Owner;
            }
            string lastPart = base.GetLastPart();
            object[] args = new object[] { lobsSchemaQname, owner };
            List<StatementSchema> list = new List<StatementSchema>();
            StatementSchema item = new StatementSchema(lastPart, 0x40, args, null, null);
            item.SetSchemaQName(lobsSchemaQname);
            list.Add(item);
            this.GetCompiledStatementBody(list);
            StatementSchema[] statements = list.ToArray();
            do
            {
                flag = false;
                for (int i = 0; i < (statements.Length - 1); i++)
                {
                    if (statements[i].Order > statements[i + 1].Order)
                    {
                        StatementSchema schema2 = statements[i + 1];
                        statements[i + 1] = statements[i];
                        statements[i] = schema2;
                        flag = true;
                    }
                }
            }
            while (flag);
            return new StatementSchemaDefinition(statements);
        }

        public StatementSchema CompileCreateSequence()
        {
            base.Read();
            NumberSequence sequence = new NumberSequence(base.ReadNewSchemaObjectName(7, false), SqlType.SqlInteger);
            this.ReadSequenceOptions(sequence, true, false);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { sequence };
            return new StatementSchema(lastPart, 0x85, args);
        }

        private StatementSchema CompileCreateTableBody(Table table)
        {
            List<Constraint> constraintList = new List<Constraint>();
            if (base.token.TokenType == 9)
            {
                return this.ReadTableAsSubqueryDefinition(table);
            }
            int position = base.GetPosition();
            base.ReadThis(0x2b7);
            Constraint item = new Constraint(null, null, 5);
            constraintList.Add(item);
            bool flag = true;
            bool flag2 = true;
            bool flag3 = false;
            while (!flag3)
            {
                switch (base.token.TokenType)
                {
                    case 0x24:
                    case 0x2f:
                    case 0x70:
                    case 0x129:
                    case 0xd4:
                    {
                        if (!flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        this.ReadConstraint(table, constraintList);
                        flag = false;
                        flag2 = false;
                        continue;
                    }
                    case 0x2aa:
                    {
                        base.Read();
                        flag3 = true;
                        continue;
                    }
                    case 0x2ac:
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        break;

                    case 0x98:
                    {
                        ColumnSchema[] schemaArray = this.ReadLikeTable(table);
                        for (int i = 0; i < schemaArray.Length; i++)
                        {
                            table.AddColumn(schemaArray[i]);
                        }
                        flag = false;
                        flag2 = false;
                        continue;
                    }
                    default:
                        goto Label_013A;
                }
                base.Read();
                flag2 = true;
                continue;
            Label_013A:
                if (!flag2)
                {
                    throw base.UnexpectedToken();
                }
                base.CheckIsSchemaObjectName();
                QNameManager.QName qName = base.database.NameManager.NewColumnQName(table.GetName(), base.token.TokenString, base.IsDelimitedIdentifier());
                base.Read();
                ColumnSchema column = this.ReadColumnDefinitionOrNull(table, qName, constraintList);
                if (column == null)
                {
                    if (!flag)
                    {
                        throw Error.GetError(0x1388);
                    }
                    this.Rewind(position);
                    return this.ReadTableAsSubqueryDefinition(table);
                }
                table.AddColumn(column);
                flag = false;
                flag2 = false;
            }
            if (base.token.TokenType == 0xc0)
            {
                if (!table.IsTemp())
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                base.ReadThis(0x2b);
                if (base.token.TokenType == 0x4e)
                {
                    base.Read();
                }
                else if (base.token.TokenType == 0x1db)
                {
                    base.Read();
                    table.PersistenceScope = 0x17;
                }
                base.ReadThis(0xf3);
            }
            object[] args = new object[3];
            args[0] = table;
            args[1] = constraintList;
            return new StatementSchema(base.GetLastPart(), 0x4d, args);
        }

        public StatementSchema CompileCreateTableBody(int type)
        {
            QNameManager.QName tableHsqlName = base.ReadNewSchemaObjectName(3, false);
            tableHsqlName.SetSchemaIfNull(base.session.GetCurrentSchemaQName());
            Table table = TableUtil.NewTable(base.database, type, tableHsqlName);
            return this.CompileCreateTableBody(table);
        }

        public StatementSchema CompileCreateTrigger()
        {
            int timing;
            int operationType;
            TriggerDef def;
            bool flag3;
            bool? nullable = null;
            bool noWait = false;
            bool flag2 = false;
            int queueSize = 0;
            QNameManager.QName name = null;
            OrderedHashSet<string> set = null;
            int[] updateColumns = null;
            base.Read();
            QNameManager.QName name2 = base.ReadNewSchemaObjectName(8, true);
            int tokenType = base.token.TokenType;
            switch (tokenType)
            {
                case 0x150:
                case 0x157:
                    timing = TriggerDef.GetTiming(base.token.TokenType);
                    base.Read();
                    break;

                default:
                    if (tokenType != 0x1a6)
                    {
                        throw base.UnexpectedToken();
                    }
                    timing = TriggerDef.GetTiming(0x1a6);
                    base.Read();
                    base.ReadThis(0xbd);
                    break;
            }
            int num4 = base.token.TokenType;
            switch (num4)
            {
                case 0x4e:
                case 0x85:
                    operationType = TriggerDef.GetOperationType(base.token.TokenType);
                    base.Read();
                    break;

                default:
                    if (num4 != 0x12d)
                    {
                        throw base.UnexpectedToken();
                    }
                    operationType = TriggerDef.GetOperationType(base.token.TokenType);
                    base.Read();
                    if ((base.token.TokenType == 0xbd) && (timing != 6))
                    {
                        base.Read();
                        set = new OrderedHashSet<string>();
                        base.ReadColumnNameList(set, null, false);
                    }
                    break;
            }
            base.ReadThis(0xc0);
            Table table = base.ReadTableName();
            if (base.token.TokenType == 0x157)
            {
                base.Read();
                base.CheckIsSimpleName();
                name = base.ReadNewSchemaObjectName(8, true);
                name.SetSchemaIfNull(table.GetSchemaName());
            }
            name2.SetSchemaIfNull(table.GetSchemaName());
            this.CheckSchemaUpdateAuthorisation(name2.schema);
            if (timing == 6)
            {
                if (!table.IsView() || (((View) table).GetCheckOption() == 2))
                {
                    throw Error.GetError(0x15a2, name2.schema.Name);
                }
            }
            else if (table.IsView())
            {
                throw Error.GetError(0x15a2, name2.schema.Name);
            }
            if (name2.schema != table.GetSchemaName())
            {
                throw Error.GetError(0x1581, name2.schema.Name);
            }
            name2.Parent = table.GetName();
            base.database.schemaManager.CheckSchemaObjectNotExists(name2);
            if (set != null)
            {
                updateColumns = table.GetColumnIndexes(set);
                for (int i = 0; i < updateColumns.Length; i++)
                {
                    if (updateColumns[i] == -1)
                    {
                        throw Error.GetError(0x15a8, set.Get(i));
                    }
                }
            }
            Expression condition = null;
            string tokenString = null;
            string tokenString = null;
            string tokenString = null;
            string tokenString = null;
            Table[] transitions = new Table[4];
            RangeVariable[] rangeVarArray = new RangeVariable[4];
            string conditionSql = null;
            if (base.token.TokenType != 0xdd)
            {
                goto Label_06D2;
            }
            base.Read();
            if ((base.token.TokenType != 0xbf) && (base.token.TokenType != 0xb1))
            {
                throw base.UnexpectedToken();
            }
        Label_02BE:
            while (base.token.TokenType == 0xbf)
            {
                if (operationType == 50)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                if (base.token.TokenType == 0x114)
                {
                    flag3 = true;
                    if ((flag3.Equals(nullable) || (tokenString != null)) || (timing == 4))
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadIfThis(9);
                    base.CheckIsSimpleName();
                    tokenString = base.token.TokenString;
                    base.Read();
                    string str7 = tokenString;
                    if ((str7.Equals(tokenString) || str7.Equals(tokenString)) || str7.Equals(tokenString))
                    {
                        throw base.UnexpectedToken();
                    }
                    nullable = false;
                    QNameManager.QName name3 = base.database.NameManager.NewQName(table.GetSchemaName(), str7, base.IsDelimitedIdentifier(), 10);
                    Table table2 = new Table(table, name3);
                    table2.CreatePrimaryKey();
                    RangeVariable variable = new RangeVariable(table2, null, null, null, base.compileContext);
                    transitions[2] = table2;
                    rangeVarArray[2] = variable;
                }
                else
                {
                    flag3 = false;
                    if (flag3.Equals(nullable) || (tokenString != null))
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadIfThis(9);
                    base.CheckIsSimpleName();
                    tokenString = base.token.TokenString;
                    base.Read();
                    string str8 = tokenString;
                    if ((str8.Equals(tokenString) || str8.Equals(tokenString)) || str8.Equals(tokenString))
                    {
                        throw base.UnexpectedToken();
                    }
                    nullable = true;
                    QNameManager.QName name4 = base.database.NameManager.NewQName(table.GetSchemaName(), str8, base.IsDelimitedIdentifier(), 10);
                    Table table3 = new Table(table, name4);
                    table3.CreatePrimaryKey();
                    RangeVariable variable2 = new RangeVariable(table3, null, null, null, base.compileContext);
                    transitions[0] = table3;
                    rangeVarArray[0] = variable2;
                }
            }
            if (base.token.TokenType == 0xb1)
            {
                if (operationType == 0x13)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                if (base.token.TokenType == 0x114)
                {
                    flag3 = true;
                    if ((flag3.Equals(nullable) || (tokenString != null)) || (timing == 0x157))
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadIfThis(9);
                    base.CheckIsSimpleName();
                    tokenString = base.token.TokenString;
                    base.Read();
                    nullable = false;
                    string str9 = tokenString;
                    if ((str9.Equals(tokenString) || str9.Equals(tokenString)) || str9.Equals(tokenString))
                    {
                        throw base.UnexpectedToken();
                    }
                    QNameManager.QName name5 = base.database.NameManager.NewQName(table.GetSchemaName(), str9, base.IsDelimitedIdentifier(), 10);
                    Table table4 = new Table(table, name5);
                    table4.CreatePrimaryKey();
                    RangeVariable variable3 = new RangeVariable(table4, null, null, null, base.compileContext);
                    transitions[3] = table4;
                    rangeVarArray[3] = variable3;
                }
                else
                {
                    flag3 = false;
                    if (flag3.Equals(nullable) || (tokenString != null))
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadIfThis(9);
                    base.CheckIsSimpleName();
                    tokenString = base.token.TokenString;
                    base.Read();
                    nullable = true;
                    string str10 = tokenString;
                    if ((str10.Equals(tokenString) || str10.Equals(tokenString)) || str10.Equals(tokenString))
                    {
                        throw base.UnexpectedToken();
                    }
                    QNameManager.QName name6 = base.database.NameManager.NewQName(table.GetSchemaName(), str10, base.IsDelimitedIdentifier(), 10);
                    Table table5 = new Table(table, name6);
                    table5.CreatePrimaryKey();
                    RangeVariable variable4 = new RangeVariable(table5, null, null, null, base.compileContext);
                    transitions[1] = table5;
                    rangeVarArray[1] = variable4;
                }
                goto Label_02BE;
            }
        Label_06D2:
            flag3 = true;
            if (flag3.Equals(nullable) && (base.token.TokenType != 0x6f))
            {
                throw base.UnexpectedToken("FOR");
            }
            if (base.token.TokenType == 0x6f)
            {
                base.Read();
                base.ReadThis(0x59);
                if (base.token.TokenType == 0xf1)
                {
                    flag3 = false;
                    if (flag3.Equals(nullable))
                    {
                        throw base.UnexpectedToken();
                    }
                    nullable = true;
                }
                else
                {
                    if (base.token.TokenType != 0x203)
                    {
                        throw base.UnexpectedToken();
                    }
                    flag3 = true;
                    if (flag3.Equals(nullable) || (timing == 4))
                    {
                        throw base.UnexpectedToken();
                    }
                    nullable = false;
                }
                base.Read();
            }
            RangeVariable variable1 = rangeVarArray[2];
            RangeVariable variable5 = rangeVarArray[3];
            if ("QUEUE".Equals(base.token.TokenString))
            {
                base.Read();
                queueSize = base.ReadInteger();
                flag2 = true;
            }
            if ("NOWAIT".Equals(base.token.TokenString))
            {
                base.Read();
                noWait = true;
            }
            if ((base.token.TokenType == 0x138) && (timing != 6))
            {
                base.Read();
                base.ReadThis(0x2b7);
                int position = base.GetPosition();
                base.IsCheckOrTriggerCondition = true;
                condition = base.XreadBooleanValueExpression();
                conditionSql = base.GetLastPart(position);
                base.IsCheckOrTriggerCondition = false;
                base.ReadThis(0x2aa);
                ExpressionColumn.CheckColumnsResolved(condition.ResolveColumnReferences(rangeVarArray, null));
                condition.ResolveTypes(base.session, null);
                if (condition.GetDataType() != SqlType.SqlBoolean)
                {
                    throw Error.GetError(0x15c0);
                }
            }
            if (!nullable.HasValue)
            {
                nullable = false;
            }
            if (base.token.TokenType == 0x18)
            {
                int position = base.GetPosition();
                try
                {
                    base.Read();
                    base.CheckIsSimpleName();
                    base.CheckIsDelimitedIdentifier();
                    string tokenString = base.token.TokenString;
                    base.Read();
                    def = new TriggerDef(name2, timing, operationType, nullable.Value, table, transitions, rangeVarArray, condition, conditionSql, updateColumns, tokenString, noWait, queueSize);
                    string sql = base.GetLastPart();
                    object[] objArray1 = new object[] { def, name };
                    return new StatementSchema(sql, 80, objArray1, null, table.GetName());
                }
                catch (CoreException)
                {
                    this.Rewind(position);
                }
            }
            if (flag2)
            {
                throw base.UnexpectedToken("QUEUE");
            }
            if (noWait)
            {
                throw base.UnexpectedToken("NOWAIT");
            }
            Routine routine = this.CompileTriggerRoutine(table, rangeVarArray, timing, operationType);
            def = new TriggerDefSQL(name2, timing, operationType, nullable.Value, table, transitions, rangeVarArray, condition, conditionSql, updateColumns, routine);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { def, name };
            return new StatementSchema(lastPart, 80, args, null, table.GetName());
        }

        public StatementSchema CompileCreateType()
        {
            SqlType type;
            base.Read();
            QNameManager.QName name = base.ReadNewSchemaObjectName(12, false);
            base.ReadThis(9);
            if (base.token.TokenType == 0x114)
            {
                base.Read();
                QNameManager.QName name2 = base.database.NameManager.NewQName(name.Name, name.IsNameQuoted, 3);
                name2.SetSchemaIfNull(SqlInvariants.ModuleQname);
                Table table = new TableDerived(base.database, name2, 11);
                base.ReadTableDefinition(null, table, true);
                base.ReadIfThis(0x2bb);
                type = new TableType(table);
            }
            else
            {
                type = base.ReadTypeDefinition(false, false).Duplicate();
            }
            base.ReadIfThis(400);
            UserTypeModifier modifier = new UserTypeModifier(name, 12, type);
            type.userTypeModifier = modifier;
            string lastPart = base.GetLastPart();
            object[] args = new object[] { type };
            return new StatementSchema(lastPart, 0x53, args);
        }

        public StatementSchema CompileCreateUser()
        {
            bool flag = false;
            Grantee grantee = base.session.GetGrantee();
            base.Read();
            QNameManager.QName name = this.ReadNewUserIdentifier();
            base.ReadThis(0x256);
            base.ReadThis(0x17);
            string str = this.ReadPassword();
            if (base.token.TokenType == 0x14f)
            {
                base.Read();
                flag = true;
            }
            this.CheckDatabaseUpdateAuthorisation();
            string lastPart = base.GetLastPart();
            object[] args = new object[] { name, str, grantee, flag };
            return new StatementSchema(lastPart, 0x41d, args);
        }

        public StatementSchema CompileCreateView(bool alter)
        {
            QueryExpression expression;
            base.Read();
            QNameManager.QName tableName = base.ReadNewSchemaObjectName(4, true);
            tableName.SetSchemaIfNull(base.session.GetCurrentSchemaQName());
            this.CheckSchemaUpdateAuthorisation(tableName.schema);
            QNameManager.QName[] columnNames = null;
            if (base.token.TokenType == 0x2b7)
            {
                columnNames = base.ReadColumnNames(tableName);
            }
            base.ReadThis(9);
            base.StartRecording();
            base.GetPosition();
            try
            {
                expression = base.XreadQueryExpression();
            }
            catch (CoreException)
            {
                expression = base.XreadJoinedTable();
            }
            Token[] recordedStatement = base.GetRecordedStatement();
            int check = 0;
            if (base.token.TokenType == 0x13d)
            {
                base.Read();
                check = 2;
                if (base.ReadIfThis(0x9b))
                {
                    check = 1;
                }
                else
                {
                    base.ReadIfThis(0x1b);
                }
                base.ReadThis(0x24);
                base.ReadThis(0x1c6);
            }
            View view = new View(base.database, tableName, columnNames, check);
            expression.SetView(view);
            expression.Resolve(base.session);
            view.SetStatement(Token.GetSql(recordedStatement));
            string lastPart = base.GetLastPart();
            object[] args = new object[] { view };
            int type = alter ? 0x817 : 0x54;
            StatementQuery query = new StatementQuery(base.session, expression);
            query.SetDatabseObjects(base.session, base.compileContext);
            query.CheckAccessRights(base.session);
            return new StatementSchema(lastPart, type, args) { ReadTableNames = query.ReadTableNames };
        }

        public StatementSession CompileDeclareLocalTableOrNull()
        {
            int position = base.GetPosition();
            try
            {
                base.ReadThis(0x4c);
                base.ReadThis(0x9b);
                base.ReadThis(0x209);
                base.ReadThis(0x114);
            }
            catch (Exception)
            {
                this.Rewind(position);
                return null;
            }
            if (base.token.NamePrePrefix != null)
            {
                throw base.UnexpectedToken();
            }
            if ((base.token.NamePrePrefix != null) || ((base.token.NamePrefix != null) && !"MODULE".Equals(base.token.NamePrefix)))
            {
                throw base.UnexpectedToken();
            }
            QNameManager.QName tableHsqlName = base.ReadNewSchemaObjectName(3, false);
            tableHsqlName.schema = SqlInvariants.ModuleQname;
            Table table = TableUtil.NewTable(base.database, 3, tableHsqlName);
            StatementSchema schema = this.CompileCreateTableBody(table);
            List<Constraint> list = (List<Constraint>) schema.Arguments[1];
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].GetConstraintType() == 0)
                    {
                        throw base.UnexpectedToken("FOREIGN");
                    }
                }
            }
            return new StatementSession(0x814, schema.Arguments);
        }

        public StatementSession CompileDeclareTypedLocalTableOrNull()
        {
            int position = base.GetPosition();
            try
            {
                base.ReadThis(0x4c);
                QNameManager.QName qName = base.ReadNewSchemaObjectName(3, false);
                qName.schema = SqlInvariants.ModuleQname;
                SqlType type = base.ReadTypeDefinition(true, true);
                if (!type.IsTableType())
                {
                    this.Rewind(position);
                    return null;
                }
                if (qName.Name.StartsWith("@"))
                {
                    base.compileContext.AddNoneHostParameter(qName.Name, base.GetPosition());
                }
                List<Constraint> list = new List<Constraint>();
                Constraint item = new Constraint(null, null, 5);
                list.Add(item);
                Table table = base.MakeTableFromType(qName, (TableType) type, 3);
                table.PersistenceScope = 0x17;
                ColumnSchema schema = base.MakeTableVariable(qName, type);
                object[] args = new object[4];
                args[0] = table;
                args[1] = list;
                args[3] = schema;
                return new StatementSession(0x814, args);
            }
            catch (Exception)
            {
                this.Rewind(position);
                return null;
            }
        }

        public Statement CompileDrop()
        {
            int num3;
            int num4;
            QNameManager.QName name;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            QNameManager.QName writeName = null;
            QNameManager.QName catalogName = base.database.GetCatalogName();
            int tokenType = base.token.TokenType;
            int num2 = tokenType;
            if (num2 <= 0x153)
            {
                if (num2 <= 0x101)
                {
                    switch (num2)
                    {
                        case 0x74:
                            base.Read();
                            num3 = 30;
                            num4 = 0x10;
                            writeName = catalogName;
                            flag = true;
                            flag3 = true;
                            goto Label_02B4;

                        case 0xd5:
                            base.Read();
                            num3 = 30;
                            num4 = 0x11;
                            writeName = catalogName;
                            flag = true;
                            flag3 = true;
                            goto Label_02B4;

                        case 0x101:
                            base.Read();
                            switch (base.token.TokenType)
                            {
                                case 0x74:
                                case 0xd5:
                                case 490:
                                    base.Read();
                                    num3 = 30;
                                    num4 = 0x18;
                                    writeName = catalogName;
                                    flag = true;
                                    flag3 = true;
                                    goto Label_02B4;
                            }
                            throw base.UnexpectedToken();
                    }
                    goto Label_02AD;
                }
                if (num2 <= 0x121)
                {
                    if (num2 == 0x114)
                    {
                        base.Read();
                        num3 = 0x20;
                        num4 = 3;
                        writeName = catalogName;
                        flag = true;
                        flag3 = true;
                    }
                    else
                    {
                        if (num2 != 0x121)
                        {
                            goto Label_02AD;
                        }
                        base.Read();
                        num3 = 0x22;
                        num4 = 8;
                        writeName = catalogName;
                        flag = false;
                        flag3 = true;
                    }
                }
                else if (num2 == 0x12f)
                {
                    base.Read();
                    num3 = 0x426;
                    num4 = 11;
                    writeName = catalogName;
                    flag = true;
                }
                else
                {
                    if (num2 != 0x153)
                    {
                        goto Label_02AD;
                    }
                    base.Read();
                    num3 = 0x18;
                    num4 = 6;
                    flag = true;
                }
                goto Label_02B4;
            }
            if (num2 <= 0x1f8)
            {
                if (num2 <= 0x1e9)
                {
                    if (num2 == 0x189)
                    {
                        base.Read();
                        num3 = 0x1b;
                        num4 = 13;
                        writeName = catalogName;
                        flag = true;
                        flag3 = true;
                    }
                    else
                    {
                        if (num2 != 0x1e9)
                        {
                            goto Label_02AD;
                        }
                        base.Read();
                        num3 = 0x1d;
                        num4 = 11;
                        writeName = catalogName;
                        flag = true;
                    }
                }
                else if (num2 == 0x1f0)
                {
                    base.Read();
                    num3 = 0x1f;
                    num4 = 2;
                    writeName = catalogName;
                    flag3 = true;
                    flag = true;
                }
                else
                {
                    if (num2 != 0x1f8)
                    {
                        goto Label_02AD;
                    }
                    base.Read();
                    num3 = 0x87;
                    num4 = 7;
                    writeName = catalogName;
                    flag = true;
                    flag3 = true;
                }
                goto Label_02B4;
            }
            if (num2 <= 0x21f)
            {
                if (num2 == 0x215)
                {
                    base.Read();
                    num3 = 0x23;
                    num4 = 12;
                    writeName = catalogName;
                    flag = true;
                    flag3 = true;
                }
                else
                {
                    if (num2 != 0x21f)
                    {
                        goto Label_02AD;
                    }
                    base.Read();
                    num3 = 0x24;
                    num4 = 4;
                    writeName = catalogName;
                    flag = true;
                    flag3 = true;
                }
                goto Label_02B4;
            }
            switch (num2)
            {
                case 0x240:
                    base.Read();
                    num3 = 0x422;
                    num4 = 20;
                    flag3 = true;
                    writeName = catalogName;
                    goto Label_02B4;

                case 0x332:
                    base.Read();
                    num3 = 30;
                    num4 = 0x1b;
                    writeName = catalogName;
                    flag = true;
                    flag3 = true;
                    goto Label_02B4;
            }
        Label_02AD:
            throw base.UnexpectedToken();
        Label_02B4:
            if (flag3 && (base.token.TokenType == 0x19c))
            {
                int position = base.GetPosition();
                base.Read();
                if (base.token.TokenType == 100)
                {
                    base.Read();
                    flag4 = true;
                }
                else
                {
                    this.Rewind(position);
                }
            }
            base.CheckIsIdentifier();
            switch (tokenType)
            {
                case 0x114:
                    name = base.ReadNewSchemaObjectName(num4, false);
                    if ((base.token.NamePrePrefix == null) && "MODULE".Equals(base.token.NamePrefix))
                    {
                        if ((!flag4 & flag3) && (base.token.TokenType == 0x19c))
                        {
                            base.Read();
                            base.ReadThis(100);
                            flag4 = true;
                        }
                        object[] objArray1 = new object[] { name, flag4 };
                        return new StatementSession(0x20, objArray1);
                    }
                    break;

                case 0x12f:
                    base.CheckIsSimpleName();
                    this.CheckDatabaseUpdateAuthorisation();
                    name = base.database.GetUserManager().Get(base.token.TokenString).GetName();
                    base.Read();
                    break;

                case 0x1e9:
                    base.CheckIsSimpleName();
                    this.CheckDatabaseUpdateAuthorisation();
                    name = base.database.GetGranteeManager().GetRole(base.token.TokenString).GetName();
                    base.Read();
                    break;

                case 0x1f0:
                    name = base.ReadNewSchemaName();
                    writeName = catalogName;
                    break;

                default:
                    name = base.ReadNewSchemaObjectName(num4, false);
                    break;
            }
            if ((!flag4 & flag3) && (base.token.TokenType == 0x19c))
            {
                base.Read();
                base.ReadThis(100);
                flag4 = true;
            }
            if (flag)
            {
                if (base.token.TokenType == 0x15b)
                {
                    flag2 = true;
                    base.Read();
                    if (tokenType == 0x114)
                    {
                        base.ReadIfThis(0x175);
                    }
                }
                else if (base.token.TokenType == 0x1e4)
                {
                    base.Read();
                }
            }
            object[] args = new object[] { name, num4, flag2, flag4 };
            return new StatementSchema(base.GetLastPart(), num3, args, null, writeName);
        }

        public StatementSchema CompileGrantOrRevoke()
        {
            bool grant = base.token.TokenType == 120;
            base.Read();
            if (this.IsGrantToken() || (!grant && ((base.token.TokenType == 120) || (base.token.TokenType == 0x19b))))
            {
                return this.CompileRightGrantOrRevoke(grant);
            }
            return this.CompileRoleGrantOrRevoke(grant);
        }

        public Statement CompileRenameObject(QNameManager.QName name, int objectType)
        {
            QNameManager.QName name2 = base.ReadNewSchemaObjectName(objectType, true);
            string lastPart = base.GetLastPart();
            object[] args = new object[] { name, name2 };
            return new StatementSchema(lastPart, 0x428, args);
        }

        private StatementSchema CompileRightGrantOrRevoke(bool grant)
        {
            Right fullRights;
            int num2;
            QNameManager.QName name;
            OrderedHashSet<string> set = new OrderedHashSet<string>();
            Grantee role = null;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            if (!grant)
            {
                if (base.token.TokenType == 120)
                {
                    base.Read();
                    base.ReadThis(0x1c6);
                    base.ReadThis(0x6f);
                    flag5 = true;
                }
                else if (base.token.TokenType == 0x19b)
                {
                    throw base.UnsupportedFeature();
                }
            }
            if (base.token.TokenType == 2)
            {
                base.Read();
                if (base.token.TokenType == 0x1dd)
                {
                    base.Read();
                }
                fullRights = Right.FullRights;
                flag4 = true;
            }
            else
            {
                fullRights = new Right();
                bool flag7 = true;
                while (flag7)
                {
                    base.CheckIsNotQuoted();
                    int checkSingleRight = GranteeManager.GetCheckSingleRight(base.token.TokenString);
                    OrderedHashSet<string> set2 = null;
                    base.Read();
                    int num5 = base.token.TokenType;
                    if (num5 <= 220)
                    {
                        if (num5 <= 0x63)
                        {
                            if (num5 == 0x4e)
                            {
                                goto Label_018B;
                            }
                            if (num5 != 0x63)
                            {
                                goto Label_0171;
                            }
                            if (flag)
                            {
                                throw base.UnexpectedToken();
                            }
                            fullRights = Right.FullRights;
                            flag3 = true;
                            flag7 = false;
                            continue;
                        }
                        if ((num5 != 0x85) && (num5 != 220))
                        {
                            goto Label_0171;
                        }
                        goto Label_01A5;
                    }
                    if (num5 <= 0x121)
                    {
                        if (num5 == 0xf9)
                        {
                            goto Label_01A5;
                        }
                        if (num5 != 0x121)
                        {
                            goto Label_0171;
                        }
                        goto Label_018B;
                    }
                    if (num5 == 0x12d)
                    {
                        goto Label_01A5;
                    }
                    if (num5 == 0x21a)
                    {
                        if (flag)
                        {
                            throw base.UnexpectedToken();
                        }
                        fullRights = Right.FullRights;
                        flag2 = true;
                        flag7 = false;
                        continue;
                    }
                Label_0171:
                    if (base.token.TokenType != 0x2ac)
                    {
                        break;
                    }
                    base.Read();
                    continue;
                Label_018B:
                    if (fullRights == null)
                    {
                        fullRights = new Right();
                    }
                    fullRights.Set(checkSingleRight, set2);
                    flag = true;
                    goto Label_0171;
                Label_01A5:
                    if (base.token.TokenType == 0x2b7)
                    {
                        set2 = base.ReadColumnNames(false);
                    }
                    goto Label_018B;
                }
            }
            base.ReadThis(0xc0);
            int tokenType = base.token.TokenType;
            if (tokenType > 0x114)
            {
                if (tokenType <= 490)
                {
                    if (tokenType != 0x189)
                    {
                        if (tokenType != 490)
                        {
                            goto Label_0384;
                        }
                        if (!flag3 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num2 = 0x12;
                    }
                    else
                    {
                        if (!flag2 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num2 = 13;
                    }
                    goto Label_03A1;
                }
                switch (tokenType)
                {
                    case 0x1f8:
                        if (!flag2 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num2 = 7;
                        goto Label_03A1;

                    case 0x215:
                        if (!flag2 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num2 = 12;
                        goto Label_03A1;

                    case 0x332:
                        if (!flag3 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num2 = 0x1b;
                        goto Label_03A1;
                }
            }
            else
            {
                if (tokenType <= 0xd5)
                {
                    if (tokenType != 0x74)
                    {
                        if (tokenType != 0xd5)
                        {
                            goto Label_0384;
                        }
                        if (!flag3 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num2 = 0x11;
                    }
                    else
                    {
                        if (!flag3 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num2 = 0x10;
                    }
                    goto Label_03A1;
                }
                switch (tokenType)
                {
                    case 0x101:
                        if (!flag3 && !flag4)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        switch (base.token.TokenType)
                        {
                            case 490:
                            case 0x332:
                            case 0x74:
                            case 0xd5:
                                base.Read();
                                num2 = 0x18;
                                goto Label_03A1;
                        }
                        throw base.UnexpectedToken();
                }
            }
        Label_0384:
            if (!flag && !flag4)
            {
                throw base.UnexpectedToken();
            }
            base.ReadIfThis(0x114);
            num2 = 3;
        Label_03A1:
            name = base.ReadNewSchemaObjectName(num2, false);
            if (grant)
            {
                base.ReadThis(0x11b);
            }
            else
            {
                base.ReadThis(0x72);
            }
        Label_03C4:
            base.CheckIsSimpleName();
            set.Add(base.token.TokenString);
            base.Read();
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_03C4;
            }
            if (grant)
            {
                if (base.token.TokenType == 0x13d)
                {
                    base.Read();
                    base.ReadThis(120);
                    base.ReadThis(0x1c6);
                    flag5 = true;
                }
                if (base.token.TokenType == 410)
                {
                    base.Read();
                    base.ReadThis(0x17);
                    if (base.token.TokenType == 0x44)
                    {
                        base.Read();
                    }
                    else
                    {
                        base.ReadThis(0x3f);
                        if (base.session.GetRole() == null)
                        {
                            throw Error.GetError(0x898);
                        }
                        role = base.session.GetRole();
                    }
                }
            }
            else if (base.token.TokenType == 0x15b)
            {
                flag6 = true;
                base.Read();
            }
            else
            {
                base.ReadThis(0x1e4);
            }
            int type = grant ? 0x30 : 0x3b;
            object[] args = new object[] { set, name, fullRights, role, flag6, flag5 };
            return new StatementSchema(base.GetLastPart(), type, args, null, null);
        }

        private StatementSchema CompileRoleGrantOrRevoke(bool grant)
        {
            Grantee role = base.session.GetGrantee();
            OrderedHashSet<string> set = new OrderedHashSet<string>();
            OrderedHashSet<string> set2 = new OrderedHashSet<string>();
            bool flag = false;
            if (!grant && (base.token.TokenType == 0x14f))
            {
                throw base.UnsupportedFeature();
            }
        Label_0036:
            base.CheckIsSimpleName();
            set.Add(base.token.TokenString);
            base.Read();
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0036;
            }
            if (grant)
            {
                base.ReadThis(0x11b);
            }
            else
            {
                base.ReadThis(0x72);
            }
        Label_0086:
            base.CheckIsSimpleName();
            set2.Add(base.token.TokenString);
            base.Read();
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0086;
            }
            if (grant && (base.token.TokenType == 0x13d))
            {
                throw base.UnsupportedFeature();
            }
            if (base.token.TokenType == 410)
            {
                base.Read();
                base.ReadThis(0x17);
                if (base.token.TokenType == 0x44)
                {
                    base.Read();
                }
                else
                {
                    base.ReadThis(0x3f);
                    if (base.session.GetRole() == null)
                    {
                        throw Error.GetError(0x898);
                    }
                    role = base.session.GetRole();
                }
            }
            if (!grant)
            {
                if (base.token.TokenType == 0x15b)
                {
                    flag = true;
                    base.Read();
                }
                else
                {
                    base.ReadThis(0x1e4);
                }
            }
            int type = grant ? 0x31 : 0x81;
            object[] args = new object[] { set2, set, role, flag };
            return new StatementSchema(base.GetLastPart(), type, args, null, null);
        }

        private Routine CompileTriggerRoutine(Table table, RangeVariable[] ranges, int beforeOrAfter, int operation)
        {
            Routine routine;
            base.compileContext.ClearNoneHostParameters();
            base.compileContext.InRoutine = true;
            try
            {
                int impact = 4;
                Routine routine2 = new Routine(table, ranges, impact, beforeOrAfter, operation);
                base.StartRecording();
                Statement statement = base.CompileSQLProcedureStatementOrNull(routine2, null);
                if (statement == null)
                {
                    throw base.UnexpectedToken();
                }
                string sql = Token.GetSql(base.GetRecordedStatement());
                statement.SetSql(sql);
                routine2.SetProcedure(statement);
                routine2.Resolve(base.session);
                routine = routine2;
            }
            finally
            {
                base.compileContext.ClearNoneHostParameters();
                base.compileContext.InRoutine = false;
            }
            return routine;
        }

        public void GetCompiledStatementBody(List<StatementSchema> list)
        {
            bool flag = false;
            while (!flag)
            {
                int num4;
                StatementSchema item = null;
                int position = base.GetPosition();
                int tokenType = base.token.TokenType;
                if (tokenType > 120)
                {
                    goto Label_02C4;
                }
                if (tokenType != 0x36)
                {
                    goto Label_02A9;
                }
                base.Read();
                int num3 = base.token.TokenType;
                if (num3 <= 0x1e9)
                {
                    if (num3 <= 0x121)
                    {
                        if (num3 <= 0x77)
                        {
                            switch (num3)
                            {
                                case 0x74:
                                    num4 = 14;
                                    item = new StatementSchema(base.GetStatementForRoutine(position, StartStatementTokensSchema), num4, null);
                                    goto Label_02E0;

                                case 0x77:
                                    goto Label_027E;
                            }
                            goto Label_02A2;
                        }
                        switch (num3)
                        {
                            case 0xd5:
                                num4 = 14;
                                item = new StatementSchema(base.GetStatementForRoutine(position, StartStatementTokensSchema), num4, null);
                                goto Label_02E0;

                            case 0x114:
                                goto Label_027E;
                        }
                        if (num3 != 0x121)
                        {
                            goto Label_02A2;
                        }
                        num4 = 80;
                        item = new StatementSchema(base.GetStatement(position, StartStatementTokensSchema), num4, null);
                    }
                    else
                    {
                        if (num3 <= 0x12f)
                        {
                            switch (num3)
                            {
                                case 0x129:
                                case 0x12f:
                                    goto Label_029B;
                            }
                            goto Label_02A2;
                        }
                        switch (num3)
                        {
                            case 0x153:
                                throw base.UnexpectedToken();

                            case 0x189:
                                num4 = 0x17;
                                item = new StatementSchema(base.GetStatement(position, StartStatementTokensSchema), num4, null);
                                goto Label_02E0;
                        }
                        if (num3 != 0x1e9)
                        {
                            goto Label_02A2;
                        }
                        item = this.CompileCreateRole();
                        item.Sql = base.GetLastPart(position);
                    }
                    goto Label_02E0;
                }
                if (num3 <= 0x21f)
                {
                    if (num3 <= 0x1f8)
                    {
                        if (num3 == 0x1f0)
                        {
                            goto Label_029B;
                        }
                        if (num3 != 0x1f8)
                        {
                            goto Label_02A2;
                        }
                        item = this.CompileCreateSequence();
                        item.Sql = base.GetLastPart(position);
                    }
                    else
                    {
                        if (num3 == 0x209)
                        {
                            goto Label_027E;
                        }
                        if (num3 == 0x215)
                        {
                            item = this.CompileCreateType();
                            item.Sql = base.GetLastPart(position);
                        }
                        else
                        {
                            if (num3 != 0x21f)
                            {
                                goto Label_02A2;
                            }
                            num4 = 0x54;
                            item = new StatementSchema(base.GetStatement(position, StartStatementTokensSchema), num4, null);
                        }
                    }
                    goto Label_02E0;
                }
                if (num3 <= 0x240)
                {
                    if (num3 == 0x22d)
                    {
                        goto Label_027E;
                    }
                    if (num3 != 0x240)
                    {
                        goto Label_02A2;
                    }
                    num4 = 0x41c;
                    item = new StatementSchema(base.GetStatement(position, StartStatementTokensSchema), num4, null);
                    goto Label_02E0;
                }
                if ((num3 != 0x246) && (num3 != 0x253))
                {
                    if (num3 != 0x332)
                    {
                        goto Label_02A2;
                    }
                    num4 = 14;
                    item = new StatementSchema(base.GetStatementForRoutine(position, StartStatementTokensSchema), num4, null);
                    goto Label_02E0;
                }
            Label_027E:
                num4 = 0x4d;
                item = new StatementSchema(base.GetStatement(position, StartStatementTokensSchema), num4, null, null, null);
                goto Label_02E0;
            Label_029B:
                throw base.UnexpectedToken();
            Label_02A2:
                throw base.UnexpectedToken();
            Label_02A9:
                if (tokenType != 120)
                {
                    goto Label_02F3;
                }
                item = this.CompileGrantOrRevoke();
                item.Sql = base.GetLastPart(position);
                goto Label_02E0;
            Label_02C4:
                if (tokenType != 0x2bb)
                {
                    if (tokenType != 0x2ec)
                    {
                        goto Label_02F3;
                    }
                    flag = true;
                }
                else
                {
                    base.Read();
                    flag = true;
                }
            Label_02E0:
                if (item != null)
                {
                    item.IsSchemaDefinition = true;
                    list.Add(item);
                }
                continue;
            Label_02F3:
                throw base.UnexpectedToken();
            }
        }

        private QNameManager.QName[] GetReferenceArray(QNameManager.QName objectName, bool cascade)
        {
            if (cascade)
            {
                OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
                base.database.schemaManager.GetCascadingReferencingObjectNames(objectName, set);
                Iterator<QNameManager.QName> iterator = set.GetIterator();
                while (iterator.HasNext())
                {
                    if (iterator.Next().type != 3)
                    {
                        iterator.Remove();
                    }
                }
                set.Add(objectName);
                QNameManager.QName[] a = new QNameManager.QName[set.Size()];
                set.ToArray(a);
                return a;
            }
            return new QNameManager.QName[] { objectName };
        }

        private bool IsGrantToken()
        {
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x85)
            {
                if (tokenType > 0x4e)
                {
                    switch (tokenType)
                    {
                        case 0x63:
                        case 0x85:
                            goto Label_005F;
                    }
                    return false;
                }
                if ((tokenType != 2) && (tokenType != 0x4e))
                {
                    return false;
                }
            }
            else if (tokenType <= 0xf9)
            {
                if ((tokenType != 220) && (tokenType != 0xf9))
                {
                    return false;
                }
            }
            else if ((tokenType != 0x12d) && (tokenType != 0x21a))
            {
                return false;
            }
        Label_005F:
            return true;
        }

        public void ProcessAlter()
        {
            base.session.SetScripting(true);
            base.ReadThis(4);
            int tokenType = base.token.TokenType;
            if (tokenType == 0x114)
            {
                base.Read();
                this.ProcessAlterTable();
            }
            else
            {
                if (tokenType != 0x189)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                this.ProcessAlterDomain();
            }
        }

        public void ProcessAlterColumn(Table table, ColumnSchema column, int columnIndex)
        {
            int position = base.GetPosition();
            switch (base.token.TokenType)
            {
                case 0x57:
                    base.Read();
                    if (base.token.TokenType == 0x4d)
                    {
                        base.Read();
                        new TableWorks(base.session, table).SetColDefaultExpression(columnIndex, null);
                    }
                    else
                    {
                        if (base.token.TokenType != 0x197)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        column.SetIdentity(null);
                        column.SetGeneratingExpression(null);
                        table.SetColumnTypeVars(columnIndex);
                    }
                    return;

                case 0xfc:
                    base.Read();
                    switch (base.token.TokenType)
                    {
                        case 0x4d:
                        {
                            base.Read();
                            SqlType dataType = column.GetDataType();
                            Expression def = base.ReadDefaultClause(dataType);
                            new TableWorks(base.session, table).SetColDefaultExpression(columnIndex, def);
                            return;
                        }
                        case 0xb5:
                            base.Read();
                            base.ReadThis(0xb8);
                            base.session.Commit(false);
                            new TableWorks(base.session, table).SetColNullability(column, false);
                            return;

                        case 0xb8:
                            base.Read();
                            base.session.Commit(false);
                            new TableWorks(base.session, table).SetColNullability(column, true);
                            return;

                        case 0x17a:
                            base.Read();
                            base.ReadThis(0x215);
                            this.ProcessAlterColumnDataType(table, column);
                            return;
                    }
                    this.Rewind(position);
                    break;
            }
            if ((base.token.TokenType != 0xfc) && (base.token.TokenType != 0x1e3))
            {
                this.ProcessAlterColumnType(table, column, true);
            }
            else
            {
                if (!column.IsIdentity())
                {
                    throw Error.GetError(0x159f);
                }
                this.ProcessAlterColumnSequenceOptions(column);
            }
        }

        private void ProcessAlterColumnDataType(Table table, ColumnSchema oldCol)
        {
            this.ProcessAlterColumnType(table, oldCol, false);
        }

        public void ProcessAlterColumnSequenceOptions(ColumnSchema column)
        {
            bool flag;
            OrderedIntHashSet set = new OrderedIntHashSet();
            NumberSequence other = column.GetIdentitySequence().Duplicate();
            do
            {
                flag = false;
                switch (base.token.TokenType)
                {
                    case 0xfc:
                    {
                        base.Read();
                        int tokenType = base.token.TokenType;
                        if (tokenType > 0xb2)
                        {
                            if (tokenType != 0x1a1)
                            {
                                if (tokenType != 0x1b5)
                                {
                                    if (tokenType != 0x1b9)
                                    {
                                        goto Label_0237;
                                    }
                                    if (!set.Add(base.token.TokenType))
                                    {
                                        throw base.UnexpectedToken();
                                    }
                                    base.Read();
                                    long num4 = base.ReadBigint();
                                    other.SetMinValueNoCheck(num4);
                                }
                                else
                                {
                                    if (!set.Add(base.token.TokenType))
                                    {
                                        throw base.UnexpectedToken();
                                    }
                                    base.Read();
                                    long num5 = base.ReadBigint();
                                    other.SetMaxValueNoCheck(num5);
                                }
                            }
                            else
                            {
                                if (!set.Add(base.token.TokenType))
                                {
                                    throw base.UnexpectedToken();
                                }
                                base.Read();
                                base.ReadThis(0x17);
                                long num6 = base.ReadBigint();
                                other.SetIncrement(num6);
                            }
                            break;
                        }
                        if (tokenType == 70)
                        {
                            if (!set.Add(base.token.TokenType))
                            {
                                throw base.UnexpectedToken();
                            }
                            base.Read();
                            other.SetCycle(true);
                            break;
                        }
                        if (tokenType != 0xb2)
                        {
                            goto Label_0237;
                        }
                        base.Read();
                        if (base.token.TokenType == 0x1b5)
                        {
                            other.SetDefaultMaxValue();
                        }
                        else if (base.token.TokenType == 0x1b9)
                        {
                            other.SetDefaultMinValue();
                        }
                        else
                        {
                            if (base.token.TokenType != 70)
                            {
                                throw base.UnexpectedToken();
                            }
                            other.SetCycle(false);
                        }
                        if (!set.Add(base.token.TokenType))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        break;
                    }
                    case 0x1e3:
                    {
                        if (!set.Add(base.token.TokenType))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x13d);
                        long num2 = base.ReadBigint();
                        other.SetStartValue(num2);
                        break;
                    }
                    default:
                        flag = true;
                        break;
                }
            }
            while (!flag);
            other.CheckValues();
            column.GetIdentitySequence().Reset(other);
            return;
        Label_0237:
            throw base.UnexpectedToken();
        }

        private void ProcessAlterColumnType(Table table, ColumnSchema oldCol, bool fullDefinition)
        {
            ColumnSchema schema;
            if (oldCol.IsGenerated())
            {
                throw Error.GetError(0x15b9);
            }
            if (fullDefinition)
            {
                List<Constraint> constraintList = new List<Constraint>();
                Constraint item = table.GetPrimaryConstraint() ?? new Constraint(null, null, 5);
                constraintList.Add(item);
                schema = this.ReadColumnDefinitionOrNull(table, oldCol.GetName(), constraintList);
                if (schema == null)
                {
                    throw Error.GetError(0x1388);
                }
                if (oldCol.IsIdentity() && schema.IsIdentity())
                {
                    throw Error.GetError(0x1595);
                }
                if (constraintList.Count > 1)
                {
                    throw Error.GetError(0x1594);
                }
            }
            else
            {
                SqlType type = base.ReadTypeDefinition(true, false);
                if (oldCol.IsIdentity() && !type.IsIntegralType())
                {
                    throw Error.GetError(0x15b9);
                }
                schema = oldCol.Duplicate();
                schema.SetType(type);
            }
            new TableWorks(base.session, table).RetypeColumn(oldCol, schema);
        }

        public void ProcessAlterDomain()
        {
            QNameManager.QName schemaQName = base.session.GetSchemaQName(base.token.NamePrefix);
            this.CheckSchemaUpdateAuthorisation(schemaQName);
            SqlType dataType = base.database.schemaManager.GetDomain(base.token.TokenString, schemaQName.Name, true);
            base.Read();
            int tokenType = base.token.TokenType;
            switch (tokenType)
            {
                case 0x57:
                    base.Read();
                    if (base.token.TokenType == 0x4d)
                    {
                        base.Read();
                        dataType.userTypeModifier.RemoveDefaultClause();
                    }
                    else
                    {
                        if (base.token.TokenType != 0x2f)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.CheckIsSchemaObjectName();
                        QNameManager.QName name = base.database.schemaManager.GetSchemaObjectName(dataType.GetSchemaName(), base.token.TokenString, 5, true);
                        base.Read();
                        base.database.schemaManager.RemoveSchemaObject(name);
                    }
                    return;

                case 0xfc:
                {
                    base.Read();
                    base.ReadThis(0x4d);
                    Expression defaultExpression = base.ReadDefaultClause(dataType);
                    dataType.userTypeModifier.SetDefaultClause(defaultExpression);
                    return;
                }
            }
            if (tokenType == 0x14e)
            {
                base.Read();
                if ((base.token.TokenType == 0x2f) || (base.token.TokenType == 0x24))
                {
                    List<Constraint> constraintList = new List<Constraint>();
                    base.compileContext.CurrentDomain = dataType;
                    try
                    {
                        this.ReadConstraint(dataType, constraintList);
                    }
                    finally
                    {
                        base.compileContext.CurrentDomain = null;
                    }
                    Constraint c = constraintList[0];
                    c.PrepareCheckConstraint(base.session, null, false);
                    dataType.userTypeModifier.AddConstraint(c);
                    base.database.schemaManager.AddSchemaObject(c);
                    return;
                }
            }
            throw base.UnexpectedToken();
        }

        private void ProcessAlterTable()
        {
            string tokenString = base.token.TokenString;
            QNameManager.QName schemaQName = base.session.GetSchemaQName(base.token.NamePrefix);
            this.CheckSchemaUpdateAuthorisation(schemaQName);
            Table table = base.database.schemaManager.GetUserTable(base.session, tokenString, schemaQName.Name);
            if (table.IsView())
            {
                throw Error.GetError(0x157d, tokenString);
            }
            base.Read();
            int tokenType = base.token.TokenType;
            if (tokenType == 4)
            {
                base.Read();
                if (base.token.TokenType == 0x2a)
                {
                    base.Read();
                }
                int columnIndex = table.GetColumnIndex(base.token.TokenString);
                ColumnSchema column = table.GetColumn(columnIndex);
                base.Read();
                this.ProcessAlterColumn(table, column, columnIndex);
            }
            else
            {
                if (tokenType != 0x14e)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                QNameManager.QName name = null;
                if (base.token.TokenType == 0x2f)
                {
                    base.Read();
                    name = base.ReadNewDependentSchemaObjectName(table.GetName(), 5);
                    base.database.schemaManager.CheckSchemaObjectNotExists(name);
                }
                switch (base.token.TokenType)
                {
                    case 0x24:
                        base.Read();
                        this.ProcessAlterTableAddCheckConstraint(table, name);
                        return;

                    case 0x2a:
                        if (name != null)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.CheckIsSimpleName();
                        this.ProcessAlterTableAddColumn(table);
                        return;

                    case 0x70:
                        base.Read();
                        base.ReadThis(0x1ab);
                        this.ProcessAlterTableAddForeignKeyConstraint(table, name);
                        return;

                    case 0xd4:
                        base.Read();
                        base.ReadThis(0x1ab);
                        this.ProcessAlterTableAddPrimaryKey(table, name);
                        return;

                    case 0x129:
                        base.Read();
                        this.ProcessAlterTableAddUniqueConstraint(table, name);
                        return;
                }
                if (name != null)
                {
                    throw base.UnexpectedToken();
                }
                base.CheckIsSimpleName();
                this.ProcessAlterTableAddColumn(table);
            }
        }

        public void ProcessAlterTableAddCheckConstraint(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.database.NameManager.NewAutoName("CT", table.GetSchemaName(), table.GetName(), 5);
            }
            Constraint c = new Constraint(name, null, 3);
            this.ReadCheckConstraintCondition(c);
            base.session.Commit(false);
            new TableWorks(base.session, table).AddCheckConstraint(c);
        }

        public void ProcessAlterTableAddColumn(Table table)
        {
            int columnCount = table.GetColumnCount();
            List<Constraint> constraintList = new List<Constraint>();
            Constraint item = new Constraint(null, null, 5);
            constraintList.Add(item);
            base.CheckIsSchemaObjectName();
            QNameManager.QName qName = base.database.NameManager.NewColumnQName(table.GetName(), base.token.TokenString, base.IsDelimitedIdentifier());
            base.Read();
            ColumnSchema column = this.ReadColumnDefinitionOrNull(table, qName, constraintList);
            if (column == null)
            {
                throw Error.GetError(0x1388);
            }
            if (base.token.TokenType == 0x157)
            {
                base.Read();
                columnCount = table.GetColumnIndex(base.token.TokenString);
                base.Read();
            }
            base.session.Commit(false);
            new TableWorks(base.session, table).AddColumn(column, columnCount, constraintList);
        }

        public void ProcessAlterTableAddForeignKeyConstraint(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.database.NameManager.NewAutoName("FK", table.GetSchemaName(), table.GetName(), 5);
            }
            OrderedHashSet<string> refColSet = base.ReadColumnNames(false);
            Constraint c = this.ReadFKReferences(table, name, refColSet);
            QNameManager.QName mainTableName = c.GetMainTableName();
            c.Core.MainTable = base.database.schemaManager.GetTable(base.session, mainTableName.Name, mainTableName.schema.Name);
            c.SetColumnsIndexes(table);
            base.session.Commit(false);
            new TableWorks(base.session, table).AddForeignKey(c);
        }

        public void ProcessAlterTableAddPrimaryKey(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.session.database.NameManager.NewAutoName("PK", table.GetSchemaName(), table.GetName(), 5);
            }
            OrderedHashSet<string> mainCols = base.ReadColumnNames(false);
            Constraint constraint = new Constraint(name, mainCols, 4);
            constraint.SetColumnsIndexes(table);
            base.session.Commit(false);
            new TableWorks(base.session, table).AddPrimaryKey(constraint, name);
        }

        public void ProcessAlterTableAddUniqueConstraint(Table table, QNameManager.QName name)
        {
            if (name == null)
            {
                name = base.database.NameManager.NewAutoName("CT", table.GetSchemaName(), table.GetName(), 5);
            }
            int[] cols = this.ReadColumnList(table, false);
            base.session.Commit(false);
            new TableWorks(base.session, table).AddUniqueConstraint(cols, name);
        }

        private void ReadCheckConstraintCondition(Constraint c)
        {
            base.ReadThis(0x2b7);
            base.StartRecording();
            base.IsCheckOrTriggerCondition = true;
            Expression expression = base.XreadBooleanValueExpression();
            base.IsCheckOrTriggerCondition = false;
            base.GetRecordedStatement();
            base.ReadThis(0x2aa);
            c.Check = expression;
        }

        private void ReadColumnConstraints(Table table, ColumnSchema column, List<Constraint> constraintList)
        {
            QNameManager.QName name;
            OrderedHashSet<string> set;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
        Label_0008:
            name = null;
            if (base.token.TokenType == 0x2f)
            {
                base.Read();
                name = base.ReadNewDependentSchemaObjectName(table.GetName(), 5);
            }
            int tokenType = base.token.TokenType;
            if (tokenType <= 0xb5)
            {
                if (tokenType <= 0x4d)
                {
                    if (tokenType != 0x24)
                    {
                        if (tokenType != 0x4d)
                        {
                            goto Label_035A;
                        }
                        if (column.GetDefaultExpression() != null)
                        {
                            base.UnexpectedToken("DEFAULT");
                        }
                        base.Read();
                        Expression expr = base.ReadDefaultClause(column.GetDataType());
                        column.SetDefaultExpression(expr);
                    }
                    else
                    {
                        base.Read();
                        if (name == null)
                        {
                            name = base.database.NameManager.NewAutoName("CT", table.GetSchemaName(), table.GetName(), 5);
                        }
                        Constraint c = new Constraint(name, null, 3);
                        this.ReadCheckConstraintCondition(c);
                        OrderedHashSet<Expression> checkColumnExpressions = c.GetCheckColumnExpressions();
                        for (int i = 0; i < checkColumnExpressions.Size(); i++)
                        {
                            ExpressionColumn column2 = (ExpressionColumn) checkColumnExpressions.Get(i);
                            if (!column.GetName().Name.Equals(column2.GetColumnName()))
                            {
                                throw Error.GetError(0x157d);
                            }
                            if ((column2.GetSchemaName() != null) && (column2.GetSchemaName() != table.GetSchemaName().Name))
                            {
                                throw Error.GetError(0x1581);
                            }
                        }
                        constraintList.Add(c);
                    }
                }
                else
                {
                    if (tokenType == 0x70)
                    {
                        base.Read();
                        base.ReadThis(0x1ab);
                        goto Label_0329;
                    }
                    if (tokenType != 0xb5)
                    {
                        goto Label_035A;
                    }
                    if (flag2 | flag3)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadThis(0xb8);
                    if (name == null)
                    {
                        name = base.database.NameManager.NewAutoName("CT", table.GetSchemaName(), table.GetName(), 5);
                    }
                    Constraint constraint3 = new Constraint(name, null, 3) {
                        Check = new ExpressionLogical(column)
                    };
                    constraintList.Add(constraint3);
                    flag2 = true;
                }
            }
            else if (tokenType <= 0xd4)
            {
                if (tokenType != 0xb8)
                {
                    if (tokenType != 0xd4)
                    {
                        goto Label_035A;
                    }
                    if (flag3 | flag4)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadThis(0x1ab);
                    if (constraintList[0].ConstType == 4)
                    {
                        throw Error.GetError(0x159c);
                    }
                    OrderedHashSet<string> mainCols = new OrderedHashSet<string>();
                    mainCols.Add(column.GetName().Name);
                    if (name == null)
                    {
                        name = base.database.NameManager.NewAutoName("PK", table.GetSchemaName(), table.GetName(), 5);
                    }
                    Constraint constraint4 = new Constraint(name, mainCols, 4);
                    constraintList[0] = constraint4;
                    column.SetPrimaryKey(true);
                    flag4 = true;
                }
                else
                {
                    if ((flag2 | flag3) | flag4)
                    {
                        throw base.UnexpectedToken();
                    }
                    if (name != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    flag3 = true;
                }
            }
            else
            {
                if (tokenType == 220)
                {
                    goto Label_0329;
                }
                if (tokenType != 0x129)
                {
                    goto Label_035A;
                }
                base.Read();
                OrderedHashSet<string> mainCols = new OrderedHashSet<string>();
                mainCols.Add(column.GetName().Name);
                if (name == null)
                {
                    name = base.database.NameManager.NewAutoName("CT", table.GetSchemaName(), table.GetName(), 5);
                }
                Constraint constraint5 = new Constraint(name, mainCols, 2);
                constraintList.Add(constraint5);
            }
        Label_0322:
            if (!flag)
            {
                goto Label_0008;
            }
            return;
        Label_0329:
            set = new OrderedHashSet<string>();
            set.Add(column.GetName().Name);
            Constraint item = this.ReadFKReferences(table, name, set);
            constraintList.Add(item);
            goto Label_0322;
        Label_035A:
            flag = true;
            goto Label_0322;
        }

        private ColumnSchema ReadColumnDefinitionOrNull(Table table, QNameManager.QName qName, List<Constraint> constraintList)
        {
            SqlType sqlInteger;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            Expression expr = null;
            bool isNullable = true;
            Expression defaultExpression = null;
            NumberSequence sequence = null;
            if (base.token.TokenType == 0x197)
            {
                base.Read();
                base.ReadThis(0x151);
                throw base.UnexpectedToken("GENERATED");
            }
            if (base.token.TokenType == 0x7f)
            {
                base.Read();
                flag2 = true;
                flag3 = true;
                sqlInteger = SqlType.SqlInteger;
                if (base.token.TokenType == 0x2b7)
                {
                    sequence = new NumberSequence(null, sqlInteger);
                    base.Read();
                    long num = base.ReadBigint();
                    sequence.SetStartValueNoCheck(num);
                    base.ReadThis(0x2ac);
                    num = base.ReadBigint();
                    sequence.SetIncrement(num);
                    base.ReadThis(0x2aa);
                }
                else
                {
                    sequence = new NumberSequence(null, 0L, 1L, sqlInteger);
                }
            }
            else
            {
                if (base.token.TokenType == 0x2ac)
                {
                    return null;
                }
                if (base.token.TokenType == 0x2aa)
                {
                    return null;
                }
                sqlInteger = base.ReadTypeDefinition(true, false);
            }
            if (!flag && !flag2)
            {
                if (base.token.TokenType == 0x4d)
                {
                    base.Read();
                    defaultExpression = base.ReadDefaultClause(sqlInteger);
                }
                else if ((base.token.TokenType == 0x7f) && !flag2)
                {
                    base.Read();
                    flag2 = true;
                    flag3 = true;
                    if (base.token.TokenType == 0x2b7)
                    {
                        sequence = new NumberSequence(null, sqlInteger);
                        base.Read();
                        long num2 = base.ReadBigint();
                        sequence.SetStartValueNoCheck(num2);
                        base.ReadThis(0x2ac);
                        num2 = base.ReadBigint();
                        sequence.SetIncrement(num2);
                        base.ReadThis(0x2aa);
                    }
                    else
                    {
                        sequence = new NumberSequence(null, 0L, 1L, sqlInteger);
                    }
                }
                else if ((base.token.TokenType == 0x197) && !flag2)
                {
                    base.Read();
                    if (base.token.TokenType == 0x17)
                    {
                        base.Read();
                        base.ReadThis(0x4d);
                    }
                    else
                    {
                        base.ReadThis(0x151);
                        flag4 = true;
                    }
                    base.ReadThis(9);
                    if (base.token.TokenType == 0x7f)
                    {
                        base.Read();
                        sequence = new NumberSequence(null, sqlInteger);
                        sequence.SetAlways(flag4);
                        if (base.token.TokenType == 0x2b7)
                        {
                            base.Read();
                            this.ReadSequenceOptions(sequence, false, false);
                            base.ReadThis(0x2aa);
                        }
                        flag2 = true;
                    }
                    else if (base.token.TokenType == 0x2b7)
                    {
                        if (!flag4)
                        {
                            throw base.UnexpectedTokenRequire("ALWAYS");
                        }
                        flag = true;
                    }
                }
                else if ((base.token.TokenType == 0x7f) && !flag2)
                {
                    base.Read();
                    flag2 = true;
                    flag3 = true;
                    sequence = new NumberSequence(null, 0L, 1L, sqlInteger);
                }
            }
            if (flag)
            {
                base.ReadThis(0x2b7);
                expr = base.XreadValueExpression();
                base.ReadThis(0x2aa);
            }
            ColumnSchema column = new ColumnSchema(qName, sqlInteger, isNullable, false, defaultExpression);
            column.SetGeneratingExpression(expr);
            this.ReadColumnConstraints(table, column, constraintList);
            if ((base.token.TokenType == 0x7f) && !flag2)
            {
                base.Read();
                flag2 = true;
                flag3 = true;
                sequence = new NumberSequence(null, 0L, 1L, sqlInteger);
            }
            if (flag2)
            {
                column.SetIdentity(sequence);
            }
            if (flag3 && !column.IsPrimaryKey())
            {
                OrderedHashSet<string> mainCols = new OrderedHashSet<string>();
                mainCols.Add(column.GetName().Name);
                Constraint constraint = new Constraint(base.database.NameManager.NewAutoName("PK", table.GetSchemaName(), table.GetName(), 5), mainCols, 4);
                constraintList[0] = constraint;
                column.SetPrimaryKey(true);
            }
            return column;
        }

        private int[] ReadColumnList(Table table, bool ascOrDesc)
        {
            OrderedHashSet<string> set = base.ReadColumnNames(ascOrDesc);
            return table.GetColumnIndexes(set);
        }

        private void ReadConstraint(ISchemaObject schemaObject, List<Constraint> constraintList)
        {
            QNameManager.QName name = null;
            if (base.token.TokenType == 0x2f)
            {
                base.Read();
                name = base.ReadNewDependentSchemaObjectName(schemaObject.GetName(), 5);
            }
            switch (base.token.TokenType)
            {
                case 0x24:
                {
                    base.Read();
                    if (name == null)
                    {
                        name = base.database.NameManager.NewAutoName("CT", schemaObject.GetSchemaName(), schemaObject.GetName(), 5);
                    }
                    Constraint c = new Constraint(name, null, 3);
                    this.ReadCheckConstraintCondition(c);
                    constraintList.Add(c);
                    return;
                }
                case 0x70:
                {
                    if (schemaObject.GetName().type != 3)
                    {
                        throw base.UnexpectedTokenRequire("CHECK");
                    }
                    base.Read();
                    base.ReadThis(0x1ab);
                    OrderedHashSet<string> refColSet = base.ReadColumnNames(false);
                    Constraint item = this.ReadFKReferences((Table) schemaObject, name, refColSet);
                    constraintList.Add(item);
                    return;
                }
                case 0xd4:
                {
                    if (schemaObject.GetName().type != 3)
                    {
                        throw base.UnexpectedTokenRequire("CHECK");
                    }
                    base.Read();
                    base.ReadThis(0x1ab);
                    if (constraintList[0].ConstType == 4)
                    {
                        throw Error.GetError(0x159c);
                    }
                    if (name == null)
                    {
                        name = base.database.NameManager.NewAutoName("PK", schemaObject.GetSchemaName(), schemaObject.GetName(), 5);
                    }
                    OrderedHashSet<string> mainCols = base.ReadColumnNames(false);
                    Constraint constraint4 = new Constraint(name, mainCols, 4);
                    constraintList[0] = constraint4;
                    return;
                }
                case 0x129:
                {
                    if (schemaObject.GetName().type != 3)
                    {
                        throw base.UnexpectedTokenRequire("CHECK");
                    }
                    base.Read();
                    OrderedHashSet<string> mainCols = base.ReadColumnNames(false);
                    if (name == null)
                    {
                        name = base.database.NameManager.NewAutoName("CT", schemaObject.GetSchemaName(), schemaObject.GetName(), 5);
                    }
                    Constraint item = new Constraint(name, mainCols, 2);
                    constraintList.Add(item);
                    return;
                }
            }
            if (name != null)
            {
                throw base.UnexpectedToken();
            }
        }

        private Constraint ReadFKReferences(Table refTable, QNameManager.QName constraintName, OrderedHashSet<string> refColSet)
        {
            OrderedHashSet<string> mainCols = null;
            QNameManager.QName schemaName;
            QNameManager.QName name2;
            int num2;
            base.ReadThis(220);
            if (base.token.NamePrefix == null)
            {
                schemaName = refTable.GetSchemaName();
            }
            else
            {
                schemaName = base.database.schemaManager.GetSchemaQName(base.token.NamePrefix);
            }
            if ((refTable.GetSchemaName() == schemaName) && refTable.GetName().Name.Equals(base.token.TokenString))
            {
                name2 = refTable.GetName();
                base.Read();
            }
            else
            {
                name2 = this.ReadFKTableName(schemaName);
            }
            if (base.token.TokenType == 0x2b7)
            {
                mainCols = base.ReadColumnNames(false);
            }
            else
            {
                refTable.GetName();
            }
            int matchType = 0x3b;
            if (base.token.TokenType == 160)
            {
                base.Read();
                int tokenType = base.token.TokenType;
                switch (tokenType)
                {
                    case 0x73:
                        base.Read();
                        matchType = 0x3d;
                        goto Label_00FE;

                    case 0x1d5:
                        throw base.UnsupportedFeature();
                }
                if (tokenType != 0x1fd)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
            }
        Label_00FE:
            num2 = 3;
            int updateAction = 3;
            OrderedIntHashSet set2 = new OrderedIntHashSet();
            while (base.token.TokenType == 0xc0)
            {
                base.Read();
                if (!set2.Add(base.token.TokenType))
                {
                    throw base.UnexpectedToken();
                }
                if (base.token.TokenType == 0x4e)
                {
                    base.Read();
                    if (base.token.TokenType == 0xfc)
                    {
                        base.Read();
                        int tokenType = base.token.TokenType;
                        if (tokenType != 0x4d)
                        {
                            if (tokenType != 0xb8)
                            {
                                throw base.UnexpectedToken();
                            }
                            base.Read();
                            num2 = 2;
                        }
                        else
                        {
                            base.Read();
                            num2 = 4;
                        }
                    }
                    else if (base.token.TokenType == 0x15b)
                    {
                        base.Read();
                        num2 = 0;
                    }
                    else if (base.token.TokenType == 0x1e4)
                    {
                        base.Read();
                    }
                    else
                    {
                        base.ReadThis(0xb2);
                        base.ReadThis(0x14c);
                    }
                }
                else
                {
                    if (base.token.TokenType != 0x12d)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    if (base.token.TokenType == 0xfc)
                    {
                        base.Read();
                        int tokenType = base.token.TokenType;
                        if (tokenType != 0x4d)
                        {
                            if (tokenType != 0xb8)
                            {
                                throw base.UnexpectedToken();
                            }
                            base.Read();
                            updateAction = 2;
                        }
                        else
                        {
                            base.Read();
                            updateAction = 4;
                        }
                    }
                    else if (base.token.TokenType == 0x15b)
                    {
                        base.Read();
                        updateAction = 0;
                    }
                    else
                    {
                        if (base.token.TokenType == 0x1e4)
                        {
                            base.Read();
                            continue;
                        }
                        base.ReadThis(0xb2);
                        base.ReadThis(0x14c);
                    }
                }
            }
            if (constraintName == null)
            {
                constraintName = base.database.NameManager.NewAutoName("FK", refTable.GetSchemaName(), refTable.GetName(), 5);
            }
            return new Constraint(constraintName, refTable.GetName(), refColSet, name2, mainCols, 0, num2, updateAction, matchType);
        }

        private QNameManager.QName ReadFKTableName(QNameManager.QName schema)
        {
            QNameManager.QName name;
            base.CheckIsSchemaObjectName();
            Table table = base.database.schemaManager.FindUserTable(base.session, base.token.TokenString, schema.Name);
            if (table == null)
            {
                name = base.database.NameManager.NewQName(schema, base.token.TokenString, base.IsDelimitedIdentifier(), 3);
            }
            else
            {
                name = table.GetName();
            }
            base.Read();
            return name;
        }

        private ColumnSchema[] ReadLikeTable(Table table)
        {
            base.Read();
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            Table table2 = base.ReadTableName();
            OrderedIntHashSet set = new OrderedIntHashSet();
            while (true)
            {
                bool flag4 = base.token.TokenType == 0x1a0;
                if (!flag4 && (base.token.TokenType != 0x18f))
                {
                    break;
                }
                base.Read();
                int tokenType = base.token.TokenType;
                if (tokenType != 0x7f)
                {
                    if (tokenType != 0x17d)
                    {
                        if (tokenType != 0x197)
                        {
                            throw base.UnexpectedToken();
                        }
                        if (!set.Add(base.token.TokenType))
                        {
                            throw base.UnexpectedToken();
                        }
                        flag = flag4;
                    }
                    else
                    {
                        if (!set.Add(base.token.TokenType))
                        {
                            throw base.UnexpectedToken();
                        }
                        flag3 = flag4;
                    }
                }
                else
                {
                    if (!set.Add(base.token.TokenType))
                    {
                        throw base.UnexpectedToken();
                    }
                    flag2 = flag4;
                }
                base.Read();
            }
            ColumnSchema[] schemaArray = new ColumnSchema[table2.GetColumnCount()];
            for (int i = 0; i < schemaArray.Length; i++)
            {
                ColumnSchema schema = table2.GetColumn(i).Duplicate();
                QNameManager.QName name = base.database.NameManager.NewColumnSchemaQName(table.GetName(), schema.GetName());
                schema.SetName(name);
                schema.SetNullable(true);
                schema.SetPrimaryKey(false);
                if (flag2)
                {
                    if (schema.IsIdentity())
                    {
                        schema.SetIdentity(schema.GetIdentitySequence().Duplicate());
                    }
                }
                else
                {
                    if (schema.IsIdentity())
                    {
                        schema.SetPrimaryKey(false);
                        schema.SetNullable(true);
                    }
                    schema.SetIdentity(null);
                }
                if (!flag3)
                {
                    schema.SetDefaultExpression(null);
                }
                if (!flag)
                {
                    schema.SetGeneratingExpression(null);
                }
                schemaArray[i] = schema;
            }
            return schemaArray;
        }

        public QNameManager.QName ReadNewUserIdentifier()
        {
            base.CheckIsSimpleName();
            string tokenString = base.token.TokenString;
            bool isquoted = base.IsDelimitedIdentifier();
            if (tokenString.Equals("SA", StringComparison.OrdinalIgnoreCase))
            {
                tokenString = "SA";
                isquoted = false;
            }
            base.Read();
            return base.database.NameManager.NewQName(tokenString, isquoted, 11);
        }

        public string ReadPassword()
        {
            base.Read();
            return base.token.TokenString;
        }

        private void ReadSequenceOptions(NumberSequence sequence, bool withType, bool isAlter)
        {
            bool flag;
            OrderedIntHashSet set = new OrderedIntHashSet();
        Label_0006:
            flag = false;
            if (set.Contains(base.token.TokenType))
            {
                throw base.UnexpectedToken();
            }
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x1a1)
            {
                if (tokenType <= 70)
                {
                    if (tokenType != 9)
                    {
                        if (tokenType != 70)
                        {
                            goto Label_027A;
                        }
                        set.Add(base.token.TokenType);
                        base.Read();
                        sequence.SetCycle(true);
                    }
                    else
                    {
                        if (!withType)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        SqlType type = base.ReadTypeDefinition(true, false);
                        sequence.SetDefaults(sequence.GetName(), type);
                    }
                }
                else if (tokenType != 0xb2)
                {
                    if (tokenType != 0x109)
                    {
                        if (tokenType != 0x1a1)
                        {
                            goto Label_027A;
                        }
                        set.Add(base.token.TokenType);
                        base.Read();
                        base.ReadThis(0x17);
                        long num2 = base.ReadBigint();
                        sequence.SetIncrement(num2);
                    }
                    else
                    {
                        set.Add(base.token.TokenType);
                        base.Read();
                        base.ReadThis(0x13d);
                        long num3 = base.ReadBigint();
                        sequence.SetStartValueNoCheck(num3);
                    }
                }
                else
                {
                    base.Read();
                    if (base.token.TokenType == 0x1b5)
                    {
                        sequence.SetDefaultMaxValue();
                    }
                    else if (base.token.TokenType == 0x1b9)
                    {
                        sequence.SetDefaultMinValue();
                    }
                    else
                    {
                        if (base.token.TokenType != 70)
                        {
                            throw base.UnexpectedToken();
                        }
                        sequence.SetCycle(false);
                    }
                    set.Add(base.token.TokenType);
                    base.Read();
                }
            }
            else if (tokenType <= 0x1b9)
            {
                if (tokenType != 0x1b5)
                {
                    if (tokenType != 0x1b9)
                    {
                        goto Label_027A;
                    }
                    set.Add(base.token.TokenType);
                    base.Read();
                    long num4 = base.ReadBigint();
                    sequence.SetMinValueNoCheck(num4);
                }
                else
                {
                    set.Add(base.token.TokenType);
                    base.Read();
                    long num5 = base.ReadBigint();
                    sequence.SetMaxValueNoCheck(num5);
                }
            }
            else if (tokenType != 0x1e3)
            {
                if (tokenType != 0x22c)
                {
                    if (tokenType != 0x324)
                    {
                        goto Label_027A;
                    }
                    base.Read();
                }
                else
                {
                    base.Read();
                    base.ReadBigint();
                }
            }
            else if (!isAlter)
            {
                flag = true;
            }
            else
            {
                set.Add(base.token.TokenType);
                base.Read();
                if (base.ReadIfThis(0x13d))
                {
                    long num6 = base.ReadBigint();
                    sequence.SetCurrentValueNoCheck(num6);
                }
                else
                {
                    sequence.SetStartValueDefault();
                }
            }
        Label_0272:
            if (!flag)
            {
                goto Label_0006;
            }
            sequence.CheckValues();
            return;
        Label_027A:
            flag = true;
            goto Label_0272;
        }

        public StatementSchema ReadTableAsSubqueryDefinition(Table table)
        {
            QNameManager.QName readName = null;
            bool flag = true;
            QNameManager.QName[] columnNames = null;
            StatementQuery query = null;
            if (base.token.TokenType == 0x2b7)
            {
                columnNames = base.ReadColumnNames(table.GetName());
            }
            base.ReadThis(9);
            base.ReadThis(0x2b7);
            QueryExpression queryExpression = base.XreadQueryExpression();
            queryExpression.SetReturningResult();
            queryExpression.Resolve(base.session);
            base.ReadThis(0x2aa);
            base.ReadThis(0x13d);
            if (base.token.TokenType == 0xb2)
            {
                base.Read();
                flag = false;
            }
            base.ReadThis(0x17a);
            if (base.token.TokenType == 0xc0)
            {
                if (!table.IsTemp())
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                base.ReadThis(0x2b);
                if ((base.token.TokenType != 0x4e) && (base.token.TokenType == 0x1db))
                {
                    table.PersistenceScope = 0x17;
                }
                base.Read();
                base.ReadThis(0xf3);
            }
            if (columnNames == null)
            {
                columnNames = queryExpression.GetResultColumnNames();
            }
            else if (columnNames.Length != queryExpression.GetColumnCount())
            {
                throw Error.GetError(0x15d9);
            }
            TableUtil.SetColumnsInSchemaTable(table, columnNames, queryExpression.GetColumnTypes(), null);
            table.CreatePrimaryKey();
            if (flag)
            {
                query = new StatementQuery(base.session, queryExpression);
                query.SetDatabseObjects(base.session, base.compileContext);
                query.CheckAccessRights(base.session);
                if (query.GetTableNamesForRead().Length != 0)
                {
                    readName = query.GetTableNamesForRead()[0];
                }
            }
            object[] args = new object[3];
            args[0] = table;
            args[2] = query;
            return new StatementSchema(base.GetLastPart(), 0x4d, args, readName, null);
        }
    }
}


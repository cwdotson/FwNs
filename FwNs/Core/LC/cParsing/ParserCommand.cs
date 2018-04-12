namespace FwNs.Core.LC.cParsing
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class ParserCommand : ParserDDL
    {
        public ParserCommand(Session session, Scanner t) : base(session, t)
        {
        }

        private Statement CompileBackup()
        {
            base.Read();
            base.ReadThis(0x234);
            base.ReadThis(0x11b);
            string str = base.ReadQuotedString();
            bool? nullable = null;
            bool? nullable2 = null;
            bool? nullable3 = null;
            while (true)
            {
                int tokenType = base.token.TokenType;
                if (tokenType <= 0x22b)
                {
                    if (tokenType != 0xb5)
                    {
                        if (tokenType != 0x22b)
                        {
                            break;
                        }
                        if (nullable.HasValue)
                        {
                            throw base.UnexpectedToken();
                        }
                        nullable = true;
                        base.Read();
                    }
                    else
                    {
                        base.Read();
                        if (base.token.TokenType == 0x232)
                        {
                            if (nullable3.HasValue)
                            {
                                throw base.UnexpectedToken();
                            }
                            nullable3 = false;
                            base.Read();
                        }
                        else
                        {
                            if (base.token.TokenType != 0x22b)
                            {
                                throw base.UnexpectedToken();
                            }
                            nullable = false;
                            base.Read();
                        }
                    }
                }
                else if (tokenType != 0x232)
                {
                    if (tokenType != 0x250)
                    {
                        break;
                    }
                    if (nullable2.HasValue)
                    {
                        throw base.UnexpectedToken();
                    }
                    nullable2 = true;
                    base.Read();
                }
                else
                {
                    if (nullable3.HasValue)
                    {
                        throw base.UnexpectedToken();
                    }
                    nullable3 = true;
                    base.Read();
                }
            }
            if (nullable2.HasValue)
            {
                throw ParserBase.UnsupportedFeature("SCRIPT");
            }
            nullable2 = false;
            if (!nullable.HasValue)
            {
                throw base.UnexpectedTokenRequire("BLOCKING");
            }
            if (!nullable3.HasValue)
            {
                nullable3 = true;
            }
            object[] args = new object[] { str, nullable, nullable2, nullable3 };
            QNameManager.QName[] catalogAndBaseTableNames = base.database.schemaManager.GetCatalogAndBaseTableNames();
            return new StatementCommand(0x3e9, args) { WriteTableNames = catalogAndBaseTableNames };
        }

        private Statement CompileCheckpoint()
        {
            bool defrag = false;
            base.Read();
            if (base.token.TokenType == 0x235)
            {
                defrag = true;
                base.Read();
            }
            if (base.token.TokenType == 0x2bb)
            {
                base.Read();
            }
            if (base.token.TokenType != 0x2ec)
            {
                throw base.UnexpectedToken();
            }
            base.GetLastPart();
            return GetCheckpointStatement(base.database, defrag);
        }

        private Statement CompileCommit()
        {
            bool flag = false;
            base.Read();
            base.ReadIfThis(0x220);
            if (base.token.TokenType == 5)
            {
                base.Read();
                if (base.token.TokenType == 0xb2)
                {
                    base.Read();
                }
                else
                {
                    flag = true;
                }
                base.ReadThis(350);
            }
            base.GetLastPart();
            object[] args = new object[] { flag };
            return new StatementSession(11, args);
        }

        private Statement CompileConnect()
        {
            base.Read();
            base.ReadThis(0x12f);
            base.CheckIsSimpleName();
            string tokenString = base.token.TokenString;
            base.Read();
            base.ReadThis(0x256);
            base.ReadThis(0x17);
            string o = base.ReadPassword();
            Expression[] args = new Expression[] { new ExpressionValue(tokenString, SqlType.SqlVarchar), new ExpressionValue(o, SqlType.SqlVarchar) };
            return new StatementSession(0x4c, args);
        }

        public override Statement CompileDeallocatePrepare()
        {
            base.ReadThis(0xd3);
            base.CheckIsSchemaObjectName();
            if (base.token.NamePrefix != null)
            {
                throw base.UnexpectedToken();
            }
            string tokenString = base.token.TokenString;
            base.Read();
            return new StatementSession(0x10, new object[] { tokenString });
        }

        private Statement CompileDeclare()
        {
            Statement statement = base.CompileDeclareLocalTableOrNull();
            if (statement != null)
            {
                return statement;
            }
            statement = base.CompileDeclareTypedLocalTableOrNull();
            if (statement != null)
            {
                return statement;
            }
            ColumnSchema[] schemaArray = base.ReadLocalVariableDeclarationOrNull();
            if (schemaArray != null)
            {
                object[] args = new object[] { schemaArray };
                return new StatementSession(0x420, args);
            }
            return base.CompileDeclareCursor(false, base.session.sessionContext.SessionVariablesRange);
        }

        private Statement CompileDisconnect()
        {
            base.Read();
            return new StatementSession(0x16, null);
        }

        public override Statement CompileExecute(RangeVariable[] rangeVars)
        {
            base.Read();
            if (base.token.TokenType == 0x19e)
            {
                base.Read();
                Expression expression = base.XreadValueExpression();
                List<Expression> sourceSet = expression.ResolveColumnReferences(rangeVars, rangeVars.Length, null, false);
                ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVars, rangeVars.Length, sourceSet, null));
                expression.ResolveTypes(base.session, null);
                if (!expression.GetDataType().IsCharacterType())
                {
                    throw Error.GetError(0x15cd);
                }
                return new StatementSession(0x2b, new object[] { expression });
            }
            base.CheckIsSchemaObjectName();
            if (base.token.NamePrefix != null)
            {
                throw base.UnexpectedToken();
            }
            string tokenString = base.token.TokenString;
            base.Read();
            OrderedHashSet<Expression> set = new OrderedHashSet<Expression>();
            if (base.token.TokenType == 0x130)
            {
                base.Read();
                while (true)
                {
                    Expression key = base.XreadValueExpression();
                    List<Expression> sourceSet = key.ResolveColumnReferences(rangeVars, rangeVars.Length, null, false);
                    ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVars, rangeVars.Length, sourceSet, null));
                    key.ResolveTypes(base.session, null);
                    set.Add(key);
                    if (base.token.TokenType != 0x2ac)
                    {
                        break;
                    }
                    base.Read();
                }
            }
            return new StatementSession(0x2c, new object[] { tokenString, set });
        }

        private Statement CompileExplainPlan()
        {
            base.Read();
            base.ReadThis(0x24b);
            base.ReadThis(0x6f);
            Statement statement1 = this.CompilePart(ResultProperties.DefaultPropsValue);
            statement1.SetDescribe();
            return statement1;
        }

        private Statement CompileIf()
        {
            Routine routine = new Routine(0x11) {
                IsAnnonymous = true
            };
            Statement statement1 = base.CompileIf(routine, null, true);
            if (statement1 == null)
            {
                throw base.UnexpectedToken();
            }
            statement1.SetSql(null);
            routine.Resolve(base.session);
            return statement1;
        }

        private Statement CompilePart(int props)
        {
            Statement statement;
            base.compileContext.Reset();
            base.SetParsePosition(base.GetPosition());
            if (base.token.TokenType == 0x2ed)
            {
                base.Read();
            }
            int tokenType = base.token.TokenType;
            if (tokenType <= 0xe7)
            {
                if (tokenType <= 0x4e)
                {
                    if (tokenType <= 0x2b)
                    {
                        if (tokenType <= 0x10)
                        {
                            if (tokenType == 4)
                            {
                                statement = base.CompileAlter();
                            }
                            else
                            {
                                if (tokenType != 0x10)
                                {
                                    goto Label_03FE;
                                }
                                statement = base.CompileAnnonymousBlock(base.session.sessionContext.SessionVariablesRange);
                            }
                        }
                        else if (tokenType == 0x18)
                        {
                            statement = base.CompileCallStatement(base.session.sessionContext.SessionVariablesRange, false);
                        }
                        else
                        {
                            if (tokenType != 0x2b)
                            {
                                goto Label_03FE;
                            }
                            statement = this.CompileCommit();
                        }
                        goto Label_0405;
                    }
                    if (tokenType <= 0x36)
                    {
                        if (tokenType == 0x2e)
                        {
                            statement = this.CompileConnect();
                        }
                        else
                        {
                            if (tokenType != 0x36)
                            {
                                goto Label_03FE;
                            }
                            statement = this.CompileCreate();
                        }
                        goto Label_0405;
                    }
                    switch (tokenType)
                    {
                        case 0x4c:
                            statement = this.CompileDeclare();
                            goto Label_0405;

                        case 0x4e:
                            statement = base.CompileDeleteStatement(RangeVariable.EmptyArray);
                            goto Label_0405;

                        case 0x49:
                            base.Read();
                            statement = this.CompileDeallocatePrepare();
                            goto Label_0405;
                    }
                    goto Label_03FE;
                }
                if (tokenType <= 120)
                {
                    if (tokenType <= 0x57)
                    {
                        if (tokenType == 0x53)
                        {
                            statement = this.CompileDisconnect();
                        }
                        else
                        {
                            if (tokenType != 0x57)
                            {
                                goto Label_03FE;
                            }
                            base.Read();
                            if (base.token.TokenType == 0xd3)
                            {
                                statement = this.CompileDeallocatePrepare();
                            }
                            else
                            {
                                statement = base.CompileDrop();
                            }
                        }
                        goto Label_0405;
                    }
                    switch (tokenType)
                    {
                        case 0x63:
                            statement = this.CompileExecute(base.session.sessionContext.SessionVariablesRange);
                            goto Label_0405;

                        case 120:
                            goto Label_03F5;
                    }
                    goto Label_03FE;
                }
                if (tokenType <= 0xa4)
                {
                    if (tokenType == 0x85)
                    {
                        statement = base.CompileInsertStatement(RangeVariable.EmptyArray);
                    }
                    else
                    {
                        if (tokenType != 0xa4)
                        {
                            goto Label_03FE;
                        }
                        statement = base.CompileMergeStatement(RangeVariable.EmptyArray);
                    }
                }
                else if (tokenType == 0xd3)
                {
                    statement = this.CompilePrepare(base.session.sessionContext.SessionVariablesRange);
                }
                else
                {
                    if (tokenType != 0xe7)
                    {
                        goto Label_03FE;
                    }
                    statement = this.CompileReleaseSavepoint();
                }
            }
            else if (tokenType <= 0x12d)
            {
                if (tokenType <= 0xfc)
                {
                    if (tokenType <= 0xf4)
                    {
                        switch (tokenType)
                        {
                            case 0xed:
                                goto Label_03F5;

                            case 0xee:
                                goto Label_03FE;

                            case 0xef:
                                statement = this.CompileRollback();
                                goto Label_0405;
                        }
                        if (tokenType != 0xf4)
                        {
                            goto Label_03FE;
                        }
                        statement = this.CompileSavepoint();
                    }
                    else
                    {
                        if (tokenType == 0xf9)
                        {
                            goto Label_0205;
                        }
                        if (tokenType != 0xfc)
                        {
                            goto Label_03FE;
                        }
                        statement = this.CompileSet();
                    }
                }
                else
                {
                    if (tokenType <= 0x114)
                    {
                        switch (tokenType)
                        {
                            case 0x109:
                                statement = this.CompileStartTransaction();
                                goto Label_0405;

                            case 0x114:
                                goto Label_0205;
                        }
                        goto Label_03FE;
                    }
                    if (tokenType == 0x125)
                    {
                        statement = base.CompileDeleteStatement(RangeVariable.EmptyArray);
                    }
                    else
                    {
                        if (tokenType != 0x12d)
                        {
                            goto Label_03FE;
                        }
                        statement = base.CompileUpdateStatement(RangeVariable.EmptyArray);
                    }
                }
            }
            else if (tokenType <= 0x229)
            {
                if (tokenType <= 0x13d)
                {
                    switch (tokenType)
                    {
                        case 0x132:
                            statement = base.CompileShortCursorSpecification(props);
                            goto Label_0405;

                        case 0x13d:
                            goto Label_0205;
                    }
                    goto Label_03FE;
                }
                if (tokenType == 0x19c)
                {
                    statement = this.CompileIf();
                }
                else
                {
                    if (tokenType != 0x229)
                    {
                        goto Label_03FE;
                    }
                    statement = this.CompileBackup();
                }
            }
            else if (tokenType <= 0x238)
            {
                if (tokenType == 0x22f)
                {
                    statement = this.CompileCheckpoint();
                }
                else
                {
                    if (tokenType != 0x238)
                    {
                        goto Label_03FE;
                    }
                    statement = this.CompileExplainPlan();
                }
            }
            else
            {
                switch (tokenType)
                {
                    case 0x250:
                        statement = this.CompileScript();
                        goto Label_0405;

                    case 0x251:
                        goto Label_03FE;

                    case 0x252:
                        statement = this.CompileShutdown();
                        goto Label_0405;

                    case 0x2b7:
                        goto Label_0205;
                }
                if (tokenType != 0x335)
                {
                    goto Label_03FE;
                }
                statement = base.CompileComment();
            }
            goto Label_0405;
        Label_0205:
            statement = base.CompileCursorSpecification(props, false, base.session.sessionContext.SessionVariablesRange);
            goto Label_0405;
        Label_03F5:
            statement = base.CompileGrantOrRevoke();
            goto Label_0405;
        Label_03FE:
            throw base.UnexpectedToken();
        Label_0405:
            if (statement.type != 0x4c)
            {
                statement.SetSql(base.GetLastPart());
            }
            if (base.token.TokenType == 0x2bb)
            {
                base.Read();
                return statement;
            }
            int num1 = base.token.TokenType;
            return statement;
        }

        public override Statement CompilePrepare(RangeVariable[] rangeVars)
        {
            base.Read();
            base.CheckIsSchemaObjectName();
            if (base.token.NamePrefix != null)
            {
                throw base.UnexpectedToken();
            }
            string tokenString = base.token.TokenString;
            base.Read();
            base.ReadThis(0x72);
            Expression expression = base.XreadValueExpression();
            List<Expression> sourceSet = expression.ResolveColumnReferences(rangeVars, rangeVars.Length, null, false);
            ExpressionColumn.CheckColumnsResolved(Expression.ResolveColumnSet(rangeVars, rangeVars.Length, sourceSet, null));
            expression.ResolveTypes(base.session, null);
            if (!expression.GetDataType().IsCharacterType())
            {
                throw Error.GetError(0x15cd);
            }
            return new StatementSession(0x38, new object[] { tokenString, expression });
        }

        private Statement CompileReleaseSavepoint()
        {
            base.Read();
            base.ReadThis(0xf4);
            string tokenString = base.token.TokenString;
            base.Read();
            base.GetLastPart();
            object[] args = new object[] { tokenString };
            return new StatementSession(0x39, args);
        }

        private Statement CompileRollback()
        {
            bool flag = false;
            base.Read();
            if (base.token.TokenType == 0x11b)
            {
                base.Read();
                base.ReadThis(0xf4);
                base.CheckIsSimpleName();
                string tokenString = base.token.TokenString;
                base.Read();
                base.GetLastPart();
                object[] objArray1 = new object[] { tokenString };
                return new StatementSession(0x418, objArray1);
            }
            if (base.token.TokenType == 0x220)
            {
                base.Read();
            }
            if (base.token.TokenType == 5)
            {
                base.Read();
                if (base.token.TokenType == 0xb2)
                {
                    base.Read();
                }
                else
                {
                    flag = true;
                }
                base.ReadThis(350);
            }
            base.GetLastPart();
            object[] args = new object[] { flag };
            return new StatementSession(0x3e, args);
        }

        private Statement CompileSavepoint()
        {
            base.Read();
            base.CheckIsSimpleName();
            string tokenString = base.token.TokenString;
            base.Read();
            base.GetLastPart();
            object[] args = new object[] { tokenString };
            return new StatementSession(0x3f, args);
        }

        private Statement CompileScript()
        {
            string str = null;
            base.Read();
            if (base.token.TokenType == 0x2e9)
            {
                str = base.ReadQuotedString();
            }
            object[] args = new object[] { str };
            QNameManager.QName[] catalogAndBaseTableNames = base.database.schemaManager.GetCatalogAndBaseTableNames();
            return new StatementCommand(0x3ec, args) { ReadTableNames = catalogAndBaseTableNames };
        }

        private Statement CompileSessionSettings()
        {
            int tokenType = base.token.TokenType;
            switch (tokenType)
            {
                case 14:
                {
                    base.Read();
                    Expression expression = base.XreadValueSpecificationOrNull();
                    if (expression == null)
                    {
                        throw Error.GetError(0x15d0);
                    }
                    expression.ResolveTypes(base.session, null);
                    if (expression.IsUnresolvedParam())
                    {
                        expression.DataType = SqlType.SqlVarchar;
                    }
                    if ((expression.DataType == null) || !expression.DataType.IsCharacterType())
                    {
                        throw Error.GetError(0x15bb);
                    }
                    Expression[] args = new Expression[2];
                    args[0] = expression;
                    return new StatementSession(0x4c, args);
                }
                case 0xea:
                {
                    base.Read();
                    base.ReadThis(0x246);
                    base.ReadThis(0xf3);
                    int num2 = base.ReadIntegerObject();
                    object[] args = new object[] { num2 };
                    return new StatementSession(0x417, args);
                }
            }
            if (tokenType != 0x162)
            {
                throw base.UnexpectedToken();
            }
            base.Read();
            base.ReadThis(9);
            base.ReadThis(0x20c);
            return new StatementSession(0x6d, this.ProcessTransactionCharacteristics());
        }

        private Statement CompileSet()
        {
            int position = base.GetPosition();
            base.session.SetScripting(false);
            base.Read();
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x20c)
            {
                if (tokenType > 0x15c)
                {
                    if (tokenType > 0x1f0)
                    {
                        if (tokenType == 0x1fb)
                        {
                            base.Read();
                            return this.CompileSessionSettings();
                        }
                        if (tokenType == 0x20c)
                        {
                            base.Read();
                            object[] args = this.ProcessTransactionCharacteristics();
                            if ((args[0] == null) && (args[1] == null))
                            {
                                throw base.UnexpectedToken();
                            }
                            return new StatementSession(0x4b, args);
                        }
                    }
                    else
                    {
                        switch (tokenType)
                        {
                            case 0x1e9:
                                base.Read();
                                return this.CompileSetRole();

                            case 0x1f0:
                            {
                                base.Read();
                                Expression expression2 = base.XreadValueSpecificationOrNull();
                                if (expression2 == null)
                                {
                                    QNameManager.QName name = base.ReadSchemaName();
                                    object[] objArray13 = new object[] { name };
                                    return new StatementSession(0x4a, objArray13);
                                }
                                if (!expression2.GetDataType().IsCharacterType())
                                {
                                    throw Error.GetError(0x898);
                                }
                                if ((expression2.GetExprType() != 1) && ((expression2.GetExprType() != 0x1c) || !((FunctionSQL) expression2).IsValueFunction()))
                                {
                                    throw Error.GetError(0x898);
                                }
                                Expression[] args = new Expression[] { expression2 };
                                return new StatementSession(0x4a, args);
                            }
                        }
                    }
                }
                else if (tokenType > 0x114)
                {
                    if (tokenType == 0x117)
                    {
                        base.Read();
                        return this.CompileSetTimeZone();
                    }
                    if (tokenType == 0x15c)
                    {
                        base.Read();
                        Expression expression = base.XreadValueSpecificationOrNull();
                        if (!expression.GetDataType().IsCharacterType())
                        {
                            throw Error.GetError(0x898);
                        }
                        if ((expression.GetExprType() != 1) && ((expression.GetExprType() != 0x1c) || !((FunctionSQL) expression).IsValueFunction()))
                        {
                            throw Error.GetError(0x898);
                        }
                        Expression[] args = new Expression[] { expression };
                        return new StatementSession(0x42, args);
                    }
                }
                else
                {
                    switch (tokenType)
                    {
                        case 0x4d:
                        {
                            base.Read();
                            base.ReadThis(0x114);
                            base.ReadThis(0x215);
                            int num3 = 4;
                            int num4 = base.token.TokenType;
                            if (num4 != 0x22d)
                            {
                                if (num4 != 0x246)
                                {
                                    throw base.UnexpectedToken();
                                }
                            }
                            else
                            {
                                num3 = 5;
                            }
                            base.Read();
                            object[] args = new object[] { num3 };
                            return new StatementCommand(0x3f4, args, null, null);
                        }
                        case 0x114:
                        {
                            base.Read();
                            Table table = base.ReadTableName();
                            object[] objArray12 = new object[2];
                            objArray12[0] = table.GetName();
                            object[] args = objArray12;
                            switch (base.token.TokenType)
                            {
                                case 0x1df:
                                {
                                    base.Read();
                                    bool flag = false;
                                    if (base.token.TokenType == 0x221)
                                    {
                                        base.Read();
                                    }
                                    else
                                    {
                                        base.ReadThis(0xc1);
                                        flag = true;
                                    }
                                    args[1] = flag;
                                    return new StatementCommand(0x42c, args, null, table.GetName());
                                }
                                case 0x215:
                                    int num6;
                                    base.Read();
                                    if (base.token.TokenType == 0x22d)
                                    {
                                        num6 = 5;
                                    }
                                    else
                                    {
                                        if (base.token.TokenType != 0x246)
                                        {
                                            throw base.UnexpectedToken();
                                        }
                                        num6 = 4;
                                    }
                                    base.Read();
                                    args[1] = num6;
                                    return new StatementCommand(0x42f, args, null, table.GetName());

                                case 0x240:
                                {
                                    base.Read();
                                    base.CheckIsValue();
                                    string tokenString = base.token.TokenString;
                                    base.Read();
                                    args[1] = tokenString;
                                    return new StatementCommand(0x42b, args);
                                }
                                case 0x24d:
                                {
                                    base.Read();
                                    bool flag2 = this.ProcessTrueOrFalseObject();
                                    args[1] = flag2;
                                    return new StatementCommand(0x42c, args, null, table.GetName());
                                }
                            }
                            throw base.UnexpectedToken();
                        }
                    }
                }
            }
            else if (tokenType <= 0x23d)
            {
                if (tokenType > 0x234)
                {
                    if (tokenType == 570)
                    {
                        return this.CompileSetFilesProperty();
                    }
                    if (tokenType == 0x23d)
                    {
                        base.Read();
                        bool flag4 = this.ProcessTrueOrFalseObject();
                        object[] args = new object[] { flag4 };
                        return new StatementSession(0x40a, args);
                    }
                }
                else
                {
                    switch (tokenType)
                    {
                        case 0x228:
                        {
                            base.Read();
                            bool flag3 = this.ProcessTrueOrFalseObject();
                            object[] args = new object[] { flag3 };
                            return new StatementSession(0x415, args);
                        }
                        case 0x234:
                            return this.CompileSetDatabaseProperty();
                    }
                }
            }
            else
            {
                switch (tokenType)
                {
                    case 0x241:
                    {
                        QNameManager.QName schemaQName;
                        base.Read();
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
                        object[] args = new object[2];
                        args[1] = schemaQName;
                        return new StatementCommand(0x412, args, null, null);
                    }
                    case 0x245:
                    {
                        base.Read();
                        int num7 = base.ReadIntegerObject();
                        object[] args = new object[] { num7 };
                        return new StatementSession(0x416, args);
                    }
                    case 0x24c:
                        return this.CompileSetProperty();

                    case 0x24d:
                    {
                        base.Read();
                        bool flag5 = this.ProcessTrueOrFalseObject();
                        object[] args = new object[] { flag5 };
                        return new StatementSession(0x6d, args);
                    }
                    case 0x255:
                    {
                        int defaultWriteDelay;
                        base.Read();
                        if (base.token.TokenType == 0x124)
                        {
                            defaultWriteDelay = base.database.GetProperties().GetDefaultWriteDelay();
                            base.Read();
                        }
                        else if (base.token.TokenType == 0x69)
                        {
                            defaultWriteDelay = 0;
                            base.Read();
                        }
                        else
                        {
                            defaultWriteDelay = base.ReadInteger();
                            if (defaultWriteDelay < 0)
                            {
                                defaultWriteDelay = 0;
                            }
                            if (base.token.TokenType == 0x247)
                            {
                                base.Read();
                            }
                            else
                            {
                                defaultWriteDelay *= 0x3e8;
                            }
                        }
                        object[] args = new object[] { defaultWriteDelay };
                        return new StatementCommand(0x402, args);
                    }
                    case 0x256:
                    {
                        base.Read();
                        base.ReadThis(0x17);
                        string str2 = base.ReadPassword();
                        object[] args = new object[2];
                        args[1] = str2;
                        return new StatementCommand(0x413, args);
                    }
                }
            }
            this.Rewind(position);
            return base.CompileSetStatement(base.session.sessionContext.SessionVariablesRange, false);
        }

        public StatementCommand CompileSetDatabaseProperty()
        {
            base.Read();
            base.CheckDatabaseUpdateAuthorisation();
            int tokenType = base.token.TokenType;
            if (tokenType > 0x166)
            {
                if (tokenType > 0x237)
                {
                    switch (tokenType)
                    {
                        case 0x23b:
                        {
                            base.Read();
                            int num7 = base.ReadIntegerObject();
                            object[] args = new object[] { num7 };
                            return new StatementCommand(0x403, args, null, null);
                        }
                        case 590:
                        {
                            base.Read();
                            base.ReadThis(0x23f);
                            bool flag = this.ProcessTrueOrFalse();
                            object[] args = new object[] { flag };
                            return new StatementCommand(0x40b, args, null, null);
                        }
                    }
                }
                else
                {
                    if (tokenType == 0x20c)
                    {
                        base.Read();
                        base.ReadThis(0x233);
                        int num4 = 0;
                        switch (base.token.TokenType)
                        {
                            case 580:
                                base.Read();
                                num4 = 0;
                                break;

                            case 0x283:
                                base.Read();
                                num4 = 2;
                                break;

                            case 0x284:
                                base.Read();
                                num4 = 1;
                                break;
                        }
                        object[] args = new object[] { num4 };
                        QNameManager.QName[] catalogAndBaseTableNames = base.database.schemaManager.GetCatalogAndBaseTableNames();
                        return new StatementCommand(0x7fe, args, null, null) { WriteTableNames = catalogAndBaseTableNames };
                    }
                    if (tokenType == 0x237)
                    {
                        base.Read();
                        base.ReadThis(0x27e);
                        base.ReadThis(0x1b0);
                        int num6 = base.ReadIntegerObject();
                        object[] args = new object[] { num6 };
                        return new StatementCommand(0x3f9, args, null, null);
                    }
                }
            }
            else if (tokenType > 0x103)
            {
                if (tokenType == 0x129)
                {
                    base.Read();
                    base.ReadThis(0x1bc);
                    base.IsUndelimitedSimpleName();
                    string tokenString = base.token.TokenString;
                    base.Read();
                    if (tokenString.Length != 0x12)
                    {
                        throw Error.GetError(0x15b3);
                    }
                    if (!Charset.IsInSet(tokenString, Charset.UnquotedIdentifier) || !Charset.StartsWith(tokenString, Charset.UppercaseLetters))
                    {
                        throw Error.GetError(0x157d);
                    }
                    object[] args = new object[] { tokenString };
                    return new StatementCommand(0x800, args, null, null);
                }
                if (tokenType == 0x166)
                {
                    base.Read();
                    base.CheckIsSimpleName();
                    string tokenString = base.token.TokenString;
                    base.Read();
                    object[] args = new object[] { tokenString };
                    return new StatementCommand(0x409, args, null, null);
                }
            }
            else
            {
                if (tokenType == 0x4d)
                {
                    return this.CompileSetDefault();
                }
                if (tokenType == 0x103)
                {
                    base.Read();
                    int type = 0;
                    bool? nullable = null;
                    int num3 = base.token.TokenType;
                    switch (num3)
                    {
                        case 220:
                            base.Read();
                            type = 0x7fc;
                            nullable = new bool?(this.ProcessTrueOrFalseObject());
                            break;

                        case 0x1bd:
                            base.Read();
                            type = 0x40c;
                            nullable = new bool?(this.ProcessTrueOrFalseObject());
                            break;

                        default:
                            if (num3 != 510)
                            {
                                base.UnexpectedToken();
                            }
                            else
                            {
                                base.Read();
                                type = 0x40d;
                                nullable = new bool?(this.ProcessTrueOrFalseObject());
                            }
                            break;
                    }
                    object[] args = new object[] { nullable };
                    return new StatementCommand(type, args, null, null);
                }
            }
            throw base.UnexpectedToken();
        }

        private StatementCommand CompileSetDefault()
        {
            base.Read();
            switch (base.token.TokenType)
            {
                case 0xea:
                {
                    base.Read();
                    base.ReadThis(0x246);
                    base.ReadThis(0xf3);
                    int num2 = base.ReadIntegerObject();
                    object[] args = new object[] { num2 };
                    return new StatementCommand(0x407, args);
                }
                case 0x114:
                {
                    base.Read();
                    base.ReadThis(0x215);
                    int num3 = 4;
                    int tokenType = base.token.TokenType;
                    if (tokenType != 0x22d)
                    {
                        if (tokenType != 0x246)
                        {
                            throw base.UnexpectedToken();
                        }
                    }
                    else
                    {
                        num3 = 5;
                    }
                    base.Read();
                    object[] args = new object[] { num3 };
                    return new StatementCommand(0x3f4, args);
                }
                case 0x1a8:
                {
                    int num6;
                    base.Read();
                    base.ReadThis(0x1b0);
                    int tokenType = base.token.TokenType;
                    if (tokenType != 0x1df)
                    {
                        if (tokenType != 0x1f9)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        num6 = 0x100000;
                    }
                    else
                    {
                        base.Read();
                        base.ReadThis(0x16d);
                        num6 = 0x1000;
                    }
                    base.Read();
                    object[] args = new object[] { num6 };
                    return new StatementCommand(0x7ff, args);
                }
                case 0x241:
                {
                    base.Read();
                    base.ReadThis(0x1f0);
                    QNameManager.QName schemaQName = base.database.schemaManager.GetSchemaQName(base.token.TokenString);
                    base.Read();
                    object[] args = new object[] { schemaQName };
                    return new StatementCommand(0x3f3, args);
                }
            }
            throw base.UnexpectedToken();
        }

        public StatementCommand CompileSetFilesProperty()
        {
            object[] objArray1;
            int? nullable5;
            int num5;
            base.Read();
            int type = 0;
            bool? nullable = null;
            int? nullable2 = null;
            base.CheckDatabaseUpdateAuthorisation();
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x22c)
            {
                if (tokenType <= 0x221)
                {
                    if (tokenType == 0x1ef)
                    {
                        base.Read();
                        nullable2 = new int?(base.ReadIntegerObject());
                        type = 0x3ff;
                    }
                    else
                    {
                        int defaultWriteDelay;
                        if (tokenType != 0x221)
                        {
                            goto Label_0276;
                        }
                        base.Read();
                        base.ReadThis(0x236);
                        type = 0x402;
                        if (base.token.TokenType == 0x124)
                        {
                            defaultWriteDelay = base.database.GetProperties().GetDefaultWriteDelay();
                            base.Read();
                        }
                        else if (base.token.TokenType == 0x69)
                        {
                            defaultWriteDelay = 0;
                            base.Read();
                        }
                        else
                        {
                            defaultWriteDelay = base.ReadInteger();
                            if (defaultWriteDelay < 0)
                            {
                                defaultWriteDelay = 0;
                            }
                            if (base.token.TokenType == 0x247)
                            {
                                base.Read();
                            }
                            else
                            {
                                defaultWriteDelay *= 0x3e8;
                            }
                        }
                        nullable2 = new int?(defaultWriteDelay);
                    }
                }
                else if (tokenType == 0x229)
                {
                    base.Read();
                    type = 0x3f5;
                    base.ReadThis(0x1a1);
                    nullable = new bool?(this.ProcessTrueOrFalseObject());
                }
                else
                {
                    if (tokenType != 0x22c)
                    {
                        goto Label_0276;
                    }
                    base.Read();
                    if (base.ReadIfThis(510))
                    {
                        nullable2 = new int?(base.ReadIntegerObject());
                        type = 0x3f7;
                    }
                    else if (base.ReadIfThis(0xf3))
                    {
                        nullable2 = new int?(base.ReadIntegerObject());
                        type = 0x3f6;
                    }
                }
                goto Label_027D;
            }
            if (tokenType <= 0x27e)
            {
                if (tokenType == 0x235)
                {
                    base.Read();
                    type = 0x3f8;
                    nullable2 = new int?(base.ReadIntegerObject());
                }
                else
                {
                    if (tokenType != 0x27e)
                    {
                        goto Label_0276;
                    }
                    base.Read();
                    if (base.token.TokenType == 510)
                    {
                        base.ReadThis(510);
                        type = 0x3fb;
                        nullable2 = new int?(base.ReadIntegerObject());
                    }
                    else
                    {
                        type = 0x801;
                        nullable = new bool?(this.ProcessTrueOrFalseObject());
                    }
                }
                goto Label_027D;
            }
            switch (tokenType)
            {
                case 0x285:
                    base.Read();
                    type = 0x3fc;
                    nullable = new bool?(this.ProcessTrueOrFalseObject());
                    goto Label_027D;

                case 0x32e:
                    base.Read();
                    base.ReadThis(0x1ef);
                    nullable2 = new int?(base.ReadIntegerObject());
                    type = 0x3fa;
                    goto Label_027D;
            }
        Label_0276:
            throw base.UnexpectedToken();
        Label_027D:
            objArray1 = new object[1];
            int index = 0;
            bool? nullable4 = nullable;
            if (!nullable4.HasValue && nullable2.HasValue)
            {
                nullable5 = nullable2;
                num5 = 1;
            }
            objArray1[index] = (nullable5.GetValueOrDefault() == num5) ? ((object) nullable4.GetValueOrDefault()) : ((object) 0);
            return new StatementCommand(type, objArray1, base.database.GetCatalogName(), null);
        }

        private StatementCommand CompileSetProperty()
        {
            object tokenValue;
            base.Read();
            base.database.GetProperties();
            base.CheckIsSimpleName();
            base.CheckIsDelimitedIdentifier();
            string tokenString = base.token.TokenString;
            bool flag = LibCoreDatabaseProperties.IsBoolean(base.token.TokenString);
            bool flag2 = LibCoreDatabaseProperties.IsIntegral(base.token.TokenString);
            bool flag3 = LibCoreDatabaseProperties.IsString(base.token.TokenString);
            if ((!flag && !flag2) && !flag3)
            {
                throw Error.GetError(0x15b3);
            }
            int num = flag ? 0x10 : (flag2 ? 4 : 1);
            base.Read();
            if (base.token.TokenType == 0x124)
            {
                tokenValue = true;
                if (!flag)
                {
                    throw Error.GetError(0x15bd, base.token.TokenString);
                }
            }
            else if (base.token.TokenType == 0x69)
            {
                tokenValue = false;
                if (!flag)
                {
                    throw Error.GetError(0x15bd, base.token.TokenString);
                }
            }
            else
            {
                base.CheckIsValue();
                tokenValue = base.token.TokenValue;
                if (base.token.DataType.TypeCode != num)
                {
                    throw Error.GetError(0x15bd, base.token.TokenString);
                }
            }
            base.Read();
            object[] args = new object[] { tokenString, tokenValue };
            return new StatementCommand(0x404, args);
        }

        private Statement CompileSetRole()
        {
            Expression expression;
            if (base.token.TokenType == 0xb3)
            {
                base.Read();
                expression = new ExpressionValue(null, SqlType.SqlVarchar);
            }
            else
            {
                expression = base.XreadValueSpecificationOrNull();
                if (expression == null)
                {
                    throw Error.GetError(0x1004);
                }
                if (!expression.GetDataType().IsCharacterType())
                {
                    throw Error.GetError(0x898);
                }
                if ((expression.GetExprType() != 1) && ((expression.GetExprType() != 0x1c) || !((FunctionSQL) expression).IsValueFunction()))
                {
                    throw Error.GetError(0x898);
                }
            }
            base.GetLastPart();
            return new StatementSession(0x49, new Expression[] { expression });
        }

        private Statement CompileSetTimeZone()
        {
            Expression expression;
            base.ReadThis(0x222);
            if (base.token.TokenType == 0x9b)
            {
                base.Read();
                expression = new ExpressionValue(null, SqlType.SqlIntervalHourToMinute);
            }
            else
            {
                expression = base.XreadIntervalValueExpression();
                ExpressionColumn.CheckColumnsResolved(expression.ResolveColumnReferences(RangeVariable.EmptyArray, null));
                expression.ResolveTypes(base.session, null);
                if (expression.DataType == null)
                {
                    throw Error.GetError(0x15bd);
                }
                if (expression.DataType.TypeCode != 0x6f)
                {
                    throw Error.GetError(0x15bd);
                }
            }
            base.GetLastPart();
            return new StatementSession(0x47, new Expression[] { expression });
        }

        private Statement CompileShutdown()
        {
            base.session.CheckAdmin();
            int num = 0;
            base.Read();
            switch (base.token.TokenType)
            {
                case 0x231:
                    num = 1;
                    base.Read();
                    break;

                case 0x23e:
                    num = -1;
                    base.Read();
                    break;

                case 0x250:
                    num = 2;
                    base.Read();
                    break;
            }
            if (base.token.TokenType == 0x2bb)
            {
                base.Read();
            }
            if (base.token.TokenType != 0x2ec)
            {
                throw base.UnexpectedToken();
            }
            base.GetLastPart();
            object[] args = new object[] { num };
            return new StatementCommand(0x3eb, args, null, null);
        }

        private Statement CompileStartTransaction()
        {
            base.Read();
            base.ReadThis(0x20c);
            return new StatementSession(0x6f, this.ProcessTransactionCharacteristics());
        }

        public Statement CompileStatement(int props)
        {
            Statement statement = this.CompilePart(props);
            if (base.token.TokenType != 0x2ec)
            {
                throw base.UnexpectedToken();
            }
            if (statement.GetSchemaName() == null)
            {
                statement.SetSchemaQName(base.session.GetCurrentSchemaQName());
            }
            return statement;
        }

        public List<Statement> CompileStatements(string sql, Result cmd)
        {
            List<Statement> list = new List<Statement>();
            Statement item = null;
            this.Reset(sql);
            while (base.token.TokenType != 0x2ec)
            {
                item = this.CompilePart(cmd.GetExecuteProperties());
                if (!item.IsExplain && (item.GetParametersMetaData().GetColumnCount() > 0))
                {
                    throw Error.GetError(0x15c7);
                }
                list.Add(item);
            }
            int statementType = cmd.GetStatementType();
            if (statementType != 0)
            {
                if (item.GetGroup() == 0x7d3)
                {
                    if (statementType == 1)
                    {
                        throw Error.GetError(0x4e5);
                    }
                    return list;
                }
                if (statementType == 2)
                {
                    throw Error.GetError(0x4e6);
                }
            }
            return list;
        }

        public static Statement GetCheckpointStatement(Database database, bool defrag)
        {
            object[] args = new object[] { defrag };
            QNameManager.QName[] catalogAndBaseTableNames = database.schemaManager.GetCatalogAndBaseTableNames();
            return new StatementCommand(0x3ea, args) { WriteTableNames = catalogAndBaseTableNames };
        }

        private object[] ProcessTransactionCharacteristics()
        {
            int num2;
            int num = 0;
            bool flag = false;
            object[] objArray = new object[2];
        Label_000B:
            num2 = base.token.TokenType;
            switch (num2)
            {
                case 0x1a8:
                {
                    if (objArray[1] != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadThis(0x1b0);
                    int tokenType = base.token.TokenType;
                    switch (tokenType)
                    {
                        case 0x1df:
                            base.Read();
                            if (base.token.TokenType != 0x16d)
                            {
                                if (base.token.TokenType != 0x217)
                                {
                                    throw base.UnexpectedToken();
                                }
                                base.Read();
                                num = 0x100;
                            }
                            else
                            {
                                base.Read();
                                num = 0x1000;
                            }
                            goto Label_013E;

                        case 480:
                            goto Label_017D;

                        case 0x1e1:
                            base.Read();
                            base.ReadThis(0x1df);
                            num = 0x10000;
                            goto Label_013E;
                    }
                    if (tokenType != 0x1f9)
                    {
                        goto Label_017D;
                    }
                    base.Read();
                    num = 0x100000;
                    break;
                }
                case 0x1df:
                    if (objArray[0] != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    if (base.token.TokenType == 0xc1)
                    {
                        base.Read();
                        flag = true;
                    }
                    else
                    {
                        base.ReadThis(0x221);
                        flag = false;
                    }
                    objArray[0] = flag;
                    goto Label_000B;

                default:
                    if (num2 == 0x2ac)
                    {
                        if ((objArray[0] == null) && (objArray[1] == null))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        goto Label_000B;
                    }
                    if (!flag && (num == 1))
                    {
                        throw base.UnexpectedToken("WRITE");
                    }
                    return objArray;
            }
        Label_013E:
            objArray[1] = num;
            goto Label_000B;
        Label_017D:
            throw base.UnexpectedToken();
        }

        private bool ProcessTrueOrFalse()
        {
            if (base.token.NamePrefix != null)
            {
                throw base.UnexpectedToken();
            }
            if ((base.token.TokenType == 0x124) || (base.token.TokenType == 0xc0))
            {
                base.Read();
                return true;
            }
            if ((base.token.TokenType != 0x69) && (base.token.TokenType != 0x249))
            {
                throw base.UnexpectedToken();
            }
            base.Read();
            return false;
        }

        private bool ProcessTrueOrFalseObject()
        {
            if (base.token.NamePrefix != null)
            {
                throw base.UnexpectedToken();
            }
            if ((base.token.TokenType == 0x124) || (base.token.TokenType == 0xc0))
            {
                base.Read();
                return true;
            }
            if ((base.token.TokenType != 0x69) && (base.token.TokenType != 0x249))
            {
                throw base.UnexpectedToken();
            }
            base.Read();
            return false;
        }
    }
}


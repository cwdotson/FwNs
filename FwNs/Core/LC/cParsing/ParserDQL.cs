namespace FwNs.Core.LC.cParsing
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ParserDQL : ParserBase
    {
        public CompileContext compileContext;
        protected Database database;
        public CoreException LastError;
        protected Session session;

        public ParserDQL(Session session, Scanner t) : base(t)
        {
            this.session = session;
            this.database = session.GetDatabase();
            this.compileContext = new CompileContext(session, this);
        }

        public void CheckIsSchemaObjectName()
        {
            if (this.database.SqlEnforceNames)
            {
                base.CheckIsNonReservedIdentifier();
            }
            else
            {
                base.CheckIsNonCoreReservedIdentifier();
            }
        }

        public void CheckValidCatalogName(string name)
        {
            if ((name != null) && !name.Equals(this.database.GetCatalogName().Name))
            {
                throw Error.GetError(0x157d, name);
            }
        }

        public StatementQuery CompileCursorSpecification(int props, bool isRoutine, RangeVariable[] outerRanges)
        {
            StatementQuery query;
            QueryExpression expression = this.XreadQueryExpression();
            if (base.token.TokenType == 0x6f)
            {
                base.Read();
                if (base.token.TokenType == 0x1df)
                {
                    base.Read();
                    base.ReadThis(0xc1);
                }
                else
                {
                    base.ReadThis(0x12d);
                    props = ResultProperties.AddUpdatable(props, true);
                    if (base.token.TokenType == 0xbd)
                    {
                        base.ReadThis(0xbd);
                        OrderedHashSet<string> set = new OrderedHashSet<string>();
                        this.ReadColumnNameList(set, null, false);
                    }
                }
            }
            if (ResultProperties.IsUpdatable(props))
            {
                expression.IsUpdatable = true;
            }
            expression.SetReturningResult();
            if (outerRanges == null)
            {
                expression.Resolve(this.session);
            }
            else
            {
                expression.Resolve(this.session, outerRanges, null);
            }
            if (isRoutine)
            {
                query = new StatementCursor(this.session, expression);
            }
            else
            {
                query = new StatementQuery(this.session, expression);
            }
            query.SetDatabseObjects(this.session, this.compileContext);
            query.CheckAccessRights(this.session);
            return query;
        }

        public StatementQuery CompileDeclareCursor(bool isRoutine, RangeVariable[] outerRanges)
        {
            int sensitive = 0;
            int scrollable = 0;
            int holdable = 0;
            int returnable = 0;
            int position = base.GetPosition();
            base.ReadThis(0x4c);
            QNameManager.QName name = this.ReadNewSchemaObjectName(0x13, true);
            switch (base.token.TokenType)
            {
                case 10:
                    base.Read();
                    break;

                case 0x84:
                    base.Read();
                    sensitive = 1;
                    break;

                case 250:
                    base.Read();
                    sensitive = 2;
                    break;
            }
            if (base.token.TokenType == 0xb2)
            {
                base.ReadThis(0xf6);
            }
            else if (base.token.TokenType == 0xf6)
            {
                base.Read();
                scrollable = 1;
            }
            if (base.token.TokenType != 0x45)
            {
                this.Rewind(position);
                return null;
            }
            base.ReadThis(0x45);
            for (int i = 0; i < 2; i++)
            {
                if (base.token.TokenType == 0x13d)
                {
                    base.Read();
                    if ((i == 0) && (base.token.TokenType == 0x7d))
                    {
                        base.Read();
                        holdable = 1;
                    }
                    else
                    {
                        base.ReadThis(0xeb);
                        i++;
                        returnable = 1;
                    }
                }
                else if (base.token.TokenType == 0x13f)
                {
                    base.Read();
                    if ((i == 0) && (base.token.TokenType == 0x7d))
                    {
                        base.Read();
                    }
                    else
                    {
                        base.ReadThis(0xeb);
                        i++;
                    }
                }
            }
            base.ReadThis(0x6f);
            int props = ResultProperties.GetProperties(sensitive, 1, scrollable, holdable, returnable);
            StatementQuery query1 = this.CompileCursorSpecification(props, isRoutine, outerRanges);
            query1.CursorName = name;
            query1.SetCursorName(name);
            base.ReadIfThis(0x2bb);
            return query1;
        }

        public StatementDMQL CompileShortCursorSpecification(int props)
        {
            QueryExpression queryExpression = this.XreadQueryExpression();
            if (ResultProperties.IsUpdatable(props))
            {
                queryExpression.IsUpdatable = true;
            }
            queryExpression.SetReturningResult();
            queryExpression.Resolve(this.session);
            StatementQuery query1 = new StatementQuery(this.session, queryExpression);
            query1.SetDatabseObjects(this.session, this.compileContext);
            query1.CheckAccessRights(this.session);
            return query1;
        }

        private bool IsNonHostParameter(string name)
        {
            if (!this.compileContext.InRoutine && !this.session.sessionContext.SessionVariables.ContainsKey(name))
            {
                return this.compileContext.IsNoneHostParameter(name);
            }
            return true;
        }

        public bool IsNoSetOperationsAllowd()
        {
            return (this.compileContext.NoSetOperations && (this.compileContext.NoSetOperationsSubQueryDepth == this.compileContext.SubqueryDepth));
        }

        private Expression ReadAggregate()
        {
            int tokenType = base.token.TokenType;
            base.Read();
            base.ReadThis(0x2b7);
            base.ReadThis(0x2aa);
            return this.ReadAggregateExpression(tokenType);
        }

        private Expression ReadAggregateExpression(int tokenT)
        {
            int expressionType = ParserBase.GetExpressionType(tokenT);
            bool distinct = false;
            bool flag2 = false;
            if (base.token.TokenType == 0x54)
            {
                distinct = true;
                base.Read();
            }
            else if (base.token.TokenType == 2)
            {
                flag2 = true;
                base.Read();
            }
            Expression e = this.XreadValueExpression();
            int num2 = expressionType;
            if (num2 != 0x47)
            {
                if ((num2 - 0x4e) > 3)
                {
                    if (e.GetExprType() == 9)
                    {
                        throw base.UnexpectedToken();
                    }
                }
                else if (flag2 | distinct)
                {
                    throw Error.GetError(0x15ce, flag2 ? "ALL" : "DISTINCT");
                }
            }
            else if (e.GetExprType() == 0x61)
            {
                if (((ExpressionColumn) e).TableName != null)
                {
                    throw base.UnexpectedToken();
                }
                if (flag2 | distinct)
                {
                    throw base.UnexpectedToken();
                }
                e.OpType = 9;
            }
            return new ExpressionAggregate(expressionType, distinct, e);
        }

        private Expression ReadCaseExpression()
        {
            Expression l = null;
            base.Read();
            if (base.token.TokenType != 0x138)
            {
                l = this.XreadRowValuePredicand();
            }
            return this.ReadCaseWhen(l);
        }

        private Expression ReadCaseWhen(Expression l)
        {
            Expression expression3;
            Expression expression5;
            base.ReadThis(0x138);
            Expression left = null;
            if (l == null)
            {
                left = this.XreadBooleanValueExpression();
                goto Label_0062;
            }
        Label_0019:
            expression5 = this.XreadPredicateRightPart(l);
            if (l == expression5)
            {
                expression5 = new ExpressionLogical(l, this.XreadRowValuePredicand());
            }
            if (left == null)
            {
                left = expression5;
            }
            else
            {
                left = new ExpressionLogical(50, left, expression5);
            }
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0019;
            }
        Label_0062:
            base.ReadThis(0x116);
            Expression expression2 = this.XreadValueExpression();
            if (base.token.TokenType == 0x138)
            {
                expression3 = this.ReadCaseWhen(l);
            }
            else if (base.token.TokenType == 0x5b)
            {
                base.Read();
                expression3 = this.XreadValueExpression();
                base.ReadThis(0x5d);
                base.ReadIfThis(0x1c);
            }
            else
            {
                expression3 = new ExpressionValue(null, SqlType.SqlAllTypes);
                base.ReadThis(0x5d);
                base.ReadIfThis(0x1c);
            }
            return new ExpressionOp(0x5d, left, new ExpressionOp(0x60, expression2, expression3));
        }

        private Expression ReadCaseWhenExpression()
        {
            base.Read();
            base.ReadThis(0x2b7);
            Expression left = this.XreadBooleanValueExpression();
            base.ReadThis(0x2ac);
            Expression expression2 = this.XreadRowValueExpression();
            base.ReadThis(0x2ac);
            expression2 = new ExpressionOp(0x60, expression2, this.XreadValueExpression());
            left = new ExpressionOp(0x5d, left, expression2);
            base.ReadThis(0x2aa);
            return left;
        }

        private Expression ReadCastExpression()
        {
            base.Read();
            base.ReadThis(0x2b7);
            Expression e = this.XreadValueExpressionOrNull();
            if (base.token.TokenType == 0x30)
            {
                base.ReadThis(0x2ac);
            }
            else
            {
                base.ReadThis(9);
            }
            SqlType type = this.ReadTypeDefinition(true, false);
            if (e.IsUnresolvedParam())
            {
                e.SetDataType(this.session, type);
            }
            else
            {
                e = new ExpressionOp(e, type);
            }
            base.ReadThis(0x2aa);
            return e;
        }

        public int ReadCloseBrackets(int limit)
        {
            int num = 0;
            while ((num < limit) && (base.token.TokenType == 0x2aa))
            {
                base.Read();
                num++;
            }
            return num;
        }

        private Expression ReadCoalesceExpression()
        {
            Expression expression = null;
            Expression expression3;
            base.Read();
            base.ReadThis(0x2b7);
            Expression expression2 = null;
            while (true)
            {
                expression3 = this.XreadValueExpression();
                if ((expression2 != null) && (base.token.TokenType == 0x2aa))
                {
                    break;
                }
                Expression left = new ExpressionLogical(0x2f, expression3);
                Expression right = new ExpressionOp(0x60, new ExpressionValue(null, null), expression3);
                Expression e = new ExpressionOp(0x5d, left, right);
                if (expression == null)
                {
                    expression = e;
                }
                else
                {
                    expression2.SetLeftNode(e);
                }
                expression2 = right;
                base.ReadThis(0x2ac);
            }
            base.ReadThis(0x2aa);
            expression2.SetLeftNode(expression3);
            return expression;
        }

        public ISchemaObject ReadCollateClauseOrNull()
        {
            if (base.token.TokenType == 40)
            {
                base.Read();
                return this.database.schemaManager.GetSchemaObject(base.token.NamePrefix, base.token.TokenString, 15);
            }
            return null;
        }

        public Expression ReadCollection(int type)
        {
            base.Read();
            if (base.token.TokenType == 0x2b7)
            {
                return this.XreadArrayConstructor();
            }
            base.ReadThis(820);
            List<Expression> list = new List<Expression>();
            for (int i = 0; base.token.TokenType != 0x333; i++)
            {
                if (i > 0)
                {
                    base.ReadThis(0x2ac);
                }
                Expression item = this.XreadValueExpressionOrNull();
                list.Add(item);
            }
            base.Read();
            return new Expression(0x6b, list.ToArray());
        }

        private QNameManager.SimpleName[] ReadColumnNameList(OrderedHashSet<string> set)
        {
            BitArray quotedFlags = new BitArray(0x200);
            base.ReadThis(0x2b7);
            this.ReadColumnNameList(set, quotedFlags, false);
            base.ReadThis(0x2aa);
            QNameManager.SimpleName[] nameArray = new QNameManager.SimpleName[set.Size()];
            for (int i = 0; i < set.Size(); i++)
            {
                nameArray[i] = QNameManager.GetSimpleName(set.Get(i), quotedFlags[i]);
            }
            return nameArray;
        }

        public void ReadColumnNameList(OrderedHashSet<string> set, BitArray quotedFlags, bool readAscDesc)
        {
            int index = 0;
            do
            {
                if (this.session.IsProcessingScript())
                {
                    if (!base.IsSimpleName())
                    {
                        base.token.IsDelimitedIdentifier = true;
                    }
                }
                else
                {
                    base.CheckIsSimpleName();
                }
                if (!set.Add(base.token.TokenString))
                {
                    throw Error.GetError(0x15c9, base.token.TokenString);
                }
                if ((quotedFlags != null) && base.IsDelimitedIdentifier())
                {
                    quotedFlags.Set(index, true);
                }
                base.Read();
                index++;
                if (readAscDesc && ((base.token.TokenType == 0x152) || (base.token.TokenType == 0x185)))
                {
                    base.Read();
                }
            }
            while (base.ReadIfThis(0x2ac));
        }

        public QNameManager.QName[] ReadColumnNames(QNameManager.QName tableName)
        {
            BitArray quotedFlags = new BitArray(0x200, false);
            OrderedHashSet<string> set = this.ReadColumnNames(quotedFlags, false);
            QNameManager.QName[] nameArray = new QNameManager.QName[set.Size()];
            for (int i = 0; i < nameArray.Length; i++)
            {
                string name = set.Get(i);
                bool isquoted = quotedFlags.Get(i);
                nameArray[i] = this.database.NameManager.NewQName(tableName.schema, name, isquoted, 9, tableName);
            }
            return nameArray;
        }

        public OrderedHashSet<string> ReadColumnNames(bool readAscDesc)
        {
            return this.ReadColumnNames(null, readAscDesc);
        }

        public OrderedHashSet<string> ReadColumnNames(BitArray quotedFlags, bool readAscDesc)
        {
            base.ReadThis(0x2b7);
            OrderedHashSet<string> set = new OrderedHashSet<string>();
            this.ReadColumnNameList(set, quotedFlags, readAscDesc);
            base.ReadThis(0x2aa);
            return set;
        }

        private Expression ReadColumnOrFunctionOrAggregateExpression()
        {
            FunctionSQLInvoked invoked;
            Expression expression5;
            string tokenString = base.token.TokenString;
            base.IsDelimitedSimpleName();
            string namePrefix = base.token.NamePrefix;
            string namePrePrefix = base.token.NamePrePrefix;
            string namePrePrePrefix = base.token.NamePrePrePrefix;
            Token recordedToken = base.GetRecordedToken();
            if (base.IsUndelimitedSimpleName())
            {
                FunctionSQL function = FunctionCustom.NewCustomFunction(base.token.TokenString, base.token.TokenType);
                if (function != null)
                {
                    int position = base.GetPosition();
                    try
                    {
                        Expression expression = this.ReadSQLFunction(function);
                        if (expression != null)
                        {
                            return expression;
                        }
                    }
                    catch (CoreException exception)
                    {
                        exception.SetLevel(this.compileContext.SubqueryDepth);
                        if ((this.LastError == null) || (this.LastError.GetLevel() < exception.GetLevel()))
                        {
                            this.LastError = exception;
                        }
                        this.Rewind(position);
                    }
                }
                else if (base.IsReservedKey())
                {
                    function = FunctionSQL.NewSqlFunction(tokenString, this.compileContext);
                    if (function != null)
                    {
                        Expression expression3 = this.ReadSQLFunction(function);
                        if (expression3 != null)
                        {
                            return expression3;
                        }
                    }
                }
            }
            base.Read();
            if (base.token.TokenType != 0x2b7)
            {
                this.CheckValidCatalogName(namePrePrePrefix);
                Expression expression4 = this.TryReadSequenceColumnExpression(namePrePrefix, namePrefix, tokenString);
                if (expression4 != null)
                {
                    return expression4;
                }
                return new ExpressionColumn(namePrePrefix, namePrefix, tokenString, this.database.SqlEnforceRefs);
            }
            if (namePrePrePrefix != null)
            {
                throw Error.GetError(0x15af, namePrePrePrefix);
            }
            this.CheckValidCatalogName(namePrePrefix);
            namePrefix = this.session.GetSchemaName(namePrefix);
            RoutineSchema routineSchema = ((RoutineSchema) this.database.schemaManager.FindSchemaObject(tokenString, namePrefix, 0x10)) ?? ((RoutineSchema) this.database.schemaManager.FindSchemaObject(tokenString, namePrefix, 0x1b));
            if (routineSchema == null)
            {
                throw Error.GetError(0x157d, tokenString);
            }
            List<Expression> list = new List<Expression>();
            base.ReadThis(0x2b7);
            if (base.token.TokenType == 0x2aa)
            {
                base.Read();
                goto Label_020B;
            }
        Label_01D5:
            expression5 = this.XreadValueExpression();
            list.Add(expression5);
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_01D5;
            }
            base.ReadThis(0x2aa);
        Label_020B:
            invoked = new FunctionSQLInvoked(routineSchema);
            Expression[] newNodes = list.ToArray();
            invoked.SetArguments(newNodes);
            this.compileContext.AddFunctionCall(invoked);
            recordedToken.SetExpression(routineSchema);
            return invoked;
        }

        private Expression ReadDecodeExpression()
        {
            Expression expression4;
            base.Read();
            base.ReadThis(0x2b7);
            Expression expression = null;
            Expression expression2 = null;
            Expression left = this.XreadValueExpression();
            base.ReadThis(0x2ac);
            while (true)
            {
                expression4 = this.XreadValueExpression();
                if (base.token.TokenType != 0x2ac)
                {
                    break;
                }
                base.ReadThis(0x2ac);
                Expression expression5 = new ExpressionLogical(left, expression4);
                Expression expression6 = this.XreadValueExpression();
                Expression right = new ExpressionOp(0x60, expression6, null);
                Expression e = new ExpressionOp(0x5d, expression5, right);
                if (expression == null)
                {
                    expression = e;
                }
                else
                {
                    expression2.SetRightNode(e);
                }
                expression2 = right;
                if (base.token.TokenType != 0x2ac)
                {
                    expression2.SetRightNode(new ExpressionValue(null, null));
                    goto Label_00BD;
                }
                base.ReadThis(0x2ac);
            }
            expression2.SetRightNode(expression4);
        Label_00BD:
            base.ReadThis(0x2aa);
            return expression;
        }

        private void ReadExpression(List<Expression> exprList, short[] parseList, int start, int count, bool isOption)
        {
            int index = start;
            while (index < (start + count))
            {
                int num1;
                Expression expression2;
                int num4;
                Expression expression4;
                int num2 = parseList[index];
                int num3 = num2;
                if (((num3 - 0x2aa) > 2) && ((num3 - 0x2b7) > 1))
                {
                    switch (num3)
                    {
                        case 740:
                            goto Label_0068;

                        case 0x2e5:
                            goto Label_0096;

                        case 0x2e6:
                        {
                            index++;
                            int num5 = exprList.Count;
                            int position = base.GetPosition();
                            int num7 = parseList[index++];
                            int num8 = num5;
                            try
                            {
                                this.ReadExpression(exprList, parseList, index, num7, true);
                            }
                            catch (CoreException exception)
                            {
                                exception.SetLevel(this.compileContext.SubqueryDepth);
                                if ((this.LastError == null) || (this.LastError.GetLevel() < exception.GetLevel()))
                                {
                                    this.LastError = exception;
                                }
                                this.Rewind(position);
                                exprList.RemoveRange(num5, exprList.Count - num5);
                                for (int i = index; i < (index + num7); i++)
                                {
                                    if (((parseList[i] == 0x2b9) || (parseList[i] == 0x2e5)) || (parseList[i] == 0x2e8))
                                    {
                                        exprList.Add(null);
                                    }
                                }
                                index += num7 - 1;
                                goto Label_0244;
                            }
                            if (num8 == exprList.Count)
                            {
                                exprList.Add(null);
                            }
                            index += num7 - 1;
                            goto Label_0244;
                        }
                        case 0x2e7:
                        {
                            int num12;
                            index++;
                            int num10 = parseList[index++];
                            int num11 = index;
                            do
                            {
                                num12 = exprList.Count;
                                this.ReadExpression(exprList, parseList, num11, num10, true);
                            }
                            while (exprList.Count != num12);
                            index += num10 - 1;
                            goto Label_0244;
                        }
                        case 0x2e8:
                            num1 = base.ReadIntegerObject();
                            if (num1 < 0)
                            {
                                throw Error.GetError(0x15d8);
                            }
                            goto Label_022B;

                        case 0x2b9:
                        {
                            Expression expression = this.XreadAllTypesCommonValueExpression(false);
                            exprList.Add(expression);
                            goto Label_0244;
                        }
                    }
                }
                goto Label_024A;
            Label_0068:
                expression2 = this.XreadAllTypesCommonValueExpression(false);
                exprList.Add(expression2);
                if (base.token.TokenType != 0x2ac)
                {
                    goto Label_0244;
                }
                base.Read();
                goto Label_0068;
            Label_0096:
                num4 = parseList[++index];
                Expression item = null;
                if (ArrayUtil.Find(parseList, base.token.TokenType, index + 1, num4) == -1)
                {
                    if (!isOption)
                    {
                        throw base.UnexpectedToken();
                    }
                }
                else
                {
                    item = new ExpressionValue(base.token.TokenType, SqlType.SqlInteger);
                    base.Read();
                }
                exprList.Add(item);
                index += num4;
                goto Label_0244;
            Label_022B:
                expression4 = new ExpressionValue(num1, SqlType.SqlInteger);
                exprList.Add(expression4);
            Label_0244:
                index++;
                continue;
            Label_024A:
                if (base.token.TokenType != num2)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                goto Label_0244;
            }
        }

        private Expression ReadGreatestExpression()
        {
            base.Read();
            base.ReadThis(0x2b7);
            Expression e = null;
            while (true)
            {
                e = this.ReadValue(e, 0x2b);
                if (base.token.TokenType != 0x2ac)
                {
                    break;
                }
                base.ReadThis(0x2ac);
            }
            base.ReadThis(0x2aa);
            return e;
        }

        private Expression ReadLeastExpression()
        {
            base.Read();
            base.ReadThis(0x2b7);
            Expression e = null;
            while (true)
            {
                e = this.ReadValue(e, 0x2c);
                if (base.token.TokenType != 0x2ac)
                {
                    break;
                }
                base.ReadThis(0x2ac);
            }
            base.ReadThis(0x2aa);
            return e;
        }

        protected Table ReadNamedSubqueryOrNull()
        {
            if (!base.IsSimpleName())
            {
                return null;
            }
            SubQuery namedSubQuery = this.compileContext.GetNamedSubQuery(base.token.TokenString);
            if (namedSubQuery == null)
            {
                return null;
            }
            base.Read();
            return namedSubQuery.GetTable();
        }

        public QNameManager.QName ReadNewDependentSchemaObjectName(QNameManager.QName parentName, int type)
        {
            QNameManager.QName name = this.ReadNewSchemaObjectName(type, true);
            name.Parent = parentName;
            name.SetSchemaIfNull(parentName.schema);
            if (((name.schema != null) && (parentName.schema != null)) && (name.schema != parentName.schema))
            {
                throw Error.GetError(0x1581, base.token.NamePrefix);
            }
            return name;
        }

        public QNameManager.QName ReadNewSchemaName()
        {
            this.CheckIsSchemaObjectName();
            this.CheckValidCatalogName(base.token.NamePrefix);
            SqlInvariants.CheckSchemaNameNotSystem(base.token.TokenString);
            base.Read();
            return this.database.NameManager.NewQName(base.token.TokenString, base.IsDelimitedIdentifier(), 2);
        }

        public QNameManager.QName ReadNewSchemaObjectName(int type, bool checkSchema)
        {
            this.CheckIsSchemaObjectName();
            QNameManager.QName name = this.database.NameManager.NewQName(base.token.TokenString, base.IsDelimitedIdentifier(), type);
            if (base.token.NamePrefix != null)
            {
                QNameManager.QName schemaQName;
                switch (type)
                {
                    case 9:
                        if (base.token.NamePrefix != null)
                        {
                            throw base.TooManyIdentifiers();
                        }
                        goto Label_0189;

                    case 11:
                    case 0x15:
                    case 0x16:
                    case 0x19:
                    case 0x1a:
                    case 1:
                        throw base.UnexpectedToken();

                    case 2:
                        this.CheckValidCatalogName(base.token.NamePrefix);
                        if (base.token.NamePrePrefix != null)
                        {
                            throw base.TooManyIdentifiers();
                        }
                        goto Label_0189;

                    case 0x13:
                        if (((base.token.NamePrePrefix != null) || !"MODULE".Equals(base.token.NamePrefix)) || base.token.IsDelimitedPrefix)
                        {
                            throw base.UnexpectedTokenRequire("MODULE");
                        }
                        goto Label_0189;
                }
                this.CheckValidCatalogName(base.token.NamePrePrefix);
                if (checkSchema)
                {
                    schemaQName = this.session.GetSchemaQName(base.token.NamePrefix);
                }
                else
                {
                    schemaQName = this.session.database.schemaManager.FindSchemaQName(base.token.NamePrefix);
                    if (schemaQName == null)
                    {
                        schemaQName = this.database.NameManager.NewQName(base.token.NamePrefix, base.IsDelimitedIdentifier(), 2);
                    }
                }
                name.SetSchemaIfNull(schemaQName);
            }
        Label_0189:
            base.Read();
            return name;
        }

        private Expression ReadNullIfExpression()
        {
            base.Read();
            base.ReadThis(0x2b7);
            Expression right = this.XreadValueExpression();
            base.ReadThis(0x2ac);
            Expression expression2 = new ExpressionOp(0x60, new ExpressionValue(null, null), right);
            right = new ExpressionLogical(right, this.XreadValueExpression());
            right = new ExpressionOp(0x5d, right, expression2);
            base.ReadThis(0x2aa);
            return right;
        }

        public int ReadOpenBrackets()
        {
            int num = 0;
            while (base.token.TokenType == 0x2b7)
            {
                num++;
                base.Read();
            }
            return num;
        }

        public Expression ReadRow()
        {
            Expression expression = null;
            Expression expression2;
        Label_0002:
            expression2 = this.XreadValueExpressionWithContext();
            if (expression == null)
            {
                expression = expression2;
            }
            else if (expression.GetExprType() == 0x19)
            {
                if ((expression2.GetExprType() == 0x19) && (expression.nodes[0].GetExprType() != 0x19))
                {
                    Expression[] list = new Expression[] { expression, expression2 };
                    expression = new Expression(0x19, list);
                }
                else
                {
                    expression.nodes = ArrayUtil.ResizeArray<Expression>(expression.nodes, expression.nodes.Length + 1);
                    expression.nodes[expression.nodes.Length - 1] = expression2;
                }
            }
            else
            {
                Expression[] list = new Expression[] { expression, expression2 };
                expression = new Expression(0x19, list);
            }
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0002;
            }
            return expression;
        }

        public QNameManager.QName ReadSchemaName()
        {
            this.CheckIsSchemaObjectName();
            this.CheckValidCatalogName(base.token.NamePrefix);
            base.Read();
            return this.session.GetSchemaQName(base.token.TokenString);
        }

        public ISchemaObject ReadSchemaObjectName(int type)
        {
            this.CheckIsSchemaObjectName();
            this.CheckValidCatalogName(base.token.NamePrePrefix);
            string schemaName = this.session.GetSchemaName(base.token.NamePrefix);
            base.Read();
            return this.database.schemaManager.GetSchemaObject(base.token.TokenString, schemaName, type);
        }

        public ISchemaObject ReadSchemaObjectName(QNameManager.QName schemaName, int type)
        {
            this.CheckIsSchemaObjectName();
            if (base.token.NamePrefix != null)
            {
                if (!base.token.NamePrefix.Equals(schemaName.Name))
                {
                    throw Error.GetError(0x1581, base.token.NamePrefix);
                }
                if ((base.token.NamePrePrefix != null) && !base.token.NamePrePrefix.Equals(this.database.GetCatalogName().Name))
                {
                    throw Error.GetError(0x1581, base.token.NamePrefix);
                }
            }
            base.Read();
            return this.database.schemaManager.GetSchemaObject(base.token.TokenString, schemaName.Name, type);
        }

        private Expression ReadSequenceExpression()
        {
            base.Read();
            base.ReadThis(0x131);
            base.ReadThis(0x6f);
            this.CheckIsSchemaObjectName();
            string schemaName = this.session.GetSchemaName(base.token.NamePrefix);
            NumberSequence sequence = this.database.schemaManager.GetSequence(base.token.TokenString, schemaName, true);
            base.Read();
            Expression expression = new ExpressionColumn(sequence);
            base.GetRecordedToken().SetExpression(sequence);
            this.compileContext.AddSequence(sequence);
            return expression;
        }

        private ColumnSchema ReadSimpleColumnName(Table table)
        {
            base.CheckIsIdentifier();
            if (base.token.NamePrefix != null)
            {
                throw base.TooManyIdentifiers();
            }
            int i = table.FindColumn(base.token.TokenString);
            if (i == -1)
            {
                throw Error.GetError(0x157d, base.token.TokenString);
            }
            base.Read();
            return table.GetColumn(i);
        }

        private string ReadSimpleColumnName(RangeVariable rangeVar)
        {
            base.CheckIsIdentifier();
            if (base.token.NamePrefix != null)
            {
                throw base.TooManyIdentifiers();
            }
            int i = rangeVar.FindColumn(base.token.TokenString);
            if (((i <= -1) || !rangeVar.ResolvesTableName(base.token.NamePrefix)) || !rangeVar.ResolvesSchemaName(base.token.NamePrePrefix))
            {
                throw Error.GetError(0x157d, base.token.TokenString);
            }
            rangeVar.GetTable().GetColumn(i);
            base.Read();
            return base.token.TokenString;
        }

        public void ReadSimpleColumnNames(OrderedHashSet<string> columns, Table table)
        {
            do
            {
                ColumnSchema schema = this.ReadSimpleColumnName(table);
                if (!columns.Add(schema.GetName().Name))
                {
                    throw Error.GetError(0x15c9, schema.GetName().Name);
                }
            }
            while (base.ReadIfThis(0x2ac));
            if (base.token.TokenType != 0x2aa)
            {
                throw base.UnexpectedToken();
            }
        }

        public void ReadSimpleColumnNames(OrderedHashSet<string> columns, RangeVariable rangeVar)
        {
            do
            {
                string key = this.ReadSimpleColumnName(rangeVar);
                if (!columns.Add(key))
                {
                    throw Error.GetError(0x15cb, key);
                }
            }
            while (base.ReadIfThis(0x2ac));
            if (base.token.TokenType != 0x2aa)
            {
                throw base.UnexpectedToken();
            }
        }

        public QNameManager.SimpleName ReadSimpleName()
        {
            base.CheckIsSimpleName();
            base.Read();
            return QNameManager.GetSimpleName(base.token.TokenString, base.IsDelimitedIdentifier());
        }

        protected RangeVariable ReadSimpleRangeVariable(int operation)
        {
            Table subqueryTable = this.ReadTableName();
            QNameManager.SimpleName alias = null;
            if (operation != 0x451)
            {
                if (base.token.TokenType == 9)
                {
                    base.Read();
                    base.CheckIsNonCoreReservedIdentifier();
                }
                if (base.IsNonCoreReservedIdentifier())
                {
                    alias = QNameManager.GetSimpleName(base.token.TokenString, base.IsDelimitedIdentifier());
                    base.Read();
                }
            }
            if (subqueryTable.IsView())
            {
                if (operation != 0x13)
                {
                    if (operation == 0x52)
                    {
                        if (!subqueryTable.IsTriggerUpdatable() && !subqueryTable.IsUpdatable())
                        {
                            throw Error.GetError(0x15a9);
                        }
                    }
                    else if (((operation == 0x80) && (!subqueryTable.IsTriggerUpdatable() || !subqueryTable.IsTriggerDeletable())) && (!subqueryTable.IsUpdatable() || !subqueryTable.IsInsertable()))
                    {
                        throw Error.GetError(0x15a9);
                    }
                }
                else if (!subqueryTable.IsTriggerDeletable() && !subqueryTable.IsUpdatable())
                {
                    throw Error.GetError(0x15a9);
                }
                subqueryTable = ((View) subqueryTable).GetSubqueryTable();
            }
            return new RangeVariable(subqueryTable, alias, null, null, this.compileContext);
        }

        public Expression ReadSQLFunction(FunctionSQL function)
        {
            int position = base.GetPosition();
            base.Read();
            short[] parseList = function.ParseList;
            if (parseList.Length == 0)
            {
                return function;
            }
            List<Expression> exprList = new List<Expression>();
            bool flag = base.token.TokenType == 0x2b7;
            try
            {
                this.ReadExpression(exprList, parseList, 0, parseList.Length, false);
            }
            catch (CoreException)
            {
                if (!flag)
                {
                    this.Rewind(position);
                    return null;
                }
                if (function.ParseListAlt == null)
                {
                    throw;
                }
                this.Rewind(position);
                base.Read();
                parseList = function.ParseListAlt;
                exprList = new List<Expression>();
                this.ReadExpression(exprList, parseList, 0, parseList.Length, false);
            }
            Expression[] newNodes = exprList.ToArray();
            function.SetArguments(newNodes);
            return function.GetFunctionExpression();
        }

        public Table ReadTableName()
        {
            base.CheckIsIdentifier();
            if (base.token.NamePrePrefix != null)
            {
                this.CheckValidCatalogName(base.token.NamePrePrefix);
            }
            Table expression = this.database.schemaManager.GetTable(this.session, base.token.TokenString, base.token.NamePrefix);
            base.GetRecordedToken().SetExpression(expression);
            base.Read();
            return expression;
        }

        protected RangeVariable ReadTableOrSubquery()
        {
            QNameManager.SimpleName alias = null;
            QNameManager.SimpleName[] columnNameList = null;
            OrderedHashSet<string> set = null;
            Table subqueryTable;
            bool flag;
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x114)
            {
                if (tokenType == 0x93)
                {
                    subqueryTable = this.XreadLateralDerivedTable().GetTable();
                }
                else
                {
                    if (tokenType != 0x114)
                    {
                        goto Label_0076;
                    }
                    subqueryTable = this.XreadTableFunctionDerivedTable().GetTable();
                }
                goto Label_009F;
            }
            switch (tokenType)
            {
                case 0x12b:
                    subqueryTable = this.XreadCollectionDerivedTable().GetTable();
                    goto Label_009F;

                case 0x2b7:
                    subqueryTable = this.XreadTableSubqueryOrJoinedTable().GetTable();
                    goto Label_009F;
            }
        Label_0076:;
            subqueryTable = this.ReadNamedSubqueryOrNull() ?? this.ReadTableName();
            if (subqueryTable.IsView())
            {
                subqueryTable = ((View) subqueryTable).GetSubqueryTable();
            }
        Label_009F:
            flag = false;
            if (base.token.TokenType == 9)
            {
                base.Read();
                base.CheckIsNonCoreReservedIdentifier();
                flag = true;
            }
            if (base.IsNonCoreReservedIdentifier())
            {
                bool flag2 = ((base.token.TokenType == 0x242) || (base.token.TokenType == 190)) || (base.token.TokenType == 0x6a);
                bool flag3 = base.token.TokenType == 0x248;
                int position = base.GetPosition();
                alias = QNameManager.GetSimpleName(base.token.TokenString, base.IsDelimitedIdentifier());
                base.Read();
                if (base.token.TokenType == 0x2b7)
                {
                    set = new OrderedHashSet<string>();
                    columnNameList = this.ReadColumnNameList(set);
                }
                else if (!flag & flag2)
                {
                    if (((base.token.TokenType == 0x2ab) || (base.token.TokenType == 0x2b9)) || (base.token.TokenType == 0x2e9))
                    {
                        alias = null;
                        this.Rewind(position);
                    }
                }
                else if (!flag & flag3)
                {
                    this.Rewind(position);
                }
            }
            return new RangeVariable(subqueryTable, alias, set, columnNameList, this.compileContext);
        }

        public void ReadTargetSpecificationList(OrderedHashSet<Expression> targets, RangeVariable[] rangeVars, LongDeque colIndexList)
        {
            do
            {
                Expression key = this.XreadTargetSpecification(rangeVars, colIndexList);
                if (!targets.Add(key))
                {
                    ColumnSchema column = key.GetColumn();
                    throw Error.GetError(0x15cb, column.GetName().Name);
                }
            }
            while (base.ReadIfThis(0x2ac));
            if (base.token.TokenType != 0x2aa)
            {
                int tokenType = base.token.TokenType;
            }
        }

        public SqlType ReadTypeDefinition(bool includeUserTypes, bool includeTableTypes)
        {
            int num5;
            int typeNr = -2147483648;
            bool flag = false;
            bool flag2 = false;
            base.CheckIsIdentifier();
            if (base.token.NamePrefix == null)
            {
                typeNr = SqlType.GetTypeNr(base.token.TokenString);
            }
            if (typeNr == -2147483648)
            {
                if (includeUserTypes)
                {
                    this.CheckIsSchemaObjectName();
                    string schemaName = this.session.GetSchemaName(base.token.NamePrefix);
                    SqlType expression = this.database.schemaManager.GetDomain(base.token.TokenString, schemaName, false);
                    if (expression == null)
                    {
                        expression = this.database.schemaManager.GetDistinctType(base.token.TokenString, schemaName, false);
                    }
                    if (expression != null)
                    {
                        if (!includeTableTypes && (expression.IsRowType() || expression.IsTableType()))
                        {
                            throw Error.GetError(0x1585, base.token.TokenString);
                        }
                        base.GetRecordedToken().SetExpression(expression);
                        this.compileContext.AddSchemaObject(expression);
                        base.Read();
                        return expression;
                    }
                }
                throw Error.GetError(0x1585, base.token.TokenString);
            }
            base.Read();
            int num2 = typeNr;
            switch (num2)
            {
                case 8:
                    if (base.token.TokenType == 210)
                    {
                        base.Read();
                    }
                    break;

                case 9:
                case 11:
                    break;

                case 10:
                    return base.ReadIntervalType(false);

                case 12:
                case 1:
                    if (base.token.TokenType == 0x137)
                    {
                        base.Read();
                        typeNr = 12;
                    }
                    else if (base.token.TokenType == 0x91)
                    {
                        base.Read();
                        base.ReadThis(0x1c4);
                        typeNr = 40;
                    }
                    break;

                default:
                    if (num2 == 60)
                    {
                        if (base.token.TokenType == 0x137)
                        {
                            base.Read();
                            typeNr = 0x3d;
                        }
                        else if (base.token.TokenType == 0x91)
                        {
                            base.Read();
                            base.ReadThis(0x1c4);
                            typeNr = 30;
                        }
                    }
                    break;
            }
            long precision = (typeNr == 0x5d) ? 6L : 0L;
            int scale = 0;
            if ((Types.RequiresPrecision(typeNr) && (base.token.TokenType != 0x2b7)) && (this.database.SqlEnforceSize && !this.session.IsProcessingScript()))
            {
                throw Error.GetError(0x15df, SqlType.GetDefaultType(typeNr).GetNameString());
            }
            if (!Types.AcceptsPrecision(typeNr))
            {
                goto Label_04F4;
            }
            if (base.token.TokenType != 0x2b7)
            {
                if ((typeNr == 30) || (typeNr == 40))
                {
                    precision = 0x1000000L;
                }
                else if (this.database.SqlEnforceSize && ((typeNr == 1) || (typeNr == 60)))
                {
                    precision = 1L;
                }
                goto Label_0461;
            }
            int num6 = 1;
            base.Read();
            int tokenType = base.token.TokenType;
            if (tokenType == 0x2e9)
            {
                if ((base.token.DataType.TypeCode != 4) && (base.token.DataType.TypeCode != 0x19))
                {
                    throw base.UnexpectedToken();
                }
                goto Label_0349;
            }
            if (tokenType != 0x2f0)
            {
                throw base.UnexpectedToken();
            }
            if ((typeNr != 30) && (typeNr != 40))
            {
                throw base.UnexpectedToken(base.token.GetFullString());
            }
            int lobMultiplierType = base.token.LobMultiplierType;
            if (lobMultiplierType <= 0x1aa)
            {
                if (lobMultiplierType == 0x195)
                {
                    num6 = 0x40000000;
                }
                else
                {
                    if (lobMultiplierType != 0x1aa)
                    {
                        goto Label_0302;
                    }
                    num6 = 0x400;
                }
                goto Label_0349;
            }
            switch (lobMultiplierType)
            {
                case 0x1b2:
                    num6 = 0x100000;
                    goto Label_0349;
            }
        Label_0302:
            throw base.UnexpectedToken();
        Label_0349:
            flag = true;
            precision = Convert.ToInt64(base.token.TokenValue);
            if ((precision < 0L) || ((precision == 0) && !Types.AcceptsZeroPrecision(typeNr)))
            {
                throw Error.GetError(0x15d8);
            }
            precision *= num6;
            base.Read();
            if (((typeNr == 1) || (typeNr == 12)) || (typeNr == 40))
            {
                if ((base.token.TokenType == 0x163) || (base.token.TokenType == 0x337))
                {
                    base.Read();
                }
                else if ((base.token.TokenType == 0x1c5) || (base.token.TokenType == 0x338))
                {
                    base.Read();
                }
            }
            if (Types.AcceptsScaleCreateParam(typeNr) && (base.token.TokenType == 0x2ac))
            {
                base.Read();
                scale = base.ReadInteger();
                if (scale < 0)
                {
                    throw Error.GetError(0x15d8);
                }
                flag2 = true;
            }
            base.ReadThis(0x2aa);
        Label_0461:
            if ((typeNr == 0x5d) || (typeNr == 0x5c))
            {
                if (precision > 9L)
                {
                    throw Error.GetError(0x15d8);
                }
                scale = (int) precision;
                precision = 0L;
                if (base.token.TokenType == 0x13d)
                {
                    base.Read();
                    base.ReadThis(0x117);
                    base.ReadThis(0x222);
                    if (typeNr == 0x5d)
                    {
                        typeNr = 0x5f;
                    }
                    else
                    {
                        typeNr = 0x5e;
                    }
                }
                else if (base.token.TokenType == 0x13f)
                {
                    base.Read();
                    base.ReadThis(0x117);
                    base.ReadThis(0x222);
                }
            }
        Label_04F4:
            num5 = typeNr;
            switch (num5)
            {
                case -4:
                    typeNr = 0x3d;
                    if (!flag)
                    {
                        precision = 0x7fffffffL;
                    }
                    break;

                case -3:
                case -2:
                case 0:
                case 1:
                    break;

                case -1:
                    typeNr = 12;
                    if (!flag)
                    {
                        precision = 0x7fffffffL;
                    }
                    break;

                case 2:
                case 3:
                    if (!flag && !flag2)
                    {
                        precision = 0x7fffffffL;
                        scale = 0x7fff;
                    }
                    if (precision == 0x1cL)
                    {
                        precision = 0x7fffffffL;
                    }
                    if (scale == 0x1c)
                    {
                        precision = 0x7fffL;
                    }
                    break;

                default:
                    if (((num5 == 12) || ((num5 != 60) && (num5 == 0x3d))) && !flag)
                    {
                        precision = 0x8000L;
                    }
                    break;
            }
            if (this.session.IgnoreCase && (typeNr == 12))
            {
                typeNr = 100;
            }
            SqlType dataType = SqlType.GetDataType(typeNr, 0, precision, scale);
            if (base.token.TokenType != 8)
            {
                return dataType;
            }
            if (dataType.IsLobType())
            {
                throw base.UnexpectedToken();
            }
            base.Read();
            int cardinality = 0x400;
            if (base.token.TokenType == 820)
            {
                base.Read();
                cardinality = base.ReadInteger();
                if (scale < 0)
                {
                    throw Error.GetError(0x15d8);
                }
                base.ReadThis(0x333);
            }
            return new ArrayType(dataType, cardinality);
        }

        private Expression ReadValue(Expression e, int opType)
        {
            Expression right = this.XreadValueExpression();
            if (e == null)
            {
                return right;
            }
            Expression left = new ExpressionLogical(opType, e, right);
            return new ExpressionOp(0x5d, left, new ExpressionOp(0x60, e, right));
        }

        private void ReadWhereGroupHaving(QuerySpecification select)
        {
            if (base.token.TokenType == 0x13a)
            {
                base.Read();
                Expression e = this.XreadBooleanValueExpression();
                select.AddQueryCondition(e);
            }
            if (base.token.TokenType == 0x79)
            {
                base.Read();
                base.ReadThis(0x17);
                while (true)
                {
                    Expression e = this.XreadValueExpression();
                    select.AddGroupByColumnExpression(e);
                    if (base.token.TokenType != 0x2ac)
                    {
                        break;
                    }
                    base.Read();
                }
            }
            if (base.token.TokenType == 0x7c)
            {
                base.Read();
                Expression e = this.XreadBooleanValueExpression();
                select.AddHavingExpression(e);
            }
        }

        public override void Reset(string sql)
        {
            base.Reset(sql);
            this.compileContext.Reset();
            this.LastError = null;
        }

        public override void Rewind(int position)
        {
            base.Rewind(position);
            this.compileContext.Rewind(position);
        }

        private Expression TryReadSequenceColumnExpression(string sch, string seq, string column)
        {
            if (string.IsNullOrEmpty(seq) || string.IsNullOrEmpty(column))
            {
                return null;
            }
            string schemaName = this.session.GetSchemaName(sch);
            NumberSequence sequence = this.database.schemaManager.GetSequence(seq, schemaName, false);
            if (sequence == null)
            {
                return null;
            }
            Expression expression = null;
            if (column == "NEXTVAL")
            {
                expression = new ExpressionColumn(sequence, 0x6d);
            }
            else
            {
                if (column != "CURRVAL")
                {
                    return null;
                }
                expression = new ExpressionColumn(sequence, 110);
            }
            this.compileContext.AddSequence(sequence);
            return expression;
        }

        public Expression XreadAllTypesCommonValueExpression(bool boole)
        {
            int num2;
            Expression right = this.XreadAllTypesTerm(boole);
            int type = 0;
            bool flag = false;
        Label_000C:
            num2 = base.token.TokenType;
            if (num2 <= 0x2ad)
            {
                if (num2 != 0xc3)
                {
                    if (num2 != 0x2ad)
                    {
                        goto Label_0090;
                    }
                    type = 0x24;
                    boole = false;
                }
                else
                {
                    if (!boole)
                    {
                        goto Label_0090;
                    }
                    type = 50;
                }
            }
            else if (num2 != 0x2b5)
            {
                if (num2 != 0x2b8)
                {
                    goto Label_0090;
                }
                type = 0x20;
                boole = false;
            }
            else
            {
                type = 0x21;
                boole = false;
            }
        Label_005E:
            if (flag)
            {
                return right;
            }
            base.Read();
            Expression left = right;
            right = this.XreadAllTypesTerm(boole);
            if (boole)
            {
                right = new ExpressionLogical(type, left, right);
            }
            else
            {
                right = new ExpressionArithmetic(type, left, right);
            }
            goto Label_000C;
        Label_0090:
            flag = true;
            goto Label_005E;
        }

        private Expression XreadAllTypesFactor(bool boole)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            switch (base.token.TokenType)
            {
                case 0xb5:
                    if (boole)
                    {
                        base.Read();
                        flag2 = true;
                    }
                    break;

                case 0x2b5:
                    base.Read();
                    boole = false;
                    flag = true;
                    break;

                case 0x2b8:
                    base.Read();
                    boole = false;
                    break;
            }
            Expression e = this.XreadAllTypesPrimary(boole);
            if (base.token.TokenType == 140)
            {
                int position = base.GetPosition();
                base.Read();
                if (base.token.TokenType == 0xb5)
                {
                    base.Read();
                    flag2 = !flag2;
                }
                if (base.token.TokenType != 0x124)
                {
                    if (base.token.TokenType != 0x69)
                    {
                        if (base.token.TokenType != 0x12a)
                        {
                            if ((base.token.TokenType != 0xb8) && (base.token.TokenType != 0x54))
                            {
                                throw base.UnexpectedToken();
                            }
                            this.Rewind(position);
                            return e;
                        }
                        base.Read();
                        flag3 = true;
                    }
                    else
                    {
                        base.Read();
                        flag2 = !flag2;
                    }
                }
                else
                {
                    base.Read();
                }
            }
            if (flag3)
            {
                return new ExpressionLogical(0x2f, e);
            }
            if (flag)
            {
                return new ExpressionArithmetic(0x1f, e);
            }
            if (flag2)
            {
                e = new ExpressionLogical(0x30, e);
            }
            return e;
        }

        private Expression XreadAllTypesPrimary(bool boole)
        {
            FunctionSQL nsql;
            int tokenType = base.token.TokenType;
            if (tokenType <= 180)
            {
                switch (tokenType)
                {
                    case 1:
                    case 0x1a:
                    case 30:
                    case 0x1f:
                    case 0x21:
                    case 0x23:
                    case 0x66:
                    case 0x68:
                    case 110:
                    case 0x9a:
                    case 0x9f:
                    case 0xa8:
                    case 180:
                        goto Label_012B;
                }
                goto Label_01EE;
            }
            if (tokenType <= 0x107)
            {
                if (tokenType <= 0xc9)
                {
                    if (((tokenType - 0xbb) <= 1) || (tokenType == 0xc9))
                    {
                        goto Label_012B;
                    }
                }
                else if (((tokenType - 0xcf) <= 2) || (tokenType == 0x107))
                {
                    goto Label_012B;
                }
                goto Label_01EE;
            }
            if (tokenType <= 0x11e)
            {
                if (((tokenType - 270) <= 1) || (tokenType == 0x11e))
                {
                    goto Label_012B;
                }
                goto Label_01EE;
            }
            if (((tokenType != 290) && (tokenType != 0x12e)) && (tokenType != 0x13b))
            {
                goto Label_01EE;
            }
        Label_012B:
            nsql = FunctionSQL.NewSqlFunction(base.token.TokenString, this.compileContext);
            if (nsql == null)
            {
                throw base.UnsupportedFeature();
            }
            Expression left = this.ReadSQLFunction(nsql);
            if (left == null)
            {
                goto Label_01EE;
            }
            if ((((base.token.TokenType == 0x18c) || (base.token.TokenType == 0x2b6)) || ((base.token.TokenType == 680) || (base.token.TokenType == 0x2b3))) || ((base.token.TokenType == 690) || (base.token.TokenType == 0x2b4)))
            {
                base.Read();
                return new ExpressionLogical(ParserBase.GetExpressionType(base.token.TokenType), left, this.XreadAllTypesValueExpressionPrimary(boole));
            }
            goto Label_01F6;
        Label_01EE:
            left = this.XreadAllTypesValueExpressionPrimary(boole);
        Label_01F6:
            return this.XreadModifier(left);
        }

        private Expression XreadAllTypesTerm(bool boole)
        {
            int num2;
            Expression right = this.XreadAllTypesFactor(boole);
            int type = 0;
            bool flag = false;
        Label_000C:
            num2 = base.token.TokenType;
            if (num2 <= 0x2a9)
            {
                if (num2 != 5)
                {
                    if (num2 != 0x2a9)
                    {
                        goto Label_0091;
                    }
                    type = 0x22;
                    boole = false;
                }
                else
                {
                    if (!boole)
                    {
                        goto Label_0091;
                    }
                    type = 0x31;
                }
            }
            else if (num2 != 0x2ae)
            {
                if (num2 != 0x330)
                {
                    goto Label_0091;
                }
                type = 0x65;
                boole = false;
            }
            else
            {
                type = 0x23;
                boole = false;
            }
        Label_005A:
            if (flag)
            {
                return right;
            }
            base.Read();
            Expression left = right;
            right = this.XreadAllTypesFactor(boole);
            if (right == null)
            {
                throw base.UnexpectedToken();
            }
            if (boole)
            {
                right = new ExpressionLogical(type, left, right);
            }
            else
            {
                right = new ExpressionArithmetic(type, left, right);
            }
            goto Label_000C;
        Label_0091:
            flag = true;
            goto Label_005A;
        }

        private Expression XreadAllTypesValueExpressionPrimary(bool boole)
        {
            Expression e = null;
            int tokenType = base.token.TokenType;
            if (tokenType == 100)
            {
                goto Label_0060;
            }
            if (tokenType != 0xf1)
            {
                if (tokenType == 0x129)
                {
                    goto Label_0060;
                }
                e = this.XreadSimpleValueExpressionPrimary();
                if (e != null)
                {
                    e = this.XreadArrayElementReference(e);
                }
            }
            else if (!boole)
            {
                base.Read();
                base.ReadThis(0x2b7);
                e = this.XreadRowElementList(true);
                base.ReadThis(0x2aa);
            }
            goto Label_006A;
        Label_0060:
            if (boole)
            {
                return this.XreadPredicate();
            }
        Label_006A:
            if ((e == null) && (base.token.TokenType == 0x2b7))
            {
                base.Read();
                e = this.XreadRowElementList(true);
                e.NoBreak = true;
                base.ReadThis(0x2aa);
            }
            if (boole && (e != null))
            {
                e = this.XreadPredicateRightPart(e);
            }
            return e;
        }

        public Expression XreadArrayConstructor()
        {
            Expression expression;
            base.ReadThis(0x2b7);
            int position = base.GetPosition();
            this.compileContext.SubqueryDepth++;
            try
            {
                QueryExpression queryExpression = this.XreadQueryExpression();
                queryExpression.ResolveReferences(this.session, RangeVariable.EmptyArray);
                SubQuery sq = new SubQuery(this.database, this.compileContext.SubqueryDepth, queryExpression, 0x17) {
                    Sql = base.GetLastPart(position)
                };
                base.ReadThis(0x2aa);
                expression = new Expression(0x6a, sq);
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            return expression;
        }

        public Expression XreadArrayElementReference(Expression e)
        {
            if (base.token.TokenType == 820)
            {
                base.Read();
                Expression right = this.XreadNumericValueExpression();
                base.ReadThis(0x333);
                e = new ExpressionAccessor(e, right);
            }
            return e;
        }

        private ExpressionLogical XreadBetweenPredicateRightPart(Expression a)
        {
            bool flag = false;
            base.Read();
            if (base.token.TokenType == 11)
            {
                base.Read();
            }
            else if (base.token.TokenType == 0x111)
            {
                flag = true;
                base.Read();
            }
            Expression right = this.XreadRowValuePredicand();
            base.ReadThis(5);
            Expression expression2 = this.XreadRowValuePredicand();
            if (a.IsUnresolvedParam() && right.IsUnresolvedParam())
            {
                throw Error.GetError(0x15bf);
            }
            if (a.IsUnresolvedParam() && expression2.IsUnresolvedParam())
            {
                throw Error.GetError(0x15bf);
            }
            Expression left = new ExpressionLogical(0x2a, a, right);
            Expression expression4 = new ExpressionLogical(0x2d, a, expression2);
            ExpressionLogical logical = new ExpressionLogical(0x31, left, expression4);
            if (flag)
            {
                left = new ExpressionLogical(0x2d, a, right);
                expression4 = new ExpressionLogical(0x2a, a, expression2);
                return new ExpressionLogical(50, logical, new ExpressionLogical(0x31, left, expression4));
            }
            return logical;
        }

        public Expression XreadBooleanFactorOrNull()
        {
            bool flag = false;
            bool flag2 = false;
            if (base.token.TokenType == 0xb5)
            {
                base.Read();
                flag = true;
            }
            Expression e = this.XreadBooleanPrimaryOrNull();
            if (e == null)
            {
                return null;
            }
            if (base.token.TokenType == 140)
            {
                base.Read();
                if (base.token.TokenType == 0xb5)
                {
                    base.Read();
                    flag = !flag;
                }
                if (base.token.TokenType == 0x124)
                {
                    base.Read();
                }
                else if (base.token.TokenType == 0x69)
                {
                    flag = !flag;
                    base.Read();
                }
                else
                {
                    if (base.token.TokenType != 0x12a)
                    {
                        throw base.UnexpectedToken();
                    }
                    flag2 = true;
                    base.Read();
                }
            }
            if (flag2)
            {
                e = new ExpressionLogical(0x2f, e);
            }
            if (flag)
            {
                e = new ExpressionLogical(0x30, e);
            }
            return e;
        }

        public Expression XreadBooleanPrimaryOrNull()
        {
            Expression l = null;
            switch (base.token.TokenType)
            {
                case 100:
                case 0x129:
                    return this.XreadPredicate();

                case 0xf1:
                    base.Read();
                    base.ReadThis(0x2b7);
                    l = this.XreadRowElementList(true);
                    base.ReadThis(0x2aa);
                    break;

                default:
                {
                    int position = base.GetPosition();
                    try
                    {
                        l = this.XreadAllTypesCommonValueExpression(false);
                    }
                    catch (CoreException exception)
                    {
                        exception.SetLevel(this.compileContext.SubqueryDepth);
                        if ((this.LastError == null) || (this.LastError.GetLevel() < exception.GetLevel()))
                        {
                            this.LastError = exception;
                        }
                        this.Rewind(position);
                    }
                    break;
                }
            }
            if ((l == null) && (base.token.TokenType == 0x2b7))
            {
                base.Read();
                int position = base.GetPosition();
                try
                {
                    l = this.XreadRowElementList(true);
                    base.ReadThis(0x2aa);
                }
                catch (CoreException exception2)
                {
                    exception2.SetLevel(this.compileContext.SubqueryDepth);
                    if ((this.LastError == null) || (this.LastError.GetLevel() < exception2.GetLevel()))
                    {
                        this.LastError = exception2;
                    }
                    this.Rewind(position);
                    l = this.XreadBooleanValueExpression();
                    l.NoBreak = true;
                    base.ReadThis(0x2aa);
                }
            }
            if (l != null)
            {
                l = this.XreadPredicateRightPart(l);
            }
            return l;
        }

        public Expression XreadBooleanTermOrNull()
        {
            Expression right = this.XreadBooleanFactorOrNull();
            if (right != null)
            {
                while (base.token.TokenType == 5)
                {
                    base.Read();
                    Expression left = right;
                    right = this.XreadBooleanFactorOrNull();
                    if (right == null)
                    {
                        throw base.UnexpectedToken();
                    }
                    right = new ExpressionLogical(0x31, left, right);
                }
                return right;
            }
            return null;
        }

        public Expression XreadBooleanValueExpression()
        {
            Expression expression;
            try
            {
                Expression right = this.XreadBooleanTermOrNull();
                if (right != null)
                {
                    goto Label_003C;
                }
                throw Error.GetError(0x15c0);
            Label_0015:
                base.Read();
                Expression left = right;
                right = this.XreadBooleanTermOrNull();
                if (right == null)
                {
                    throw Error.GetError(0x15c0);
                }
                right = new ExpressionLogical(50, left, right);
            Label_003C:
                if (base.token.TokenType == 0xc3)
                {
                    goto Label_0015;
                }
                if (right == null)
                {
                    throw Error.GetError(0x15c0);
                }
                expression = right;
            }
            catch (CoreException exception)
            {
                exception.SetLevel(this.compileContext.SubqueryDepth);
                if ((this.LastError == null) || (this.LastError.GetLevel() < exception.GetLevel()))
                {
                    this.LastError = exception;
                }
                throw this.LastError;
            }
            return expression;
        }

        private Expression XreadCharacterPrimary()
        {
            switch (base.token.TokenType)
            {
                case 270:
                case 290:
                case 0x12e:
                case 0x9f:
                case 0xc9:
                {
                    FunctionSQL function = FunctionSQL.NewSqlFunction(base.token.TokenString, this.compileContext);
                    Expression expression = this.ReadSQLFunction(function);
                    if (expression != null)
                    {
                        return expression;
                    }
                    break;
                }
            }
            return this.XreadValueExpressionPrimary();
        }

        private Expression XreadCharacterValueExpression()
        {
            Expression right = this.XreadCharacterPrimary();
            this.ReadCollateClauseOrNull();
            while (base.token.TokenType == 0x2ad)
            {
                base.Read();
                Expression left = right;
                right = this.XreadCharacterPrimary();
                this.ReadCollateClauseOrNull();
                right = new ExpressionArithmetic(0x24, left, right);
            }
            return right;
        }

        public Expression XreadCollectionDerivedTable()
        {
            bool ordinality = false;
            int position = base.GetPosition();
            base.ReadThis(0x12b);
            base.ReadThis(0x2b7);
            this.compileContext.SubqueryDepth++;
            Expression e = null;
            try
            {
                e = this.XreadValueExpression();
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            base.ReadThis(0x2aa);
            if (base.token.TokenType == 0x13d)
            {
                base.Read();
                base.ReadThis(0x1c9);
                ordinality = true;
            }
            e = new ExpressionTable(e, null, ordinality);
            SubQuery query1 = new SubQuery(this.database, this.compileContext.SubqueryDepth, e, 0x17);
            query1.CreateTable();
            query1.Sql = base.GetLastPart(position);
            return e;
        }

        public Expression XreadContextuallyTypedTable(int degree)
        {
            Expression expression = this.ReadRow();
            Expression[] nodes = expression.nodes;
            bool flag = false;
            if (degree == 1)
            {
                if (expression.GetExprType() == 0x19)
                {
                    expression.OpType = 0x1a;
                    for (int j = 0; j < nodes.Length; j++)
                    {
                        if (nodes[j].GetExprType() != 0x19)
                        {
                            Expression[] expressionArray1 = new Expression[] { nodes[j] };
                            nodes[j] = new Expression(0x19, expressionArray1);
                        }
                        else if (nodes[j].nodes.Length != degree)
                        {
                            throw Error.GetError(0x15bc);
                        }
                    }
                    return expression;
                }
                Expression[] list = new Expression[] { expression };
                expression = new Expression(0x19, list);
                return new Expression(0x1a, new Expression[] { expression });
            }
            if (expression.GetExprType() != 0x19)
            {
                throw Error.GetError(0x15bc);
            }
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].GetExprType() == 0x19)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                expression.OpType = 0x1a;
                for (int j = 0; j < nodes.Length; j++)
                {
                    if (nodes[j].GetExprType() != 0x19)
                    {
                        throw Error.GetError(0x15bc);
                    }
                    Expression[] expressionArray2 = nodes[j].nodes;
                    if (expressionArray2.Length != degree)
                    {
                        throw Error.GetError(0x15bc);
                    }
                    for (int k = 0; k < degree; k++)
                    {
                        if (expressionArray2[k].GetExprType() == 0x19)
                        {
                            throw Error.GetError(0x15bc);
                        }
                    }
                }
                return expression;
            }
            if (nodes.Length != degree)
            {
                throw Error.GetError(0x15bc);
            }
            return new Expression(0x1a, new Expression[] { expression });
        }

        private static Expression XreadCurrentCollationSpec()
        {
            throw Error.GetError(0x5dc);
        }

        public Expression XreadDateTimeIntervalTerm()
        {
            FunctionSQL nsql;
            int tokenType = base.token.TokenType;
            if (tokenType <= 60)
            {
                switch (tokenType)
                {
                    case 1:
                    case 60:
                        goto Label_002D;
                }
                goto Label_0056;
            }
            if (((tokenType - 0x41) > 1) && ((tokenType - 0x9c) > 1))
            {
                goto Label_0056;
            }
        Label_002D:
            nsql = FunctionSQL.NewSqlFunction(base.token.TokenString, this.compileContext);
            if (nsql == null)
            {
                throw base.UnexpectedToken();
            }
            return this.ReadSQLFunction(nsql);
        Label_0056:
            return this.XreadValueExpressionPrimary();
        }

        public Expression XreadDateTimeValueFunctionOrNull()
        {
            FunctionSQL nsql;
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x9d)
            {
                if (((tokenType != 60) && ((tokenType - 0x41) > 1)) && ((tokenType - 0x9c) > 1))
                {
                    goto Label_009E;
                }
                nsql = FunctionSQL.NewSqlFunction(base.token.TokenString, this.compileContext);
                goto Label_00A0;
            }
            if (tokenType <= 0x298)
            {
                switch (tokenType)
                {
                    case 0x286:
                        goto Label_0080;

                    case 0x298:
                        goto Label_006D;
                }
                goto Label_009E;
            }
            if (tokenType == 0x29d)
            {
                goto Label_0080;
            }
            if (tokenType != 0x336)
            {
                goto Label_009E;
            }
        Label_006D:
            nsql = FunctionSQL.NewSqlFunction("LOCALTIMESTAMP", this.compileContext);
            goto Label_00A0;
        Label_0080:
            nsql = FunctionCustom.NewCustomFunction(base.token.TokenString, base.token.TokenType);
            goto Label_00A0;
        Label_009E:
            return null;
        Label_00A0:
            if (nsql == null)
            {
                throw base.UnexpectedToken();
            }
            return this.ReadSQLFunction(nsql);
        }

        private Expression XreadExplicitRowValueConstructorOrNull()
        {
            int tokenType = base.token.TokenType;
            if (tokenType == 0xf1)
            {
                base.Read();
                base.ReadThis(0x2b7);
                base.ReadThis(0x2aa);
                return this.XreadRowElementList(false);
            }
            if (tokenType != 0x2b7)
            {
                return null;
            }
            base.Read();
            int position = base.GetPosition();
            this.ReadOpenBrackets();
            switch (base.token.TokenType)
            {
                case 0xf9:
                case 0x114:
                case 0x132:
                {
                    this.Rewind(position);
                    SubQuery sq = this.XreadSubqueryBody(false, 0x16);
                    base.ReadThis(0x2aa);
                    return new Expression(0x16, sq);
                }
            }
            this.Rewind(position);
            base.ReadThis(0x2aa);
            return this.XreadRowElementList(true);
        }

        private Expression XreadFactor()
        {
            bool flag = false;
            if (base.token.TokenType == 0x2b8)
            {
                base.Read();
            }
            else if (base.token.TokenType == 0x2b5)
            {
                base.Read();
                flag = true;
            }
            Expression e = this.XreadNumericPrimary();
            if (e == null)
            {
                return null;
            }
            if (flag)
            {
                e = new ExpressionArithmetic(0x1f, e);
            }
            return e;
        }

        private void XreadFromClause(QuerySpecification select)
        {
            if (base.token.TokenType != 0x72)
            {
                this.XreadPseudoDual(select);
            }
            else
            {
                base.ReadThis(0x72);
                do
                {
                    this.XreadTableReference(select);
                }
                while (base.ReadIfThis(0x2ac));
            }
        }

        private ExpressionLogical XreadInPredicateRightPart(Expression l)
        {
            Expression expression;
            int degree = l.GetDegree();
            base.Read();
            base.ReadThis(0x2b7);
            int position = base.GetPosition();
            int limit = this.ReadOpenBrackets();
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x114)
            {
                switch (tokenType)
                {
                    case 0xf9:
                    case 0x114:
                        goto Label_008A;
                }
                goto Label_00B4;
            }
            if (tokenType == 0x12b)
            {
                expression = this.XreadCollectionDerivedTable();
                expression.GetTable().GetSubQuery().SetUniqueRows();
                base.ReadThis(0x2aa);
                this.ReadCloseBrackets(limit);
                goto Label_00CF;
            }
            if (tokenType != 0x132)
            {
                goto Label_00B4;
            }
        Label_008A:
            this.Rewind(position);
            SubQuery sq = this.XreadSubqueryBody(false, 0x36);
            expression = new Expression(0x17, sq);
            base.ReadThis(0x2aa);
            goto Label_00CF;
        Label_00B4:
            this.Rewind(position);
            expression = this.XreadInValueListConstructor(degree);
            base.ReadThis(0x2aa);
        Label_00CF:
            if (base.IsCheckOrTriggerCondition)
            {
                return new ExpressionLogical(0x36, l, expression);
            }
            ExpressionLogical logical = new ExpressionLogical(0x29, l, expression);
            logical.SetSubType(0x34);
            return logical;
        }

        public Expression XreadIntervalValueExpression()
        {
            Expression right = this.XreadDateTimeIntervalTerm();
            while (true)
            {
                int num;
                if (base.token.TokenType == 0x2b8)
                {
                    num = 0x20;
                }
                else
                {
                    if (base.token.TokenType != 0x2b5)
                    {
                        return right;
                    }
                    num = 0x21;
                }
                base.Read();
                Expression left = right;
                right = this.XreadDateTimeIntervalTerm();
                right = new ExpressionArithmetic(num, left, right);
            }
        }

        private Expression XreadInValueList(int degree)
        {
            Expression expression2;
            List<Expression> list = new List<Expression>();
        Label_0006:
            expression2 = this.XreadValueExpression();
            if (expression2.GetExprType() != 0x19)
            {
                Expression[] expressionArray1 = new Expression[] { expression2 };
                expression2 = new Expression(0x19, expressionArray1);
            }
            list.Add(expression2);
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0006;
            }
            Expression[] expressionArray = list.ToArray();
            Expression expression = new Expression(0x1a, expressionArray);
            for (int i = 0; i < expressionArray.Length; i++)
            {
                if (expressionArray[i].GetExprType() != 0x19)
                {
                    Expression[] expressionArray3 = new Expression[] { expressionArray[i] };
                    expressionArray[i] = new Expression(0x19, expressionArray3);
                }
                Expression[] nodes = expressionArray[i].nodes;
                if (nodes.Length != degree)
                {
                    throw base.UnexpectedToken();
                }
                for (int j = 0; j < degree; j++)
                {
                    if (nodes[j].GetExprType() == 0x19)
                    {
                        throw base.UnexpectedToken();
                    }
                }
            }
            return expression;
        }

        private Expression XreadInValueListConstructor(int degree)
        {
            Expression expression;
            int position = base.GetPosition();
            this.compileContext.SubqueryDepth++;
            try
            {
                Expression dataExpression = this.XreadInValueList(degree);
                new SubQuery(this.database, this.compileContext.SubqueryDepth, dataExpression, 0x36).Sql = base.GetLastPart(position);
                expression = dataExpression;
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            return expression;
        }

        public QueryExpression XreadJoinedTable()
        {
            QuerySpecification select = new QuerySpecification(this.compileContext);
            Expression e = new ExpressionColumn(0x61);
            select.AddSelectColumnExpression(e);
            this.XreadTableReference(select);
            return select;
        }

        private SubQuery XreadJoinedTableAsSubquery()
        {
            SubQuery query;
            int position = base.GetPosition();
            this.compileContext.SubqueryDepth++;
            try
            {
                QueryExpression queryExpression = this.XreadJoinedTable();
                queryExpression.Resolve(this.session);
                if (((QuerySpecification) queryExpression).RangeVariables.Length < 2)
                {
                    throw base.UnexpectedTokenRequire("JOIN");
                }
                SubQuery query1 = new SubQuery(this.database, this.compileContext.SubqueryDepth, queryExpression, 0x17) {
                    Sql = base.GetLastPart(position)
                };
                query1.PrepareTable(this.session);
                query = query1;
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            return query;
        }

        private Expression XreadLateralDerivedTable()
        {
            Expression expression;
            base.ReadThis(0x93);
            base.ReadThis(0x2b7);
            int position = base.GetPosition();
            this.compileContext.SubqueryDepth++;
            try
            {
                QueryExpression queryExpression = this.XreadQueryExpression();
                SubQuery sq = new SubQuery(this.database, this.compileContext.SubqueryDepth, queryExpression, 0x17);
                sq.CreateTable();
                sq.Sql = base.GetLastPart(position);
                base.ReadThis(0x2aa);
                expression = new Expression(0x17, sq);
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            return expression;
        }

        private ExpressionLogical XreadLikePredicateRightPart(Expression a)
        {
            base.Read();
            Expression right = this.XreadStringValueExpression();
            Expression escape = null;
            if (base.token.TokenString.Equals("ESCAPE"))
            {
                base.Read();
                escape = this.XreadStringValueExpression();
            }
            return new ExpressionLike(a, right, escape, base.IsCheckOrTriggerCondition);
        }

        protected void XreadLimit(SortAndSlice sortAndSlice)
        {
            Expression left = null;
            Expression right = null;
            if (base.token.TokenType == 190)
            {
                base.Read();
                left = this.XreadSimpleValueSpecificationOrNull();
                if (left == null)
                {
                    throw Error.GetError(0x15bd, 0x51);
                }
                if ((base.token.TokenType == 0xf1) || (base.token.TokenType == 0xf3))
                {
                    base.Read();
                }
            }
            if (base.token.TokenType == 0x242)
            {
                base.Read();
                right = this.XreadSimpleValueSpecificationOrNull();
                if (right == null)
                {
                    throw Error.GetError(0x15bd, 0x51);
                }
                if ((left == null) && (base.token.TokenType == 190))
                {
                    base.Read();
                    left = this.XreadSimpleValueSpecificationOrNull();
                }
            }
            else if (base.token.TokenType == 0x6a)
            {
                base.Read();
                if ((base.token.TokenType == 0x191) || (base.token.TokenType == 0x1bf))
                {
                    base.Read();
                }
                right = this.XreadSimpleValueSpecificationOrNull() ?? new ExpressionValue(1, SqlType.SqlInteger);
                if ((base.token.TokenType == 0xf1) || (base.token.TokenType == 0xf3))
                {
                    base.Read();
                }
                base.ReadThis(0xc1);
            }
            if (left == null)
            {
                left = new ExpressionValue(0, SqlType.SqlInteger);
            }
            bool flag = true;
            if (left.IsUnresolvedParam())
            {
                left.SetDataType(this.session, SqlType.SqlInteger);
            }
            else
            {
                flag = (left.GetDataType().TypeCode == 4) && (Convert.ToInt32(left.GetValue(null)) >= 0);
            }
            if (right != null)
            {
                if (right.IsUnresolvedParam())
                {
                    right.SetDataType(this.session, SqlType.SqlInteger);
                }
                else
                {
                    flag &= (right.GetDataType().TypeCode == 4) && (Convert.ToInt32(right.GetValue(null)) >= 0);
                }
            }
            if (!flag)
            {
                throw Error.GetError(0x15bd, 0x51);
            }
            sortAndSlice.AddLimitCondition(new ExpressionOp(0x5f, left, right));
        }

        private ExpressionLogical XreadMatchPredicateRightPart(Expression a)
        {
            bool flag = false;
            int type = 0x3b;
            base.Read();
            if (base.token.TokenType == 0x129)
            {
                base.Read();
                flag = true;
            }
            if (base.token.TokenType == 0x1fd)
            {
                base.Read();
                type = flag ? 0x3e : 0x3b;
            }
            else if (base.token.TokenType == 0x1d5)
            {
                base.Read();
                type = flag ? 0x3f : 60;
            }
            else if (base.token.TokenType == 0x73)
            {
                base.Read();
                type = flag ? 0x40 : 0x3d;
            }
            int mode = flag ? 0x17 : 0x36;
            return new ExpressionLogical(type, a, this.XreadTableSubqueryForPredicate(mode));
        }

        private Expression XreadModifier(Expression e)
        {
            IntervalType type;
            int tokenType = base.token.TokenType;
            if (tokenType > 0x7e)
            {
                if (tokenType <= 0xab)
                {
                    if ((tokenType != 0xa7) && (tokenType != 0xab))
                    {
                        return e;
                    }
                }
                else if ((tokenType != 0xf8) && (tokenType != 0x141))
                {
                    return e;
                }
            }
            else
            {
                if (tokenType > 40)
                {
                    switch (tokenType)
                    {
                        case 0x48:
                        case 0x7e:
                            goto Label_0149;
                    }
                    return e;
                }
                if (tokenType == 12)
                {
                    base.Read();
                    Expression expression = null;
                    if (base.token.TokenType == 0x9b)
                    {
                        base.Read();
                    }
                    else
                    {
                        base.ReadThis(0x117);
                        base.ReadThis(0x222);
                        expression = this.XreadValueExpressionPrimary();
                        switch (base.token.TokenType)
                        {
                            case 0xab:
                            case 0xf8:
                            case 0x141:
                            case 0x48:
                            case 0x7e:
                            case 0xa7:
                            {
                                IntervalType dataType = base.ReadIntervalType(false);
                                if (expression.GetExprType() == 0x21)
                                {
                                    expression.DataType = dataType;
                                }
                                else
                                {
                                    expression = new ExpressionOp(expression, dataType);
                                }
                                break;
                            }
                        }
                    }
                    e = new ExpressionOp(0x5c, e, expression);
                    return e;
                }
                if (tokenType == 40)
                {
                    base.Read();
                    this.database.schemaManager.GetSchemaObject(base.token.NamePrefix, base.token.TokenString, 15);
                }
                return e;
            }
        Label_0149:
            type = base.ReadIntervalType(true);
            if (e.GetExprType() == 0x21)
            {
                e.DataType = type;
                return e;
            }
            e = new ExpressionOp(e, type);
            return e;
        }

        private List<SubQuery> XreadNamedSubQuery(bool resolve)
        {
            if (base.token.TokenType != 0x13d)
            {
                return null;
            }
            List<SubQuery> list = new List<SubQuery>();
            base.Read();
            bool flag = false;
            if (base.token.TokenType == 0xda)
            {
                base.Read();
                flag = true;
            }
            this.compileContext.InitSubqueryNames();
        Label_0048:
            base.CheckIsSimpleName();
            QNameManager.QName[] columnNames = null;
            QNameManager.QName tableName = this.database.NameManager.NewQName(base.token.TokenString, base.IsDelimitedIdentifier(), 30);
            tableName.SetSchemaIfNull(SqlInvariants.SystemSchemaQname);
            base.Read();
            if (base.token.TokenType == 0x2b7)
            {
                columnNames = this.ReadColumnNames(tableName);
            }
            base.ReadThis(9);
            base.ReadThis(0x2b7);
            SubQuery item = flag ? this.XreadTableNamedSubqueryBodyRecursive(tableName, columnNames, resolve) : this.XreadTableNamedSubqueryBody(tableName, columnNames, resolve);
            item.IsNamed = true;
            item.SetDependents();
            base.ReadThis(0x2aa);
            list.Add(item);
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0048;
            }
            return list;
        }

        private Expression XreadNumericPrimary()
        {
            switch (base.token.TokenType)
            {
                case 1:
                case 0x1a:
                case 30:
                case 0x1f:
                case 0x21:
                case 0x23:
                case 0x66:
                case 0x68:
                case 110:
                case 0x9a:
                case 0xa8:
                case 0xcf:
                case 0xd1:
                case 0x107:
                case 0xbc:
                {
                    FunctionSQL function = FunctionSQL.NewSqlFunction(base.token.TokenString, this.compileContext);
                    if (function == null)
                    {
                        throw base.UnexpectedToken();
                    }
                    Expression expression = this.ReadSQLFunction(function);
                    if (expression != null)
                    {
                        return expression;
                    }
                    break;
                }
            }
            return this.XreadValueExpressionPrimary();
        }

        private Expression XreadNumericValueExpression()
        {
            Expression right = this.XreadTerm();
            while (true)
            {
                int num;
                if (base.token.TokenType == 0x2b8)
                {
                    num = 0x20;
                }
                else
                {
                    if (base.token.TokenType != 0x2b5)
                    {
                        return right;
                    }
                    num = 0x21;
                }
                base.Read();
                Expression left = right;
                right = this.XreadTerm();
                right = new ExpressionArithmetic(num, left, right);
            }
        }

        private SortAndSlice XreadOrderBy()
        {
            SortAndSlice slice = new SortAndSlice();
            while (true)
            {
                ExpressionOrderBy e = new ExpressionOrderBy(this.XreadValueExpression());
                if (base.token.TokenType == 0x185)
                {
                    e.SetDescending();
                    base.Read();
                }
                else if (base.token.TokenType == 0x152)
                {
                    base.Read();
                }
                if (base.token.TokenType == 450)
                {
                    base.Read();
                    if (base.token.TokenType == 0x191)
                    {
                        base.Read();
                    }
                    else
                    {
                        if (base.token.TokenType != 430)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        e.SetNullsLast();
                    }
                }
                slice.AddOrderExpression(e);
                if (base.token.TokenType != 0x2ac)
                {
                    return slice;
                }
                base.Read();
            }
        }

        private SortAndSlice XreadOrderByExpression()
        {
            SortAndSlice sortAndSlice = null;
            if (base.token.TokenType == 0xc4)
            {
                base.Read();
                base.ReadThis(0x17);
                sortAndSlice = this.XreadOrderBy();
            }
            if (((base.token.TokenType == 0x242) || (base.token.TokenType == 0x6a)) || (base.token.TokenType == 190))
            {
                if (sortAndSlice == null)
                {
                    sortAndSlice = new SortAndSlice();
                }
                this.XreadLimit(sortAndSlice);
            }
            return (sortAndSlice ?? new SortAndSlice());
        }

        private ExpressionLogical XreadOverlapsPredicateRightPart(Expression l)
        {
            if (l.GetExprType() != 0x19)
            {
                throw Error.GetError(0x15bc);
            }
            if (l.nodes.Length != 2)
            {
                throw Error.GetError(0x15bc);
            }
            base.Read();
            if (base.token.TokenType != 0x2b7)
            {
                throw base.UnexpectedToken();
            }
            Expression right = this.XreadRowValuePredicand();
            if (right.nodes.Length != 2)
            {
                throw Error.GetError(0x15bc);
            }
            return new ExpressionLogical(0x38, l, right);
        }

        public Expression XreadPredicate()
        {
            int tokenType = base.token.TokenType;
            if (tokenType == 100)
            {
                base.Read();
                return new ExpressionLogical(0x37, this.XreadTableSubqueryForPredicate(0x37));
            }
            if (tokenType != 0x129)
            {
                Expression l = this.XreadRowValuePredicand();
                return this.XreadPredicateRightPart(l);
            }
            base.Read();
            return new ExpressionLogical(0x39, this.XreadTableSubqueryForPredicate(0x39));
        }

        public Expression XreadPredicateRightPart(Expression l)
        {
            ExpressionLogical logical;
            bool flag = false;
            if (base.token.TokenType == 0xb5)
            {
                base.Read();
                flag = true;
            }
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x98)
            {
                if (tokenType <= 0x80)
                {
                    if (tokenType == 0x11)
                    {
                        logical = this.XreadBetweenPredicateRightPart(l);
                    }
                    else
                    {
                        if (tokenType != 0x80)
                        {
                            goto Label_0212;
                        }
                        logical = this.XreadInPredicateRightPart(l);
                        logical.NoOptimisation = base.IsCheckOrTriggerCondition;
                    }
                }
                else if (tokenType != 140)
                {
                    if (tokenType != 0x98)
                    {
                        goto Label_0212;
                    }
                    logical = this.XreadLikePredicateRightPart(l);
                    logical.NoOptimisation = base.IsCheckOrTriggerCondition;
                }
                else
                {
                    if (flag)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    if (base.token.TokenType == 0xb5)
                    {
                        flag = true;
                        base.Read();
                    }
                    if (base.token.TokenType == 0x54)
                    {
                        base.Read();
                        base.ReadThis(0x72);
                        Expression right = this.XreadRowValuePredicand();
                        logical = new ExpressionLogical(0x3a, l, right);
                        flag = !flag;
                    }
                    else
                    {
                        if ((base.token.TokenType != 0xb8) && (base.token.TokenType != 0x12a))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        logical = new ExpressionLogical(0x2f, l);
                    }
                }
                goto Label_021E;
            }
            if (tokenType <= 200)
            {
                if (tokenType == 160)
                {
                    logical = this.XreadMatchPredicateRightPart(l);
                }
                else
                {
                    if (tokenType != 200)
                    {
                        goto Label_0212;
                    }
                    if (flag)
                    {
                        throw base.UnexpectedToken();
                    }
                    logical = this.XreadOverlapsPredicateRightPart(l);
                }
                goto Label_021E;
            }
            switch (tokenType)
            {
                case 690:
                case 0x2b3:
                case 0x2b4:
                case 0x2b6:
                case 680:
                case 0x18c:
                {
                    if (flag)
                    {
                        throw base.UnexpectedToken();
                    }
                    int expressionType = ParserBase.GetExpressionType(base.token.TokenType);
                    base.Read();
                    switch (base.token.TokenType)
                    {
                        case 2:
                        case 6:
                        case 0x100:
                            logical = this.XreadQuantifiedComparisonRightPart(expressionType, l);
                            goto Label_021E;
                    }
                    Expression right = this.XreadRowValuePredicand();
                    logical = new ExpressionLogical(expressionType, l, right);
                    goto Label_021E;
                }
            }
        Label_0212:
            if (flag)
            {
                throw base.UnexpectedToken();
            }
            return l;
        Label_021E:
            if (flag)
            {
                logical = new ExpressionLogical(0x30, logical);
            }
            return logical;
        }

        private void XreadPseudoDual(QuerySpecification select)
        {
            RangeVariable rangeVar = new RangeVariable(this.database.schemaManager.GetTable(this.session, "DUAL", null), null, null, null, this.compileContext);
            select.AddRangeVariable(rangeVar);
        }

        private ExpressionLogical XreadQuantifiedComparisonRightPart(int exprType, Expression l)
        {
            int num3;
            Expression expression;
            int tokenType = base.token.TokenType;
            int num2 = base.token.TokenType;
            if (num2 != 2)
            {
                if ((num2 != 6) && (num2 != 0x100))
                {
                    throw Error.RuntimeError(0xc9, "ParserDQL");
                }
                num3 = 0x34;
            }
            else
            {
                num3 = 0x33;
            }
            base.Read();
            base.ReadThis(0x2b7);
            int position = base.GetPosition();
            this.ReadOpenBrackets();
            switch (base.token.TokenType)
            {
                case 0xf9:
                case 0x114:
                case 0x132:
                {
                    this.Rewind(position);
                    SubQuery sq = this.XreadSubqueryBody(false, 0x36);
                    expression = new Expression(0x17, sq);
                    base.ReadThis(0x2aa);
                    break;
                }
                default:
                    this.Rewind(position);
                    expression = this.ReadAggregateExpression(tokenType);
                    base.ReadThis(0x2aa);
                    break;
            }
            ExpressionLogical logical1 = new ExpressionLogical(exprType, l, expression);
            logical1.SetSubType(num3);
            return logical1;
        }

        public QueryExpression XreadQueryExpression()
        {
            this.XreadNamedSubQuery(true);
            QueryExpression expression = this.XreadQueryExpressionBody();
            SortAndSlice sortAndSlice = this.XreadOrderByExpression();
            if (expression.ExprSortAndSlice == null)
            {
                expression.AddSortAndSlice(sortAndSlice);
                return expression;
            }
            if (expression.ExprSortAndSlice.HasLimit())
            {
                if (sortAndSlice.HasLimit())
                {
                    throw Error.GetError(0x15ad);
                }
                for (int i = 0; i < sortAndSlice.ExprList.Count; i++)
                {
                    ExpressionOrderBy e = sortAndSlice.ExprList[i];
                    expression.ExprSortAndSlice.AddOrderExpression(e);
                }
                return expression;
            }
            expression.AddSortAndSlice(sortAndSlice);
            return expression;
        }

        private QueryExpression XreadQueryExpressionBody()
        {
            QueryExpression queryExpression = this.XreadQueryTerm();
            if (this.IsNoSetOperationsAllowd())
            {
                return queryExpression;
            }
            while (true)
            {
                int tokenType = base.token.TokenType;
                if (((tokenType != 0x61) && (tokenType != 0x128)) && (tokenType != 0x248))
                {
                    return queryExpression;
                }
                queryExpression = this.XreadSetOperation(queryExpression);
            }
        }

        private QueryExpression XreadQueryPrimary()
        {
            switch (base.token.TokenType)
            {
                case 0x132:
                case 0xf9:
                case 0x114:
                    return this.XreadSimpleTable();

                case 0x2b7:
                {
                    base.Read();
                    this.XreadNamedSubQuery(true);
                    QueryExpression expression = this.XreadQueryExpressionBody();
                    SortAndSlice sortAndSlice = this.XreadOrderByExpression();
                    base.ReadThis(0x2aa);
                    if (expression.ExprSortAndSlice == null)
                    {
                        expression.AddSortAndSlice(sortAndSlice);
                        return expression;
                    }
                    if (expression.ExprSortAndSlice.HasLimit())
                    {
                        if (sortAndSlice.HasLimit())
                        {
                            throw Error.GetError(0x15ad);
                        }
                        for (int i = 0; i < sortAndSlice.ExprList.Count; i++)
                        {
                            ExpressionOrderBy e = sortAndSlice.ExprList[i];
                            expression.ExprSortAndSlice.AddOrderExpression(e);
                        }
                        return expression;
                    }
                    expression.AddSortAndSlice(sortAndSlice);
                    return expression;
                }
            }
            throw base.UnexpectedToken();
        }

        public QuerySpecification XreadQuerySpecification()
        {
            QuerySpecification select = this.XreadSelect();
            this.XreadTableExpression(select);
            return select;
        }

        private QueryExpression XreadQueryTerm()
        {
            QueryExpression queryExpression = this.XreadQueryPrimary();
            if (!this.IsNoSetOperationsAllowd())
            {
                while (base.token.TokenType == 0x88)
                {
                    queryExpression = this.XreadSetOperation(queryExpression);
                }
                return queryExpression;
            }
            return queryExpression;
        }

        private Expression XreadRowElementList(bool multiple)
        {
            Expression expression;
            List<Expression> list = new List<Expression>();
            while (true)
            {
                expression = this.XreadValueExpression();
                list.Add(expression);
                if (base.token.TokenType != 0x2ac)
                {
                    break;
                }
                base.Read();
            }
            if (multiple && (list.Count == 1))
            {
                return expression;
            }
            return new Expression(0x19, list.ToArray());
        }

        public Expression XreadRowOrCommonValueExpression()
        {
            return this.XreadAllTypesCommonValueExpression(false);
        }

        private Expression XreadRowValueExpression()
        {
            Expression expression = this.XreadExplicitRowValueConstructorOrNull();
            if (expression != null)
            {
                return expression;
            }
            return this.XreadRowValueSpecialCase();
        }

        private SubQuery XreadRowValueExpressionList()
        {
            SubQuery query;
            base.GetPosition();
            this.compileContext.SubqueryDepth++;
            try
            {
                Expression dataExpression = this.XreadRowValueExpressionListBody();
                ExpressionColumn.CheckColumnsResolved(dataExpression.ResolveColumnReferences(RangeVariable.EmptyArray, null));
                dataExpression.ResolveTypes(this.session, null);
                dataExpression.PrepareTable(this.session, null, dataExpression.nodes[0].nodes.Length);
                SubQuery query1 = new SubQuery(this.database, this.compileContext.SubqueryDepth, dataExpression, 0x1a);
                query1.PrepareTable(this.session);
                query = query1;
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            return query;
        }

        private Expression XreadRowValueExpressionListBody()
        {
            Expression expression = null;
            int num2;
        Label_0002:
            num2 = this.ReadOpenBrackets();
            Expression expression2 = this.ReadRow();
            this.ReadCloseBrackets(num2);
            if (expression == null)
            {
                Expression[] list = new Expression[] { expression2 };
                expression = new Expression(0x19, list);
            }
            else
            {
                expression.nodes = ArrayUtil.ResizeArray<Expression>(expression.nodes, expression.nodes.Length + 1);
                expression.nodes[expression.nodes.Length - 1] = expression2;
            }
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_0002;
            }
            Expression[] nodes = expression.nodes;
            int length = 1;
            if (nodes[0].GetExprType() == 0x19)
            {
                length = nodes[0].nodes.Length;
            }
            expression.OpType = 0x1a;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].GetExprType() == 0x19)
                {
                    if (nodes[i].nodes.Length != length)
                    {
                        throw Error.GetError(0x15bc);
                    }
                }
                else
                {
                    if (length != 1)
                    {
                        throw Error.GetError(0x15bc);
                    }
                    Expression[] list = new Expression[] { nodes[i] };
                    nodes[i] = new Expression(0x19, list);
                }
            }
            return expression;
        }

        public Expression XreadRowValuePredicand()
        {
            return this.XreadRowOrCommonValueExpression();
        }

        public Expression XreadRowValueSpecialCase()
        {
            Expression e = this.XreadSimpleValueExpressionPrimary();
            if (e != null)
            {
                e = this.XreadArrayElementReference(e);
            }
            return e;
        }

        public QuerySpecification XreadSelect()
        {
            QuerySpecification specification = new QuerySpecification(this.compileContext);
            base.ReadThis(0xf9);
            if ((base.token.TokenType == 670) || (base.token.TokenType == 0x242))
            {
                SortAndSlice sortAndSlice = this.XreadTopOrLimit();
                if (sortAndSlice != null)
                {
                    specification.AddSortAndSlice(sortAndSlice);
                }
            }
            if (base.token.TokenType == 0x54)
            {
                specification.IsDistinctSelect = true;
                base.Read();
            }
            else if (base.token.TokenType == 2)
            {
                base.Read();
            }
            do
            {
                Expression e = this.XreadValueExpression();
                if (base.token.TokenType == 9)
                {
                    base.Read();
                    base.CheckIsNonCoreReservedIdentifier();
                }
                if (base.IsNonCoreReservedIdentifier())
                {
                    QNameManager.SimpleName simpleName = QNameManager.GetSimpleName(base.token.TokenString, base.IsDelimitedIdentifier());
                    e.SetAlias(simpleName);
                    base.Read();
                }
                specification.AddSelectColumnExpression(e);
            }
            while (((base.token.TokenType != 0x72) && (base.token.TokenType != 0x8b)) && base.ReadIfThis(0x2ac));
            return specification;
        }

        private QueryExpression XreadSetOperation(QueryExpression queryExpression)
        {
            queryExpression = new QueryExpression(this.compileContext, queryExpression);
            int unionType = this.XreadUnionType();
            this.XreadUnionCorrespondingClause(queryExpression);
            QueryExpression expression = this.XreadQueryTerm();
            queryExpression.AddUnion(expression, unionType);
            return queryExpression;
        }

        private QuerySpecification XreadSimpleTable()
        {
            QuerySpecification specification;
            int tokenType = base.token.TokenType;
            switch (tokenType)
            {
                case 0xf9:
                    return this.XreadQuerySpecification();

                case 0x114:
                {
                    base.Read();
                    Table table = this.ReadTableName();
                    specification = new QuerySpecification(this.session, table, this.compileContext);
                    specification.ResolveReferences(this.session, RangeVariable.EmptyArray);
                    specification.ResolveTypes(this.session);
                    return specification;
                }
            }
            if (tokenType != 0x132)
            {
                throw base.UnexpectedToken();
            }
            base.Read();
            SubQuery query = this.XreadRowValueExpressionList();
            specification = new QuerySpecification(this.session, query.GetTable(), this.compileContext);
            specification.ResolveReferences(this.session, RangeVariable.EmptyArray);
            specification.ResolveTypes(this.session);
            return specification;
        }

        private Expression XreadSimpleValueExpressionPrimary()
        {
            SubQuery query2;
            Expression expression = this.XreadUnsignedValueSpecificationOrNull();
            if (expression != null)
            {
                return expression;
            }
            int tokenType = base.token.TokenType;
            if (tokenType <= 0xa6)
            {
                switch (tokenType)
                {
                    case 0x27:
                        goto Label_033B;

                    case 0x30:
                    case 0x1d:
                        return this.ReadCastExpression();

                    case 0x33:
                    case 6:
                    case 15:
                    case 0x60:
                    case 0xa1:
                    case 0xa6:
                        goto Label_0355;

                    case 8:
                        return this.ReadCollection(0x6b);

                    case 0x1c:
                        return this.ReadCaseExpression();

                    case 0x47:
                    case 0x8a:
                        goto Label_0342;

                    case 0x97:
                        goto Label_034E;
                }
                goto Label_035C;
            }
            if (tokenType <= 280)
            {
                if (tokenType <= 0x100)
                {
                    switch (tokenType)
                    {
                        case 0xb9:
                            return this.ReadNullIfExpression();

                        case 0xee:
                            goto Label_034E;

                        case 0x100:
                            goto Label_0355;
                    }
                }
                else
                {
                    if ((tokenType - 0x10b) <= 1)
                    {
                        goto Label_0355;
                    }
                    switch (tokenType)
                    {
                        case 0x114:
                        {
                            base.Read();
                            base.ReadThis(0x2b7);
                            SubQuery sq = this.XreadTableSubqueryBody(false);
                            base.ReadThis(0x2aa);
                            return new Expression(0x17, sq);
                        }
                        case 0x117:
                        case 280:
                            goto Label_0342;

                        case 0x110:
                            goto Label_0355;
                    }
                }
                goto Label_035C;
            }
            if (tokenType <= 0x22e)
            {
                if ((tokenType - 0x133) > 1)
                {
                    if (tokenType == 0x1bf)
                    {
                        return this.ReadSequenceExpression();
                    }
                    if (tokenType == 0x22e)
                    {
                        return this.ReadCaseWhenExpression();
                    }
                    goto Label_035C;
                }
                goto Label_0355;
            }
            if (tokenType <= 0x2a9)
            {
                switch (tokenType)
                {
                    case 0x2a5:
                        return this.ReadGreatestExpression();

                    case 0x2a6:
                        return this.ReadLeastExpression();

                    case 0x2a9:
                        expression = new ExpressionColumn(base.token.NamePrePrefix, base.token.NamePrefix);
                        base.GetRecordedToken().SetExpression(expression);
                        base.Read();
                        return expression;

                    case 0x277:
                        goto Label_033B;
                }
                goto Label_035C;
            }
            if (tokenType != 0x2b7)
            {
                if (tokenType == 0x329)
                {
                    return this.ReadDecodeExpression();
                }
                goto Label_035C;
            }
            int position = base.GetPosition();
            base.Read();
            int num3 = base.GetPosition();
            this.ReadOpenBrackets();
            int num4 = base.token.TokenType;
            if (num4 <= 0x114)
            {
                switch (num4)
                {
                    case 0xf9:
                    case 0x114:
                        goto Label_029D;
                }
                goto Label_0323;
            }
            if (num4 != 0x132)
            {
                if (num4 != 0x13d)
                {
                    goto Label_0323;
                }
                this.XreadNamedSubQuery(true);
            }
        Label_029D:
            this.Rewind(num3);
            try
            {
                query2 = this.XreadSubqueryBody(false, 0x15);
                base.ReadThis(0x2aa);
            }
            catch (CoreException exception)
            {
                exception.SetLevel(this.compileContext.SubqueryDepth);
                if ((this.LastError == null) || (this.LastError.GetLevel() < exception.GetLevel()))
                {
                    this.LastError = exception;
                }
                this.Rewind(position);
                return null;
            }
            if (query2.queryExpression.IsSingleColumn())
            {
                return new Expression(0x15, query2);
            }
            return new Expression(0x16, query2);
        Label_0323:
            this.Rewind(position);
            return null;
        Label_033B:
            return this.ReadCoalesceExpression();
        Label_0342:
            expression = base.ReadDateTimeIntervalLiteral();
            if (expression != null)
            {
                return expression;
            }
        Label_034E:
            return this.ReadColumnOrFunctionOrAggregateExpression();
        Label_0355:
            return this.ReadAggregate();
        Label_035C:
            if (base.IsCoreReservedKey())
            {
                throw base.UnexpectedToken();
            }
            goto Label_034E;
        }

        private Expression XreadSimpleValueSpecificationOrNull()
        {
            int tokenType = base.token.TokenType;
            switch (tokenType)
            {
                case 0x2b9:
                {
                    Expression e = new ExpressionColumn(8);
                    this.compileContext.AddParameter(e, base.GetPosition());
                    base.Read();
                    return e;
                }
                case 0x2e9:
                    base.Read();
                    return new ExpressionValue(base.token.TokenValue, base.token.DataType);
            }
            if ((tokenType - 0x2ea) > 1)
            {
                return null;
            }
            Expression expression2 = this.TryReadSequenceColumnExpression(base.token.NamePrePrefix, base.token.NamePrefix, base.token.TokenString);
            if (expression2 != null)
            {
                return expression2;
            }
            this.CheckValidCatalogName(base.token.NamePrePrePrefix);
            return new ExpressionColumn(base.token.NamePrePrefix, base.token.NamePrefix, base.token.TokenString, this.database.SqlEnforceRefs);
        }

        private Expression XreadStringValueExpression()
        {
            return this.XreadCharacterValueExpression();
        }

        public SubQuery XreadSubqueryBody(bool resolve, int mode)
        {
            SubQuery query;
            int position = base.GetPosition();
            this.compileContext.SubqueryDepth++;
            try
            {
                QueryExpression queryExpression = this.XreadQueryExpression();
                if (mode == 0x37)
                {
                    if (queryExpression.ExprSortAndSlice == null)
                    {
                        SortAndSlice sortAndSlice = new SortAndSlice();
                        ExpressionValue left = new ExpressionValue(0, SqlType.SqlInteger);
                        ExpressionValue right = new ExpressionValue(0x7fffffff, SqlType.SqlInteger);
                        sortAndSlice.AddLimitCondition(new ExpressionOp(0x5f, left, right));
                        queryExpression.AddSortAndSlice(sortAndSlice);
                    }
                    else if (queryExpression.ExprSortAndSlice.HasLimit())
                    {
                        queryExpression.ExprSortAndSlice.LimitCondition.nodes[1] = new ExpressionValue(1, SqlType.SqlInteger);
                    }
                    else
                    {
                        ExpressionValue left = new ExpressionValue(0, SqlType.SqlInteger);
                        ExpressionValue right = new ExpressionValue(1, SqlType.SqlInteger);
                        queryExpression.ExprSortAndSlice.AddLimitCondition(new ExpressionOp(0x5f, left, right));
                    }
                }
                if (resolve)
                {
                    queryExpression.Resolve(this.session);
                }
                else
                {
                    queryExpression.ResolveReferences(this.session, RangeVariable.EmptyArray);
                }
                query = new SubQuery(this.database, this.compileContext.SubqueryDepth, queryExpression, mode) {
                    Sql = base.GetLastPart(position)
                };
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            return query;
        }

        public void XreadTableExpression(QuerySpecification select)
        {
            this.XreadFromClause(select);
            this.ReadWhereGroupHaving(select);
        }

        private Expression XreadTableFunctionDerivedTable()
        {
            int position = base.GetPosition();
            base.ReadThis(0x114);
            base.ReadThis(0x2b7);
            this.compileContext.SubqueryDepth++;
            Expression e = null;
            try
            {
                e = this.XreadValueExpression();
                if (e.GetExprType() != 0x1b)
                {
                    throw base.UnexpectedToken("TABLE");
                }
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            base.ReadThis(0x2aa);
            e = new ExpressionTable(e, null, false);
            SubQuery query1 = new SubQuery(this.database, this.compileContext.SubqueryDepth, e, 0x17);
            query1.SetCorrelated();
            query1.CreateTable();
            query1.Sql = base.GetLastPart(position);
            return e;
        }

        public SubQuery XreadTableNamedSubqueryBody(QNameManager.QName name, QNameManager.QName[] columnNames, bool resolve)
        {
            SubQuery subquery = this.XreadSubqueryBody(resolve, 0x17);
            subquery.PrepareTable(this.session, name, columnNames);
            this.compileContext.RegisterSubquery(name.Name, subquery);
            return subquery;
        }

        public SubQuery XreadTableNamedSubqueryBodyRecursive(QNameManager.QName name, QNameManager.QName[] columnNames, bool resolve)
        {
            int position = base.GetPosition();
            this.compileContext.SubqueryDepth++;
            bool noSetOperations = this.compileContext.NoSetOperations;
            int noSetOperationsSubQueryDepth = this.compileContext.NoSetOperationsSubQueryDepth;
            this.compileContext.NoSetOperations = true;
            this.compileContext.NoSetOperationsSubQueryDepth = this.compileContext.SubqueryDepth;
            RecursiveSubQuery subquery = null;
            try
            {
                int num;
                QueryExpression queryExpression = this.XreadQueryExpression();
            Label_0061:
                num = base.GetPosition();
                try
                {
                    queryExpression = this.XreadSetOperation(queryExpression);
                    goto Label_0061;
                }
                catch (Exception)
                {
                    this.Rewind(num);
                }
                if (resolve)
                {
                    if (this.compileContext.InRoutine || (this.session.sessionContext.SessionVariablesRange == null))
                    {
                        queryExpression.Resolve(this.session);
                    }
                    else
                    {
                        queryExpression.Resolve(this.session, this.session.sessionContext.SessionVariablesRange, null);
                    }
                }
                else
                {
                    queryExpression.ResolveReferences(this.session, RangeVariable.EmptyArray);
                }
                queryExpression.IsMergeable = false;
                subquery = new RecursiveSubQuery(this.database, this.compileContext.SubqueryDepth, queryExpression, 0x17) {
                    Sql = base.GetLastPart(position)
                };
                this.compileContext.SubqueryDepth--;
                subquery.PrepareTable(this.session, name, columnNames);
                this.compileContext.RegisterSubquery(name.Name, subquery);
                int unionType = this.XreadUnionTypeRecursiveSubQueryConnector();
                this.compileContext.NoSetOperations = false;
                this.compileContext.SubqueryDepth++;
                if (unionType == 0)
                {
                    this.compileContext.SubqueryDepth--;
                    return subquery;
                }
                QueryExpression expression2 = this.XreadQueryExpression();
                if (resolve)
                {
                    if (this.compileContext.InRoutine || (this.session.sessionContext.SessionVariablesRange == null))
                    {
                        expression2.Resolve(this.session);
                    }
                    else
                    {
                        expression2.Resolve(this.session, this.session.sessionContext.SessionVariablesRange, null);
                    }
                }
                else
                {
                    expression2.ResolveReferences(this.session, RangeVariable.EmptyArray);
                }
                expression2.IsMergeable = false;
                QueryExpression expression3 = new QueryExpression(this.compileContext, queryExpression);
                expression3.AddUnion(expression2, unionType);
                if (resolve)
                {
                    if (this.compileContext.InRoutine || (this.session.sessionContext.SessionVariablesRange == null))
                    {
                        expression3.Resolve(this.session);
                    }
                    else
                    {
                        expression3.Resolve(this.session, this.session.sessionContext.SessionVariablesRange, null);
                    }
                }
                else
                {
                    expression3.ResolveReferences(this.session, RangeVariable.EmptyArray);
                }
                subquery.UnionType = unionType;
                subquery.RecursiveQueryExpression = expression2;
                subquery.AnchorQueryExpression = queryExpression;
                subquery.UnionDataTypes = subquery.AnchorQueryExpression.UnionColumnTypes = expression3.UnionColumnTypes;
                subquery.ExtraSubQueries = expression2.GetSubqueries();
                this.compileContext.SubqueryDepth--;
                subquery.PrepareTable2();
            }
            finally
            {
                this.compileContext.NoSetOperations = noSetOperations;
                this.compileContext.NoSetOperationsSubQueryDepth = noSetOperationsSubQueryDepth;
            }
            return subquery;
        }

        private void XreadTableReference(QuerySpecification select)
        {
            bool flag2;
            bool flag = false;
            RangeVariable rangeVar = this.ReadTableOrSubquery();
            select.AddRangeVariable(rangeVar);
        Label_0010:
            flag2 = false;
            bool isRight = false;
            bool flag4 = false;
            int tokenType = base.token.TokenType;
            int num2 = base.token.TokenType;
            if (num2 <= 0x8e)
            {
                if (num2 <= 0x73)
                {
                    if (num2 != 0x37)
                    {
                        if (num2 != 0x73)
                        {
                            goto Label_02FB;
                        }
                        base.Read();
                        base.ReadIfThis(0xc6);
                        base.ReadThis(0x8e);
                        flag2 = true;
                        isRight = true;
                    }
                    else
                    {
                        if (flag)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x8e);
                    }
                }
                else if (num2 != 130)
                {
                    if (num2 != 0x8e)
                    {
                        goto Label_02FB;
                    }
                    base.Read();
                    tokenType = 130;
                }
                else
                {
                    base.Read();
                    base.ReadThis(0x8e);
                }
            }
            else if (num2 <= 0xae)
            {
                if (num2 != 0x97)
                {
                    if (num2 != 0xae)
                    {
                        goto Label_02FB;
                    }
                    if (flag)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    flag = true;
                    goto Label_0010;
                }
                base.Read();
                base.ReadIfThis(0xc6);
                base.ReadThis(0x8e);
                flag2 = true;
            }
            else if (num2 != 0xee)
            {
                if (num2 != 0x128)
                {
                    if (num2 != 0x2ac)
                    {
                        goto Label_02FB;
                    }
                    if (flag)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    tokenType = 0x2ac;
                }
                else
                {
                    if (flag)
                    {
                        throw base.UnexpectedToken();
                    }
                    int position = base.GetPosition();
                    base.Read();
                    if (base.token.TokenType == 0x8e)
                    {
                        base.Read();
                    }
                    else
                    {
                        this.Rewind(position);
                        flag4 = true;
                    }
                }
            }
            else
            {
                base.Read();
                base.ReadIfThis(0xc6);
                base.ReadThis(0x8e);
                isRight = true;
            }
        Label_01B3:
            if (flag4)
            {
                return;
            }
            rangeVar = this.ReadTableOrSubquery();
            Expression e = null;
            int num3 = tokenType;
            if (num3 <= 130)
            {
                if (num3 == 0x37)
                {
                    select.AddRangeVariable(rangeVar);
                    goto Label_0245;
                }
                if ((num3 != 0x73) && (num3 != 130))
                {
                    goto Label_0245;
                }
                goto Label_0254;
            }
            if (num3 <= 0xee)
            {
                if ((num3 != 0x97) && (num3 != 0xee))
                {
                    goto Label_0245;
                }
                goto Label_0254;
            }
            switch (num3)
            {
                case 0x128:
                    select.AddRangeVariable(rangeVar);
                    e = new ExpressionLogical(false);
                    rangeVar.SetJoinType(true, true);
                    break;

                case 0x2ac:
                    rangeVar.IsBoundary = true;
                    select.AddRangeVariable(rangeVar);
                    break;
            }
        Label_0245:
            rangeVar.AddJoinCondition(e);
            flag = false;
            goto Label_0010;
        Label_0254:
            if (flag)
            {
                OrderedHashSet<string> uniqueColumnNameSet = rangeVar.GetUniqueColumnNameSet();
                e = select.GetEquiJoinExpressions(uniqueColumnNameSet, rangeVar, false);
                select.AddRangeVariable(rangeVar);
            }
            else if (base.token.TokenType == 0x130)
            {
                base.Read();
                OrderedHashSet<string> columns = new OrderedHashSet<string>();
                base.ReadThis(0x2b7);
                this.ReadSimpleColumnNames(columns, rangeVar);
                base.ReadThis(0x2aa);
                e = select.GetEquiJoinExpressions(columns, rangeVar, true);
                select.AddRangeVariable(rangeVar);
            }
            else
            {
                if (base.token.TokenType != 0xc0)
                {
                    throw base.UnexpectedToken();
                }
                base.Read();
                e = this.XreadBooleanValueExpression();
                select.AddRangeVariable(rangeVar);
            }
            rangeVar.SetJoinType(flag2, isRight);
            goto Label_0245;
        Label_02FB:
            if (flag)
            {
                throw base.UnexpectedToken();
            }
            flag4 = true;
            goto Label_01B3;
        }

        public SubQuery XreadTableSubqueryBody(bool resolve)
        {
            SubQuery query = this.XreadSubqueryBody(resolve, 0x17);
            if (resolve)
            {
                query.PrepareTable(this.session);
            }
            return query;
        }

        private Expression XreadTableSubqueryForPredicate(int mode)
        {
            base.ReadThis(0x2b7);
            SubQuery sq = this.XreadSubqueryBody(false, mode);
            base.ReadThis(0x2aa);
            return new Expression(0x17, sq);
        }

        private Expression XreadTableSubqueryOrJoinedTable()
        {
            bool flag = false;
            base.ReadThis(0x2b7);
            int position = base.GetPosition();
            this.ReadOpenBrackets();
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x114)
            {
                if ((tokenType != 0xf9) && (tokenType != 0x114))
                {
                    goto Label_0051;
                }
                goto Label_0053;
            }
            switch (tokenType)
            {
                case 0x132:
                case 0x13d:
                    goto Label_0053;
            }
        Label_0051:
            flag = true;
        Label_0053:
            this.Rewind(position);
            if (flag)
            {
                SubQuery query2 = this.XreadJoinedTableAsSubquery();
                base.ReadThis(0x2aa);
                return new Expression(0x17, query2);
            }
            SubQuery sq = this.XreadTableSubqueryBody(true);
            base.ReadThis(0x2aa);
            return new Expression(0x17, sq);
        }

        public Expression XreadTargetSpecification(RangeVariable[] rangeVars, LongDeque colIndexList)
        {
            ColumnSchema column = null;
            int i = -1;
            base.CheckIsIdentifier();
            if (base.token.NamePrePrePrefix != null)
            {
                this.CheckValidCatalogName(base.token.NamePrePrePrefix);
            }
            for (int j = 0; j < rangeVars.Length; j++)
            {
                if (rangeVars[j] != null)
                {
                    i = rangeVars[j].FindColumn(base.token.TokenString);
                    if (((i > -1) && rangeVars[j].ResolvesTableName(base.token.NamePrefix)) && rangeVars[j].ResolvesSchemaName(base.token.NamePrePrefix))
                    {
                        column = rangeVars[j].GetColumn(i);
                        base.Read();
                        break;
                    }
                }
            }
            if (column == null)
            {
                throw Error.GetError(0x157d, base.token.TokenString);
            }
            colIndexList.Add((long) i);
            if (base.token.TokenType != 820)
            {
                return column.GetAccessor();
            }
            if (!column.GetDataType().IsArrayType())
            {
                throw base.UnexpectedToken();
            }
            base.Read();
            Expression right = this.XreadNumericValueExpression();
            if (right == null)
            {
                throw Error.GetError(0x157d, base.token.TokenString);
            }
            right = new ExpressionAccessor(column.GetAccessor(), right);
            base.ReadThis(0x333);
            return right;
        }

        private Expression XreadTerm()
        {
            Expression right = this.XreadFactor();
            while (true)
            {
                int num;
                if (base.token.TokenType == 0x2a9)
                {
                    num = 0x22;
                }
                else
                {
                    if (base.token.TokenType != 0x2ae)
                    {
                        return right;
                    }
                    num = 0x23;
                }
                base.Read();
                Expression left = right;
                right = this.XreadFactor();
                if (right == null)
                {
                    throw base.UnexpectedToken();
                }
                right = new ExpressionArithmetic(num, left, right);
            }
        }

        protected SortAndSlice XreadTopOrLimit()
        {
            Expression left = null;
            Expression right = null;
            if (base.token.TokenType == 0x242)
            {
                int position = base.GetPosition();
                base.Read();
                left = this.XreadSimpleValueSpecificationOrNull();
                if (left == null)
                {
                    this.Rewind(position);
                    return null;
                }
                right = this.XreadSimpleValueSpecificationOrNull();
                if (right == null)
                {
                    throw Error.GetError(0x15bb, 0x51);
                }
            }
            else if (base.token.TokenType == 670)
            {
                int position = base.GetPosition();
                base.Read();
                right = this.XreadSimpleValueSpecificationOrNull();
                if (right == null)
                {
                    this.Rewind(position);
                    return null;
                }
                left = new ExpressionValue(0, SqlType.SqlInteger);
            }
            bool flag = true;
            if (left.IsUnresolvedParam())
            {
                left.SetDataType(this.session, SqlType.SqlInteger);
            }
            else
            {
                flag = (left.GetDataType().TypeCode == 4) && (Convert.ToUInt32(left.GetValue(null)) >= 0);
            }
            if (right.IsUnresolvedParam())
            {
                right.SetDataType(this.session, SqlType.SqlInteger);
            }
            else
            {
                flag &= (right.GetDataType().TypeCode == 4) && (Convert.ToUInt32(right.GetValue(null)) >= 0);
            }
            if (!flag)
            {
                throw Error.GetError(0x15bd, 0x51);
            }
            SortAndSlice slice1 = new SortAndSlice();
            slice1.AddLimitCondition(new ExpressionOp(0x5f, left, right));
            return slice1;
        }

        public void XreadUnionCorrespondingClause(QueryExpression queryExpression)
        {
            if (base.token.TokenType == 50)
            {
                base.Read();
                queryExpression.SetUnionCorresoponding();
                if (base.token.TokenType == 0x17)
                {
                    base.Read();
                    OrderedHashSet<string> names = this.ReadColumnNames(false);
                    queryExpression.SetUnionCorrespondingColumns(names);
                }
            }
        }

        public int XreadUnionType()
        {
            int num = 0;
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x88)
            {
                switch (tokenType)
                {
                    case 0x61:
                        goto Label_00AC;

                    case 0x88:
                        base.Read();
                        num = 3;
                        if (base.token.TokenType == 2)
                        {
                            num = 4;
                            base.Read();
                            return num;
                        }
                        if (base.token.TokenType == 0x54)
                        {
                            base.Read();
                            return num;
                        }
                        break;
                }
                return num;
            }
            if (tokenType != 0x128)
            {
                if (tokenType != 0x248)
                {
                    return num;
                }
            }
            else
            {
                base.Read();
                num = 1;
                if (base.token.TokenType == 2)
                {
                    num = 2;
                    base.Read();
                    return num;
                }
                if (base.token.TokenType == 0x54)
                {
                    base.Read();
                    return num;
                }
                return num;
            }
        Label_00AC:
            base.Read();
            num = 6;
            if (base.token.TokenType == 2)
            {
                num = 5;
                base.Read();
                return num;
            }
            if (base.token.TokenType == 0x54)
            {
                base.Read();
            }
            return num;
        }

        public int XreadUnionTypeRecursiveSubQueryConnector()
        {
            int num = 0;
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x88)
            {
                if ((tokenType != 0x61) && (tokenType != 0x88))
                {
                    return num;
                }
            }
            else
            {
                if (tokenType == 0x128)
                {
                    base.Read();
                    num = 1;
                    if (base.token.TokenType == 2)
                    {
                        num = 2;
                        base.Read();
                        return num;
                    }
                    if (base.token.TokenType == 0x54)
                    {
                        base.Read();
                        return num;
                    }
                    return num;
                }
                if (tokenType != 0x248)
                {
                    return num;
                }
            }
            base.UnexpectedToken(base.token.TokenString);
            return num;
        }

        private Expression XreadUnsignedValueSpecificationOrNull()
        {
            FunctionSQL nsql;
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x113)
            {
                switch (tokenType)
                {
                    case 0x3b:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x40:
                    case 0x43:
                    case 0x44:
                    case 0xfb:
                    case 0x113:
                        goto Label_01B1;

                    case 0x4d:
                        if (!this.compileContext.ContextuallyTypedExpression)
                        {
                            break;
                        }
                        base.Read();
                        return new ExpressionColumn(4);

                    case 0x69:
                        base.Read();
                        return new ExpressionLogical(false);

                    case 0xb8:
                        base.Read();
                        return new ExpressionValue(null, null);
                }
                goto Label_01D5;
            }
            if (tokenType <= 0x166)
            {
                switch (tokenType)
                {
                    case 0x12f:
                    case 0x131:
                        goto Label_01B1;

                    case 0x124:
                        base.Read();
                        return new ExpressionLogical(true);

                    case 0x166:
                        return XreadCurrentCollationSpec();
                }
                goto Label_01D5;
            }
            switch (tokenType)
            {
                case 0x2b9:
                {
                    Expression e = new ExpressionColumn(8);
                    this.compileContext.AddParameter(e, base.GetPosition());
                    base.Read();
                    return e;
                }
                case 0x2e9:
                    base.Read();
                    return new ExpressionValue(base.token.TokenValue, base.token.DataType);

                default:
                    if ((tokenType - 0x2ea) <= 1)
                    {
                        if (!base.token.IsHostParameter || this.IsNonHostParameter(base.token.TokenString))
                        {
                            return null;
                        }
                        Expression e = new ExpressionColumn(0x62, base.token.TokenString);
                        this.compileContext.AddParameter(e, base.GetPosition());
                        base.Read();
                        return e;
                    }
                    if (tokenType != 0x33a)
                    {
                        goto Label_01D5;
                    }
                    break;
            }
        Label_01B1:
            nsql = FunctionSQL.NewSqlFunction(base.token.TokenString, this.compileContext);
            if (nsql == null)
            {
                return null;
            }
            return this.ReadSQLFunction(nsql);
        Label_01D5:
            return null;
        }

        public Expression XreadValueExpression()
        {
            Expression left = this.XreadAllTypesCommonValueExpression(true);
            if (base.token.TokenType == 820)
            {
                base.Read();
                Expression right = this.XreadNumericValueExpression();
                base.ReadThis(0x333);
                left = new ExpressionAccessor(left, right);
            }
            return left;
        }

        public Expression XreadValueExpressionOrNull()
        {
            Expression expression = this.XreadAllTypesCommonValueExpression(true);
            if (expression == null)
            {
                return null;
            }
            return expression;
        }

        private Expression XreadValueExpressionPrimary()
        {
            Expression e = this.XreadSimpleValueExpressionPrimary();
            if (e != null)
            {
                return this.XreadArrayElementReference(e);
            }
            if (base.token.TokenType == 0x2b7)
            {
                base.Read();
                e = this.XreadValueExpression();
                e.NoBreak = true;
                base.ReadThis(0x2aa);
                return e;
            }
            return null;
        }

        public Expression XreadValueExpressionWithContext()
        {
            this.compileContext.ContextuallyTypedExpression = true;
            this.compileContext.ContextuallyTypedExpression = false;
            return this.XreadValueExpressionOrNull();
        }

        public Expression XreadValueSpecificationOrNull()
        {
            Expression e = null;
            bool flag = false;
            switch (base.token.TokenType)
            {
                case 0x2b5:
                    base.Read();
                    flag = true;
                    break;

                case 0x2b8:
                    base.Read();
                    break;
            }
            e = this.XreadUnsignedValueSpecificationOrNull();
            if (e == null)
            {
                return null;
            }
            if (flag)
            {
                e = new ExpressionArithmetic(0x1f, e);
            }
            return e;
        }

        public SubQuery XreadViewSubquery(View view)
        {
            SubQuery query;
            this.compileContext.SubqueryDepth++;
            try
            {
                QueryExpression expression;
                try
                {
                    expression = this.XreadQueryExpression();
                }
                catch (CoreException)
                {
                    expression = this.XreadJoinedTable();
                }
                expression.SetView(view);
                expression.Resolve(this.session);
                query = new SubQuery(this.database, this.compileContext.SubqueryDepth, expression, view);
            }
            finally
            {
                this.compileContext.SubqueryDepth--;
            }
            return query;
        }

        public class CompileContext
        {
            private readonly List<RangeVariable> _rangeVariables = new List<RangeVariable>();
            private readonly List<ISchemaObject> _usedObjects = new List<ISchemaObject>();
            private readonly List<FunctionSQLInvoked> _usedRoutines = new List<FunctionSQLInvoked>();
            private readonly List<NumberSequence> _usedSequences = new List<NumberSequence>();
            private OrderedIntKeyHashMap<string> _noneHostParameters = new OrderedIntKeyHashMap<string>();
            public bool InRoutine;
            public bool NoSetOperations;
            public int NoSetOperationsSubQueryDepth = -1;
            private int _rangeVarIndex;
            public Routine CallProcedure;
            public bool ContextuallyTypedExpression;
            public SqlType CurrentDomain;
            private List<HashMappedList<string, SubQuery>> _namedSubqueries;
            public OrderedIntKeyHashMap<Expression> Parameters = new OrderedIntKeyHashMap<Expression>();
            public ParserBase Parser;
            public Session session;
            public int SubqueryDepth;

            public CompileContext(Session session, ParserBase parser)
            {
                this.session = session;
                this.Parser = parser;
                this.Reset();
            }

            public void AddFunctionCall(FunctionSQLInvoked function)
            {
                this._usedRoutines.Add(function);
            }

            public void AddNoneHostParameter(string name, int position)
            {
                this._noneHostParameters.Put(position, name);
            }

            public void AddParameter(Expression e, int position)
            {
                this.Parameters.Put(position, e);
            }

            public void AddProcedureCall(Routine procedure)
            {
                this.CallProcedure = procedure;
            }

            public void AddSchemaObject(ISchemaObject obj)
            {
                this._usedObjects.Add(obj);
            }

            public void AddSequence(NumberSequence sequence)
            {
                this._usedSequences.Add(sequence);
            }

            public void ClearNoneHostParameters()
            {
                this._noneHostParameters.Clear();
            }

            public void ClearParameters()
            {
                this.Parameters.Clear();
            }

            public SubQuery GetNamedSubQuery(string name)
            {
                if (this._namedSubqueries != null)
                {
                    for (int i = this.SubqueryDepth; i >= 0; i--)
                    {
                        if (this._namedSubqueries.Count > i)
                        {
                            HashMappedList<string, SubQuery> list = this._namedSubqueries[i];
                            if (list != null)
                            {
                                SubQuery query = list.Get(name);
                                if (query != null)
                                {
                                    return query;
                                }
                            }
                        }
                    }
                }
                return null;
            }

            public int GetNextRangeVarIndex()
            {
                int num = this._rangeVarIndex;
                this._rangeVarIndex = num + 1;
                return num;
            }

            public ExpressionColumn[] GetParameters()
            {
                if (this.Parameters.Size() == 0)
                {
                    return ExpressionColumn.EmptyArray;
                }
                ExpressionColumn[] array = new ExpressionColumn[this.Parameters.Size()];
                this.Parameters.ValuesToArray(array);
                this.Parameters.Clear();
                return array;
            }

            public int GetRangeVarCount()
            {
                return this._rangeVarIndex;
            }

            public RangeVariable[] GetRangeVariables()
            {
                return this._rangeVariables.ToArray();
            }

            public Routine[] GetRoutines()
            {
                if ((this.CallProcedure == null) && (this._usedRoutines.Count == 0))
                {
                    return Routine.EmptyArray;
                }
                List<Routine> list = new List<Routine>();
                for (int i = 0; i < this._usedRoutines.Count; i++)
                {
                    FunctionSQLInvoked invoked = this._usedRoutines[i];
                    list.Add(invoked.routine);
                }
                if (this.CallProcedure != null)
                {
                    list.Add(this.CallProcedure);
                }
                return list.ToArray();
            }

            public OrderedHashSet<QNameManager.QName> GetSchemaObjectNames()
            {
                OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
                for (int i = 0; i < this._usedSequences.Count; i++)
                {
                    set.Add(this._usedSequences[i].GetName());
                }
                for (int j = 0; j < this._usedObjects.Count; j++)
                {
                    set.Add(this._usedObjects[j].GetName());
                }
                for (int k = 0; k < this._rangeVariables.Count; k++)
                {
                    RangeVariable variable = this._rangeVariables[k];
                    QNameManager.QName name = variable.RangeTable.GetName();
                    if (name.schema != SqlInvariants.SystemSchemaQname)
                    {
                        set.Add(variable.RangeTable.GetName());
                        set.AddAll(variable.GetColumnNames());
                    }
                    else if (name.type == 10)
                    {
                        set.AddAll(variable.GetColumnNames());
                    }
                }
                Routine[] routines = this.GetRoutines();
                for (int m = 0; m < routines.Length; m++)
                {
                    set.Add(routines[m].GetSpecificName());
                }
                return set;
            }

            public NumberSequence[] GetSequences()
            {
                if (this._usedSequences.Count == 0)
                {
                    return NumberSequence.EmptyArray;
                }
                return this._usedSequences.ToArray();
            }

            public void InitSubqueryNames()
            {
                if (this._namedSubqueries == null)
                {
                    this._namedSubqueries = new List<HashMappedList<string, SubQuery>>();
                }
                if (this._namedSubqueries.Count <= this.SubqueryDepth)
                {
                    for (int i = this._namedSubqueries.Count; i < (this.SubqueryDepth + 1); i++)
                    {
                        this._namedSubqueries.Add(null);
                    }
                }
                HashMappedList<string, SubQuery> list = this._namedSubqueries[this.SubqueryDepth];
                if (list == null)
                {
                    list = new HashMappedList<string, SubQuery>();
                    this._namedSubqueries[this.SubqueryDepth] = list;
                }
                else
                {
                    list.Clear();
                }
            }

            public bool IsNoneHostParameter(string name)
            {
                return this._noneHostParameters.ContainsValue(name);
            }

            public void RegisterRangeVariable(RangeVariable range)
            {
                range.ParsePosition = (this.Parser == null) ? 0 : this.Parser.GetPosition();
                range.RangePosition = this.GetNextRangeVarIndex();
                range.Level = this.SubqueryDepth;
                this._rangeVariables.Add(range);
            }

            public void RegisterSubquery(string name, SubQuery subquery)
            {
                if (!this._namedSubqueries[this.SubqueryDepth].Add(name, subquery))
                {
                    throw Error.GetError(0x1580);
                }
            }

            public void Reset()
            {
                this.Reset(1);
            }

            public void Reset(int n)
            {
                this._rangeVarIndex = n;
                this._rangeVariables.Clear();
                this.SubqueryDepth = 0;
                this.NoSetOperations = false;
                this.NoSetOperationsSubQueryDepth = -1;
                this.Parameters.Clear();
                this._usedSequences.Clear();
                this._usedRoutines.Clear();
                this.CallProcedure = null;
                this._usedObjects.Clear();
                if (this._namedSubqueries != null)
                {
                    this._namedSubqueries.Clear();
                }
                this.CurrentDomain = null;
                this.ContextuallyTypedExpression = false;
            }

            public void Rewind(int position)
            {
                for (int i = this._rangeVariables.Count - 1; i >= 0; i--)
                {
                    if (this._rangeVariables[i].ParsePosition > position)
                    {
                        this._rangeVariables.RemoveAt(i);
                    }
                }
                Iterator<int> iterator = this.Parameters.GetKeySet().GetIterator();
                while (iterator.HasNext())
                {
                    if (iterator.Next() >= position)
                    {
                        iterator.Remove();
                    }
                }
                iterator = this._noneHostParameters.GetKeySet().GetIterator();
                while (iterator.HasNext())
                {
                    if (iterator.Next() >= position)
                    {
                        iterator.Remove();
                    }
                }
            }

            public void UnRegisterSubquery(string name)
            {
                this._namedSubqueries[this.SubqueryDepth].Remove(name);
            }
        }
    }
}


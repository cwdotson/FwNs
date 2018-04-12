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
    using System.Collections.Generic;

    public class ParserRoutine : ParserDML
    {
        public ParserRoutine(Session session, Scanner t) : base(session, t)
        {
        }

        public StatementSchema CompileAlterSpecificRoutine()
        {
            base.ReadThis(0x101);
            base.ReadThis(490);
            Routine routine = (Routine) base.ReadSchemaObjectName(0x18);
            routine = routine.Duplicate();
            if (routine.Language == 1)
            {
                base.ReadThis(0x1bc);
                base.CheckIsValue(12);
                routine.SetMethodFqn((string) base.token.TokenValue);
                base.Read();
            }
            else
            {
                base.ReadThis(0xfc);
                base.ReadThis(0x33c);
                this.ReadRoutineBody(routine);
            }
            object[] args = new object[] { routine };
            return new StatementSchema(base.GetLastPart(), 0x11, args);
        }

        public Statement CompileAnnonymousBlock(RangeVariable[] outerRanges)
        {
            Statement statement;
            base.compileContext.ClearNoneHostParameters();
            base.compileContext.InRoutine = true;
            try
            {
                Routine routine = new Routine(0x11) {
                    IsAnnonymous = true
                };
                routine.SetLanguage(2);
                routine.SetDataImpact(4);
                Expression[] arguments = new Expression[base.session.sessionContext.SessionVariables.Size()];
                Iterator<ColumnSchema> iterator = base.session.sessionContext.SessionVariables.GetValues().GetIterator();
                int index = 0;
                List<Expression> unresolvedSet = null;
                while (iterator.HasNext())
                {
                    ColumnSchema column = iterator.Next();
                    arguments[index] = new ExpressionColumn(column);
                    ColumnSchema param = new ColumnSchema(new QNameManager.QName(base.session.database.NameManager, column.GetName().Name, column.GetName().IsNameQuoted, 0x17), column.GetDataType(), true, false, null);
                    param.SetParameterMode(column.GetParameterMode());
                    routine.AddParameter(param);
                    unresolvedSet = arguments[index].ResolveColumnReferences(outerRanges, unresolvedSet);
                    arguments[index].ResolveTypes(base.session, null);
                    index++;
                }
                ExpressionColumn.CheckColumnsResolved(unresolvedSet);
                base.StartRecording();
                Statement statement2 = this.CompileSQLProcedureStatementOrNull(routine, null);
                if (statement2 == null)
                {
                    throw base.UnexpectedToken();
                }
                string sql = Token.GetSql(base.GetRecordedStatement());
                statement2.SetSql(sql);
                base.GetLastPart();
                routine.SetProcedure(statement2);
                routine.Resolve(base.session);
                base.compileContext.AddProcedureCall(routine);
                StatementProcedure procedure1 = new StatementProcedure(base.session, routine, arguments);
                procedure1.SetDatabseObjects(base.session, base.compileContext);
                procedure1.CheckAccessRights(base.session);
                statement = procedure1;
            }
            finally
            {
                base.compileContext.ClearNoneHostParameters();
                base.compileContext.InRoutine = false;
            }
            return statement;
        }

        private Statement CompileCase(Routine routine, StatementCompound context)
        {
            List<Statement> list;
            Statement[] statementArray;
            base.ReadThis(0x1c);
            if (base.token.TokenType == 0x138)
            {
                list = this.ReadCaseWhen(routine, context);
            }
            else
            {
                list = this.ReadSimpleCaseWhen(routine, context);
            }
            if (base.token.TokenType == 0x5b)
            {
                base.Read();
                Expression expression = new ExpressionLogical(true);
                StatementExpression item = new StatementExpression(base.session, 0x44d, expression);
                item.SetDatabseObjects(base.session, base.compileContext);
                item.CheckAccessRights(base.session);
                list.Add(item);
                statementArray = this.CompileSqlProcedureStatementList(routine, context);
                for (int i = 0; i < statementArray.Length; i++)
                {
                    list.Add(statementArray[i]);
                }
            }
            base.ReadThis(0x5d);
            base.ReadThis(0x1c);
            statementArray = list.ToArray();
            StatementCompound compound1 = new StatementCompound(0x58, null);
            compound1.SetRoot(routine);
            compound1.SetParent(context);
            compound1.SetLocalDeclarations(new object[0]);
            compound1.SetStatements(statementArray);
            return compound1;
        }

        private Statement CompileCloseCursorStatement(RangeVariable[] rangeVariables)
        {
            base.Read();
            QNameManager.QName label = base.ReadNewSchemaObjectName(0x13, true);
            return new StatementSimple(9, label, label.Name);
        }

        private Statement CompileCompoundStatement(Routine routine, StatementCompound context, QNameManager.QName label)
        {
            bool atomic = true;
            base.ReadThis(0x10);
            base.ReadIfThis(13);
            StatementCompound compound = new StatementCompound(12, label);
            compound.SetAtomic(atomic);
            compound.SetRoot(routine);
            compound.SetParent(context);
            object[] declarations = this.ReadLocalDeclarationList(routine, context);
            compound.SetLocalDeclarations(declarations);
            base.session.sessionContext.PushRoutineTables(compound.ScopeTables);
            try
            {
                Statement[] statements = this.CompileSqlProcedureStatementList(routine, compound);
                compound.SetStatements(statements);
            }
            finally
            {
                base.session.sessionContext.PopRoutineTables();
            }
            base.ReadThis(0x5d);
            if (base.IsSimpleName() && !base.IsReservedKey())
            {
                if (label == null)
                {
                    throw base.UnexpectedToken();
                }
                if (!label.Name.Equals(base.token.TokenString))
                {
                    throw Error.GetError(0x1584, base.token.TokenString);
                }
                base.Read();
            }
            return compound;
        }

        private Condition CompileConditionDeclarationOrNull(Routine routine, StatementCompound context)
        {
            Condition condition2;
            int position = base.GetPosition();
            base.ReadThis(0x4c);
            QNameManager.QName name = base.ReadNewSchemaObjectName(0x1a, true);
            if (base.token.TokenType != 0x2d)
            {
                this.Rewind(position);
                return null;
            }
            base.Read();
            base.ReadThis(0x6f);
            if (base.ReadIfThis(0x105))
            {
                string sqlState = this.ParseSqlStateValue();
                condition2 = new Condition(name, sqlState);
            }
            else
            {
                Expression errNo = base.XreadValueExpression();
                condition2 = new Condition(name, errNo);
            }
            base.ReadIfThis(0x2bb);
            return condition2;
        }

        public virtual StatementSchema CompileCreate()
        {
            return null;
        }

        public StatementSchema CompileCreateProcedureOrFunction()
        {
            StatementSchema schema;
            bool isAggregate = false;
            base.compileContext.ClearNoneHostParameters();
            base.compileContext.InRoutine = true;
            try
            {
                ColumnSchema schema2;
                if (base.token.TokenType == 0x332)
                {
                    isAggregate = true;
                    base.Read();
                    if (base.token.TokenType == 0xd5)
                    {
                        throw base.UnexpectedToken();
                    }
                }
                int num = (base.token.TokenType == 0xd5) ? 0x11 : 0x10;
                if (!isAggregate)
                {
                    base.Read();
                }
                else
                {
                    base.ReadIfThis(0x74);
                }
                QNameManager.QName name = base.ReadNewSchemaObjectName(num, false);
                Routine routine = new Routine(num);
                routine.SetName(name);
                routine.SetAggregate(isAggregate);
                base.ReadThis(0x2b7);
                if (base.token.TokenType == 0x2aa)
                {
                    base.Read();
                    goto Label_0106;
                }
            Label_00BF:
                schema2 = this.ReadRoutineParameter(routine, true);
                routine.AddParameter(schema2);
                if (base.token.TokenType == 0x2ac)
                {
                    base.Read();
                    goto Label_00BF;
                }
                base.ReadThis(0x2aa);
                this.SetRoutineTables(base.database, routine);
            Label_0106:
                if (num != 0x11)
                {
                    base.ReadThis(0xec);
                    if (base.token.TokenType == 0x114)
                    {
                        base.Read();
                        TableDerived table = new TableDerived(base.database, SqlInvariants.ModuleQname, 11);
                        this.ReadTableDefinition(routine, table, false);
                        routine.SetReturnTable(table);
                    }
                    else
                    {
                        SqlType type = base.ReadTypeDefinition(true, false);
                        routine.SetReturnType(type);
                    }
                }
                this.ReadRoutineCharacteristics(routine);
                this.ReadRoutineBody(routine);
                object[] args = new object[] { routine };
                schema = new StatementSchema(base.GetLastPart(), 14, args);
            }
            finally
            {
                base.compileContext.ClearNoneHostParameters();
                base.compileContext.InRoutine = false;
            }
            return schema;
        }

        public virtual Statement CompileDeallocatePrepare()
        {
            return null;
        }

        public virtual Statement CompileExecute(RangeVariable[] rangeVariables)
        {
            return null;
        }

        private Statement CompileFetchCursorStatement(RangeVariable[] rangeVariables)
        {
            base.Read();
            QNameManager.QName name = base.ReadNewSchemaObjectName(0x13, true);
            OrderedHashSet<string> colNames = new OrderedHashSet<string>();
            colNames.Add(name.Name);
            int[] indexes = new int[colNames.Size()];
            ColumnSchema[] variables = new ColumnSchema[colNames.Size()];
            StatementSet.SetVariables(rangeVariables, colNames, indexes, variables);
            if (indexes[0] < 0)
            {
                throw Error.GetError(0xe10);
            }
            if (variables[0] == null)
            {
                throw Error.GetError(0xe10);
            }
            ExpressionColumnAccessor cursorAccessor = new ExpressionColumnAccessor(variables[0]);
            base.ReadThis(0x8b);
            OrderedHashSet<Expression> targets = new OrderedHashSet<Expression>();
            LongDeque colIndexList = new LongDeque();
            base.ReadTargetSpecificationList(targets, rangeVariables, colIndexList);
            int[] array = new int[colIndexList.Size()];
            colIndexList.ToArray(array);
            Expression[] a = new Expression[targets.Size()];
            targets.ToArray(a);
            StatementSet set1 = new StatementSet(base.session, a, array, cursorAccessor);
            set1.SetDatabseObjects(base.session, base.compileContext);
            set1.CheckAccessRights(base.session);
            return set1;
        }

        private Statement CompileFor(Routine routine, StatementCompound context, QNameManager.QName label)
        {
            RangeVariable[] outerRanges = (context == null) ? routine.GetParameterRangeVariables() : context.GetRangeVariables();
            base.ReadThis(0x6f);
            StatementQuery cursorStatement = base.CompileCursorSpecification(ResultProperties.DefaultPropsValue, false, outerRanges);
            base.ReadThis(0x55);
            StatementCompound compound = new StatementCompound(0x2e, label);
            compound.SetAtomic(true);
            compound.SetRoot(routine);
            compound.SetParent(context);
            compound.SetLoopStatement(cursorStatement);
            Statement[] statements = this.CompileSqlProcedureStatementList(routine, compound);
            base.ReadThis(0x5d);
            base.ReadThis(0x6f);
            if (base.IsSimpleName() && !base.IsReservedKey())
            {
                if (label == null)
                {
                    throw base.UnexpectedToken();
                }
                if (!label.Name.Equals(base.token.TokenString))
                {
                    throw Error.GetError(0x1584, base.token.TokenString);
                }
                base.Read();
            }
            compound.SetStatements(statements);
            return compound;
        }

        protected Statement CompileIf(Routine routine, StatementCompound context, bool external)
        {
            Statement[] statementArray;
            List<Statement> list = new List<Statement>();
            base.ReadThis(0x19c);
            Expression e = base.XreadBooleanValueExpression();
            this.ResolveOuterReferencesAndTypes(routine, context, e);
            StatementExpression item = new StatementExpression(base.session, 0x44d, e);
            item.SetDatabseObjects(base.session, base.compileContext);
            item.CheckAccessRights(base.session);
            list.Add(item);
            bool flag = false;
            if (base.token.TokenType == 0x116)
            {
                flag = true;
                base.Read();
            }
            if (external)
            {
                Statement statement = this.CompileSQLProcedureStatementOrNull(routine, context);
                list.Add(statement);
            }
            else
            {
                statementArray = this.CompileSqlProcedureStatementList(routine, context);
                for (int i = 0; i < statementArray.Length; i++)
                {
                    list.Add(statementArray[i]);
                }
            }
            while (base.token.TokenType == 0x5c)
            {
                base.Read();
                e = base.XreadBooleanValueExpression();
                this.ResolveOuterReferencesAndTypes(routine, context, e);
                item = new StatementExpression(base.session, 0x44d, e);
                item.SetDatabseObjects(base.session, base.compileContext);
                item.CheckAccessRights(base.session);
                list.Add(item);
                base.ReadThis(0x116);
                statementArray = this.CompileSqlProcedureStatementList(routine, context);
                for (int i = 0; i < statementArray.Length; i++)
                {
                    list.Add(statementArray[i]);
                }
            }
            if (base.token.TokenType == 0x5b)
            {
                base.Read();
                e = new ExpressionLogical(true);
                item = new StatementExpression(base.session, 0x44d, e);
                item.SetDatabseObjects(base.session, base.compileContext);
                item.CheckAccessRights(base.session);
                list.Add(item);
                statementArray = this.CompileSqlProcedureStatementList(routine, context);
                for (int i = 0; i < statementArray.Length; i++)
                {
                    list.Add(statementArray[i]);
                }
            }
            if (flag)
            {
                base.ReadThis(0x5d);
                base.ReadThis(0x19c);
            }
            statementArray = list.ToArray();
            StatementCompound compound1 = new StatementCompound(0x58, null);
            compound1.SetRoot(routine);
            compound1.SetParent(context);
            compound1.SetLocalDeclarations(new object[0]);
            compound1.SetStatements(statementArray);
            return compound1;
        }

        private Statement CompileIterate()
        {
            base.ReadThis(0x8d);
            return new StatementSimple(0x66, base.ReadNewSchemaObjectName(0x15, false));
        }

        private Statement CompileLeave(Routine routine, StatementCompound context)
        {
            base.ReadThis(150);
            return new StatementSimple(0x59, base.ReadNewSchemaObjectName(0x15, false));
        }

        private StatementHandler CompileLocalHandlerDeclarationOrNull(Routine routine, StatementCompound context)
        {
            int num3;
            int position = base.GetPosition();
            base.ReadThis(0x4c);
            int tokenType = base.token.TokenType;
            switch (tokenType)
            {
                case 0x65:
                    base.Read();
                    num3 = 6;
                    break;

                case 0x127:
                    base.Read();
                    num3 = 7;
                    break;

                default:
                    if (tokenType != 0x178)
                    {
                        this.Rewind(position);
                        return null;
                    }
                    base.Read();
                    num3 = 5;
                    break;
            }
            base.ReadThis(0x7b);
            base.ReadThis(0x6f);
            StatementHandler handler = new StatementHandler(num3);
            bool flag = false;
            bool flag2 = true;
            while (!flag)
            {
                int conditionType = 0;
                int num5 = base.token.TokenType;
                if (num5 <= 0x106)
                {
                    switch (num5)
                    {
                        case 260:
                            goto Label_01D7;

                        case 0x105:
                            conditionType = 4;
                            goto Label_01D7;

                        case 0x106:
                            goto Label_01B8;

                        case 0xb5:
                            goto Label_012D;
                    }
                    goto Label_01C7;
                }
                switch (num5)
                {
                    case 0x2ac:
                    {
                        if (flag2)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        flag2 = true;
                        continue;
                    }
                    case 0x2e9:
                        if (base.token.DataType != SqlType.SqlInteger)
                        {
                            throw base.UnexpectedToken();
                        }
                        conditionType = 5;
                        goto Label_01D7;

                    default:
                        if (((num5 - 0x2ea) > 1) || !flag2)
                        {
                            goto Label_01C7;
                        }
                        conditionType = 6;
                        goto Label_01D7;
                }
            Label_012D:
                if (conditionType == 0)
                {
                    conditionType = 3;
                }
                if (!flag2)
                {
                    throw base.UnexpectedToken();
                }
                flag2 = false;
                switch (conditionType)
                {
                    case 3:
                        base.Read();
                        base.ReadThis(0x194);
                        break;

                    case 4:
                    {
                        base.Read();
                        string sqlState = this.ParseSqlStateValue();
                        handler.AddConditionState(sqlState);
                        continue;
                    }
                    case 5:
                    {
                        int errorCode = base.ReadInteger();
                        handler.AddConditionErrorCode(errorCode);
                        continue;
                    }
                    case 6:
                    {
                        QNameManager.QName conditionName = base.ReadNewSchemaObjectName(0x1a, false);
                        handler.AddConditionName(conditionName);
                        continue;
                    }
                    default:
                        base.Read();
                        break;
                }
                handler.AddConditionType(conditionType);
                continue;
            Label_01B8:
                if (conditionType == 0)
                {
                    conditionType = 2;
                }
                goto Label_012D;
            Label_01C7:
                if (flag2)
                {
                    throw base.UnexpectedToken();
                }
                flag = true;
                continue;
            Label_01D7:
                if (conditionType == 0)
                {
                    conditionType = 1;
                }
                goto Label_01B8;
            }
            if (base.token.TokenType == 0x2bb)
            {
                base.Read();
                return handler;
            }
            Statement s = this.CompileSQLProcedureStatementOrNull(routine, context);
            if (s == null)
            {
                throw base.UnexpectedToken();
            }
            base.ReadThis(0x2bb);
            handler.AddStatement(s);
            return handler;
        }

        private Statement CompileLoop(Routine routine, StatementCompound context, QNameManager.QName label)
        {
            base.ReadThis(0x9e);
            Statement[] statements = this.CompileSqlProcedureStatementList(routine, context);
            base.ReadThis(0x5d);
            base.ReadThis(0x9e);
            if (base.IsSimpleName() && !base.IsReservedKey())
            {
                if (label == null)
                {
                    throw base.UnexpectedToken();
                }
                if (!label.Name.Equals(base.token.TokenString))
                {
                    throw Error.GetError(0x1584, base.token.TokenString);
                }
                base.Read();
            }
            StatementCompound compound1 = new StatementCompound(90, label);
            compound1.SetRoot(routine);
            compound1.SetParent(context);
            compound1.SetLocalDeclarations(new object[0]);
            compound1.SetStatements(statements);
            return compound1;
        }

        private Statement CompileOpenCursorStatement(RangeVariable[] rangeVariables)
        {
            base.Read();
            QNameManager.QName label = base.ReadNewSchemaObjectName(0x13, true);
            return new StatementSimple(0x35, label, label.Name);
        }

        public virtual Statement CompilePrepare(RangeVariable[] rangeVars)
        {
            return null;
        }

        private Statement CompileRepeat(Routine routine, StatementCompound context, QNameManager.QName label)
        {
            base.ReadThis(0xe8);
            Statement[] statements = this.CompileSqlProcedureStatementList(routine, context);
            base.ReadThis(300);
            Expression e = base.XreadBooleanValueExpression();
            this.ResolveOuterReferencesAndTypes(routine, context, e);
            StatementExpression condition = new StatementExpression(base.session, 0x44d, e);
            condition.SetDatabseObjects(base.session, base.compileContext);
            condition.CheckAccessRights(base.session);
            base.ReadThis(0x5d);
            base.ReadThis(0xe8);
            if (base.IsSimpleName() && !base.IsReservedKey())
            {
                if (label == null)
                {
                    throw base.UnexpectedToken();
                }
                if (!label.Name.Equals(base.token.TokenString))
                {
                    throw Error.GetError(0x1584, base.token.TokenString);
                }
                base.Read();
            }
            StatementCompound compound1 = new StatementCompound(0x5f, label);
            compound1.SetRoot(routine);
            compound1.SetParent(context);
            compound1.SetLocalDeclarations(new object[0]);
            compound1.SetStatements(statements);
            compound1.SetCondition(condition);
            return compound1;
        }

        private Statement CompileResignal(Routine routine, StatementCompound context, QNameManager.QName label)
        {
            string sqlState = null;
            base.ReadThis(0xe9);
            if (base.ReadIfThis(0x105))
            {
                sqlState = this.ParseSqlStateValue();
            }
            Dictionary<int, Expression> conditionInformationItems = null;
            if (base.token.TokenType == 0xfc)
            {
                base.Read();
                conditionInformationItems = this.ReadConditionInformationItems();
                if (conditionInformationItems.Count == 0)
                {
                    base.UnexpectedToken("SET");
                }
            }
            return new StatementSimple(0x5b, sqlState, conditionInformationItems);
        }

        private Statement CompileReturnValue(Routine routine, StatementCompound context)
        {
            Expression e = base.XreadValueExpressionOrNull();
            if (e == null)
            {
                base.CheckIsValue();
                if (base.token.TokenValue == null)
                {
                    e = new ExpressionValue(null, null);
                }
            }
            this.ResolveOuterReferencesAndTypes(routine, context, e);
            if (routine.IsProcedure())
            {
                throw Error.GetError(0x15e2);
            }
            StatementExpression expression1 = new StatementExpression(base.session, 0x3a, e, routine.GetReturnType());
            expression1.SetDatabseObjects(base.session, base.compileContext);
            expression1.CheckAccessRights(base.session);
            return expression1;
        }

        public Statement CompileSelectSingleRowStatement(RangeVariable[] rangeVars, QuerySpecification select)
        {
            OrderedHashSet<Expression> targets = new OrderedHashSet<Expression>();
            LongDeque colIndexList = new LongDeque();
            base.ReadThis(0x8b);
            base.ReadTargetSpecificationList(targets, rangeVars, colIndexList);
            base.XreadTableExpression(select);
            if (base.token.TokenType == 0x242)
            {
                base.token.TokenType = 670;
                SortAndSlice sortAndSlice = base.XreadTopOrLimit();
                if (sortAndSlice != null)
                {
                    select.AddSortAndSlice(sortAndSlice);
                }
            }
            select.SetReturningResult();
            int[] array = new int[colIndexList.Size()];
            colIndexList.ToArray(array);
            Expression[] a = new Expression[targets.Size()];
            targets.ToArray(a);
            SqlType[] targetTypes = new SqlType[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                a[i].GetColumn().GetParameterMode();
                targetTypes[i] = a[i].GetDataType();
            }
            select.Resolve(base.session, rangeVars, targetTypes);
            if (select.GetColumnCount() != a.Length)
            {
                throw Error.GetError(0x15bc, "INTO");
            }
            StatementSet set1 = new StatementSet(base.session, a, select, array);
            set1.SetDatabseObjects(base.session, base.compileContext);
            set1.CheckAccessRights(base.session);
            return set1;
        }

        public Statement CompileSelectStatement(RangeVariable[] rangeVars, Routine routine)
        {
            int position = base.GetPosition();
            QuerySpecification select = base.XreadSelect();
            if (base.token.TokenType == 0x8b)
            {
                return this.CompileSelectSingleRowStatement(rangeVars, select);
            }
            this.Rewind(position);
            if (routine.RoutineType != 0x11)
            {
                throw Error.GetError(0x15e2, "SELECT");
            }
            return base.CompileCursorSpecification(ResultProperties.DefaultPropsValue, false, rangeVars);
        }

        public Statement CompileSetStatement(RangeVariable[] rangeVars, bool isTrigger)
        {
            base.Read();
            OrderedHashSet<Expression> targets = new OrderedHashSet<Expression>();
            List<Expression> expressions = new List<Expression>();
            LongDeque colIndexList = new LongDeque();
            base.ReadSetClauseList(rangeVars, targets, colIndexList, expressions);
            if (expressions.Count > 1)
            {
                throw Error.GetError(0x15e2);
            }
            Expression e = expressions[0];
            if (e.GetDegree() != targets.Size())
            {
                throw Error.GetError(0x15aa, "SET");
            }
            int[] array = new int[colIndexList.Size()];
            colIndexList.ToArray(array);
            Expression[] a = new Expression[targets.Size()];
            targets.ToArray(a);
            RangeVariable[] destinationArray = rangeVars;
            if (isTrigger)
            {
                destinationArray = new RangeVariable[rangeVars.Length];
                Array.Copy(rangeVars, destinationArray, rangeVars.Length);
                destinationArray[1] = null;
                destinationArray[3] = null;
                destinationArray[0] = null;
                destinationArray[2] = null;
            }
            for (int i = 0; i < a.Length; i++)
            {
                base.ResolveOuterReferencesAndTypes(destinationArray, a[i]);
            }
            base.ResolveOuterReferencesAndTypes(rangeVars, e);
            for (int j = 0; j < a.Length; j++)
            {
                a[j].GetColumn().GetParameterMode();
                if (!a[j].GetDataType().CanBeAssignedFrom(e.GetNodeDataType(j)))
                {
                    throw Error.GetError(0x15b9);
                }
            }
            StatementSet set1 = new StatementSet(base.session, a, e, array);
            set1.SetDatabseObjects(base.session, base.compileContext);
            set1.CheckAccessRights(base.session);
            return set1;
        }

        private Statement CompileSignal(Routine routine, StatementCompound context, QNameManager.QName label)
        {
            base.ReadThis(0xfd);
            string sqlState = string.Empty;
            QNameManager.QName conditionName = null;
            if (base.ReadIfThis(0x105))
            {
                sqlState = this.ParseSqlStateValue();
            }
            else
            {
                conditionName = base.ReadNewSchemaObjectName(0x1a, false);
            }
            Dictionary<int, Expression> conditionInformationItems = null;
            if (base.token.TokenType == 0xfc)
            {
                base.Read();
                conditionInformationItems = this.ReadConditionInformationItems();
                if (conditionInformationItems.Count == 0)
                {
                    base.UnexpectedToken("SET");
                }
            }
            if (conditionName == null)
            {
                return new StatementSimple(0x5c, sqlState, conditionInformationItems);
            }
            return new StatementSimple(0x5c, conditionName, conditionInformationItems);
        }

        private Statement[] CompileSqlProcedureStatementList(Routine routine, StatementCompound context)
        {
            List<Statement> list = new List<Statement>();
            while (true)
            {
                Statement item = this.CompileSQLProcedureStatementOrNull(routine, context);
                if (item == null)
                {
                    break;
                }
                base.ReadThis(0x2bb);
                list.Add(item);
            }
            if (list.Count == 0)
            {
                throw base.UnexpectedToken();
            }
            return list.ToArray();
        }

        public Statement CompileSQLProcedureStatementOrNull(Routine routine, StatementCompound context)
        {
            Statement statement = null;
            QNameManager.QName label = null;
            RangeVariable[] outerRanges = (context == null) ? routine.GetParameterRangeVariables() : context.GetRangeVariables();
            if ((!routine.IsTrigger() && base.IsSimpleName()) && !base.IsReservedKey())
            {
                label = base.ReadNewSchemaObjectName(0x15, false);
                base.ReadThis(0x2ab);
            }
            base.SetSubParsePosition(base.GetPosition());
            base.compileContext.Reset();
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x85)
            {
                if (tokenType <= 0x36)
                {
                    if (tokenType <= 0x18)
                    {
                        if (tokenType == 0x10)
                        {
                            statement = this.CompileCompoundStatement(routine, context, label);
                        }
                        else
                        {
                            if (tokenType != 0x18)
                            {
                                goto Label_04EC;
                            }
                            if (label != null)
                            {
                                throw base.UnexpectedToken();
                            }
                            statement = base.CompileCallStatement(outerRanges, true);
                            Routine procedure = ((StatementProcedure) statement).Procedure;
                            if (procedure != null)
                            {
                                switch (routine.DataImpact)
                                {
                                    case 2:
                                        if ((procedure.DataImpact == 3) || (procedure.DataImpact == 4))
                                        {
                                            throw Error.GetError(0x15e8, routine.GetDataImpactString());
                                        }
                                        break;

                                    case 3:
                                        if (routine.DataImpact == 4)
                                        {
                                            throw Error.GetError(0x15e8, routine.GetDataImpactString());
                                        }
                                        break;
                                }
                            }
                        }
                        goto Label_04EE;
                    }
                    switch (tokenType)
                    {
                        case 0x1c:
                            statement = this.CompileCase(routine, context);
                            goto Label_04EE;

                        case 0x26:
                            if (label != null)
                            {
                                throw base.UnexpectedToken();
                            }
                            statement = this.CompileCloseCursorStatement(outerRanges);
                            goto Label_04EE;

                        case 0x36:
                            if (!routine.IsAnnonymous)
                            {
                                return null;
                            }
                            statement = this.CompileCreate();
                            goto Label_04EE;
                    }
                    goto Label_04EC;
                }
                if (tokenType <= 0x63)
                {
                    if (tokenType != 0x49)
                    {
                        if (tokenType == 0x4e)
                        {
                            goto Label_04D8;
                        }
                        if (tokenType != 0x63)
                        {
                            goto Label_04EC;
                        }
                        if (label != null)
                        {
                            throw base.UnexpectedToken();
                        }
                        statement = this.CompileExecute(outerRanges);
                    }
                    else
                    {
                        if (label != null)
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        statement = this.CompileDeallocatePrepare();
                    }
                }
                else if (tokenType == 0x6a)
                {
                    if (label != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    statement = this.CompileFetchCursorStatement(outerRanges);
                }
                else if (tokenType != 0x6f)
                {
                    if (tokenType != 0x85)
                    {
                        goto Label_04EC;
                    }
                    if (label != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    statement = base.CompileInsertStatement(outerRanges);
                }
                else
                {
                    if (routine.IsTrigger())
                    {
                        throw base.UnexpectedToken();
                    }
                    statement = this.CompileFor(routine, context, label);
                }
                goto Label_04EE;
            }
            if (tokenType <= 0xd3)
            {
                if (tokenType <= 0x9e)
                {
                    if (tokenType == 0x8d)
                    {
                        if (routine.IsTrigger() || (label != null))
                        {
                            throw base.UnexpectedToken();
                        }
                        statement = this.CompileIterate();
                    }
                    else if (tokenType != 150)
                    {
                        if (tokenType != 0x9e)
                        {
                            goto Label_04EC;
                        }
                        if (routine.IsTrigger())
                        {
                            throw base.UnexpectedToken();
                        }
                        statement = this.CompileLoop(routine, context, label);
                    }
                    else
                    {
                        if (routine.IsTrigger() || (label != null))
                        {
                            throw base.UnexpectedToken();
                        }
                        statement = this.CompileLeave(routine, context);
                    }
                }
                else if (tokenType != 0xa4)
                {
                    if (tokenType != 0xc2)
                    {
                        if (tokenType != 0xd3)
                        {
                            goto Label_04EC;
                        }
                        if (label != null)
                        {
                            throw base.UnexpectedToken();
                        }
                        statement = this.CompilePrepare(outerRanges);
                    }
                    else
                    {
                        if (label != null)
                        {
                            throw base.UnexpectedToken();
                        }
                        statement = this.CompileOpenCursorStatement(outerRanges);
                    }
                }
                else
                {
                    if (label != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    statement = base.CompileMergeStatement(outerRanges);
                }
                goto Label_04EE;
            }
            if (tokenType > 0x125)
            {
                if (tokenType != 0x12d)
                {
                    if (tokenType != 320)
                    {
                        if (tokenType != 0x19c)
                        {
                            goto Label_04EC;
                        }
                        statement = this.CompileIf(routine, context, false);
                    }
                    else
                    {
                        if (routine.IsTrigger())
                        {
                            throw base.UnexpectedToken();
                        }
                        statement = this.CompileWhile(routine, context, label);
                    }
                }
                else
                {
                    if (label != null)
                    {
                        throw base.UnexpectedToken();
                    }
                    statement = base.CompileUpdateStatement(outerRanges);
                }
                goto Label_04EE;
            }
            switch (tokenType)
            {
                case 0xe8:
                    if (routine.IsTrigger())
                    {
                        throw base.UnexpectedToken();
                    }
                    statement = this.CompileRepeat(routine, context, label);
                    goto Label_04EE;

                case 0xe9:
                    if (routine.IsTrigger() || (label != null))
                    {
                        throw base.UnexpectedToken();
                    }
                    statement = this.CompileResignal(routine, context, label);
                    goto Label_04EE;

                case 0xea:
                    goto Label_04EC;

                case 0xeb:
                    if (routine.IsTrigger() || (label != null))
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    statement = this.CompileReturnValue(routine, context);
                    goto Label_04EE;

                default:
                    switch (tokenType)
                    {
                        case 0xf9:
                            if (label != null)
                            {
                                throw base.UnexpectedToken();
                            }
                            statement = this.CompileSelectStatement(outerRanges, routine);
                            goto Label_04EE;

                        case 250:
                        case 0xfb:
                            goto Label_04EC;

                        case 0xfc:
                            if (label != null)
                            {
                                throw base.UnexpectedToken();
                            }
                            if (routine.IsTrigger())
                            {
                                int position = base.GetPosition();
                                bool flag = false;
                                try
                                {
                                    statement = this.CompileTriggerSetStatement(routine.TriggerTable, outerRanges);
                                    flag = true;
                                }
                                catch (Exception)
                                {
                                    this.Rewind(position);
                                    statement = this.CompileSetStatement(outerRanges, true);
                                }
                                if (flag)
                                {
                                    if (routine.TriggerOperation == 0x13)
                                    {
                                        this.Rewind(position);
                                        throw base.UnexpectedToken();
                                    }
                                    if (routine.TriggerType != 4)
                                    {
                                        this.Rewind(position);
                                        throw base.UnexpectedToken();
                                    }
                                }
                            }
                            else
                            {
                                statement = this.CompileSetStatement(outerRanges, false);
                            }
                            goto Label_04EE;

                        case 0xfd:
                            if (routine.IsTrigger() || (label != null))
                            {
                                throw base.UnexpectedToken();
                            }
                            statement = this.CompileSignal(routine, context, label);
                            goto Label_04EE;
                    }
                    if (tokenType == 0x125)
                    {
                        break;
                    }
                    goto Label_04EC;
            }
        Label_04D8:
            if (label != null)
            {
                throw base.UnexpectedToken();
            }
            statement = base.CompileDeleteStatement(outerRanges);
            goto Label_04EE;
        Label_04EC:
            return null;
        Label_04EE:
            statement.SetSql(base.GetLastSubPart());
            statement.SetRoot(routine);
            statement.SetParent(context);
            return statement;
        }

        private StatementDMQL CompileTriggerSetStatement(Table table, RangeVariable[] rangeVars)
        {
            base.Read();
            OrderedHashSet<Expression> targets = new OrderedHashSet<Expression>();
            List<Expression> expressions = new List<Expression>();
            RangeVariable[] variableArray = new RangeVariable[] { rangeVars[1] };
            LongDeque colIndexList = new LongDeque();
            base.ReadSetClauseList(variableArray, targets, colIndexList, expressions);
            int[] array = new int[colIndexList.Size()];
            colIndexList.ToArray(array);
            Expression[] a = new Expression[targets.Size()];
            targets.ToArray(a);
            for (int i = 0; i < a.Length; i++)
            {
                base.ResolveOuterReferencesAndTypes(RangeVariable.EmptyArray, a[i]);
            }
            Expression[] colExpressions = expressions.ToArray();
            base.ResolveUpdateExpressions(table, rangeVars, array, colExpressions, RangeVariable.EmptyArray);
            StatementSet set1 = new StatementSet(base.session, a, table, rangeVars, array, colExpressions);
            set1.SetDatabseObjects(base.session, base.compileContext);
            set1.CheckAccessRights(base.session);
            return set1;
        }

        private Statement CompileWhile(Routine routine, StatementCompound context, QNameManager.QName label)
        {
            base.ReadThis(320);
            Expression e = base.XreadBooleanValueExpression();
            this.ResolveOuterReferencesAndTypes(routine, context, e);
            StatementExpression condition = new StatementExpression(base.session, 0x44d, e);
            condition.SetDatabseObjects(base.session, base.compileContext);
            condition.CheckAccessRights(base.session);
            base.ReadThis(0x55);
            Statement[] statements = this.CompileSqlProcedureStatementList(routine, context);
            base.ReadThis(0x5d);
            base.ReadThis(320);
            if (base.IsSimpleName() && !base.IsReservedKey())
            {
                if (label == null)
                {
                    throw base.UnexpectedToken();
                }
                if (!label.Name.Equals(base.token.TokenString))
                {
                    throw Error.GetError(0x1584, base.token.TokenString);
                }
                base.Read();
            }
            StatementCompound compound1 = new StatementCompound(0x61, label);
            compound1.SetRoot(routine);
            compound1.SetParent(context);
            compound1.SetLocalDeclarations(new object[0]);
            compound1.SetStatements(statements);
            compound1.SetCondition(condition);
            return compound1;
        }

        public Table MakeTableFromType(QNameManager.QName qName, TableType type, int tableType)
        {
            Table table = new Table(base.database, qName, tableType);
            Table table2 = type.GetTable();
            for (int i = 0; i < table2.GetColumnCount(); i++)
            {
                ColumnSchema column = table2.GetColumn(i).Duplicate();
                QNameManager.QName name = base.database.NameManager.NewQName(column.GetName().Name, column.GetName().IsNameQuoted, 9);
                name.SetSchemaIfNull(qName.schema);
                name.Parent = qName;
                column.SetName(name);
                table.AddColumn(column);
            }
            return table;
        }

        public ColumnSchema MakeTableVariable(QNameManager.QName qName, SqlType type)
        {
            ColumnSchema schema1 = new ColumnSchema(base.database.NameManager.NewQName(qName.Name, qName.IsNameQuoted, 0x16), type, true, false, null);
            schema1.SetParameterMode(1);
            return schema1;
        }

        private string ParseSqlStateValue()
        {
            base.ReadIfThis(0x131);
            base.CheckIsValue(12);
            if (base.token.TokenString.Length != 5)
            {
                throw Error.GetError(0x15e7);
            }
            base.Read();
            return base.token.TokenString;
        }

        private List<Statement> ReadCaseWhen(Routine routine, StatementCompound context)
        {
            List<Statement> list = new List<Statement>();
            do
            {
                base.ReadThis(0x138);
                Expression e = base.XreadBooleanValueExpression();
                this.ResolveOuterReferencesAndTypes(routine, context, e);
                StatementExpression item = new StatementExpression(base.session, 0x44d, e);
                item.SetDatabseObjects(base.session, base.compileContext);
                item.CheckAccessRights(base.session);
                list.Add(item);
                base.ReadThis(0x116);
                Statement[] statementArray = this.CompileSqlProcedureStatementList(routine, context);
                for (int i = 0; i < statementArray.Length; i++)
                {
                    list.Add(statementArray[i]);
                }
            }
            while (base.token.TokenType == 0x138);
            return list;
        }

        private Dictionary<int, Expression> ReadConditionInformationItems()
        {
            Dictionary<int, Expression> dictionary = new Dictionary<int, Expression>();
            bool flag = false;
            while (!flag)
            {
                int tokenType = base.token.TokenType;
                if (tokenType > 0x174)
                {
                    goto Label_01E2;
                }
                if (tokenType <= 0x164)
                {
                    if (tokenType != 0x15d)
                    {
                        if (tokenType != 0x164)
                        {
                            goto Label_0367;
                        }
                        if (dictionary.ContainsKey(0x164))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x18c);
                        Expression expression = base.XreadValueExpression();
                        dictionary.Add(0x164, expression);
                    }
                    else
                    {
                        if (dictionary.ContainsKey(0x15d))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x18c);
                        Expression expression2 = base.XreadValueExpression();
                        dictionary.Add(0x15d, expression2);
                    }
                    continue;
                }
                switch (tokenType)
                {
                    case 370:
                        if (dictionary.ContainsKey(370))
                        {
                            throw base.UnexpectedToken();
                        }
                        break;

                    case 0x173:
                        if (dictionary.ContainsKey(0x173))
                        {
                            throw base.UnexpectedToken();
                        }
                        goto Label_0139;

                    case 0x174:
                        if (dictionary.ContainsKey(0x174))
                        {
                            throw base.UnexpectedToken();
                        }
                        goto Label_0178;

                    case 0x16a:
                    {
                        if (dictionary.ContainsKey(0x16a))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x18c);
                        Expression expression6 = base.XreadValueExpression();
                        dictionary.Add(0x16a, expression6);
                        continue;
                    }
                    default:
                        goto Label_0367;
                }
                base.Read();
                base.ReadThis(0x18c);
                Expression expression3 = base.XreadValueExpression();
                dictionary.Add(370, expression3);
                continue;
            Label_0139:
                base.Read();
                base.ReadThis(0x18c);
                Expression expression4 = base.XreadValueExpression();
                dictionary.Add(0x173, expression4);
                continue;
            Label_0178:
                base.Read();
                base.ReadThis(0x18c);
                Expression expression5 = base.XreadValueExpression();
                dictionary.Add(0x174, expression5);
                continue;
            Label_01E2:
                if (tokenType <= 0x1f1)
                {
                    if (tokenType != 440)
                    {
                        if (tokenType != 0x1f1)
                        {
                            goto Label_0367;
                        }
                        if (dictionary.ContainsKey(0x1f1))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x18c);
                        Expression expression7 = base.XreadValueExpression();
                        dictionary.Add(0x1f1, expression7);
                    }
                    else
                    {
                        if (dictionary.ContainsKey(440))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x18c);
                        Expression expression8 = base.XreadValueExpression();
                        dictionary.Add(440, expression8);
                    }
                    continue;
                }
                switch (tokenType)
                {
                    case 0x206:
                        if (dictionary.ContainsKey(0x206))
                        {
                            throw base.UnexpectedToken();
                        }
                        break;

                    case 0x207:
                        goto Label_0367;

                    case 520:
                        if (dictionary.ContainsKey(520))
                        {
                            throw base.UnexpectedToken();
                        }
                        goto Label_02EB;

                    default:
                        goto Label_0313;
                }
                base.Read();
                base.ReadThis(0x18c);
                Expression expression9 = base.XreadValueExpression();
                dictionary.Add(0x206, expression9);
                continue;
            Label_02EB:
                base.Read();
                base.ReadThis(0x18c);
                Expression expression10 = base.XreadValueExpression();
                dictionary.Add(520, expression10);
                continue;
            Label_0313:
                switch (tokenType)
                {
                    case 0x2ac:
                    {
                        base.Read();
                        continue;
                    }
                    case 0x331:
                    {
                        if (dictionary.ContainsKey(0x331))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x18c);
                        Expression expression11 = base.XreadValueExpression();
                        dictionary.Add(0x331, expression11);
                        continue;
                    }
                }
            Label_0367:
                flag = true;
            }
            return dictionary;
        }

        public Expression ReadDefaultClause(SqlType dataType)
        {
            Expression e = null;
            bool flag = false;
            int limit = base.ReadOpenBrackets();
            if (base.token.TokenType == 0xb8)
            {
                base.Read();
                e = new ExpressionValue(null, dataType);
                base.ReadCloseBrackets(limit);
                return e;
            }
            if (!dataType.IsDateTimeType() && !dataType.IsIntervalType())
            {
                if (dataType.IsNumberType())
                {
                    if (base.token.TokenType == 0x2b5)
                    {
                        base.Read();
                        flag = true;
                    }
                }
                else if (!dataType.IsCharacterType())
                {
                    if (dataType.IsBooleanType())
                    {
                        switch (base.token.TokenType)
                        {
                            case 0x69:
                                base.Read();
                                e = new ExpressionLogical(false);
                                base.ReadCloseBrackets(limit);
                                return e;

                            case 0x124:
                                base.Read();
                                e = new ExpressionLogical(true);
                                base.ReadCloseBrackets(limit);
                                return e;
                        }
                    }
                    else if (dataType.IsGuidType())
                    {
                        if (base.token.TokenType == 0x33a)
                        {
                            FunctionSQL function = FunctionSQL.NewSqlFunction(base.token.TokenString, base.compileContext);
                            e = base.ReadSQLFunction(function);
                        }
                    }
                    else if (dataType.IsArrayType())
                    {
                        e = base.ReadCollection(0x6b);
                        if (e.nodes.Length != 0)
                        {
                            throw Error.GetError(0x15ba);
                        }
                        base.ResolveOuterReferencesAndTypes(RangeVariable.EmptyArray, e);
                        base.ReadCloseBrackets(limit);
                        return e;
                    }
                }
                else
                {
                    switch (base.token.TokenType)
                    {
                        case 0xfb:
                        case 0x113:
                        case 0x12f:
                        case 0x3b:
                        case 0x3e:
                        case 0x3f:
                        case 0x40:
                        case 0x44:
                        {
                            FunctionSQL function = FunctionSQL.NewSqlFunction(base.token.TokenString, base.compileContext);
                            e = base.ReadSQLFunction(function);
                            break;
                        }
                    }
                }
                goto Label_0266;
            }
            int tokenType = base.token.TokenType;
            if (tokenType <= 0x8a)
            {
                switch (tokenType)
                {
                    case 0x47:
                    case 0x8a:
                        goto Label_008D;
                }
                goto Label_00CE;
            }
            if ((tokenType - 0x117) > 1)
            {
                if (tokenType == 0x2e9)
                {
                    goto Label_0266;
                }
                goto Label_00CE;
            }
        Label_008D:
            e = base.ReadDateTimeIntervalLiteral();
            if (e.DataType.TypeCode != dataType.TypeCode)
            {
                throw base.UnexpectedToken();
            }
            e = new ExpressionValue(e.GetValue(base.session, dataType), dataType);
            base.ReadCloseBrackets(limit);
            return e;
        Label_00CE:
            e = base.XreadDateTimeValueFunctionOrNull();
        Label_0266:
            if (e != null)
            {
                e.ResolveTypes(base.session, null);
                if (dataType.TypeComparisonGroup != e.GetDataType().TypeComparisonGroup)
                {
                    throw Error.GetError(0x15ba);
                }
                base.ReadCloseBrackets(limit);
                return e;
            }
            if (base.token.TokenType != 0x2e9)
            {
                throw base.UnexpectedToken();
            }
            object a = dataType.ConvertToType(base.session, base.token.TokenValue, base.token.DataType);
            base.Read();
            if (flag)
            {
                a = dataType.Negate(a);
            }
            e = new ExpressionValue(a, dataType);
            base.ReadCloseBrackets(limit);
            return e;
        }

        private object[] ReadLocalDeclarationList(Routine routine, StatementCompound context)
        {
            List<object> list = new List<object>();
            while (base.token.TokenType == 0x4c)
            {
                object item = this.ReadLocalTableVariableDeclarationOrNull(routine);
                if (item == null)
                {
                    item = this.ReadTypedLocalTableVariableDeclarationOrNull(routine);
                }
                if (item == null)
                {
                    item = this.ReadLocalVariableDeclarationOrNull();
                }
                if (item == null)
                {
                    item = this.CompileLocalHandlerDeclarationOrNull(routine, context);
                }
                if (item == null)
                {
                    item = this.CompileConditionDeclarationOrNull(routine, context);
                }
                if (item == null)
                {
                    RangeVariable[] outerRanges = (context == null) ? routine.GetParameterRangeVariables() : context.GetRangeVariables();
                    item = base.CompileDeclareCursor(false, outerRanges);
                }
                if (item is object[])
                {
                    list.AddRange((object[]) item);
                }
                else
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        private Table ReadLocalTableVariableDeclarationOrNull(Routine routine)
        {
            int position = base.GetPosition();
            base.ReadThis(0x4c);
            try
            {
                if (base.token.TokenType == 0x114)
                {
                    base.Read();
                    QNameManager.QName name = base.ReadNewSchemaObjectName(3, false);
                    name.SetSchemaIfNull(SqlInvariants.ModuleQname);
                    Table table = new Table(base.database, name, 11);
                    this.ReadTableDefinition(routine, table, true);
                    base.ReadThis(0x2bb);
                    return table;
                }
                this.Rewind(position);
                return null;
            }
            catch (Exception)
            {
                this.Rewind(position);
                return null;
            }
        }

        public ColumnSchema[] ReadLocalVariableDeclarationOrNull()
        {
            SqlType type;
            int position = base.GetPosition();
            QNameManager.QName[] emptyArray = QNameManager.QName.EmptyArray;
            try
            {
                base.ReadThis(0x4c);
                if (base.IsReservedKey())
                {
                    this.Rewind(position);
                    return null;
                }
            Label_002C:
                emptyArray = ArrayUtil.ResizeArray<QNameManager.QName>(emptyArray, emptyArray.Length + 1);
                QNameManager.QName name = base.ReadNewSchemaObjectName(0x16, false);
                emptyArray[emptyArray.Length - 1] = name;
                if (name.Name.StartsWith("@"))
                {
                    base.compileContext.AddNoneHostParameter(name.Name, base.GetPosition());
                }
                if (base.token.TokenType == 0x2ac)
                {
                    base.Read();
                    goto Label_002C;
                }
                type = base.ReadTypeDefinition(true, false);
            }
            catch (Exception)
            {
                this.Rewind(position);
                return null;
            }
            Expression defaultExpression = null;
            if (base.token.TokenType == 0x4d)
            {
                base.Read();
                defaultExpression = this.ReadDefaultClause(type);
            }
            ColumnSchema[] schemaArray = new ColumnSchema[emptyArray.Length];
            for (int i = 0; i < emptyArray.Length; i++)
            {
                schemaArray[i] = new ColumnSchema(emptyArray[i], type, true, false, defaultExpression);
                schemaArray[i].SetParameterMode(2);
            }
            base.ReadThis(0x2bb);
            return schemaArray;
        }

        private void ReadRoutineBody(Routine routine)
        {
            if (base.token.TokenType == 0x67)
            {
                if (routine.GetLanguage() == 2)
                {
                    throw base.UnexpectedToken();
                }
                routine.SetLanguage(1);
                base.Read();
                base.ReadThis(0x1bc);
                base.CheckIsValue(12);
                routine.SetMethodFqn((string) base.token.TokenValue);
                base.Read();
            }
            else
            {
                if (routine.GetLanguage() == 1)
                {
                    throw base.UnexpectedToken();
                }
                routine.SetLanguage(2);
                base.StartRecording();
                Statement statement = null;
                bool allowLocalTemporaryTables = base.session.sessionContext.AllowLocalTemporaryTables;
                base.session.sessionContext.AllowLocalTemporaryTables = false;
                try
                {
                    statement = this.CompileSQLProcedureStatementOrNull(routine, null);
                }
                finally
                {
                    base.session.sessionContext.AllowLocalTemporaryTables = allowLocalTemporaryTables;
                }
                if (statement == null)
                {
                    throw base.UnexpectedToken();
                }
                string sql = Token.GetSql(base.GetRecordedStatement());
                statement.SetSql(sql);
                routine.SetProcedure(statement);
            }
        }

        private void ReadRoutineCharacteristics(Routine routine)
        {
            OrderedIntHashSet set = new OrderedIntHashSet();
            bool flag = false;
            while (!flag)
            {
                int tokenType = base.token.TokenType;
                if (tokenType > 0xa9)
                {
                    goto Label_01BD;
                }
                if (tokenType <= 0x52)
                {
                    if (tokenType != 0x19)
                    {
                        if (tokenType != 0x52)
                        {
                            goto Label_042C;
                        }
                        if (!set.Add(0x52))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        routine.SetDeterministic(true);
                    }
                    else
                    {
                        if (!set.Add(0xb8) || routine.IsProcedure())
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0xc0);
                        base.ReadThis(0xb8);
                        base.ReadThis(0x1a3);
                        routine.SetNullInputOutput(false);
                    }
                }
                else
                {
                    switch (tokenType)
                    {
                        case 0x58:
                            if (!set.Add(0xea) || routine.IsFunction())
                            {
                                throw base.UnexpectedToken();
                            }
                            goto Label_0195;

                        case 0x90:
                        {
                            if (!set.Add(0x90))
                            {
                                throw base.UnexpectedToken();
                            }
                            base.Read();
                            if (base.token.TokenType != 0x1a9)
                            {
                                if (base.token.TokenType != 0x103)
                                {
                                    throw base.UnexpectedToken();
                                }
                                base.Read();
                                routine.SetLanguage(2);
                            }
                            else
                            {
                                base.Read();
                                routine.SetLanguage(1);
                            }
                            continue;
                        }
                    }
                    if (tokenType != 0xa9)
                    {
                        goto Label_042C;
                    }
                    if (!set.Add(0x103))
                    {
                        throw base.UnexpectedToken();
                    }
                    if (routine.GetSchemaObjectType() == 0x10)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadThis(0x103);
                    base.ReadThis(0x17a);
                    routine.SetDataImpact(4);
                }
                continue;
            Label_0195:
                base.Read();
                base.ReadThis(0xea);
                base.ReadThis(0x1fc);
                base.ReadBigint();
                continue;
            Label_01BD:
                if (tokenType > 0xd8)
                {
                    goto Label_033E;
                }
                switch (tokenType)
                {
                    case 0xb1:
                        if ((routine.GetSchemaObjectType() == 0x10) || !set.Add(0xf4))
                        {
                            throw base.UnexpectedToken();
                        }
                        break;

                    case 0xb2:
                        if (!set.Add(0x103))
                        {
                            throw base.UnexpectedToken();
                        }
                        goto Label_0247;

                    case 0xb3:
                    case 180:
                        goto Label_042C;

                    case 0xb5:
                        if (!set.Add(0x52))
                        {
                            throw base.UnexpectedToken();
                        }
                        goto Label_0275;

                    default:
                        goto Label_028F;
                }
                base.Read();
                base.ReadThis(0xf4);
                base.ReadThis(0x1b0);
                routine.SetNewSavepointLevel(true);
                continue;
            Label_0247:
                base.Read();
                base.ReadThis(0x103);
                routine.SetDataImpact(1);
                continue;
            Label_0275:
                base.Read();
                base.ReadThis(0x52);
                routine.SetDeterministic(false);
                continue;
            Label_028F:
                if (tokenType != 0xca)
                {
                    if (tokenType != 0xd8)
                    {
                        goto Label_042C;
                    }
                    if (!set.Add(0x103))
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadThis(0x103);
                    base.ReadThis(0x17a);
                    routine.SetDataImpact(3);
                }
                else
                {
                    if (!set.Add(0xca))
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    base.ReadThis(0x205);
                    if (base.token.TokenType == 0x1a9)
                    {
                        base.Read();
                        routine.SetParameterStyle(1);
                    }
                    else
                    {
                        base.ReadThis(0x103);
                        routine.SetParameterStyle(2);
                    }
                }
                continue;
            Label_033E:
                switch (tokenType)
                {
                    case 0xec:
                        if (!set.Add(0xb8) || routine.IsProcedure())
                        {
                            throw base.UnexpectedToken();
                        }
                        break;

                    case 0x101:
                    {
                        if (!set.Add(0x101))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        QNameManager.QName name = base.ReadNewSchemaObjectName(0x18, false);
                        routine.SetSpecificName(name);
                        continue;
                    }
                    default:
                    {
                        if (tokenType != 0x177)
                        {
                            goto Label_042C;
                        }
                        if (!set.Add(0x103))
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        base.ReadThis(0x103);
                        routine.SetDataImpact(2);
                        continue;
                    }
                }
                if (routine.IsAggregate())
                {
                    throw Error.GetError(0x15e4, base.token.TokenString);
                }
                base.Read();
                base.ReadThis(0xb8);
                base.ReadThis(0xc0);
                base.ReadThis(0xb8);
                base.ReadThis(0x1a3);
                routine.SetNullInputOutput(true);
                continue;
            Label_042C:
                flag = true;
            }
        }

        private ColumnSchema ReadRoutineParameter(Routine routine, bool isParam)
        {
            QNameManager.QName name = null;
            byte mode = 1;
            if (isParam)
            {
                int tokenType = base.token.TokenType;
                switch (tokenType)
                {
                    case 0x80:
                        base.Read();
                        goto Label_0073;

                    case 0x83:
                        if ((routine.GetSchemaObjectType() != 0x11) && !routine.IsAggregate())
                        {
                            throw base.UnexpectedToken();
                        }
                        base.Read();
                        mode = 2;
                        goto Label_0073;
                }
                if (tokenType == 0xc5)
                {
                    if (routine.GetSchemaObjectType() != 0x11)
                    {
                        throw base.UnexpectedToken();
                    }
                    base.Read();
                    mode = 4;
                }
            }
        Label_0073:
            if (!base.IsReservedKey())
            {
                name = base.ReadNewDependentSchemaObjectName(routine.GetName(), 0x17);
                if (name.Name.StartsWith("@"))
                {
                    base.compileContext.AddNoneHostParameter(name.Name, base.GetPosition());
                }
            }
            SqlType type = base.ReadTypeDefinition(true, routine.IsProcedure());
            ColumnSchema schema = new ColumnSchema(name, type, true, false, null);
            if (isParam)
            {
                if (type.IsRowType())
                {
                    name.SetSchemaIfNull(SqlInvariants.ModuleQname);
                    if (mode != 1)
                    {
                        throw base.UnexpectedTokenRequire("IN");
                    }
                }
                schema.SetParameterMode(mode);
                return schema;
            }
            if (type.IsRowType())
            {
                throw Error.GetError(0x1585, "TABLE");
            }
            return schema;
        }

        private List<Statement> ReadSimpleCaseWhen(Routine routine, StatementCompound context)
        {
            Expression expression4;
            List<Statement> list = new List<Statement>();
            Expression left = null;
            Expression l = base.XreadRowValuePredicand();
        Label_000F:
            base.ReadThis(0x138);
        Label_001A:
            expression4 = base.XreadPredicateRightPart(l);
            if (l == expression4)
            {
                expression4 = new ExpressionLogical(l, base.XreadRowValuePredicand());
            }
            this.ResolveOuterReferencesAndTypes(routine, context, expression4);
            if (left == null)
            {
                left = expression4;
            }
            else
            {
                left = new ExpressionLogical(50, left, expression4);
            }
            if (base.token.TokenType == 0x2ac)
            {
                base.Read();
                goto Label_001A;
            }
            StatementExpression item = new StatementExpression(base.session, 0x44d, left);
            item.SetDatabseObjects(base.session, base.compileContext);
            item.CheckAccessRights(base.session);
            list.Add(item);
            base.ReadThis(0x116);
            Statement[] statementArray = this.CompileSqlProcedureStatementList(routine, context);
            for (int i = 0; i < statementArray.Length; i++)
            {
                list.Add(statementArray[i]);
            }
            if (base.token.TokenType == 0x138)
            {
                goto Label_000F;
            }
            return list;
        }

        private ColumnSchema ReadTableColumn(Table table)
        {
            QNameManager.QName name = null;
            if (!base.IsReservedKey())
            {
                name = base.ReadNewDependentSchemaObjectName(table.GetName(), 9);
            }
            return new ColumnSchema(name, base.ReadTypeDefinition(true, false), true, false, null);
        }

        protected void ReadTableDefinition(Routine routine, Table table, bool isTable)
        {
            ColumnSchema schema;
            base.ReadThis(0x2b7);
            int num = 0;
        Label_000D:
            if (isTable)
            {
                schema = this.ReadTableColumn(table);
            }
            else
            {
                schema = this.ReadRoutineParameter(routine, false);
            }
            if (schema.GetName() == null)
            {
                throw base.UnexpectedToken();
            }
            table.AddColumn(schema);
            if (base.token.TokenType != 0x2ac)
            {
                base.ReadThis(0x2aa);
                table.CreatePrimaryKey();
            }
            else
            {
                base.Read();
                num++;
                goto Label_000D;
            }
        }

        private object[] ReadTypedLocalTableVariableDeclarationOrNull(Routine routine)
        {
            int position = base.GetPosition();
            base.ReadThis(0x4c);
            try
            {
                QNameManager.QName qName = base.ReadNewSchemaObjectName(3, false);
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
                qName.schema = SqlInvariants.ModuleQname;
                Table table = this.MakeTableFromType(qName, (TableType) type, 11);
                table.CreatePrimaryKey();
                base.ReadThis(0x2bb);
                ColumnSchema schema = this.MakeTableVariable(qName, type);
                return new object[] { table, schema };
            }
            catch (Exception)
            {
                this.Rewind(position);
                return null;
            }
        }

        private void ResolveOuterReferencesAndTypes(Routine routine, StatementCompound context, Expression e)
        {
            RangeVariable[] parameterRangeVariables = routine.GetParameterRangeVariables();
            if (context != null)
            {
                parameterRangeVariables = context.GetRangeVariables();
            }
            base.ResolveOuterReferencesAndTypes(parameterRangeVariables, e);
        }

        public void SetRoutineTables(Database database, Routine routine)
        {
            if (routine.ParameterList.Size() != 0)
            {
                HashMappedList<string, Table> list = new HashMappedList<string, Table>();
                for (int i = 0; i < routine.ParameterList.Size(); i++)
                {
                    ColumnSchema schema = routine.ParameterList.Get(i);
                    SqlType dataType = schema.GetDataType();
                    if (dataType.IsTableType())
                    {
                        QNameManager.QName qName = database.NameManager.NewQName(schema.GetName().Name, schema.GetName().IsNameQuoted, 3);
                        qName.schema = SqlInvariants.ModuleQname;
                        Table table = this.MakeTableFromType(qName, (TableType) dataType, 11);
                        table.CreatePrimaryKey();
                        if (!list.Add(qName.Name, table))
                        {
                            throw Error.GetError(0x15e6, qName.Name);
                        }
                    }
                }
                routine.ScopeTables = list;
            }
        }
    }
}


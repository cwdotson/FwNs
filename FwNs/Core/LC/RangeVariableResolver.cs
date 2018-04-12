namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class RangeVariableResolver
    {
        private readonly OrderedIntHashSet _colIndexSetEqual = new OrderedIntHashSet();
        private readonly Dictionary<int, int> _colIndexSetOther = new Dictionary<int, int>();
        private readonly ParserDQL.CompileContext _compileContext;
        private readonly bool _hasOuterJoin;
        private readonly Expression[] _inExpressions;
        private readonly bool[] _inInJoin;
        private readonly List<Expression>[] _joinExpressions;
        private readonly List<Expression> _queryExpressions = new List<Expression>();
        private readonly OrderedHashSet<RangeVariable> _rangeVarSet = new OrderedHashSet<RangeVariable>();
        private readonly List<Expression>[] _tempJoinExpressions;
        private readonly MultiValueHashMap<ColumnSchema, Expression> _tempMap = new MultiValueHashMap<ColumnSchema, Expression>();
        private readonly OrderedHashSet<object> _tempSet = new OrderedHashSet<object>();
        private readonly OrderedHashSet<RangeVariable> _tempSetRangeVar = new OrderedHashSet<RangeVariable>();
        private readonly List<Expression>[] _whereExpressions;
        public RangeVariable[] RangeVariables;
        private Expression _conditions;
        private int _inExpressionCount;
        public Session session;

        public RangeVariableResolver(RangeVariable[] rangeVars, Expression conditions, ParserDQL.CompileContext compileContext)
        {
            this.RangeVariables = rangeVars;
            this._conditions = conditions;
            this._compileContext = compileContext;
            for (int i = 0; i < rangeVars.Length; i++)
            {
                RangeVariable key = rangeVars[i];
                this._rangeVarSet.Add(key);
                if (key.IsLeftJoin || key.IsRightJoin)
                {
                    this._hasOuterJoin = true;
                }
            }
            this._inExpressions = new Expression[rangeVars.Length];
            this._inInJoin = new bool[rangeVars.Length];
            this._tempJoinExpressions = new List<Expression>[rangeVars.Length];
            for (int j = 0; j < rangeVars.Length; j++)
            {
                this._tempJoinExpressions[j] = new List<Expression>();
            }
            this._joinExpressions = new List<Expression>[rangeVars.Length];
            for (int k = 0; k < rangeVars.Length; k++)
            {
                this._joinExpressions[k] = new List<Expression>();
            }
            this._whereExpressions = new List<Expression>[rangeVars.Length];
            for (int m = 0; m < rangeVars.Length; m++)
            {
                this._whereExpressions[m] = new List<Expression>();
            }
        }

        public void AssignToJoinLists(Expression e, List<Expression>[] expressionLists, int first)
        {
            this._tempSetRangeVar.Clear();
            e.CollectRangeVariables(this.RangeVariables, this._tempSetRangeVar);
            int largestIndex = this._rangeVarSet.GetLargestIndex(this._tempSetRangeVar);
            if (largestIndex == -1)
            {
                largestIndex = 0;
            }
            if (largestIndex < first)
            {
                largestIndex = first;
            }
            expressionLists[largestIndex].Add(e);
        }

        public void AssignToLists()
        {
            int num = -1;
            int first = -1;
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                if (this.RangeVariables[i].IsLeftJoin)
                {
                    num = i;
                }
                if (this.RangeVariables[i].IsRightJoin)
                {
                    num = i;
                    first = i;
                }
                if (num == i)
                {
                    this._joinExpressions[i].AddRange(this._tempJoinExpressions[i]);
                }
                else
                {
                    for (int k = 0; k < this._tempJoinExpressions[i].Count; k++)
                    {
                        this.AssignToJoinLists(this._tempJoinExpressions[i][k], this._joinExpressions, num + 1);
                    }
                }
            }
            for (int j = 0; j < this._queryExpressions.Count; j++)
            {
                this.AssignToWhereLists(this._queryExpressions[j], this._whereExpressions, first);
            }
        }

        private static void AssignToRangeVariable(RangeVariable.RangeVariableConditions conditions, List<Expression> exprList)
        {
            int num = 0;
            int count = exprList.Count;
            while (num < count)
            {
                Expression e = exprList[num];
                conditions.AddCondition(e);
                num++;
            }
        }

        private void AssignToRangeVariable(RangeVariable rangeVar, RangeVariable.RangeVariableConditions conditions, int rangeVarIndex, List<Expression> exprList)
        {
            if (exprList.Count != 0)
            {
                this.SetIndexConditions(conditions, exprList, rangeVarIndex, true);
            }
        }

        public void AssignToRangeVariables()
        {
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                bool flag = false;
                if (this.RangeVariables[i].IsLeftJoin || this.RangeVariables[i].IsRightJoin)
                {
                    RangeVariable.RangeVariableConditions conditions = this.RangeVariables[i].JoinConditions[0];
                    this.AssignToRangeVariable(this.RangeVariables[i], conditions, i, this._joinExpressions[i]);
                    conditions = this.RangeVariables[i].JoinConditions[0];
                    if (conditions.HasIndexCondition())
                    {
                        flag = true;
                    }
                    conditions = this.RangeVariables[i].WhereConditions[0];
                    if (this.RangeVariables[i].IsRightJoin)
                    {
                        AssignToRangeVariable(conditions, this._whereExpressions[i]);
                    }
                    else if (flag)
                    {
                        AssignToRangeVariable(conditions, this._whereExpressions[i]);
                    }
                    else
                    {
                        this.AssignToRangeVariable(this.RangeVariables[i], conditions, i, this._whereExpressions[i]);
                    }
                }
                else
                {
                    RangeVariable.RangeVariableConditions conditions = this.RangeVariables[i].JoinConditions[0];
                    if (this._hasOuterJoin)
                    {
                        AssignToRangeVariable(this.RangeVariables[i].WhereConditions[0], this._whereExpressions[i]);
                    }
                    else
                    {
                        this._joinExpressions[i].AddRange(this._whereExpressions[i]);
                    }
                    this.AssignToRangeVariable(this.RangeVariables[i], conditions, i, this._joinExpressions[i]);
                }
            }
            if (this._inExpressionCount != 0)
            {
                this.SetInConditionsAsTables();
            }
        }

        public void AssignToWhereLists(Expression e, List<Expression>[] expressionLists, int first)
        {
            this._tempSetRangeVar.Clear();
            e.CollectRangeVariables(this.RangeVariables, this._tempSetRangeVar);
            int largestIndex = this._rangeVarSet.GetLargestIndex(this._tempSetRangeVar);
            if (largestIndex == -1)
            {
                largestIndex = 0;
            }
            if (largestIndex < first)
            {
                largestIndex = first;
            }
            expressionLists[largestIndex].Add(e);
        }

        public void CloseJoinChain(List<Expression>[] array, Expression e1, Expression e2)
        {
            int index = this._rangeVarSet.GetIndex(e1.GetRangeVariable());
            int num2 = this._rangeVarSet.GetIndex(e2.GetRangeVariable());
            int num3 = (index > num2) ? index : num2;
            Expression item = new ExpressionLogical(e1, e2);
            for (int i = 0; i < array[num3].Count; i++)
            {
                if (item.Equals(array[num3][i]))
                {
                    return;
                }
            }
            array[num3].Add(item);
        }

        private void CollectAndOrChains(Expression expr, List<Expression> exprList, List<int> opList)
        {
            if ((expr.OpType != 50) && (expr.OpType != 0x31))
            {
                exprList.Add(expr);
            }
            else if ((expr.nodes[0].OpType == expr.OpType) && !expr.NoBreak)
            {
                this.CollectAndOrChains(expr.nodes[0], exprList, opList);
                exprList.Add(expr.nodes[1]);
                opList.Add(expr.OpType);
            }
            else
            {
                this.FlattenAndOrChains(expr.nodes[0]);
                this.FlattenAndOrChains(expr.nodes[1]);
                exprList.Add(expr);
            }
        }

        public static Expression DecomposeAndConditions(Expression e, List<Expression> conditions)
        {
            if (e != null)
            {
                Expression leftNode = e.GetLeftNode();
                Expression rightNode = e.GetRightNode();
                int exprType = e.GetExprType();
                if (exprType == 0x31)
                {
                    leftNode = DecomposeAndConditions(leftNode, conditions);
                    rightNode = DecomposeAndConditions(rightNode, conditions);
                    if ((leftNode.ValueData != null) && ((bool) leftNode.ValueData))
                    {
                        return rightNode;
                    }
                    if ((rightNode.ValueData != null) && ((bool) rightNode.ValueData))
                    {
                        return leftNode;
                    }
                    e.SetLeftNode(leftNode);
                    e.SetRightNode(rightNode);
                    return e;
                }
                if (((exprType == 0x29) && (leftNode.GetExprType() == 0x19)) && (rightNode.GetExprType() == 0x19))
                {
                    for (int i = 0; i < leftNode.nodes.Length; i++)
                    {
                        Expression item = new ExpressionLogical(leftNode.nodes[i], rightNode.nodes[i]);
                        item.ResolveTypes(null, null);
                        conditions.Add(item);
                    }
                    return new ExpressionLogical(true);
                }
                if ((e.ValueData == null) || !((bool) e.ValueData))
                {
                    conditions.Add(e);
                }
            }
            return new ExpressionLogical(true);
        }

        private static Expression DecomposeOrConditions(Expression e, List<Expression> conditions)
        {
            bool flag;
            if (e == null)
            {
                return new ExpressionLogical(false);
            }
            Expression leftNode = e.GetLeftNode();
            Expression rightNode = e.GetRightNode();
            if (e.GetExprType() != 50)
            {
                flag = false;
                if (!flag.Equals(e.ValueData))
                {
                    conditions.Add(e);
                }
                return new ExpressionLogical(false);
            }
            leftNode = DecomposeOrConditions(leftNode, conditions);
            rightNode = DecomposeOrConditions(rightNode, conditions);
            flag = false;
            if (flag.Equals(leftNode.ValueData))
            {
                return rightNode;
            }
            flag = false;
            if (flag.Equals(rightNode.ValueData))
            {
                return leftNode;
            }
            e = new ExpressionLogical(50, leftNode, rightNode);
            return e;
        }

        public void ExpandConditions()
        {
            this.ExpandConditions(this._joinExpressions, true);
            if (!this._hasOuterJoin)
            {
                this.ExpandConditions(this._joinExpressions, false);
            }
        }

        public void ExpandConditions(List<Expression>[] array, bool isJoin)
        {
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                List<Expression> list = array[i];
                this._tempMap.Clear();
                this._tempSet.Clear();
                bool flag = false;
                for (int j = 0; j < list.Count; j++)
                {
                    Expression expression = list[j];
                    if (expression.IsColumnEqual && (expression.GetLeftNode().GetRangeVariable() != expression.GetRightNode().GetRangeVariable()))
                    {
                        if (expression.GetLeftNode().GetRangeVariable() == this.RangeVariables[i])
                        {
                            this._tempMap.Put(expression.GetLeftNode().GetColumn(), expression.GetRightNode());
                            if (!this._tempSet.Add(expression.GetLeftNode().GetColumn()))
                            {
                                flag = true;
                            }
                        }
                        else if (expression.GetRightNode().GetRangeVariable() == this.RangeVariables[i])
                        {
                            this._tempMap.Put(expression.GetRightNode().GetColumn(), expression.GetLeftNode());
                            if (!this._tempSet.Add(expression.GetRightNode().GetColumn()))
                            {
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    Iterator<ColumnSchema> iterator = this._tempMap.GetKeySet().GetIterator();
                    while (iterator.HasNext())
                    {
                        ColumnSchema key = iterator.Next();
                        Iterator<Expression> iterator2 = this._tempMap.Get(key);
                        this._tempSet.Clear();
                        while (iterator2.HasNext())
                        {
                            this._tempSet.Add(iterator2.Next());
                        }
                        while (this._tempSet.Size() > 1)
                        {
                            Expression expression2 = (Expression) this._tempSet.Remove((int) (this._tempSet.Size() - 1));
                            for (int k = 0; k < this._tempSet.Size(); k++)
                            {
                                Expression expression3 = (Expression) this._tempSet.Get(k);
                                this.CloseJoinChain(array, expression2, expression3);
                            }
                        }
                    }
                }
            }
        }

        private void FlattenAndOrChains(Expression expr)
        {
            List<Expression> exprList = new List<Expression>();
            List<int> opList = new List<int>();
            this.CollectAndOrChains(expr, exprList, opList);
            if ((opList.Count > 1) && (exprList.Count == (opList.Count + 1)))
            {
                expr.nodes = exprList.ToArray();
                expr.OpType = 0x6f;
                expr.OpTypeChain = opList.ToArray();
            }
        }

        private void FlattenWhereExpressionLists()
        {
            for (int i = 0; i < this._whereExpressions.Length; i++)
            {
                for (int j = 0; j < this._whereExpressions[i].Count; j++)
                {
                    this.FlattenAndOrChains(this._whereExpressions[i][j]);
                }
            }
        }

        public RangeVariable[] GetRangeVariables()
        {
            return this.RangeVariables;
        }

        public void ProcessConditions(Session session)
        {
            this.session = session;
            DecomposeAndConditions(this._conditions, this._queryExpressions);
            for (int i = 0; i < this.RangeVariables.Length; i++)
            {
                if (this.RangeVariables[i].JoinCondition != null)
                {
                    DecomposeAndConditions(this.RangeVariables[i].JoinCondition, this._tempJoinExpressions[i]);
                }
            }
            this._conditions = null;
            this.AssignToLists();
            this.FlattenWhereExpressionLists();
            bool flag1 = this._hasOuterJoin;
            this.ExpandConditions();
            this.AssignToRangeVariables();
        }

        private void SetEqaulityConditions(RangeVariable.RangeVariableConditions conditions, List<Expression> exprList)
        {
            Index index = conditions.RangeVar.RangeTable.GetIndexForColumns(this.session, this._colIndexSetEqual, false);
            if (index != null)
            {
                int[] columns = index.GetColumns();
                int length = columns.Length;
                Expression[] expressionArray = new Expression[columns.Length];
                for (int i = 0; i < exprList.Count; i++)
                {
                    Expression e = exprList[i];
                    if (e != null)
                    {
                        switch (e.GetExprType())
                        {
                            case 0x29:
                            case 0x2f:
                            {
                                if (e.GetLeftNode().GetRangeVariable() != conditions.RangeVar)
                                {
                                    continue;
                                }
                                int num4 = ArrayUtil.Find(columns, e.GetLeftNode().GetColumnIndex());
                                if ((num4 != -1) && (expressionArray[num4] == null))
                                {
                                    expressionArray[num4] = e;
                                    exprList[i] = null;
                                    continue;
                                }
                                break;
                            }
                        }
                        conditions.AddCondition(e);
                        exprList[i] = null;
                    }
                }
                bool flag = false;
                for (int j = 0; j < expressionArray.Length; j++)
                {
                    Expression e = expressionArray[j];
                    if (e == null)
                    {
                        if (length == columns.Length)
                        {
                            length = j;
                        }
                        flag = true;
                    }
                    else if (flag)
                    {
                        conditions.AddCondition(e);
                        expressionArray[j] = null;
                    }
                }
                conditions.AddIndexCondition(expressionArray, index, length);
            }
        }

        private void SetInConditionsAsTables()
        {
            for (int i = this.RangeVariables.Length - 1; i >= 0; i--)
            {
                RangeVariable range = this.RangeVariables[i];
                ExpressionLogical e = (ExpressionLogical) this._inExpressions[i];
                if (e != null)
                {
                    OrderedIntHashSet set = new OrderedIntHashSet();
                    e.AddLeftColumnsForAllAny(range, set);
                    Index index = range.RangeTable.GetIndexForColumns(this.session, set, false);
                    int num2 = 0;
                    for (int j = 0; j < index.GetColumnCount(); j++)
                    {
                        if (set.Contains(index.GetColumns()[j]))
                        {
                            num2++;
                        }
                    }
                    RangeVariable addition = new RangeVariable(e.GetRightNode().GetTable(), null, null, null, this._compileContext) {
                        IsGenerated = true
                    };
                    RangeVariable[] dest = new RangeVariable[this.RangeVariables.Length + 1];
                    ArrayUtil.CopyAdjustArray<RangeVariable>(this.RangeVariables, dest, addition, i, 1);
                    this.RangeVariables = dest;
                    Expression[] exprList = new Expression[num2];
                    for (int k = 0; k < num2; k++)
                    {
                        int num5 = index.GetColumns()[k];
                        int colIndexRight = set.GetIndex(num5);
                        exprList[k] = new ExpressionLogical(range, num5, addition, colIndexRight);
                    }
                    bool flag = this.RangeVariables[i].IsLeftJoin || this.RangeVariables[i].IsRightJoin;
                    RangeVariable.RangeVariableConditions conditions1 = (!this._inInJoin[i] & flag) ? range.WhereConditions[0] : range.JoinConditions[0];
                    conditions1.AddIndexCondition(exprList, index, exprList.Length);
                    conditions1.AddCondition(e);
                }
            }
        }

        private void SetIndexConditions(RangeVariable.RangeVariableConditions conditions, List<Expression> exprList, int rangeVarIndex, bool includeOr)
        {
            this._colIndexSetEqual.Clear();
            this._colIndexSetOther.Clear();
            int num = 0;
            int count = exprList.Count;
            while (num < count)
            {
                Expression expression = exprList[num];
                if ((expression != null) && expression.IsIndexable(conditions.RangeVar))
                {
                    switch (expression.GetExprType())
                    {
                        case 0x29:
                            if (((expression.ExprSubType != 0x34) && (expression.ExprSubType != 0x33)) && (expression.GetLeftNode().GetRangeVariable() == conditions.RangeVar))
                            {
                                int columnIndex = expression.GetLeftNode().GetColumnIndex();
                                this._colIndexSetEqual.Add(columnIndex);
                            }
                            goto Label_01C9;

                        case 0x2a:
                        case 0x2b:
                        case 0x2c:
                        case 0x2d:
                            if (expression.GetLeftNode().GetRangeVariable() == conditions.RangeVar)
                            {
                                int num8;
                                int columnIndex = expression.GetLeftNode().GetColumnIndex();
                                if (!this._colIndexSetOther.TryGetValue(columnIndex, out num8))
                                {
                                    num8 = 0;
                                }
                                this._colIndexSetOther[columnIndex] = num8 + 1;
                            }
                            goto Label_01C9;

                        case 0x2f:
                            if (expression.GetLeftNode().GetRangeVariable() == conditions.RangeVar)
                            {
                                int columnIndex = expression.GetLeftNode().GetColumnIndex();
                                this._colIndexSetEqual.Add(columnIndex);
                            }
                            goto Label_01C9;

                        case 0x30:
                            if (expression.GetLeftNode().GetLeftNode().GetRangeVariable() == conditions.RangeVar)
                            {
                                int num11;
                                int columnIndex = expression.GetLeftNode().GetLeftNode().GetColumnIndex();
                                if (!this._colIndexSetOther.TryGetValue(columnIndex, out num11))
                                {
                                    num11 = 0;
                                }
                                this._colIndexSetOther[columnIndex] = num11 + 1;
                            }
                            goto Label_01C9;

                        case 50:
                        case 2:
                            goto Label_01C9;
                    }
                    Error.RuntimeError(0xc9, "RangeVariableResolver");
                }
            Label_01C9:
                num++;
            }
            this.SetEqaulityConditions(conditions, exprList);
            if (!conditions.HasIndexCondition())
            {
                this.SetNonEqualityConditions(conditions, exprList);
            }
            bool flag = conditions.HasIndexCondition();
            bool flag2 = false;
            if (!flag & includeOr)
            {
                int num12 = 0;
                int num13 = exprList.Count;
                while (num12 < num13)
                {
                    Expression expression2 = exprList[num12];
                    if (expression2 != null)
                    {
                        if (expression2.GetExprType() == 50)
                        {
                            flag = expression2.IsIndexable(conditions.RangeVar);
                            if (flag)
                            {
                                flag = this.SetOrConditions(conditions, (ExpressionLogical) expression2, rangeVarIndex);
                            }
                            if (!flag)
                            {
                                goto Label_02FC;
                            }
                            exprList[num12] = null;
                            flag2 = true;
                            break;
                        }
                        if (((expression2.GetExprType() == 0x29) && (expression2.ExprSubType == 0x34)) && !expression2.GetRightNode().IsCorrelated())
                        {
                            OrderedIntHashSet set = new OrderedIntHashSet();
                            ((ExpressionLogical) expression2).AddLeftColumnsForAllAny(conditions.RangeVar, set);
                            if ((conditions.RangeVar.RangeTable.GetIndexForColumns(this.session, set, false) != null) && (this._inExpressions[rangeVarIndex] == null))
                            {
                                this._inExpressions[rangeVarIndex] = expression2;
                                this._inInJoin[rangeVarIndex] = conditions.IsJoin;
                                this._inExpressionCount++;
                                exprList[num12] = null;
                                break;
                            }
                        }
                    }
                Label_02FC:
                    num12++;
                }
            }
            int num3 = 0;
            int num4 = exprList.Count;
            while (num3 < num4)
            {
                Expression expression3 = exprList[num3];
                if (expression3 != null)
                {
                    if (flag2)
                    {
                        for (int i = 0; i < conditions.RangeVar.JoinConditions.Length; i++)
                        {
                            if (conditions.IsJoin)
                            {
                                conditions.RangeVar.JoinConditions[i].NonIndexCondition = ExpressionLogical.AndExpressions(expression3, conditions.RangeVar.JoinConditions[i].NonIndexCondition);
                            }
                            else
                            {
                                conditions.RangeVar.WhereConditions[i].NonIndexCondition = ExpressionLogical.AndExpressions(expression3, conditions.RangeVar.WhereConditions[i].NonIndexCondition);
                            }
                        }
                    }
                    else
                    {
                        conditions.AddCondition(expression3);
                    }
                }
                num3++;
            }
        }

        private void SetNonEqualityConditions(RangeVariable.RangeVariableConditions conditions, List<Expression> exprList)
        {
            if (this._colIndexSetOther.Count != 0)
            {
                int num = 0;
                int col = 0;
                foreach (KeyValuePair<int, int> pair in this._colIndexSetOther)
                {
                    int key = pair.Key;
                    if (pair.Value > num)
                    {
                        col = key;
                    }
                }
                Index indexForColumn = conditions.RangeVar.RangeTable.GetIndexForColumn(this.session, col);
                if (indexForColumn == null)
                {
                    foreach (int num4 in this._colIndexSetOther.Keys)
                    {
                        if (num4 != col)
                        {
                            indexForColumn = conditions.RangeVar.RangeTable.GetIndexForColumn(this.session, num4);
                            if (indexForColumn != null)
                            {
                                break;
                            }
                        }
                    }
                }
                if (indexForColumn != null)
                {
                    int[] columns = indexForColumn.GetColumns();
                    for (int i = 0; i < exprList.Count; i++)
                    {
                        Expression expression = exprList[i];
                        if (expression == null)
                        {
                            continue;
                        }
                        bool flag = false;
                        int exprType = expression.GetExprType();
                        if ((exprType - 0x2a) > 3)
                        {
                            if (exprType == 0x30)
                            {
                                goto Label_013F;
                            }
                        }
                        else if (((columns[0] == expression.GetLeftNode().GetColumnIndex()) && (expression.GetRightNode() != null)) && !expression.GetRightNode().IsCorrelated())
                        {
                            flag = true;
                        }
                        goto Label_0169;
                    Label_013F:
                        if ((expression.GetLeftNode().GetExprType() == 0x2f) && (columns[0] == expression.GetLeftNode().GetLeftNode().GetColumnIndex()))
                        {
                            flag = true;
                        }
                    Label_0169:
                        if (flag)
                        {
                            Expression[] expressionArray = new Expression[indexForColumn.GetColumnCount()];
                            expressionArray[0] = expression;
                            conditions.AddIndexCondition(expressionArray, indexForColumn, 1);
                            exprList[i] = null;
                            return;
                        }
                    }
                }
            }
        }

        private bool SetOrConditions(RangeVariable.RangeVariableConditions conditions, ExpressionLogical orExpression, int rangeVarIndex)
        {
            List<Expression> list = new List<Expression>();
            DecomposeOrConditions(orExpression, list);
            RangeVariable.RangeVariableConditions[] array = new RangeVariable.RangeVariableConditions[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                List<Expression> list2 = new List<Expression>();
                DecomposeAndConditions(list[i], list2);
                RangeVariable.RangeVariableConditions conditions2 = new RangeVariable.RangeVariableConditions(conditions);
                this.SetIndexConditions(conditions2, list2, rangeVarIndex, false);
                array[i] = conditions2;
                if (!conditions2.HasIndexCondition())
                {
                    return false;
                }
            }
            Expression left = null;
            for (int j = 0; j < array.Length; j++)
            {
                RangeVariable.RangeVariableConditions conditions3 = array[j];
                array[j].ExcludeConditions = left;
                if (j > 1)
                {
                    Expression excludeConditions = array[j - 1].ExcludeConditions;
                    left = new ExpressionLogical(50, left, excludeConditions);
                }
                if (conditions3.IndexCond != null)
                {
                    for (int k = 0; k < conditions3.IndexedColumnCount; k++)
                    {
                        left = ExpressionLogical.AndExpressions(left, conditions3.IndexCond[k]);
                    }
                }
                left = ExpressionLogical.AndExpressions(ExpressionLogical.AndExpressions(left, conditions3.IndexEndCondition), conditions3.NonIndexCondition);
            }
            if (conditions.IsJoin)
            {
                conditions.RangeVar.JoinConditions = array;
                array = new RangeVariable.RangeVariableConditions[list.Count];
                ArrayUtil.FillArray(array, conditions.RangeVar.WhereConditions[0]);
                conditions.RangeVar.WhereConditions = array;
            }
            else
            {
                conditions.RangeVar.WhereConditions = array;
                array = new RangeVariable.RangeVariableConditions[list.Count];
                ArrayUtil.FillArray(array, conditions.RangeVar.JoinConditions[0]);
                conditions.RangeVar.JoinConditions = array;
            }
            return true;
        }
    }
}


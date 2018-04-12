namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;

    public sealed class SortAndSlice
    {
        public int[] SortOrder;
        public bool[] SortDescending;
        public bool[] SortNullsLast;
        public bool SortUnion;
        public List<ExpressionOrderBy> ExprList = new List<ExpressionOrderBy>();
        public Expression LimitCondition;
        public int ColumnCount;
        public bool HasNullsLast;
        public bool SkipSort;
        public bool SkipFullResult;
        public Index index;
        private int[] _columnIndexes;
        public bool IsGenerated;

        public void AddLimitCondition(Expression expression)
        {
            this.LimitCondition = expression;
        }

        public void AddOrderExpression(ExpressionOrderBy e)
        {
            this.ExprList.Add(e);
        }

        public int GetLimitCount(Session session, int rowCount)
        {
            int num = 0;
            if (this.LimitCondition != null)
            {
                object obj2 = this.LimitCondition.GetRightNode().GetValue(session);
                if (obj2 != null)
                {
                    num = (int) obj2;
                }
            }
            if ((rowCount == 0) || ((num != 0) && (rowCount >= num)))
            {
                return num;
            }
            return rowCount;
        }

        public int GetLimitStart(Session session)
        {
            if (this.LimitCondition != null)
            {
                object obj2 = this.LimitCondition.GetLeftNode().GetValue(session);
                if (obj2 != null)
                {
                    return (int) obj2;
                }
            }
            return 0;
        }

        public int GetOrderLength()
        {
            return this.ExprList.Count;
        }

        public bool HasLimit()
        {
            return (this.LimitCondition > null);
        }

        public bool HasOrder()
        {
            return (this.ExprList.Count > 0);
        }

        public void Prepare(QuerySpecification select)
        {
            this.ColumnCount = this.ExprList.Count;
            if (this.ColumnCount != 0)
            {
                this.SortOrder = new int[this.ColumnCount];
                this.SortDescending = new bool[this.ColumnCount];
                this.SortNullsLast = new bool[this.ColumnCount];
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    ExpressionOrderBy by = this.ExprList[i];
                    if (by.GetLeftNode().QueryTableColumnIndex == -1)
                    {
                        this.SortOrder[i] = select.IndexStartOrderBy + i;
                    }
                    else
                    {
                        this.SortOrder[i] = by.GetLeftNode().QueryTableColumnIndex;
                    }
                    this.SortDescending[i] = by.IsDescending();
                    this.SortNullsLast[i] = by.IsNullsLast();
                    this.HasNullsLast |= this.SortNullsLast[i];
                }
                if (((select != null) && !this.HasNullsLast) && (!select.IsDistinctSelect && !(select.IsGrouped | select.IsAggregated)))
                {
                    int[] numArray = new int[this.ColumnCount];
                    for (int j = 0; j < this.ColumnCount; j++)
                    {
                        Expression leftNode = this.ExprList[j].GetLeftNode();
                        if (leftNode.GetExprType() != 2)
                        {
                            return;
                        }
                        if (leftNode.GetRangeVariable() != select.RangeVariables[0])
                        {
                            return;
                        }
                        numArray[j] = leftNode.ColumnIndex;
                    }
                    this._columnIndexes = numArray;
                }
            }
        }

        public bool PrepareSpecial(Session session, QuerySpecification select)
        {
            Expression leftNode = select.ExprColumns[select.IndexStartAggregates];
            int exprType = leftNode.GetExprType();
            leftNode = leftNode.GetLeftNode();
            if (leftNode.GetExprType() != 2)
            {
                return false;
            }
            if (leftNode.GetRangeVariable() != select.RangeVariables[0])
            {
                return false;
            }
            Index sortIndex = select.RangeVariables[0].GetSortIndex();
            if (sortIndex == null)
            {
                return false;
            }
            int[] columns = sortIndex.GetColumns();
            if (select.RangeVariables[0].HasIndexCondition())
            {
                if (columns[0] != leftNode.GetColumnIndex())
                {
                    return false;
                }
                if (exprType == 0x4a)
                {
                    select.RangeVariables[0].ReverseOrder();
                }
            }
            else
            {
                Index indexForColumn = select.RangeVariables[0].GetTable().GetIndexForColumn(session, leftNode.GetColumnIndex());
                if (indexForColumn == null)
                {
                    return false;
                }
                if (!select.RangeVariables[0].SetSortIndex(indexForColumn, exprType == 0x4a))
                {
                    return false;
                }
            }
            this.ColumnCount = 1;
            this.SortOrder = new int[this.ColumnCount];
            this.SortDescending = new bool[this.ColumnCount];
            this.SortNullsLast = new bool[this.ColumnCount];
            this._columnIndexes = new int[this.ColumnCount];
            this._columnIndexes[0] = leftNode.ColumnIndex;
            this.SkipSort = true;
            this.SkipFullResult = true;
            return true;
        }

        public void SetIndex(Session session, TableBase table)
        {
            try
            {
                this.index = table.CreateAndAddIndexStructure(session, null, this.SortOrder, this.SortDescending, this.SortNullsLast, false, false, false);
            }
            catch (Exception)
            {
                throw Error.RuntimeError(0xc9, "SortAndSlice");
            }
        }

        public void SetSortRange(QuerySpecification select)
        {
            if (!this.IsGenerated)
            {
                if (this.ColumnCount == 0)
                {
                    if (((this.LimitCondition != null) && !select.IsDistinctSelect) && (!select.IsGrouped && !select.IsAggregated))
                    {
                        this.SkipFullResult = true;
                    }
                }
                else
                {
                    for (int i = 0; i < this.ColumnCount; i++)
                    {
                        SqlType dataType = this.ExprList[i].GetLeftNode().GetDataType();
                        if (dataType.IsArrayType() || dataType.IsLobType())
                        {
                            throw Error.GetError(0x159e);
                        }
                    }
                    if (this._columnIndexes != null)
                    {
                        Index sortIndex = select.RangeVariables[0].GetSortIndex();
                        if (sortIndex != null)
                        {
                            int[] columns = sortIndex.GetColumns();
                            int num2 = ArrayUtil.CountTrueElements(this.SortDescending);
                            bool reversed = num2 == this.ColumnCount;
                            if (reversed || (num2 <= 0))
                            {
                                if (!select.RangeVariables[0].HasIndexCondition())
                                {
                                    Index fullIndexForColumns = select.RangeVariables[0].GetTable().GetFullIndexForColumns(this._columnIndexes);
                                    if ((fullIndexForColumns != null) && select.RangeVariables[0].SetSortIndex(fullIndexForColumns, reversed))
                                    {
                                        this.SkipSort = true;
                                        this.SkipFullResult = true;
                                    }
                                }
                                else if (ArrayUtil.HaveEqualArrays(this._columnIndexes, columns, this._columnIndexes.Length) && (!reversed || select.RangeVariables[0].ReverseOrder()))
                                {
                                    this.SkipSort = true;
                                    this.SkipFullResult = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}


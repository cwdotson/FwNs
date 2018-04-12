namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class FilterExpressionGroup
    {
        private List<object> _expressions = new List<object>();

        public void Add(BoolOpEnum boolOp, object searchExpressionOrGroup)
        {
            if (searchExpressionOrGroup is FilterExpression)
            {
                ((FilterExpression) searchExpressionOrGroup).BoolOp = boolOp;
            }
            else if (searchExpressionOrGroup is FilterExpressionGroup)
            {
                ((FilterExpressionGroup) searchExpressionOrGroup).BoolOp = boolOp;
            }
            this._expressions.Add(searchExpressionOrGroup);
        }

        public static FilterExpressionGroup Parse(string expression)
        {
            FilterExpressionGroup parentSrchExpGrp = null;
            if (expression != null)
            {
                parentSrchExpGrp = new FilterExpressionGroup();
                int pos = 0;
                parseExpression(expression, ref pos, parentSrchExpGrp);
                while (parentSrchExpGrp.Expressions.Count == 1)
                {
                    FilterExpressionGroup group2 = parentSrchExpGrp.Expressions[0] as FilterExpressionGroup;
                    if (group2 == null)
                    {
                        return parentSrchExpGrp;
                    }
                    parentSrchExpGrp.Expressions.Clear();
                    parentSrchExpGrp = group2;
                }
            }
            return parentSrchExpGrp;
        }

        private static void parseExpression(string filter, ref int pos, FilterExpressionGroup parentSrchExpGrp)
        {
            ParseState left = ParseState.Left;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            string fieldName = null;
            object searchVal = null;
            StringBuilder builder = new StringBuilder();
            EqualityEnum equal = EqualityEnum.Equal;
            MatchTypeEnum useCase = MatchTypeEnum.UseCase;
            BoolOpEnum and = BoolOpEnum.And;
            int startIndex = pos;
            while (pos < filter.Length)
            {
                if (!char.IsWhiteSpace(filter[pos]))
                {
                    break;
                }
                pos++;
            }
            while (pos < filter.Length)
            {
                switch (left)
                {
                    case ParseState.Left:
                    {
                        if (filter[pos] == '[')
                        {
                            flag = true;
                            pos++;
                            startIndex = pos;
                        }
                        if (flag)
                        {
                            if (filter[pos] == ']')
                            {
                                fieldName = filter.Substring(startIndex, pos - startIndex).Trim();
                                pos++;
                            }
                        }
                        else if (filter[pos] == '(')
                        {
                            pos++;
                            FilterExpressionGroup searchExpressionOrGroup = new FilterExpressionGroup();
                            parentSrchExpGrp.Add(and, searchExpressionOrGroup);
                            parseExpression(filter, ref pos, searchExpressionOrGroup);
                            left = ParseState.BoolOp;
                        }
                        else if (char.IsWhiteSpace(filter[pos]) || ((!char.IsLetterOrDigit(filter[pos]) && (filter[pos] != '_')) && (filter[pos] != '~')))
                        {
                            fieldName = filter.Substring(startIndex, pos - startIndex).Trim();
                        }
                        if (fieldName != null)
                        {
                            if (fieldName[0] == '~')
                            {
                                fieldName = fieldName.Substring(1);
                                useCase = MatchTypeEnum.IgnoreCase;
                            }
                            left = ParseState.CompareOp;
                        }
                        else
                        {
                            pos++;
                        }
                        continue;
                    }
                    case ParseState.CompareOp:
                    {
                        while ((pos < filter.Length) && char.IsWhiteSpace(filter[pos]))
                        {
                            pos++;
                        }
                        if (char.IsLetter(filter[pos]))
                        {
                            if ((pos + 4) >= filter.Length)
                            {
                                throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                            }
                            if (((char.ToUpper(filter[pos]) == 'N') && (char.ToUpper(filter[pos + 1]) == 'O')) && ((char.ToUpper(filter[pos + 2]) == 'T') && char.IsWhiteSpace(filter[pos + 3])))
                            {
                                pos += 3;
                                flag3 = true;
                                continue;
                            }
                            if (((char.ToUpper(filter[pos]) == 'I') && (char.ToUpper(filter[pos + 1]) == 'N')) && (char.IsWhiteSpace(filter[pos + 2]) || (filter[pos + 2] == '(')))
                            {
                                pos += 2;
                                if (char.IsWhiteSpace(filter[pos]))
                                {
                                    while ((pos < filter.Length) && char.IsWhiteSpace(filter[pos]))
                                    {
                                        pos++;
                                    }
                                    if (filter[pos] != '(')
                                    {
                                        throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos - 4)), FileDbExceptionsEnum.InvalidFilterConstruct);
                                    }
                                }
                                equal = flag3 ? EqualityEnum.NotIn : EqualityEnum.In;
                            }
                            else
                            {
                                if (((char.ToUpper(filter[pos]) != 'L') || (char.ToUpper(filter[pos + 1]) != 'I')) || (((char.ToUpper(filter[pos + 2]) != 'K') || (char.ToUpper(filter[pos + 3]) != 'E')) || !char.IsWhiteSpace(filter[pos + 4])))
                                {
                                    throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos - 4)), FileDbExceptionsEnum.InvalidFilterConstruct);
                                }
                                pos += 4;
                                equal = flag3 ? EqualityEnum.NotLike : EqualityEnum.Like;
                            }
                        }
                        else if (filter[pos] == '!')
                        {
                            int num2 = pos + 1;
                            pos = num2;
                            if (num2 >= filter.Length)
                            {
                                throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                            }
                            if (filter[pos] != '=')
                            {
                                throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                            }
                            equal = EqualityEnum.NotEqual;
                        }
                        else if (filter[pos] == '=')
                        {
                            equal = EqualityEnum.Equal;
                        }
                        else if (filter[pos] == '<')
                        {
                            if ((pos + 1) >= filter.Length)
                            {
                                throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                            }
                            if (filter[pos + 1] == '>')
                            {
                                pos++;
                                equal = EqualityEnum.NotEqual;
                            }
                            else if (filter[pos + 1] == '=')
                            {
                                pos++;
                                equal = EqualityEnum.LessThanOrEqual;
                            }
                            else
                            {
                                equal = EqualityEnum.LessThan;
                            }
                        }
                        else
                        {
                            if (filter[pos] != '>')
                            {
                                throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                            }
                            if ((pos + 1) >= filter.Length)
                            {
                                throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                            }
                            if (filter[pos + 1] == '=')
                            {
                                pos++;
                                equal = EqualityEnum.GreaterThanOrEqual;
                            }
                            else
                            {
                                equal = EqualityEnum.GreaterThan;
                            }
                        }
                        pos++;
                        left = ParseState.Right;
                        continue;
                    }
                    default:
                        if (left != ParseState.Right)
                        {
                            goto Label_0681;
                        }
                        switch (equal)
                        {
                            case EqualityEnum.In:
                            case EqualityEnum.NotIn:
                                goto Label_05EB;
                        }
                        if (flag2)
                        {
                            if (filter[pos] == '\'')
                            {
                                if (((pos + 1) >= filter.Length) || (filter[pos + 1] != '\''))
                                {
                                    flag2 = false;
                                    searchVal = builder.ToString();
                                    builder.Length = 0;
                                    FilterExpression searchExpressionOrGroup = new FilterExpression(fieldName, searchVal, equal, useCase);
                                    parentSrchExpGrp.Add(and, searchExpressionOrGroup);
                                    fieldName = null;
                                    left = ParseState.BoolOp;
                                    goto Label_0676;
                                }
                                pos++;
                            }
                            builder.Append(filter[pos]);
                        }
                        else
                        {
                            if (builder.Length == 0)
                            {
                                while ((pos < filter.Length) && char.IsWhiteSpace(filter[pos]))
                                {
                                    pos++;
                                }
                            }
                            if ((builder.Length > 0) && ((filter[pos] == ')') || char.IsWhiteSpace(filter[pos])))
                            {
                                searchVal = builder.ToString();
                                builder.Length = 0;
                                FilterExpression searchExpressionOrGroup = new FilterExpression(fieldName, searchVal, equal, useCase);
                                parentSrchExpGrp.Add(and, searchExpressionOrGroup);
                                if (filter[pos] == ')')
                                {
                                    return;
                                }
                                fieldName = null;
                                left = ParseState.BoolOp;
                            }
                            else if ((builder.Length == 0) && (filter[pos] == '\''))
                            {
                                flag2 = true;
                            }
                            else
                            {
                                builder.Append(filter[pos]);
                            }
                        }
                        goto Label_0676;
                }
            Label_05E5:
                pos++;
            Label_05EB:
                if ((pos < filter.Length) && char.IsWhiteSpace(filter[pos]))
                {
                    goto Label_05E5;
                }
                if (filter[pos] == '(')
                {
                    pos++;
                }
                int num3 = pos;
                while ((num3 < filter.Length) && (filter[num3] != ')'))
                {
                    num3++;
                }
                if (num3 >= filter.Length)
                {
                    throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                }
                searchVal = parseInVals(filter.Substring(pos, num3 - pos), useCase);
                pos = num3;
            Label_0676:
                pos++;
                continue;
            Label_0681:
                if (builder.Length == 0)
                {
                    while ((pos < filter.Length) && char.IsWhiteSpace(filter[pos]))
                    {
                        pos++;
                    }
                }
                if (filter[pos] == ')')
                {
                    return;
                }
                if (char.IsWhiteSpace(filter[pos]))
                {
                    if (builder.Length == 0)
                    {
                        throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                    }
                    string strA = builder.ToString();
                    builder.Length = 0;
                    if (string.Compare(strA, "AND", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        and = BoolOpEnum.And;
                    }
                    else
                    {
                        if (string.Compare(strA, "OR", StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            throw new FileDbException(string.Format("Invalid Filter construct near '{0}'", filter.Substring(pos)), FileDbExceptionsEnum.InvalidFilterConstruct);
                        }
                        and = BoolOpEnum.Or;
                    }
                    left = ParseState.Left;
                    while ((pos < filter.Length) && char.IsWhiteSpace(filter[pos]))
                    {
                        pos++;
                    }
                    startIndex = pos;
                    flag = false;
                }
                else
                {
                    builder.Append(filter[pos]);
                    pos++;
                }
            }
            if (left == ParseState.Right)
            {
                if ((equal != EqualityEnum.In) && (equal != EqualityEnum.NotIn))
                {
                    searchVal = builder.ToString();
                    builder.Length = 0;
                }
                FilterExpression searchExpressionOrGroup = new FilterExpression(fieldName, searchVal, equal, useCase);
                parentSrchExpGrp.Add(and, searchExpressionOrGroup);
            }
        }

        private static HashSet<object> parseInVals(string inVals, MatchTypeEnum matchType)
        {
            HashSet<object> set = new HashSet<object>();
            bool flag = false;
            bool flag2 = false;
            StringBuilder builder = new StringBuilder(100);
            int num = 0;
            while (num < inVals.Length)
            {
                if (!char.IsWhiteSpace(inVals[num]))
                {
                    break;
                }
                num++;
            }
            while (num < inVals.Length)
            {
                char ch = inVals[num];
                if (ch == '\'')
                {
                    if (flag)
                    {
                        if ((num < (inVals.Length - 1)) && (inVals[num + 1] == '\''))
                        {
                            num++;
                            goto Label_00C5;
                        }
                        flag = false;
                    }
                    else
                    {
                        flag2 = flag = true;
                    }
                }
                else
                {
                    if ((ch != ',') | flag)
                    {
                        goto Label_00C5;
                    }
                    string item = builder.ToString();
                    if (flag2)
                    {
                        if (matchType == MatchTypeEnum.IgnoreCase)
                        {
                            item = item.ToUpper();
                        }
                    }
                    else
                    {
                        item = item.Trim();
                    }
                    set.Add(item);
                    builder.Length = 0;
                }
            Label_00BD:
                num++;
                continue;
            Label_00C5:
                if ((ch != ' ') | flag)
                {
                    builder.Append(ch);
                }
                goto Label_00BD;
            }
            if (builder.Length > 0)
            {
                string item = builder.ToString();
                if (flag2)
                {
                    if (matchType == MatchTypeEnum.IgnoreCase)
                    {
                        item = item.ToUpper();
                    }
                }
                else
                {
                    item = item.Trim();
                }
                set.Add(item);
            }
            return set;
        }

        internal BoolOpEnum BoolOp { get; set; }

        public List<object> Expressions
        {
            get
            {
                return this._expressions;
            }
        }

        internal enum ParseState
        {
            Left,
            Right,
            CompareOp,
            BoolOp
        }
    }
}


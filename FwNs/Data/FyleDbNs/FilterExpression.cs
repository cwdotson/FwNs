namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class FilterExpression
    {
        public FilterExpression(string fieldName, object searchVal, EqualityEnum equality) : this(fieldName, searchVal, equality, MatchTypeEnum.UseCase)
        {
        }

        public FilterExpression(string fieldName, object searchVal, EqualityEnum equality, MatchTypeEnum matchType)
        {
            this.FieldName = fieldName;
            this.SearchVal = searchVal;
            this.MatchType = matchType;
            this.Equality = equality;
        }

        public static FilterExpression CreateInExpressionFromTable(string fieldName, Table table, string fieldNameInTable)
        {
            if (!table.Fields.ContainsKey(fieldNameInTable))
            {
                throw new Exception(string.Format("Field {0} is not in the table", fieldNameInTable));
            }
            HashSet<object> searchVal = new HashSet<object>();
            using (List<Record>.Enumerator enumerator = table.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    object item = enumerator.Current[fieldNameInTable];
                    searchVal.Add(item);
                }
            }
            return new FilterExpression(fieldName, searchVal, EqualityEnum.In);
        }

        public static FilterExpression CreateInExpressionFromTable<T>(string fieldName, IList<T> list, string propertyName)
        {
            HashSet<object> searchVal = new HashSet<object>();
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                throw new Exception(string.Format("Field {0} is not a property of {1}", propertyName, type.Name));
            }
            foreach (T local in list)
            {
                object item = property.GetValue(local, null);
                searchVal.Add(item);
            }
            return new FilterExpression(fieldName, searchVal, EqualityEnum.In);
        }

        public static FilterExpression Parse(string expression)
        {
            FilterExpression expression2 = null;
            FilterExpressionGroup group = FilterExpressionGroup.Parse(expression);
            if (group != null)
            {
                expression2 = group.Expressions[0] as FilterExpression;
            }
            return expression2;
        }

        internal BoolOpEnum BoolOp { get; set; }

        public string FieldName { get; set; }

        public object SearchVal { get; set; }

        public MatchTypeEnum MatchType { get; set; }

        public EqualityEnum Equality { get; set; }
    }
}


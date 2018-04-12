namespace FwNs.Core.LC.cParsing
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Text;

    public sealed class Token
    {
        public int TokenType = -1;
        private object _expression;
        public string TokenString = "";
        public string CharsetName;
        public string CharsetSchema;
        public ExpressionColumn ColumnExpression;
        public SqlType DataType;
        public string FullString;
        public bool IsCoreReservedIdentifier;
        public bool IsDelimitedIdentifier;
        public bool IsDelimitedPrePrePrefix;
        public bool IsDelimitedPrePrefix;
        public bool IsDelimitedPrefix;
        public bool IsDelimiter;
        public bool IsHostParameter;
        public bool IsMalformed;
        public bool IsReservedIdentifier;
        public bool IsUndelimitedIdentifier;
        public int LobMultiplierType = -1;
        public string NamePrePrePrefix;
        public string NamePrePrefix;
        public string NamePrefix;
        public int Position;
        public object TokenValue;

        public Token Duplicate()
        {
            return new Token { 
                TokenString = this.TokenString,
                TokenType = this.TokenType,
                DataType = this.DataType,
                TokenValue = this.TokenValue,
                NamePrefix = this.NamePrefix,
                NamePrePrefix = this.NamePrePrefix,
                NamePrePrePrefix = this.NamePrePrePrefix,
                CharsetSchema = this.CharsetSchema,
                CharsetName = this.CharsetName,
                FullString = this.FullString,
                LobMultiplierType = this.LobMultiplierType,
                IsDelimiter = this.IsDelimiter,
                IsDelimitedIdentifier = this.IsDelimitedIdentifier,
                IsDelimitedPrefix = this.IsDelimitedPrefix,
                IsDelimitedPrePrefix = this.IsDelimitedPrePrefix,
                IsDelimitedPrePrePrefix = this.IsDelimitedPrePrePrefix,
                IsUndelimitedIdentifier = this.IsUndelimitedIdentifier,
                IsReservedIdentifier = this.IsReservedIdentifier,
                IsCoreReservedIdentifier = this.IsCoreReservedIdentifier,
                IsHostParameter = this.IsHostParameter,
                IsMalformed = this.IsMalformed
            };
        }

        public string GetFullString()
        {
            return this.FullString;
        }

        public string GetSql()
        {
            ExpressionColumn column = this._expression as ExpressionColumn;
            if (column != null)
            {
                if (this.TokenType == 0x2a9)
                {
                    StringBuilder builder = new StringBuilder();
                    if (((column == null) || (column.OpType != 0x61)) || (column.nodes.Length == 0))
                    {
                        return this.TokenString;
                    }
                    builder.Append(' ');
                    for (int i = 0; i < column.nodes.Length; i++)
                    {
                        Expression expression = column.nodes[i];
                        ColumnSchema schema = expression.GetColumn();
                        if (expression.OpType == 3)
                        {
                            if (i > 0)
                            {
                                builder.Append(',');
                            }
                            builder.Append(expression.GetColumnName());
                        }
                        else
                        {
                            string schemaQualifiedStatementName;
                            if (expression.GetRangeVariable().TableAlias == null)
                            {
                                schemaQualifiedStatementName = schema.GetName().GetSchemaQualifiedStatementName();
                            }
                            else
                            {
                                schemaQualifiedStatementName = expression.GetRangeVariable().TableAlias.GetStatementName() + "." + schema.GetName().StatementName;
                            }
                            if (i > 0)
                            {
                                builder.Append(',');
                            }
                            builder.Append(schemaQualifiedStatementName);
                        }
                    }
                    builder.Append(' ');
                    return builder.ToString();
                }
            }
            else
            {
                SqlType type = this._expression as SqlType;
                if (type != null)
                {
                    this.IsDelimiter = false;
                    if (!type.IsDistinctType() && !type.IsDomainType())
                    {
                        return type.GetNameString();
                    }
                    return type.GetName().GetSchemaQualifiedStatementName();
                }
                ISchemaObject obj2 = this._expression as ISchemaObject;
                if (obj2 != null)
                {
                    this.IsDelimiter = false;
                    return obj2.GetName().GetSchemaQualifiedStatementName();
                }
            }
            if ((this.NamePrefix == null) && this.IsUndelimitedIdentifier)
            {
                return this.TokenString;
            }
            if (this.TokenType == 0x2e9)
            {
                return this.DataType.ConvertToSQLString(this.TokenValue);
            }
            StringBuilder builder2 = new StringBuilder();
            if (this.NamePrePrefix != null)
            {
                if (this.IsDelimitedPrePrefix)
                {
                    builder2.Append('"');
                    builder2.Append(this.NamePrePrefix);
                    builder2.Append('"');
                }
                else
                {
                    builder2.Append(this.NamePrePrefix);
                    this.IsDelimiter = false;
                }
                builder2.Append('.');
            }
            if (this.NamePrefix != null)
            {
                if (this.IsDelimitedPrefix)
                {
                    builder2.Append('"');
                    builder2.Append(this.NamePrefix);
                    builder2.Append('"');
                }
                else
                {
                    builder2.Append(this.NamePrefix);
                    this.IsDelimiter = false;
                }
                builder2.Append('.');
            }
            if (this.IsDelimitedIdentifier)
            {
                builder2.Append('"');
                builder2.Append(this.TokenString);
                builder2.Append('"');
                builder2.Append(' ');
            }
            else
            {
                builder2.Append(this.TokenString);
            }
            return builder2.ToString();
        }

        public static string GetSql(Token[] statement)
        {
            bool isDelimiter = true;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < statement.Length; i++)
            {
                string sql = statement[i].GetSql();
                if (!statement[i].IsDelimiter && !isDelimiter)
                {
                    builder.Append(' ');
                }
                builder.Append(sql);
                isDelimiter = statement[i].IsDelimiter;
            }
            return builder.ToString();
        }

        public void Reset()
        {
            this.TokenString = "";
            this.TokenType = -1;
            this.DataType = null;
            this.TokenValue = this._expression = null;
            this.NamePrefix = this.NamePrePrefix = this.NamePrePrePrefix = this.CharsetSchema = this.CharsetName = (string) (this.FullString = null);
            this.LobMultiplierType = -1;
            this.IsDelimiter = this.IsDelimitedIdentifier = this.IsDelimitedPrefix = this.IsDelimitedPrePrefix = this.IsDelimitedPrePrePrefix = this.IsUndelimitedIdentifier = this.IsReservedIdentifier = this.IsCoreReservedIdentifier = this.IsHostParameter = this.IsMalformed = false;
        }

        public void SetExpression(object expression)
        {
            this._expression = expression;
        }
    }
}


namespace FwNs.Core.LC.cTables
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Text;

    public sealed class ColumnSchema : ColumnBase, ISchemaObject
    {
        public static ColumnSchema[] EmptyArray = new ColumnSchema[0];
        public QNameManager.QName ColumnName;
        private bool _isPrimaryKey;
        private Expression _defaultExpression;
        private Expression _generatingExpression;
        private NumberSequence _sequence;
        private readonly OrderedHashSet<QNameManager.QName> _references = new OrderedHashSet<QNameManager.QName>();
        private Expression _accessor;
        public bool HasDefault;

        public ColumnSchema(QNameManager.QName name, SqlType type, bool isNullable, bool isPrimaryKey, Expression defaultExpression)
        {
            this.ColumnName = name;
            base.Nullability = isNullable ? ((byte) 1) : ((byte) 0);
            base.DataType = type;
            this._isPrimaryKey = isPrimaryKey;
            this._defaultExpression = defaultExpression;
            this.HasDefault = defaultExpression > null;
            this.SetReferences();
        }

        public void Compile(Session session, ISchemaObject table)
        {
            if (this._generatingExpression != null)
            {
                this._generatingExpression.ResetColumnReferences();
                this._generatingExpression.ResolveCheckOrGenExpression(session, ((Table) table).DefaultRanges, false);
                if (base.DataType.TypeComparisonGroup != this._generatingExpression.GetDataType().TypeComparisonGroup)
                {
                    throw Error.GetError(0x15b9);
                }
                this.SetReferences();
            }
        }

        public ColumnSchema Duplicate()
        {
            ColumnSchema schema1 = new ColumnSchema(this.ColumnName, base.DataType, true, this._isPrimaryKey, this._defaultExpression);
            schema1.SetNullability(base.Nullability);
            schema1.SetGeneratingExpression(this._generatingExpression);
            schema1.SetIdentity(this._sequence);
            return schema1;
        }

        public Expression GetAccessor()
        {
            Expression expression = this._accessor;
            if (expression == null)
            {
                expression = this._accessor = new ExpressionColumnAccessor(this);
            }
            return expression;
        }

        public QNameManager.QName GetCatalogName()
        {
            if (this.ColumnName.schema != null)
            {
                return this.ColumnName.schema.schema;
            }
            return null;
        }

        public override string GetCatalogNameString()
        {
            if ((this.ColumnName.schema != null) && (this.ColumnName.schema.schema != null))
            {
                return this.ColumnName.schema.schema.Name;
            }
            return null;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public Expression GetDefaultExpression()
        {
            if (this._defaultExpression != null)
            {
                return this._defaultExpression;
            }
            if (base.DataType.IsDomainType())
            {
                return base.DataType.userTypeModifier.GetDefaultClause();
            }
            return null;
        }

        public string GetDefaultSql()
        {
            if (this._defaultExpression != null)
            {
                return this._defaultExpression.GetSql();
            }
            return null;
        }

        public object GetDefaultValue(Session session)
        {
            if (this._defaultExpression != null)
            {
                return this._defaultExpression.GetValue(session, base.DataType);
            }
            return null;
        }

        public object GetGeneratedValue(Session session)
        {
            if (this._generatingExpression != null)
            {
                return this._generatingExpression.GetValue(session, base.DataType);
            }
            return null;
        }

        public Expression GetGeneratingExpression()
        {
            return this._generatingExpression;
        }

        public NumberSequence GetIdentitySequence()
        {
            return this._sequence;
        }

        public QNameManager.QName GetName()
        {
            return this.ColumnName;
        }

        public override string GetNameString()
        {
            return this.ColumnName.Name;
        }

        public override byte GetNullability()
        {
            if (!this._isPrimaryKey)
            {
                return base.GetNullability();
            }
            return 0;
        }

        public Grantee GetOwner()
        {
            if (this.ColumnName.schema != null)
            {
                return this.ColumnName.schema.Owner;
            }
            return null;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return this._references;
        }

        public QNameManager.QName GetSchemaName()
        {
            return this.ColumnName.schema;
        }

        public override string GetSchemaNameString()
        {
            if (this.ColumnName.schema != null)
            {
                return this.ColumnName.schema.Name;
            }
            return null;
        }

        public int GetSchemaObjectType()
        {
            return this.ColumnName.type;
        }

        public string GetSql()
        {
            StringBuilder builder = new StringBuilder();
            switch (base.ParameterMode)
            {
                case 1:
                    builder.Append("IN").Append(' ');
                    break;

                case 2:
                    builder.Append("INOUT").Append(' ');
                    break;

                case 4:
                    builder.Append("OUT").Append(' ');
                    break;
            }
            if (this.ColumnName != null)
            {
                builder.Append(this.ColumnName.StatementName);
                builder.Append(' ');
            }
            builder.Append(base.DataType.GetTypeDefinition());
            return builder.ToString();
        }

        public override string GetTableNameString()
        {
            if (this.ColumnName.Parent != null)
            {
                return this.ColumnName.Parent.Name;
            }
            return null;
        }

        public bool IsGenerated()
        {
            return (this._generatingExpression > null);
        }

        public override bool IsNullable()
        {
            bool flag = base.IsNullable();
            if (flag && base.DataType.IsDomainType())
            {
                return base.DataType.userTypeModifier.IsNullable();
            }
            return flag;
        }

        public bool IsPrimaryKey()
        {
            return this._isPrimaryKey;
        }

        public override bool IsSearchable()
        {
            return Types.IsSearchable(base.DataType.TypeCode);
        }

        public bool IsUserVariable()
        {
            return ((this.ColumnName.type == 0x19) && this.ColumnName.Name.StartsWith("@"));
        }

        public override bool IsWriteable()
        {
            return !this.IsGenerated();
        }

        public void SetDefaultExpression(Expression expr)
        {
            this._defaultExpression = expr;
            this.HasDefault = expr > null;
        }

        public void SetGeneratingExpression(Expression expr)
        {
            this._generatingExpression = expr;
        }

        public void SetIdentity(NumberSequence sequence)
        {
            this._sequence = sequence;
            base._isIdentity = sequence > null;
        }

        public void SetName(QNameManager.QName name)
        {
            this.ColumnName = name;
        }

        public void SetPrimaryKey(bool value)
        {
            this._isPrimaryKey = value;
        }

        private void SetReferences()
        {
            this._references.Clear();
            if (base.DataType.IsDomainType() || base.DataType.IsDistinctType())
            {
                QNameManager.QName key = base.DataType.GetName();
                this._references.Add(key);
            }
            if (this._generatingExpression != null)
            {
                this._generatingExpression.CollectObjectNames(this._references);
                Iterator<QNameManager.QName> iterator = this._references.GetIterator();
                while (iterator.HasNext())
                {
                    QNameManager.QName name2 = iterator.Next();
                    if ((name2.type == 9) || (name2.type == 3))
                    {
                        iterator.Remove();
                    }
                }
            }
        }

        public override void SetType(SqlType type)
        {
            base.DataType = type;
            this.SetReferences();
        }

        public void SetType(ColumnSchema other)
        {
            base.Nullability = other.Nullability;
            base.DataType = other.DataType;
        }

        public override void SetWriteable(bool value)
        {
            throw Error.RuntimeError(0xc9, "ColumnSchema");
        }
    }
}


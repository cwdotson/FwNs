namespace FwNs.Core.LC.cDataTypes
{
    using FwNs.Core.LC.cConstraints;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cExpressions;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Text;

    public sealed class UserTypeModifier
    {
        private readonly QNameManager.QName _name;
        private readonly int _schemaObjectType;
        private readonly SqlType _dataType;
        private Constraint[] _constraints = Constraint.EmptyArray;
        private Expression _defaultExpression;
        private bool _isNullable = true;

        public UserTypeModifier(QNameManager.QName name, int type, SqlType dataType)
        {
            this._name = name;
            this._schemaObjectType = type;
            this._dataType = dataType;
        }

        public void AddConstraint(Constraint c)
        {
            int length = this._constraints.Length;
            this._constraints = ArrayUtil.ResizeArray<Constraint>(this._constraints, length + 1);
            this._constraints[length] = c;
            this.SetNotNull();
        }

        public void Compile(Session session)
        {
            for (int i = 0; i < this._constraints.Length; i++)
            {
                this._constraints[i].Compile(session, null);
            }
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            if (this._constraints == null)
            {
                return null;
            }
            OrderedHashSet<ISchemaObject> set1 = new OrderedHashSet<ISchemaObject>();
            set1.AddAll(this._constraints);
            return set1;
        }

        public Constraint GetConstraint(string name)
        {
            for (int i = 0; i < this._constraints.Length; i++)
            {
                if (this._constraints[i].GetName().Name.Equals(name))
                {
                    return this._constraints[i];
                }
            }
            return null;
        }

        public Constraint[] GetConstraints()
        {
            return this._constraints;
        }

        public Expression GetDefaultClause()
        {
            return this._defaultExpression;
        }

        public QNameManager.QName GetName()
        {
            return this._name;
        }

        public Grantee GetOwner()
        {
            return this._name.schema.Owner;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            for (int i = 0; i < this._constraints.Length; i++)
            {
                OrderedHashSet<QNameManager.QName> references = this._constraints[i].GetReferences();
                if (references != null)
                {
                    set.AddAll(references);
                }
            }
            return set;
        }

        public QNameManager.QName GetSchemaName()
        {
            return this._name.schema;
        }

        public int GetSchemaObjectType()
        {
            return this._schemaObjectType;
        }

        public string GetSQL()
        {
            StringBuilder builder = new StringBuilder();
            if (this._schemaObjectType == 12)
            {
                builder.Append("CREATE").Append(' ').Append("TYPE").Append(' ');
                builder.Append(this._name.GetSchemaQualifiedStatementName());
                builder.Append(' ').Append("AS").Append(' ');
                builder.Append(this._dataType.GetDefinition());
            }
            else
            {
                builder.Append("CREATE").Append(' ').Append("DOMAIN").Append(' ');
                builder.Append(this._name.GetSchemaQualifiedStatementName());
                builder.Append(' ').Append("AS").Append(' ');
                builder.Append(this._dataType.GetDefinition());
                if (this._defaultExpression != null)
                {
                    builder.Append(' ').Append("DEFAULT").Append(' ');
                    builder.Append(this._defaultExpression.GetSql());
                }
                for (int i = 0; i < this._constraints.Length; i++)
                {
                    builder.Append(' ').Append("CONSTRAINT").Append(' ');
                    builder.Append(this._constraints[i].GetName().StatementName).Append(' ');
                    builder.Append("CHECK").Append('(').Append(this._constraints[i].GetCheckSql()).Append(')');
                }
            }
            return builder.ToString();
        }

        public bool IsNullable()
        {
            return this._isNullable;
        }

        public void RemoveConstraint(string name)
        {
            for (int i = 0; i < this._constraints.Length; i++)
            {
                if (this._constraints[i].GetName().Name.Equals(name))
                {
                    this._constraints = ArrayUtil.ToAdjustedArray<Constraint>(this._constraints, null, i, -1);
                    break;
                }
            }
            this.SetNotNull();
        }

        public void RemoveDefaultClause()
        {
            this._defaultExpression = null;
        }

        public int SchemaObjectType()
        {
            return this._schemaObjectType;
        }

        public void SetDefaultClause(Expression defaultExpression)
        {
            this._defaultExpression = defaultExpression;
        }

        private void SetNotNull()
        {
            this._isNullable = true;
            for (int i = 0; i < this._constraints.Length; i++)
            {
                if (this._constraints[i].IsNotNull())
                {
                    this._isNullable = false;
                }
            }
        }
    }
}


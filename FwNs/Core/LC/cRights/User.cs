namespace FwNs.Core.LC.cRights
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cSchemas;
    using System;
    using System.Text;

    public sealed class User : Grantee
    {
        private string _password;
        private QNameManager.QName _initialSchema;

        public User(QNameManager.QName name, GranteeManager manager) : base(name, manager)
        {
            if (manager != null)
            {
                base.UpdateAllRights();
            }
        }

        public void CheckPassword(string value)
        {
            if (!value.Equals(this._password))
            {
                throw Error.GetError(0xfa0);
            }
        }

        public override long GetChangeTimestamp()
        {
            return 0L;
        }

        public string GetConnectUserSQL()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("SET").Append(' ');
            builder1.Append("SESSION").Append(' ');
            builder1.Append("AUTHORIZATION").Append(' ');
            builder1.Append(StringConverter.ToQuotedString(base.GetNameString(), '\'', true));
            return builder1.ToString();
        }

        public QNameManager.QName GetInitialOrDefaultSchema()
        {
            if (this._initialSchema != null)
            {
                return this._initialSchema;
            }
            QNameManager.QName name2 = base.granteeManager.database.schemaManager.FindSchemaQName(base.GetNameString());
            if (name2 == null)
            {
                return base.granteeManager.database.schemaManager.GetDefaultSchemaQName();
            }
            return name2;
        }

        public QNameManager.QName GetInitialSchema()
        {
            return this._initialSchema;
        }

        public string GetInitialSchemaSQL()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("ALTER").Append(' ');
            builder1.Append("USER").Append(' ');
            builder1.Append(base.GetStatementName()).Append(' ');
            builder1.Append("SET").Append(' ');
            builder1.Append("INITIAL").Append(' ');
            builder1.Append("SCHEMA").Append(' ');
            builder1.Append(this._initialSchema.StatementName);
            return builder1.ToString();
        }

        public override string GetSql()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("CREATE").Append(' ').Append("USER");
            builder1.Append(' ').Append(base.GranteeName.StatementName).Append(' ');
            builder1.Append("IDENTIFIED").Append(' ').Append("BY").Append(' ');
            builder1.Append('\'').Append(this._password).Append('\'');
            return builder1.ToString();
        }

        public void SetInitialSchema(QNameManager.QName schema)
        {
            this._initialSchema = schema;
        }

        public void SetPassword(string password)
        {
            this._password = password;
        }
    }
}


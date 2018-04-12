namespace FwNs.Core.LC.cStatements
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cSchemas;
    using System;

    public abstract class Statement
    {
        public const int MetaResetViews = 1;
        public const int MetaResetStatements = 2;
        public static readonly Statement[] EmptyArray = new Statement[0];
        private bool _isError;
        protected bool isLogged;
        protected bool isTransactionStatement;
        protected bool isValid;
        public long CompileTimestamp;
        public int Group;
        protected long Id;
        public bool IsExplain;
        protected int MetaDataImpact;
        public StatementCompound Parent;
        public QNameManager.QName[] ReadTableNames;
        public OrderedHashSet<QNameManager.QName> References;
        public Routine Root;
        public QNameManager.QName SchemaName;
        public string Sql;
        public int type;
        public QNameManager.QName[] WriteTableNames;
        public QNameManager.QName Label;

        protected Statement(int type)
        {
            this.isLogged = true;
            this.isValid = true;
            this.ReadTableNames = QNameManager.QName.EmptyArray;
            this.References = new OrderedHashSet<QNameManager.QName>();
            this.WriteTableNames = QNameManager.QName.EmptyArray;
            this.type = type;
        }

        protected Statement(int type, int group)
        {
            this.isLogged = true;
            this.isValid = true;
            this.ReadTableNames = QNameManager.QName.EmptyArray;
            this.References = new OrderedHashSet<QNameManager.QName>();
            this.WriteTableNames = QNameManager.QName.EmptyArray;
            this.type = type;
            this.Group = group;
        }

        public virtual void ClearVariables()
        {
        }

        public abstract string Describe(Session session);
        public abstract Result Execute(Session session);
        public virtual ResultMetaData GeneratedResultMetaData()
        {
            return null;
        }

        public long GetCompileTimestamp()
        {
            return this.CompileTimestamp;
        }

        public int GetGroup()
        {
            return this.Group;
        }

        public long GetId()
        {
            return this.Id;
        }

        public virtual ResultMetaData GetParametersMetaData()
        {
            return ResultMetaData.EmptyParamMetaData;
        }

        public virtual RangeVariable[] GetRangeVariables()
        {
            return RangeVariable.EmptyArray;
        }

        public virtual OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return this.References;
        }

        public virtual ResultMetaData GetResultMetaData()
        {
            return ResultMetaData.EmptyResultMetaData;
        }

        public virtual int GetResultProperties()
        {
            return ResultProperties.DefaultPropsValue;
        }

        public virtual QNameManager.QName GetSchemaName()
        {
            return this.SchemaName;
        }

        public virtual string GetSql()
        {
            return this.Sql;
        }

        public int GetStatementType()
        {
            return this.type;
        }

        public QNameManager.QName[] GetTableNamesForRead()
        {
            return this.ReadTableNames;
        }

        public QNameManager.QName[] GetTableNamesForWrite()
        {
            return this.WriteTableNames;
        }

        public virtual bool HasGeneratedColumns()
        {
            return false;
        }

        public virtual bool IsAutoCommitStatement()
        {
            return false;
        }

        public virtual bool IsCatalogChange()
        {
            int group = this.Group;
            if ((group - 0x7d1) > 1)
            {
                if (group == 0x7e0)
                {
                    return true;
                }
                if (group != 0x7e1)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsError()
        {
            return this._isError;
        }

        public bool IsLogged()
        {
            return this.isLogged;
        }

        public bool IsTransactionStatement()
        {
            return this.isTransactionStatement;
        }

        public bool IsValid()
        {
            return this.isValid;
        }

        public virtual void Resolve(Session session)
        {
        }

        public virtual void Resolve(Session session, RangeVariable[] rangeVars)
        {
        }

        public void SetCompileTimestamp(long ts)
        {
            this.CompileTimestamp = ts;
        }

        public void SetDescribe()
        {
            this.IsExplain = true;
        }

        public virtual void SetGeneratedColumnInfo(int mode, ResultMetaData meta)
        {
        }

        public void SetId(long csid)
        {
            this.Id = csid;
        }

        public void SetIsLogged(bool val)
        {
            this.isLogged = val;
        }

        public void SetParent(StatementCompound statement)
        {
            this.Parent = statement;
        }

        public virtual void SetRoot(Routine root)
        {
            this.Root = root;
        }

        public void SetSchemaQName(QNameManager.QName name)
        {
            this.SchemaName = name;
        }

        public void SetSql(string sql)
        {
            this.Sql = sql;
        }
    }
}


namespace FwNs.Core.LC.cSchemas
{
    using FwNs.Core.LC;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class Schema : ISchemaObject
    {
        public SchemaObjectSet AggregateLookup;
        public SchemaObjectSet AssertionLookup;
        public long ChangeTimestamp;
        public SchemaObjectSet CharsetLookup;
        public SchemaObjectSet CollationLookup;
        public SchemaObjectSet ConstraintLookup;
        public SchemaObjectSet FunctionLookup;
        public SchemaObjectSet IndexLookup;
        public QNameManager.QName Name;
        public Grantee Owner;
        public SchemaObjectSet ProcedureLookup;
        public HashMappedList<string, object> SequenceList;
        public SchemaObjectSet SequenceLookup;
        public SchemaObjectSet SpecificRoutineLookup;
        public HashMappedList<string, object> TableList;
        public SchemaObjectSet TableLookup;
        public SchemaObjectSet TriggerLookup;
        public SchemaObjectSet TypeLookup;

        public Schema(QNameManager.QName name, Grantee owner)
        {
            this.Name = name;
            this.TriggerLookup = new SchemaObjectSet(8);
            this.IndexLookup = new SchemaObjectSet(20);
            this.ConstraintLookup = new SchemaObjectSet(5);
            this.TableLookup = new SchemaObjectSet(3);
            this.SequenceLookup = new SchemaObjectSet(7);
            this.TypeLookup = new SchemaObjectSet(12);
            this.CharsetLookup = new SchemaObjectSet(14);
            this.CollationLookup = new SchemaObjectSet(15);
            this.ProcedureLookup = new SchemaObjectSet(0x11);
            this.FunctionLookup = new SchemaObjectSet(0x10);
            this.AggregateLookup = new SchemaObjectSet(0x1b);
            this.SpecificRoutineLookup = new SchemaObjectSet(0x18);
            this.AssertionLookup = new SchemaObjectSet(6);
            this.TableList = (HashMappedList<string, object>) this.TableLookup.Map;
            this.SequenceList = (HashMappedList<string, object>) this.SequenceLookup.Map;
            this.Owner = owner;
            name.Owner = owner;
        }

        public void AddSimpleObjects(OrderedHashSet<object> unresolved)
        {
            Iterator<object> iterator = this.SpecificRoutineLookup.Map.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                Routine key = (Routine) iterator.Next();
                if ((key.DataImpact == 1) || (key.DataImpact == 2))
                {
                    unresolved.Add(key);
                }
            }
            unresolved.AddAll(this.TypeLookup.Map.GetValues());
            unresolved.AddAll(this.CharsetLookup.Map.GetValues());
            unresolved.AddAll(this.CollationLookup.Map.GetValues());
        }

        public void ClearStructures()
        {
            this.TableList.Clear();
            this.SequenceList.Clear();
            this.TriggerLookup = null;
            this.IndexLookup = null;
            this.ConstraintLookup = null;
            this.ProcedureLookup = null;
            this.FunctionLookup = null;
            this.SequenceLookup = null;
            this.TableLookup = null;
            this.TypeLookup = null;
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
        }

        public QNameManager.QName GetCatalogName()
        {
            return null;
        }

        public long GetChangeTimestamp()
        {
            return this.ChangeTimestamp;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public QNameManager.QName GetName()
        {
            return this.Name;
        }

        public SchemaObjectSet GetObjectSet(int type)
        {
            switch (type)
            {
                case 3:
                case 4:
                    return this.TableLookup;

                case 5:
                    return this.ConstraintLookup;

                case 6:
                    return this.AssertionLookup;

                case 7:
                    return this.SequenceLookup;

                case 8:
                    return this.TriggerLookup;

                case 12:
                case 13:
                    return this.TypeLookup;

                case 14:
                    return this.CharsetLookup;

                case 15:
                    return this.CollationLookup;

                case 0x10:
                    return this.FunctionLookup;

                case 0x11:
                    return this.ProcedureLookup;

                case 0x12:
                    return this.FunctionLookup;

                case 20:
                    return this.IndexLookup;

                case 0x18:
                    return this.SpecificRoutineLookup;
            }
            throw Error.RuntimeError(0xc9, "Schema");
        }

        public Grantee GetOwner()
        {
            return this.Owner;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return new OrderedHashSet<QNameManager.QName>();
        }

        public QNameManager.QName GetSchemaName()
        {
            return null;
        }

        public int GetSchemaObjectType()
        {
            return 2;
        }

        public string[] GetSequenceRestartSql()
        {
            List<string> list = new List<string>();
            Iterator<object> iterator = this.SequenceLookup.Map.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                string restartSql = ((NumberSequence) iterator.Next()).GetRestartSql();
                list.Add(restartSql);
            }
            return list.ToArray();
        }

        public static string GetSetSchemaSql(QNameManager.QName schemaName)
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("SET").Append(' ');
            builder1.Append("SCHEMA").Append(' ');
            builder1.Append(schemaName.StatementName);
            return builder1.ToString();
        }

        public string GetSql()
        {
            StringBuilder builder1 = new StringBuilder(0x80);
            builder1.Append("CREATE").Append(' ');
            builder1.Append("SCHEMA").Append(' ');
            builder1.Append(this.GetName().StatementName).Append(' ');
            builder1.Append("AUTHORIZATION").Append(' ');
            builder1.Append(this.GetOwner().GetStatementName());
            return builder1.ToString();
        }

        public string[] GetSqlArray(OrderedHashSet<object> resolved, OrderedHashSet<object> unresolved)
        {
            List<string> list = new List<string>();
            string setSchemaSql = GetSetSchemaSql(this.Name);
            list.Add(setSchemaSql);
            string[] sql = this.SequenceLookup.GetSql(resolved, unresolved);
            list.AddRange(sql);
            sql = this.TableLookup.GetSql(resolved, unresolved);
            list.AddRange(sql);
            sql = this.FunctionLookup.GetSql(resolved, unresolved);
            list.AddRange(sql);
            sql = this.ProcedureLookup.GetSql(resolved, unresolved);
            list.AddRange(sql);
            sql = this.AggregateLookup.GetSql(resolved, unresolved);
            list.AddRange(sql);
            sql = this.AssertionLookup.GetSql(resolved, unresolved);
            list.AddRange(sql);
            if (list.Count == 1)
            {
                return new string[0];
            }
            return list.ToArray();
        }

        public string[] GetTriggerSql()
        {
            List<string> list = new List<string>();
            Iterator<object> iterator = this.TableLookup.Map.GetValues().GetIterator();
            while (iterator.HasNext())
            {
                string[] triggerSql = ((Table) iterator.Next()).GetTriggerSql();
                list.AddRange(triggerSql);
            }
            return list.ToArray();
        }

        public bool IsEmpty()
        {
            return ((((this.SequenceLookup.IsEmpty() && this.TableLookup.IsEmpty()) && (this.TypeLookup.IsEmpty() && this.CharsetLookup.IsEmpty())) && this.CollationLookup.IsEmpty()) && this.SpecificRoutineLookup.IsEmpty());
        }

        public Iterator<object> SchemaObjectIterator(int type)
        {
            switch (type)
            {
                case 3:
                case 4:
                    return this.TableLookup.Map.GetValues().GetIterator();

                case 5:
                    return this.ConstraintLookup.Map.GetValues().GetIterator();

                case 6:
                    return this.AssertionLookup.Map.GetValues().GetIterator();

                case 7:
                    return this.SequenceLookup.Map.GetValues().GetIterator();

                case 8:
                    return this.TriggerLookup.Map.GetValues().GetIterator();

                case 12:
                case 13:
                    return this.TypeLookup.Map.GetValues().GetIterator();

                case 14:
                    return this.CharsetLookup.Map.GetValues().GetIterator();

                case 15:
                    return this.CollationLookup.Map.GetValues().GetIterator();

                case 0x10:
                    return this.FunctionLookup.Map.GetValues().GetIterator();

                case 0x11:
                    return this.ProcedureLookup.Map.GetValues().GetIterator();

                case 0x12:
                    return new WrapperIterator<object>(this.FunctionLookup.Map.GetValues().GetIterator(), this.ProcedureLookup.Map.GetValues().GetIterator());

                case 20:
                    return this.IndexLookup.Map.GetValues().GetIterator();

                case 0x18:
                    return this.SpecificRoutineLookup.Map.GetValues().GetIterator();
            }
            throw Error.RuntimeError(0xc9, "Schema");
        }
    }
}


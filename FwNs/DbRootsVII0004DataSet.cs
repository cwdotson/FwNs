namespace FwNs
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, DesignerCategory("code"), ToolboxItem(true), XmlSchemaProvider("GetTypedDataSetSchema"), XmlRoot("DbRootsVII0004DataSet"), HelpKeyword("vs.data.DataSet")]
    public class DbRootsVII0004DataSet : DataSet
    {
        private CtnrsDataTable tableCtnrs;
        private CTypsDataTable tableCTyps;
        private DbRootsDataTable tableDbRoots;
        private DrvsDataTable tableDrvs;
        private FTypsDataTable tableFTyps;
        private ItmsDataTable tableItms;
        private MchnsDataTable tableMchns;
        private PartsDataTable tableParts;
        private TblIdsDataTable tableTblIds;
        private UsrsDataTable tableUsrs;
        private WrdsDataTable tableWrds;
        private DataRelation relationCtnrs_FK00;
        private DataRelation relationCtnrs_FK01;
        private DataRelation relationCTyps_FK00;
        private DataRelation relationDrvs_FK00;
        private DataRelation relationFTyps_FK00;
        private DataRelation relationItms_FK00;
        private DataRelation relationItms_FK01;
        private DataRelation relationItms_FK02;
        private DataRelation relationMchns_FK00;
        private DataRelation relationParts_FK00;
        private DataRelation relationUsrs_FK00;
        private DataRelation relationWrds_FK00;
        private System.Data.SchemaSerializationMode _schemaSerializationMode;

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public DbRootsVII0004DataSet()
        {
            this._schemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            base.BeginInit();
            this.InitClass();
            CollectionChangeEventHandler handler = new CollectionChangeEventHandler(this.SchemaChanged);
            base.Tables.CollectionChanged += handler;
            base.Relations.CollectionChanged += handler;
            base.EndInit();
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected DbRootsVII0004DataSet(SerializationInfo info, StreamingContext context) : base(info, context, false)
        {
            this._schemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            if (base.IsBinarySerialized(info, context))
            {
                this.InitVars(false);
                CollectionChangeEventHandler handler2 = new CollectionChangeEventHandler(this.SchemaChanged);
                this.Tables.CollectionChanged += handler2;
                this.Relations.CollectionChanged += handler2;
            }
            else
            {
                string s = (string) info.GetValue("XmlSchema", typeof(string));
                if (base.DetermineSchemaSerializationMode(info, context) == System.Data.SchemaSerializationMode.IncludeSchema)
                {
                    DataSet dataSet = new DataSet();
                    dataSet.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
                    if (dataSet.Tables["Ctnrs"] != null)
                    {
                        base.Tables.Add(new CtnrsDataTable(dataSet.Tables["Ctnrs"]));
                    }
                    if (dataSet.Tables["CTyps"] != null)
                    {
                        base.Tables.Add(new CTypsDataTable(dataSet.Tables["CTyps"]));
                    }
                    if (dataSet.Tables["DbRoots"] != null)
                    {
                        base.Tables.Add(new DbRootsDataTable(dataSet.Tables["DbRoots"]));
                    }
                    if (dataSet.Tables["Drvs"] != null)
                    {
                        base.Tables.Add(new DrvsDataTable(dataSet.Tables["Drvs"]));
                    }
                    if (dataSet.Tables["FTyps"] != null)
                    {
                        base.Tables.Add(new FTypsDataTable(dataSet.Tables["FTyps"]));
                    }
                    if (dataSet.Tables["Itms"] != null)
                    {
                        base.Tables.Add(new ItmsDataTable(dataSet.Tables["Itms"]));
                    }
                    if (dataSet.Tables["Mchns"] != null)
                    {
                        base.Tables.Add(new MchnsDataTable(dataSet.Tables["Mchns"]));
                    }
                    if (dataSet.Tables["Parts"] != null)
                    {
                        base.Tables.Add(new PartsDataTable(dataSet.Tables["Parts"]));
                    }
                    if (dataSet.Tables["TblIds"] != null)
                    {
                        base.Tables.Add(new TblIdsDataTable(dataSet.Tables["TblIds"]));
                    }
                    if (dataSet.Tables["Usrs"] != null)
                    {
                        base.Tables.Add(new UsrsDataTable(dataSet.Tables["Usrs"]));
                    }
                    if (dataSet.Tables["Wrds"] != null)
                    {
                        base.Tables.Add(new WrdsDataTable(dataSet.Tables["Wrds"]));
                    }
                    base.DataSetName = dataSet.DataSetName;
                    base.Prefix = dataSet.Prefix;
                    base.Namespace = dataSet.Namespace;
                    base.Locale = dataSet.Locale;
                    base.CaseSensitive = dataSet.CaseSensitive;
                    base.EnforceConstraints = dataSet.EnforceConstraints;
                    base.Merge(dataSet, false, MissingSchemaAction.Add);
                    this.InitVars();
                }
                else
                {
                    base.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
                }
                base.GetSerializationData(info, context);
                CollectionChangeEventHandler handler = new CollectionChangeEventHandler(this.SchemaChanged);
                base.Tables.CollectionChanged += handler;
                this.Relations.CollectionChanged += handler;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public override DataSet Clone()
        {
            DbRootsVII0004DataSet set1 = (DbRootsVII0004DataSet) base.Clone();
            set1.InitVars();
            set1.SchemaSerializationMode = this.SchemaSerializationMode;
            return set1;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected override XmlSchema GetSchemaSerializable()
        {
            MemoryStream w = new MemoryStream();
            base.WriteXmlSchema(new XmlTextWriter(w, null));
            w.Position = 0L;
            return XmlSchema.Read(new XmlTextReader(w), null);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public static XmlSchemaComplexType GetTypedDataSetSchema(XmlSchemaSet xs)
        {
            DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            XmlSchemaSequence sequence = new XmlSchemaSequence();
            XmlSchemaAny item = new XmlSchemaAny {
                Namespace = set.Namespace
            };
            sequence.Items.Add(item);
            type.Particle = sequence;
            XmlSchema schemaSerializable = set.GetSchemaSerializable();
            if (xs.Contains(schemaSerializable.TargetNamespace))
            {
                MemoryStream stream = new MemoryStream();
                MemoryStream stream2 = new MemoryStream();
                try
                {
                    schemaSerializable.Write(stream);
                    IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        stream2.SetLength(0L);
                        ((XmlSchema) enumerator.Current).Write(stream2);
                        if (stream.Length == stream2.Length)
                        {
                            stream.Position = 0L;
                            stream2.Position = 0L;
                            while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                            {
                            }
                            if (stream.Position == stream.Length)
                            {
                                return type;
                            }
                        }
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    if (stream2 != null)
                    {
                        stream2.Close();
                    }
                }
            }
            xs.Add(schemaSerializable);
            return type;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void InitClass()
        {
            base.DataSetName = "DbRootsVII0004DataSet";
            base.Prefix = "";
            base.Namespace = "http://tempuri.org/DbRootsVII0004DataSet.xsd";
            base.EnforceConstraints = true;
            this.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            this.tableCtnrs = new CtnrsDataTable();
            base.Tables.Add(this.tableCtnrs);
            this.tableCTyps = new CTypsDataTable();
            base.Tables.Add(this.tableCTyps);
            this.tableDbRoots = new DbRootsDataTable();
            base.Tables.Add(this.tableDbRoots);
            this.tableDrvs = new DrvsDataTable();
            base.Tables.Add(this.tableDrvs);
            this.tableFTyps = new FTypsDataTable();
            base.Tables.Add(this.tableFTyps);
            this.tableItms = new ItmsDataTable();
            base.Tables.Add(this.tableItms);
            this.tableMchns = new MchnsDataTable();
            base.Tables.Add(this.tableMchns);
            this.tableParts = new PartsDataTable();
            base.Tables.Add(this.tableParts);
            this.tableTblIds = new TblIdsDataTable();
            base.Tables.Add(this.tableTblIds);
            this.tableUsrs = new UsrsDataTable();
            base.Tables.Add(this.tableUsrs);
            this.tableWrds = new WrdsDataTable();
            base.Tables.Add(this.tableWrds);
            DataColumn[] parentColumns = new DataColumn[] { this.tableDrvs.IdColumn };
            DataColumn[] childColumns = new DataColumn[] { this.tableCtnrs.IdDColumn };
            this.relationCtnrs_FK00 = new DataRelation("Ctnrs_FK00", parentColumns, childColumns, false);
            this.Relations.Add(this.relationCtnrs_FK00);
            DataColumn[] columnArray3 = new DataColumn[] { this.tableWrds.IdColumn };
            DataColumn[] columnArray4 = new DataColumn[] { this.tableCtnrs.IdWColumn };
            this.relationCtnrs_FK01 = new DataRelation("Ctnrs_FK01", columnArray3, columnArray4, false);
            this.Relations.Add(this.relationCtnrs_FK01);
            DataColumn[] columnArray5 = new DataColumn[] { this.tableDbRoots.IdColumn };
            DataColumn[] columnArray6 = new DataColumn[] { this.tableCTyps.IdRColumn };
            this.relationCTyps_FK00 = new DataRelation("CTyps_FK00", columnArray5, columnArray6, false);
            this.Relations.Add(this.relationCTyps_FK00);
            DataColumn[] columnArray7 = new DataColumn[] { this.tableDbRoots.IdColumn };
            DataColumn[] columnArray8 = new DataColumn[] { this.tableDrvs.IdRColumn };
            this.relationDrvs_FK00 = new DataRelation("Drvs_FK00", columnArray7, columnArray8, false);
            this.Relations.Add(this.relationDrvs_FK00);
            DataColumn[] columnArray9 = new DataColumn[] { this.tableCTyps.IdColumn };
            DataColumn[] columnArray10 = new DataColumn[] { this.tableFTyps.IdpColumn };
            this.relationFTyps_FK00 = new DataRelation("FTyps_FK00", columnArray9, columnArray10, false);
            this.Relations.Add(this.relationFTyps_FK00);
            DataColumn[] columnArray11 = new DataColumn[] { this.tableCtnrs.IdColumn };
            DataColumn[] columnArray12 = new DataColumn[] { this.tableItms.IdCColumn };
            this.relationItms_FK00 = new DataRelation("Itms_FK00", columnArray11, columnArray12, false);
            this.Relations.Add(this.relationItms_FK00);
            DataColumn[] columnArray13 = new DataColumn[] { this.tableFTyps.IdColumn };
            DataColumn[] columnArray14 = new DataColumn[] { this.tableItms.IdXColumn };
            this.relationItms_FK01 = new DataRelation("Itms_FK01", columnArray13, columnArray14, false);
            this.Relations.Add(this.relationItms_FK01);
            DataColumn[] columnArray15 = new DataColumn[] { this.tableWrds.IdColumn };
            DataColumn[] columnArray16 = new DataColumn[] { this.tableItms.IdWColumn };
            this.relationItms_FK02 = new DataRelation("Itms_FK02", columnArray15, columnArray16, false);
            this.Relations.Add(this.relationItms_FK02);
            DataColumn[] columnArray17 = new DataColumn[] { this.tableDbRoots.IdColumn };
            DataColumn[] columnArray18 = new DataColumn[] { this.tableMchns.IdRColumn };
            this.relationMchns_FK00 = new DataRelation("Mchns_FK00", columnArray17, columnArray18, false);
            this.Relations.Add(this.relationMchns_FK00);
            DataColumn[] columnArray19 = new DataColumn[] { this.tableItms.IdColumn };
            DataColumn[] columnArray20 = new DataColumn[] { this.tableParts.IdpColumn };
            this.relationParts_FK00 = new DataRelation("Parts_FK00", columnArray19, columnArray20, false);
            this.Relations.Add(this.relationParts_FK00);
            DataColumn[] columnArray21 = new DataColumn[] { this.tableMchns.IdColumn };
            DataColumn[] columnArray22 = new DataColumn[] { this.tableUsrs.IdMColumn };
            this.relationUsrs_FK00 = new DataRelation("Usrs_FK00", columnArray21, columnArray22, false);
            this.Relations.Add(this.relationUsrs_FK00);
            DataColumn[] columnArray23 = new DataColumn[] { this.tableDbRoots.IdColumn };
            DataColumn[] columnArray24 = new DataColumn[] { this.tableWrds.IdRColumn };
            this.relationWrds_FK00 = new DataRelation("Wrds_FK00", columnArray23, columnArray24, false);
            this.Relations.Add(this.relationWrds_FK00);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected override void InitializeDerivedDataSet()
        {
            base.BeginInit();
            this.InitClass();
            base.EndInit();
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        internal void InitVars()
        {
            this.InitVars(true);
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        internal void InitVars(bool initTable)
        {
            this.tableCtnrs = (CtnrsDataTable) base.Tables["Ctnrs"];
            if (initTable && (this.tableCtnrs != null))
            {
                this.tableCtnrs.InitVars();
            }
            this.tableCTyps = (CTypsDataTable) base.Tables["CTyps"];
            if (initTable && (this.tableCTyps != null))
            {
                this.tableCTyps.InitVars();
            }
            this.tableDbRoots = (DbRootsDataTable) base.Tables["DbRoots"];
            if (initTable && (this.tableDbRoots != null))
            {
                this.tableDbRoots.InitVars();
            }
            this.tableDrvs = (DrvsDataTable) base.Tables["Drvs"];
            if (initTable && (this.tableDrvs != null))
            {
                this.tableDrvs.InitVars();
            }
            this.tableFTyps = (FTypsDataTable) base.Tables["FTyps"];
            if (initTable && (this.tableFTyps != null))
            {
                this.tableFTyps.InitVars();
            }
            this.tableItms = (ItmsDataTable) base.Tables["Itms"];
            if (initTable && (this.tableItms != null))
            {
                this.tableItms.InitVars();
            }
            this.tableMchns = (MchnsDataTable) base.Tables["Mchns"];
            if (initTable && (this.tableMchns != null))
            {
                this.tableMchns.InitVars();
            }
            this.tableParts = (PartsDataTable) base.Tables["Parts"];
            if (initTable && (this.tableParts != null))
            {
                this.tableParts.InitVars();
            }
            this.tableTblIds = (TblIdsDataTable) base.Tables["TblIds"];
            if (initTable && (this.tableTblIds != null))
            {
                this.tableTblIds.InitVars();
            }
            this.tableUsrs = (UsrsDataTable) base.Tables["Usrs"];
            if (initTable && (this.tableUsrs != null))
            {
                this.tableUsrs.InitVars();
            }
            this.tableWrds = (WrdsDataTable) base.Tables["Wrds"];
            if (initTable && (this.tableWrds != null))
            {
                this.tableWrds.InitVars();
            }
            this.relationCtnrs_FK00 = this.Relations["Ctnrs_FK00"];
            this.relationCtnrs_FK01 = this.Relations["Ctnrs_FK01"];
            this.relationCTyps_FK00 = this.Relations["CTyps_FK00"];
            this.relationDrvs_FK00 = this.Relations["Drvs_FK00"];
            this.relationFTyps_FK00 = this.Relations["FTyps_FK00"];
            this.relationItms_FK00 = this.Relations["Itms_FK00"];
            this.relationItms_FK01 = this.Relations["Itms_FK01"];
            this.relationItms_FK02 = this.Relations["Itms_FK02"];
            this.relationMchns_FK00 = this.Relations["Mchns_FK00"];
            this.relationParts_FK00 = this.Relations["Parts_FK00"];
            this.relationUsrs_FK00 = this.Relations["Usrs_FK00"];
            this.relationWrds_FK00 = this.Relations["Wrds_FK00"];
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected override void ReadXmlSerializable(XmlReader reader)
        {
            if (base.DetermineSchemaSerializationMode(reader) == System.Data.SchemaSerializationMode.IncludeSchema)
            {
                this.Reset();
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(reader);
                if (dataSet.Tables["Ctnrs"] != null)
                {
                    base.Tables.Add(new CtnrsDataTable(dataSet.Tables["Ctnrs"]));
                }
                if (dataSet.Tables["CTyps"] != null)
                {
                    base.Tables.Add(new CTypsDataTable(dataSet.Tables["CTyps"]));
                }
                if (dataSet.Tables["DbRoots"] != null)
                {
                    base.Tables.Add(new DbRootsDataTable(dataSet.Tables["DbRoots"]));
                }
                if (dataSet.Tables["Drvs"] != null)
                {
                    base.Tables.Add(new DrvsDataTable(dataSet.Tables["Drvs"]));
                }
                if (dataSet.Tables["FTyps"] != null)
                {
                    base.Tables.Add(new FTypsDataTable(dataSet.Tables["FTyps"]));
                }
                if (dataSet.Tables["Itms"] != null)
                {
                    base.Tables.Add(new ItmsDataTable(dataSet.Tables["Itms"]));
                }
                if (dataSet.Tables["Mchns"] != null)
                {
                    base.Tables.Add(new MchnsDataTable(dataSet.Tables["Mchns"]));
                }
                if (dataSet.Tables["Parts"] != null)
                {
                    base.Tables.Add(new PartsDataTable(dataSet.Tables["Parts"]));
                }
                if (dataSet.Tables["TblIds"] != null)
                {
                    base.Tables.Add(new TblIdsDataTable(dataSet.Tables["TblIds"]));
                }
                if (dataSet.Tables["Usrs"] != null)
                {
                    base.Tables.Add(new UsrsDataTable(dataSet.Tables["Usrs"]));
                }
                if (dataSet.Tables["Wrds"] != null)
                {
                    base.Tables.Add(new WrdsDataTable(dataSet.Tables["Wrds"]));
                }
                base.DataSetName = dataSet.DataSetName;
                base.Prefix = dataSet.Prefix;
                base.Namespace = dataSet.Namespace;
                base.Locale = dataSet.Locale;
                base.CaseSensitive = dataSet.CaseSensitive;
                base.EnforceConstraints = dataSet.EnforceConstraints;
                base.Merge(dataSet, false, MissingSchemaAction.Add);
                this.InitVars();
            }
            else
            {
                base.ReadXml(reader);
                this.InitVars();
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private void SchemaChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                this.InitVars();
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeCtnrs()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeCTyps()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeDbRoots()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeDrvs()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeFTyps()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeItms()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeMchns()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeParts()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected override bool ShouldSerializeRelations()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        protected override bool ShouldSerializeTables()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeTblIds()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeUsrs()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        private bool ShouldSerializeWrds()
        {
            return false;
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CtnrsDataTable Ctnrs
        {
            get
            {
                return this.tableCtnrs;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CTypsDataTable CTyps
        {
            get
            {
                return this.tableCTyps;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DbRootsDataTable DbRoots
        {
            get
            {
                return this.tableDbRoots;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DrvsDataTable Drvs
        {
            get
            {
                return this.tableDrvs;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FTypsDataTable FTyps
        {
            get
            {
                return this.tableFTyps;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ItmsDataTable Itms
        {
            get
            {
                return this.tableItms;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MchnsDataTable Mchns
        {
            get
            {
                return this.tableMchns;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PartsDataTable Parts
        {
            get
            {
                return this.tableParts;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TblIdsDataTable TblIds
        {
            get
            {
                return this.tableTblIds;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UsrsDataTable Usrs
        {
            get
            {
                return this.tableUsrs;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public WrdsDataTable Wrds
        {
            get
            {
                return this.tableWrds;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override System.Data.SchemaSerializationMode SchemaSerializationMode
        {
            get
            {
                return this._schemaSerializationMode;
            }
            set
            {
                this._schemaSerializationMode = value;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTableCollection Tables
        {
            get
            {
                return base.Tables;
            }
        }

        [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataRelationCollection Relations
        {
            get
            {
                return base.Relations;
            }
        }

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class CtnrsDataTable : TypedTableBase<DbRootsVII0004DataSet.CtnrsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdD;
            private DataColumn columnIdC;
            private DataColumn columnIdW;
            private DataColumn columnCr8D8;
            private DataColumn columnChkD8;
            private DataColumn columnNfo;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CtnrsRowChangeEventHandler CtnrsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CtnrsRowChangeEventHandler CtnrsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CtnrsRowChangeEventHandler CtnrsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CtnrsRowChangeEventHandler CtnrsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public CtnrsDataTable()
            {
                base.TableName = "Ctnrs";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal CtnrsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected CtnrsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddCtnrsRow(DbRootsVII0004DataSet.CtnrsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CtnrsRow AddCtnrsRow(int Id, DbRootsVII0004DataSet.DrvsRow parentDrvsRowByCtnrs_FK00, int IdC, DbRootsVII0004DataSet.WrdsRow parentWrdsRowByCtnrs_FK01, DateTime Cr8D8, DateTime ChkD8, string Nfo)
            {
                DbRootsVII0004DataSet.CtnrsRow row = (DbRootsVII0004DataSet.CtnrsRow) base.NewRow();
                object[] objArray1 = new object[7];
                objArray1[0] = Id;
                objArray1[2] = IdC;
                objArray1[4] = Cr8D8;
                objArray1[5] = ChkD8;
                objArray1[6] = Nfo;
                object[] objArray = objArray1;
                if (parentDrvsRowByCtnrs_FK00 != null)
                {
                    objArray[1] = parentDrvsRowByCtnrs_FK00[0];
                }
                if (parentWrdsRowByCtnrs_FK01 != null)
                {
                    objArray[3] = parentWrdsRowByCtnrs_FK01[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.CtnrsDataTable table1 = (DbRootsVII0004DataSet.CtnrsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.CtnrsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CtnrsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.CtnrsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.CtnrsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "CtnrsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdD = new DataColumn("IdD", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdD);
                this.columnIdC = new DataColumn("IdC", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdC);
                this.columnIdW = new DataColumn("IdW", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdW);
                this.columnCr8D8 = new DataColumn("Cr8D8", typeof(DateTime), null, MappingType.Element);
                base.Columns.Add(this.columnCr8D8);
                this.columnChkD8 = new DataColumn("ChkD8", typeof(DateTime), null, MappingType.Element);
                base.Columns.Add(this.columnChkD8);
                this.columnNfo = new DataColumn("Nfo", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNfo);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnNfo.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdD = base.Columns["IdD"];
                this.columnIdC = base.Columns["IdC"];
                this.columnIdW = base.Columns["IdW"];
                this.columnCr8D8 = base.Columns["Cr8D8"];
                this.columnChkD8 = base.Columns["ChkD8"];
                this.columnNfo = base.Columns["Nfo"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CtnrsRow NewCtnrsRow()
            {
                return (DbRootsVII0004DataSet.CtnrsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.CtnrsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.CtnrsRowChanged != null)
                {
                    this.CtnrsRowChanged(this, new DbRootsVII0004DataSet.CtnrsRowChangeEvent((DbRootsVII0004DataSet.CtnrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.CtnrsRowChanging != null)
                {
                    this.CtnrsRowChanging(this, new DbRootsVII0004DataSet.CtnrsRowChangeEvent((DbRootsVII0004DataSet.CtnrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.CtnrsRowDeleted != null)
                {
                    this.CtnrsRowDeleted(this, new DbRootsVII0004DataSet.CtnrsRowChangeEvent((DbRootsVII0004DataSet.CtnrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.CtnrsRowDeleting != null)
                {
                    this.CtnrsRowDeleting(this, new DbRootsVII0004DataSet.CtnrsRowChangeEvent((DbRootsVII0004DataSet.CtnrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveCtnrsRow(DbRootsVII0004DataSet.CtnrsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdDColumn
            {
                get
                {
                    return this.columnIdD;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdCColumn
            {
                get
                {
                    return this.columnIdC;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdWColumn
            {
                get
                {
                    return this.columnIdW;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn Cr8D8Column
            {
                get
                {
                    return this.columnCr8D8;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn ChkD8Column
            {
                get
                {
                    return this.columnChkD8;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NfoColumn
            {
                get
                {
                    return this.columnNfo;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CtnrsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.CtnrsRow) base.Rows[index];
                }
            }
        }

        public class CtnrsRow : DataRow
        {
            private DbRootsVII0004DataSet.CtnrsDataTable tableCtnrs;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal CtnrsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableCtnrs = (DbRootsVII0004DataSet.CtnrsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow[] GetItmsRows()
            {
                if (base.Table.ChildRelations["Itms_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.ItmsRow[0];
                }
                return (DbRootsVII0004DataSet.ItmsRow[]) base.GetChildRows(base.Table.ChildRelations["Itms_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsChkD8Null()
            {
                return base.IsNull(this.tableCtnrs.ChkD8Column);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsCr8D8Null()
            {
                return base.IsNull(this.tableCtnrs.Cr8D8Column);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdCNull()
            {
                return base.IsNull(this.tableCtnrs.IdCColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdDNull()
            {
                return base.IsNull(this.tableCtnrs.IdDColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdWNull()
            {
                return base.IsNull(this.tableCtnrs.IdWColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNfoNull()
            {
                return base.IsNull(this.tableCtnrs.NfoColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetChkD8Null()
            {
                base[this.tableCtnrs.ChkD8Column] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetCr8D8Null()
            {
                base[this.tableCtnrs.Cr8D8Column] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdCNull()
            {
                base[this.tableCtnrs.IdCColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdDNull()
            {
                base[this.tableCtnrs.IdDColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdWNull()
            {
                base[this.tableCtnrs.IdWColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNfoNull()
            {
                base[this.tableCtnrs.NfoColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableCtnrs.IdColumn];
                }
                set
                {
                    base[this.tableCtnrs.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdD
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableCtnrs.IdDColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdD' in table 'Ctnrs' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableCtnrs.IdDColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdC
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableCtnrs.IdCColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdC' in table 'Ctnrs' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableCtnrs.IdCColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdW
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableCtnrs.IdWColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdW' in table 'Ctnrs' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableCtnrs.IdWColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DateTime Cr8D8
            {
                get
                {
                    DateTime time;
                    try
                    {
                        time = (DateTime) base[this.tableCtnrs.Cr8D8Column];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Cr8D8' in table 'Ctnrs' is DBNull.", exception);
                    }
                    return time;
                }
                set
                {
                    base[this.tableCtnrs.Cr8D8Column] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DateTime ChkD8
            {
                get
                {
                    DateTime time;
                    try
                    {
                        time = (DateTime) base[this.tableCtnrs.ChkD8Column];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'ChkD8' in table 'Ctnrs' is DBNull.", exception);
                    }
                    return time;
                }
                set
                {
                    base[this.tableCtnrs.ChkD8Column] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nfo
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableCtnrs.NfoColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nfo' in table 'Ctnrs' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableCtnrs.NfoColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.DrvsRow DrvsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.DrvsRow) base.GetParentRow(base.Table.ParentRelations["Ctnrs_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Ctnrs_FK00"]);
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.WrdsRow WrdsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.WrdsRow) base.GetParentRow(base.Table.ParentRelations["Ctnrs_FK01"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Ctnrs_FK01"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class CtnrsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.CtnrsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public CtnrsRowChangeEvent(DbRootsVII0004DataSet.CtnrsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CtnrsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void CtnrsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.CtnrsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class CTypsDataTable : TypedTableBase<DbRootsVII0004DataSet.CTypsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdR;
            private DataColumn columnNm;
            private DataColumn columnFldr;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CTypsRowChangeEventHandler CTypsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CTypsRowChangeEventHandler CTypsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CTypsRowChangeEventHandler CTypsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.CTypsRowChangeEventHandler CTypsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public CTypsDataTable()
            {
                base.TableName = "CTyps";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal CTypsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected CTypsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddCTypsRow(DbRootsVII0004DataSet.CTypsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CTypsRow AddCTypsRow(int Id, DbRootsVII0004DataSet.DbRootsRow parentDbRootsRowByCTyps_FK00, string Nm, string Fldr)
            {
                DbRootsVII0004DataSet.CTypsRow row = (DbRootsVII0004DataSet.CTypsRow) base.NewRow();
                object[] objArray1 = new object[4];
                objArray1[0] = Id;
                objArray1[2] = Nm;
                objArray1[3] = Fldr;
                object[] objArray = objArray1;
                if (parentDbRootsRowByCTyps_FK00 != null)
                {
                    objArray[1] = parentDbRootsRowByCTyps_FK00[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.CTypsDataTable table1 = (DbRootsVII0004DataSet.CTypsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.CTypsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CTypsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.CTypsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.CTypsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "CTypsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdR = new DataColumn("IdR", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdR);
                this.columnNm = new DataColumn("Nm", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNm);
                this.columnFldr = new DataColumn("Fldr", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnFldr);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnNm.MaxLength = 0xff;
                this.columnFldr.MaxLength = 0xff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdR = base.Columns["IdR"];
                this.columnNm = base.Columns["Nm"];
                this.columnFldr = base.Columns["Fldr"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CTypsRow NewCTypsRow()
            {
                return (DbRootsVII0004DataSet.CTypsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.CTypsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.CTypsRowChanged != null)
                {
                    this.CTypsRowChanged(this, new DbRootsVII0004DataSet.CTypsRowChangeEvent((DbRootsVII0004DataSet.CTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.CTypsRowChanging != null)
                {
                    this.CTypsRowChanging(this, new DbRootsVII0004DataSet.CTypsRowChangeEvent((DbRootsVII0004DataSet.CTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.CTypsRowDeleted != null)
                {
                    this.CTypsRowDeleted(this, new DbRootsVII0004DataSet.CTypsRowChangeEvent((DbRootsVII0004DataSet.CTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.CTypsRowDeleting != null)
                {
                    this.CTypsRowDeleting(this, new DbRootsVII0004DataSet.CTypsRowChangeEvent((DbRootsVII0004DataSet.CTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveCTypsRow(DbRootsVII0004DataSet.CTypsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdRColumn
            {
                get
                {
                    return this.columnIdR;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NmColumn
            {
                get
                {
                    return this.columnNm;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn FldrColumn
            {
                get
                {
                    return this.columnFldr;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CTypsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.CTypsRow) base.Rows[index];
                }
            }
        }

        public class CTypsRow : DataRow
        {
            private DbRootsVII0004DataSet.CTypsDataTable tableCTyps;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal CTypsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableCTyps = (DbRootsVII0004DataSet.CTypsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.FTypsRow[] GetFTypsRows()
            {
                if (base.Table.ChildRelations["FTyps_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.FTypsRow[0];
                }
                return (DbRootsVII0004DataSet.FTypsRow[]) base.GetChildRows(base.Table.ChildRelations["FTyps_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsFldrNull()
            {
                return base.IsNull(this.tableCTyps.FldrColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdRNull()
            {
                return base.IsNull(this.tableCTyps.IdRColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNmNull()
            {
                return base.IsNull(this.tableCTyps.NmColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetFldrNull()
            {
                base[this.tableCTyps.FldrColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdRNull()
            {
                base[this.tableCTyps.IdRColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNmNull()
            {
                base[this.tableCTyps.NmColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableCTyps.IdColumn];
                }
                set
                {
                    base[this.tableCTyps.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdR
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableCTyps.IdRColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdR' in table 'CTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableCTyps.IdRColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nm
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableCTyps.NmColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nm' in table 'CTyps' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableCTyps.NmColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Fldr
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableCTyps.FldrColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Fldr' in table 'CTyps' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableCTyps.FldrColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.DbRootsRow DbRootsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.DbRootsRow) base.GetParentRow(base.Table.ParentRelations["CTyps_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["CTyps_FK00"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class CTypsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.CTypsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public CTypsRowChangeEvent(DbRootsVII0004DataSet.CTypsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CTypsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void CTypsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.CTypsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class DbRootsDataTable : TypedTableBase<DbRootsVII0004DataSet.DbRootsRow>
        {
            private DataColumn columnId;
            private DataColumn columnD8t;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DbRootsRowChangeEventHandler DbRootsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DbRootsRowChangeEventHandler DbRootsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DbRootsRowChangeEventHandler DbRootsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DbRootsRowChangeEventHandler DbRootsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsDataTable()
            {
                base.TableName = "DbRoots";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal DbRootsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected DbRootsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddDbRootsRow(DbRootsVII0004DataSet.DbRootsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DbRootsRow AddDbRootsRow(int Id, DateTime D8t)
            {
                DbRootsVII0004DataSet.DbRootsRow row = (DbRootsVII0004DataSet.DbRootsRow) base.NewRow();
                object[] objArray = new object[] { Id, D8t };
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.DbRootsDataTable table1 = (DbRootsVII0004DataSet.DbRootsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.DbRootsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DbRootsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.DbRootsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.DbRootsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "DbRootsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnD8t = new DataColumn("D8t", typeof(DateTime), null, MappingType.Element);
                base.Columns.Add(this.columnD8t);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnD8t = base.Columns["D8t"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DbRootsRow NewDbRootsRow()
            {
                return (DbRootsVII0004DataSet.DbRootsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.DbRootsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.DbRootsRowChanged != null)
                {
                    this.DbRootsRowChanged(this, new DbRootsVII0004DataSet.DbRootsRowChangeEvent((DbRootsVII0004DataSet.DbRootsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.DbRootsRowChanging != null)
                {
                    this.DbRootsRowChanging(this, new DbRootsVII0004DataSet.DbRootsRowChangeEvent((DbRootsVII0004DataSet.DbRootsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.DbRootsRowDeleted != null)
                {
                    this.DbRootsRowDeleted(this, new DbRootsVII0004DataSet.DbRootsRowChangeEvent((DbRootsVII0004DataSet.DbRootsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.DbRootsRowDeleting != null)
                {
                    this.DbRootsRowDeleting(this, new DbRootsVII0004DataSet.DbRootsRowChangeEvent((DbRootsVII0004DataSet.DbRootsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveDbRootsRow(DbRootsVII0004DataSet.DbRootsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn D8tColumn
            {
                get
                {
                    return this.columnD8t;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DbRootsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.DbRootsRow) base.Rows[index];
                }
            }
        }

        public class DbRootsRow : DataRow
        {
            private DbRootsVII0004DataSet.DbRootsDataTable tableDbRoots;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal DbRootsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableDbRoots = (DbRootsVII0004DataSet.DbRootsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CTypsRow[] GetCTypsRows()
            {
                if (base.Table.ChildRelations["CTyps_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.CTypsRow[0];
                }
                return (DbRootsVII0004DataSet.CTypsRow[]) base.GetChildRows(base.Table.ChildRelations["CTyps_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DrvsRow[] GetDrvsRows()
            {
                if (base.Table.ChildRelations["Drvs_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.DrvsRow[0];
                }
                return (DbRootsVII0004DataSet.DrvsRow[]) base.GetChildRows(base.Table.ChildRelations["Drvs_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.MchnsRow[] GetMchnsRows()
            {
                if (base.Table.ChildRelations["Mchns_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.MchnsRow[0];
                }
                return (DbRootsVII0004DataSet.MchnsRow[]) base.GetChildRows(base.Table.ChildRelations["Mchns_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.WrdsRow[] GetWrdsRows()
            {
                if (base.Table.ChildRelations["Wrds_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.WrdsRow[0];
                }
                return (DbRootsVII0004DataSet.WrdsRow[]) base.GetChildRows(base.Table.ChildRelations["Wrds_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsD8tNull()
            {
                return base.IsNull(this.tableDbRoots.D8tColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetD8tNull()
            {
                base[this.tableDbRoots.D8tColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableDbRoots.IdColumn];
                }
                set
                {
                    base[this.tableDbRoots.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DateTime D8t
            {
                get
                {
                    DateTime time;
                    try
                    {
                        time = (DateTime) base[this.tableDbRoots.D8tColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'D8t' in table 'DbRoots' is DBNull.", exception);
                    }
                    return time;
                }
                set
                {
                    base[this.tableDbRoots.D8tColumn] = value;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class DbRootsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.DbRootsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsRowChangeEvent(DbRootsVII0004DataSet.DbRootsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DbRootsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void DbRootsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.DbRootsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class DrvsDataTable : TypedTableBase<DbRootsVII0004DataSet.DrvsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdR;
            private DataColumn columnIdM;
            private DataColumn columnIdU;
            private DataColumn columnLttr;
            private DataColumn columnNfo;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DrvsRowChangeEventHandler DrvsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DrvsRowChangeEventHandler DrvsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DrvsRowChangeEventHandler DrvsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.DrvsRowChangeEventHandler DrvsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DrvsDataTable()
            {
                base.TableName = "Drvs";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal DrvsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected DrvsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddDrvsRow(DbRootsVII0004DataSet.DrvsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DrvsRow AddDrvsRow(int Id, DbRootsVII0004DataSet.DbRootsRow parentDbRootsRowByDrvs_FK00, int IdM, int IdU, string Lttr, string Nfo)
            {
                DbRootsVII0004DataSet.DrvsRow row = (DbRootsVII0004DataSet.DrvsRow) base.NewRow();
                object[] objArray1 = new object[6];
                objArray1[0] = Id;
                objArray1[2] = IdM;
                objArray1[3] = IdU;
                objArray1[4] = Lttr;
                objArray1[5] = Nfo;
                object[] objArray = objArray1;
                if (parentDbRootsRowByDrvs_FK00 != null)
                {
                    objArray[1] = parentDbRootsRowByDrvs_FK00[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.DrvsDataTable table1 = (DbRootsVII0004DataSet.DrvsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.DrvsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DrvsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.DrvsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.DrvsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "DrvsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdR = new DataColumn("IdR", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdR);
                this.columnIdM = new DataColumn("IdM", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdM);
                this.columnIdU = new DataColumn("IdU", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdU);
                this.columnLttr = new DataColumn("Lttr", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnLttr);
                this.columnNfo = new DataColumn("Nfo", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNfo);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnLttr.MaxLength = 0x3fffffff;
                this.columnNfo.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdR = base.Columns["IdR"];
                this.columnIdM = base.Columns["IdM"];
                this.columnIdU = base.Columns["IdU"];
                this.columnLttr = base.Columns["Lttr"];
                this.columnNfo = base.Columns["Nfo"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DrvsRow NewDrvsRow()
            {
                return (DbRootsVII0004DataSet.DrvsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.DrvsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.DrvsRowChanged != null)
                {
                    this.DrvsRowChanged(this, new DbRootsVII0004DataSet.DrvsRowChangeEvent((DbRootsVII0004DataSet.DrvsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.DrvsRowChanging != null)
                {
                    this.DrvsRowChanging(this, new DbRootsVII0004DataSet.DrvsRowChangeEvent((DbRootsVII0004DataSet.DrvsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.DrvsRowDeleted != null)
                {
                    this.DrvsRowDeleted(this, new DbRootsVII0004DataSet.DrvsRowChangeEvent((DbRootsVII0004DataSet.DrvsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.DrvsRowDeleting != null)
                {
                    this.DrvsRowDeleting(this, new DbRootsVII0004DataSet.DrvsRowChangeEvent((DbRootsVII0004DataSet.DrvsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveDrvsRow(DbRootsVII0004DataSet.DrvsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdRColumn
            {
                get
                {
                    return this.columnIdR;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdMColumn
            {
                get
                {
                    return this.columnIdM;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdUColumn
            {
                get
                {
                    return this.columnIdU;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn LttrColumn
            {
                get
                {
                    return this.columnLttr;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NfoColumn
            {
                get
                {
                    return this.columnNfo;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DrvsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.DrvsRow) base.Rows[index];
                }
            }
        }

        public class DrvsRow : DataRow
        {
            private DbRootsVII0004DataSet.DrvsDataTable tableDrvs;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal DrvsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableDrvs = (DbRootsVII0004DataSet.DrvsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CtnrsRow[] GetCtnrsRows()
            {
                if (base.Table.ChildRelations["Ctnrs_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.CtnrsRow[0];
                }
                return (DbRootsVII0004DataSet.CtnrsRow[]) base.GetChildRows(base.Table.ChildRelations["Ctnrs_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdMNull()
            {
                return base.IsNull(this.tableDrvs.IdMColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdRNull()
            {
                return base.IsNull(this.tableDrvs.IdRColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdUNull()
            {
                return base.IsNull(this.tableDrvs.IdUColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsLttrNull()
            {
                return base.IsNull(this.tableDrvs.LttrColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNfoNull()
            {
                return base.IsNull(this.tableDrvs.NfoColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdMNull()
            {
                base[this.tableDrvs.IdMColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdRNull()
            {
                base[this.tableDrvs.IdRColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdUNull()
            {
                base[this.tableDrvs.IdUColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetLttrNull()
            {
                base[this.tableDrvs.LttrColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNfoNull()
            {
                base[this.tableDrvs.NfoColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableDrvs.IdColumn];
                }
                set
                {
                    base[this.tableDrvs.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdR
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableDrvs.IdRColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdR' in table 'Drvs' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableDrvs.IdRColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdM
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableDrvs.IdMColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdM' in table 'Drvs' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableDrvs.IdMColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdU
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableDrvs.IdUColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdU' in table 'Drvs' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableDrvs.IdUColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Lttr
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableDrvs.LttrColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Lttr' in table 'Drvs' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableDrvs.LttrColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nfo
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableDrvs.NfoColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nfo' in table 'Drvs' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableDrvs.NfoColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.DbRootsRow DbRootsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.DbRootsRow) base.GetParentRow(base.Table.ParentRelations["Drvs_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Drvs_FK00"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class DrvsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.DrvsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DrvsRowChangeEvent(DbRootsVII0004DataSet.DrvsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.DrvsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void DrvsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.DrvsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class FTypsDataTable : TypedTableBase<DbRootsVII0004DataSet.FTypsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdp;
            private DataColumn columnExtn;
            private DataColumn columnApplctn;
            private DataColumn columnDscrptn;
            private DataColumn columnIsApp;
            private DataColumn columnIsData;
            private DataColumn columnIsDoc;
            private DataColumn columnIsGeo;
            private DataColumn columnIsCode;
            private DataColumn columnIsGrp;
            private DataColumn columnIsProj;
            private DataColumn columnEf0;
            private DataColumn columnEf1;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.FTypsRowChangeEventHandler FTypsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.FTypsRowChangeEventHandler FTypsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.FTypsRowChangeEventHandler FTypsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.FTypsRowChangeEventHandler FTypsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FTypsDataTable()
            {
                base.TableName = "FTyps";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal FTypsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected FTypsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddFTypsRow(DbRootsVII0004DataSet.FTypsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.FTypsRow AddFTypsRow(int Id, DbRootsVII0004DataSet.CTypsRow parentCTypsRowByFTyps_FK00, string Extn, string Applctn, string Dscrptn, short IsApp, short IsData, short IsDoc, short IsGeo, short IsCode, short IsGrp, short IsProj, short Ef0, short Ef1)
            {
                DbRootsVII0004DataSet.FTypsRow row = (DbRootsVII0004DataSet.FTypsRow) base.NewRow();
                object[] objArray1 = new object[14];
                objArray1[0] = Id;
                objArray1[2] = Extn;
                objArray1[3] = Applctn;
                objArray1[4] = Dscrptn;
                objArray1[5] = IsApp;
                objArray1[6] = IsData;
                objArray1[7] = IsDoc;
                objArray1[8] = IsGeo;
                objArray1[9] = IsCode;
                objArray1[10] = IsGrp;
                objArray1[11] = IsProj;
                objArray1[12] = Ef0;
                objArray1[13] = Ef1;
                object[] objArray = objArray1;
                if (parentCTypsRowByFTyps_FK00 != null)
                {
                    objArray[1] = parentCTypsRowByFTyps_FK00[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.FTypsDataTable table1 = (DbRootsVII0004DataSet.FTypsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.FTypsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.FTypsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.FTypsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.FTypsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "FTypsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdp = new DataColumn("Idp", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdp);
                this.columnExtn = new DataColumn("Extn", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnExtn);
                this.columnApplctn = new DataColumn("Applctn", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnApplctn);
                this.columnDscrptn = new DataColumn("Dscrptn", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnDscrptn);
                this.columnIsApp = new DataColumn("IsApp", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnIsApp);
                this.columnIsData = new DataColumn("IsData", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnIsData);
                this.columnIsDoc = new DataColumn("IsDoc", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnIsDoc);
                this.columnIsGeo = new DataColumn("IsGeo", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnIsGeo);
                this.columnIsCode = new DataColumn("IsCode", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnIsCode);
                this.columnIsGrp = new DataColumn("IsGrp", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnIsGrp);
                this.columnIsProj = new DataColumn("IsProj", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnIsProj);
                this.columnEf0 = new DataColumn("Ef0", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnEf0);
                this.columnEf1 = new DataColumn("Ef1", typeof(short), null, MappingType.Element);
                base.Columns.Add(this.columnEf1);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnExtn.MaxLength = 0x3fffffff;
                this.columnApplctn.MaxLength = 0x3fffffff;
                this.columnDscrptn.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdp = base.Columns["Idp"];
                this.columnExtn = base.Columns["Extn"];
                this.columnApplctn = base.Columns["Applctn"];
                this.columnDscrptn = base.Columns["Dscrptn"];
                this.columnIsApp = base.Columns["IsApp"];
                this.columnIsData = base.Columns["IsData"];
                this.columnIsDoc = base.Columns["IsDoc"];
                this.columnIsGeo = base.Columns["IsGeo"];
                this.columnIsCode = base.Columns["IsCode"];
                this.columnIsGrp = base.Columns["IsGrp"];
                this.columnIsProj = base.Columns["IsProj"];
                this.columnEf0 = base.Columns["Ef0"];
                this.columnEf1 = base.Columns["Ef1"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.FTypsRow NewFTypsRow()
            {
                return (DbRootsVII0004DataSet.FTypsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.FTypsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.FTypsRowChanged != null)
                {
                    this.FTypsRowChanged(this, new DbRootsVII0004DataSet.FTypsRowChangeEvent((DbRootsVII0004DataSet.FTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.FTypsRowChanging != null)
                {
                    this.FTypsRowChanging(this, new DbRootsVII0004DataSet.FTypsRowChangeEvent((DbRootsVII0004DataSet.FTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.FTypsRowDeleted != null)
                {
                    this.FTypsRowDeleted(this, new DbRootsVII0004DataSet.FTypsRowChangeEvent((DbRootsVII0004DataSet.FTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.FTypsRowDeleting != null)
                {
                    this.FTypsRowDeleting(this, new DbRootsVII0004DataSet.FTypsRowChangeEvent((DbRootsVII0004DataSet.FTypsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveFTypsRow(DbRootsVII0004DataSet.FTypsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdpColumn
            {
                get
                {
                    return this.columnIdp;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn ExtnColumn
            {
                get
                {
                    return this.columnExtn;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn ApplctnColumn
            {
                get
                {
                    return this.columnApplctn;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn DscrptnColumn
            {
                get
                {
                    return this.columnDscrptn;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IsAppColumn
            {
                get
                {
                    return this.columnIsApp;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IsDataColumn
            {
                get
                {
                    return this.columnIsData;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IsDocColumn
            {
                get
                {
                    return this.columnIsDoc;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IsGeoColumn
            {
                get
                {
                    return this.columnIsGeo;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IsCodeColumn
            {
                get
                {
                    return this.columnIsCode;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IsGrpColumn
            {
                get
                {
                    return this.columnIsGrp;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IsProjColumn
            {
                get
                {
                    return this.columnIsProj;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn Ef0Column
            {
                get
                {
                    return this.columnEf0;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn Ef1Column
            {
                get
                {
                    return this.columnEf1;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.FTypsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.FTypsRow) base.Rows[index];
                }
            }
        }

        public class FTypsRow : DataRow
        {
            private DbRootsVII0004DataSet.FTypsDataTable tableFTyps;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal FTypsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableFTyps = (DbRootsVII0004DataSet.FTypsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow[] GetItmsRows()
            {
                if (base.Table.ChildRelations["Itms_FK01"] == null)
                {
                    return new DbRootsVII0004DataSet.ItmsRow[0];
                }
                return (DbRootsVII0004DataSet.ItmsRow[]) base.GetChildRows(base.Table.ChildRelations["Itms_FK01"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsApplctnNull()
            {
                return base.IsNull(this.tableFTyps.ApplctnColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsDscrptnNull()
            {
                return base.IsNull(this.tableFTyps.DscrptnColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsEf0Null()
            {
                return base.IsNull(this.tableFTyps.Ef0Column);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsEf1Null()
            {
                return base.IsNull(this.tableFTyps.Ef1Column);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsExtnNull()
            {
                return base.IsNull(this.tableFTyps.ExtnColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdpNull()
            {
                return base.IsNull(this.tableFTyps.IdpColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIsAppNull()
            {
                return base.IsNull(this.tableFTyps.IsAppColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIsCodeNull()
            {
                return base.IsNull(this.tableFTyps.IsCodeColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIsDataNull()
            {
                return base.IsNull(this.tableFTyps.IsDataColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIsDocNull()
            {
                return base.IsNull(this.tableFTyps.IsDocColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIsGeoNull()
            {
                return base.IsNull(this.tableFTyps.IsGeoColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIsGrpNull()
            {
                return base.IsNull(this.tableFTyps.IsGrpColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIsProjNull()
            {
                return base.IsNull(this.tableFTyps.IsProjColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetApplctnNull()
            {
                base[this.tableFTyps.ApplctnColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetDscrptnNull()
            {
                base[this.tableFTyps.DscrptnColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetEf0Null()
            {
                base[this.tableFTyps.Ef0Column] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetEf1Null()
            {
                base[this.tableFTyps.Ef1Column] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetExtnNull()
            {
                base[this.tableFTyps.ExtnColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdpNull()
            {
                base[this.tableFTyps.IdpColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIsAppNull()
            {
                base[this.tableFTyps.IsAppColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIsCodeNull()
            {
                base[this.tableFTyps.IsCodeColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIsDataNull()
            {
                base[this.tableFTyps.IsDataColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIsDocNull()
            {
                base[this.tableFTyps.IsDocColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIsGeoNull()
            {
                base[this.tableFTyps.IsGeoColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIsGrpNull()
            {
                base[this.tableFTyps.IsGrpColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIsProjNull()
            {
                base[this.tableFTyps.IsProjColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableFTyps.IdColumn];
                }
                set
                {
                    base[this.tableFTyps.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Idp
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableFTyps.IdpColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Idp' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IdpColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Extn
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableFTyps.ExtnColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Extn' in table 'FTyps' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableFTyps.ExtnColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Applctn
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableFTyps.ApplctnColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Applctn' in table 'FTyps' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableFTyps.ApplctnColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Dscrptn
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableFTyps.DscrptnColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Dscrptn' in table 'FTyps' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableFTyps.DscrptnColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short IsApp
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.IsAppColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IsApp' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IsAppColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short IsData
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.IsDataColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IsData' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IsDataColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short IsDoc
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.IsDocColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IsDoc' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IsDocColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short IsGeo
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.IsGeoColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IsGeo' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IsGeoColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short IsCode
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.IsCodeColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IsCode' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IsCodeColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short IsGrp
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.IsGrpColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IsGrp' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IsGrpColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short IsProj
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.IsProjColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IsProj' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.IsProjColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short Ef0
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.Ef0Column];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Ef0' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.Ef0Column] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public short Ef1
            {
                get
                {
                    short num;
                    try
                    {
                        num = (short) base[this.tableFTyps.Ef1Column];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Ef1' in table 'FTyps' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableFTyps.Ef1Column] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.CTypsRow CTypsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.CTypsRow) base.GetParentRow(base.Table.ParentRelations["FTyps_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["FTyps_FK00"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class FTypsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.FTypsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FTypsRowChangeEvent(DbRootsVII0004DataSet.FTypsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.FTypsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void FTypsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.FTypsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class ItmsDataTable : TypedTableBase<DbRootsVII0004DataSet.ItmsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdC;
            private DataColumn columnIdW;
            private DataColumn columnIdX;
            private DataColumn columnCr8D8;
            private DataColumn columnChkD8;
            private DataColumn columnNfo;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.ItmsRowChangeEventHandler ItmsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.ItmsRowChangeEventHandler ItmsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.ItmsRowChangeEventHandler ItmsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.ItmsRowChangeEventHandler ItmsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public ItmsDataTable()
            {
                base.TableName = "Itms";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal ItmsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected ItmsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddItmsRow(DbRootsVII0004DataSet.ItmsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow AddItmsRow(int Id, DbRootsVII0004DataSet.CtnrsRow parentCtnrsRowByItms_FK00, DbRootsVII0004DataSet.WrdsRow parentWrdsRowByItms_FK02, DbRootsVII0004DataSet.FTypsRow parentFTypsRowByItms_FK01, DateTime Cr8D8, DateTime ChkD8, string Nfo)
            {
                DbRootsVII0004DataSet.ItmsRow row = (DbRootsVII0004DataSet.ItmsRow) base.NewRow();
                object[] objArray1 = new object[7];
                objArray1[0] = Id;
                objArray1[4] = Cr8D8;
                objArray1[5] = ChkD8;
                objArray1[6] = Nfo;
                object[] objArray = objArray1;
                if (parentCtnrsRowByItms_FK00 != null)
                {
                    objArray[1] = parentCtnrsRowByItms_FK00[0];
                }
                if (parentWrdsRowByItms_FK02 != null)
                {
                    objArray[2] = parentWrdsRowByItms_FK02[0];
                }
                if (parentFTypsRowByItms_FK01 != null)
                {
                    objArray[3] = parentFTypsRowByItms_FK01[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.ItmsDataTable table1 = (DbRootsVII0004DataSet.ItmsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.ItmsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.ItmsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.ItmsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "ItmsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdC = new DataColumn("IdC", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdC);
                this.columnIdW = new DataColumn("IdW", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdW);
                this.columnIdX = new DataColumn("IdX", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdX);
                this.columnCr8D8 = new DataColumn("Cr8D8", typeof(DateTime), null, MappingType.Element);
                base.Columns.Add(this.columnCr8D8);
                this.columnChkD8 = new DataColumn("ChkD8", typeof(DateTime), null, MappingType.Element);
                base.Columns.Add(this.columnChkD8);
                this.columnNfo = new DataColumn("Nfo", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNfo);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnNfo.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdC = base.Columns["IdC"];
                this.columnIdW = base.Columns["IdW"];
                this.columnIdX = base.Columns["IdX"];
                this.columnCr8D8 = base.Columns["Cr8D8"];
                this.columnChkD8 = base.Columns["ChkD8"];
                this.columnNfo = base.Columns["Nfo"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow NewItmsRow()
            {
                return (DbRootsVII0004DataSet.ItmsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.ItmsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.ItmsRowChanged != null)
                {
                    this.ItmsRowChanged(this, new DbRootsVII0004DataSet.ItmsRowChangeEvent((DbRootsVII0004DataSet.ItmsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.ItmsRowChanging != null)
                {
                    this.ItmsRowChanging(this, new DbRootsVII0004DataSet.ItmsRowChangeEvent((DbRootsVII0004DataSet.ItmsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.ItmsRowDeleted != null)
                {
                    this.ItmsRowDeleted(this, new DbRootsVII0004DataSet.ItmsRowChangeEvent((DbRootsVII0004DataSet.ItmsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.ItmsRowDeleting != null)
                {
                    this.ItmsRowDeleting(this, new DbRootsVII0004DataSet.ItmsRowChangeEvent((DbRootsVII0004DataSet.ItmsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveItmsRow(DbRootsVII0004DataSet.ItmsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdCColumn
            {
                get
                {
                    return this.columnIdC;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdWColumn
            {
                get
                {
                    return this.columnIdW;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdXColumn
            {
                get
                {
                    return this.columnIdX;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn Cr8D8Column
            {
                get
                {
                    return this.columnCr8D8;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn ChkD8Column
            {
                get
                {
                    return this.columnChkD8;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NfoColumn
            {
                get
                {
                    return this.columnNfo;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.ItmsRow) base.Rows[index];
                }
            }
        }

        public class ItmsRow : DataRow
        {
            private DbRootsVII0004DataSet.ItmsDataTable tableItms;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal ItmsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableItms = (DbRootsVII0004DataSet.ItmsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.PartsRow[] GetPartsRows()
            {
                if (base.Table.ChildRelations["Parts_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.PartsRow[0];
                }
                return (DbRootsVII0004DataSet.PartsRow[]) base.GetChildRows(base.Table.ChildRelations["Parts_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsChkD8Null()
            {
                return base.IsNull(this.tableItms.ChkD8Column);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsCr8D8Null()
            {
                return base.IsNull(this.tableItms.Cr8D8Column);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdCNull()
            {
                return base.IsNull(this.tableItms.IdCColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdWNull()
            {
                return base.IsNull(this.tableItms.IdWColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdXNull()
            {
                return base.IsNull(this.tableItms.IdXColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNfoNull()
            {
                return base.IsNull(this.tableItms.NfoColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetChkD8Null()
            {
                base[this.tableItms.ChkD8Column] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetCr8D8Null()
            {
                base[this.tableItms.Cr8D8Column] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdCNull()
            {
                base[this.tableItms.IdCColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdWNull()
            {
                base[this.tableItms.IdWColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdXNull()
            {
                base[this.tableItms.IdXColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNfoNull()
            {
                base[this.tableItms.NfoColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableItms.IdColumn];
                }
                set
                {
                    base[this.tableItms.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdC
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableItms.IdCColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdC' in table 'Itms' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableItms.IdCColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdW
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableItms.IdWColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdW' in table 'Itms' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableItms.IdWColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdX
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableItms.IdXColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdX' in table 'Itms' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableItms.IdXColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DateTime Cr8D8
            {
                get
                {
                    DateTime time;
                    try
                    {
                        time = (DateTime) base[this.tableItms.Cr8D8Column];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Cr8D8' in table 'Itms' is DBNull.", exception);
                    }
                    return time;
                }
                set
                {
                    base[this.tableItms.Cr8D8Column] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DateTime ChkD8
            {
                get
                {
                    DateTime time;
                    try
                    {
                        time = (DateTime) base[this.tableItms.ChkD8Column];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'ChkD8' in table 'Itms' is DBNull.", exception);
                    }
                    return time;
                }
                set
                {
                    base[this.tableItms.ChkD8Column] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nfo
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableItms.NfoColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nfo' in table 'Itms' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableItms.NfoColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.CtnrsRow CtnrsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.CtnrsRow) base.GetParentRow(base.Table.ParentRelations["Itms_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Itms_FK00"]);
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.FTypsRow FTypsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.FTypsRow) base.GetParentRow(base.Table.ParentRelations["Itms_FK01"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Itms_FK01"]);
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.WrdsRow WrdsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.WrdsRow) base.GetParentRow(base.Table.ParentRelations["Itms_FK02"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Itms_FK02"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class ItmsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.ItmsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public ItmsRowChangeEvent(DbRootsVII0004DataSet.ItmsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void ItmsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.ItmsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class MchnsDataTable : TypedTableBase<DbRootsVII0004DataSet.MchnsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdR;
            private DataColumn columnNm;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.MchnsRowChangeEventHandler MchnsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.MchnsRowChangeEventHandler MchnsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.MchnsRowChangeEventHandler MchnsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.MchnsRowChangeEventHandler MchnsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public MchnsDataTable()
            {
                base.TableName = "Mchns";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal MchnsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected MchnsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddMchnsRow(DbRootsVII0004DataSet.MchnsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.MchnsRow AddMchnsRow(int Id, DbRootsVII0004DataSet.DbRootsRow parentDbRootsRowByMchns_FK00, string Nm)
            {
                DbRootsVII0004DataSet.MchnsRow row = (DbRootsVII0004DataSet.MchnsRow) base.NewRow();
                object[] objArray1 = new object[3];
                objArray1[0] = Id;
                objArray1[2] = Nm;
                object[] objArray = objArray1;
                if (parentDbRootsRowByMchns_FK00 != null)
                {
                    objArray[1] = parentDbRootsRowByMchns_FK00[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.MchnsDataTable table1 = (DbRootsVII0004DataSet.MchnsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.MchnsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.MchnsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.MchnsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.MchnsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "MchnsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdR = new DataColumn("IdR", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdR);
                this.columnNm = new DataColumn("Nm", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNm);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnNm.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdR = base.Columns["IdR"];
                this.columnNm = base.Columns["Nm"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.MchnsRow NewMchnsRow()
            {
                return (DbRootsVII0004DataSet.MchnsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.MchnsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.MchnsRowChanged != null)
                {
                    this.MchnsRowChanged(this, new DbRootsVII0004DataSet.MchnsRowChangeEvent((DbRootsVII0004DataSet.MchnsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.MchnsRowChanging != null)
                {
                    this.MchnsRowChanging(this, new DbRootsVII0004DataSet.MchnsRowChangeEvent((DbRootsVII0004DataSet.MchnsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.MchnsRowDeleted != null)
                {
                    this.MchnsRowDeleted(this, new DbRootsVII0004DataSet.MchnsRowChangeEvent((DbRootsVII0004DataSet.MchnsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.MchnsRowDeleting != null)
                {
                    this.MchnsRowDeleting(this, new DbRootsVII0004DataSet.MchnsRowChangeEvent((DbRootsVII0004DataSet.MchnsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveMchnsRow(DbRootsVII0004DataSet.MchnsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdRColumn
            {
                get
                {
                    return this.columnIdR;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NmColumn
            {
                get
                {
                    return this.columnNm;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.MchnsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.MchnsRow) base.Rows[index];
                }
            }
        }

        public class MchnsRow : DataRow
        {
            private DbRootsVII0004DataSet.MchnsDataTable tableMchns;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal MchnsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableMchns = (DbRootsVII0004DataSet.MchnsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.UsrsRow[] GetUsrsRows()
            {
                if (base.Table.ChildRelations["Usrs_FK00"] == null)
                {
                    return new DbRootsVII0004DataSet.UsrsRow[0];
                }
                return (DbRootsVII0004DataSet.UsrsRow[]) base.GetChildRows(base.Table.ChildRelations["Usrs_FK00"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdRNull()
            {
                return base.IsNull(this.tableMchns.IdRColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNmNull()
            {
                return base.IsNull(this.tableMchns.NmColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdRNull()
            {
                base[this.tableMchns.IdRColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNmNull()
            {
                base[this.tableMchns.NmColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableMchns.IdColumn];
                }
                set
                {
                    base[this.tableMchns.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdR
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableMchns.IdRColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdR' in table 'Mchns' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableMchns.IdRColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nm
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableMchns.NmColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nm' in table 'Mchns' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableMchns.NmColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.DbRootsRow DbRootsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.DbRootsRow) base.GetParentRow(base.Table.ParentRelations["Mchns_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Mchns_FK00"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class MchnsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.MchnsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public MchnsRowChangeEvent(DbRootsVII0004DataSet.MchnsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.MchnsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void MchnsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.MchnsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class PartsDataTable : TypedTableBase<DbRootsVII0004DataSet.PartsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdp;
            private DataColumn columnTyp;
            private DataColumn columnNdx;
            private DataColumn columnVal;
            private DataColumn columnNfo;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.PartsRowChangeEventHandler PartsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.PartsRowChangeEventHandler PartsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.PartsRowChangeEventHandler PartsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.PartsRowChangeEventHandler PartsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public PartsDataTable()
            {
                base.TableName = "Parts";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal PartsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected PartsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddPartsRow(DbRootsVII0004DataSet.PartsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.PartsRow AddPartsRow(int Id, DbRootsVII0004DataSet.ItmsRow parentItmsRowByParts_FK00, int Typ, int Ndx, string Val, string Nfo)
            {
                DbRootsVII0004DataSet.PartsRow row = (DbRootsVII0004DataSet.PartsRow) base.NewRow();
                object[] objArray1 = new object[6];
                objArray1[0] = Id;
                objArray1[2] = Typ;
                objArray1[3] = Ndx;
                objArray1[4] = Val;
                objArray1[5] = Nfo;
                object[] objArray = objArray1;
                if (parentItmsRowByParts_FK00 != null)
                {
                    objArray[1] = parentItmsRowByParts_FK00[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.PartsDataTable table1 = (DbRootsVII0004DataSet.PartsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.PartsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.PartsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.PartsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.PartsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "PartsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdp = new DataColumn("Idp", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdp);
                this.columnTyp = new DataColumn("Typ", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnTyp);
                this.columnNdx = new DataColumn("Ndx", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnNdx);
                this.columnVal = new DataColumn("Val", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnVal);
                this.columnNfo = new DataColumn("Nfo", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNfo);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnVal.MaxLength = 0x3fffffff;
                this.columnNfo.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdp = base.Columns["Idp"];
                this.columnTyp = base.Columns["Typ"];
                this.columnNdx = base.Columns["Ndx"];
                this.columnVal = base.Columns["Val"];
                this.columnNfo = base.Columns["Nfo"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.PartsRow NewPartsRow()
            {
                return (DbRootsVII0004DataSet.PartsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.PartsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.PartsRowChanged != null)
                {
                    this.PartsRowChanged(this, new DbRootsVII0004DataSet.PartsRowChangeEvent((DbRootsVII0004DataSet.PartsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.PartsRowChanging != null)
                {
                    this.PartsRowChanging(this, new DbRootsVII0004DataSet.PartsRowChangeEvent((DbRootsVII0004DataSet.PartsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.PartsRowDeleted != null)
                {
                    this.PartsRowDeleted(this, new DbRootsVII0004DataSet.PartsRowChangeEvent((DbRootsVII0004DataSet.PartsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.PartsRowDeleting != null)
                {
                    this.PartsRowDeleting(this, new DbRootsVII0004DataSet.PartsRowChangeEvent((DbRootsVII0004DataSet.PartsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemovePartsRow(DbRootsVII0004DataSet.PartsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdpColumn
            {
                get
                {
                    return this.columnIdp;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn TypColumn
            {
                get
                {
                    return this.columnTyp;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NdxColumn
            {
                get
                {
                    return this.columnNdx;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn ValColumn
            {
                get
                {
                    return this.columnVal;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NfoColumn
            {
                get
                {
                    return this.columnNfo;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.PartsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.PartsRow) base.Rows[index];
                }
            }
        }

        public class PartsRow : DataRow
        {
            private DbRootsVII0004DataSet.PartsDataTable tableParts;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal PartsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableParts = (DbRootsVII0004DataSet.PartsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdpNull()
            {
                return base.IsNull(this.tableParts.IdpColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNdxNull()
            {
                return base.IsNull(this.tableParts.NdxColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNfoNull()
            {
                return base.IsNull(this.tableParts.NfoColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsTypNull()
            {
                return base.IsNull(this.tableParts.TypColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsValNull()
            {
                return base.IsNull(this.tableParts.ValColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdpNull()
            {
                base[this.tableParts.IdpColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNdxNull()
            {
                base[this.tableParts.NdxColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNfoNull()
            {
                base[this.tableParts.NfoColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetTypNull()
            {
                base[this.tableParts.TypColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetValNull()
            {
                base[this.tableParts.ValColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableParts.IdColumn];
                }
                set
                {
                    base[this.tableParts.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Idp
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableParts.IdpColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Idp' in table 'Parts' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableParts.IdpColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Typ
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableParts.TypColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Typ' in table 'Parts' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableParts.TypColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Ndx
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableParts.NdxColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Ndx' in table 'Parts' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableParts.NdxColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Val
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableParts.ValColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Val' in table 'Parts' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableParts.ValColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nfo
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableParts.NfoColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nfo' in table 'Parts' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableParts.NfoColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.ItmsRow ItmsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.ItmsRow) base.GetParentRow(base.Table.ParentRelations["Parts_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Parts_FK00"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class PartsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.PartsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public PartsRowChangeEvent(DbRootsVII0004DataSet.PartsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.PartsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void PartsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.PartsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class TblIdsDataTable : TypedTableBase<DbRootsVII0004DataSet.TblIdsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdR;
            private DataColumn columnNm;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.TblIdsRowChangeEventHandler TblIdsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.TblIdsRowChangeEventHandler TblIdsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.TblIdsRowChangeEventHandler TblIdsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.TblIdsRowChangeEventHandler TblIdsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public TblIdsDataTable()
            {
                base.TableName = "TblIds";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal TblIdsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected TblIdsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddTblIdsRow(DbRootsVII0004DataSet.TblIdsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.TblIdsRow AddTblIdsRow(int Id, int IdR, string Nm)
            {
                DbRootsVII0004DataSet.TblIdsRow row = (DbRootsVII0004DataSet.TblIdsRow) base.NewRow();
                object[] objArray = new object[] { Id, IdR, Nm };
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.TblIdsDataTable table1 = (DbRootsVII0004DataSet.TblIdsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.TblIdsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.TblIdsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "TblIdsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdR = new DataColumn("IdR", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdR);
                this.columnNm = new DataColumn("Nm", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNm);
                this.columnNm.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdR = base.Columns["IdR"];
                this.columnNm = base.Columns["Nm"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.TblIdsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.TblIdsRow NewTblIdsRow()
            {
                return (DbRootsVII0004DataSet.TblIdsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.TblIdsRowChanged != null)
                {
                    this.TblIdsRowChanged(this, new DbRootsVII0004DataSet.TblIdsRowChangeEvent((DbRootsVII0004DataSet.TblIdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.TblIdsRowChanging != null)
                {
                    this.TblIdsRowChanging(this, new DbRootsVII0004DataSet.TblIdsRowChangeEvent((DbRootsVII0004DataSet.TblIdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.TblIdsRowDeleted != null)
                {
                    this.TblIdsRowDeleted(this, new DbRootsVII0004DataSet.TblIdsRowChangeEvent((DbRootsVII0004DataSet.TblIdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.TblIdsRowDeleting != null)
                {
                    this.TblIdsRowDeleting(this, new DbRootsVII0004DataSet.TblIdsRowChangeEvent((DbRootsVII0004DataSet.TblIdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveTblIdsRow(DbRootsVII0004DataSet.TblIdsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdRColumn
            {
                get
                {
                    return this.columnIdR;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NmColumn
            {
                get
                {
                    return this.columnNm;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.TblIdsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.TblIdsRow) base.Rows[index];
                }
            }
        }

        public class TblIdsRow : DataRow
        {
            private DbRootsVII0004DataSet.TblIdsDataTable tableTblIds;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal TblIdsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableTblIds = (DbRootsVII0004DataSet.TblIdsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdNull()
            {
                return base.IsNull(this.tableTblIds.IdColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdRNull()
            {
                return base.IsNull(this.tableTblIds.IdRColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNmNull()
            {
                return base.IsNull(this.tableTblIds.NmColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdNull()
            {
                base[this.tableTblIds.IdColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdRNull()
            {
                base[this.tableTblIds.IdRColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNmNull()
            {
                base[this.tableTblIds.NmColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableTblIds.IdColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Id' in table 'TblIds' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableTblIds.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdR
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableTblIds.IdRColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdR' in table 'TblIds' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableTblIds.IdRColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nm
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableTblIds.NmColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nm' in table 'TblIds' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableTblIds.NmColumn] = value;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class TblIdsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.TblIdsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public TblIdsRowChangeEvent(DbRootsVII0004DataSet.TblIdsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.TblIdsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void TblIdsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.TblIdsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class UsrsDataTable : TypedTableBase<DbRootsVII0004DataSet.UsrsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdM;
            private DataColumn columnNm;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.UsrsRowChangeEventHandler UsrsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.UsrsRowChangeEventHandler UsrsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.UsrsRowChangeEventHandler UsrsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.UsrsRowChangeEventHandler UsrsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public UsrsDataTable()
            {
                base.TableName = "Usrs";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal UsrsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected UsrsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddUsrsRow(DbRootsVII0004DataSet.UsrsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.UsrsRow AddUsrsRow(int Id, DbRootsVII0004DataSet.MchnsRow parentMchnsRowByUsrs_FK00, string Nm)
            {
                DbRootsVII0004DataSet.UsrsRow row = (DbRootsVII0004DataSet.UsrsRow) base.NewRow();
                object[] objArray1 = new object[3];
                objArray1[0] = Id;
                objArray1[2] = Nm;
                object[] objArray = objArray1;
                if (parentMchnsRowByUsrs_FK00 != null)
                {
                    objArray[1] = parentMchnsRowByUsrs_FK00[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.UsrsDataTable table1 = (DbRootsVII0004DataSet.UsrsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.UsrsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.UsrsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.UsrsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.UsrsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "UsrsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdM = new DataColumn("IdM", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdM);
                this.columnNm = new DataColumn("Nm", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnNm);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnNm.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdM = base.Columns["IdM"];
                this.columnNm = base.Columns["Nm"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.UsrsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.UsrsRow NewUsrsRow()
            {
                return (DbRootsVII0004DataSet.UsrsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.UsrsRowChanged != null)
                {
                    this.UsrsRowChanged(this, new DbRootsVII0004DataSet.UsrsRowChangeEvent((DbRootsVII0004DataSet.UsrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.UsrsRowChanging != null)
                {
                    this.UsrsRowChanging(this, new DbRootsVII0004DataSet.UsrsRowChangeEvent((DbRootsVII0004DataSet.UsrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.UsrsRowDeleted != null)
                {
                    this.UsrsRowDeleted(this, new DbRootsVII0004DataSet.UsrsRowChangeEvent((DbRootsVII0004DataSet.UsrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.UsrsRowDeleting != null)
                {
                    this.UsrsRowDeleting(this, new DbRootsVII0004DataSet.UsrsRowChangeEvent((DbRootsVII0004DataSet.UsrsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveUsrsRow(DbRootsVII0004DataSet.UsrsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdMColumn
            {
                get
                {
                    return this.columnIdM;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn NmColumn
            {
                get
                {
                    return this.columnNm;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.UsrsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.UsrsRow) base.Rows[index];
                }
            }
        }

        public class UsrsRow : DataRow
        {
            private DbRootsVII0004DataSet.UsrsDataTable tableUsrs;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal UsrsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableUsrs = (DbRootsVII0004DataSet.UsrsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdMNull()
            {
                return base.IsNull(this.tableUsrs.IdMColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsNmNull()
            {
                return base.IsNull(this.tableUsrs.NmColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdMNull()
            {
                base[this.tableUsrs.IdMColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetNmNull()
            {
                base[this.tableUsrs.NmColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableUsrs.IdColumn];
                }
                set
                {
                    base[this.tableUsrs.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdM
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableUsrs.IdMColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdM' in table 'Usrs' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableUsrs.IdMColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Nm
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableUsrs.NmColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Nm' in table 'Usrs' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableUsrs.NmColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.MchnsRow MchnsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.MchnsRow) base.GetParentRow(base.Table.ParentRelations["Usrs_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Usrs_FK00"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class UsrsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.UsrsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public UsrsRowChangeEvent(DbRootsVII0004DataSet.UsrsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.UsrsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void UsrsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.UsrsRowChangeEvent e);

        [Serializable, XmlSchemaProvider("GetTypedTableSchema")]
        public class WrdsDataTable : TypedTableBase<DbRootsVII0004DataSet.WrdsRow>
        {
            private DataColumn columnId;
            private DataColumn columnIdR;
            private DataColumn columnVal;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.WrdsRowChangeEventHandler WrdsRowChanged;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.WrdsRowChangeEventHandler WrdsRowChanging;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.WrdsRowChangeEventHandler WrdsRowDeleted;

            [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            [field: CompilerGenerated]
            public event DbRootsVII0004DataSet.WrdsRowChangeEventHandler WrdsRowDeleting;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public WrdsDataTable()
            {
                base.TableName = "Wrds";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal WrdsDataTable(DataTable table)
            {
                base.TableName = table.TableName;
                if (table.CaseSensitive != table.DataSet.CaseSensitive)
                {
                    base.CaseSensitive = table.CaseSensitive;
                }
                if (table.Locale.ToString() != table.DataSet.Locale.ToString())
                {
                    base.Locale = table.Locale;
                }
                if (table.Namespace != table.DataSet.Namespace)
                {
                    base.Namespace = table.Namespace;
                }
                base.Prefix = table.Prefix;
                base.MinimumCapacity = table.MinimumCapacity;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected WrdsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this.InitVars();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void AddWrdsRow(DbRootsVII0004DataSet.WrdsRow row)
            {
                base.Rows.Add(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.WrdsRow AddWrdsRow(int Id, DbRootsVII0004DataSet.DbRootsRow parentDbRootsRowByWrds_FK00, string Val)
            {
                DbRootsVII0004DataSet.WrdsRow row = (DbRootsVII0004DataSet.WrdsRow) base.NewRow();
                object[] objArray1 = new object[3];
                objArray1[0] = Id;
                objArray1[2] = Val;
                object[] objArray = objArray1;
                if (parentDbRootsRowByWrds_FK00 != null)
                {
                    objArray[1] = parentDbRootsRowByWrds_FK00[0];
                }
                row.ItemArray = objArray;
                base.Rows.Add(row);
                return row;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public override DataTable Clone()
            {
                DbRootsVII0004DataSet.WrdsDataTable table1 = (DbRootsVII0004DataSet.WrdsDataTable) base.Clone();
                table1.InitVars();
                return table1;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataTable CreateInstance()
            {
                return new DbRootsVII0004DataSet.WrdsDataTable();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.WrdsRow FindById(int Id)
            {
                object[] keys = new object[] { Id };
                return (DbRootsVII0004DataSet.WrdsRow) base.Rows.Find(keys);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override Type GetRowType()
            {
                return typeof(DbRootsVII0004DataSet.WrdsRow);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
            {
                XmlSchemaComplexType type = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                DbRootsVII0004DataSet set = new DbRootsVII0004DataSet();
                XmlSchemaAny item = new XmlSchemaAny {
                    Namespace = "http://www.w3.org/2001/XMLSchema",
                    MinOccurs = 0M,
                    MaxOccurs = 79228162514264337593543950335M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(item);
                XmlSchemaAny any2 = new XmlSchemaAny {
                    Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
                    MinOccurs = 1M,
                    ProcessContents = XmlSchemaContentProcessing.Lax
                };
                sequence.Items.Add(any2);
                XmlSchemaAttribute attribute = new XmlSchemaAttribute {
                    Name = "namespace",
                    FixedValue = set.Namespace
                };
                type.Attributes.Add(attribute);
                XmlSchemaAttribute attribute2 = new XmlSchemaAttribute {
                    Name = "tableTypeName",
                    FixedValue = "WrdsDataTable"
                };
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                XmlSchema schemaSerializable = set.GetSchemaSerializable();
                if (xs.Contains(schemaSerializable.TargetNamespace))
                {
                    MemoryStream stream = new MemoryStream();
                    MemoryStream stream2 = new MemoryStream();
                    try
                    {
                        schemaSerializable.Write(stream);
                        IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            stream2.SetLength(0L);
                            ((XmlSchema) enumerator.Current).Write(stream2);
                            if (stream.Length == stream2.Length)
                            {
                                stream.Position = 0L;
                                stream2.Position = 0L;
                                while ((stream.Position != stream.Length) && (stream.ReadByte() == stream2.ReadByte()))
                                {
                                }
                                if (stream.Position == stream.Length)
                                {
                                    return type;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                        if (stream2 != null)
                        {
                            stream2.Close();
                        }
                    }
                }
                xs.Add(schemaSerializable);
                return type;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            private void InitClass()
            {
                this.columnId = new DataColumn("Id", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnId);
                this.columnIdR = new DataColumn("IdR", typeof(int), null, MappingType.Element);
                base.Columns.Add(this.columnIdR);
                this.columnVal = new DataColumn("Val", typeof(string), null, MappingType.Element);
                base.Columns.Add(this.columnVal);
                DataColumn[] columns = new DataColumn[] { this.columnId };
                base.Constraints.Add(new UniqueConstraint("Constraint1", columns, true));
                this.columnId.AllowDBNull = false;
                this.columnId.Unique = true;
                this.columnVal.MaxLength = 0x3fffffff;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal void InitVars()
            {
                this.columnId = base.Columns["Id"];
                this.columnIdR = base.Columns["IdR"];
                this.columnVal = base.Columns["Val"];
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
            {
                return new DbRootsVII0004DataSet.WrdsRow(builder);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.WrdsRow NewWrdsRow()
            {
                return (DbRootsVII0004DataSet.WrdsRow) base.NewRow();
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanged(DataRowChangeEventArgs e)
            {
                base.OnRowChanged(e);
                if (this.WrdsRowChanged != null)
                {
                    this.WrdsRowChanged(this, new DbRootsVII0004DataSet.WrdsRowChangeEvent((DbRootsVII0004DataSet.WrdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowChanging(DataRowChangeEventArgs e)
            {
                base.OnRowChanging(e);
                if (this.WrdsRowChanging != null)
                {
                    this.WrdsRowChanging(this, new DbRootsVII0004DataSet.WrdsRowChangeEvent((DbRootsVII0004DataSet.WrdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleted(DataRowChangeEventArgs e)
            {
                base.OnRowDeleted(e);
                if (this.WrdsRowDeleted != null)
                {
                    this.WrdsRowDeleted(this, new DbRootsVII0004DataSet.WrdsRowChangeEvent((DbRootsVII0004DataSet.WrdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            protected override void OnRowDeleting(DataRowChangeEventArgs e)
            {
                base.OnRowDeleting(e);
                if (this.WrdsRowDeleting != null)
                {
                    this.WrdsRowDeleting(this, new DbRootsVII0004DataSet.WrdsRowChangeEvent((DbRootsVII0004DataSet.WrdsRow) e.Row, e.Action));
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void RemoveWrdsRow(DbRootsVII0004DataSet.WrdsRow row)
            {
                base.Rows.Remove(row);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdColumn
            {
                get
                {
                    return this.columnId;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn IdRColumn
            {
                get
                {
                    return this.columnIdR;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataColumn ValColumn
            {
                get
                {
                    return this.columnVal;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0"), Browsable(false)]
            public int Count
            {
                get
                {
                    return base.Rows.Count;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.WrdsRow this[int index]
            {
                get
                {
                    return (DbRootsVII0004DataSet.WrdsRow) base.Rows[index];
                }
            }
        }

        public class WrdsRow : DataRow
        {
            private DbRootsVII0004DataSet.WrdsDataTable tableWrds;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            internal WrdsRow(DataRowBuilder rb) : base(rb)
            {
                this.tableWrds = (DbRootsVII0004DataSet.WrdsDataTable) base.Table;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.CtnrsRow[] GetCtnrsRows()
            {
                if (base.Table.ChildRelations["Ctnrs_FK01"] == null)
                {
                    return new DbRootsVII0004DataSet.CtnrsRow[0];
                }
                return (DbRootsVII0004DataSet.CtnrsRow[]) base.GetChildRows(base.Table.ChildRelations["Ctnrs_FK01"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.ItmsRow[] GetItmsRows()
            {
                if (base.Table.ChildRelations["Itms_FK02"] == null)
                {
                    return new DbRootsVII0004DataSet.ItmsRow[0];
                }
                return (DbRootsVII0004DataSet.ItmsRow[]) base.GetChildRows(base.Table.ChildRelations["Itms_FK02"]);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsIdRNull()
            {
                return base.IsNull(this.tableWrds.IdRColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public bool IsValNull()
            {
                return base.IsNull(this.tableWrds.ValColumn);
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetIdRNull()
            {
                base[this.tableWrds.IdRColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public void SetValNull()
            {
                base[this.tableWrds.ValColumn] = Convert.DBNull;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int Id
            {
                get
                {
                    return (int) base[this.tableWrds.IdColumn];
                }
                set
                {
                    base[this.tableWrds.IdColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public int IdR
            {
                get
                {
                    int num;
                    try
                    {
                        num = (int) base[this.tableWrds.IdRColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'IdR' in table 'Wrds' is DBNull.", exception);
                    }
                    return num;
                }
                set
                {
                    base[this.tableWrds.IdRColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public string Val
            {
                get
                {
                    string str;
                    try
                    {
                        str = (string) base[this.tableWrds.ValColumn];
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new StrongTypingException("The value for column 'Val' in table 'Wrds' is DBNull.", exception);
                    }
                    return str;
                }
                set
                {
                    base[this.tableWrds.ValColumn] = value;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public FwNs.DbRootsVII0004DataSet.DbRootsRow DbRootsRow
            {
                get
                {
                    return (FwNs.DbRootsVII0004DataSet.DbRootsRow) base.GetParentRow(base.Table.ParentRelations["Wrds_FK00"]);
                }
                set
                {
                    base.SetParentRow(value, base.Table.ParentRelations["Wrds_FK00"]);
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public class WrdsRowChangeEvent : EventArgs
        {
            private DbRootsVII0004DataSet.WrdsRow eventRow;
            private DataRowAction eventAction;

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public WrdsRowChangeEvent(DbRootsVII0004DataSet.WrdsRow row, DataRowAction action)
            {
                this.eventRow = row;
                this.eventAction = action;
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DbRootsVII0004DataSet.WrdsRow Row
            {
                get
                {
                    return this.eventRow;
                }
            }

            [DebuggerNonUserCode, GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
            public DataRowAction Action
            {
                get
                {
                    return this.eventAction;
                }
            }
        }

        [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "15.0.0.0")]
        public delegate void WrdsRowChangeEventHandler(object sender, DbRootsVII0004DataSet.WrdsRowChangeEvent e);
    }
}


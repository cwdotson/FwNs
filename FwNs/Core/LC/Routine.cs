namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cNavigators;
    using FwNs.Core.LC.cParsing;
    using FwNs.Core.LC.cResults;
    using FwNs.Core.LC.cRights;
    using FwNs.Core.LC.cSchemas;
    using FwNs.Core.LC.cStatements;
    using FwNs.Core.LC.cTables;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.LibCore.Clr;
    using System.Reflection;
    using System.Text;

    public sealed class Routine : ISchemaObject
    {
        public const int NoSql = 1;
        public const int ContainsSql = 2;
        public const int ReadsSql = 3;
        public const int ModifiesSql = 4;
        public const int LanguageDotnet = 1;
        public const int LanguageSql = 2;
        public const int ParamStyleDotnet = 1;
        public const int ParamStyleSql = 2;
        public static Routine[] EmptyArray = new Routine[0];
        public RoutineSchema routineSchema;
        private QNameManager.QName _name;
        private QNameManager.QName _specificName;
        public SqlType[] ParameterTypes;
        public int TypeGroups;
        public SqlType ReturnType;
        public SqlType[] TableType;
        public Table ReturnTable;
        public int RoutineType;
        public int Language;
        public int DataImpact;
        public int ParameterStyle;
        private bool _isDeterministic;
        private bool _isNullInputOutput;
        private bool _isNewSavepointLevel;
        private bool _isPsm;
        private bool _returnsTable;
        public Statement statement;
        public bool IsAnnonymous;
        public bool IsRecursive;
        public bool isAggregate;
        private string _methodName;
        private MethodInfo _cSharpMethod;
        public object CSharpClassInstance;
        public bool CSharpMethodWithConnection;
        private bool _isLibraryRoutine;
        public HashMappedList<string, ColumnSchema> ParameterList;
        public int ScopeVariableCount;
        public RangeVariable[] Ranges;
        public int VariableCount;
        private OrderedHashSet<QNameManager.QName> _references;
        public Table TriggerTable;
        public int TriggerType;
        public int TriggerOperation;
        public string ClassName;
        private MethodInfo _fillRowMethod;
        public HashMappedList<string, Table> ScopeTables;

        public Routine(int type)
        {
            this.DataImpact = 4;
            this._isNewSavepointLevel = true;
            this.ParameterList = new HashMappedList<string, ColumnSchema>();
            this.RoutineType = type;
            this.ReturnType = SqlType.SqlAllTypes;
            if (this.RoutineType == 0x10)
            {
                this.DataImpact = 2;
            }
            this.Ranges = new RangeVariable[] { new RangeVariable(this.ParameterList, false) };
        }

        public Routine(Table table, RangeVariable[] ranges, int impact, int triggerType, int operationType)
        {
            this.DataImpact = 4;
            this._isNewSavepointLevel = true;
            this.ParameterList = new HashMappedList<string, ColumnSchema>();
            this.RoutineType = 8;
            this.ReturnType = SqlType.SqlAllTypes;
            this.DataImpact = impact;
            this.Ranges = ranges;
            this.TriggerTable = table;
            this.TriggerType = triggerType;
            this.TriggerOperation = operationType;
        }

        public void AddParameter(ColumnSchema param)
        {
            QNameManager.QName name = param.GetName();
            string key = (name == null) ? QNameManager.GetAutoNoNameColumnString(this.ParameterList.Size()) : name.Name;
            this.ParameterList.Add(key, param);
        }

        private static void CheckNoSqlData(Database database, OrderedHashSet<QNameManager.QName> set)
        {
            for (int i = 0; i < set.Size(); i++)
            {
                QNameManager.QName name = set.Get(i);
                if (name.type == 0x18)
                {
                    Routine schemaObject = (Routine) database.schemaManager.GetSchemaObject(name);
                    if (schemaObject.DataImpact == 3)
                    {
                        throw Error.GetError(0x15e8, "READS SQL");
                    }
                    if (schemaObject.DataImpact == 4)
                    {
                        throw Error.GetError(0x15e8, "MODIFIES SQL");
                    }
                    if (name.type == 3)
                    {
                        throw Error.GetError(0x15e8, "READS SQL");
                    }
                }
            }
        }

        public void Compile(Session session, ISchemaObject parentObject)
        {
            using (Scanner scanner = new Scanner(this.statement.GetSql()))
            {
                ParserRoutine routine = new ParserRoutine(session, scanner) {
                    compileContext = { InRoutine = true }
                };
                try
                {
                    routine.Read();
                    routine.StartRecording();
                    Statement statement = routine.CompileSQLProcedureStatementOrNull(this, null);
                    string sql = Token.GetSql(routine.GetRecordedStatement());
                    statement.SetSql(sql);
                    this.SetProcedure(statement);
                    statement.Resolve(session);
                    this.SetReferences();
                }
                finally
                {
                    routine.compileContext.InRoutine = false;
                }
            }
        }

        public object[] ConvertArgsToCSharp(Session session, object[] callArguments)
        {
            int num = this.CSharpMethodWithConnection ? 1 : 0;
            object[] objArray = new object[callArguments.Length + num];
            SqlType[] parameterTypes = this.GetParameterTypes();
            for (int i = 0; i < callArguments.Length; i++)
            {
                object a = callArguments[i];
                ColumnSchema parameter = this.GetParameter(i);
                if (parameter.ParameterMode == 1)
                {
                    objArray[i + num] = parameterTypes[i].ConvertSQLToCSharp(session, a);
                }
                else if (parameter.ParameterMode == 4)
                {
                    objArray[i + num] = null;
                }
                else if (parameter.ParameterMode == 2)
                {
                    objArray[i + num] = parameterTypes[i].ConvertSQLToCSharp(session, a);
                }
                else
                {
                    object obj3 = parameterTypes[i].ConvertSQLToCSharp(session, a);
                    object obj4 = Array.CreateInstance(parameterTypes[i].GetCSharpClass(), 1);
                    ((Array) obj4).SetValue(obj3, 0);
                    objArray[i + num] = obj4;
                }
            }
            return objArray;
        }

        public void ConvertArgsToSql(Session session, object[] callArguments, object[] data)
        {
            int num = this.CSharpMethodWithConnection ? 1 : 0;
            SqlType[] parameterTypes = this.GetParameterTypes();
            for (int i = 0; i < callArguments.Length; i++)
            {
                object a = data[i + num];
                ColumnSchema parameter = this.GetParameter(i);
                if (((parameter.ParameterMode != 1) && (parameter.ParameterMode != 4)) && (parameter.ParameterMode != 2))
                {
                    a = ((Array) a).GetValue(0);
                }
                callArguments[i] = parameterTypes[i].ConvertCSharpToSQL(session, a);
            }
        }

        public Routine Duplicate()
        {
            Routine routine;
            try
            {
                routine = (Routine) base.MemberwiseClone();
            }
            catch (Exception)
            {
                throw Error.RuntimeError(0xc9, "Type");
            }
            return routine;
        }

        public QNameManager.QName GetCatalogName()
        {
            return this._name.schema.schema;
        }

        public long GetChangeTimestamp()
        {
            return 0L;
        }

        public OrderedHashSet<ISchemaObject> GetComponents()
        {
            return null;
        }

        public int GetDataImpact()
        {
            return this.DataImpact;
        }

        public string GetDataImpactString()
        {
            StringBuilder builder = new StringBuilder();
            switch (this.DataImpact)
            {
                case 1:
                    builder.Append("NO").Append(' ').Append("SQL");
                    break;

                case 2:
                    builder.Append("CONTAINS").Append(' ').Append("SQL");
                    break;

                case 3:
                    builder.Append("READS").Append(' ').Append("SQL").Append(' ').Append("DATA");
                    break;

                case 4:
                    builder.Append("MODIFIES").Append(' ').Append("SQL").Append(' ').Append("DATA");
                    break;
            }
            return builder.ToString();
        }

        public string GetDefinitionSql(bool withBody)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("CREATE").Append(' ');
            if (this.isAggregate)
            {
                builder.Append("AGGREGATE").Append(' ');
            }
            builder.Append((this.RoutineType == 0x11) ? "PROCEDURE" : "FUNCTION");
            builder.Append(' ');
            builder.Append(this._name.GetSchemaQualifiedStatementName());
            builder.Append('(');
            for (int i = 0; i < this.ParameterList.Size(); i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(this.ParameterList.Get(i).GetSql());
            }
            builder.Append(')');
            builder.Append(' ');
            if ((this.RoutineType == 0x10) || (this.RoutineType == 0x1b))
            {
                builder.Append("RETURNS");
                builder.Append(' ');
                if (this._returnsTable)
                {
                    builder.Append("TABLE");
                    builder.Append(this.ReturnTable.GetColumnListWithTypeSql());
                }
                else
                {
                    builder.Append(this.ReturnType.GetTypeDefinition());
                }
                builder.Append(' ');
            }
            if (this._specificName != null)
            {
                builder.Append("SPECIFIC");
                builder.Append(' ');
                builder.Append(this._specificName.GetStatementName());
                builder.Append(' ');
            }
            builder.Append("LANGUAGE");
            builder.Append(' ');
            builder.Append((this.Language == 1) ? "DOTNET" : "SQL");
            builder.Append(' ');
            if (!this._isDeterministic)
            {
                builder.Append("NOT");
                builder.Append(' ');
            }
            builder.Append("DETERMINISTIC");
            builder.Append(' ');
            builder.Append(this.GetDataImpactString());
            builder.Append(' ');
            if ((this.RoutineType == 0x10) || (this.RoutineType == 0x1b))
            {
                if (this._isNullInputOutput)
                {
                    builder.Append("RETURNS").Append(' ').Append("NULL");
                }
                else
                {
                    builder.Append("CALLED");
                }
                builder.Append(' ').Append("ON").Append(' ');
                builder.Append("NULL").Append(' ').Append("INPUT");
                builder.Append(' ');
            }
            else
            {
                builder.Append(this._isNewSavepointLevel ? "NEW" : "OLD");
                builder.Append(' ').Append("SAVEPOINT").Append(' ');
                builder.Append("LEVEL").Append(' ');
            }
            if (this.Language == 1)
            {
                builder.Append("EXTERNAL").Append(' ').Append("NAME");
                builder.Append(' ').Append('\'').Append(this._methodName).Append('\'');
            }
            else if (withBody)
            {
                builder.Append(this.statement.GetSql());
            }
            else
            {
                builder.Append("SIGNAL").Append(' ');
                builder.Append("SQLSTATE").Append(' ');
                builder.Append('\'').Append("45000").Append('\'');
            }
            return builder.ToString();
        }

        private MethodInfo GetFillRowMethod(string name)
        {
            MethodInfo method = this._cSharpMethod.DeclaringType.GetMethod(name);
            if (method != null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    return null;
                }
                if ((parameters[0].ParameterType != typeof(object)) || parameters[0].IsOut)
                {
                    return null;
                }
                SqlType[] columnTypes = this.ReturnTable.GetColumnTypes();
                if (columnTypes.Length != (parameters.Length - 1))
                {
                    return null;
                }
                for (int i = 1; i < columnTypes.Length; i++)
                {
                    if (!parameters[i].IsOut)
                    {
                        return null;
                    }
                    if (columnTypes[i - 1].TypeCode != Types.GetParameterSqlType(parameters[i].ParameterType.GetElementType()).TypeCode)
                    {
                        return null;
                    }
                }
            }
            return method;
        }

        public object[] GetFillRowTemplate()
        {
            object[] objArray1 = new object[this.ReturnTable.GetColumnTypes().Length + 1];
            objArray1[0] = null;
            return objArray1;
        }

        public int GetLanguage()
        {
            return this.Language;
        }

        public MethodInfo GetMethod()
        {
            return this._cSharpMethod;
        }

        private MethodInfo GetMethod(string name, Routine routine, bool[] hasConnection, bool returnsTable)
        {
            MethodInfo[] methods = this.GetMethods(name);
            int i = -1;
            for (int j = 0; j < methods.Length; j++)
            {
                int num4;
                int num3 = 0;
                hasConnection[0] = false;
                MethodInfo info = methods[j];
                ParameterInfo[] parameters = info.GetParameters();
                if ((parameters.Length != 0) && parameters[0].ParameterType.Equals(typeof(DbConnection)))
                {
                    num3 = 1;
                    hasConnection[0] = true;
                }
                if ((parameters.Length - num3) != routine.ParameterTypes.Length)
                {
                    continue;
                }
                if (returnsTable)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(info.ReturnType))
                    {
                        goto Label_00AB;
                    }
                    continue;
                }
                SqlType parameterSqlType = Types.GetParameterSqlType(methods[j].ReturnType);
                if ((parameterSqlType == null) || (parameterSqlType.TypeCode != routine.ReturnType.TypeCode))
                {
                    continue;
                }
            Label_00AB:
                num4 = 0;
                while (num4 < routine.ParameterTypes.Length)
                {
                    bool flag = false;
                    ParameterInfo info2 = parameters[num4 + num3];
                    Type parameterType = info2.ParameterType;
                    if (parameterType.IsArray && !typeof(byte[]).Equals(parameterType))
                    {
                        parameterType = parameterType.GetElementType();
                        if (parameterType.IsPrimitive)
                        {
                            info = null;
                            break;
                        }
                        flag = true;
                    }
                    SqlType type3 = Types.GetParameterSqlType(parameterType);
                    if (type3 == null)
                    {
                        info = null;
                        break;
                    }
                    ColumnSchema parameter = routine.GetParameter(num4);
                    if (parameter.GetParameterMode() == 1)
                    {
                        if (!info2.IsOut && !parameterType.IsByRef)
                        {
                            goto Label_0183;
                        }
                        info = null;
                        break;
                    }
                    if (parameter.GetParameterMode() == 4)
                    {
                        if (info2.IsOut && parameterType.IsByRef)
                        {
                            goto Label_0183;
                        }
                        info = null;
                        break;
                    }
                    if ((parameter.GetParameterMode() == 2) && !parameterType.IsByRef)
                    {
                        info = null;
                        break;
                    }
                Label_0183:
                    parameter.SetNullable(!parameters[j].ParameterType.IsPrimitive);
                    bool flag2 = routine.ParameterTypes[num4].TypeComparisonGroup == type3.TypeComparisonGroup;
                    if (flag2 && routine.ParameterTypes[num4].IsNumberType())
                    {
                        flag2 = routine.ParameterTypes[num4].TypeCode == type3.TypeCode;
                    }
                    if (flag && (routine.GetParameter(num4).ParameterMode == 1))
                    {
                        flag2 = false;
                    }
                    if (!flag2)
                    {
                        info = null;
                        if ((num4 + num3) > i)
                        {
                            i = num4 + num3;
                        }
                        break;
                    }
                    num4++;
                }
                if (info != null)
                {
                    for (int k = 0; k < routine.ParameterTypes.Length; k++)
                    {
                        routine.GetParameter(k).SetNullable(!parameters[k + num3].ParameterType.IsPrimitive);
                    }
                    return info;
                }
            }
            if (i >= 0)
            {
                ColumnSchema parameter = routine.GetParameter(i);
                throw Error.GetError(0x1785, parameter.GetNameString());
            }
            return null;
        }

        public MethodInfo[] GetMethods(string name)
        {
            int length = name.LastIndexOf('.');
            if (length == -1)
            {
                throw Error.GetError(0x157d, name);
            }
            string str = name.Substring(0, length);
            string str2 = name.Substring(length + 1);
            Type type = null;
            try
            {
                int index = str.IndexOf(":");
                if (index != -1)
                {
                    str = str.Substring(index + 1);
                    type = Assembly.Load(str.Substring(0, index)).GetType(str);
                }
                else
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        type = assemblies[i].GetType(str);
                        if (type != null)
                        {
                            break;
                        }
                    }
                }
                if (type == null)
                {
                    throw Error.GetError(0x5dc);
                }
            }
            catch (Exception exception)
            {
                object[] add = new object[] { exception.Message, str };
                throw Error.GetError(exception, 0x157d, 0x1a, add);
            }
            MethodInfo[] methods = type.GetMethods();
            List<MethodInfo> list = new List<MethodInfo>();
            for (length = 0; length < methods.Length; length++)
            {
                int num4 = 0;
                MethodInfo info = methods[length];
                if ((info.Name.Equals(str2) && (info.IsStatic || ((this.IsAggregate() || (info.GetCustomAttributes(typeof(SqlProcedure), false).Length != 0)) && (!this.IsAggregate() || (info.GetCustomAttributes(typeof(SqlUserDefinedAggregate), false).Length != 0))))) && info.IsPublic)
                {
                    ParameterInfo[] parameters = methods[length].GetParameters();
                    if ((parameters.Length != 0) && parameters[0].ParameterType.Equals(typeof(DbConnection)))
                    {
                        num4 = 1;
                    }
                    for (int i = num4; i < parameters.Length; i++)
                    {
                        Type parameterType = parameters[i].ParameterType;
                        if (parameterType.IsArray && !typeof(byte[]).Equals(parameterType))
                        {
                            parameterType = parameterType.GetElementType();
                            if (parameterType.IsPrimitive)
                            {
                                info = null;
                                break;
                            }
                        }
                        if (Types.GetParameterSqlType(parameterType) == null)
                        {
                            info = null;
                            break;
                        }
                    }
                    if (info != null)
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(info.ReturnType))
                        {
                            list.Add(methods[length]);
                        }
                        else if (Types.GetParameterSqlType(info.ReturnType) != null)
                        {
                            list.Add(methods[length]);
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public QNameManager.QName GetName()
        {
            return this._name;
        }

        public Grantee GetOwner()
        {
            return this._name.schema.Owner;
        }

        public ColumnSchema GetParameter(int i)
        {
            return this.ParameterList.Get(i);
        }

        public int GetParameterCount()
        {
            return this.ParameterTypes.Length;
        }

        public int GetParameterIndex(string name)
        {
            return this.ParameterList.GetIndex(name);
        }

        public RangeVariable[] GetParameterRangeVariables()
        {
            return this.Ranges;
        }

        public int GetParameterSignature()
        {
            return this.TypeGroups;
        }

        public Table GetParameterTable(string name)
        {
            if (this.ScopeTables == null)
            {
                return null;
            }
            return this.ScopeTables.Get(name);
        }

        public SqlType[] GetParameterTypes()
        {
            return this.ParameterTypes;
        }

        public Statement GetProcedure()
        {
            return this.statement;
        }

        public OrderedHashSet<QNameManager.QName> GetReferences()
        {
            return this._references;
        }

        private ResultMetaData GetResultTableMetaData()
        {
            ResultMetaData data = ResultMetaData.NewResultMetaData(this.ReturnTable.GetColumnCount());
            for (int i = 0; i < this.ReturnTable.GetColumnCount(); i++)
            {
                data.columns[i] = this.ReturnTable.GetColumn(i);
                data.ColumnTypes[i] = this.ReturnTable.GetColumnTypes()[i];
            }
            return data;
        }

        public SqlType GetReturnType()
        {
            return this.ReturnType;
        }

        public QNameManager.QName GetSchemaName()
        {
            return this._name.schema;
        }

        public int GetSchemaObjectType()
        {
            return this.RoutineType;
        }

        public QNameManager.QName GetSpecificName()
        {
            return this._specificName;
        }

        public string GetSql()
        {
            return this.GetDefinitionSql(true);
        }

        public string GetSqlAlter()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("ALTER").Append(' ').Append("SPECIFIC");
            builder1.Append(' ').Append("ROUTINE").Append(' ');
            builder1.Append(this._specificName.GetSchemaQualifiedStatementName());
            builder1.Append(' ').Append("SET");
            builder1.Append(' ').Append("BODY");
            builder1.Append(' ').Append(this.statement.GetSql());
            return builder1.ToString();
        }

        public string GetSqlDeclaration()
        {
            return this.GetDefinitionSql(false);
        }

        public Table GetTable()
        {
            return this.ReturnTable;
        }

        public QNameManager.QName[] GetTableNamesForRead()
        {
            if (this.statement == null)
            {
                return QNameManager.QName.EmptyArray;
            }
            return this.statement.GetTableNamesForRead();
        }

        public QNameManager.QName[] GetTableNamesForWrite()
        {
            if (this.statement == null)
            {
                return QNameManager.QName.EmptyArray;
            }
            return this.statement.GetTableNamesForWrite();
        }

        public SqlType[] GetTableType()
        {
            return this.TableType;
        }

        public int GetVariableCount()
        {
            return this.VariableCount;
        }

        public Result InvokeClrMethod(Session session, object[] data)
        {
            try
            {
                object obj2;
                if (this.DataImpact == 1)
                {
                    session.sessionContext.IsReadOnly = true;
                    session.SetNoSql();
                }
                else if (this.DataImpact == 2)
                {
                    session.sessionContext.IsReadOnly = true;
                }
                else if (this.DataImpact == 3)
                {
                    session.sessionContext.IsReadOnly = true;
                }
                try
                {
                    UtlContext.session = session;
                    obj2 = this._cSharpMethod.Invoke(this.CSharpClassInstance, data);
                }
                finally
                {
                    UtlContext.session = null;
                }
                if (this.ReturnsTable())
                {
                    IEnumerable enumerable = obj2 as IEnumerable;
                    if ((obj2 == null) || (enumerable == null))
                    {
                        throw Error.RuntimeError(0xc9, "FunctionSQLInvoked");
                    }
                    ResultMetaData resultTableMetaData = this.GetResultTableMetaData();
                    Result result = Result.NewDataResult(resultTableMetaData);
                    RowSetNavigator navigator = result.InitialiseNavigator();
                    object[] fillRowTemplate = this.GetFillRowTemplate();
                    foreach (object obj3 in enumerable)
                    {
                        object[] objArray2 = new object[resultTableMetaData.GetColumnCount()];
                        fillRowTemplate[0] = obj3;
                        this.FillRowMethod.Invoke(this.CSharpClassInstance, fillRowTemplate);
                        for (int i = 0; i < objArray2.Length; i++)
                        {
                            objArray2[i] = fillRowTemplate[i + 1];
                        }
                        navigator.Add(objArray2);
                    }
                    return result;
                }
                return Result.NewPsmResult(this.ReturnType.ConvertCSharpToSQL(session, obj2));
            }
            catch (TargetInvocationException)
            {
                return Result.NewErrorResult(Error.GetError(0x1770, this.GetName().Name), null);
            }
            catch (Exception)
            {
                return Result.NewErrorResult(Error.GetError(0x1770, this.GetName().Name), null);
            }
        }

        public bool IsAggregate()
        {
            return this.isAggregate;
        }

        public bool IsDeterministic()
        {
            return this._isDeterministic;
        }

        public bool IsFunction()
        {
            return (this.RoutineType == 0x10);
        }

        public bool IsLibraryRoutine()
        {
            return this._isLibraryRoutine;
        }

        public bool IsNullInputOutput()
        {
            return this._isNullInputOutput;
        }

        public bool IsProcedure()
        {
            return (this.RoutineType == 0x11);
        }

        public bool IsPsm()
        {
            return this._isPsm;
        }

        public bool IsTrigger()
        {
            return (this.RoutineType == 8);
        }

        public void Resolve(Session session)
        {
            this.SetLanguage(this.Language);
            if (this.Language == 2)
            {
                if (this.DataImpact == 1)
                {
                    throw Error.GetError(0x15e4);
                }
                if (this.ParameterStyle == 1)
                {
                    throw Error.GetError(0x15e4);
                }
            }
            if (((this.Language == 2) && (this.ParameterStyle != 0)) && (this.ParameterStyle != 2))
            {
                throw Error.GetError(0x15e4);
            }
            this.ParameterTypes = new SqlType[this.ParameterList.Size()];
            this.TypeGroups = 0;
            for (int i = 0; i < this.ParameterTypes.Length; i++)
            {
                ColumnSchema schema = this.ParameterList.Get(i);
                this.ParameterTypes[i] = schema.DataType;
                if (i < 4)
                {
                    BitMap.SetByte(this.TypeGroups, (byte) schema.DataType.TypeComparisonGroup, i * 8);
                }
            }
            if (this.statement != null)
            {
                this.statement.Resolve(session, this.Ranges);
                if (this.DataImpact == 2)
                {
                    CheckNoSqlData(session.database, this.statement.GetReferences());
                }
            }
            if ((this._methodName != null) && (this._cSharpMethod == null))
            {
                bool[] hasConnection = new bool[1];
                this._cSharpMethod = this.GetMethod(this._methodName, this, hasConnection, this._returnsTable);
                if (this._cSharpMethod == null)
                {
                    throw Error.GetError(0x177d);
                }
                if (!this._cSharpMethod.IsStatic)
                {
                    this.CSharpClassInstance = Activator.CreateInstance(this._cSharpMethod.DeclaringType);
                    if (this.CSharpClassInstance == null)
                    {
                        throw Error.GetError(0x177d);
                    }
                }
                this.CSharpMethodWithConnection = hasConnection[0];
                if (this._cSharpMethod.DeclaringType.FullName.Equals("System.Math"))
                {
                    this._isLibraryRoutine = true;
                }
            }
            if (this.isAggregate)
            {
                if (this.ParameterTypes.Length < 4)
                {
                    throw Error.GetError(0x15ea);
                }
                bool flag = this.ParameterTypes[this.ParameterList.Size() - 3].TypeCode == 0x10;
                ColumnSchema schema2 = this.ParameterList.Get(0);
                flag &= schema2.GetParameterMode() == 1;
                int index = 1;
                while (index < (this.ParameterList.Size() - 3))
                {
                    schema2 = this.ParameterList.Get(index);
                    flag &= schema2.GetParameterMode() == 1;
                    index++;
                }
                schema2 = this.ParameterList.Get(index);
                flag &= schema2.GetParameterMode() == 1;
                schema2 = this.ParameterList.Get(index + 1);
                flag &= schema2.GetParameterMode() == 2;
                schema2 = this.ParameterList.Get(index + 2);
                if (!(flag & (schema2.GetParameterMode() == 2)))
                {
                    throw Error.GetError(0x15ea);
                }
            }
            else if ((this._methodName != null) && this.ReturnsTable())
            {
                object[] customAttributes = this._cSharpMethod.GetCustomAttributes(typeof(SqlFunction), false);
                if ((customAttributes == null) || (customAttributes.Length == 0))
                {
                    throw Error.GetError(0x177d);
                }
                string fillRowMethodName = ((SqlFunction) customAttributes[0]).FillRowMethodName;
                this._fillRowMethod = this.GetFillRowMethod(fillRowMethodName);
                if (this._fillRowMethod == null)
                {
                    throw Error.GetError(0x177d);
                }
            }
            this.SetReferences();
        }

        public void ResolveReferences(Session session)
        {
            if (this.statement != null)
            {
                this.statement.Resolve(session);
            }
            if ((this._methodName != null) && (this._cSharpMethod == null))
            {
                bool[] hasConnection = new bool[1];
                this._cSharpMethod = this.GetMethod(this._methodName, this, hasConnection, this._returnsTable);
                if (this._cSharpMethod == null)
                {
                    throw Error.GetError(0x177d);
                }
                if (!this._cSharpMethod.IsStatic)
                {
                    this.CSharpClassInstance = Activator.CreateInstance(this._cSharpMethod.DeclaringType);
                    if (this.CSharpClassInstance == null)
                    {
                        throw Error.GetError(0x177d);
                    }
                }
                this.CSharpMethodWithConnection = hasConnection[0];
                if (this._cSharpMethod.DeclaringType.FullName.Equals("System.Math"))
                {
                    this._isLibraryRoutine = true;
                }
            }
            this.SetReferences();
        }

        public bool ReturnsTable()
        {
            return this._returnsTable;
        }

        public void SetAggregate(bool isAggregate)
        {
            this.isAggregate = isAggregate;
        }

        public void SetAsAlteredRoutine(Routine routine)
        {
            this._cSharpMethod = routine._cSharpMethod;
            this.statement = routine.statement;
            this._references = routine._references;
            this.IsRecursive = routine.IsRecursive;
            this.VariableCount = routine.VariableCount;
        }

        public void SetDataImpact(int impact)
        {
            this.DataImpact = impact;
        }

        public void SetDeterministic(bool value)
        {
            this._isDeterministic = value;
        }

        public void SetLanguage(int lang)
        {
            this.Language = lang;
            this._isPsm = this.Language == 2;
        }

        public void SetMethod(MethodInfo method)
        {
            this._cSharpMethod = method;
        }

        public void SetMethodFqn(string fqn)
        {
            this._methodName = fqn;
        }

        public void SetName(QNameManager.QName name)
        {
            this._name = name;
        }

        public void SetNewSavepointLevel(bool value)
        {
            this._isNewSavepointLevel = value;
        }

        public void SetNullInputOutput(bool value)
        {
            this._isNullInputOutput = value;
        }

        public void SetParameterStyle(int style)
        {
            this.ParameterStyle = style;
        }

        public void SetProcedure(Statement statement)
        {
            this.statement = statement;
        }

        private void SetReferences()
        {
            OrderedHashSet<QNameManager.QName> set = new OrderedHashSet<QNameManager.QName>();
            for (int i = 0; i < this.ParameterTypes.Length; i++)
            {
                set.AddAll(this.ParameterList.Get(i).GetReferences());
            }
            if (this.statement != null)
            {
                set.AddAll(this.statement.GetReferences());
            }
            if (set.Contains(this.GetSpecificName()))
            {
                set.Remove(this.GetSpecificName());
                this.IsRecursive = true;
            }
            this._references = set;
        }

        public void SetReturnTable(TableDerived table)
        {
            this.ReturnTable = table;
            this._returnsTable = true;
            SqlType[] columnTypes = table.GetColumnTypes();
            this.ReturnType = new RowType(columnTypes);
        }

        public void SetReturnType(SqlType type)
        {
            this.ReturnType = type;
        }

        public void SetSpecificName(QNameManager.QName name)
        {
            this._specificName = name;
        }

        public void SetTableType(SqlType[] types)
        {
            this.TableType = types;
        }

        public MethodInfo FillRowMethod
        {
            get
            {
                return this._fillRowMethod;
            }
        }
    }
}


namespace FwNs.Data
{
    using System;
    using System.Data.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class CtxtXtnz
    {
        public static readonly Func<DbRootCtxt, MethodInfo> FuncGetTableMethod;
        public static readonly Func<DbRootCtxt, string, ITable> FuncITable;

        static CtxtXtnz()
        {
            FuncGetTableMethod = new Func<DbRootCtxt, MethodInfo>(<>c.<>9, this.<.cctor>b__7_0);
            FuncITable = new Func<DbRootCtxt, string, ITable>(<>c.<>9, this.<.cctor>b__7_1);
        }

        public static string ConStr_FSysDb1()
        {
            return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\aCode\data\FSysDb1.accdb;Jet OLEDB:Create System Database=True";
        }

        public static string ConStr_ICtnrSQL()
        {
            return @"Data Source=AX3950\SQLEXPRESS;Initial Catalog=ICtnrSQL;Integrated Security=True";
        }

        public static string ConStr_StrgDb1SQL()
        {
            return @"Data Source=AX3950\SQLEXPRESS;Initial Catalog=StrgDb1SQL;Integrated Security=True";
        }

        public static string GetConStr()
        {
            throw new Exception();
        }

        public static string GetConStr(string dbNm)
        {
            string str = Environment.MachineName.ToUpper();
            string str2 = str.StartsWith("XPS") ? "SQL2K12" : (str.EndsWith("4400") ? (dbNm.ToLower().StartsWith("DbRoots".ToLower()) ? "sqlm4400" : "sqlm4400") : "SqlExpress");
            string[] textArray1 = new string[] { "Data Source=", str, @"\", str2, ";Initial Catalog=", dbNm, ";Integrated Security=True" };
            return string.Concat(textArray1);
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly CtxtXtnz.<>c <>9 = new CtxtXtnz.<>c();

            internal MethodInfo <.cctor>b__7_0(DbRootCtxt ctx)
            {
                Type[] types = new Type[] { typeof(Type) };
                return ctx.GetType().GetMethod("GetTable", types);
            }

            internal ITable <.cctor>b__7_1(DbRootCtxt ctx, string x)
            {
                object[] parameters = new object[] { Type.GetType(x) };
                return (ITable) CtxtXtnz.FuncGetTableMethod.Invoke(ctx).Invoke(ctx, parameters);
            }
        }
    }
}


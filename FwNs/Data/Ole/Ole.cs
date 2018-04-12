namespace FwNs.Data.Ole
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;

    [Extension]
    public static class Ole
    {
        public static void CopyAttr2Elmnt(XElement arg)
        {
            for (int i = 0; i < Enumerable.Count<XAttribute>(arg.Attributes()); i++)
            {
                XAttribute attribute = Enumerable.ElementAt<XAttribute>(arg.Attributes(), i);
                if (arg.Element(attribute.Name) == null)
                {
                    arg.Add(new XElement(attribute.Name, attribute.Value));
                }
                else
                {
                    arg.SetElementValue(attribute.Name, attribute.Value);
                }
            }
        }

        public static void CopyAttr2Elmnts(XElement arg)
        {
            if (arg.HasElements)
            {
                CopyAttr2Elmnt(arg);
                for (int i = 0; i < Enumerable.Count<XElement>(arg.Elements()); i++)
                {
                    CopyAttr2Elmnts(Enumerable.ElementAt<XElement>(arg.Elements(), i));
                }
            }
            else
            {
                CopyAttr2Elmnt(arg);
            }
        }

        [Extension]
        public static FileInfo GetDb(DirectoryInfo arg)
        {
            if ((arg.GetFiles("xdrDB.accdb") != null) && (Enumerable.Count<FileInfo>(arg.GetFiles("xdrDB.accdb")) > 0))
            {
                return arg.GetFiles("xdrDB.accdb")[0];
            }
            return null;
        }

        [Extension]
        public static OleDbConnection GetXDb(DirectoryInfo arg)
        {
            GetDb(arg);
            throw new Exception();
        }

        [Extension]
        public static DirectoryInfo GetXDD(DirectoryInfo arg)
        {
            if (arg.GetDirectories("xd") != null)
            {
                return arg.GetDirectories("xd")[0];
            }
            return null;
        }

        [Extension]
        public static KeyValuePair<string, XElement>[] GetXDF(DirectoryInfo arg1, string arg2)
        {
            <>c__DisplayClass0_0 class_;
            int lastD = 0;
            if (arg1.Exists && (Enumerable.Count<DirectoryInfo>(arg1.EnumerateDirectories()) > 0))
            {
                lastD = Enumerable.Max(Enumerable.ToArray<int>(Enumerable.Select(Enumerable.Select(arg1.EnumerateDirectories(), <>c.<>9__0_1 ?? (<>c.<>9__0_1 = new Func<DirectoryInfo, <>f__AnonymousType0<DirectoryInfo, string>>(<>c.<>9, this.<GetXDF>b__0_1))), <>c.<>9__0_2 ?? (<>c.<>9__0_2 = new Func<<>f__AnonymousType0<DirectoryInfo, string>, int>(<>c.<>9, this.<GetXDF>b__0_2)))));
            }
            return Enumerable.ToArray<KeyValuePair<string, XElement>>(Enumerable.Select(Enumerable.Where(Enumerable.Select(Enumerable.ToArray<string>(Enumerable.Where<string>(Directory.GetDirectories(arg2, "*", SearchOption.TopDirectoryOnly), <>c.<>9__0_0 ?? (<>c.<>9__0_0 = new Func<string, bool>(<>c.<>9, this.<GetXDF>b__0_0)))), <>c.<>9__0_3 ?? (<>c.<>9__0_3 = new Func<string, <>f__AnonymousType1<string, XElement>>(<>c.<>9, this.<GetXDF>b__0_3))), new Func<<>f__AnonymousType1<string, XElement>, bool>(class_, this.<GetXDF>b__4)), <>c.<>9__0_5 ?? (<>c.<>9__0_5 = new Func<<>f__AnonymousType1<string, XElement>, KeyValuePair<string, XElement>>(<>c.<>9, this.<GetXDF>b__0_5))));
        }

        [Extension]
        public static XElement GetXDX(DirectoryInfo arg)
        {
            if (arg.GetFiles("xd.xml") != null)
            {
                return XElement.Load(arg.GetFiles("xd.xml")[0].FullName);
            }
            return null;
        }

        public static void MakeDs()
        {
        }

        public static void UpdateDB(DirectoryInfo arg, DataSet DS)
        {
            OleDbConnection xDb = GetXDb(arg);
            xDb.Open();
            List<string> list1 = new List<string> { 
                "-",
                " "
            };
            foreach (DataTable table in DS.Tables)
            {
                try
                {
                    OleDbDataAdapter adapter1 = new OleDbDataAdapter();
                    OleDbCommand command1 = new OleDbCommand {
                        CommandType = CommandType.TableDirect,
                        CommandText = table.TableName,
                        Connection = xDb
                    };
                    adapter1.InsertCommand = command1;
                    adapter1.Update(DS, table.TableName);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            xDb.Close();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FwNs.Data.Ole.Ole.<>c <>9 = new FwNs.Data.Ole.Ole.<>c();
            public static Func<string, bool> <>9__0_0;
            public static Func<DirectoryInfo, <>f__AnonymousType0<DirectoryInfo, string>> <>9__0_1;
            public static Func<<>f__AnonymousType0<DirectoryInfo, string>, int> <>9__0_2;
            public static Func<string, <>f__AnonymousType1<string, XElement>> <>9__0_3;
            public static Func<<>f__AnonymousType1<string, XElement>, KeyValuePair<string, XElement>> <>9__0_5;

            internal bool <GetXDF>b__0_0(string u)
            {
                return (Enumerable.Count<string>(Directory.GetFiles(u, "xd.xml", SearchOption.TopDirectoryOnly)) == 1);
            }

            internal <>f__AnonymousType0<DirectoryInfo, string> <GetXDF>b__0_1(DirectoryInfo u)
            {
                return new { 
                    u = u,
                    nm = u.FullName
                };
            }

            internal int <GetXDF>b__0_2(<>f__AnonymousType0<DirectoryInfo, string> <>h__TransparentIdentifier0)
            {
                char[] separator = new char[] { '_' };
                return Convert.ToInt32(Enumerable.Last<string>(<>h__TransparentIdentifier0.nm.Substring(<>h__TransparentIdentifier0.nm.LastIndexOf('\\')).Split(separator)));
            }

            internal <>f__AnonymousType1<string, XElement> <GetXDF>b__0_3(string u)
            {
                return new { 
                    u = u,
                    xu = XElement.Load(Path.Combine(u, "xd.xml"))
                };
            }

            internal KeyValuePair<string, XElement> <GetXDF>b__0_5(<>f__AnonymousType1<string, XElement> <>h__TransparentIdentifier0)
            {
                return new KeyValuePair<string, XElement>(<>h__TransparentIdentifier0.u, <>h__TransparentIdentifier0.xu);
            }
        }
    }
}


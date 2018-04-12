namespace FwNs.FSys
{
    using FwNs.Core;
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    [Extension]
    public static class FSysFw
    {
        [Extension]
        public static void PileText(DirectoryInfo dNfoIn, FileInfo outFile, string fltr, int szLim)
        {
            int num = 0;
            StringBuilder builder = new StringBuilder();
            if (<>c.<>9__1_0 == null)
            {
                <>c.<>9__1_0 = new Func<string, string>(<>c.<>9, this.<PileText>b__1_0);
            }
            if (<>c.<>9__1_1 == null)
            {
                <>c.<>9__1_1 = new Func<string, Action>(<>c.<>9, this.<PileText>b__1_1);
            }
            if (<>c.<>9__1_3 == null)
            {
                <>c.<>9__1_3 = new Func<string, Func<string, string>, int, string>(<>c.<>9, this.<PileText>b__1_3);
            }
            Func<string, string> func = (<>c.<>9__1_4 ?? (<>c.<>9__1_4 = new Func<DirectoryInfo, Func<string, string>, Func<string, string>>(<>c.<>9, this.<PileText>b__1_4))).Invoke(outFile.Directory, <>c.<>9__1_6 ?? (<>c.<>9__1_6 = new Func<string, string>(<>c.<>9, this.<PileText>b__1_6)));
            Func<string, int, string> func2 = (<>c.<>9__1_7 ?? (<>c.<>9__1_7 = new Func<Func<int, string>, Func<string, int, string>>(<>c.<>9, this.<PileText>b__1_7))).Invoke(<>c.<>9__1_9 ?? (<>c.<>9__1_9 = new Func<int, string>(<>c.<>9, this.<PileText>b__1_9)));
            Func<Func<string, string>, Func<string, int, string>, Func<FileInfo, Func<int, string>>> func1 = <>c.<>9__1_10 ?? (<>c.<>9__1_10 = new Func<Func<string, string>, Func<string, int, string>, Func<FileInfo, Func<int, string>>>(<>c.<>9, this.<PileText>b__1_10));
            func1.Invoke(func, func2);
            Func<int, string> func3 = func1.Invoke(func, func2).Invoke(outFile);
            if (<>c.<>9__1_13 == null)
            {
                <>c.<>9__1_13 = new Func<string, Action>(<>c.<>9, this.<PileText>b__1_13);
            }
            foreach (FileInfo fi in dNfoIn.EnumerateFiles())
            {
                string txt = "";
                if (TryActn(() => txt = txt + File.ReadAllText(fi.FullName)))
                {
                    if ((0 < szLim) && (szLim < (txt.Length + builder.Length)))
                    {
                        string path = func3.Invoke(num);
                        if (0 < num)
                        {
                            string directoryName = Path.GetDirectoryName(path);
                            if (Directory.Exists(directoryName))
                            {
                                throw new Exception();
                            }
                            Directory.CreateDirectory(directoryName);
                        }
                        File.AppendAllText(path, builder.ToString());
                        builder.Clear();
                        num++;
                    }
                    builder.Append(txt);
                }
            }
            if (0 < num)
            {
                for (int i = 0; i < num; i++)
                {
                    string tmpFile = func3.Invoke(i);
                    string text1 = string.Format("//{0}({1})", tmpFile, DateTime.Now);
                    if (!TryActn(() => text1 = text1 + File.ReadAllText(tmpFile)))
                    {
                        throw new Exception();
                    }
                    if (!TryActn(() => File.AppendAllText(outFile.FullName, text1)))
                    {
                        throw new Exception();
                    }
                    File.Delete(tmpFile);
                    if ((num - i) == 1)
                    {
                        string directoryName = Path.GetDirectoryName(tmpFile);
                        if (0 < Enumerable.Count<string>(Directory.GetFiles(directoryName)))
                        {
                            throw new Exception();
                        }
                        Directory.Delete(directoryName);
                    }
                }
            }
            else
            {
                File.AppendAllText(outFile.FullName, builder.ToString());
            }
        }

        [Extension]
        public static void PileText(DirectoryInfo dNfoIn, string fOutPath, string fltr, int szLim)
        {
            PileText(dNfoIn, new FileInfo(fOutPath), fltr, szLim);
        }

        [Extension]
        public static void PileText(DirectoryInfo dNfoIn, DirectoryInfo dNfoOut, string fOutNm0, string fltr, int szLim)
        {
            fOutNm0 = fOutNm0 + (fOutNm0.Contains(".") ? "" : ".txt");
            string path = Path.Combine(dNfoOut.FullName, fOutNm0);
            if (File.Exists(path))
            {
                <>c__DisplayClass3_0 class_;
                Enumerable.ToArray<FileInfo>(Enumerable.Where<FileInfo>(dNfoOut.GetFiles(fOutNm0.Replace(".", "*.")), new Func<FileInfo, bool>(class_, this.<PileText>b__0)));
                path = Path.Combine(dNfoOut.FullName, fOutNm0.Replace(".", "(" + Enumerable.Count<FileInfo>(dNfoOut.GetFiles(fOutNm0.Replace(".", "*."))) + ")."));
            }
            PileText(dNfoIn, path, fltr, szLim);
        }

        [Extension]
        public static void PileText(DirectoryInfo dNfoIn, string dOutNm, string fOutNm, string fltr, int szLim)
        {
            DirectoryInfo dNfoOut = new DirectoryInfo(dOutNm);
            if (!dNfoOut.Exists)
            {
                dNfoOut.Create();
            }
            PileText(dNfoIn, dNfoOut, fOutNm, fltr, szLim);
            dNfoOut.Refresh();
            if (Enumerable.Count<FileInfo>(dNfoOut.GetFiles()) < 1)
            {
                throw new Exception();
            }
        }

        public static void PileText(string dInNm, string dOutNm, string fOutNm, string fltr, int szLim)
        {
            PileText(new DirectoryInfo(dInNm), dOutNm, fOutNm, fltr, szLim);
        }

        [Extension]
        public static void PileTextRun(DirectoryInfo dIn, DirectoryInfo dOut, bool dltOld)
        {
            <>c__DisplayClass0_0 class_;
            if (dltOld && (0 < Enumerable.Count<FileInfo>(dOut.GetFiles())))
            {
                dOut.Delete(true);
                dOut.Create();
            }
            Func<string, string> func = new Func<string, string>(class_, this.<PileTextRun>b__0);
            Func<string, string> func2 = <>c.<>9__0_1 ?? (<>c.<>9__0_1 = new Func<string, string>(<>c.<>9, this.<PileTextRun>b__0_1));
            foreach (DirectoryInfo info in dIn.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                string fullName = info.FullName;
                string dOutNm = func.Invoke(fullName);
                string fOutNm = func2.Invoke(info.FullName);
                PileText(fullName, dOutNm, fOutNm, "*.cs", -1);
            }
        }

        private static bool TryActn(Voyd voyd)
        {
            throw new NotImplementedException();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FSysFw.<>c <>9 = new FSysFw.<>c();
            public static Func<string, string> <>9__0_1;
            public static Func<string, string> <>9__1_0;
            public static Func<string, Action> <>9__1_1;
            public static Func<string, Func<string, string>, int, string> <>9__1_3;
            public static Func<DirectoryInfo, Func<string, string>, Func<string, string>> <>9__1_4;
            public static Func<string, string> <>9__1_6;
            public static Func<Func<int, string>, Func<string, int, string>> <>9__1_7;
            public static Func<int, string> <>9__1_9;
            public static Func<Func<string, string>, Func<string, int, string>, Func<FileInfo, Func<int, string>>> <>9__1_10;
            public static Func<string, Action> <>9__1_13;

            internal string <PileText>b__1_0(string s)
            {
                return Path.GetFileNameWithoutExtension(s);
            }

            internal Action <PileText>b__1_1(string s)
            {
                FSysFw.<>c__DisplayClass1_0 class_1;
                return new Action(class_1, this.<PileText>b__2);
            }

            internal Func<FileInfo, Func<int, string>> <PileText>b__1_10(Func<string, string> nfof, Func<string, int, string> tmpFileNmF)
            {
                FSysFw.<>c__DisplayClass1_3 class_1;
                return new Func<FileInfo, Func<int, string>>(class_1, this.<PileText>b__11);
            }

            internal Action <PileText>b__1_13(string s)
            {
                FSysFw.<>c__DisplayClass1_5 class_1;
                return new Action(class_1, this.<PileText>b__14);
            }

            internal string <PileText>b__1_3(string path, Func<string, string> subfldrf, int n)
            {
                string extension = Path.GetExtension(path);
                return Path.Combine(Path.Combine(Path.GetDirectoryName(path), subfldrf.Invoke(path)), Path.GetFileNameWithoutExtension(path) + n.ToString() + extension);
            }

            internal Func<string, string> <PileText>b__1_4(DirectoryInfo nfo, Func<string, string> f)
            {
                FSysFw.<>c__DisplayClass1_1 class_1;
                return new Func<string, string>(class_1, this.<PileText>b__5);
            }

            internal string <PileText>b__1_6(string path)
            {
                return Path.GetFileNameWithoutExtension(path);
            }

            internal Func<string, int, string> <PileText>b__1_7(Func<int, string> nfof)
            {
                FSysFw.<>c__DisplayClass1_2 class_1;
                return new Func<string, int, string>(class_1, this.<PileText>b__8);
            }

            internal string <PileText>b__1_9(int n)
            {
                return ("(" + n.ToString() + ")");
            }

            internal string <PileTextRun>b__0_1(string ss)
            {
                return (ss.Substring(ss.LastIndexOf(@"\") + 1) + "Pile.cs");
            }
        }
    }
}


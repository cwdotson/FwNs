namespace FwNs.Tests
{
    using FwNs.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class OutputHlpr
    {
        public Action<string, object, Func<object, string>> AddRsltAktn;
        public FileSystemInfo DfltOutPutTarg;
        public Dictionary<string, object> DyctRslts;
        public Dlgt<object, string> Obj2StrDlgt;
        private Dlgt<OutputHlpr, FileInfo> OutFileNameFunc;
        public string RsltStrng;
        public Action<string, object, Func<object, string>> XtraAddRsltAktn;

        public OutputHlpr()
        {
            this.OutFileNameFunc = delegate (OutputHlpr toh) {
                if (toh.DfltOutPutTarg is DirectoryInfo)
                {
                    DateTime now = DateTime.Now;
                    DirectoryInfo info2 = (DirectoryInfo) toh.DfltOutPutTarg;
                    if (!Directory.Exists(info2.FullName))
                    {
                        info2.Create();
                    }
                    now.ToString("o").Substring(0, 9).Replace("-", "");
                    now.ToString("HH:mm:ss.ffffzzz");
                    now.ToString("HH:mm:ss:ffff").Replace(":", "");
                    throw new Exception();
                }
                FileInfo dfltOutPutTarg = (FileInfo) toh.DfltOutPutTarg;
                if (File.Exists(dfltOutPutTarg.FullName))
                {
                    Path.GetFileNameWithoutExtension(dfltOutPutTarg.FullName);
                    string extension = dfltOutPutTarg.Extension;
                    throw new Exception();
                }
                return dfltOutPutTarg;
            };
            throw new Exception();
        }

        public OutputHlpr(FileSystemInfo targNfo) : this()
        {
            this.DfltOutPutTarg = targNfo;
        }

        public OutputHlpr(FileSystemInfo targNfo, Action<OutputHlpr> addRsltAktn) : this()
        {
            if (null == null)
            {
                throw new Exception();
            }
        }

        public OutputHlpr(FileSystemInfo targNfo, Action<string, object, Func<object, string>> addRsltAktn) : this(targNfo)
        {
            this.XtraAddRsltAktn = addRsltAktn;
        }

        public void AddRslt(string nm, object obj)
        {
            throw new Exception();
        }

        public void Print()
        {
            Console.WriteLine(this.RsltStrng);
        }

        public void SaveRslts(bool ovrwrt)
        {
            this.SaveRslts(this.OutFileNameFunc(this), ovrwrt);
        }

        public void SaveRslts(FileInfo fileInfo, bool ovrwrt)
        {
            if (fileInfo.Exists | ovrwrt)
            {
                File.WriteAllText(fileInfo.FullName, this.RsltStrng);
            }
            else
            {
                File.AppendAllText(fileInfo.FullName, this.RsltStrng);
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly OutputHlpr.<>c <>9 = new OutputHlpr.<>c();
            public static Dlgt<OutputHlpr, FileInfo> <>9__7_0;
            public static Dlgt<object, string> <>9__7_1;

            internal FileInfo <.ctor>b__7_0(OutputHlpr toh)
            {
                if (toh.DfltOutPutTarg is DirectoryInfo)
                {
                    DateTime now = DateTime.Now;
                    DirectoryInfo info2 = (DirectoryInfo) toh.DfltOutPutTarg;
                    if (!Directory.Exists(info2.FullName))
                    {
                        info2.Create();
                    }
                    now.ToString("o").Substring(0, 9).Replace("-", "");
                    now.ToString("HH:mm:ss.ffffzzz");
                    now.ToString("HH:mm:ss:ffff").Replace(":", "");
                    throw new Exception();
                }
                FileInfo dfltOutPutTarg = (FileInfo) toh.DfltOutPutTarg;
                if (File.Exists(dfltOutPutTarg.FullName))
                {
                    Path.GetFileNameWithoutExtension(dfltOutPutTarg.FullName);
                    string extension = dfltOutPutTarg.Extension;
                    throw new Exception();
                }
                return dfltOutPutTarg;
            }

            internal string <.ctor>b__7_1(object oo)
            {
                throw new Exception();
            }
        }
    }
}


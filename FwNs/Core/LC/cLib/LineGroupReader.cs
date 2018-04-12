namespace FwNs.Core.LC.cLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class LineGroupReader
    {
        private const string Ls = "\n";
        private readonly string[] _ignoredStarts;
        private readonly StreamReader _reader;
        private readonly string[] _sectionContinuations;
        private readonly string[] _sectionStarts;
        private string _nextStartLine;
        private int _nextStartLineNumber;
        private int _startLineNumber;

        public LineGroupReader(StreamReader reader, string[] sectionStarts)
        {
            this._sectionStarts = sectionStarts;
            this._sectionContinuations = new string[0];
            this._ignoredStarts = new string[0];
            this._reader = reader;
            try
            {
                this.GetSection();
            }
            catch (Exception)
            {
            }
        }

        public void Close()
        {
            try
            {
                this._reader.Close();
            }
            catch (Exception)
            {
            }
        }

        public static string ConvertToString(List<string> list, int offset)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = offset; i < list.Count; i++)
            {
                builder.Append(list[i]).Append("\n");
            }
            return builder.ToString();
        }

        public HashMappedList<string, string> GetAsMap()
        {
            HashMappedList<string, string> list = new HashMappedList<string, string>();
            while (true)
            {
                List<string> section = this.GetSection();
                if (section.Count < 1)
                {
                    return list;
                }
                string key = section[0];
                string str2 = ConvertToString(section, 1);
                list.Put(key, str2);
            }
        }

        public List<string> GetSection()
        {
            bool flag;
            List<string> list = new List<string>();
            if (this._nextStartLine != null)
            {
                list.Add(this._nextStartLine);
                this._startLineNumber = this._nextStartLineNumber;
            }
            int num = 1;
        Label_0028:
            flag = false;
            string s = null;
            try
            {
                s = this._reader.ReadLine();
            }
            catch (Exception)
            {
            }
            num++;
            if (s != null)
            {
                s = s.Substring(0, StringUtil.RightTrimSize(s));
                if ((s.Length != 0) && !this.IsIgnoredLine(s))
                {
                    if (this.IsNewSectionLine(s))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        this._nextStartLine = s;
                        this._nextStartLineNumber = num;
                        return list;
                    }
                    list.Add(s);
                }
                goto Label_0028;
            }
            this._nextStartLine = null;
            return list;
        }

        public int GetStartLineNumber()
        {
            return this._startLineNumber;
        }

        private bool IsIgnoredLine(string line)
        {
            for (int i = 0; i < this._ignoredStarts.Length; i++)
            {
                if (line.StartsWith(this._ignoredStarts[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsNewSectionLine(string line)
        {
            if (this._sectionStarts.Length == 0)
            {
                for (int j = 0; j < this._sectionContinuations.Length; j++)
                {
                    if (line.StartsWith(this._sectionContinuations[j]))
                    {
                        return false;
                    }
                }
                return true;
            }
            for (int i = 0; i < this._sectionStarts.Length; i++)
            {
                if (line.StartsWith(this._sectionStarts[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}


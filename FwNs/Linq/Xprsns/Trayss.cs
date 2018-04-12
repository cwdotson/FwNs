namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    internal class Trayss
    {
        private static int _indent;
        private const int IndentSize = 4;
        private const int methodNameLength = 70;
        private static TextWriter _bufferWriter;
        private static StringBuilder _buffer;

        public static void FlushBuffer()
        {
            Trace.Flush();
            if (_buffer != null)
            {
                WriteToOutputOmittingBuffering(_buffer.ToString());
            }
        }

        private static void WriteToOutputOmittingBuffering(string msg)
        {
            Trace.WriteLine(msg);
        }
    }
}


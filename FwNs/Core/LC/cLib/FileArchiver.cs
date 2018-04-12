namespace FwNs.Core.LC.cLib
{
    using System;
    using System.IO;
    using System.IO.Compression;

    public class FileArchiver
    {
        public const int CompressionNone = 0;
        public const int CompressionZip = 1;
        public const int CompressionGzip = 2;
        private const int CopyBlockSize = 0x10000;

        public static void Archive(string infilename, string outfilename, UtlFileAccess storage, int compressionType)
        {
            GZipStream stream = null;
            bool flag = false;
            if (storage.IsStreamElement(infilename))
            {
                try
                {
                    byte[] buffer = new byte[0x10000];
                    using (Stream stream2 = storage.OpenInputStreamElement(infilename))
                    {
                        using (Stream stream3 = storage.OpenOutputStreamElement(outfilename))
                        {
                            int num;
                            Stream stream4 = null;
                            if (compressionType != 0)
                            {
                                if ((compressionType - 1) > 1)
                                {
                                    throw new Exception("FileArchiver" + compressionType);
                                }
                            }
                            else
                            {
                                stream4 = stream3;
                                goto Label_0061;
                            }
                            stream = new GZipStream(stream3, CompressionMode.Compress);
                        Label_0061:
                            num = stream2.Read(buffer, 0, buffer.Length);
                            if (num != 0)
                            {
                                stream4.Write(buffer, 0, num);
                                goto Label_0061;
                            }
                            if (stream != null)
                            {
                                stream.Flush();
                            }
                        }
                    }
                    flag = true;
                }
                catch (Exception exception)
                {
                    try
                    {
                        if (!flag && storage.IsStreamElement(outfilename))
                        {
                            storage.RemoveElement(outfilename);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    throw new IOException(exception.Message, exception);
                }
            }
        }

        public static void Unarchive(string infilename, string outfilename, UtlFileAccess storage, int compressionType)
        {
            bool flag = false;
            try
            {
                if (storage.IsStreamElement(infilename))
                {
                    storage.RemoveElement(outfilename);
                    byte[] buffer = new byte[0x10000];
                    using (Stream stream = storage.OpenInputStreamElement(infilename))
                    {
                        Stream stream2;
                        if (compressionType != 0)
                        {
                            if ((compressionType - 1) > 1)
                            {
                                throw new Exception("FileArchiver: " + compressionType);
                            }
                        }
                        else
                        {
                            stream2 = stream;
                            goto Label_0057;
                        }
                        stream2 = new GZipStream(stream, CompressionMode.Decompress);
                    Label_0057:
                        using (Stream stream3 = storage.OpenOutputStreamElement(outfilename))
                        {
                            int num;
                        Label_0060:
                            num = stream2.Read(buffer, 0, buffer.Length);
                            if (num != 0)
                            {
                                stream3.Write(buffer, 0, num);
                                goto Label_0060;
                            }
                            stream3.Flush();
                        }
                        flag = true;
                    }
                }
            }
            catch (Exception exception)
            {
                try
                {
                    if (!flag && storage.IsStreamElement(outfilename))
                    {
                        storage.RemoveElement(outfilename);
                    }
                }
                catch (Exception)
                {
                }
                throw new IOException(exception.Message, exception);
            }
        }
    }
}


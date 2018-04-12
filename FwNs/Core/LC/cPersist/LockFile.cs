namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    public sealed class LockFile : IDisposable
    {
        public const long HeartbeatInterval = 0x2710L;
        public const long HeartbeatIntervalPadded = 0x2774L;
        public const int UsedRegion = 0x10;
        public const int PollRetriesDefault = 10;
        public const string PollRetriesProperty = "LibCore.lockfile.poll.retries";
        public const string PollIntervalProperty = "LibCore.lockfile.poll.interval";
        public static byte[] Magic = new byte[] { 0x48, 0x53, 0x51, 0x4c, 0x4c, 0x4f, 0x43, 0x4b };
        public static Timer timer;
        private string _cpath;
        private FileStream Raf;
        private bool Locked;
        private Timer _timerTask;
        private readonly UtlFileAccess _fileAccess;

        private LockFile(UtlFileAccess fileAccess)
        {
            this._fileAccess = fileAccess;
        }

        private void CheckHeartbeat(bool withCreateNewFile)
        {
            long length;
            try
            {
                if (withCreateNewFile)
                {
                    try
                    {
                        this.Raf = (FileStream) this._fileAccess.OpenOutputStreamElement(this._cpath);
                        return;
                    }
                    catch (IOException)
                    {
                    }
                }
                if (!this._fileAccess.IsStreamElement(this._cpath))
                {
                    return;
                }
                using (Stream stream = this._fileAccess.OpenInputStreamElement(this._cpath))
                {
                    length = stream.Length;
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (length != 0x10L)
            {
                if (length != 0)
                {
                    throw new WrongLengthException(this, "checkHeartbeat", length);
                }
                File.Delete(this._cpath);
                this.Raf = null;
            }
            else
            {
                long read = DateTime.Now.Ticks / 0x2710L;
                long heartbeat = this.ReadHeartbeat();
                if (Math.Abs((long) (read - heartbeat)) <= 0x2774L)
                {
                    throw new LockHeldExternallyException(this, "checkHeartbeat", read, heartbeat);
                }
            }
        }

        private void CheckMagic(BinaryReader dis)
        {
            bool flag = true;
            byte[] magic = new byte[Magic.Length];
            try
            {
                for (int i = 0; i < Magic.Length; i++)
                {
                    magic[i] = dis.ReadByte();
                    if (Magic[i] != magic[i])
                    {
                        flag = false;
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                throw new UnexpectedEndOfFileException(this, "checkMagic", exception);
            }
            catch (IOException exception2)
            {
                throw new UnexpectedFileIOException(this, "checkMagic", exception2);
            }
            if (!flag)
            {
                throw new WrongMagicException(this, "checkMagic", magic);
            }
        }

        private void CloseRaf()
        {
            if (this.Raf != null)
            {
                try
                {
                    this.Raf.Close();
                    this.Raf.Dispose();
                }
                catch (IOException exception)
                {
                    throw new UnexpectedFileIOException(this, "closeRAF", exception);
                }
                finally
                {
                    this.Raf = null;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.TryRelease();
            }
        }

        private static bool DoOptionalLockActions()
        {
            return false;
        }

        private static bool DoOptionalReleaseActions()
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            LockFile file = obj as LockFile;
            if (file == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(this._cpath))
            {
                return this._cpath.Equals(file._cpath);
            }
            return string.IsNullOrEmpty(file._cpath);
        }

        ~LockFile()
        {
            this.Dispose(false);
        }

        public string GetCanonicalPath()
        {
            return this._cpath;
        }

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(this._cpath))
            {
                return this._cpath.GetHashCode();
            }
            return 0;
        }

        public static long GetPollHeartbeatInterval()
        {
            int pollHeartbeatRetries = GetPollHeartbeatRetries();
            long num2 = 10L + (0x2774L / ((long) pollHeartbeatRetries));
            if (num2 <= 0L)
            {
                num2 = 10L + (0x2774L / ((long) pollHeartbeatRetries));
            }
            return num2;
        }

        public static int GetPollHeartbeatRetries()
        {
            int num = 10;
            if (num < 1)
            {
                num = 1;
            }
            return num;
        }

        public bool IsLocked()
        {
            return this.Locked;
        }

        public bool IsValid()
        {
            return (((this.IsLocked() && !string.IsNullOrEmpty(this._cpath)) && this._fileAccess.IsStreamElement(this._cpath)) && (this.Raf > null));
        }

        public static LockFile NewLockFile(string path, UtlFileAccess fileAccess)
        {
            LockFile file1 = new LockFile(fileAccess);
            file1.SetPath(path);
            return file1;
        }

        public static LockFile NewLockFileLock(string path, UtlFileAccess fileAccess)
        {
            LockFile file;
            bool flag;
            try
            {
                file = NewLockFile(path + ".lck", fileAccess);
            }
            catch (BaseException exception)
            {
                throw Error.GetError(0x1c3, exception.Message);
            }
            try
            {
                flag = file.TryLock();
            }
            catch (BaseException exception2)
            {
                throw Error.GetError(0x1c3, exception2.Message);
            }
            if (!flag)
            {
                throw Error.GetError(0x1c3, file.ToString());
            }
            return file;
        }

        private void OpenRaf()
        {
            try
            {
                if (this.Raf == null)
                {
                    this.Raf = (FileStream) this._fileAccess.OpenOutputStreamElement(this._cpath);
                }
            }
            catch (FileNotFoundException exception)
            {
                throw new UnexpectedFileNotFoundException(this, "openRAF", exception);
            }
            catch (IOException exception2)
            {
                throw new UnexpectedFileNotFoundException(this, "openRAF", exception2);
            }
        }

        private void PollHeartbeat()
        {
            bool flag = false;
            long pollHeartbeatInterval = GetPollHeartbeatInterval();
            BaseException exception = null;
            for (int i = GetPollHeartbeatRetries(); i > 0; i--)
            {
                try
                {
                    this.CheckHeartbeat(true);
                    flag = true;
                    break;
                }
                catch (BaseException exception1)
                {
                    exception = exception1;
                }
                try
                {
                    Thread.Sleep((int) pollHeartbeatInterval);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
            }
            if (!flag)
            {
                if (exception is LockHeldExternallyException)
                {
                    throw exception;
                }
                if (exception is UnexpectedFileNotFoundException)
                {
                    throw exception;
                }
                if (exception is UnexpectedEndOfFileException)
                {
                    throw exception;
                }
                if (exception is UnexpectedFileIOException)
                {
                    throw exception;
                }
                if (exception is WrongLengthException)
                {
                    throw exception;
                }
                if (exception is WrongMagicException)
                {
                    throw exception;
                }
            }
        }

        private long ReadHeartbeat()
        {
            FileStream input = null;
            long num;
            try
            {
                if (!this._fileAccess.IsStreamElement(this._cpath))
                {
                    return -9223372036854775808L;
                }
                input = (FileStream) this._fileAccess.OpenInputStreamElement(this._cpath);
                BinaryReader dis = new BinaryReader(input);
                this.CheckMagic(dis);
                num = dis.ReadInt64();
            }
            catch (FileNotFoundException exception)
            {
                throw new UnexpectedFileNotFoundException(this, "readHeartbeat", exception);
            }
            catch (InvalidOperationException exception2)
            {
                throw new UnexpectedEndOfFileException(this, "readHeartbeat", exception2);
            }
            catch (IOException exception3)
            {
                throw new UnexpectedFileIOException(this, "readHeartbeat", exception3);
            }
            finally
            {
                if (input != null)
                {
                    try
                    {
                        input.Dispose();
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            return num;
        }

        public void Runnable(object ignore)
        {
            try
            {
                this.WriteHeartbeat();
            }
            catch (Exception exception1)
            {
                Error.PrintSystemOut(exception1.ToString());
            }
        }

        private void SetPath(string path)
        {
            path = this._fileAccess.CanonicalOrAbsolutePath(path);
            try
            {
                this._fileAccess.CreateParentDirs(path);
            }
            catch (Exception exception)
            {
                throw new Exception("setPath", exception);
            }
            this._cpath = this._fileAccess.CanonicalOrAbsolutePath(path);
        }

        private void StartHeartbeat()
        {
            if (this._timerTask == null)
            {
                TimerCallback callback = new TimerCallback(this.Runnable);
                this._timerTask = new Timer(callback, null, 0L, 0x2710L);
            }
        }

        private void StopHeartbeat()
        {
            if (this._timerTask != null)
            {
                this._timerTask.Dispose();
                this._timerTask = null;
            }
        }

        public override string ToString()
        {
            bool flag = this._fileAccess.IsStreamElement(this._cpath);
            return new StringBuilder(base.ToString()).Append("[file =").Append(this._cpath).Append(", exists=").Append(flag).Append(", locked=").Append(this.IsLocked()).Append(", valid=").Append(this.IsValid()).Append(", ").Append(ToStringImpl()).Append("]").ToString();
        }

        private static string ToStringImpl()
        {
            return "";
        }

        public bool TryLock()
        {
            if (this.Locked)
            {
                return true;
            }
            try
            {
                this.PollHeartbeat();
                this.OpenRaf();
                DoOptionalLockActions();
                this.WriteMagic();
                this.WriteHeartbeat();
                this._fileAccess.DeleteOnExit(this._cpath);
                this.Locked = true;
                this.StartHeartbeat();
            }
            finally
            {
                if (!this.Locked)
                {
                    DoOptionalReleaseActions();
                    try
                    {
                        this.CloseRaf();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return this.Locked;
        }

        public bool TryRelease()
        {
            bool flag = !this.Locked;
            if (flag)
            {
                return true;
            }
            this.StopHeartbeat();
            DoOptionalReleaseActions();
            UnexpectedFileIOException exception = null;
            FileSecurityException exception2 = null;
            try
            {
                try
                {
                    this.CloseRaf();
                }
                catch (UnexpectedFileIOException exception1)
                {
                    exception = exception1;
                }
                try
                {
                    Thread.Sleep(100);
                }
                catch (Exception)
                {
                }
                try
                {
                    this._fileAccess.RemoveElement(this._cpath);
                    flag = true;
                }
                catch (Exception exception3)
                {
                    exception2 = new FileSecurityException(this, "tryRelease", exception3);
                }
            }
            finally
            {
                this.Locked = false;
            }
            if (exception != null)
            {
                throw exception;
            }
            if (exception2 != null)
            {
                throw exception2;
            }
            return flag;
        }

        private void WriteHeartbeat()
        {
            try
            {
                this.Raf.Seek((long) Magic.Length, SeekOrigin.Begin);
                byte[] bytes = BitConverter.GetBytes((long) (DateTime.Now.Ticks / 0x2710L));
                this.Raf.Write(bytes, 0, bytes.Length);
            }
            catch (InvalidOperationException exception)
            {
                throw new UnexpectedEndOfFileException(this, "writeHeartbeat", exception);
            }
            catch (IOException exception2)
            {
                throw new UnexpectedFileIOException(this, "writeHeartbeat", exception2);
            }
        }

        private void WriteMagic()
        {
            try
            {
                this.Raf.Seek(0L, SeekOrigin.Begin);
                this.Raf.Write(Magic, 0, Magic.Length);
            }
            catch (InvalidOperationException exception)
            {
                throw new UnexpectedEndOfFileException(this, "writeMagic", exception);
            }
            catch (IOException exception2)
            {
                throw new UnexpectedFileIOException(this, "writeMagic", exception2);
            }
        }

        [Serializable]
        private abstract class BaseException : Exception
        {
            private readonly LockFile _lockFile;
            private readonly string _inMethod;

            protected BaseException(LockFile lockFile, string inMethod)
            {
                if (lockFile == null)
                {
                    throw new NullReferenceException("lockFile");
                }
                if (inMethod == null)
                {
                    throw new NullReferenceException("inMethod");
                }
                this._lockFile = lockFile;
                this._inMethod = inMethod;
            }

            protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._lockFile = (LockFile) info.GetValue("_lockFile", typeof(LockFile));
                this._inMethod = (string) info.GetValue("_inMethod", typeof(string));
            }

            public virtual string GetMessage()
            {
                object[] objArray1 = new object[] { "lockFile: ", this._lockFile, " method: ", this._inMethod };
                return string.Concat(objArray1);
            }
        }

        [Serializable]
        private class FileSecurityException : LockFile.BaseException
        {
            private readonly Exception _reason;

            protected FileSecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._reason = (Exception) info.GetValue("_reason", typeof(Exception));
            }

            public FileSecurityException(LockFile lockFile, string inMethod, Exception reason) : base(lockFile, inMethod)
            {
                this._reason = reason;
            }

            public override string GetMessage()
            {
                return (base.Message + " reason: " + this._reason);
            }
        }

        [Serializable]
        private class LockHeldExternallyException : LockFile.BaseException
        {
            private readonly long _read;
            private readonly long _heartbeat;

            protected LockHeldExternallyException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._read = (long) info.GetValue("_read", typeof(long));
                this._heartbeat = (long) info.GetValue("_heartbeat", typeof(long));
            }

            public LockHeldExternallyException(LockFile lockFile, string inMethod, long read, long heartbeat) : base(lockFile, inMethod)
            {
                this._read = read;
                this._heartbeat = heartbeat;
            }

            public override string GetMessage()
            {
                object[] objArray1 = new object[] { base.Message, " read: ", new DateTime(this._read), " heartbeat - read: ", this._heartbeat - this._read, " ms." };
                return string.Concat(objArray1);
            }
        }

        [Serializable]
        private class UnexpectedEndOfFileException : LockFile.BaseException
        {
            private readonly InvalidOperationException _reason;

            protected UnexpectedEndOfFileException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._reason = (InvalidOperationException) info.GetValue("_reason", typeof(InvalidOperationException));
            }

            public UnexpectedEndOfFileException(LockFile lockFile, string inMethod, InvalidOperationException reason) : base(lockFile, inMethod)
            {
                this._reason = reason;
            }

            public override string GetMessage()
            {
                return (base.Message + " reason: " + this._reason);
            }
        }

        [Serializable]
        private class UnexpectedFileIOException : LockFile.BaseException
        {
            private readonly IOException _reason;

            protected UnexpectedFileIOException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._reason = (IOException) info.GetValue("_reason", typeof(IOException));
            }

            public UnexpectedFileIOException(LockFile lockFile, string inMethod, IOException reason) : base(lockFile, inMethod)
            {
                this._reason = reason;
            }

            public override string GetMessage()
            {
                return (base.Message + " reason: " + this._reason);
            }
        }

        [Serializable]
        private class UnexpectedFileNotFoundException : LockFile.BaseException
        {
            private readonly Exception _reason;

            protected UnexpectedFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._reason = (Exception) info.GetValue("_reason", typeof(Exception));
            }

            public UnexpectedFileNotFoundException(LockFile lockFile, string inMethod, Exception reason) : base(lockFile, inMethod)
            {
                this._reason = reason;
            }

            public override string GetMessage()
            {
                return (base.Message + " reason: " + this._reason);
            }
        }

        [Serializable]
        private class WrongLengthException : LockFile.BaseException
        {
            private readonly long _length;

            protected WrongLengthException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._length = (long) info.GetValue("_length", typeof(long));
            }

            public WrongLengthException(LockFile lockFile, string inMethod, long length) : base(lockFile, inMethod)
            {
                this._length = length;
            }

            public override string GetMessage()
            {
                return (base.Message + " length: " + this._length);
            }
        }

        [Serializable]
        private class WrongMagicException : LockFile.BaseException
        {
            private readonly byte[] _magic;

            protected WrongMagicException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                this._magic = (byte[]) info.GetValue("_level", typeof(byte[]));
            }

            public WrongMagicException(LockFile lockFile, string inMethod, byte[] magic) : base(lockFile, inMethod)
            {
                this._magic = magic;
            }

            public override string GetMessage()
            {
                return (base.Message + " magic: " + ((this._magic == null) ? "null" : ("'" + StringConverter.ByteToHex(this._magic) + "'")));
            }
        }
    }
}


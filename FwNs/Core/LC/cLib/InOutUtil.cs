namespace FwNs.Core.LC.cLib
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class InOutUtil
    {
        public static object Deserialize(byte[] ba)
        {
            using (MemoryStream stream = new MemoryStream(ba))
            {
                return new BinaryFormatter().Deserialize(stream);
            }
        }

        public static byte[] Serialize(object s)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, s);
                return stream.ToArray();
            }
        }
    }
}


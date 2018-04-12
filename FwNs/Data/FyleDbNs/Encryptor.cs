namespace FwNs.Data.FyleDbNs
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    internal class Encryptor
    {
        private byte[] _key;
        private AesManaged _encryptor;

        internal Encryptor(string hashKey, string productKey)
        {
            this._key = GetHashKey(hashKey, productKey);
            this._encryptor = new AesManaged();
            this._encryptor.Key = this._key;
            this._encryptor.IV = this._key;
        }

        internal byte[] Decrypt(byte[] encryptedData)
        {
            byte[] buffer = null;
            MemoryStream stream = new MemoryStream((int) (encryptedData.Length * 1.5));
            using (CryptoStream stream2 = new CryptoStream(stream, this._encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                stream2.Write(encryptedData, 0, encryptedData.Length);
                stream2.FlushFinalBlock();
                buffer = stream.ToArray();
                stream2.Close();
            }
            return buffer;
        }

        internal byte[] Encrypt(byte[] dataToEncrypt)
        {
            byte[] buffer = null;
            MemoryStream stream = new MemoryStream((int) (dataToEncrypt.Length * 1.5));
            using (CryptoStream stream2 = new CryptoStream(stream, this._encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                stream2.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                stream2.FlushFinalBlock();
                buffer = stream.ToArray();
                stream2.Close();
            }
            return buffer;
        }

        internal static byte[] GetHashKey(string hashKey, string salt)
        {
            byte[] bytes = new UTF8Encoding().GetBytes(salt);
            return new Rfc2898DeriveBytes(hashKey, bytes).GetBytes(0x10);
        }
    }
}


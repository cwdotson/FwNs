namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cErrors;
    using System;
    using System.IO;
    using System.Security.Cryptography;

    public sealed class Crypto
    {
        private readonly byte[] _encodedIv;
        private readonly byte[] _encodedKey;
        private readonly SymmetricAlgorithm _inCipher;
        private readonly SymmetricAlgorithm _outCipher;

        public Crypto(string keyString, string iVString, string cipherName)
        {
            this._encodedKey = Convert.FromBase64String(keyString);
            this._encodedIv = Convert.FromBase64String(iVString);
            this._inCipher = GetAlgorithm(cipherName);
            this._outCipher = GetAlgorithm(cipherName);
            try
            {
                this._inCipher.Key = this._encodedKey;
                this._outCipher.Key = this._encodedKey;
                this._inCipher.IV = this._encodedIv;
                this._outCipher.IV = this._encodedIv;
            }
            catch (CryptographicException exception)
            {
                throw Error.GetError(0x14b, exception);
            }
        }

        public byte[] Decode(byte[] source, int sourceOffset, int length)
        {
            byte[] buffer;
            lock (this)
            {
                if (this._inCipher == null)
                {
                    buffer = null;
                }
                else
                {
                    try
                    {
                        buffer = this._inCipher.CreateDecryptor(this._encodedKey, this._encodedIv).TransformFinalBlock(source, sourceOffset, length);
                    }
                    catch (Exception exception)
                    {
                        throw Error.GetError(0x14b, exception);
                    }
                }
            }
            return buffer;
        }

        public byte[] Encode(byte[] source, int sourceOffset, int length)
        {
            byte[] buffer;
            lock (this)
            {
                if (this._inCipher == null)
                {
                    buffer = null;
                }
                else
                {
                    try
                    {
                        buffer = this._inCipher.CreateEncryptor(this._encodedKey, this._encodedIv).TransformFinalBlock(source, sourceOffset, length);
                    }
                    catch (Exception exception)
                    {
                        throw Error.GetError(0x14b, exception);
                    }
                }
            }
            return buffer;
        }

        private static SymmetricAlgorithm GetAlgorithm(string cipherName)
        {
            if (cipherName.Equals("DES", StringComparison.OrdinalIgnoreCase))
            {
                return new DESCryptoServiceProvider();
            }
            if (cipherName.Equals("TripleDES", StringComparison.OrdinalIgnoreCase))
            {
                return new TripleDESCryptoServiceProvider();
            }
            if (cipherName.Equals("Aes", StringComparison.OrdinalIgnoreCase))
            {
                return new AesCryptoServiceProvider();
            }
            if (!cipherName.Equals("Rijndael", StringComparison.OrdinalIgnoreCase))
            {
                throw Error.GetError(0x14b);
            }
            return new RijndaelManaged();
        }

        public int GetEncodedSize(int size)
        {
            return (((size / this._outCipher.BlockSize) + 1) * this._outCipher.BlockSize);
        }

        public Stream GetInputStream(Stream input)
        {
            Stream stream;
            lock (this)
            {
                if (this._inCipher == null)
                {
                    stream = input;
                }
                else
                {
                    try
                    {
                        stream = new CryptoStream(input, this._inCipher.CreateDecryptor(this._encodedKey, this._encodedIv), CryptoStreamMode.Read);
                    }
                    catch (Exception exception)
                    {
                        throw Error.GetError(0x14b, exception);
                    }
                }
            }
            return stream;
        }

        public static string GetNewStrIv(string cipherName)
        {
            SymmetricAlgorithm algorithm = GetAlgorithm(cipherName);
            algorithm.GenerateIV();
            return Convert.ToBase64String(algorithm.IV);
        }

        public static string GetNewStrKey(string cipherName)
        {
            SymmetricAlgorithm algorithm = GetAlgorithm(cipherName);
            algorithm.GenerateKey();
            return Convert.ToBase64String(algorithm.Key);
        }

        public Stream GetOutputStream(Stream output)
        {
            Stream stream;
            lock (this)
            {
                if (this._outCipher == null)
                {
                    stream = output;
                }
                else
                {
                    try
                    {
                        stream = new CryptoStream(output, this._outCipher.CreateEncryptor(this._encodedKey, this._encodedIv), CryptoStreamMode.Write);
                    }
                    catch (CryptographicException exception)
                    {
                        throw Error.GetError(0x14b, exception);
                    }
                }
            }
            return stream;
        }
    }
}


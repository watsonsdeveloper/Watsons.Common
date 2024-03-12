using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common
{
    public class EncryptionSql
    {
        private const int Keysize = 256;
        private const string secretKey = "KEY";

        public static string Decrypt(string FromSql)
        {
            string Password = secretKey;
            // Encode password as UTF16-LE
            byte[] passwordBytes = Encoding.Unicode.GetBytes(Password);

            // Remove leading "0x"
            FromSql = FromSql.Substring(2);

            int version = BitConverter.ToInt32(StringToByteArray(FromSql.Substring(0, 8)), 0);
            byte[] encrypted = null;

            HashAlgorithm hashAlgo = null;
            SymmetricAlgorithm cryptoAlgo = null;
            int keySize = (version == 1 ? 16 : 32);

            if (version == 1)
            {
                hashAlgo = SHA1.Create();
                cryptoAlgo = TripleDES.Create();
                cryptoAlgo.IV = StringToByteArray(FromSql.Substring(8, 16));
                encrypted = StringToByteArray(FromSql.Substring(24));
            }
            else if (version == 2)
            {
                hashAlgo = SHA256.Create();
                cryptoAlgo = Aes.Create();
                cryptoAlgo.IV = StringToByteArray(FromSql.Substring(8, 32));
                encrypted = StringToByteArray(FromSql.Substring(40));
            }
            else
            {
                throw new Exception("Unsupported encryption");
            }

            cryptoAlgo.Padding = PaddingMode.PKCS7;
            cryptoAlgo.Mode = CipherMode.CBC;

            hashAlgo.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
            cryptoAlgo.Key = hashAlgo.Hash.Take(keySize).ToArray();

            byte[] decrypted = cryptoAlgo.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
            int decryptLength = BitConverter.ToInt16(decrypted, 6);
            UInt32 magic = BitConverter.ToUInt32(decrypted, 0);
            if (magic != 0xbaadf00d)
            {
                throw new Exception("Decrypt failed");
            }

            byte[] decryptedData = decrypted.Skip(8).ToArray();
            bool isUtf16 = (Array.IndexOf(decryptedData, (byte)0) != -1);
            string decryptText = (isUtf16 ? Encoding.Unicode.GetString(decryptedData) : Encoding.UTF8.GetString(decryptedData));


            return decryptText;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
}

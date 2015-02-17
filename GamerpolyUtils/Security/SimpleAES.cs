using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Security.Cryptography;
using System.IO;

namespace GamerpolyUtils.Security {
    static public class SimpleAES {
        // Change these keys
        static private byte[] Key = { 195, 2, 52, 195, 249, 238, 95, 115, 108, 72, 201, 234, 213, 235, 23, 158, 45, 129, 146, 115, 226, 21, 87, 28, 69, 53, 188, 3, 218, 91, 49, 19 };
        static private byte[] Vector = { 187, 37, 191, 133, 126, 147, 111, 32, 39, 134, 188, 113, 42, 46, 150, 121 };

        static private RijndaelManaged rm = new RijndaelManaged() { Padding = PaddingMode.Zeros };
        static private ICryptoTransform EncryptorTransform = rm.CreateEncryptor(Key, Vector);
        static private ICryptoTransform DecryptorTransform = rm.CreateDecryptor(Key, Vector);

        /// Encrypt some text and return an encrypted byte array.
        static public byte[] Encrypt(byte[] bytes) {

            //Used to stream the data in and out of the CryptoStream.
            MemoryStream memoryStream = new MemoryStream();

            /*
             * We will have to write the unencrypted bytes to the stream,
             * then read the encrypted result back from the stream.
             */
            #region Write the decrypted value to the encryption stream
            CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            #endregion

            #region Read encrypted value back out of the stream
            memoryStream.Position = 0;
            byte[] encrypted = new byte[memoryStream.Length];
            memoryStream.Read(encrypted, 0, encrypted.Length);
            #endregion

            //Clean up.
            cs.Close();
            memoryStream.Close();

            return encrypted;
        }

        /// Decryption when working with byte arrays.    
        static public byte[] Decrypt(byte[] EncryptedValue) {
            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return decryptedBytes;
        }

    }
}
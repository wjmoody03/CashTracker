
using ct.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ct.Business
{

    public static class Encryptor
    {
        static string _encryptionKey = CashTrackerConfigurationManager.EncryptionKey;
        static byte[] _iv = CashTrackerConfigurationManager.InitializationVector.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(b => (byte)int.Parse(b)).ToArray();


        public static string Encrypt(string text)
        {
            byte[] bykey = System.Text.Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] InputByteArray = System.Text.Encoding.UTF8.GetBytes(text);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(bykey, _iv), CryptoStreamMode.Write);
            cs.Write(InputByteArray, 0, InputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string text)
        {
            string result;
            byte[] inputByteArray = new byte[text.Length + 1];
            byte[] byKey = System.Text.Encoding.UTF8.GetBytes(_encryptionKey);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(text);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, _iv), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            result = encoding.GetString(ms.ToArray());
            return result;

        }
    }
}

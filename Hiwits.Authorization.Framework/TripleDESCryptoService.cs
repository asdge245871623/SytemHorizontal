using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hiwits.Authorization.Framework
{
    public class TripleDESCryptoService
    {
        #region +   TripleDES配置参数

        private static byte[] _key { get; set; }
        public static byte[] key
        {
            get
            {
                if (_key == null)
                    _key = Encoding.UTF8.GetBytes("2VYvlxfYYGUgzQHvSfuQsJEa");

                return _key;
            }
        }

        private static byte[] _iv { get; set; }
        public static byte[] iv
        {
            get
            {
                if (_iv == null)
                    _iv = Encoding.UTF8.GetBytes("rJncy0ix");

                return _iv;
            }
        }

        #endregion

        #region +   DES 加密

        public static string encryptToBase64(string plainText)
        {
            try
            {
                byte[] plainTextArray = Encoding.UTF8.GetBytes(plainText);
                using (var mstream = new MemoryStream())
                {
                    using (var cstream = new CryptoStream(mstream,
                                                new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv),
                                                CryptoStreamMode.Write))
                    {
                        cstream.Write(plainTextArray, 0, plainTextArray.Length);

                        cstream.FlushFinalBlock();

                        return Convert.ToBase64String(mstream.ToArray());
                    }
                }

            }
            catch (CryptographicException ex)
            {
                throw ex;
            }

        }

        #endregion

        #region +   DES 解密

        public static string decryptFromBase64(string encryptText)
        {
            try
            {
                byte[] encryptDataArray = Convert.FromBase64String(encryptText.Replace(' ', '+'));

                using (var mstream = new MemoryStream(encryptDataArray))
                {
                    using (var cstream = new CryptoStream(mstream,
                                                            new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv),
                                                            CryptoStreamMode.Read))
                    {
                        byte[] DecryptDataArray = new byte[encryptDataArray.Length];

                        cstream.Read(DecryptDataArray, 0, DecryptDataArray.Length);

                        return Encoding.UTF8.GetString(DecryptDataArray).TrimEnd('\0');

                    }
                }

            }
            catch (CryptographicException ex)
            {
                throw ex;
            }

        }

        #endregion
    }

}

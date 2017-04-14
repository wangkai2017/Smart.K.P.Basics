using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartCommon.ConvertHelper
{
    public class EncryptionAndDecryptHelper
    {
        //密钥  
        private const string sKey = "aazGEh6hESZDVJeCnFPGuxzaiB7NLQM3";
        //矢量，矢量可以为空  
        private const string sIV = "ccad6X+aPLw=";
        //构造一个对称算法  
        private static SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
        public EncryptionAndDecryptHelper() { }

        #region 加密
        /// <summary> 加密字符串 </summary>
        /// <param name="Value">输入的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string EncryptString(string Value)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(sKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            //指定加密的运算模式  
            mCSP.Mode = CipherMode.ECB;
            //获取或设置加密算法的填充模式  
            mCSP.Padding = PaddingMode.PKCS7;
            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);
            byt = Encoding.UTF8.GetBytes(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
        #endregion

        #region 解密
        /// <summary> 解密字符串  </summary>
        /// <param name="Value">加过密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string DecryptString(string Value)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(sKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
            byt = Convert.FromBase64String(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        #endregion

        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        #region DES加密
        ///   <summary>   DES加密字符串   </summary>
        ///   <param   name="encryptStr">待加密的字符串</param>   
        ///   <param   name="encryptKey">加密密钥,要求为8位</param>   
        ///   <returns>加密成功返回加密后的字符串，失败返回源串</returns>   
        public static string EncryptDES(string encryptStr, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptStr);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptStr;
            }
        }
        #endregion

        #region DES解密
        ///   <summary>   DES解密字符串   </summary>
        ///   <param   name="decryptStr">待解密的字符串</param>   
        ///   <param   name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>   
        ///   <returns>解密成功返回解密后的字符串，失败返源串</returns>   
        public static string DecryptDES(string decryptStr, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptStr);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptStr;
            }
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// 获取字符数组的MD5加密值
        /// </summary>
        /// <param name="sortedArray">待计算MD5哈希值的输入字符数组</param>
        /// <param name="key">密钥</param>
        /// <param name="charset"></param>
        /// <returns>输入字符数组的MD5哈希值</returns>
        public static string GetMD5ByArray(string[] sortedArray, string key, string charset)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < sortedArray.Length; i++)
            {
                if (i == sortedArray.Length - 1)
                {
                    builder.Append(sortedArray[i]);
                }
                else
                {
                    builder.Append(sortedArray[i] + "&");
                }
            }
            builder.Append(key);
            return GetMD5(builder.ToString(), charset);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="digitLength">指定待返回的MD5哈希值的数位长度，有效取值为16或32，若输入无效值，则自动以32取代无效值</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, int digitLength)
        {
            return GetMD5(input, "utf-8", digitLength);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="charset">输入字符串的字符集</param>
        /// <param name="digitLength">指定待返回的MD5哈希值的数位长度，有效取值为16或32，若输入无效值，则自动以32取代无效值</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, string charset, int digitLength)
        {
            if (digitLength != 16 && digitLength != 32)
            {
                digitLength = 32;
            }

            if (digitLength == 32)
            {
                return GetMD5(input, charset);
            }
            else
            {
                return GetMD5(input, charset).Substring(8, 16);
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input)
        {
            return GetMD5(input, "utf-8");
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="charset">输入字符串的字符集</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, string charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(input));
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="charset">输入字符串的字符集</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, string charset, string key)
        {
            input += key;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(input));
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        } 
        #endregion
    }
}

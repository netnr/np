using System;
using System.Text;
using System.Security.Cryptography;

namespace Netnr.Core
{
    /// <summary>
    /// 算法、加密、解密
    /// </summary>
    public class CalcTo
    {
        /// <summary>
        /// 异或算法
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="key">异或因子 2-253</param>
        /// <returns>返回异或后的字符串</returns>
        public static string XorKey(string s, int key)
        {
            int n = key > 253 ? 253 : key < 2 ? 2 : key;
            byte k = byte.Parse(n.ToString());

            byte[] bytes = Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ k ^ (k + 7));
            }
            return Encoding.Unicode.GetString(bytes);
        }

        /// <summary>
        /// MD5加密 小写
        /// </summary>
        /// <param name="s">需加密的字符串</param>
        /// <param name="len">长度 默认32 可选16</param>
        /// <returns></returns>
        public static string MD5(string s, int len = 32)
        {
            string result;
            using MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(s));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            result = sb.ToString();

            //result = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToLower();
            return len == 32 ? result : result.Substring(8, 16);
        }

        #region DES 加解密

        /// <summary> 
        /// DES 加密 
        /// </summary> 
        /// <param name="Text">内容</param> 
        /// <param name="sKey">密钥</param> 
        /// <returns></returns> 
        public static string EnDES(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary> 
        /// DES 解密 
        /// </summary> 
        /// <param name="Text">内容</param> 
        /// <param name="sKey">密钥</param> 
        /// <returns></returns> 
        public static string DeDES(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        #region SHA1 加密

        /// <summary>
        /// 20字节,160位
        /// </summary>
        /// <param name="str">内容</param>
        /// <returns></returns>
        public static string SHA128(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            using SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();
            byte[] byteArr = SHA1.ComputeHash(buffer);
            return BitConverter.ToString(byteArr);
        }

        /// <summary>
        /// 32字节,256位
        /// </summary>
        /// <param name="str">内容</param>
        /// <returns></returns>
        public static string SHA256(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            using SHA256CryptoServiceProvider SHA256 = new SHA256CryptoServiceProvider();
            byte[] byteArr = SHA256.ComputeHash(buffer);
            return BitConverter.ToString(byteArr);
        }

        /// <summary>
        /// 48字节,384位
        /// </summary>
        /// <param name="str">内容</param>
        /// <returns></returns>
        public static string SHA384(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            using SHA384CryptoServiceProvider SHA384 = new SHA384CryptoServiceProvider();
            byte[] byteArr = SHA384.ComputeHash(buffer);
            return BitConverter.ToString(byteArr);
        }

        /// <summary>
        /// 64字节,512位
        /// </summary>
        /// <param name="str">内容</param>
        /// <returns></returns>
        public static string SHA512(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            using SHA512CryptoServiceProvider SHA512 = new SHA512CryptoServiceProvider();
            byte[] byteArr = SHA512.ComputeHash(buffer);
            return BitConverter.ToString(byteArr);
        }
        #endregion

        /// <summary>
        /// HMAC_SHA1 加密
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMAC_SHA1(string str, string key)
        {
            using HMACSHA1 hmacsha1 = new HMACSHA1
            {
                Key = Encoding.UTF8.GetBytes(key)
            };
            byte[] dataBuffer = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// HMAC_SHA256 加密
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMAC_SHA256(string str, string key)
        {
            using HMACSHA256 hmacsha256 = new HMACSHA256
            {
                Key = Encoding.UTF8.GetBytes(key)
            };
            byte[] dataBuffer = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = hmacsha256.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// HMACSHA384 加密
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMACSHA384(string str, string key)
        {
            using HMACSHA384 hmacsha384 = new HMACSHA384
            {
                Key = Encoding.UTF8.GetBytes(key)
            };
            byte[] dataBuffer = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = hmacsha384.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// HMACSHA512 加密
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMACSHA512(string str, string key)
        {
            using HMACSHA512 hmacsha512 = new HMACSHA512
            {
                Key = Encoding.UTF8.GetBytes(key)
            };
            byte[] dataBuffer = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = hmacsha512.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
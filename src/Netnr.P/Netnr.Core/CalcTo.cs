using System;
using System.IO;
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
        /// 编码
        /// </summary>
        public static Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// AES 构建
        /// </summary>
        /// <param name="key">密钥，默认空</param>
        /// <param name="iv">固定16位，默认空</param>
        /// <returns></returns>
        public static Aes AESBuild(string key = "", string iv = "")
        {
            var aesAlg = Aes.Create();

            byte[] bKey = new byte[32];
            Array.Copy(encoding.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bIV = new byte[16];
            Array.Copy(encoding.GetBytes(iv.PadRight(bIV.Length)), bIV, bIV.Length);

            aesAlg.Key = bKey;
            aesAlg.IV = bIV;

            return aesAlg;
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="aesAlg">AES构建对象</param>
        /// <returns></returns>
        public static string AESEncrypt(string txt, Aes aesAlg)
        {
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(txt);
            }
            var result = Convert.ToBase64String(msEncrypt.ToArray());
            aesAlg.Dispose();

            return result;
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">iv 16位 默认空</param>
        /// <returns></returns>
        public static string AESEncrypt(string txt, string key, string iv = "")
        {
            Console.WriteLine(txt);
            Console.WriteLine(key);
            return AESEncrypt(txt, AESBuild(key, iv));
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="aesAlg">AES构建对象</param>
        /// <returns></returns>
        public static string AESDecrypt(string txt, Aes aesAlg)
        {
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(Convert.FromBase64String(txt));
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            var result = srDecrypt.ReadToEnd();
            aesAlg.Dispose();

            return result;
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">iv 16位 默认空</param>
        /// <returns></returns>
        public static string AESDecrypt(string txt, string key, string iv = "")
        {
            return AESDecrypt(txt, AESBuild(key, iv));
        }

        /// <summary>
        /// DES 构建
        /// </summary>
        /// <param name="key">密钥，默认空</param>
        /// <param name="iv">固定8位，默认空</param>
        /// <returns></returns>
        public static DES DESBuild(string key = "", string iv = "")
        {
            DES DESalg = DES.Create();

            byte[] bKey = new byte[8];
            Array.Copy(encoding.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bIV = new byte[8];
            Array.Copy(encoding.GetBytes(iv.PadRight(bIV.Length)), bIV, bIV.Length);

            DESalg.Key = bKey;
            DESalg.IV = bIV;

            return DESalg;
        }

        /// <summary>
        /// DES 加密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="DESalg">DES 对象</param>
        /// <returns></returns>
        public static string DESEncrypt(string txt, DES DESalg)
        {
            var input = encoding.GetBytes(txt);

            using ICryptoTransform ct = DESalg.CreateEncryptor(DESalg.Key, DESalg.IV);
            var result = Convert.ToBase64String(ct.TransformFinalBlock(input, 0, input.Length));
            DESalg.Dispose();

            return result;
        }

        /// <summary>
        /// DES 加密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">固定8位，默认空</param>
        /// <returns></returns>
        public static string DESEncrypt(string txt, string key, string iv = "")
        {
            return DESEncrypt(txt, DESBuild(key, iv));
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="DESalg">DES 对象</param>
        /// <returns></returns>
        public static string DESDecrypt(string txt, DES DESalg)
        {
            var input = Convert.FromBase64String(txt);

            using ICryptoTransform ct = DESalg.CreateDecryptor(DESalg.Key, DESalg.IV);
            var result = encoding.GetString(ct.TransformFinalBlock(input, 0, input.Length));
            DESalg.Dispose();

            return result;
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">默认为空</param>
        /// <returns></returns>
        public static string DESDecrypt(string txt, string key, string iv = "")
        {
            return DESDecrypt(txt, DESBuild(key, iv));
        }

        /// <summary>
        /// 
        /// </summary>
        public enum HashType
        {
            /// <summary>
            /// 
            /// </summary>
            MD5,
            /// <summary>
            /// 
            /// </summary>
            SHA1,
            /// <summary>
            /// 
            /// </summary>
            SHA256,
            /// <summary>
            /// 
            /// </summary>
            SHA384,
            /// <summary>
            /// 
            /// </summary>
            SHA512,
            /// <summary>
            /// 
            /// </summary>
            HMACSHA1,
            /// <summary>
            /// 
            /// </summary>
            HMACSHA256,
            /// <summary>
            /// 
            /// </summary>
            HMACSHA384,
            /// <summary>
            /// 
            /// </summary>
            HMACSHA512,
            /// <summary>
            /// 
            /// </summary>
            HMACMD5
        }

        /// <summary>
        /// hash 加密
        /// </summary>
        /// <param name="hashType">类型</param>
        /// <param name="stream">流（注意流从开始读取，已使用的流对象需重置读取位置，so#43791043）</param>
        /// <param name="hmacKey">HMAC 密钥</param>
        /// <returns></returns>
        public static string HashBase64(HashType hashType, Stream stream, string hmacKey = "")
        {
            return Convert.ToBase64String(HashByte(hashType, stream, hmacKey));
        }

        /// <summary>
        /// hash 加密
        /// </summary>
        /// <param name="hashType">类型</param>
        /// <param name="stream">流（注意流从开始读取，已使用的流对象需重置读取位置，so#43791043）</param>
        /// <param name="hmacKey">HMAC 密钥</param>
        /// <returns></returns>
        public static string HashString(HashType hashType, Stream stream, string hmacKey = "")
        {
            return BitConverter.ToString(HashByte(hashType, stream, hmacKey)).ToLower().Replace("-", "");
        }

        /// <summary>
        /// hash 加密
        /// </summary>
        /// <param name="hashType">类型</param>
        /// <param name="stream">流（注意流从开始读取，已使用的流对象需重置读取位置，so#43791043）</param>
        /// <param name="hmacKey">HMAC 密钥</param>
        /// <returns></returns>
        public static byte[] HashByte(HashType hashType, Stream stream, string hmacKey = "")
        {
            return hashType switch
            {
                HashType.MD5 => System.Security.Cryptography.MD5.Create().ComputeHash(stream),
                HashType.SHA1 => SHA1.Create().ComputeHash(stream),
                HashType.SHA256 => SHA256.Create().ComputeHash(stream),
                HashType.SHA384 => SHA384.Create().ComputeHash(stream),
                HashType.SHA512 => SHA512.Create().ComputeHash(stream),
                HashType.HMACSHA1 => new HMACSHA1(encoding.GetBytes(hmacKey)).ComputeHash(stream),
                HashType.HMACSHA256 => new HMACSHA256(encoding.GetBytes(hmacKey)).ComputeHash(stream),
                HashType.HMACSHA384 => new HMACSHA384(encoding.GetBytes(hmacKey)).ComputeHash(stream),
                HashType.HMACSHA512 => new HMACSHA512(encoding.GetBytes(hmacKey)).ComputeHash(stream),
                HashType.HMACMD5 => new HMACMD5(encoding.GetBytes(hmacKey)).ComputeHash(stream),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// SHA 加密 小写
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        private static string GetHashString(HashAlgorithm algorithm, string txt)
        {
            return BitConverter.ToString(algorithm.ComputeHash(encoding.GetBytes(txt))).ToLower().Replace("-", "");
        }

        /// <summary>
        /// MD5加密 小写
        /// </summary>
        /// <param name="txt">需加密的字符串</param>
        /// <param name="len">长度 默认32 可选16</param>
        /// <returns></returns>
        public static string MD5(string txt, int len = 32)
        {
            var result = GetHashString(System.Security.Cryptography.MD5.Create(), txt).ToLower();
            return len == 32 ? result : result.Substring(8, 16);
        }

        /// <summary>
        /// 20字节,160位
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns></returns>
        public static string SHA_1(string txt)
        {
            return GetHashString(SHA1.Create(), txt);
        }

        /// <summary>
        /// 32字节,256位
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns></returns>
        public static string SHA_256(string txt)
        {
            return GetHashString(SHA256.Create(), txt);
        }

        /// <summary>
        /// 48字节,384位
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns></returns>
        public static string SHA_384(string txt)
        {
            return GetHashString(SHA384.Create(), txt);
        }

        /// <summary>
        /// 64字节,512位
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns></returns>
        public static string SHA_512(string txt)
        {
            return GetHashString(SHA512.Create(), txt);
        }

        /// <summary>
        /// HMAC_SHA1 加密
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMAC_SHA1(string txt, string key)
        {
            return GetHashString(new HMACSHA1(encoding.GetBytes(key)), txt);
        }

        /// <summary>
        /// HMAC_SHA256 加密
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMAC_SHA256(string txt, string key)
        {
            return GetHashString(new HMACSHA256(encoding.GetBytes(key)), txt);
        }

        /// <summary>
        /// HMACSHA384 加密
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMAC_SHA384(string txt, string key)
        {
            return GetHashString(new HMACSHA384(encoding.GetBytes(key)), txt);
        }

        /// <summary>
        /// HMACSHA512 加密
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMAC_SHA512(string txt, string key)
        {
            return GetHashString(new HMACSHA512(encoding.GetBytes(key)), txt);
        }

        /// <summary>
        /// HMACMD5 加密
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string HMAC_MD5(string txt, string key)
        {
            return GetHashString(new HMACMD5(encoding.GetBytes(key)), txt);
        }

        /// <summary>
        /// 生成 私钥、公钥 
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="keySize"></param>
        public static void RSAKey(out string xmlPrivateKey, out string xmlPublicKey, int? keySize = null)
        {
            using RSACryptoServiceProvider rsa = keySize == null ? new() : new(keySize.Value);
            xmlPrivateKey = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="encryptContent">加密内容</param>
        /// <returns></returns>
        public static string RSAEncrypt(string xmlPublicKey, string encryptContent)
        {
            return RSAEncrypt(xmlPublicKey, encoding.GetBytes(encryptContent));
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="encryptContent">加密内容</param>
        /// <returns></returns>
        public static string RSAEncrypt(string xmlPublicKey, byte[] encryptContent)
        {
            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(xmlPublicKey);

            var byteEncrypt = rsa.Encrypt(encryptContent, false);
            var result = Convert.ToBase64String(byteEncrypt);

            return result;
        }

        /// <summary>
        /// RSA 解密
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="decryptContent"></param>
        /// <returns></returns>
        public static string RSADecrypt(string xmlPrivateKey, string decryptContent)
        {
            return RSADecrypt(xmlPrivateKey, Convert.FromBase64String(decryptContent));
        }

        /// <summary>
        /// RSA 解密
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="decryptContent">解密内容</param>
        /// <returns></returns>
        public static string RSADecrypt(string xmlPrivateKey, byte[] decryptContent)
        {
            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(xmlPrivateKey);

            var byteDecrypt = rsa.Decrypt(decryptContent, false);
            var result = encoding.GetString(byteDecrypt);

            return result;
        }
    }
}
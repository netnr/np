using System;

namespace Netease.Cloud.NOS.Authentication
{
    internal abstract class ServiceSignature
    {
        /// <summary>
        /// 获取加密算法
        /// </summary>
        public abstract string SignatureMethod { get; }

        /// <summary>
        /// 计算签名字符串
        /// </summary>
        /// <param name="key">用户用于加密签名字符串和NOS用来验证签名字符串的密钥</param>
        /// <param name="data">需加密的内容</param>
        /// <returns>签名字符串</returns>
        public string ComputeSignature(string key, string data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key to compute signature is null or empty", "key");
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("The data of compute signature is null or empty", "data");

            return ComputeSignatureCore(key, data);
        }

        protected abstract string ComputeSignatureCore(string key, string data);

        public static ServiceSignature Create()
        {
            return new HmacSHA256Signature();
        }
    }

}

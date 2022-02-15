using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Netease.Cloud.NOS.Authentication
{
    /// <summary>
    /// HmacSHA1签名算法
    /// </summary>
    internal class HmacSHA256Signature : ServiceSignature
    {
        private static readonly Encoding Encoding = Encoding.UTF8;

        public override string SignatureMethod
        {
            get { return "HmacSHA256"; }
        }

        protected override string ComputeSignatureCore(string key, string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            //using (var algorithm = KeyedHashAlgorithm.Create(SignatureMethod.ToUpperInvariant()))
            using (var algorithm = new HMACSHA256())
            {
                algorithm.Key = Encoding.GetBytes(key.ToCharArray());
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.GetBytes(data.ToCharArray())));
            }
        }
    }
}

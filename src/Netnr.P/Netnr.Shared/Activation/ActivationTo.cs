#if Full || Activation

using System.Security.Cryptography;

namespace Netnr;

/// <summary>
/// 激活
/// </summary>
public class ActivationTo
{
    /// <summary>
    /// 默认编码
    /// </summary>
    private static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// 生成密钥对
    /// </summary>
    /// <param name="publicKeyPem"></param>
    /// <param name="privateKeyPem"></param>
    public static void GenerateKeyPair(out string publicKeyPem, out string privateKeyPem)
    {
        using var rsa = new RSACryptoServiceProvider(4096);
        rsa.PersistKeyInCsp = false;

        publicKeyPem = rsa.ExportRSAPublicKeyPem();
        privateKeyPem = rsa.ExportRSAPrivateKeyPem();
    }

    /// <summary>
    /// 公钥加密数据
    /// </summary>
    /// <param name="publicKeyPem">公钥</param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Encrypt(string publicKeyPem, byte[] data)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        var result = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA512);
        return result;
    }

    /// <summary>
    /// 私钥解密数据
    /// </summary>
    /// <param name="privateKeyPem"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Decrypt(string privateKeyPem, byte[] data)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        var result = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA512);
        return result;
    }

    /// <summary>
    /// 码实体
    /// </summary>
    public class CodeModel
    {
        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; } = "netnr";
        /// <summary>
        /// 颁发给
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 主体信息
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 开始
        /// </summary>
        public DateTime NotBefore { get; set; } = DateTime.Now;
        /// <summary>
        /// 结束
        /// </summary>
        public DateTime NotAfter { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; } = 1;
    }
    private static string ToModelString(CodeModel model)
    {
        var result = string.Join("&", [
            $"Issuer={Uri.EscapeDataString(model.Issuer??"")}",
            $"Subject={Uri.EscapeDataString(model.Subject??"")}",
            $"Body={Uri.EscapeDataString(model.Body??"")}",
            $"NotBefore={model.NotBefore}",
            $"NotAfter={model.NotAfter}",
            $"Version={model.Version}"
        ]);

        return result;
    }
    private static CodeModel DeModelString(string modelString)
    {
        try
        {
            var items = modelString.Split("&");
            var result = new CodeModel();
            foreach (var item in items)
            {
                var nv = item.Split("=");
                var name = nv[0];
                var value = nv[1];
                if (name == "Issuer")
                {
                    result.Issuer = Uri.UnescapeDataString(value);
                }
                else if (name == "Subject")
                {
                    result.Subject = Uri.UnescapeDataString(value);
                }
                else if (name == "Body")
                {
                    result.Body = Uri.UnescapeDataString(value);
                }
                else if (name == "NotBefore")
                {
                    if (DateTime.TryParse(value, out var val))
                    {
                        result.NotBefore = val;
                    }
                }
                else if (name == "NotAfter")
                {
                    if (DateTime.TryParse(value, out var val))
                    {
                        result.NotAfter = val;
                    }
                }
                else if (name == "Version")
                {
                    result.Version = Convert.ToInt32(value);
                }
            }
            return result;
        }
        catch (Exception) { }
        return null;
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="publicKeyPem">公钥</param>
    /// <param name="model"></param>
    /// <returns></returns>
    public static string CodeGenerate(string publicKeyPem, CodeModel model)
    {
        var data = DefaultEncoding.GetBytes(ToModelString(model));
        var result = Encrypt(publicKeyPem, data);
        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// 解密激活码
    /// </summary>
    /// <param name="privateKeyPem">私钥</param>
    /// <param name="code"></param>
    /// <returns></returns>
    public static CodeModel CodeDecrypt(string privateKeyPem, string code)
    {
        try
        {
            var bytes = Decrypt(privateKeyPem, Convert.FromBase64String(code));
            var result = DeModelString(DefaultEncoding.GetString(bytes));
            return result;
        }
        catch (Exception) { }

        return null;
    }
}

#endif
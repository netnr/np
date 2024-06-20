using OtpNet;
using Xunit;

namespace Netnr.Test
{
    public class TestOTP
    {
        [Fact]
        public void TOTP()
        {
            string secretKey = ""; // 粘贴密钥
            var totp = new Totp(Base32Encoding.ToBytes(secretKey));
            var code = totp.ComputeTotp();
            Debug.WriteLine(code);
        }

        [Fact]
        public void GenerateSecertTOTP()
        {
            var otpKey = KeyGeneration.GenerateRandomKey();
            var secert = Base32Encoding.ToString(otpKey);

            var otpUrl = new OtpUri(OtpType.Totp, secert, "netnr@netnr.com", "netnrcom@gmail.com").ToString();
            Debug.WriteLine(otpUrl);
        }
    }
}
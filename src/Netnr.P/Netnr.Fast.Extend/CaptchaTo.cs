using System.Runtime.InteropServices;

namespace Netnr.Fast
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class CaptchaTo
    {
        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <param name="code">随机码</param>
        public static byte[] CreateImg(string code)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CaptchaForDrawingCommon.CreateImg(code);
            }
            else
            {
                return CaptchaForSkiaSharp.CreateImg(code);
            }
        }
    }
}

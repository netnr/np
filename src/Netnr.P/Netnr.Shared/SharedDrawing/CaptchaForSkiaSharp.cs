#if Full || Drawing

using System;
using SkiaSharp;

namespace Netnr.SharedDrawing
{
    /// <summary>
    /// 引用组件：SkiaSharp、SkiaSharp.NativeAssets.Linux
    /// 跨平台
    /// </summary>
    public class CaptchaForSkiaSharp
    {
        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <param name="code">随机码</param>
        public static byte[] CreateImg(string code)
        {
            var random = new Random();

            //为验证码插入空格
            for (int i = 0; i < 2; i++)
            {
                code = code.Insert(random.Next(code.Length - 1), " ");
            }

            //验证码颜色集合  
            SKColor[] colors = { SKColors.LightBlue, SKColors.LightCoral, SKColors.LightGreen, SKColors.LightPink, SKColors.LightSkyBlue, SKColors.LightSteelBlue, SKColors.LightSalmon };

            //旋转角度
            int randAngle = 40;

            using SKBitmap bitmap = new SKBitmap(code.Length * 22, 38);
            using SKCanvas canvas = new SKCanvas(bitmap);
            //背景设为白色
            canvas.Clear(SKColors.White);

            //在随机位置画背景点
            for (int i = 0; i < 200; i++)
            {
                int x = random.Next(0, bitmap.Width);
                int y = random.Next(0, bitmap.Height);

                var paint = new SKPaint() { Color = colors[random.Next(colors.Length)] };
                canvas.DrawRect(new SKRect(x, y, x + 2, y + 2), paint);
            }

            //验证码绘制
            for (int i = 0; i < code.Length; i++)
            {
                //角度
                float angle = random.Next(-randAngle, randAngle);

                //不同高度
                int ii = random.Next(20) * (random.Next(1) % 2 == 0 ? -1 : 1) + 20;

                SKPoint point = new SKPoint(18, 20);

                canvas.Translate(point);
                canvas.RotateDegrees(angle);

                var textPaint = new SKPaint()
                {
                    TextAlign = SKTextAlign.Center,
                    Color = colors[random.Next(colors.Length)],
                    TextSize = 28,
                    IsAntialias = true,
                    FakeBoldText = true
                };

                canvas.DrawText(code.Substring(i, 1), new SKPoint(0, ii), textPaint);
                canvas.RotateDegrees(-angle);
                canvas.Translate(0, -point.Y);
            }

            canvas.Translate(-4, 0);

            using var image = SKImage.FromBitmap(bitmap);
            using var ms = new System.IO.MemoryStream();
            image.Encode(SKEncodedImageFormat.Jpeg, 90).SaveTo(ms);

            return ms.ToArray();
        }
    }
}

#endif
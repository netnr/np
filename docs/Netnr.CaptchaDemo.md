# Netnr.CaptchaDemo
跨平台的绘图使用，生成验证码，加水印，裁剪等

### 测验
- `System.Drawing.Common` 最快，Windows 首选，Linux 需依赖 **libgdiplus** 组件
- `SkiaSharp` 快，和 `System.Drawing.Common` 速度非常接近，跨平台优势
- `SixLabors.ImageSharp.Drawing` 很慢，跨平台
- `Magick.NET` 很慢
# Netnr.CaptchaDemo
验证码示例

> 演示：<https://d-captcha.zme.ink>

### 测验
- `System.Drawing.Common` 最快，Windows 首选，Linux 需依赖 **libgdiplus** 组件
- `SkiaSharp` 快，和 `System.Drawing.Common` 速度非常接近，跨平台优势
- `SixLabors.ImageSharp.Drawing` 很慢，跨平台
- `Magick.NET` 很慢
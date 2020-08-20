# Netnr.CaptchaDemo
Verification code example

> Demo: <https://netnr-captcha.herokuapp.com>

### Quiz
- `System.Drawing.Common` is the fastest, Windows is the first choice, Linux requires **libgdiplus** components
- `SkiaSharp` is fast, very close to `System.Drawing.Common` speed, cross-platform advantage
- `SixLabors.ImageSharp.Drawing` is slow and cross-platform
- `Magick.NET` is slow
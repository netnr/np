# Netnr.Fast.Extend
Netnr.Fast 的拓展

### 类
- CaptchaTo.cs　　生成验证码
- ClientTo.cs　　获取客户端的一些信息（需要传入 `HttpContext`）
- DownTo.cs　　流下载文件（需要传入 `HttpResponse`）
- ImageTo.cs　　图片操作（缩略图、水印）
- NpoiTo.cs　　操作Excel（依赖 `NPOI`，Excel文件与DataTable相互转换，支持 .xls、.xlsx）
- OSInfoTo.cs　　获取系统信息（依赖`Cmd`、`Shell`命令）
- PinyinTo.cs　　中文转拼音（NPinyin）
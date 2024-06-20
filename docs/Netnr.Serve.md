# NS (Netnr.Serve)
简单的 HTTP 静态文件服务  
Simple HTTP static file serving

### Start (启动)
启动逐个参数设置
```bat
--urls (default: http://*:7713/):
--root (default: D:/site): #根目录，默认命令行启动位置
--index (default: index.html):
--404 (default: 404.html):
--suffix (default: .html):
--charset (default: utf-8):
--readonly (default: false): #只读则不能上传、删除
--auth (user:pass): #设置 Basic 授权访问
--headers (k1:v1||k2:v2):
```

静默带参启动
```bat
ns --urls http://*:7713/ --readonly true
```

### List (列表)
```bash
curl http://localhost:7713/ #列表，默认浏览器 FTP，curl 友好
curl http://localhost:7713/-u user:pass #带授权

(iwr http://localhost:7713/).content #PowerShell
```
### Download (下载)
```bash
curl http://localhost:7713//file.exe -O
curl http://localhost:7713/dir/?zip #下载文件夹，不压缩打包目录并实时输出流

iwr http://localhost:7713//file.exe -outfile file.exe #PowerShell
```
### Upload (上传)
```bash
curl http://localhost:7713/-T file.ext #上传文件
curl http://localhost:7713/dir/rename.ext -T file.ext #自定义路径上传

iwr http://localhost:7713/dir/rename.ext -method put -infile file.ext
```
### Delete (删除)
```bash
curl http://localhost:7713/file.ext -X delete #删除

iwr http://localhost:7713/file.ext -method delete #PowerShell
```

### Release (发布)
win-x64 NativeAOT 3.14M  
https://r2.zme.ink/releases/ns-8.0.1-win-x64.exe

linux-x64 NativeAOT 7M  
https://r2.zme.ink/releases/ns-8.0.1-linux-x64.aot

osx-x64 NativeAOT 5.25M  
https://r2.zme.ink/releases/ns-8.0.1-osx-x64.aot

建议下载后改名为 `ns` 再配一个环境变量，更方便使用  
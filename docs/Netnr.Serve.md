# NS (Netnr.Serve)
简单 WEB 服务  
Simple Web Service

https://github.com/netnr/np/releases

### Start (启动)
```bat
--urls http://*:713/ #服务端口号
--root D:\site\npp\docs #根目录，默认当前
--index index.html #默认首页
--404 404.html #支持 history 
--suffix .html #默认后缀
--charset utf-8 #编码
--auth user:pass #Basic 授权
--headers access-control-allow-headers:*||access-control-allow-origin:* #添加跨域头
```
### List (列表)
```bash
curl http://*:713/ #列表，默认浏览器FTP，已为 curl 优化显示
curl http://*:713/ -u user:pass #带授权
(iwr http://*:713/).content #PowerShell
```
### Download (下载)
```bash
curl http://*:713//file.exe -O
iwr http://*:713//file.exe -outfile file.exe #PowerShell
```
### Upload (上传)
```bash
curl http://*:713/ -T file.ext #上传文件
curl http://*:713/dir/rename.ext -T file.ext #自定义路径上传
```
### Delete (删除)
```bash
curl http://*:713/file.ext -X delete #删除
curl http://*:713/dir/?force -X delete #删除目录
iwr http://*:713/file.ext -method delete #PowerShell
```
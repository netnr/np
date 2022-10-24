# NFS (Netnr.FileServer)
简单文件服务器  
simple file server

`appsettings.json` 为配置文件，文件数据库 和 上传目录 赋予读写权限

https://github.com/netnr/np/releases

### 功能
- [x] 获取 Token 授权操作（有效期内访问所有接口）
- [x] 创建 FixedToken 并配置允许访问的接口（永久有效访问授权的接口）
- [x] 上传文件和分块上传文件
- [x] 复制已上传的文件
- [x] 上传覆盖文件
- [x] 删除文件
- [x] 上传临时文件
- [x] 清理临时文件

### 接口
- `/API/CreateApp` 创建 App
- `/API/GetAppList` 获取 App 列表
- `/API/GetAppInfo` 获取 App 信息
- `/API/ResetAll` 清空数据库和上传目录
- `/API/ClearTmp` 清理临时目录
- ==== 以上为管理接口，需密码验证，设为空密码可关闭管理接口
- `/API/GetToken` 根据 AppId、AppKey 请求 Token
- `/API/CreateFixedToken` 创建 FixedToken
- `/API/DelFixedToken` 删除 FixedToken
- `/API/Upload` 上传文件
- `/API/UploadChunk` 分块上传文件
- `/API/Copy` 复制已上传的文件
- `/API/Cover` 上传文件覆盖
- `/API/Delete` 删除文件
- `/API/UploadTmp` 上传临时文件

### 授权
首先创建 App 得到 AppId 、AppKey，然后根据 AppId、AppKey 请求得到 Token，  
Token 可根据配置设置有效期，默认 30 分钟有效，缓存 20 分钟（即 20 分钟内请求 Token 返回结果相同）  
或创建 FixedToken 固定永久有效

### 分离
为了更好的维护或数据的安全，需要分离文件数据库和上传的静态目录，  
可以用 `软链接` 的方式来做，`非` Windows 的快捷方式  

```bash
# Windows 软链接
mklink /d 软链接目录 物理目录 # 命令格式
mklink /d static D:\static # 示例 在当前创建 static 目录 指向 D 盘的 static 目录

# Linux 软链接
ln -s 源文件 软链接文件 # 命令格式
ln -s /mnt/static/site/fileserver/wwwroot/static # 示例 static 指向 /mnt/static 目录
```
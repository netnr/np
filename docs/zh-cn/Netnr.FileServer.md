# Netnr.FileServer
基于 .NET Core 的简单文件服务器，数据库为 SQLite

> 生产环境在创建 App 之后需设置 `IsDev:false`，设置直接生效不用重启服务，`appsettings.json` 为配置文件  
> 文件数据库 和 上传目录 赋予读写权限


### 运行
- 有环境依赖
    - 在根目录运行：`dotnet Netnr.FileServer.dll "http://*:55"`
    - Linux后台运行：`nohup dotnet Netnr.FileServer.dll "http://*:55" &`
- 无环境依赖
    - Linux运行：`chmod +x Netnr.FileServer` 先给运行权限，`./Netnr.FileServer "http://*:55"` 再运行
- Windows
    - Windows可直接双击`Netnr.FileServer.exe`，或命令运行`Netnr.FileServer.exe "http://*:55"`
    - 挂载IIS

### 访问
服务运行后，访问 `{Host}/swagger`，可以直接使用所有的接口

### 授权
首先创建 App 得到 AppId 、AppKey，然后根据 AppId、AppKey 请求得到 Token，  
Token 可根据配置设置有效期，默认30分钟有效，缓存20分钟（即20分钟内请求Token返回结果相同）

### 接口
- `/api/createapp` 创建App，**非生产环境使用**
- `/api/getapplist` 获取App列表，**非生产环境使用**
- `/api/resetall` 清空数据库和上传目录，**非生产环境使用**
- 在生产环境下一定要修改配置 `IsDev:false` , 关闭以上接口
- 
- `/api/gettoken` 根据AppId、AppKey请求Token
- `/api/upload` 上传文件
- `/api/copy` 复制文件
- `/api/cover` 上传文件覆盖
- `/api/delete` 删除文件

### 上传
默认上传到目录 **wwwroot/static/** ， `/static/`可根据配置文件配置

### 分离
为了更好的维护或数据的安全，需要分离文件数据库和上传的静态目录，  
可以用 `软链接` 的方式来做，`非` Windows的快捷方式  
```
// Windows 软链接
// 命令格式
mklink /d 软链接目录 物理目录 
// 示例 在当前创建 static 目录 指向 D盘的 static 目录
mklink /d static D:\static
```
```
# Linux 软链接
ln -s 源文件 软链接文件 # 命令格式
# 示例 gs 指向 /netnr/site/static 目录
ln -s /netnr/site/static /netnr/site/www/wwwroot/gs
```
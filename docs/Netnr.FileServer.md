# Netnr.FileServer
Simple file server based on .NET Core, database is SQLite

> Demo: <https://d-fileserver.zme.ink>


> After creating the App in the production environment, you need to set `IsDev:false`  
> the setting will take effect directly without restarting the service, `appsettings.json` is the configuration file  
> File database and upload directory grant read and write permissions

### Run
- Environment dependent
    - Run in the root directory: `dotnet Netnr.FileServer.dll "http://*:55"`
    - Linux background running: `nohup dotnet Netnr.FileServer.dll "http://*:55" &`
- No environment dependence
    - Linux operation: `chmod +x Netnr.FileServer` first give the permission to run, `./Netnr.FileServer "http://*:55"` then run
- Windows
    - Windows can directly double click `Netnr.FileServer.exe`, or command to run `Netnr.FileServer.exe "http://*:55"`
    - Mount IIS

### Visit
After the service is running, visit `{Host}/swagger`, you can use all the interfaces directly

### Authorization
First create App to get AppId and AppKey, and then get Token according to AppId and AppKey request,  
Token can set the validity period according to the configuration, the default is 30 minutes, and the cache is 20 minutes (that is, the request token returns the same result within 20 minutes)

### Interface
- `/api/createapp` create App, **use in non-production environment**
- `/api/getapplist` Get App list, **use in non-production environment**
- `/api/resetall` to clear the database and upload the directory, **for non-production environment use**
- Be sure to modify the configuration `IsDev:false` in the production environment and close the above interface
- 
- `/api/gettoken` request Token based on AppId and AppKey
- `/api/upload` upload files
- `/api/copy` to copy files
- `/api/cover` upload file coverage
- `/api/delete` delete files

### Upload
Upload to the directory **wwwroot/static/** by default, `/static/` can be configured according to the configuration file

### Separation
For better maintenance or data security, it is necessary to separate the file database and the uploaded static directory,
It can be done with `soft link`, `non-` Windows shortcut
```
// Windows soft link
// Command format
mklink /d soft <link directory> <physical directory>
// Example Create a static directory in the current point to the static directory of the D drive
mklink /d static D:\static
```
```
# Linux soft link
ln -s source <file soft> <link file> # command format
# Example gs points to the /netnr/site/static directory
ln -s /netnr/site/static /netnr/site/www/wwwroot/gs
```
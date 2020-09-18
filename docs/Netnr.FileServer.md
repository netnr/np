# Netnr.FileServer (NFS)
Simple file server based on .NET Core with SQLite database

> Demo: <https://d-fileserver.zme.ink>


> `appsettings.json` is the configuration file, the settings take effect directly without restarting the service
> File database and upload directory grant read and write permissions

### Features
- [x] Get Token authorization operation (access all interfaces within the validity period)
- [x] Create FixToken and configure allowed access interface (permanent and effective access authorized interface)
- [x] Upload files and upload files in blocks
- [x] Copy uploaded files
- [x] Upload overlay file
- [x] Delete file
- [x] Upload temporary files
- [x] Clean up temporary files

### Interface
- `/API/CreateApp` Create App
- `/API/GetAppList` Get App List
- `/API/GetAppInfo` to get App information
- `/API/ResetAll` Clear database and upload directory
- `/API/ClearTmp` to clean up the temporary directory
- The above is the management interface, password verification is required, set a blank password to close the management interface
- `/API/GetToken` requests tokens based on AppId and AppKey
- `/API/CreateFixToken` Create FixToken
- `/API/DelFixToken` delete FixToken
- `/API/Upload` upload files
- `/API/UploadChunk` upload files in chunks
- `/API/Copy` to copy uploaded files
- `/API/Cover` upload file coverage
- `/API/Delete` delete files
- `/API/UploadTmp` upload temporary files

### Run
- Environment dependent
    - Run in the root directory: `dotnet Netnr.FileServer.dll "http://*:55"`
    - Linux background operation: `nohup dotnet Netnr.FileServer.dll "http://*:55" &`
- No environment dependence
    - Linux operation: `chmod +x Netnr.FileServer` first give the permission to run, `./Netnr.FileServer "http://*:55"` then run
- Windows
    - Windows can directly double click `Netnr.FileServer.exe`, or command to run `Netnr.FileServer.exe "http://*:55"`
    - Mount IIS

### Authorization
First create App to get AppId and AppKey, and then get Token according to AppId and AppKey request,  
The validity period of the token can be set according to the configuration, the default is 30 minutes, and the cache is 20 minutes (that is, the result of requesting the token within 20 minutes is the same)

### Separation
For better maintenance or data security, it is necessary to separate the file database and the uploaded static directory.  
You can do it in the way of `soft link`, `non-Windows shortcut

```sh
# Windows soft link
mklink /d soft link_directory physical_directory # command format
mklink /d static D:\static # Example Create a static directory at the current point to the static directory of D drive

# Linux soft link
ln -s source file_soft link_file # command format
ln -s /mnt/static /site/fileserver/wwwroot/static # example static points to the /mnt/static directory
```
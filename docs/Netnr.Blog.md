# Netnr.Blog
Personal site

> Demo: <https://www.netnr.com>

<h3><a href="static/pd/Netnr.Blog.SQLServer.pdm" title="PD design" target="_blank">Netnr.Blog.SQLServer.pdm</a></h3>

### Framework components
- jQuery + Bootstrap4
- .NET (latest)
- EF + Linq
- Support: SQLServer, MySQL, PostgreSQL, SQLite, InMemory, etc.
- ==========================================
- FluentScheduler (timed task)
- MailKit (mailbox verification)
- Netease.Cloud.Nos (NetEase Object Storage)
- Netnr.Core (public class library)
- Netnr.Login (third-party login)
- Netnr.WeChat (WeChat public account)
- Qcloud.Shared.NetCore (Tencent Object Storage)
- Qiniu.Shared (seven cattle object storage)
- sqlite-net-pcl (SQLite, log)
- Swashbuckle.AspNetCore (Swagger generation interface)

### Functional module
- Login, register (third party direct login: QQ, Weibo, GitHub, Taobao, Microsoft)
- Article: Post an article (Markdown Editor)
- Article message: Support anonymous message, get avatar from Gravatar according to mailbox
- WeChat public number: (toy)
- Gist: code snippet, automatically sync GitHub, Gitee
- Run: Run HTML code online and write demo
- Doc: document management, API documentation
- Draw: draw, integrate open source project mxGraph, Baidu brain map
- Note: Notepad (Markdown editor)
- Storage: Cloud storage, object storage
- Backup: automatic database backup
- Log: access log records, statistics

### Changelog
- <https://www.netnr.com/home/list/131>
- The source code will be updated only after the station is updated, asynchronous update

### FQA

**Sample data**

The first run of the project automatically writes sample data, account: `netnr`, password: `123456`  
The sample data is stored in the static resource wwwroot directory, access address: `{Host}/scripts/example/data.json`

**How ​​to add article tags**

Visit `{Host}/admin/keyvalues` to add tags, administrator access,  
Tags depend on KeyValues ​​and KeyValueSynonym  
If you enter `javascript`, grab the description of the word from Wikipedia (the probability of grabbing failure is high, you need to try again), (optional) add the synonym `js`, then add `javascript` to the tag

**What the Markdown editor uses**

Please see here: <https://md.netnr.com>
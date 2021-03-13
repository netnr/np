# Netnr.Blog
个人站点

### 框架组件
- jQuery + Bootstrap4
- .NET (latest)
- EF + Linq
- 支持：SQLServer、MySQL、PostgreSQL、SQLite、InMemory等
- ==========================================
- FluentScheduler（定时任务）
- MailKit（邮箱验证）
- Netease.Cloud.Nos（网易对象存储）
- Netnr.Core（公共类库）
- Netnr.Login（第三方登录）
- Netnr.WeChat（微信公众号）
- Qcloud.Shared.NetCore（腾讯对象存储）
- Qiniu.Shared（七牛对象存储）
- sqlite-net-pcl（SQLite，日志）
- Swashbuckle.AspNetCore（Swagger 生成接口）

### 功能模块
- 登录、注册（第三方直接登录：QQ、微博、GitHub、淘宝、Microsoft）
- 文章：发布文章（Markdown编辑器）
- 文章留言：支持匿名留言，根据邮箱从 Gravatar 获取头像
- 公众号：（玩具）
- Gist：代码片段，自动同步GitHub、Gitee
- Run：在线运行HTML代码，写demo用
- Doc：文档管理，API说明文档
- Draw：绘制，集成开源项目 mxGraph、百度脑图
- Note：记事本（Markdown编辑器）
- 存储：云存储，对象存储
- 备份：自动备份数据库
- 日志：访问日志记录、统计

### FQA
- 示例数据  
  - 第一次运行项目自动写入示例数据，账号：`netnr`，密码：`123456`，示例数据存放在 `db/data.json`
- 后台管理
  - 管理员登录后访问 `/admin`
  - 文章管理、回复管理、日志列表、日志图表、键值标签
- 添加文章标签
  - 管理员访问 `/admin/keyvalues`
  - 标签表(Tags)依赖键值表(KeyValues)和键值同义词表(KeyValueSynonym)
  - 如输入`javascript`，从百科抓取该词描述（抓取失败机率高，需重试），（可选）添加同义词`js`，再添加 `javascript` 到标签
- Markdown 编辑器用的什么
  - 请看这里：<https://md.js.org>
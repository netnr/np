# Netnr.Blog
个人站点

> https://www.netnr.com  
> https://netnr.zme.ink 镜像站点

### 框架组件
- Bootstrap
- .NET （latest） + EF + Linq
- 支持：SQLServer、MySQL、PostgreSQL、SQLite 等
- ==================================
- Hangfire（定时任务）
- Markdig （markdown 解析）
- HtmlSanitizer （XSS 清洗）
- SkiaSharp （验证码）
- Swashbuckle.AspNetCore（Swagger 生成接口）
- MailKit（邮箱验证）
- Netnr.Login（第三方登录）

### 功能模块
- 登录、注册（第三方直接登录：QQ、微博、GitHub、淘宝、Microsoft）
- 文章：发布文章（Markdown 编辑器）
- 留言：文章留言（Markdown 编辑器）
- Gist：代码片段
- Run：在线运行 HTML 代码，写 demo 用
- Doc：文档管理，API 说明文档
- Draw：绘制，集成开源项目 mxGraph、百度脑图
- Guff：尬服，分享有趣的任何
- Note：记事本（Markdown 编辑器）
- 备份：自动备份数据库到私有 GitHub、Gitee
- 管理：文章、留言管理、日志记录、日志统计、键值标签等

### FQA
- 先修改配置，appsettings.json 修改为自己对应的参数
  - 数据库连接、域名、资源路径、三方登录 Key、接口密钥、邮箱、备份数据库的私有仓库
  - 首次运行项目自动导入示例数据，账号：`netnr`，密码：`123456`，示例数据存放在 `db` 目录
- 后台管理
  - 管理员登录后访问 `/admin`
  - 文章管理、回复管理、日志列表、日志图表、键值标签
  - 添加文章标签 `/admin/keyvalues`
  - 标签表 （Tags） 依赖键值表 （KeyValues） 和键值同义词表 （KeyValueSynonym）
  - 如输入 `javascript`，从百科抓取该词描述（抓取失败机率高，需重试），（可选）添加同义词 `js`，再添加 `javascript` 到标签
- Markdown 编辑器 <https://md.js.org>
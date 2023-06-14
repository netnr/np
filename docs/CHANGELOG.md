# Change Log

### 2023-04
- 升级 个站前端去除 shoelace-style 组件库，因为 bootstrap-5.3 已经开始支持暗黑主题，使用 webpack 打包
- 升级 .NET 7.0.5
- 删除 站点对象存储接入（使用次数较少）
- 删除 Gist 同步
- 优化 DbHelper 为 DbKit 并为异步读取数据，DataKit 依赖为 DbKit

### 2022-08
- 调整 Newtonsoft.Json 组件为 System.Text.Json

### 2022-04
- 升级 站点引入 shoelace-style 组件库，去 jquery、font-awesome

### 2022-02
- 发布 NDK (Netnr.DataKit)
- 发布 NDX (Netnr.DataX)
- 发布 NS (Netnr.Serve)
- 发布 NFS (Netnr.FileServer)
- https://gitee.com/netnr/np/releases
- https://github.com/netnr/np/releases

### 2021-06
- 新增 Netnr.DataX 数据转换
- 优化 Proxy 代理接口
- 新增 接口生成二维码
- 升级 .NET6 Preview
- 调整 项目数据库从 MySQL 迁移到 SQLServer

### 2021-04
- 新增 Blog 文章详情支持目录定位
- 修复 DbHelper 批量写入 SQLite 丢失几条数据的问题

### 2021-03
- 调整 Blog 项目改为宽屏
- 整合 项目
- 优化 Run 预览、输出等
- 新增 Gist 集成 asciinema 终端文本录屏

### 2021-02
- 新增 Netnr.ScriptService 项目 PowerDesigner 解析查看
- 调整 Blog、NRF 项目数据库从 SQLServer 迁移到 MySQL

### 2021-01
- 修复 Netnr.ResponseFramework 项目 Iframe 选项卡隐藏造成 EasyUI 表格显示不正常的问题（从隐藏切换改为浮动层 & 透明度的模式）
- 新增 Draw 支持私有设置分享码查看
- 修复 Blog 文章分类图标错误
- 调整 Blog 关闭匿名回复
- 修复 Netnr.DataKit 项目修复 Oracle 查询默认值（Long 类型）丢失的问题（`InitialLONGFetchSize = -1`）

### 2020-12
- 升级 .NET5
- 调整 提取共享项目
- 调整 Netnr.Login 删除 Netnr.Core 依赖
- 调整 Netnr.WeChat 删除 Netnr.Core 依赖
- 新增 数据库连接字符串可配置明文或密文（使用 Netnr.Tool 工具加密解密）
- 调整 ConsoleTo.Log 日志写入添加安全队列，可并发写入
- 调整 日志缓存队列 Queue 改为 ConcurrentQueue
- 修复 验证码清理避免重复使用的漏洞

### 2020-11
- 调整 Netnr.ResponseFramework 项目重置数据库的方法（并删除内存数据库）
- 新增 Netnr.Fast 类库 ActionResultVM 类添加 log 集合对象，用于填充日志输出，添加片段耗时方法 PartTime

### 2020-10
- 修复 Netnr.Logging 类库 UV 统计（已第一个 IP 统计）
- 新增 Netnr.Blog.Web 项目日志添加搜索
- 修复 日志缓存队列 Queue 的 Count 属性线程不安全问题

### 2020-09
- 修复 Netnr.ResponseFramework.Web 项目 表格配置拖拽排序重置后无法使用
- 调整 Netnr.Blog.Web 项目 Gist 代码片段响应滚轮事件
- 更新 Netnr.Guff 附件托管
- 升级 Netnr.FileServer 项目 支持限时 Token 和永久 Token 授权
- 调整 Netnr.ScriptService 项目 SVG 图标合并
- 调整 Netnr.ResponseFramework.Web 项目 日志缓存为队列 Queue
- 调整 Netnr.Logging 项目 日志缓存为队列 Queue
- 整合 数据库访问为一个类库 Netnr.Data

### 2020-08
- 新增 Netnr.Fast.Extend 项目的 OSInfoTo.cs 类库新增 `ToView ()` 方法，可视化输出
- 新增 测试项目 Netnr.Test
- 整合 Netnr.Login.Sample 项目到 Netnr.Test 
- 整合 Netnr.WeChat.Sample 项目到 Netnr.Test 
- 删除 Netnr.Blog.Web 项目的实验室模块
# Change Log

### 2021-03
- 调整 Blog 项目改为宽屏
- 整合 项目

### 2021-02
- 新增 Netnr.ScriptService 项目 PowerDesigner 解析查看
- 调整 Blog、NRF 项目数据库从 SQLServer 迁移到 MySQL

### 2021-01
- 修复 Netnr.ResponseFramework 项目 Iframe 选项卡隐藏造成 EasyUI 表格显示不正常的问题（从隐藏切换改为浮动层&透明度的模式）
- 新增 Draw 支持私有设置分享码查看
- 修复 Blog 文章分类图标错误
- 调整 Blog 关闭匿名回复
- 修复 Netnr.DataKit 项目修复 Oracle 查询默认值（Long类型）丢失的问题（`InitialLONGFetchSize = -1`）

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
- 修复 Netnr.Logging 类库 UV统计（已第一个IP统计）
- 新增 Netnr.Blog.Web 项目日志添加搜索
- 修复 日志缓存队列 Queue 的 Count 属性线程不安全问题

### 2020-09
- 修复 Netnr.ResponseFramework.Web 项目 表格配置拖拽排序重置后无法使用
- 调整 Netnr.Blog.Web 项目 Gist 代码片段响应滚轮事件
- 更新 Netnr.Guff 附件托管
- 升级 Netnr.FileServer 项目 支持限时Token和永久Token授权
- 调整 Netnr.ScriptService 项目 SVG 图标合并
- 调整 Netnr.ResponseFramework.Web 项目 日志缓存为队列 Queue
- 调整 Netnr.Logging 项目 日志缓存为队列 Queue
- 整合 数据库访问为一个类库 Netnr.Data

### 2020-08
- 新增 Netnr.Fast.Extend 项目的 OSInfoTo.cs 类库新增 `ToView()` 方法，可视化输出
- 新增 测试项目 Netnr.Test
- 整合 Netnr.Login.Sample 项目到 Netnr.Test 
- 整合 Netnr.WeChat.Sample 项目到 Netnr.Test 
- 删除 Netnr.Blog.Web 项目的实验室模块
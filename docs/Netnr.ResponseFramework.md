# NRF (Netnr.ResponseFramework) 响应式框架
.NET (latest) 的响应式框架，基于 Ace Admin 框架菜单导航，Bootstrap 布局，fontAwesome 图标，内嵌 Iframe 用 EasyUI 做数据绑定，动态配置列表，动态配置表单

> ### 存档，不在更新

### [文档说明](Netnr.ResponseFramework.Document.md)

### 架构
- 前端采用 jQuery + Bootstrap + EasyUI + AceAdmin + fontAwesome
- 后端采用 .NET (latest) + EF + SQL（SQLServer、MySQL、PostgreSQL、SQLite）
    - 初始启动自动创建数据库
    - 数据库转换使用的工具：<https://fishcodelib.com/DBMigration.htm>
    - 全部采用 LINQ，跨数据库、避免 SQL 注入

### 项目结构
- Netnr.ResponseFramework.Domain 领域层（Entities Enums Models）
- Netnr.ResponseFramework.Application 应用层（Datas Services）
- Netnr.ResponseFramework.Web 站点

### 数据表
- 用户（SysUser）
- 角色、角色权限（SysRole）
- 菜单（SysMenu）
- 按钮（SysButton）
- 日志（SysLog）
- 字典（SysDictionary）
- 表配置（SysTableConfig）

### 功能
- 登录：系统账号登录
- 权限：角色权限，控制菜单及页面按钮
- 表格：动态配置标题、宽度、排序、对齐方式、格式化、冻结、点击排序等
- 表单：动态生成表单，自定义标题、排序、跨列、类型、必填等，支持多表单生成
- 查询：动态生成查询面板，自定义字段查询，以JSON格式表达查询条件
- 日志：访问日志记录
- 字典：通用的字典表
- 工具：库工具 - NDK(Netnr.DataKit)
- 导出：公共导出 Excel 表，自定义查询主体，支持条件查询、列格式化，支持追加操作等
- 上传：通用的上传接口
- 接口：所有非页面请求规范化为接口，并用 swagger 生成可视化接口文档

### 使用说明
1. 创建表（须设主键）、写字段注释（用于生成表配置）
2. 生成表配置，可以用【系统管理】-【库工具】-【快捷】-【生成代码】
3. 修改表配置，表格，表单、查询，调整为需要展示的形式（标题、宽度、排序、输入类型、列格式化、必填、默认值等，根据业务拓展配置项）
4. 修改表配置，输入类型配置，需要配置下拉框、下拉树等，在`Common`控制器写方法，`url`源指向这个方法访问的地址
5. 修改表配置，列格式化配置，比如状态需要格式化为`启用`、`停用`，有常用公共的格式化方法，也可以配置自定义格式化方法`col_custom_字段小写`
6. 基于【库工具】生成的 `Controllers` 、`Views`、`js` 添加到项目，菜单表添加此页面，配置操作按钮
7. 基于`z.js`封装的表格方法（API与EasyUI保持一致，看EasyUI文档即可），配置查询表的请求地址、表格类型、分页、复选等

### 截图

#### 列表 

![列表](https://gs.zme.ink/2018/05/18/403ce7d002.png)

#### 新增、编辑、查看

![表单](https://gs.zme.ink/2018/05/18/8d25d345b2.png)

#### 列表配置

![列表配置](https://gs.zme.ink/2018/05/18/13da6572a3.png)

#### 表单配置

![表单配置](https://gs.zme.ink/2018/05/18/0c98ee578c.png)

#### 角色权限配置（树）

![角色权限配置](https://gs.zme.ink/2018/08/16/31a55cac78.png)
# Netnr.DataKit (NDK)
数据库构建代码（Database build code）

> <https://dk.netnr.eu.org>  
> <https://netnr-dk.azurewebsites.net>

### 功能
- 支持的数据库：MySQL、SQLite、Oracle、SQLServer、PostgreSQL
- 加载、导出 表信息和列信息
- 修改表、列注释
- 查询、导出 表数据
- 根据语言模版构建代码，支持 csharp、java、php 等
- 语言模版构建基于`JS`脚本编写，并且支持调试脚本后再构建
- 支持拓展语言模版，拓展语言模版对象：`dk.build.language`，类型映射：`dk.build.typeMapping`

### 语言模版构建的进度
- [x] csharp/model  （生成C#对应的实体）
- [x] csharp/dal    （生成C#对应的数据访问方法，增删改查等）
- [ ] java  （java系列）
- [ ] php  （php系列）

> 由于本人是 `.NET` 开发人员，对 java、php 不是很熟悉，如果有兴趣的同学可以构建自己熟悉的语言模版集成进来
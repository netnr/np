# NDX (Netnr.DataX)
数据库导入导出、迁移，常用工具集成

> https://github.com/netnr/np/releases

### Data -> 数据
- Connection Test -> 连接测试
- Parameter Optimization (SQLite MySQL) -> 参数优化
- Migrate Data -> 迁移数据
- Export Data -> 导出数据
- Export Excel -> 导出Excel
- Import Data -> 导入数据
- Import Excel -> 导入Excel
- Generate Table Mapping -> 生成 表映射(读=>写)
- Generate Column Mapping -> 生成 列映射(读=>写)
- Generate Table DDL -> 生成 DDL
- Execute SQL -> 执行 SQL

### Silent -> 静默
- work (Work) -> 作业, 以 Work_ 开头
- tcping (TCP Port Probing) -> TCP端口探测
- tcpscan (TCP Port Scan) -> TCP端口扫描
- traceroute (Trace Route) -> 路由追踪
- dns (DNS Resolve) -> DNS解析
- whois (Whois) -> Whois查询
- ip (IP) -> IP查询
- icp (ICP) -> ICP查询
- sci (Server Certificate Information) -> 服务器证书信息
- dni (Domain Name Information) -> 域名信息查询（合集）
- serve (Serve) -> 启动服务 # Netnr.Serve
- ss (System Information) -> 系统信息
- sming (System Monitor) -> 系统监控
- uuid (Generate UUID) -> 生成UUID
- snow (Generate Snowflake) -> 生成雪花ID
- ocr (OCR) -> 识别图片文字, 默认Windows截图
- hash (File HASH) -> HASH计算
- base64 (Base64) -> Base64编码解码
- tec (Text encoding conversion) -> 文本编码转换
- aes (AES encryption and decryption) -> AES加密解密

### Mix -> 综合
- View version -> 查看版本
- Check for updates -> 检查更新
- Console encoding -> 控制台编码
- Open the hub directory -> 打开 hub 目录
- Clear memory -> 清理内存
- Set environment variables -> 设置环境变量

---

### Q&A
- 支持 SQLite、MySQL、Oracle、SQLServer、PostgreSQL
- 配置文件 ud/config.json，默认输出目录 ud/hub，日志输出目录 logs
- 数据库连接信息 ConnectionRemark 为连接别名，作业连接引用，所以保持唯一
- 静默作业
  - 参数配置参考 Works.Work_Demo 示例，建议保留示例新建作业
  - 不在示例的方法不支持静默执行，作业名以 Task_ 开头
  - 带参执行指定作业 `ndx work Work_Demo Work_2`，多个作业空格分隔
  - 数据包参数 PackagePath 支持时间格式化 `{yyyyMMdd_HHmmss}`，`~` 指向 ud/hub 目录
- 读写数据
  - 读取表数据 ReadDataSQL 一般配置为 `select * from table`，如果表数据大内存不够，可以配置分页读取
  - 表名可带模式名，如 dbo.UserInfo、public.UserInfo 等
  - 迁移数据可支持表列映射（可生成映射配置，并支持相似匹配）
  - 读写项配置 ListReadWrite 可以根据 GenerateTableMapping 生成
  - 读写列名映射 ReadWriteColumnMap 可以根据 GenerateColumnMapping 生成
  - 按行读取数据（内存占用不定，有时很低有时读取的表数据一直在内存中直到读取结束）
  - 默认按 10000 行分批操作，写入保留自增标识
- MySQL 参数设置（可选择 Mix > Database parameter optimization ，再选择 MySQL 连接）
  - local_infile，设置 `SET GLOBAL local_infile = ON` 允许加载本地数据
  - max_allowed_packet，设置 `SET GLOBAL max_allowed_packet = 1073741824` 传输大小
  - innodb_lock_wait_timeout，设置 `SET GLOBAL innodb_lock_wait_timeout = 600` 超时
  - 参数查询 `SHOW VARIABLES like 'local_infile'`
- Linux 关闭全球化运行 System.Globalization.Invariant
  - Couldn't find a valid ICU package installed on the system
  - 设置环境变量 `export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`
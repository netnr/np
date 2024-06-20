# NDX (Netnr.DataX)
工具箱，数据库导入、导出、迁移，静默执行（配置后可定时任务）  
Toolbox, database import, export, migration, silent execution (scheduled tasks can be configured after configuration)

https://github.com/netnr/np/releases

### Menu 菜单
```
[Data] <work> Work 作业, 以 Work 开头
[Data] Migrate Data 迁移数据
[Data] Export Data 导出数据
[Data] Import Data 导入数据
[Data] Export Excel 导出 Excel
[Data] Import Excel 导入 Excel
[Data] Generate Table Mapping 生成 表映射(读=>写)
[Data] Generate Column Mapping 生成 列映射(读=>写)
[Data] Generate Table DDL 生成 DDL
[Data] <conntest> Connection Test 连接测试
[Data] Parameter Optimization (SQLite MySQL) 参数优化
[Data] Execute SQL 执行 SQL
[Data] Full Text Search 全文检索
[Data] Generate CreateTable In ClickHouse 生成创建表

[About] Exit 退出
[About] <version> View version 查看版本
[About] Console encoding 控制台编码
[About] GC 清理
[About] <basedir> Open Base Directory 打开根目录
[About] Try Color 颜色
[About] Try Directory 路径信息
[About] Try Assembly 程序集
[About] Try Tmp 临时

[Network] <tcping> TCPing TCP 端口探测
[Network] <tcpscan> TCP Scan TCP端口扫描（1-65535）
[Network] <devicescan> Device Scan 设备扫描
[Network] <traceroute> Trace Route 路由追踪
[Network] <wol> Wake On LAN 局域网唤醒
[Network] <whois> Whois Whois查询
[Network] <dns> DNS Resolve DNS解析
[Network] <ip> IP IP查询
[Network] <icp> ICP ICP查询
[Network] <ssl> SSL 证书信息
[Network] <dni> Domain Name Information 域名信息查询（合集）
[Network] <serve> Serve 启动服务

[Tool] <sinfo> System Info 系统信息
[Tool] <sming> System Monitor 系统监控
[Tool] <hinfo> Hardware Info 硬件信息
[Tool] <pinfo> Process Info 程序信息
[Tool] <consume> Consume 消耗
[Tool] <clearmemory> Clear Memory 清理内存（仅限 Windows）
[Tool] <pipe> Pipeline 管道工具
[Tool] <env> Environment variables 环境变量
[Tool] <dotnetframework> .NET Framework 已安装的 .NET Framework
[Tool] <uuid> Generate UUID 生成UUID
[Tool] <snow> Generate Snowflake 雪花ID
[Tool] <tail> Tail 读取文件最新内容
[Tool] <wget> Wget 下载文件
[Tool] <textmining> Text Mining 文本挖掘
[Tool] <ddel> deep delete 深度删除匹配的文件（夹）
[Tool] <directorytime> Directory Time 目录时间
[Tool] <gitpull> Git Pull 批量拉取
[Tool] <aesconn> AES Conn 连接字符串加密解密
```

### Q&A
- 支持 SQLite、MySQL、Oracle、SQLServer、PostgreSQL
- 配置文件 ud/config.json，默认输出目录 ud/hub，日志输出目录 logs
- 数据库连接信息 ConnectionRemark 为连接别名，作业连接引用，所以保持唯一
- `<xxx>` 代表支持静默运行，如 `ndx version`、`ndx tcping zme.ink`
- 静默作业
  - 参数配置参考 Works.Work_Demo 示例，建议保留示例新建作业
  - 不在示例的方法不支持静默执行，作业名以 Work_ 开头
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
- MySQL 参数设置（可选择 Data > Parameter Optimization ，再选择 MySQL 连接）
  - local_infile，设置 `SET GLOBAL local_infile = ON` 允许加载本地数据
  - max_allowed_packet，设置 `SET GLOBAL max_allowed_packet = 1073741824` 传输大小
  - innodb_lock_wait_timeout，设置 `SET GLOBAL innodb_lock_wait_timeout = 600` 超时
  - 参数查询 `SHOW VARIABLES like 'local_infile'`
- SQLServer
  - SqlClient 需要 ICU 环境
- Linux 关闭全球化运行 System.Globalization.Invariant
  - Couldn't find a valid ICU package installed on the system
  - 安装 `yum install icu` （推荐）
  - 或设置环境变量 `export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`
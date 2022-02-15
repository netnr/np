## NDX (Netnr.DataX)
数据库导入、导出、迁移，静默执行（配置后可定时任务）

### Q&A
- 支持 SQLite、MySQL、Oracle、SQLServer、PostgreSQL
- 配置文件 ud/config.json，默认输出目录 ud/hub，日志输出目录 logs
- 控制台输出编码可配置，默认 Windows 为 Unicode，其它为 UTF8
- 数据库连接信息 ConnectionRemark 为连接别名，静默任务连接引用，所以保持唯一
- 静默执行任务  
  - 参数配置参考 Silence.Task_Demo 示例，建议保留示例新建任务
  - 不在示例的方法不支持静默执行，静默执行任务名以 Task_ 开头
  - 带参执行指定任务 `ndx Task_Demo Task_1`，多个任务空格分隔
  - 数据包参数 ZipPath 支持时间格式化 `{yyyyMMdd_HHmmss}`，`~` 指向 ud/hub 目录
- 读写数据
  - 读取表数据 ReadSQL 一般配置为 `select * from table`，如果表数据大内存不够，可以配置分页读取
  - 迁移数据可支持表列映射（可生成映射配置，并支持相似匹配）
  - 读写项配置 ListReadWrite 可以根据 GenerateTableMapping 生成
  - 读写列名映射 ReadWriteColumnMap 可以根据 GenerateColumnMapping 生成
  - 按行读取数据（内存占用不定，有时很低有时读取的表数据一直在内存中直到读取结束）
  - 默认按 10000 行分批操作，写入保留自增标识
- MySQL 参数设置（可选择 Ready > 数据库参数优化，再选择 MySQL 连接）
  - local_infile，设置 `SET GLOBAL local_infile = ON` 允许加载本地数据
  - max_allowed_packet，设置 `SET GLOBAL max_allowed_packet = 1073741824` 传输大小
  - innodb_lock_wait_timeout，设置 `SET GLOBAL innodb_lock_wait_timeout = 600` 超时
  - 参数查询 `SHOW VARIABLES like 'local_infile'`
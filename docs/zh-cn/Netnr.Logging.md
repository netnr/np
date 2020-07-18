# Netnr.Logging
基于 SQLite 的日志存储

### 说明
- 支持按 年、月、日 拆分日志存储（SQLite 附加数据库有限制，默认设置为 30 个）
- IP归属地查询引用 `ip2region` 组件，需将 `ip2region.db` 文件拷贝到 `logs` 根目录下
- 支持简单的分页查询、统计`PV`、`UV`，属性排行（如 url、引荐来源、浏览器 等字段列）
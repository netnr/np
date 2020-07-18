# Netnr.Logging
SQLite-based log storage

### Description
- Support split log storage by year, month, and day (SQLite additional database is limited, the default setting is 30)
- The IP attribution query refers to the `ip2region` component, and the `ip2region.db` file needs to be copied to the `logs` root directory
- Support simple paging query, statistics `PV`, `UV`, attribute ranking (such as url, referrer source, browser and other field columns)
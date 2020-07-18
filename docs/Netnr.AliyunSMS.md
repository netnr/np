# Netnr.AliyunSMS
Based on Alibaba Cloud's SMS service, regularly query the database to send SMS records, perform SMS sending, modify the status after sending successfully, and record logs

### Functional
- The configuration file supports hot update (only the scheduled task waiting time is not supported, the service needs to be restarted)
- Support database MySQL, SQLite, Oracle, SQLServer, PostgreSQL
- Support database table column name configuration
- Support custom query not sent SQL, sent successfully modified SQL
- Support to pause scheduled tasks
- Support to configure whether to output logs

### Usage
1. Configure the database connection string
2. Modify the database table column name, not sent SQL, send successfully modify SQL
3. Configure Aliyun `accessKeyId`, `secret` parameters
4. Modify the waiting time parameter of scheduled tasks `WaitSeconds`, the default is **30** seconds
5. run
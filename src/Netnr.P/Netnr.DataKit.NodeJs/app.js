const os = require('os');
const fs = require("fs");
const sql = require('mssql');
const mysql = require('mysql');
const request = require("request");
const oracledb = require('oracledb');
const pgclient = require('pg').Client;
const initSqlJs = require('./public/sql-wasm.js');

var dk = {

    //数据库类型
    typeDB: ["MySQL", "SQLite", "Oracle", "SQLServer", "PostgreSQL"],

    //接口名称
    interfaceName: ["GetTable", "GetColumn", "SetTableComment", "SetColumnComment", "GetData", "GetDEI"],

    //实体
    model: {
        dkDEI: function () {
            let obj = {};
            "DeiName,DeiVersion,DeiCompile,DeiDirInstall,DeiDirData,DeiEngine,DeiCharSet,DeiTimeZone,DeiDateTime,DeiMaxConn,DeiCurrConn,DeiTimeout,DeiIgnoreCase,DeiSystem"
                .split(',').map(k => { obj[k] = null });
            return obj;
        }
    },

    /**
     * 判断是 NULL、空字符串
     * @param {any} obj
     */
    isNullOrWhiteSpace: function (obj) {
        if (obj == null || (obj + "").trim() == "") {
            return true;
        }
        return false;
    },

    /**
     * 获取时间
     * @param {any} timezone 时区，默认中国 东8区
     */
    time: function (timezone) {
        timezone = timezone == null ? 8 : timezone;
        return new Date(new Date().valueOf() + timezone * 3600000).toISOString().replace('Z', '').replace('T', ' ');
    },

    /**
     * 去除末尾匹配字符
     * @param {any} str 字符串
     * @param {any} c 匹配字符
     */
    trimEnd: function (str, c) {
        if (!c) { return this; }
        while (true) {
            if (str.substr(str.length - c.length, c.length) != c) {
                break;
            }
            str = str.substr(0, str.length - c.length);
        }
        return str;
    },

    /**
     * 数据库连接字符串解析
     */
    connectionOptions: function () {
        var ops = {
            code: null, //错误码
            msg: null   //错误消息
        }

        try {

            var conn = dk.pars.conn;
            var tdb = this.typeDB[Number(dk.pars.tdb)];;

            if (conn && conn.length > 5) {
                conn += ';';

                var matchEngine = {
                    regexs: {
                        "Data Source": /Data Source=(.*?);/i,
                        "Server": /Server=(.*?);/i,
                        "Host": /Host=(.*?);/i,
                        "(Host)": /\(Host=(.*?)\)/i,
                        "Port": /Port=(.*?);/i,
                        "(Port)": /\(Port=(.*?)\)/i,
                        "(SERVICE_NAME)": /\(SERVICE_NAME=(.*?)\)/i,
                        "(SID)": /\(SID=(.*?)\)/i,
                        "Username": /Username=(.*?);/i,
                        "User Id": /User Id=(.*?);/i,
                        "UserId": /UserId=(.*?);/i,
                        "Uid": /Uid=(.*?);/i,
                        "Password": /Password=(.*?);/i,
                        "Pwd": /Pwd=(.*?);/i,
                        "Database": /Database=(.*?);/i
                    },
                    result: function (rgx) {
                        for (var i = 0; i < rgx.length; i++) {
                            var rr = matchEngine.regexs[rgx[i]].exec(conn);
                            if (rr) {
                                return rr[1]
                            }
                        }
                        return null;
                    }
                }

                switch (tdb) {
                    case "MySQL":
                        {
                            ops = {
                                host: matchEngine.result(["Data Source", "Server", "Host"]),
                                port: matchEngine.result(["Port"]) || 3306,
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                database: matchEngine.result(["Database"])
                            }
                        }
                        break;
                    case "SQLite":
                        {
                            var ds = dk.trimEnd(conn.substring(12), ';');
                            if (ds.toLowerCase().indexOf("http") == 0) {
                                ops = {
                                    type: "link",
                                    filename: ds
                                }
                            } else {
                                ops = {
                                    type: "file",
                                    filename: ds
                                }
                            }
                        }
                        break;
                    case "Oracle":
                        {
                            var host = matchEngine.result(["(Host)"]);
                            var port = matchEngine.result(["(Port)"]);
                            port = port ? ":" + port : "";
                            var server_name = matchEngine.result(["(SERVICE_NAME)", "(SID)"]);

                            ops = {
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                connectString: host + port + "/" + server_name,
                                _privilege: oracledb.SYSDBA
                            }
                        }
                        break;
                    case "SQLServer":
                        {
                            var serverandport = matchEngine.result(["Data Source", "Server"]).split(',');

                            ops = {
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                server: serverandport[0],
                                port: serverandport[1] || 1433,
                                database: matchEngine.result(["Database"]),
                                pool: {
                                    max: 10,
                                    min: 0,
                                    idleTimeoutMillis: 30000
                                }
                            }
                        }
                        break;
                    case "PostgreSQL":
                        {
                            ops = {
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                host: matchEngine.result(["Data Source", "Server", "Host"]),
                                port: matchEngine.result(["Port"]) || 5432,
                                database: matchEngine.result(["Database"])
                            }
                        }
                        break;
                }
            } else {
                ops.code = 1;
                ops.msg = "连接字符串似乎有问题？";
            }
        } catch (e) {
            ops.code = -1;
            ops.msg = e;
        }

        return ops;
    },

    //数据库
    db: {

        MySQL: {

            /**
             * 查询
             * @param {any} dk
             * @param {any} cmd
             */
            Query: function (dk, cmd) {

                var config = dk.connectionOptions();

                var connection = mysql.createConnection(config);

                return new Promise(function (resolve, reject) {
                    connection.query(cmd, function (error, results, fields) {
                        connection.end();
                        if (error) {
                            reject(error)
                        } else {
                            resolve(results)
                        }
                    })
                })

            },

            /**
             * 查询数据
             * @param {any} dk
             * @param {any} cmds SQL脚本数组
             */
            QueryData: function (dk, cmds) {

                var that = this;
                var plist = [];
                cmds.forEach(c => {
                    plist.push(that.Query(dk, c))
                })

                return Promise.all(plist).then(rets => {
                    return {
                        data: rets[0],
                        total: rets[1][0]["total"]
                    }
                })
            },

            /**
             * 获取所有表名及注释
             * @param {any} dk
             */
            GetTable: function (dk) {

                var cmd = `
                    SELECT
                        table_name AS TableName,
                        table_comment AS TableComment
                    FROM
                        information_schema.tables
                    WHERE
                        table_schema = '{dataBaseName}' 
                    ORDER BY table_name
                    `;
                cmd = cmd.replace("{dataBaseName}", dk.connectionOptions().database)

                return this.Query(dk, cmd);
            },

            /**
             * 获取所有列
             * @param {any} dk
             */
            GetColumn: function (dk) {

                var cmd = `
                    SELECT
                        T.table_name AS TableName,
                        T.table_comment AS TableComment,
                        C.column_name AS FieldName,
                        C.column_type AS DataTypeLength,
                        C.data_type AS DataType,
                        CASE
                        WHEN C.character_maximum_length IS NOT NULL THEN C.character_maximum_length
                        WHEN C.numeric_precision IS NOT NULL THEN C.numeric_precision
                        ELSE NULL
                        end AS DataLength,
                        C.numeric_scale AS DataScale,
                        C.ordinal_position AS FieldOrder,
                        CASE
                        WHEN (
                            SELECT
                            Count(1)
                            FROM
                            information_schema.key_column_usage
                            WHERE
                            table_schema = T.table_schema
                            AND table_name = T.table_name
                            AND column_name = C.column_name
                            LIMIT
                            0, 1
                        ) = 0 THEN ''
                        ELSE 'YES'
                        end AS PrimaryKey,
                        CASE
                        WHEN C.EXTRA = 'auto_increment' THEN 'YES'
                        ELSE ''
                        END AS AutoAdd,
                        CASE
                        WHEN C.is_nullable = 'YES' THEN ''
                        ELSE 'YES'
                        end AS NotNull,
                        C.column_default AS DefaultValue,
                        C.column_comment AS FieldComment
                    FROM
                        information_schema.columns C
                        LEFT JOIN information_schema.tables T ON C.table_schema = T.table_schema
                        AND C.table_name = T.table_name
                    WHERE
                        T.table_schema = '{dataBaseName}'
                        AND 1 = 1 {sqlWhere}
                    ORDER BY
                        T.table_name,
                        C.ordinal_position
                    `;
                cmd = cmd.replace("{dataBaseName}", dk.connectionOptions().database)

                var whereSql = "";
                var tns = dk.pars.filterTableName;
                if (!dk.isNullOrWhiteSpace(tns)) {
                    whereSql = "AND T.table_name in ('" + tns.replace("'", "").split(',').join("','") + "')";
                }
                cmd = cmd.replace("{sqlWhere}", whereSql);

                return this.Query(dk, cmd);
            },

            /**
             * 设置表注释
             * @param {any} dk
             */
            SetTableComment: function (dk) {

                var cmd = "ALTER TABLE `{dataTableName}` COMMENT '{comment}'";

                cmd = cmd.replace("{dataTableName}", dk.pars.TableName.replace("'", "")).replace("{comment}", dk.pars.TableComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 设置列注释
             * @param {any} dk
             */
            SetColumnComment: function (dk) {
                var cmd = "ALTER TABLE `{dataTableName}` MODIFY COLUMN `{dataColumnName}` INT COMMENT '{comment}'"

                cmd = cmd.replace("{dataTableName}", dk.pars.TableName.replace("'", "")).replace("{dataColumnName}", dk.pars.FieldName.replace("'", "")).replace("{comment}", dk.pars.FieldComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 查询数据
             * @param {any} dk
             */
            GetData: function (dk) {

                var listFieldName = dk.pars.listFieldName;
                if (dk.isNullOrWhiteSpace(listFieldName)) {
                    listFieldName = "*";
                }
                var whereSql = dk.pars.whereSql;
                if (dk.isNullOrWhiteSpace(whereSql)) {
                    whereSql = "";
                } else {
                    whereSql = "WHERE " + whereSql;
                }

                var TableName = dk.pars.TableName;
                var sort = dk.pars.sort;
                var order = ((dk.pars.order || "") + "").toLowerCase() == "desc" ? "desc" : "asc";
                var rows = Number(dk.pars.rows) || 30;
                var page = Number(dk.pars.page) || 1;

                var cmd = `
                    SELECT
                        ` + listFieldName + `
                    FROM
                        ` + TableName + ` ` + whereSql + `
                    ORDER BY
                        ` + sort + ` ` + order + `
                    LIMIT
                        ` + (page - 1) * rows + `,` + rows;

                var cmds = [];
                cmds.push(cmd);
                cmds.push(`select count(1) as total from ` + TableName + ` ` + whereSql);

                return this.QueryData(dk, cmds);
            },

            /**
             * 查询数据库环境信息
             * @param {any} dk
             */
            GetDEI: function (dk) {

                var mo = dk.model.dkDEI();

                var cmds = `show variables;select now();SELECT 'a'='A';show status like 'Threads_connected'`.split(';');
                var pms = [];
                for (var i = 0; i < cmds.length; i++) {
                    pms.push(this.Query(dk, cmds[i]))
                }

                return Promise.all(pms).then(results => {

                    var ts1 = results[0];
                    mo.DeiName = ts1.filter(x => x.Variable_name == "version_comment")[0].Value;
                    mo.DeiVersion = ts1.filter(x => x.Variable_name == "version")[0].Value;
                    mo.DeiCompile = ts1.filter(x => x.Variable_name == "version_compile_machine")[0].Value;
                    mo.DeiDirInstall = ts1.filter(x => x.Variable_name == "basedir")[0].Value;
                    mo.DeiDirData = ts1.filter(x => x.Variable_name == "datadir")[0].Value;
                    mo.DeiEngine = ts1.filter(x => x.Variable_name == "storage_engine" || x.Variable_name == "default_storage_engine")[0].Value;
                    mo.DeiCharSet = ts1.filter(x => x.Variable_name == "collation_server")[0].Value;
                    mo.DeiTimeZone = ts1.filter(x => x.Variable_name == "system_time_zone")[0].Value;
                    if (dk.isNullOrWhiteSpace(mo.DeiTimeZone)) {
                        mo.DeiTimeZone = ts1.filter(x => x.Variable_name == "time_zone")[0].Value;
                    }
                    mo.DeiDateTime = Object.values(results[1][0])[0];
                    mo.DeiMaxConn = parseInt(ts1.filter(x => x.Variable_name == "max_connections")[0].Value);
                    mo.DeiCurrConn = parseInt(results[3][0].Value);
                    mo.DeiTimeout = parseInt(ts1.filter(x => x.Variable_name == "wait_timeout")[0].Value);
                    mo.DeiIgnoreCase = Object.values(results[2][0]).pop() == "1";
                    mo.DeiSystem = ts1.filter(x => x.Variable_name == "version_compile_os")[0].Value;

                    return mo;
                })
            }

        },

        SQLite: {

            /**
             * 查询
             * @param {any} dk
             * @param {any} cmd
             */
            Query: function (dk, cmd) {
                return new Promise(function (resolve, reject) {
                    try {
                        var config = dk.connectionOptions();

                        var pquery = function (sdo) {
                            initSqlJs().then(function (SQL) {
                                var sqlitedb = new SQL.Database(sdo);
                                var sr = sqlitedb.exec(cmd);

                                //处理结果
                                var rows = [], columns = sr[0].columns, values = sr[0].values;
                                values.forEach(value => {
                                    var row = {};
                                    for (var i = 0; i < columns.length; i++) {
                                        row[columns[i]] = value[i]
                                    }
                                    rows.push(row);
                                });
                                resolve(rows);
                            });
                        }

                        //远程数据库文件或本地文件
                        dk.dc = dk.dc || {};
                        if (config.type == "link") {
                            var sdo = dk.dc[config.filename];
                            if (sdo != null) {
                                pquery(sdo);
                            } else {
                                request.get({
                                    url: config.filename,
                                    encoding: null
                                }, function (_error, _response, body) {
                                    dk.dc[config.filename] = body;
                                    pquery(body);
                                });
                            }
                        } else {
                            var sqlitefile = fs.readFileSync(config.filename);
                            pquery(sqlitefile);
                        }
                    } catch (err) {
                        reject(err);
                    }
                })
            },

            /**
             * 查询 多个
             * @param {any} dk
             * @param {any} cmds SQL脚本数组
             */
            Querys: function (dk, cmds) {

                var that = this;
                var plist = [];
                cmds.forEach(c => {
                    plist.push(that.Query(dk, c))
                })

                return Promise.all(plist).then(rets => {
                    return rets;
                })
            },

            /**
             * 查询 数据
             * @param {any} dk
             * @param {any} cmds SQL脚本数组
             */
            QueryData: function (dk, cmds) {

                return this.Querys(dk, cmds).then(rets => {
                    return {
                        data: rets[0],
                        total: rets[1][0]["total"]
                    }
                })
            },

            /**
             * 获取所有表名及注释
             * @param {any} dk
             */
            GetTable: function (dk) {

                var cmd = `
                    SELECT
                        tbl_name AS TableName,
                        '' AS TableComment
                    FROM
                        sqlite_master
                    WHERE
                        type = 'table'
                    ORDER BY tbl_name
                     `;

                return this.Query(dk, cmd);
            },

            /**
             * 获取所有列
             * @param {any} dk
             */
            GetColumn: function (dk) {

                var cmd = `PRAGMA table_info('@DataTableName@')`;

                var that = this;

                let gt = function () {
                    return new Promise(function (resolve, reject) {
                        var tns = dk.pars.filterTableName;
                        if (dk.isNullOrWhiteSpace(tns)) {
                            that.GetTable(dk).then(ret => {
                                resolve(ret.map(x => x.TableName));
                            }).catch(err => {
                                reject(err);
                            })
                        }
                        else {
                            tns = tns.split(',').map(x => x.replace("'", ""));
                            resolve(tns);
                        }
                    });
                }

                return gt().then(tns => {
                    var cmds = [];
                    tns.forEach(tn => {
                        cmds.push(cmd.replace("@DataTableName@", tn));
                    })

                    //自增信息
                    var aasql = "SELECT name, sql from SQLITE_MASTER WHERE 1=1";
                    if (tns.length > 0) {
                        aasql += " AND name IN('" + tns.join("','") + "')";
                    }
                    cmds.push(aasql);

                    return that.Querys(dk, cmds);
                }).then(ds => {
                    var listColumn = [];
                    var aadt = ds[ds.length - 1];

                    for (var i = 0; i < aadt.length; i++) {
                        var dt = ds[i];
                        var tableName = aadt[i].name;

                        //表创建SQL （分析该SQL语句获取自增列信息）
                        var aacreate = aadt.filter(x => x["name"] == tableName)[0]["sql"];
                        var aasi = aacreate.indexOf('(');
                        var aaei = aacreate.lastIndexOf(')');
                        aacreate = aacreate.substring(aasi, aaei - aasi);
                        //有自增
                        var hasaa = aacreate.toUpperCase().indexOf("AUTOINCREMENT") >= 0;

                        var ti = 1;
                        dt.forEach(dr => {
                            var colmo = {
                                TableName: tableName,
                                TableComment: "",
                                FieldName: dr["name"],
                                DataTypeLength: dr["type"],
                                FieldOrder: ti++,
                                PrimaryKey: dr["pk"] == "1" ? "YES" : "",
                                NotNull: dr["notnull"] == "1" ? "YES" : "",
                                DefaultValue: dr["dflt_value"],
                                FieldComment: ""
                            };

                            if (colmo.DataTypeLength.indexOf("(") >= 0) {
                                var tlarr = dk.trimEnd(colmo.DataTypeLength, ')').split('(');
                                colmo.DataType = tlarr[0];
                                colmo.DataLength = tlarr[1];
                            }
                            else {
                                colmo.DataType = colmo.DataTypeLength;
                            }

                            if (hasaa) {
                                var aais = aacreate.toUpperCase().split(',').filter(x => x.indexOf("AUTOINCREMENT") >= 0 && x.indexOf(colmo.FieldName.toUpperCase()) >= 0).length > 0;
                                colmo.AutoAdd = aais ? "YES" : "";
                            }
                            else {
                                colmo.AutoAdd = "";
                            }

                            listColumn.push(colmo);
                        })
                    }

                    return listColumn;
                })
            },

            /**
             * 设置表注释
             * @param {any} dk
             */
            SetTableComment: function (dk) {
                return new Promise(function (resolve, reject) {
                    if (dk) {
                        resolve(null);
                    } else {
                        reject(null);
                    }
                })
            },

            /**
             * 设置列注释
             * @param {any} dk
             */
            SetColumnComment: function (dk) {
                return new Promise(function (resolve, reject) {
                    if (dk) {
                        resolve(null);
                    } else {
                        reject(null);
                    }
                })
            },

            /**
             * 查询数据
             * @param {any} dk
             */
            GetData: function (dk) {

                var listFieldName = dk.pars.listFieldName;
                if (dk.isNullOrWhiteSpace(listFieldName)) {
                    listFieldName = "*";
                }

                var whereSql = dk.pars.whereSql;
                if (dk.isNullOrWhiteSpace(whereSql)) {
                    whereSql = "";
                }
                else {
                    whereSql = "WHERE " + whereSql;
                }

                var TableName = dk.pars.TableName;
                var sort = dk.pars.sort;
                var order = ((dk.pars.order || "") + "").toLowerCase() == "desc" ? "desc" : "asc";
                var rows = Number(dk.pars.rows) || 30;
                var page = Number(dk.pars.page) || 1;

                var cmd = `
                    SELECT
                        ` + listFieldName + `
                    FROM
                        ` + TableName + ` ` + whereSql + `
                    ORDER BY
                        ` + sort + ` ` + order + `
                    LIMIT
                        ` + rows + ` OFFSET ` + (page - 1) * rows;

                var cmds = [];
                cmds.push(cmd);
                cmds.push(`select count(1) as total from ` + TableName + ` ` + whereSql);

                return this.QueryData(dk, cmds);
            },

            /**
             * 查询数据库环境信息
             * @param {any} dk
             */
            GetDEI: function (dk) {

                var mo = dk.model.dkDEI();

                var cmds = `select sqlite_version();PRAGMA encoding;select datetime();select 'a'='A'`.split(';');

                return this.Querys(dk, cmds).then(results => {

                    mo.DeiVersion = Object.values(results[0][0])[0];
                    mo.DeiCompile = os.arch();
                    mo.DeiDirData = mo.DeiDirInstall;
                    mo.DeiCharSet = Object.values(results[1][0])[0];
                    mo.DeiDateTime = Object.values(results[2][0])[0];
                    mo.DeiIgnoreCase = Object.values(results[3][0])[0] == "1";
                    mo.DeiSystem = os.type();

                    return mo;
                })
            }

        },

        Oracle: {

            /**
             * 查询
             * @param {any} dk
             * @param {any} cmd SQL脚本
             */
            Query: function (dk, cmd) {

                process.env.ORA_SDTZ = 'UTC';

                var config = dk.connectionOptions();

                return oracledb.getConnection(config).then(connection => {
                    return connection.execute(cmd, {}, {
                        outFormat: oracledb.OUT_FORMAT_OBJECT
                    }).then(result => {
                        connection.close()

                        return result.rows;
                    })
                })
            },

            /**
             * 查询 数据
             * @param {any} dk
             * @param {any} cmds SQL脚本数组
             */
            QueryData: function (dk, cmds) {

                var that = this;
                var plist = [];
                cmds.forEach(c => {
                    plist.push(that.Query(dk, c))
                })

                return Promise.all(plist).then(rets => {
                    return {
                        data: rets[0],
                        total: rets[1][0]["total"]
                    }
                })
            },

            /**
             * 获取所有表名及注释
             * @param {any} dk
             */
            GetTable: function (dk) {

                var cmd = `
                    SELECT
                        A.table_name AS "TableName",
                        B.comments AS "TableComment"
                    FROM
                        user_tables A,
                        user_tab_comments B
                    WHERE
                        A.table_name = B.table_name
                    ORDER BY A.table_name
                 `;

                return this.Query(dk, cmd);
            },

            /**
             * 获取所有列
             * @param {any} dk
             */
            GetColumn: function (dk) {

                var cmd = `
                    SELECT
                        A.TABLE_NAME AS "TableName",
                        B.COMMENTS AS "TableComment",
                        C.COLUMN_NAME AS "FieldName",
                        C.DATA_TYPE || '(' || CASE
                        WHEN C.CHARACTER_SET_NAME = 'NCHAR_CS' THEN C.DATA_LENGTH / 2
                        ELSE C.DATA_LENGTH
                        END || ')' AS "DataTypeLength",
                        C.DATA_TYPE AS "DataType",
                        CASE
                        WHEN C.CHARACTER_SET_NAME = 'NCHAR_CS' THEN C.DATA_LENGTH / 2
                        WHEN C.DATA_TYPE = 'NUMBER' THEN C.DATA_PRECISION
                        ELSE C.DATA_LENGTH
                        END AS "DataLength",
                        C.DATA_SCALE AS "DataScale",
                        C.COLUMN_ID AS "FieldOrder",
                        DECODE(PK.COLUMN_NAME, C.COLUMN_NAME, 'YES', '') AS "PrimaryKey",
                        DECODE(C.NULLABLE, 'N', 'YES', '') AS "NotNull",
                        C.DATA_DEFAULT AS "DefaultValue",
                        D.COMMENTS AS "FieldComment"
                    FROM
                        USER_TABLES A
                        LEFT JOIN USER_TAB_COMMENTS B ON A.TABLE_NAME = B.TABLE_NAME
                        LEFT JOIN USER_TAB_COLUMNS C ON A.TABLE_NAME = C.TABLE_NAME
                        LEFT JOIN USER_COL_COMMENTS D ON A.TABLE_NAME = D.TABLE_NAME
                        AND C.COLUMN_NAME = D.COLUMN_NAME
                        LEFT JOIN (
                        SELECT
                            E.TABLE_NAME,
                            F.COLUMN_NAME
                        FROM
                            USER_CONSTRAINTS E
                            LEFT JOIN USER_CONS_COLUMNS F ON E.TABLE_NAME = F.TABLE_NAME
                            AND E.CONSTRAINT_NAME = F.CONSTRAINT_NAME
                        WHERE
                            E.CONSTRAINT_TYPE = 'P'
                        ) PK ON PK.TABLE_NAME = A.TABLE_NAME
                        AND C.COLUMN_NAME = PK.COLUMN_NAME
                    WHERE
                        1 = 1 {sqlWhere}
                    ORDER BY
                        A.TABLE_NAME,
                        C.COLUMN_ID
                 `;

                var whereSql = "";
                var tns = dk.pars.filterTableName;
                if (!dk.isNullOrWhiteSpace(tns)) {
                    whereSql = "AND A.TABLE_NAME in ('" + tns.replace("'", "").split(',').join("','") + "')";
                }
                cmd = cmd.replace("{sqlWhere}", whereSql);

                return this.Query(dk, cmd);
            },

            /**
             * 设置表注释
             * @param {any} dk
             */
            SetTableComment: function (dk) {

                var cmd = `comment on table "{dataTableName}" is '{comment}'`;

                cmd = cmd.replace("{dataTableName}", dk.pars.TableName.replace("'", "")).replace("{comment}", dk.pars.TableComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 设置列注释
             * @param {any} dk
             */
            SetColumnComment: function (dk) {

                var cmd = `comment on column "{dataTableName}"."{dataColumnName}" is '{comment}'`;

                cmd = cmd.replace("{dataTableName}", dk.pars.TableName.replace("'", "")).replace("{dataColumnName}", dk.pars.FieldName.replace("'", "")).replace("{comment}", dk.pars.FieldComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 查询数据
             * @param {any} dk
             */
            GetData: function (dk) {

                var listFieldName = dk.pars.listFieldName;
                if (dk.isNullOrWhiteSpace(listFieldName)) {
                    listFieldName = "t.*";
                }

                var whereSql = dk.pars.whereSql;
                var countWhere = '';
                if (dk.isNullOrWhiteSpace(whereSql)) {
                    whereSql = "";
                }
                else {
                    countWhere = "WHERE " + whereSql;
                    whereSql = "AND " + whereSql;
                }

                var TableName = dk.pars.TableName;
                var sort = dk.pars.sort;
                var order = ((dk.pars.order || "") + "").toLowerCase() == "desc" ? "desc" : "asc";
                var rows = Number(dk.pars.rows) || 30;
                var page = Number(dk.pars.page) || 1;

                var cmd = `
                    SELECT
                        *
                    FROM
                        (
                        SELECT
                            ROWNUM AS rowno,` + listFieldName + `
                        FROM
                            ` + TableName + ` t
                        WHERE
                            ROWNUM <= ` + (page * rows) + ` ` + whereSql + `
                        ORDER BY ` + sort + ` ` + order + `
                        ) table_alias
                    WHERE
                        table_alias.rowno >= ` + ((page - 1) * rows + 1);

                var cmds = [];
                cmds.push(cmd);
                cmds.push(`select count(1) as total from ` + TableName + ` ` + countWhere);

                return this.QueryData(dk, cmds);
            },

            /**
             * 查询数据库环境信息
             * @param {any} dk
             */
            GetDEI: function (dk) {

                var mo = dk.model.dkDEI();

                var cmds = `select * from product_component_version;
                            SELECT file_name
                                FROM dba_data_files
                            WHERE file_id = 1
                            UNION ALL
                            SELECT value AS CharSet
                                FROM Nls_Database_Parameters
                            WHERE PARAMETER = 'NLS_CHARACTERSET'
                            UNION ALL
                            SELECT SESSIONTIMEZONE AS TimeZone
                                FROM DUAL
                            UNION ALL
                            SELECT TO_CHAR(SYSDATE, 'yyyy-mm-dd hh24:mi:ss') AS DateTime
                                FROM DUAL
                            UNION ALL
                            SELECT TO_CHAR(value) AS MaxConn
                                FROM v$parameter
                            WHERE name = 'processes'
                            UNION ALL
                            SELECT TO_CHAR(count(1)) AS CurrConn
                                FROM v$process`.split(';');

                var pms = [];
                for (var i = 0; i < cmds.length; i++) {
                    pms.push(this.Query(dk, cmds[i]))
                }

                return Promise.all(pms).then(results => {

                    var ts1 = results[0];
                    var ts2 = results[1];

                    mo.DeiName = ts1[1].PRODUCT.trim();
                    mo.DeiVersion = ts1[1].VERSION;
                    mo.DeiCompile = ts1[1].STATUS;

                    mo.DeiDirData = Object.values(ts2[0])[0];
                    mo.DeiDirData = mo.DeiDirData.substr(0, mo.DeiDirData.replace(/\\/g, '/').lastIndexOf('/') + 1);
                    mo.DeiCharSet = Object.values(ts2[1])[0];
                    mo.DeiTimeZone = Object.values(ts2[2])[0];
                    mo.DeiDateTime = Object.values(ts2[3])[0];
                    mo.DeiMaxConn = parseInt(Object.values(ts2[4])[0]);
                    mo.DeiCurrConn = parseInt(Object.values(ts2[5])[0]);

                    mo.DeiSystem = dk.trimEnd(ts1[3].PRODUCT.trim(), ':');

                    return mo;
                })
            }

        },

        PostgreSQL: {

            /**
             * 查询
             * @param {any} dk
             * @param {any} cmd SQL脚本
             */
            Query: function (dk, cmd) {

                var config = dk.connectionOptions();
                var client = new pgclient(config);

                client.connect()

                return new Promise(function (resolve, reject) {
                    client.query(cmd, (err, res) => {
                        client.end()
                        if (err) {
                            reject(err);
                        } else {
                            resolve(res.rows);
                        }
                    })
                })
            },

            /**
             * 批量查询
             * @param {any} dk
             * @param {any} cmds SQL脚本数组
             */
            Querys: function (dk, cmds) {

                var config = dk.connectionOptions();
                var client = new pgclient(config);

                client.connect();

                var run = function (cmd) {
                    return new Promise(function (resolve, reject) {
                        client.query(cmd, (err, res) => {
                            if (err) {
                                reject(err);
                            } else {
                                resolve(res.rows);
                            }
                        })
                    })
                }

                var pms = [];
                cmds.forEach(cmd => {
                    pms.push(run(cmd))
                })

                return Promise.all(pms).then(results => {
                    client.end();
                    return results;
                });
            },

            /**
             * 查询 数据
             * @param {any} dk
             * @param {any} cmds SQL脚本数组
             */
            QueryData: function (dk, cmds) {

                var that = this;
                var plist = [];
                cmds.forEach(c => {
                    plist.push(that.Query(dk, c))
                })

                return Promise.all(plist).then(rets => {
                    return {
                        data: rets[0],
                        total: rets[1][0]["total"]
                    }
                })
            },

            /**
             * 获取所有表名及注释
             * @param {any} dk
             */
            GetTable: function (dk) {

                var cmd = `
                    SELECT
                        relname AS "TableName",
                        Cast (
                        Obj_description (relfilenode, 'pg_class') AS VARCHAR
                        ) AS "TableComment"
                    FROM
                        pg_class C
                    WHERE
                        relkind = 'r'
                        AND relname NOT LIKE 'pg_%'
                        AND relname NOT LIKE 'sql_%'
                    ORDER BY
                        relname
                 `;

                return this.Query(dk, cmd);
            },

            /**
             * 获取所有列
             * @param {any} dk
             */
            GetColumn: function (dk) {

                var cmd = `
                    SELECT
                        C.relname AS "TableName",
                        CAST(
                        obj_description(relfilenode, 'pg_class') AS VARCHAR
                        ) AS "TableComment",
                        A.attname AS "FieldName",
                        concat_ws(
                        '',
                        T.typname,
                        SUBSTRING(
                            format_type(A.atttypid, A.atttypmod)
                            FROM
                            '\(.*\)'
                        )
                        ) AS "DataTypeLength",
                        T.typname AS "DataType",
                        SUBSTRING(
                        format_type(A.atttypid, A.atttypmod)
                        FROM
                            '\d+'
                        ) AS "DataLength",
                        REPLACE(
                        SUBSTRING(
                            format_type(A.atttypid, A.atttypmod)
                            FROM
                            '\,\d+'
                        ),
                        ',',
                        ''
                        ) AS "DataScale",
                        A.attnum AS "FieldOrder",
                        CASE
                        WHEN EXISTS (
                            SELECT
                            pg_attribute.attname
                            FROM
                            pg_constraint
                            INNER JOIN pg_class ON pg_constraint.conrelid = pg_class.oid
                            INNER JOIN pg_attribute ON pg_attribute.attrelid = pg_class.oid
                            AND pg_attribute.attnum = pg_constraint.conkey [1]
                            WHERE
                            relname = C.relname
                            AND attname = A.attname
                        ) THEN 'YES'
                        ELSE ''
                        END AS "PrimaryKey",
                        CASE
                        A.attnotnull
                        WHEN 't' THEN 'YES'
                        ELSE ''
                        END AS "NotNull",
                        D.adsrc AS "DefaultValue",
                        col_description(A.attrelid, A.attnum) AS "FieldComment"
                    FROM
                        pg_class C
                        LEFT JOIN pg_attribute A ON A.attrelid = C.oid
                        LEFT JOIN pg_type T ON A.atttypid = T.oid
                        LEFT JOIN (
                        SELECT
                            T1.relname,
                            T2.attname,
                            pg_get_expr(T3.adbin,T3.adrelid) as adsrc
                        FROM
                            pg_class T1,
                            pg_attribute T2,
                            pg_attrdef T3
                        WHERE
                            T3.adrelid = T1.oid
                            AND adnum = T2.attnum
                            AND attrelid = T1.oid
                        ) D ON D.relname = C.relname
                        AND D.attname = A.attname
                    WHERE
                        C.relname IN (
                            SELECT
                            relname
                            FROM
                            pg_class
                            WHERE
                            relkind = 'r'
                            AND relname NOT LIKE 'pg_%'
                            AND relname NOT LIKE 'sql_%'
                        )
                        AND A.attnum > 0 {sqlWhere}
                    ORDER BY
                        C.relname,
                        A.attnum
                 `;

                var whereSql = "";
                var tns = dk.pars.filterTableName;
                if (!dk.isNullOrWhiteSpace(tns)) {
                    whereSql = "AND C.relname IN ('" + tns.replace("'", "").split(',').join("','") + "')";
                }
                cmd = cmd.replace("{sqlWhere}", whereSql);

                return this.Query(dk, cmd);
            },

            /**
             * 设置表注释
             * @param {any} dk
             */
            SetTableComment: function (dk) {

                var cmd = `COMMENT ON TABLE "{dataTableName}" IS '{comment}'`;

                cmd = cmd.replace("{dataTableName}", dk.pars.TableName.replace("'", "")).replace("{comment}", dk.pars.TableComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 设置列注释
             * @param {any} dk
             */
            SetColumnComment: function (dk) {

                var cmd = `COMMENT ON COLUMN "{dataTableName}"."{dataColumnName}" IS '{comment}'`;

                cmd = cmd.replace("{dataTableName}", dk.pars.TableName.replace("'", "")).replace("{dataColumnName}", dk.pars.FieldName.replace("'", "")).replace("{comment}", dk.pars.FieldComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 查询数据
             * @param {any} dk
             */
            GetData: function (dk) {
                var listFieldName = dk.pars.listFieldName;
                if (dk.isNullOrWhiteSpace(listFieldName)) {
                    listFieldName = "*";
                } else {
                    listFieldName = '"' + listFieldName.split(',').join('","') + '"';
                }

                var whereSql = dk.pars.whereSql;
                if (dk.isNullOrWhiteSpace(whereSql)) {
                    whereSql = "";
                }
                else {
                    whereSql = "WHERE " + whereSql;
                }

                var TableName = '"' + dk.pars.TableName + '"';
                var sort = '"' + dk.pars.sort + '"';
                var order = ((dk.pars.order || "") + "").toLowerCase() == "desc" ? "desc" : "asc";
                var rows = Number(dk.pars.rows) || 30;
                var page = Number(dk.pars.page) || 1;

                var cmd = `
                    SELECT
                        ` + listFieldName + `
                    FROM
                        ` + TableName + ` ` + whereSql + `
                    ORDER BY
                        ` + sort + ` ` + order + `
                    LIMIT
                        ` + rows + ` OFFSET ` + (page - 1) * rows;

                var cmds = [];
                cmds.push(cmd);
                cmds.push(`select count(1) as total from ` + TableName + ` ` + whereSql);

                return this.QueryData(dk, cmds);
            },

            /**
             * 查询数据库环境信息
             * @param {any} dk
             */
            GetDEI: function (dk) {

                var mo = dk.model.dkDEI();

                var cmds = `
                            SELECT version();
                            show all;
                            select now();
                            select count(1) from pg_stat_activity`.split(';');

                return this.Querys(dk, cmds).then(results => {

                    var ts1 = results[0][0].version.split(',');
                    var ts2 = results[1];

                    mo.DeiName = ts1[0];
                    mo.DeiVersion = ts2.filter(x => x.name == "server_version")[0].setting;
                    mo.DeiCompile = ts1[1];
                    mo.DeiDirInstall = ts2.filter(x => x.name == "archive_command")[0].setting;
                    if (mo.DeiDirInstall.includes("main")) {
                        mo.DeiDirInstall = mo.DeiDirInstall.substr(0, mo.DeiDirInstall.indexOf("main"));
                    }
                    mo.DeiDirData = mo.DeiDirInstall;
                    mo.DeiCharSet = ts2.filter(x => x.name == "server_encoding")[0].setting;
                    mo.DeiTimeZone = ts2.filter(x => x.name == "TimeZone")[0].setting;
                    mo.DeiDateTime = results[2][0].now;
                    mo.DeiMaxConn = parseInt(ts2.filter(x => x.name == "max_connections")[0].setting);
                    mo.DeiCurrConn = parseInt(results[3][0].count);
                    mo.DeiTimeout = parseInt(ts2.filter(x => x.name == "statement_timeout")[0].setting);
                    mo.DeiSystem = ts1.pop();

                    return mo;
                })
            }

        },

        SQLServer: {

            /**
             * 查询
             * @param {any} dk
             * @param {any} cmd SQL脚本
             */
            Query: function (dk, cmd) {

                var config = dk.connectionOptions();

                return sql.connect(config).then(pool => {
                    return pool.request().query(cmd).then(ret => {
                        return ret.recordset
                    })
                });

            },

            /**
             * 查询
             * @param {any} dk
             * @param {any} cmds SQL脚本
             */
            Querys: function (dk, cmds) {

                var config = dk.connectionOptions();

                return sql.connect(config).then(pool => {

                    var run = function (cmd) {
                        return pool.request().query(cmd).then(ret => {
                            return ret.recordset
                        })
                    }
                    var pms = [];
                    cmds.forEach(cmd => {
                        pms.push(run(cmd));
                    })

                    return Promise.all(pms);
                });

            },

            /**
             * 查询 数据
             * @param {any} dk
             * @param {any} cmds SQL脚本数组
             */
            QueryData: function (dk, cmds) {

                var that = this;
                var plist = [];
                cmds.forEach(c => {
                    plist.push(that.Query(dk, c))
                })

                return Promise.all(plist).then(rets => {
                    return {
                        data: rets[0],
                        total: rets[1][0]["total"]
                    }
                })
            },

            /**
             * 获取所有表名及注释
             * @param {any} dk
             */
            GetTable: function (dk) {

                var cmd = `
                    SELECT
                        a.name AS TableName,
                        b.value AS TableComment
                    FROM
                        sys.TABLES a
                        left join sys.extended_properties b ON b.major_id = a.object_id AND b.minor_id = 0
                    ORDER BY a.name
                     `;

                return this.Query(dk, cmd);
            },

            /**
             * 获取所有列
             * @param {any} dk
             */
            GetColumn: function (dk) {

                var cmd = `
                    SELECT TableName = d.name,
                            TableComment = ISNULL(f.value, ''),
                            FieldName = a.name,
                            DataTypeLength = b.name + '(' + CONVERT(VARCHAR(10), COLUMNPROPERTY(a.id, a.name, 'PRECISION')) + ')',
                            DataType = b.name,
                            [DataLength] = COLUMNPROPERTY(a.id, a.name, 'PRECISION'),
                            DataScale = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0),
                            FieldOrder = a.colorder,
                            PrimaryKey = CASE
                                            WHEN EXISTS
                                                    (
                                                        SELECT 1
                                                        FROM sysobjects
                                                        WHERE xtype = 'PK'
                                                            AND name IN
                                                                (
                                                                    SELECT name
                                                                    FROM sysindexes
                                                                    WHERE indid IN
                                                                            (
                                                                                SELECT indid FROM sysindexkeys WHERE id = a.id AND colid = a.colid
                                                                            )
                                                                )
                                                    ) THEN
                                                'YES'
                                            ELSE
                                                ''
                                        END,
                            AutoAdd = CASE
                                            WHEN i.name IS NULL THEN
                                                ''
                                            ELSE
                                                'YES'
                                        END,
                            NotNull = CASE
                                            WHEN a.isnullable = 1 THEN
                                                ''
                                            ELSE
                                                'YES'
                                        END,
                            DefaultValue = e.text,
                            FieldComment = ISNULL(g.[value], '')
                    FROM syscolumns a
                        LEFT JOIN systypes b
                            ON a.xtype = b.xusertype
                        INNER JOIN sysobjects d
                            ON a.id = d.id
                                AND d.xtype = 'U'
                                AND d.name != 'dtproperties'
                        LEFT JOIN syscomments e
                            ON a.cdefault = e.id
                        LEFT JOIN sys.extended_properties g
                            ON a.id = g.major_id
                                AND a.colid = g.minor_id
                        LEFT JOIN sys.extended_properties f
                            ON d.id = f.major_id
                                AND f.minor_id = 0
                        LEFT JOIN sys.identity_columns i
                            ON i.[object_id] = OBJECT_ID(d.name)
                                AND i.name = a.name
                    WHERE 1 = 1 {sqlWhere}
                    ORDER BY d.name,
                                a.colorder;
                 `;

                var whereSql = "";
                var tns = dk.pars.filterTableName;
                if (!dk.isNullOrWhiteSpace(tns)) {
                    whereSql = "AND d.name in ('" + tns.replace("'", "").split(',').join("','") + "')";
                }
                cmd = cmd.replace("{sqlWhere}", whereSql);

                return this.Query(dk, cmd);
            },

            /**
             * 设置表注释
             * @param {any} dk
             */
            SetTableComment: function (dk) {

                var cmd = `
                    IF NOT EXISTS
                    (
                        SELECT A.name,
                                C.value
                        FROM sys.tables A
                            INNER JOIN sys.extended_properties C
                                ON C.major_id = A.object_id
                                    AND minor_id = 0
                        WHERE A.name = N'{dataTableName}'
                    )
                        EXEC sys.sp_addextendedproperty @name = N'MS_Description',
                                                        @value = N'{comment}',
                                                        @level0type = N'SCHEMA',
                                                        @level0name = N'dbo',
                                                        @level1type = N'TABLE',
                                                        @level1name = N'{dataTableName}';

                    EXEC sp_updateextendedproperty @name = N'MS_Description',
                                                    @value = N'{comment}',
                                                    @level0type = N'SCHEMA',
                                                    @level0name = N'dbo',
                                                    @level1type = N'TABLE',
                                                    @level1name = N'{dataTableName}'`;

                cmd = cmd.replace(/{dataTableName}/g, dk.pars.TableName.replace("'", "")).replace(/{comment}/g, dk.pars.TableComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 设置列注释
             * @param {any} dk
             */
            SetColumnComment: function (dk) {

                var cmd = `
                    IF NOT EXISTS
                    (
                        SELECT C.value AS column_description
                        FROM sys.tables A
                            INNER JOIN sys.columns B
                                ON B.object_id = A.object_id
                            INNER JOIN sys.extended_properties C
                                ON C.major_id = B.object_id
                                    AND C.minor_id = B.column_id
                        WHERE A.name = N'{dataTableName}'
                                AND B.name = N'{dataColumnName}'
                    )
                        EXEC sys.sp_addextendedproperty @name = N'MS_Description',
                                                        @value = N'{comment}',
                                                        @level0type = N'SCHEMA',
                                                        @level0name = N'dbo',
                                                        @level1type = N'TABLE',
                                                        @level1name = N'{dataTableName}',
                                                        @level2type = N'COLUMN',
                                                        @level2name = N'{dataColumnName}';

                    EXEC sp_updateextendedproperty @name = N'MS_Description',
                                                    @value = N'{comment}',
                                                    @level0type = N'SCHEMA',
                                                    @level0name = N'dbo',
                                                    @level1type = N'TABLE',
                                                    @level1name = N'{dataTableName}',
                                                    @level2type = N'COLUMN',
                                                    @level2name = N'{dataColumnName}'`;

                cmd = cmd.replace(/{dataTableName}/g, dk.pars.TableName.replace("'", "")).replace(/{dataColumnName}/g, dk.pars.FieldName.replace("'", "")).replace(/{comment}/g, dk.pars.FieldComment.replace("'", "''"));

                return this.Query(dk, cmd);
            },

            /**
             * 查询数据
             * @param {any} dk
             */
            GetData: function (dk) {

                var listFieldName = dk.pars.listFieldName;
                if (dk.isNullOrWhiteSpace(listFieldName)) {
                    listFieldName = "*";
                }

                var whereSql = dk.pars.whereSql;
                if (dk.isNullOrWhiteSpace(whereSql)) {
                    whereSql = "";
                }
                else {
                    whereSql = "WHERE " + whereSql;
                }

                var TableName = dk.pars.TableName;
                var sort = dk.pars.sort;
                var order = ((dk.pars.order || "") + "").toLowerCase() == "desc" ? "desc" : "asc";
                var rows = Number(dk.pars.rows) || 30;
                var page = Number(dk.pars.page) || 1;

                var cmd = `
                    select
                        *
                    from(
                        select
                            row_number() over(
                            order by
                                ` + sort + ` ` + order + `
                            ) as NumId,` + listFieldName + `
                        from
                            ` + TableName + ` ` + whereSql + `
                        ) as t
                    where
                        NumId between ` + ((page - 1) * rows + 1) + ` and ` + (page * rows);

                var cmds = [];
                cmds.push(cmd);
                cmds.push(`select count(1) as total from ` + TableName + ` ` + whereSql);

                return this.QueryData(dk, cmds);
            },

            /**
             * 查询数据库环境信息
             * @param {any} dk
             */
            GetDEI: function (dk) {

                var mo = dk.model.dkDEI();

                var cmds = `
                        SELECT SUBSTRING(@@VERSION , 1, CHARINDEX(')', @@VERSION, 1))+ ' ' + cast(SERVERPROPERTY('Edition') as varchar) AS DeiName ,
	                        SERVERPROPERTY('ProductVersion') AS DeiVersion,	
	                        SERVERPROPERTY('InstanceDefaultDataPath') AS DeiDirData,
	                        SERVERPROPERTY('Collation') AS DeiCharSet,
	                        GETDATE() as DeiDateTime,
	                        @@MAX_CONNECTIONS AS DeiMaxConn,
	                        DeiCurrConn=(SELECT COUNT(dbid) from sys.sysprocesses),
                            DeiIgnoreCase =(CASE WHEN 'a' = 'A' THEN 1 ELSE 0 END),
                            REPLACE(SUBSTRING(@@VERSION , CHARINDEX(' on ', @@VERSION, 0)+ 4, LEN(@@VERSION)-CHARINDEX(' on ', @@VERSION, 1)), CHAR(10), '') AS DeiSystem
                        ;EXEC sp_configure 'remote query timeout'
                        ;EXEC master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE',N'SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation',N'TimeZoneKeyName'`.split(';');

                return this.Querys(dk, cmds).then(results => {

                    var obj1 = results[0][0];
                    for (var i in mo) {
                        if (i in obj1) {
                            mo[i] = obj1[i];
                        }
                    }
                    mo.DeiTimeout = results[1][0].config_value;
                    mo.DeiTimeZone = results[2][0].Data;

                    return mo;
                })

            }

        }
    },

    /**
     * 初始化
     * @param {any} req
     * @param {any} res
     */
    init: function (req, res) {

        //返回对象
        var vm = {
            code: 0,
            msg: null,
            data: null,
            useTime: null
        };
        var st = Date.now();

        try {
            console.log(oracledb.SYSDBA);
            dk.pars = req.method == "POST" ? req.body : req.query;
            var tdb = dk.typeDB[Number(dk.pars.tdb)];

            //方法名
            var iname = req.url.split('?')[0].split('/')[2];

            dk.db[tdb][iname](dk).then(ret => {

                vm.code = 200;
                vm.msg = "success";
                vm.data = ret;
                vm.useTime = Date.now() - st;

                //输出结果
                res.json(vm);
            })
        } catch (e) {
            console.log(e);

            vm.code = -1;
            vm.msg = e
            vm.useTime = Date.now() - st;

            //输出结果
            res.json(vm);
        }
    }
}

const path = require("path");
const express = require('express');
const app = express();

app.use(express.urlencoded({ extended: false }));
app.use(express.static(path.join(__dirname, 'public')))

app.all('/swagger', function (_req, res) {
    res.sendFile(path.join(__dirname, 'public/swagger.html'));
});

app.all('/dk*', function (req, res) {
    dk.init(req, res);
});

app.listen(process.env.PORT || '654', () => {
    console.log('http://localhost:' + (process.env.PORT || '654'));
})
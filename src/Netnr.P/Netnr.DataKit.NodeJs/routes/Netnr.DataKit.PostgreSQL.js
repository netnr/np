const { Client } = require('pg');

module.exports = {

    /**
     * 查询
     * @param {any} dk
     * @param {any} cmd SQL脚本
     */
    Query: function (dk, cmd) {

        var config = dk.connectionOptions();

        var client = new Client(config);

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
                            T3.adsrc
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
                        AND A.attnum > 0 @SqlWhere@
                    ORDER BY
                        C.relname,
                        A.attnum
                 `;

        var whereSql = "";
        var tns = dk.pars.filterTableName;
        if (!dk.isNullOrWhiteSpace(tns)) {
            whereSql = "AND C.relname IN ('" + tns.replace("'", "").split(',').join("','") + "')";
        }
        cmd = cmd.replace("@SqlWhere@", whereSql);

        return this.Query(dk, cmd);
    },

    /**
     * 设置表注释
     * @param {any} dk
     */
    SetTableComment: function (dk) {

        var cmd = `COMMENT ON TABLE "@DataTableName@" IS '@Comment@'`;

        cmd = cmd.replace("@DataTableName@", dk.pars.TableName.replace("'", "")).replace("@Comment@", dk.pars.TableComment.replace("'", "''"));

        return this.Query(dk, cmd);
    },

    /**
     * 设置列注释
     * @param {any} dk
     */
    SetColumnComment: function (dk) {

        var cmd = `COMMENT ON COLUMN "@DataTableName@"."@DataColumnName@" IS '@Comment@'`;

        cmd = cmd.replace("@DataTableName@", dk.pars.TableName.replace("'", "")).replace("@DataColumnName@", dk.pars.FieldName.replace("'", "")).replace("@Comment@", dk.pars.FieldComment.replace("'", "''"));

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

        var TableName = '"' + dk.pars.TableName + '"';
        var sort = '"' + dk.pars.sort + '"';
        var order = ((dk.pars.order || "") + "").toLowerCase() == "desc" ? "desc" : "asc";
        var rows = Number(dk.pars.rows) || 30;
        var page = Number(dk.pars.page) || 1;

        var cmd = `
                    SELECT
                        ` + listFieldName + `
                    FROM
                        ` + TableName + `
                    ORDER BY
                        ` + sort + ` ` + order + `
                    LIMIT
                        ` + rows + ` OFFSET ` + (page - 1) * rows;

        var cmds = [];
        cmds.push(cmd);
        cmds.push(`select count(1) as total from ` + TableName);

        return this.QueryData(dk, cmds);
    }
};
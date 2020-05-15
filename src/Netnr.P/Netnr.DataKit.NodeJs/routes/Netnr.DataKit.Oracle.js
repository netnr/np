process.env.ORA_SDTZ = 'UTC';
const oracledb = require('oracledb');

module.exports = {

    /**
     * 查询
     * @param {any} dk
     * @param {any} cmd SQL脚本
     */
    Query: function (dk, cmd) {

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
            console.log(rets[1]);
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
                        LEFT JOIN ALL_TAB_COLUMNS C ON A.TABLE_NAME = C.TABLE_NAME
                        LEFT JOIN USER_COL_COMMENTS D ON A.TABLE_NAME = D.TABLE_NAME
                        AND C.COLUMN_NAME = D.COLUMN_NAME
                        LEFT JOIN (
                        SELECT
                            E.TABLE_NAME,
                            F.COLUMN_NAME
                        FROM
                            ALL_CONSTRAINTS E
                            LEFT JOIN USER_CONS_COLUMNS F ON E.TABLE_NAME = F.TABLE_NAME
                            AND E.CONSTRAINT_NAME = F.CONSTRAINT_NAME
                        WHERE
                            E.CONSTRAINT_TYPE = 'P'
                        ) PK ON PK.TABLE_NAME = A.TABLE_NAME
                        AND C.COLUMN_NAME = PK.COLUMN_NAME
                    WHERE
                        1 = 1 @SqlWhere@ 
                    ORDER BY
                        A.TABLE_NAME,
                        C.COLUMN_ID
                 `;

        var whereSql = "";
        var tns = dk.pars.filterTableName;
        if (!dk.isNullOrWhiteSpace(tns)) {
            whereSql = "AND A.TABLE_NAME in ('" + tns.replace("'", "").split(',').join("','") + "')";
        }
        cmd = cmd.replace("@SqlWhere@", whereSql);

        return this.Query(dk, cmd);
    },

    /**
     * 设置表注释
     * @param {any} dk
     */
    SetTableComment: function (dk) {

        var cmd = `comment on table "@DataTableName@" is '@Comment@'`;

        cmd = cmd.replace("@DataTableName@", dk.pars.TableName.replace("'", "")).replace("@Comment@", dk.pars.TableComment.replace("'", "''"));

        return this.Query(dk, cmd);
    },

    /**
     * 设置列注释
     * @param {any} dk
     */
    SetColumnComment: function (dk) {

        var cmd = `comment on column "@DataTableName@"."@DataColumnName@" is '@Comment@'`;

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
            listFieldName = "t.*";
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
                            ROWNUM <= ` + (page * rows) + `
                        ORDER BY ` + sort + ` ` + order + `
                        ) table_alias
                    WHERE
                        table_alias.rowno >= ` + ((page - 1) * rows + 1);

        var cmds = [];
        cmds.push(cmd);
        cmds.push(`select count(1) as total from ` + TableName);

        return this.QueryData(dk, cmds);
    }
}
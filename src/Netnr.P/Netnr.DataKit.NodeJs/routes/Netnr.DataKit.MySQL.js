var mysql = require('mysql');

module.exports = {

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
                        table_schema = '@DataBaseName@' 
                    ORDER BY table_name
                 `;
        cmd = cmd.replace("@DataBaseName@", dk.connectionOptions().database)

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
                        T.table_schema = '@DataBaseName@'
                        AND 1 = 1 @SqlWhere@
                    ORDER BY
                        T.table_name,
                        C.ordinal_position
                 `;
        cmd = cmd.replace("@DataBaseName@", dk.connectionOptions().database)

        var whereSql = "";
        var tns = dk.pars.filterTableName;
        if (!dk.isNullOrWhiteSpace(tns)) {
            whereSql = "AND T.table_name in ('" + tns.replace("'", "").split(',').join("','") + "')";
        }
        cmd = cmd.replace("@SqlWhere@", whereSql);

        return this.Query(dk, cmd);
    },

    /**
     * 设置表注释
     * @param {any} dk
     */
    SetTableComment: function (dk) {

        var cmd = "ALTER TABLE `@DataTableName@` COMMENT '@Comment@'";

        cmd = cmd.replace("@DataTableName@", dk.pars.TableName.replace("'", "")).replace("@Comment@", dk.pars.TableComment.replace("'", "''"));

        return this.Query(dk, cmd);
    },

    /**
     * 设置列注释
     * @param {any} dk
     */
    SetColumnComment: function (dk) {
        var cmd = "ALTER TABLE `@DataTableName@` MODIFY COLUMN `@DataColumnName@` INT COMMENT '@Comment@'"

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
    }
}
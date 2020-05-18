const sql = require('mssql')

module.exports = {

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
                    WHERE 1 = 1 @SqlWhere@
                    ORDER BY d.name,
                                a.colorder;
                 `;

        var whereSql = "";
        var tns = dk.pars.filterTableName;
        if (!dk.isNullOrWhiteSpace(tns)) {
            whereSql = "AND d.name in ('" + tns.replace("'", "").split(',').join("','") + "')";
        }
        cmd = cmd.replace("@SqlWhere@", whereSql);

        return this.Query(dk, cmd);
    },

    /**
     * 设置表注释
     * @param {any} dk
     */
    SetTableComment: function (dk) {

        var cmd = `EXECUTE sp_updateextendedproperty 'MS_Description',N'@Comment@','user','dbo','table','@DataTableName@',NULL,NULL`;

        cmd = cmd.replace("@DataTableName@", dk.pars.TableName.replace("'", "")).replace("@Comment@", dk.pars.TableComment.replace("'", "''"));

        return this.Query(dk, cmd);
    },

    /**
     * 设置列注释
     * @param {any} dk
     */
    SetColumnComment: function (dk) {

        var cmd = `EXECUTE sp_updateextendedproperty 'MS_Description',N'@Comment@','user','dbo','table',N'@DataTableName@','column',N'@DataColumnName@'`;

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
                            ` + TableName + `
                        ) as t
                    where
                        NumId between ` + ((page - 1) * rows + 1) + ` and ` + (page * rows);

        var cmds = [];
        cmds.push(cmd);
        cmds.push(`select count(1) as total from ` + TableName);

        return this.QueryData(dk, cmds);
    }
}
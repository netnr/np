var sqlite3 = require('sqlite3').verbose();

module.exports = {

    /**
     * 查询
     * @param {any} dk
     * @param {any} cmd
     */
    Query: function (dk, cmd) {

        return dk.connectionOptions().then(config => {
            console.log(config.filename)
            var db = new sqlite3.Database(config.filename);

            return new Promise(function (resolve, reject) {
                db.serialize(function () {
                    let rows = [];
                    db.each(cmd, function (err, row) {
                        if (err) {
                            reject(err);
                        } else {
                            rows.push(row);
                        }
                    }, function () {
                        resolve(rows);
                    });
                });

                db.close();
            })
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
    }
}
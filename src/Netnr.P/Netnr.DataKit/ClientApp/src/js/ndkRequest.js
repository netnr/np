import { ndkFunction } from './ndkFunction';
import { ndkVary } from './ndkVary';
import { ndkStorage } from './ndkStorage';
import { ndkI18n } from './ndkI18n';

// 发起请求
var ndkRequest = {

    /**
     * 状态设置
     * @param {*} isShow 
     */
    statusSet: (isShow) => {
        if (isShow == null) {
            isShow = ndkRequest.statusInfo.queueList.length > 0;
        }
        if (ndkVary.domRequestStatus.loading = isShow) {
            ndkVary.domRequestStatus.type = "danger";
            ndkRequest.statusInfo.run()
        } else {
            ndkVary.domRequestStatus.type = "default";
            ndkRequest.statusInfo.stop()
        }

        ndkVary.gridOpsQueue.api.setRowData(ndkRequest.statusInfo.queueList);
    },

    statusInfo: {
        title: document.title,
        queueList: [], //队列
        //加入队列
        queueAdd: reqObj => {
            ndkRequest.statusInfo.queueList.unshift(reqObj);
            ndkRequest.statusSet();
        },
        //移除队列
        queueRemove: reqObj => {
            for (var i = ndkRequest.statusInfo.queueList.length - 1; i >= 0; i--) {
                if (ndkRequest.statusInfo.queueList[i].id == reqObj.id) {
                    ndkRequest.statusInfo.queueList.splice(i, 1);
                }
            }
            ndkRequest.statusSet();
        },
        //终止
        queueCancel: reqObj => {
            reqObj.abortCtrl.abort();
            ndkRequest.statusInfo.queueRemove(reqObj);
        },
        si: null,
        run: (index = 1, dir = true) => {
            var arr = [], icon = ndkVary.emoji.loading;
            for (var i = 0; i < index; i++) {
                arr.push(icon);
            }
            document.title = " " + arr.join(" ");
            dir ? index++ : index--;
            dir = index == 5 ? false : dir;
            dir = index == 1 ? true : dir;

            document.querySelector('sl-icon-button').classList.add('nrc-rotate-1');

            clearTimeout(ndkRequest.statusInfo.si);
            ndkRequest.statusInfo.si = setTimeout(() => {
                ndkRequest.statusInfo.run(index, dir);
            }, 600);
        },
        stop: () => {
            clearTimeout(ndkRequest.statusInfo.si);
            document.title = ndkRequest.statusInfo.title;
            document.querySelector('sl-icon-button').classList.remove('nrc-rotate-1');
        }
    },

    /**
     * 参数拼接
     * @param {*} pars 
     * @returns 
     */
    parameterJoin: (pars) => {
        var arr = [];
        for (var i in pars) {
            arr.push(`${i}=${encodeURIComponent(pars[i])}`);
        }
        return arr.join('&');
    },

    /**
     * 请求
     * @param {*} url 
     * @param {*} options 
     * @returns 
     */
    request: (url, options) => new Promise((resolve, reject) => {

        //请求对象
        let reqObj = {
            id: ndkFunction.UUID(),
            date: ndkFunction.formatDateTime(),
            url: url,
            abortCtrl: new AbortController()
        }
        ndkRequest.statusInfo.queueAdd(reqObj);

        let setHost = [];
        fetch(url, Object.assign({ method: "GET", signal: reqObj.abortCtrl.signal }, options)).then(resp => {
            if (resp.status == 404) {
                setHost = [ndkI18n.lg.menu, ndkI18n.lg.setting, ndkI18n.lg.setServerTitle, ndkI18n.lg.setServerPlaceholder].join(' > ') + ' ? <br/>';
            } else {
                return resp.json();
            }
        }).then(res => {
            console.debug(res);
            ndkRequest.statusInfo.queueRemove(reqObj);
            if (res.code == 200) {
                resolve(res);
            } else {
                var sql;
                if (options && options.body) {
                    sql = options.body.get("sql");
                }
                if (sql) {
                    ndkFunction.alert(`${res.msg}<sl-divider></sl-divider><code style="word-break:break-all">${sql.substring(0, 999)}</code>`);
                    reject(res);
                } else {
                    ndkFunction.output(res.msg);
                    reject(res);
                }
            }
        }).catch(err => {
            ndkFunction.alert(`${setHost}${err}<sl-divider></sl-divider><code style="word-break:break-all">${url}</code>`);
            ndkRequest.statusInfo.queueRemove(reqObj);
            reject(err);
        });
    }),

    /**
     * 请求连接
     */
    reqConns: () => new Promise((resolve) => {
        ndkStorage.connsGet().then(conns => {
            if (conns.length == 0) {
                conns = ndkVary.resConnDemo;
            }
            conns.sort((a, b) => a.order - b.order);

            resolve(conns);
        });
    }),

    /**
     * 请求库名
     * @param {*} cobj
     * @param {*} forcedReload
     * @returns 
     */
    reqDatabaseName: (cobj, forcedReload = false) => new Promise((resolve, reject) => {
        new Promise(fr => {
            if (forcedReload) {
                fr()
            } else {
                ndkStorage.cc([cobj.id]).then(res => {
                    if (res != null) {
                        fr(res.data)
                    } else {
                        fr()
                    }
                }).catch(() => fr())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var pars = ndkRequest.parameterJoin({ tdb: cobj.type, conn: cobj.conn });
                ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiGetDatabaseName}?${pars}`).then(res => {
                    var dbrows = [];
                    res.data.forEach(name => {
                        dbrows.push({ DatabaseName: name })
                    })
                    ndkStorage.cc([cobj.id], dbrows) //缓存

                    resolve(dbrows);
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 请求库信息
     * @param {*} cobj
     * @param {*} filterDatabaseName
     * @param {*} forcedReload
     * @returns 
     */
    reqDatabaseInfo: (cobj, filterDatabaseName, forcedReload = false) => new Promise((resolve, reject) => {
        var fdn = filterDatabaseName == "" ? [] : filterDatabaseName.split(',');
        new Promise(fr => {
            if (forcedReload) {
                fr()
            } else {
                ndkStorage.cc([cobj.id]).then(res => {
                    if (res != null) {
                        var hasdi = true;
                        res.data.forEach(row => {
                            if (fdn.length == 0) {
                                if (!Object.hasOwnProperty.call(row, "DatabaseCharset")) {
                                    hasdi = false;
                                }
                            } else {
                                if (fdn.includes(row.DatabaseName) && !Object.hasOwnProperty.call(row, "DatabaseCharset")) {
                                    hasdi = false;
                                }
                            }
                        })
                        if (hasdi) {
                            fr(res.data)
                        } else {
                            fr()
                        }
                    } else {
                        fr()
                    }
                }).catch(() => fr())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var pars = ndkRequest.parameterJoin({ tdb: cobj.type, conn: cobj.conn, filterDatabaseName: filterDatabaseName });
                ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiGetDatabaseInfo}?${pars}`).then(res => {
                    ndkStorage.cc([cobj.id]).then(cdb => {
                        cdb.data.forEach(row => {
                            var newrow = res.data.filter(x => x.DatabaseName == row.DatabaseName);
                            if (newrow.length > 0) {
                                Object.assign(row, newrow[0]);
                            }
                        })
                        ndkStorage.cc([cobj.id], cdb.data) //缓存

                        resolve(cdb.data);
                    })
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 请求表
     * @param {*} cobj 
     * @param {*} databaseName
     * @param {*} forcedReload
     * @returns 
     */
    reqTable: (cobj, databaseName, forcedReload = false) => new Promise((resolve, reject) => {
        new Promise(fr => {
            if (forcedReload) {
                fr()
            } else {
                ndkStorage.cc([cobj.id, databaseName]).then(res => {
                    if (res != null) {
                        fr(res.data)
                    } else {
                        fr()
                    }
                }).catch(() => fr())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var pars = ndkRequest.parameterJoin({ tdb: cobj.type, conn: cobj.conn, databaseName: databaseName });
                ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiGetTable}?${pars}`).then(res => {
                    ndkStorage.cc([cobj.id, databaseName], res.data) //缓存
                    resolve(res.data);
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 请求列
     * @param {*} cobj 
     * @param {*} databaseName
     * @param {*} filterSNTN
     * @param {*} forcedReload
     * @returns 
     */
    reqColumn: (cobj, databaseName, filterSNTN, forcedReload = false) => new Promise((resolve, reject) => {
        new Promise(resolve2 => {
            if (forcedReload) {
                resolve2()
            } else {
                ndkStorage.cc([cobj.id, databaseName, '*']).then(cout => {
                    if (cout != null) {
                        var sntns = cout.data.map(x => `${x.SchemaName}.${x.TableName}`),
                            fsntns = [];
                        if (filterSNTN != null && filterSNTN != "") {
                            fsntns = filterSNTN.split(',');
                        } else {
                            var arows = agg.getAllRows(ndkVary.gridOpsTable);
                            fsntns = arows.map(x => `${x.SchemaName}.${x.TableName}`);
                        }

                        //本地缓存不够
                        if (fsntns.filter(x => !sntns.includes(x)).length) {
                            resolve2();
                        } else {
                            var fdata = cout.data.filter(x => fsntns.includes(`${x.SchemaName}.${x.TableName}`));
                            resolve2(fdata)
                        }
                    } else {
                        resolve2()
                    }
                }).catch(() => resolve2())
            }
        }).then(data => {
            if (data) {
                resolve(data)
            } else {
                var fd = new FormData();
                fd.append('tdb', cobj.type);
                fd.append('conn', cobj.conn);
                fd.append('filterSchemaNameTableName', filterSNTN);
                fd.append('databaseName', databaseName);

                ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiGetColumn}`, {
                    method: "POST",
                    body: fd
                }).then(res => {
                    //缓存
                    ndkStorage.cc([cobj.id, databaseName, '*']).then(cout => {
                        if (cout != null) {
                            //移除旧表再拼接再排序
                            var tns = res.data.map(x => x.TableName);
                            var fdata = cout.data.filter(x => !tns.includes(x.TableName));
                            fdata = fdata.concat(res.data).sort((a, b) => a.TableName.localeCompare(b.TableName))
                            ndkStorage.cc([cobj.id, databaseName, '*'], fdata)
                        } else {
                            ndkStorage.cc([cobj.id, databaseName, '*'], res.data)
                        }
                    })
                    resolve(res.data);
                }).catch(err => reject(err));
            }
        })
    }),

    /**
     * 请求表DDL
     * @param {*} cobj 
     * @param {*} databaseName
     * @param {*} schemaName
     * @param {*} tableName
     * @returns 
     */
    reqTableDDL: (cobj, databaseName, schemaName, tableName) => {
        var pars = ndkRequest.parameterJoin({
            tdb: cobj.type,
            conn: cobj.conn,
            tableName: tableName,
            schemaName: schemaName,
            databaseName: databaseName
        });
        return ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiGetTableDDL}?${pars}`);
    },

    /**
     * 设置表注释
     * @param {*} cobj 
     * @param {*} tableName 
     * @param {*} tableComment 
     * @param {*} schemaName 
     * @param {*} databaseName 
     * @returns 
     */
    setTableComment: (cobj, tableName, tableComment, schemaName, databaseName) => new Promise((resolve, reject) => {
        if (cobj.type != "SQLite") {
            var fd = new FormData();
            fd.append('tdb', cobj.type);
            fd.append('conn', cobj.conn);
            fd.append('tableName', tableName);
            fd.append('tableComment', tableComment);
            fd.append('schemaName', schemaName);
            fd.append('databaseName', databaseName);

            ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiSetTableComment}`, {
                method: "POST",
                body: fd
            }).then(res => resolve(res)).catch(err => reject(err));
        } else {
            reject()
        }
    }),

    /**
     * 设置列注释
     * @param {*} cobj 
     * @param {*} tableName 
     * @param {*} columnName 
     * @param {*} columnComment 
     * @param {*} schemaName 
     * @param {*} databaseName 
     * @returns 
     */
    setColumnComment: (cobj, tableName, columnName, columnComment, schemaName, databaseName) => new Promise((resolve, reject) => {
        if (cobj.type != "SQLite") {
            var fd = new FormData();
            fd.append('tdb', cobj.type);
            fd.append('conn', cobj.conn);
            fd.append('tableName', tableName);
            fd.append('columnName', columnName);
            fd.append('columnComment', columnComment);
            fd.append('schemaName', schemaName);
            fd.append('databaseName', databaseName);

            ndkRequest.request(`${ndkVary.apiServer}${ndkVary.apiSetColumnComment}`, {
                method: "POST",
                body: fd
            }).then(res => resolve(res)).catch(err => reject(err));
        } else {
            reject()
        }
    }),
}

export { ndkRequest }
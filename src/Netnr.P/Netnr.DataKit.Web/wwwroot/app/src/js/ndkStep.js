import { ndkVary } from './ndkVary';
import { ndkLs } from './ndkLs';
import { ndkFn } from './ndkFn';
import { ndkDb } from './ndkDb';

var ndkStep = {
    //连接记录
    "cp-1": {
        cobj: {},
        databaseName: null,
        tableName: null
    },
    /**
     * 所有key
     */
    cpKeys: ['1'],
    /**
     * 获取
     * @param {*} key 
     * @returns 
     */
    cpGet: key => ndkStep[`cp-${key}`],
    /**
     * 设置
     * @param {*} key 
     * @param {*} cobj 
     * @param {*} databaseName
     * @param {*} tableName
     * @returns 
     */
    cpSet: (key, cobj, databaseName, tableName) => {
        if (!ndkStep.cpKeys.includes(key)) {
            ndkStep.cpKeys.push(key)
        }
        ndkStep[`cp-${key}`] = { cobj, databaseName, tableName }
        ndkStep.stepSave();
    },
    /**
     * 删除
     * @param {*} key 
     */
    cpRemove: key => {
        var ki = ndkStep.cpKeys.indexOf(key);
        if (ki >= 0) {
            ndkStep.cpKeys.splice(ki, 1);
        }
        delete ndkStep[`cp-${key}`];
        ndkStep.stepSave();
    },
    /**
     * 显示
     * @param {any} key
     * @param {any} cobj
     * @param {any} databaseName
     */
    cpInfo: (key) => {
        if (key == "reset") {
            ndkVary.domWindowConns.style.display = "none";
            ndkVary.domWindowDatabase.style.display = "none";
            ndkVary.domWindowConns.innerHTML = '';
            ndkVary.domWindowDatabase.innerHTML = '';
        } else {
            var cp = ndkStep.cpGet(key);
            if (cp != null) {
                var cobj = cp.cobj;
                if (key == 1) {
                    var viewItem = `${ndkVary.iconDB(cobj.type)} ${cobj.type} ${ndkVary.icons.connConn} ${cobj.alias} ${ndkVary.iconEnv(cobj.env)} ${cobj.env} ${cp.databaseName == null ? "" : ndkVary.icons.connDatabase + " " + cp.databaseName}\n${ndkVary.icons.connConn} ${cobj.conn}`;

                    ['database', 'table', 'column'].forEach(tp => {
                        ndkVary.domTabGroup1.querySelector(`[panel="tp1-${tp}"]`).title = viewItem;
                    });
                } else {
                    ndkVary.domWindowConns.style.display = "block";
                    ndkVary.domWindowDatabase.style.display = "block";

                    ndkVary.domWindowConns.innerHTML = `<sl-menu-item value="${cobj.id}">${ndkVary.iconDB(cobj.type) + cobj.alias + ndkVary.iconEnv(cobj.env)}</sl-menu-item>`;
                    ndkVary.domWindowConns.value = cobj.id + "";
                    if (cp.databaseName) {
                        ndkVary.domWindowDatabase.innerHTML = `<sl-menu-item>${ndkVary.icons.connDatabase + cp.databaseName}</sl-menu-item>`;
                    } else {
                        ndkVary.domWindowDatabase.innerHTML = `<sl-menu-item>（none）</sl-menu-item>`;
                    }
                }
            }
        }
    },

    stepVarIng: 0,//状态（1：恢复中）
    stepVarStart: Date.now(),//恢复开始时间
    stepVarEnd: Date.now(),//恢复结束时间
    stepVarIndex: 0, //恢复索引
    stepVarDefer: null, //恢复延迟任务
    /**
     * 步骤状态
     * @param {any} ing
     */
    stepStatus: ing => {
        switch (ing) {
            //开始
            case 1:
                ndkStep.stepVarStart = Date.now();
                ndkStep.stepVarIng = ing;
                ndkStep.stepVarIndex = 0;
                break;
            //完成
            case 2:
                ndkStep.stepVarEnd = Date.now();
                ndkStep.stepVarIng = ing;
                console.warn(`step done ${ndkStep.stepVarEnd - ndkStep.stepVarStart}`);
                clearTimeout(ndkStep.stepVarDefer);
                break;
            //超时
            default:
                ndkStep.stepVarEnd = Date.now();
                ndkStep.stepVarIng = -1;
                console.warn("step timeout");
                break;
        }
    },
    /**
     * 保存步骤
     */
    stepSave: () => {
        //非恢复中
        if (ndkStep.stepVarIng != 1) {

            var sobj = {
                theme: ndkVary.theme,
                box1Size: ndkFn.cssvar(ndkVary.domMain, '--box1-width'),
                connCache: {},
                tabGroup1Show: "tp-conns",
                viewDatabase: ndkVary.gridOpsDatabase != null,
                viewTable: ndkVary.gridOpsTable != null,
                viewColumn: ndkVary.gridOpsColumn != null,
            };

            //显示选项卡
            ndkVary.domTabGroup1.querySelectorAll('sl-tab-panel').forEach(node => {
                if (node.style.display == "block") {
                    sobj.tabGroup1Show = node.name
                }
            });

            //连接缓存
            //ndkStep.cpKeys.forEach(k => sobj.connCache[k] = ndkStep.cpGet(k));
            sobj.connCache['1'] = ndkStep.cpGet(1) || {};

            if (sobj.loadTable) {
                sobj.selectedTable = ndkVary.gridOpsTable.api.getSelectedRows().map(x => x.TableName);
            }
            if (sobj.loadColumn) {
                sobj.selectedColumn = ndkVary.gridOpsColumn.api.getSelectedRows().map(x => x.TableName + ":" + x.ColumnName);
            }

            ndkLs.stepsSet(sobj)
        }
    },
    /**
     * 步骤恢复开始
     */
    stepStart: () => new Promise(resolve => {
        if (ndkStep.stepVarIng == 1) {
            ndkFn.msg("正在进行中...");
        } else {
            ndkStep.stepStatus(1);

            ndkStep.stepVarDefer = setTimeout(() => {
                ndkStep.stepStatus(-1);
                resolve();
            }, 1000 * 30);

            ndkLs.stepsGet().then(sobj => ndkStep.stepItemRun([
                "step-theme",
                "step-box1-size",
                "step-conn-cache",
                "step-view-conns",
                "step-tab-group1-show",
            ], sobj).then(() => ndkStep.stepItemRun([
                "step-view-database",
                "step-view-table",
                "step-view-column"
            ], sobj).then(() => {
                ndkStep.stepStatus(2);
                resolve()
            })));
        }
    }),
    /**
     * 步骤项运行
     * @param {any} items
     * @param {any} sobj
     */
    stepItemRun: (items, sobj) => new Promise(resolve => {
        var ics = [];
        items.forEach(item => ics.push(ndkStep.stepItemCmd(item, sobj)));
        Promise.all(ics).then(() => resolve()).catch(() => resolve())
    }),
    /**
     * 步骤项目命令（无需写入）
     * @param {*} stepItem
     * @param {*} sobj 
     * @returns 
     */
    stepItemCmd: (stepItem, sobj) => new Promise(resolve => {
        sobj = sobj || {}
        switch (stepItem) {
            //主题
            case "step-theme":
                ndkFn.actionRun(sobj.theme == "dark" ? "theme-dark" : "theme-light");
                resolve();
                break;
            //分离器大小
            case "step-box1-size":
                if (sobj.box1Size && sobj.box1Size != "") {
                    ndkFn.cssvar(ndkVary.domMain, '--box1-width', sobj.box1Size);
                }
                resolve();
                break;
            //连接-缓存
            case "step-conn-cache":
                for (var k in sobj.connCache) {
                    var cp = sobj.connCache[k];
                    ndkStep.cpSet(k, cp.cobj, cp.databaseName, cp.tableName)
                }
                resolve();
                break;
            //载入连接
            case "step-view-conns":
                ndkDb.reqConns().then(conns => {
                    ndkDb.viewConns(conns).then(() => {
                        ndkStep.stepItemCmd('step-tab-group1-show', sobj)
                        resolve();
                    })
                })
                break;
            //载入库
            case "step-view-database":
                if (sobj.viewDatabase) {
                    var cp = ndkStep.cpGet(1);
                    ndkDb.reqDatabaseName(cp.cobj).then(databases => {
                        ndkDb.viewDatabase(databases).then(() => {
                            ndkStep.stepItemCmd('step-tab-group1-show', sobj)
                            ndkStep.cpInfo(1);
                            resolve();
                        })
                    })
                } else {
                    resolve();
                }
                break;
            //载入表
            case "step-view-table":
                var cp = ndkStep.cpGet(1);
                if (sobj.viewTable && cp.databaseName != null) {
                    ndkDb.reqTable(cp.cobj, cp.databaseName).then(tables => {
                        ndkDb.viewTable(tables, cp.cobj).then(() => {
                            ndkStep.stepItemCmd('step-tab-group1-show', sobj)
                            ndkStep.cpInfo(1);
                            resolve();
                        })
                    })
                } else {
                    resolve();
                }
                break;
            //载入列
            case "step-view-column":
                var cp = ndkStep.cpGet(1);
                if (sobj.viewColumn && cp.databaseName != null && cp.tableName != null) {
                    ndkDb.reqColumn(cp.cobj, cp.databaseName, cp.tableName).then(columns => {
                        ndkDb.viewColumn(columns, cp.cobj).then(() => {
                            ndkStep.stepItemCmd('step-tab-group1-show', sobj)
                            ndkStep.cpInfo(1);
                            resolve();
                        })
                    })
                } else {
                    resolve();
                }
                break;
            //显示选项卡
            case "step-tab-group1-show":
                if (sobj.tabGroup1Show && sobj.tabGroup1Show != "") {
                    ndkVary.domTabGroup1.show(sobj.tabGroup1Show);
                }
                resolve();
                break;
            default:
                break;
        }
    })
}

export { ndkStep }
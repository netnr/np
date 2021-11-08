import { vary } from './vary';
import { ls } from './ls';
import { fn } from './fn';
import { db } from './db';

var step = {
    //连接记录
    "cp-1": {
        cobj: null,
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
    cpGet: key => step[`cp-${key}`],
    /**
     * 设置
     * @param {*} key 
     * @param {*} cobj 
     * @param {*} databaseName
     * @param {*} tableName
     * @returns 
     */
    cpSet: (key, cobj, databaseName, tableName) => {
        if (!step.cpKeys.includes(key)) {
            step.cpKeys.push(key)
        }
        step[`cp-${key}`] = { cobj, databaseName, tableName }
        step.stepSave();
    },
    /**
     * 删除
     * @param {*} key 
     */
    cpRemove: key => {
        var ki = step.cpKeys.indexOf(key);
        if (ki >= 0) {
            step.cpKeys.splice(ki, 1);
        }
        delete step[`cp-${key}`];
        step.stepSave();
    },
    /**
     * 显示
     * @param {any} key
     * @param {any} cobj
     * @param {any} databaseName
     */
    cpInfo: (key) => {
        if (key == "reset") {
            vary.domWindowConns.style.display = "none";
            vary.domWindowDatabase.style.display = "none";
            vary.domWindowConns.innerHTML = '';
            vary.domWindowDatabase.innerHTML = '';
        } else {
            var cp = step.cpGet(key);
            if (cp != null) {
                var cobj = cp.cobj;
                if (key == 1) {
                    var viewItem = `${vary.iconDB(cobj.type)} ${cobj.type} ${vary.icons.connConn} ${cobj.alias} ${vary.iconEnv(cobj.env)} ${cobj.env} ${cp.databaseName == null ? "" : vary.icons.connDatabase + " " + cp.databaseName}\n${vary.icons.connConn} ${cobj.conn}`;

                    ['database', 'table', 'column'].forEach(tp => {
                        vary.domTabGroup1.querySelector(`[panel="tp1-${tp}"]`).title = viewItem;
                    });
                } else {
                    vary.domWindowConns.style.display = "block";
                    vary.domWindowDatabase.style.display = "block";

                    vary.domWindowConns.innerHTML = `<sl-menu-item value="${cobj.id}">${vary.iconDB(cobj.type) + cobj.alias + vary.iconEnv(cobj.env)}</sl-menu-item>`;
                    vary.domWindowConns.value = cobj.id + "";
                    if (cp.databaseName) {
                        vary.domWindowDatabase.innerHTML = `<sl-menu-item>${vary.icons.connDatabase + cp.databaseName}</sl-menu-item>`;
                    } else {
                        vary.domWindowDatabase.innerHTML = `<sl-menu-item>（none）</sl-menu-item>`;
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
                step.stepVarStart = Date.now();
                step.stepVarIng = ing;
                step.stepVarIndex = 0;
                break;
            //完成
            case 2:
                step.stepVarEnd = Date.now();
                step.stepVarIng = ing;
                console.warn(`step done ${step.stepVarEnd - step.stepVarStart}`);
                clearTimeout(step.stepVarDefer);
                break;
            //超时
            default:
                step.stepVarEnd = Date.now();
                step.stepVarIng = -1;
                console.warn("step timeout");
                break;
        }
    },
    /**
     * 保存步骤
     */
    stepSave: () => {
        //非恢复中
        if (step.stepVarIng != 1) {

            var sobj = {
                theme: vary.theme,
                box1Size: fn.cssvar(vary.domMain, '--box1-width'),
                connCache: {},
                tabGroup1Show: "tp-conns",
                viewDatabase: vary.gridOpsDatabase != null,
                viewTable: vary.gridOpsTable != null,
                viewColumn: vary.gridOpsColumn != null,
            };

            //显示选项卡
            vary.domTabGroup1.querySelectorAll('sl-tab-panel').forEach(node => {
                if (node.style.display == "block") {
                    sobj.tabGroup1Show = node.name
                }
            });

            //连接缓存
            //step.cpKeys.forEach(k => sobj.connCache[k] = step.cpGet(k));
            sobj.connCache['1'] = step.cpGet(1);

            if (sobj.loadTable) {
                sobj.selectedTable = vary.gridOpsTable.api.getSelectedRows().map(x => x.TableName);
            }
            if (sobj.loadColumn) {
                sobj.selectedColumn = vary.gridOpsColumn.api.getSelectedRows().map(x => x.TableName + ":" + x.ColumnName);
            }

            ls.stepsSet(sobj)
        }
    },
    /**
     * 步骤恢复开始
     */
    stepStart: () => new Promise(resolve => {
        if (step.stepVarIng == 1) {
            fn.msg("正在进行中...");
        } else {
            step.stepStatus(1);

            step.stepVarDefer = setTimeout(() => {
                step.stepStatus(-1);
                resolve();
            }, 1000 * 30);

            ls.stepsGet().then(sobj => step.stepItemRun([
                "step-theme",
                "step-box1-size",
                "step-conn-cache",
                "step-view-conns",
                "step-tab-group1-show",
            ], sobj).then(() => step.stepItemRun([
                "step-view-database",
                "step-view-table",
                "step-view-column"
            ], sobj).then(() => {
                step.stepStatus(2);
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
        items.forEach(item => ics.push(step.stepItemCmd(item, sobj)));
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
                fn.actionRun(sobj.theme == "dark" ? "theme-dark" : "theme-light");
                resolve();
                break;
            //分离器大小
            case "step-box1-size":
                if (sobj.box1Size && sobj.box1Size != "") {
                    fn.cssvar(vary.domMain, '--box1-width', sobj.box1Size);
                }
                resolve();
                break;
            //连接-缓存
            case "step-conn-cache":
                for (var k in sobj.connCache) {
                    var cp = sobj.connCache[k];
                    step.cpSet(k, cp.cobj, cp.databaseName, cp.tableName)
                }
                resolve();
                break;
            //载入连接
            case "step-view-conns":
                db.reqConns().then(conns => {
                    db.viewConns(conns).then(() => {
                        step.stepItemCmd('step-tab-group1-show', sobj)
                        resolve();
                    })
                })
                break;
            //载入库
            case "step-view-database":
                if (sobj.viewDatabase) {
                    var cp = step.cpGet(1);
                    db.reqDatabaseName(cp.cobj).then(databases => {
                        db.viewDatabase(databases).then(() => {
                            step.stepItemCmd('step-tab-group1-show', sobj)
                            step.cpInfo(1);
                            resolve();
                        })
                    })
                } else {
                    resolve();
                }
                break;
            //载入表
            case "step-view-table":
                var cp = step.cpGet(1);
                if (sobj.viewTable && cp.databaseName != null) {
                    db.reqTable(cp.cobj, cp.databaseName).then(tables => {
                        db.viewTable(tables, cp.cobj).then(() => {
                            step.stepItemCmd('step-tab-group1-show', sobj)
                            step.cpInfo(1);
                            resolve();
                        })
                    })
                } else {
                    resolve();
                }
                break;
            //载入列
            case "step-view-column":
                var cp = step.cpGet(1);
                if (sobj.viewColumn && cp.databaseName != null && cp.tableName != null) {
                    db.reqColumn(cp.cobj, cp.databaseName, cp.tableName).then(columns => {
                        db.viewColumn(columns, cp.cobj).then(() => {
                            step.stepItemCmd('step-tab-group1-show', sobj)
                            step.cpInfo(1);
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
                    vary.domTabGroup1.show(sobj.tabGroup1Show);
                }
                resolve();
                break;
            default:
                break;
        }
    })
}

export { step }
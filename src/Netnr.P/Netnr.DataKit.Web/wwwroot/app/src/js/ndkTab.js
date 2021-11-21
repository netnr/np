import { ndkFn } from './ndkFn';
import { ndkEditor } from './ndkEditor';
import { ndkVary } from './ndkVary';
import { ndkStep } from './ndkStep';
import { ndkDb } from './ndkDb';

var ndkTab = {
    tabKeys: {},

    /**
     * 新选项卡
     * @param {any} key
     * @param {any} title
     * @param {any} type
     */
    tabOpen: (key, title, type) => new Promise(resolve => {

        key = `tp-${key}`;
        if (!(key in ndkTab.tabKeys)) {

            var pbox = '';
            switch (type) {
                default:
                    pbox = `
<div class="nr-spliter2 nrc-spliter-vertical">
    <!--SQL-->
    <div class="nr-editor-box nrc-spliter-item">
        <div class="nr-editor-tool" panel="${key}">
            <sl-tooltip content="执行选中或全部脚本（Ctrl + R）" placement="right">
              <sl-icon-button data-cmd="sql-execute-selected" panel="${key}" name="caret-right-fill" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="格式化脚本（Alt + Shift + F）" placement="right">
              <sl-icon-button data-cmd="sql-formatting" panel="${key}" name="brush-fill" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
            <sl-tooltip content="SQL 笔记" placement="right">
              <sl-icon-button data-cmd="sql-note" panel="${key}" name="journal-code" style="font-size: 1.5rem;"></sl-icon-button>
            </sl-tooltip>
        </div>
        <div class="nr-editor-sql" panel="${key}"></div>
    </div>

    <div class="nrc-spliter-bar"></div>
    <div class="nrc-spliter-bar-active"></div>

    <!--Result-->
    <div class="nrc-spliter-item">
        <sl-tab-group class="nr-tab-group-3">
        </sl-tab-group>
    </div>
</div>
                    `;
            }

            var tabbox = document.createElement("div");
            tabbox.innerHTML = `
<sl-tab slot="nav" panel="${key}" closable>${title}</sl-tab>
<sl-tab-panel name="${key}">${pbox}</sl-tab-panel>
            `;
            var domTab = tabbox.querySelector('sl-tab'),
                domTabPanel = tabbox.querySelector('sl-tab-panel'),
                domEditorTool = tabbox.querySelector('.nr-editor-tool'),
                domEditorSql = tabbox.querySelector('.nr-editor-sql'),
                domTabGroup3 = tabbox.querySelector('.nr-tab-group-3');

            //调整大小
            tabbox.querySelector('.nrc-spliter-bar').addEventListener('mousedown', function (edown) {
                var sitem1 = edown.target.previousElementSibling,
                    activebar = edown.target.nextElementSibling,
                    spliter = edown.target.parentElement,

                    sh = sitem1.clientHeight,
                    fnmove = function (emove) {
                        var ch = sh - (edown.clientY - emove.clientY);
                        ch = Math.max(0, ch);
                        ch = Math.min(spliter.clientHeight - activebar.clientHeight, ch);

                        activebar.style.display = 'block';
                        activebar.style.top = `${ch}px`;
                    }, fnup = function () {
                        activebar.style.display = 'none';

                        var psize = activebar.style.top;
                        if (!psize.includes("%")) {
                            psize = (parseInt(activebar.style.top || sitem1.clientHeight) / spliter.clientHeight * 100).toFixed(4) + '%'; //占比
                        }
                        activebar.style.top = psize;
                        sitem1.style.height = psize;

                        activebar.nextElementSibling.style.height = `calc(100% - var(--nrc-spliter-width) - ${psize})`;
                        ndkFn.size();

                        window.removeEventListener('mousemove', fnmove)
                        window.removeEventListener('mouseup', fnup)

                        this.releaseCapture && this.releaseCapture();
                    };

                window.addEventListener('mousemove', fnmove, false);
                window.addEventListener('mouseup', fnup, false);

                this.setCapture && this.setCapture();
            }, false);
            tabbox.querySelector('.nrc-spliter-bar').addEventListener('dblclick', function (event) {
                var psize = (event.clientX - event.target.getBoundingClientRect().left) / event.target.clientWidth,
                    sitem1 = event.target.previousElementSibling,
                    activebar = event.target.nextElementSibling;
                if (psize < 0.33333) {
                    sitem1.style.height = '85%';
                    activebar.style.top = '85%';
                    activebar.nextElementSibling.style.height = `calc(100% - var(--nrc-spliter-width) - 85%)`;
                } else if (psize > 0.33333 && psize < 0.66666) {
                    sitem1.style.height = '50%';
                    activebar.style.top = '50%';
                    activebar.nextElementSibling.style.height = `calc(100% - var(--nrc-spliter-width) - 50%)`;
                } else {
                    sitem1.style.height = '15%';
                    activebar.style.top = '15%';
                    activebar.nextElementSibling.style.height = `calc(100% - var(--nrc-spliter-width) - 15%)`;
                }
                ndkFn.size();
            })

            var slpanels = ndkVary.domTabGroup2.querySelectorAll("sl-tab-panel");
            ndkVary.domTabGroup2.insertBefore(domTab, slpanels[0]);
            ndkVary.domTabGroup2.appendChild(domTabPanel);
            ndkTab.tabNavFix();

            tabbox.remove();

            //存储
            ndkTab.tabKeys[key] = {
                tpkey: key,
                type,
                domTab,
                domTabPanel,
                domEditorTool,
                domEditorSql,
                domTabGroup3
            };

            //构建编辑器
            ndkEditor.create(domEditorSql).then(editor => {
                ndkTab.tabKeys[key].editor = editor;//编辑器对象

                //Ctrl + R 执行选中或全部脚本
                editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyR, function () {
                    ndkTab.tabEditorExecuteSql(key)
                })
            });
        }

        setTimeout(() => {
            ndkVary.domTabGroup2.show(key);
            resolve(key);
        }, 10)
    }),

    /**
     * 执行SQL
     * @param {any} tpkey
     */
    tabEditorExecuteSql: (tpkey) => {
        var tpobj = ndkTab.tabKeys[tpkey];

        var sql = ndkEditor.selectedOrAllValue(tpobj.editor);
        if (sql.trim() == "") {
            ndkFn.msg("执行 SQL 不能为空");
        } else {
            var tpcp = ndkStep.cpGet(tpkey)
            ndkDb.reqExecuteSql(tpcp.cobj, tpcp.databaseName, sql).then(esdata => {
                ndkDb.viewExecuteSql(esdata, tpkey)
            })
        }
    },

    /**
     * 导航修复
     */
    tabNavFix: () => {
        document.body.style.zoom = document.body.style.zoom == "1" ? 1.00001 : 1;
    }
}

export { ndkTab }
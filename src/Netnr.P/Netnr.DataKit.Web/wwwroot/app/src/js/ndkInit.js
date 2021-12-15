import { agg } from './agg';
import { ndkI18n } from './ndkI18n';
import { ndkVary } from './ndkVary'
import { ndkFn } from './ndkFn'
import { ndkTab } from './ndkTab'
import { ndkStep } from './ndkStep'
import { ndkLs } from './ndkLs';
import { ndkEditor } from './ndkEditor';

var ndkInit = {

    //事件初始化
    event: function () {

        //菜单项事件
        document.body.addEventListener('click', function (e) {
            var cmd = e.target.getAttribute("data-cmd")
            if (cmd != null) {
                ndkFn.actionRun(cmd, e.target);
            }
        }, false);

        //选项卡1连接-调整大小
        ndkVary.domSpliter1.querySelector('.nrc-spliter-bar').addEventListener('mousedown', function (edown) {
            var sitem1 = edown.target.previousElementSibling,
                activebar = edown.target.nextElementSibling,
                spliter = edown.target.parentElement,

                sw = sitem1.clientWidth,
                fnmove = function (emove) {
                    var cw = sw - (edown.clientX - emove.clientX);
                    cw = Math.max(0, cw);
                    cw = Math.min(spliter.clientWidth - activebar.clientWidth, cw);

                    activebar.style.display = 'block';
                    activebar.style.left = `${cw}px`;
                }, fnup = function () {
                    activebar.style.display = 'none';

                    var psize = activebar.style.left;
                    if (!psize.includes("%")) {
                        psize = (parseInt(activebar.style.left || sitem1.clientWidth) / spliter.clientWidth * 100).toFixed(4) + '%'; //占比
                    }
                    activebar.style.left = psize;
                    ndkFn.actionRun('box1-size', psize)

                    window.removeEventListener('mousemove', fnmove)
                    window.removeEventListener('mouseup', fnup)

                    this.releaseCapture && this.releaseCapture();
                };

            window.addEventListener('mousemove', fnmove, false);
            window.addEventListener('mouseup', fnup, false);

            this.setCapture && this.setCapture();
        }, false);
        ndkVary.domSpliter1.querySelector('.nrc-spliter-bar').addEventListener('dblclick', function (event) {
            var psize = (event.clientY - event.target.getBoundingClientRect().top) / event.target.clientHeight,
                activebar = event.target.nextElementSibling;
            if (psize < 0.33333) {
                activebar.style.left = '85%';
                ndkFn.actionRun('box1-size', '85%')
            } else if (psize > 0.33333 && psize < 0.66666) {
                activebar.style.left = '50%';
                ndkFn.actionRun('box1-size', '50%')
            } else {
                activebar.style.left = '15%';
                ndkFn.actionRun('box1-size', '15%')
            }
        })
        //选项卡1连接-连接、库、表、列 面板切换调整大小
        ndkVary.domTabGroup1.addEventListener('sl-tab-show', function () {
            new Promise(resolve => resolve()).then(() => {
                ndkFn.size()
                ndkStep.stepSave()
            })
        }, false);
        //搜索-连接、库、表、列
        ['conns', 'database', 'table', 'column'].forEach(vkey => {
            vkey = ndkFn.fu(vkey);
            ndkVary[`domFilter${vkey}`].addEventListener('input', function (event) {
                clearTimeout(event.target.si);
                event.target.si = setTimeout(() => {
                    var gridOps = ndkVary[`gridOps${vkey}`];
                    if (gridOps && gridOps.api) {
                        gridOps.api.setQuickFilter(this.value);
                        ndkStep.stepSave();
                    }
                }, 200)
            })
        })

        //选项卡2窗口-关闭
        ndkVary.domTabGroup2.addEventListener('sl-close', async event => {
            var sltab = event.target;
            var panel = ndkVary.domTabGroup2.querySelector(`sl-tab-panel[name="${sltab.panel}"]`);

            if (sltab.active) {
                var otab = sltab.previousElementSibling || sltab.nextElementSibling;
                if (otab.nodeName != "sl-tab-panel".toUpperCase()) {
                    ndkVary.domTabGroup2.show(otab.panel);
                } else {
                    ndkStep.cpInfo("reset"); //重置连接信息
                }
            }

            sltab.remove();
            panel.remove();
            ndkTab.tabNavFix();

            //删除key
            delete ndkTab.tabKeys[sltab.panel];
            ndkStep.cpRemove(sltab.panel)
        });
        //选项卡2窗口-显示
        ndkVary.domTabGroup2.addEventListener('sl-tab-show', function (event) {
            if (event.target.classList.contains("nr-tab-group-2")) {
                ndkStep.cpInfo(event.detail.name); //显示连接                
            }
            new Promise(resolve => resolve()).then(() => ndkFn.size());
        }, false);

        //全局错误日志
        window.addEventListener('error', function (event) {
            if (event.error) {
                var eobj = { message: event.error.message, stack: event.error.stack };
                ndkLs.errorsAdd(eobj); //记录错误日志
                ndkFn.notify("错误提醒", { body: eobj.message, icon: "favicon.ico", tag: "error" })
            }
        });

        //浏览器变更主题
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
            if (ndkVary.theme == "auto") {
                let theme = event.matches ? "dark" : "light";
                console.debug(`Change the theme to ${theme}`);
                ndkFn.actionRun("theme-auto");
            }
        });

        //调整窗口大小
        window.addEventListener('resize', () => ndkFn.size(), false);
    },

    dom: () => {
        var main = document.createElement('div');
        main.className = "nr-main";
        main.style.visibility = "hidden";
        main.innerHTML = `
        <!--顶部-->
        <div class="nr-box0">
            <!--菜单-->
            <sl-dropdown class="nr-menu mb-2">
                <sl-button slot="trigger" size="small" caret class="nr-request-status">${ndkVary.icons.menu}${ndkI18n.lg.menu}</sl-button>
                <sl-menu>
                    <sl-menu-item data-cmd="setting-manager">${ndkI18n.lg.setting}</sl-menu-item>
                    <sl-divider></sl-divider>
                    <sl-menu-item data-cmd="theme" data-val="light">${ndkI18n.lg.themeLight}</sl-menu-item>
                    <sl-menu-item data-cmd="theme" data-val="dark">${ndkI18n.lg.themeDark}</sl-menu-item>
                    <sl-menu-item data-cmd="theme" data-val="auto">${ndkI18n.lg.themeAuto}</sl-menu-item>
                    <sl-divider></sl-divider>
                    <sl-menu-item data-cmd="language" data-val="zh-CN">${ndkI18n.lg.languageZHCN}</sl-menu-item>
                    <sl-menu-item data-cmd="language" data-val="en-US">${ndkI18n.lg.languageENUS}</sl-menu-item>
                    <sl-menu-item data-cmd="language" data-val="auto">${ndkI18n.lg.languageAuto}</sl-menu-item>
                </sl-menu>
            </sl-dropdown>

            <!--快捷01-->
            <sl-dropdown class="nr-quick01 mb-2">
                <sl-button slot="trigger" size="small" caret>${ndkVary.icons.quick}${ndkI18n.lg.quick}</sl-button>
                <sl-menu>
                    <sl-menu-item data-cmd="quick-note">${ndkVary.icons.comment}${ndkI18n.lg.note}</sl-menu-item>
                    <sl-divider></sl-divider>
                    <sl-menu-item data-cmd="quick-note">${ndkVary.icons.comment}${ndkI18n.lg.note}</sl-menu-item>
                </sl-menu>
            </sl-dropdown>

            <!--设置弹窗-->
            <sl-dialog label="${ndkVary.icons.cog}${ndkI18n.lg.setTitle}" class="nr-dialog-setting dialog-width" style="--width: 70vw;">                
                <sl-details summary="${ndkVary.icons.server}${ndkI18n.lg.setServerTitle}" open>
                    <sl-input class="nr-text-api-server mb-2" placeholder="${ndkI18n.lg.setServerPlaceholder}"></sl-input>
                    <sl-button type="text" data-cmd="set-api-server" data-val="current" >${ndkI18n.lg.current}</sl-button>
                    <sl-button type="text" data-cmd="set-api-server" data-val="https://localhost:5001" >https://localhost:5001</sl-button>
                </sl-details>
                <sl-details summary="${ndkVary.icons.parameter}${ndkI18n.lg.setParameterConfigTitle}" open>
                    <div class="nr-parameter-config"></div>
                </sl-details>
                <sl-details summary="${ndkVary.icons.io}${ndkI18n.lg.setExportTitle}" open>
                    <sl-button type="default" class="mb-2" data-cmd="config-export-file">${ndkVary.icons.save}${ndkI18n.lg.setExportSave}</sl-button>
                    <sl-button type="default" class="mb-2" data-cmd="config-export-clipboard">${ndkVary.icons.clipboard}${ndkI18n.lg.setExportSaveClipboard}</sl-button>
                    <sl-divider></sl-divider>
                    <sl-textarea class="mb-2" placeholder="${ndkI18n.lg.setImportPlaceholder}"></sl-textarea>
                    <sl-button type="default" data-cmd="config-import">${ndkVary.icons.full}${ndkI18n.lg.setImportButton}</sl-button>
                </sl-details>
            </sl-dialog>

            <!--当前选项卡连接信息-->
            <sl-dropdown class="nr-tabconns mb-2"></sl-dropdown>
            <sl-dropdown class="nr-tabdatabase mb-2"></sl-dropdown>
        </div>

        <!--主体分离器-->
        <div class="nr-spliter1 nrc-spliter-horizontal">

            <!--左-->
            <div class="nr-box1 nrc-spliter-item">
                <!--选项卡-->
                <sl-tab-group class="nr-tab-group-1">
                    <sl-tab slot="nav" panel="tp1-conns">${ndkVary.icons.connConn}${ndkI18n.lg.tab1Conns}</sl-tab>
                    <sl-tab slot="nav" panel="tp1-database">${ndkVary.icons.connDatabase}${ndkI18n.lg.tab1Database}</sl-tab>
                    <sl-tab slot="nav" panel="tp1-table">${ndkVary.icons.connTable}${ndkI18n.lg.tab1Table}</sl-tab>
                    <sl-tab slot="nav" panel="tp1-column">${ndkVary.icons.connColumn}${ndkI18n.lg.tab1Column}</sl-tab>

                    <sl-tab-panel name="tp1-conns">
                        <sl-input class="nr-filter-conns mb-2" placeholder="${ndkI18n.lg.search}" size="small"></sl-input>
                        <div class="nr-grid-conns"></div>
                    </sl-tab-panel>
                    <sl-tab-panel name="tp1-database">
                        <sl-input class="nr-filter-database mb-2" placeholder="${ndkI18n.lg.search}" size="small"></sl-input>
                        <div class="nr-grid-database"></div>
                    </sl-tab-panel>
                    <sl-tab-panel name="tp1-table">
                        <sl-input class="nr-filter-table mb-2" placeholder="${ndkI18n.lg.search}" size="small"></sl-input>
                        <div class="nr-grid-table"></div>
                    </sl-tab-panel>
                    <sl-tab-panel name="tp1-column">
                        <sl-input class="nr-filter-column mb-2" placeholder="${ndkI18n.lg.search}" size="small"></sl-input>
                        <div class="nr-grid-column"></div>
                    </sl-tab-panel>
                </sl-tab-group>
            </div>
            <div class="nrc-spliter-bar"></div>
            <div class="nrc-spliter-bar-active"></div>
            <!--右-->
            <div class="nr-box2 nrc-spliter-item">
                <sl-tab-group class="nr-tab-group-2">
                </sl-tab-group>
            </div>
        </div>
        `;

        document.body.appendChild(main);
    }
}

window.addEventListener("DOMContentLoaded", function () {
    agg.lk();

    // 初始化语言
    ndkLs.stepsGet().then(sobj => {
        if (sobj?.language) {
            ndkI18n.language = sobj.language;
        }

        // 初始化dom
        ndkInit.dom();
        //dom对象
        document.querySelectorAll('*').forEach(node => {
            if (node.classList.value.startsWith('nr-')) {
                var vkey = 'dom';
                node.classList[0].substring(3).split('-').forEach(c => vkey += c.substring(0, 1).toUpperCase() + c.substring(1))
                ndkVary[vkey] = node;
            }
        });
        // 语言选中
        ndkFn.actionRun('language', ndkI18n.language);

        // 编辑器初始化
        ndkEditor.init(meRequire);

        //事件
        ndkInit.event();
        //步骤恢复
        ndkStep.stepStart().then(() => {
            ndkVary.domLoading.style.display = "none";
            ndkVary.domMain.style.visibility = "visible";

            //setTimeout(() => console.clear(), 1000 * 2);
        })
    });
}, false);

export { ndkInit }
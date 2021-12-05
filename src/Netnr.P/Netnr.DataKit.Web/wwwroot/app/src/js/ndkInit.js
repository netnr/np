import { agg } from './agg';
import { ndkVary } from './ndkVary'
import { ndkFn } from './ndkFn'
import { ndkTab } from './ndkTab'
import { ndkStep } from './ndkStep'

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

        //选项卡1-调整大小
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
        //选项卡1-连接、库、表、列 面板切换调整大小
        ndkVary.domTabGroup1.addEventListener('sl-tab-show', function () {
            setTimeout(() => {
                ndkFn.size()
                ndkStep.stepSave()
            }, 1)
        }, false);
        //搜索-连接、库、表、列
        ['conns', 'database', 'table', 'column'].forEach(vkey => {
            vkey = ndkFn.fu(vkey);
            ndkVary[`domFilter${vkey}`].addEventListener('input', function () {
                var gridOps = ndkVary[`gridOps${vkey}`];
                if (gridOps && gridOps.api) {
                    gridOps.api.setQuickFilter(this.value);
                }
            })
        })

        //选项卡2-关闭
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
        //选项卡2-显示
        ndkVary.domTabGroup2.addEventListener('sl-tab-show', function (event) {
            if (event.target.classList.contains("nr-tab-group-2")) {
                ndkStep.cpInfo(event.detail.name); //显示连接                
            }
            setTimeout(() => {
                ndkFn.size()
            }, 1)
        }, false);

        window.addEventListener('resize', () => ndkFn.size(), false)
    },

}

window.addEventListener("DOMContentLoaded", function () {
    agg.lk();

    //dom对象
    document.querySelectorAll('*').forEach(node => {
        if (node.classList.value.startsWith('nr-')) {
            var vkey = 'dom';
            node.classList[0].substring(3).split('-').forEach(c => vkey += c.substring(0, 1).toUpperCase() + c.substring(1))
            ndkVary[vkey] = node;
        }
    })

    //事件
    ndkInit.event();
    //步骤恢复
    ndkStep.stepStart().then(() => {
        ndkVary.domLoading.style.display = "none";
        ndkVary.domMain.style.visibility = "visible";

        //setTimeout(() => console.clear(), 1000 * 2);
    })

}, false);

export { ndkInit }
import { vary } from './vary'
import { fn } from './fn'
import { tab } from './tab';

var init = {

    //事件初始化
    event: function () {

        //菜单项事件
        document.body.addEventListener('click', function (e) {
            var cmd = e.target.getAttribute("data-cmd")
            if (cmd != null) {
                fn.actionRun(cmd, e.target);
            }
        }, false);

        //选项卡1-调整大小
        vary.domSpliter1.querySelector('.nrc-spliter-bar').addEventListener('mousedown', function (edown) {
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
                    fn.actionRun('box1-size', psize)

                    window.removeEventListener('mousemove', fnmove)
                    window.removeEventListener('mouseup', fnup)

                    this.releaseCapture && this.releaseCapture();
                };

            window.addEventListener('mousemove', fnmove, false);
            window.addEventListener('mouseup', fnup, false);

            this.setCapture && this.setCapture();
        }, false);
        vary.domSpliter1.querySelector('.nrc-spliter-bar').addEventListener('dblclick', function (event) {
            var psize = (event.clientY - event.target.getBoundingClientRect().top) / event.target.clientHeight,
                activebar = event.target.nextElementSibling;
            if (psize < 0.33333) {
                activebar.style.left = '85%';
                fn.actionRun('box1-size', '85%')
            } else if (psize > 0.33333 && psize < 0.66666) {
                activebar.style.left = '50%';
                fn.actionRun('box1-size', '50%')
            } else {
                activebar.style.left = '15%';
                fn.actionRun('box1-size', '15%')
            }
        })
        //选项卡1-连接、库、表、列 面板切换调整大小
        vary.domTabGroup1.addEventListener('sl-tab-show', function () {
            setTimeout(() => {
                fn.size()
                step.stepSave()
            }, 1)
        }, false);
        //搜索-连接、库、表、列
        ['conns', 'database', 'table', 'column'].forEach(vkey => {
            vkey = fn.fu(vkey);
            vary[`domFilter${vkey}`].addEventListener('input', function () {
                var gridOps = vary[`gridOps${vkey}`];
                if (gridOps && gridOps.api) {
                    gridOps.api.setQuickFilter(this.value);
                }
            })
        })

        //选项卡2-关闭
        vary.domTabGroup2.addEventListener('sl-close', async event => {
            var sltab = event.target;
            var panel = vary.domTabGroup2.querySelector(`sl-tab-panel[name="${sltab.panel}"]`);

            if (sltab.active) {
                var otab = sltab.previousElementSibling || sltab.nextElementSibling;
                if (otab.nodeName != "sl-tab-panel".toUpperCase()) {
                    vary.domTabGroup2.show(otab.panel);
                } else {
                    step.cpInfo("reset"); //重置连接信息
                }
            }

            sltab.remove();
            panel.remove();
            tab.tabNavFix();

            //删除key
            delete tab.tabKeys[sltab.panel];
            step.cpRemove(sltab.panel)
        });
        //选项卡2-显示
        vary.domTabGroup2.addEventListener('sl-tab-show', function (event) {
            step.cpInfo(event.detail.name); //显示连接
            setTimeout(() => {
                fn.size()
            }, 1)
        }, false);

        window.addEventListener('resize', () => fn.size(), false)
    },

}

window.addEventListener("DOMContentLoaded", function () {
    init.event();
    step.stepStart().then(() => {
        vary.domLoading.style.display = "none";
        vary.domMain.style.visibility = "visible";
        console.clear();
    })

}, false);

export { init }
import { nrcBase } from "./nrcBase";
import { nrcRely } from "./nrcRely";

// 图表
let nrECharts = {

    /**
     * 资源依赖，默认远程，可重写为本地
     */
    init: async () => {
        await nrcRely.remote("echarts");
    },

    /**
     * 创建
     * @param {*} domChart 容器对象
     * @param {*} option 选项
     * @param {*} theme 主题，可选
     * @returns 
     */
    bind: async (domChart, option, theme) => {
        await nrECharts.init();

        option = Object.assign({
            renderer: 'svg',
            backgroundColor: 'transparent',
        }, option);

        if (theme == null) {
            theme = nrcBase.isDark() ? "dark" : null;
        }
        //初始化
        if (domChart['chart']) {
            domChart['chart'].dispose();
        } else {
            domChart.innerHTML = "";
        }
        let chart = echarts.init(domChart, theme, { renderer: option.renderer });
        chart.setOption(option);

        //大小自适应
        if (!domChart.dataset.bind) {
            domChart.dataset.bind = true;
            window.addEventListener('resize', () => {
                let chart = domChart['chart'];
                if (chart) {
                    chart.resize();
                }
            });
        }

        domChart['chartOption'] = option;
        domChart['chart'] = chart;
        return chart;
    },

    /**
     * 播放 tooltip
     * @param {*} domChart 
     * @param {*} interval 间隔时间，默认 5000 毫秒
     */
    playTooltip: (domChart, interval) => {
        if (domChart.dataset.bingPlay != "true") {
            domChart.dataset.bingPlay = "true";

            let currentIndex = -1;
            let autoplay = function () {
                // 只有当容器存在时才执行
                if (document.body.contains(domChart)) {
                    if (currentIndex == -1) {
                        currentIndex = 0;
                    } else {
                        let chart = domChart['chart'];
                        var dataLen = chart.getOption().series[0].data.length;
                        if (domChart.dataset.playPause != "true") {
                            // 取消之前高亮的图形
                            chart.dispatchAction({
                                type: 'downplay',
                                seriesIndex: 0,
                                dataIndex: currentIndex
                            });

                            currentIndex = (currentIndex + 1) % dataLen;

                            // 高亮当前图形
                            chart.dispatchAction({
                                type: 'highlight',
                                seriesIndex: 0,
                                dataIndex: currentIndex
                            });

                            // 显示 tooltip
                            chart.dispatchAction({
                                type: 'showTip',
                                seriesIndex: 0,
                                dataIndex: currentIndex
                            });
                        }
                    }
                    setTimeout(autoplay, interval || 5000);
                }
            }

            // 停止自动轮播
            domChart.addEventListener('mouseover', function () {
                domChart.dataset.playPause = "true";
            });

            // 恢复自动轮播
            domChart.addEventListener('mouseout', function () {
                domChart.dataset.playPause = "false";
            });

            autoplay();
        }
    },

    /**
     * 修改主题
     * @param {*} domChart 容器节点
     * @param {*} theme 可选
     */
    setTheme: async (domChart, theme) => {
        if (domChart) {
            let chart = domChart["chart"];
            let option = domChart["chartOption"];
            if (chart && option) {
                await nrECharts.bind(domChart, option, theme);
            }
        }
    },

    //macarons
    color2: ["#2ec7c9", "#b6a2de", "#5ab1ef", "#ffb980", "#d87a80", "#8d98b3", "#e5cf0d", "#97b552", "#95706d", "#dc69aa", "#07a2a4", "#9a7fd1", "#588dd5", "#f5994e", "#c05050", "#59678c", "#c9ab00", "#7eb00a", "#6f5553", "#c14089"],
    //infographic
    color3: ["#c1232b", "#27727b", "#fcce10", "#e87c25", "#b5c334", "#fe8463", "#9bca63", "#fad860", "#f3a43b", "#60c0dd", "#d7504b", "#c6e579", "#f4e001", "#f0805a", "#26c0c0"],
    //roma
    color4: ["#e01f54", "#001852", "#f5e8c8", "#b8d2c7", "#c6b38e", "#a4d8c2", "#f3d999", "#d3758f", "#dcc392", "#2e4783", "#82b6e9", "#ff6347", "#a092f1", "#0a915d", "#eaf889", "#6699FF", "#ff6666", "#3cb371", "#d5b158", "#38b6b6"],
}

Object.assign(window, { nrECharts });
export { nrECharts }
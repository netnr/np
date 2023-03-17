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

    color2: ["#fc97af", "#87f7cf", "#f7f494", "#72ccff", "#f7c5a0", "#d4a4eb", "#d2f5a6", "#76f2f2"],
    color3: ["#c12e34", "#e6b600", "#0098d9", "#2b821d", "#005eaa", "#339ca8", "#cda819", "#32a487"],
    color4: ["#3fb1e3", "#6be6c1", "#626c91", "#a0a7e6", "#c4ebad", "#96dee8"],
}

export { nrECharts }
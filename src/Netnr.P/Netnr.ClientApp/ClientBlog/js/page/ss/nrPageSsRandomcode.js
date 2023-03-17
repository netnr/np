import { nrcRandomatic } from "../../../../frame/nrcRandomatic";
import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/randomcode",
    ckey: "/ss/randomcode/config",

    init: async () => {
        await nrPage.readConfig();
        await nrPage.generate();

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domBtnBuild.addEventListener('click', nrPage.generate);
        nrVary.domTxtLength.addEventListener('input', nrPage.generate);
        nrVary.domTxtExclude.addEventListener('input', nrPage.generate);
        ['upper', 'lower', 'number', 'symbol'].forEach(t => {
            let dom = document.getElementById(`flag-${t}`);
            dom.addEventListener('click', nrPage.generate);
        });
        nrVary.domBtnReset.addEventListener('click', () => {
            [nrVary.domTxtLength, nrVary.domTxtExclude].forEach(dom => dom.value = dom.defaultValue);

            ['upper', 'lower', 'number', 'symbol'].forEach(t => {
                let dom = document.getElementById(`flag-${t}`);
                dom.checked = true;
            });

            nrPage.generate();
        });

    },

    readConfig: async () => {
        let config = await nrStorage.getItem(nrPage.ckey);
        if (config) {

            [nrVary.domTxtLength, nrVary.domTxtExclude].forEach((dom) => {
                if (dom.defaultValue == null) {
                    dom.defaultValue = dom.value;
                    dom.addEventListener('input', nrPage.generate);
                }

                let val = config[dom.id];
                if (val != null) {
                    dom.value = val;
                }
            });

            ['upper', 'lower', 'number', 'symbol'].forEach(t => {
                let dom = document.getElementById(`flag-${t}`);
                if (dom.defaultValue == null) {
                    dom.defaultValue = dom.checked ? 1 : 0;
                    dom.addEventListener('input', nrPage.generate);
                }

                if (dom.id in config) {
                    let val = config[dom.id];
                    dom.checked = (val != null && val != "");
                }
            });
        }
    },

    /**
     * 生成
     */
    generate: async () => {

        let mm = '';
        let len = nrVary.domTxtLength.value * 1;
        let ex = nrVary.domTxtExclude.value.trim();
        let config = {
            'flag-length': len,
            'flag-exclude': ex,
        };

        ['upper', 'lower', 'number', 'symbol'].forEach(t => {
            let dom = document.getElementById(`flag-${t}`);
            if (dom.checked) {
                mm += dom.value;
            }
            config[dom.id] = dom.checked ? dom.value : "";
        })

        if (mm != '' && len > 0) {
            let results = [];
            while (results.length < 9) {
                results.push(nrcRandomatic.randomize(mm, len, { exclude: ex == '' ? null : ex }));
            }
            nrVary.domTxtResult.value = results.join('\r\n');
        }

        //保存配置
        await nrStorage.setItem(nrPage.ckey, config);
    }
}

export { nrPage };
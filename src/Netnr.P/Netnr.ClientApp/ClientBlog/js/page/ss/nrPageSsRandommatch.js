import { nrStorage } from "../../../../frame/nrStorage";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/randommatch",
    ckey: "/ss/randommatch/config",

    init: async () => {
        await nrPage.readConfig();
        await nrPage.generate();

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domBtnBuild.addEventListener('click', nrPage.generate);
        nrVary.domBtnReset.addEventListener('click', function () {
            [nrVary.domTxtRange1, nrVary.domTxtRange2, nrVary.domTxtCount, nrVary.domTxtGroup, nrVary.domSeUnique].forEach(function (dom) {
                dom.value = dom.defaultValue;
            });
            nrPage.generate();
        });
        [nrVary.domTxtRange1, nrVary.domTxtRange2, nrVary.domTxtCount, nrVary.domTxtGroup, nrVary.domSeUnique].forEach(function (dom) {
            dom.addEventListener('input', nrPage.generate);
        });
    },

    readConfig: async () => {
        let config = await nrStorage.getItem(nrPage.ckey);
        if (config) {
            [nrVary.domTxtRange1, nrVary.domTxtRange2, nrVary.domTxtCount, nrVary.domTxtGroup].forEach(function (dom) {
                if (dom.defaultValue == null) {
                    dom.defaultValue = dom.value;
                    dom.addEventListener('input', nrPage.generate);
                }
                let val = config[dom.classList[0].split('-').pop()];
                if (val != null) {
                    dom.value = val;
                }
            });

            [nrVary.domSeUnique].forEach(function (dom) {
                if (dom.defaultValue == null) {
                    dom.defaultValue = dom.value;
                    dom.addEventListener('input', nrPage.generate);
                }
                let val = config[dom.classList[0].split('-').pop()];
                if (val != null) {
                    dom.value = val + "";
                }
            });
        }
    },

    /**
     * 生成
     */
    generate: async () => {

        let vm = { err: [], data: [] };
        try {
            let r1 = nrVary.domTxtRange1.value * 1;
            let r2 = nrVary.domTxtRange2.value * 1;
            let count = nrVary.domTxtCount.value * 1;
            let isUnique = nrVary.domSeUnique.value * 1;
            let group = (nrVary.domTxtGroup.value * 1) || 0;

            let config = {
                'range1': r1,
                'range2': r2,
                'count': count,
                'group': group,
                'unique': isUnique,
            }

            if (isNaN(r1) || isNaN(r2) || isNaN(count)) {
                vm.err.push("请输入有效的数字");
            }
            if (r1 > r2) {
                vm.err.push("随机范围有误");
            }
            if (r2 - r1 < (count - 1) && isUnique == 1) {
                vm.err.push("随机个数须小于等于范围数量");
            }

            if (!vm.err.length) {
                let rr = r2 - r1;
                let rv = [];

                while (rv.length < count) {
                    let ri = Math.floor(Math.random() * (rr + 1));
                    ri = r1 + ri;
                    if (isUnique == 1 && rv.indexOf(ri) >= 0) {
                        continue;
                    } else {
                        rv.push(ri);
                    }
                }

                let ni = 0, ci = 1;
                while (ni++ < rv.length) {
                    if (ni == rv.length) {
                        continue;
                    }
                    if (ci++ == group) {
                        ci = 1;
                        rv.splice(ni++, 0, "\r\n");
                    } else {
                        rv.splice(ni++, 0, "\t");
                    }
                }
                vm.data = rv;

                //保存配置
                await nrStorage.setItem(nrPage.ckey, config);
            }
        } catch (ex) {
            nrApp.logError(ex, '操作太骚，报错了')
        }

        if (vm.err.length) {
            nrApp.alert(vm.err.join("<br/>"));
        } else {
            nrVary.domTxtResult.value = vm.data.join("");
        }
    }
}

export { nrPage };
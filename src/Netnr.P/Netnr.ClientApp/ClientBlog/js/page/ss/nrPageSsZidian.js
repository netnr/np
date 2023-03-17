import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrVary } from "../../nrVary";

let zidian = {
    host: "https://npmcdn.com/zidian@0.0.3/dist",

    size: { word: 30, ci: 600, idiom: 150 },
    cache: {},

    /**
     * 查找
     * @param {*} type 类型：word ci idiom
     * @param {*} txt 关键词
     * @param {*} isSearch 是否搜索
     */
    find: async (type, txt, isSearch) => {
        let tocJson = zidian.cache[`${type}/00`];
        if (!tocJson) {
            let resp = await fetch(`${zidian.host}/${type}/00.json`);
            zidian.cache[`${type}/00`] = tocJson = await resp.json();
        }

        if (isSearch) {
            //搜索
            let result = tocJson.filter(x => x.includes(txt));
            if (result.length) {
                return result;
            }
        } else {
            //查询
            let ti = tocJson.indexOf(txt);
            if (ti >= 0) {
                let pi = Math.ceil((ti + 1) / zidian.size[type]) - 1;
                let ii = ti - pi * zidian.size[type];

                let indexJson = zidian.cache[`${type}/${pi}`];
                if (!indexJson) {
                    let resp = await fetch(`${zidian.host}/${type}/${pi}.json`);
                    zidian.cache[`${type}/${pi}`] = indexJson = await resp.json();
                }
                return indexJson[ii];
            }
        }
    }
}

Object.assign(window, { zidian });

let nrPage = {
    pathname: "/ss/zidian",

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        document.querySelectorAll('input').forEach(dom => {

            dom.nextElementSibling.addEventListener('click', async function () {
                let domTxt = this.previousElementSibling;
                let txt = domTxt.value.trim();
                if (txt != "") {
                    let type = domTxt.dataset.type;
                    let isSearch = domTxt.dataset.search == 1;

                    document.querySelectorAll('button').forEach(btn => {
                        nrApp.setLoading(btn)
                    })

                    await nrcRely.remote('netnrmd');
                    let fout = await zidian.find(type, txt, isSearch);
                    let result = "### 未找到";

                    switch (nrcBase.type(fout)) {
                        case "Array":
                            result = fout.join('<br/>');
                            break;
                        case "Object":
                            let rows = [];
                            for (const key in fout) {
                                const row = fout[key];
                                rows.push(`### ${key}\r\n${row.replace(/\n/g, '<br/>')}`);
                            }
                            result = rows.join('\r\n\r\n');
                            break;
                    }
                    nrVary.domResult.innerHTML = netnrmd.render(result);

                    document.querySelectorAll('button').forEach(btn => {
                        nrApp.setLoading(btn, true)
                    })
                } else {
                    nrVary.domResult.innerHTML = "";
                }
            })

            dom.addEventListener('keydown', async function (event) {
                if (event.code == "Enter") {
                    this.nextElementSibling.click();
                }
            });
        })
    }
}

export { nrPage, zidian };
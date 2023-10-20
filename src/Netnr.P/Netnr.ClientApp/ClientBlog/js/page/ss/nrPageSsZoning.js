import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrcBase } from "../../../../frame/nrcBase";

let nrPage = {
    pathname: "/ss/zoning",

    init: async () => {
        await nrPage.linkage(nrVary.domZoning3, 3);
        await nrPage.linkage(nrVary.domZoning5, 5);
    },

    npmsrc: nrcBase.mirrorNPM("https://npmcdn.com/zoningjs@2.2023.0/xx.json").replace("xx.json", ""),
    cacheJson: {},

    /**
     * 请求源
     * @param {*} json 
     * @returns 
     */
    reqJson: async (json) => {
        let result = nrPage.cacheJson[json];
        if (!result) {
            result = await nrWeb.reqServer(json);
            nrPage.cacheJson[json] = result;
        }
        return result;
    },

    /**
     * 联动
     * @param {*} box 
     * @param {*} deep 
     */
    linkage: async (box, deep) => {
        let ses = box.querySelectorAll("select");
        for (let i = 0; i < deep; i++) {
            let sei = ses[i];
            sei.setAttribute('data-index', i);

            let domItem = document.createElement("option");
            domItem.value = "";
            domItem.innerHTML = "（请选择）";
            sei.appendChild(domItem);

            //构建第一级
            let se0 = box.querySelector('select');
            if (i == 0) {
                let json = await nrPage.reqJson(`${nrPage.npmsrc}0.json`);

                let fdata = json.filter(x => x.pid == "0");
                fdata.forEach(item => {
                    let domItem = document.createElement("option");
                    domItem.value = item.id;
                    domItem.innerHTML = `${item.txt} - ${item.id}`;
                    domItem.setAttribute('data-item', JSON.stringify(item))
                    se0.appendChild(domItem);
                });
            }

            //选择联动
            sei.addEventListener('input', async function () {
                let sekey = this.value,
                    sei = Number(this.getAttribute('data-index')),
                    senext = box.querySelector(`[data-index="${sei + 1}"]`);

                //子节点重置
                box.querySelectorAll('select').forEach(se => {
                    if (se.getAttribute('data-index') > sei) {
                        se.innerHTML = '';
                        se.value = '';

                        let domItem = document.createElement("option");
                        domItem.value = "";
                        domItem.innerHTML = "（请选择）";
                        se.appendChild(domItem);
                    }
                });

                //显示值
                let val1 = [], val2 = [];
                box.querySelectorAll('select').forEach(se => {
                    if (se.value != "") {
                        let seitem;
                        for (let ci = 0; ci < se.children.length; ci++) {
                            let node = se.children[ci];
                            if (node.value == se.value) {
                                seitem = JSON.parse(node.getAttribute('data-item'));
                                break;
                            }
                        }
                        if (seitem.ct != null) {
                            val1.push(`${seitem.id},${seitem.ct}`);
                            val2.push(`${seitem.id},${seitem.ct}`);
                        } else {
                            val1.push(seitem.id);
                            val2.push(seitem.sid);
                        }
                    }
                });
                box.nextElementSibling.innerHTML = `<div>ID：<b>${val1.join(' / ')}</b></div><div>Short ID：<b>${val2.join(' / ')}</b></div>`;

                //子节点绑值
                if (senext && sekey != "") {
                    let seitem;
                    for (let ci = 0; ci < this.children.length; ci++) {
                        let node = this.children[ci];
                        if (node.value == this.value) {
                            seitem = JSON.parse(node.getAttribute('data-item'));
                            break;
                        }
                    }

                    if (seitem.deep >= 3) {
                        if (seitem.leaf == 1) {
                            senext.innerHTML = '';
                            senext.value = '';

                            let domItem = document.createElement("option");
                            domItem.value = "";
                            domItem.innerHTML = "（无数据）";
                            senext.appendChild(domItem);
                        } else {
                            let json = await nrPage.reqJson(`${nrPage.npmsrc}${seitem.id.substring(0, 4)}.json`);

                            //根据父节点筛选数据
                            let fdata = json.filter(x => x.pid == sekey);
                            if (fdata.length) {
                                fdata.forEach(item => {
                                    let domItem = document.createElement("option");
                                    domItem.value = item.id;
                                    domItem.innerHTML = `${item.txt} - ${item.id}`;
                                    domItem.setAttribute('data-item', JSON.stringify(item))
                                    senext.appendChild(domItem);
                                });
                            }
                        }
                    } else {
                        let json = await nrPage.reqJson(`${nrPage.npmsrc}0.json`);

                        let fdata = json.filter(x => x.pid == sekey);
                        fdata.forEach(item => {
                            let domItem = document.createElement("option");
                            domItem.value = item.id;
                            domItem.innerHTML = `${item.txt} - ${item.id}`;
                            domItem.setAttribute('data-item', JSON.stringify(item))
                            senext.appendChild(domItem);
                        });
                    }
                }
            });
        }
    }

}

export { nrPage };
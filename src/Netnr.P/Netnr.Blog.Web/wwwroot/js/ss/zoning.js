var npmsrc = "https://unpkg.com/zoningjs@2.2021.0/";

function requestJson(json) {
    return new Promise((resolve, reject) => {
        if (!window.cacheJson) {
            window.cacheJson = {};
        }

        if (json in window.cacheJson) {
            resolve(window.cacheJson[json])
        } else {
            fetch(json).then(x => x.json()).then(res => {
                window.cacheJson[json] = res;
                resolve(res);
            }).catch(err => {
                reject(err);
            })
        }
    });
}

function linkage(box, deep) {
    var ses = box.querySelectorAll("sl-select");
    for (var i = 0; i < deep; i++) {
        var sei = ses[i];
        sei.setAttribute('data-index', i);

        let domItem = document.createElement("sl-menu-item");
        domItem.value = "";
        domItem.innerHTML = "（请选择）";
        sei.appendChild(domItem);

        //构建第一级
        var se0 = box.querySelector('sl-select');
        if (i == 0) {
            requestJson(`${npmsrc}0.json`).then(json => {
                var fdata = json.filter(x => x.pid == "0");
                fdata.forEach(item => {
                    let domItem = document.createElement("sl-menu-item");
                    domItem.value = item.id;
                    domItem.innerHTML = `${item.txt} - ${item.id}`;
                    domItem.setAttribute('data-item', JSON.stringify(item))
                    se0.appendChild(domItem);
                });
            });
        }

        //选择联动
        sei.addEventListener('sl-change', function () {
            var sekey = this.value,
                sei = Number(this.getAttribute('data-index')),
                senext = box.querySelector(`[data-index="${sei + 1}"]`);

            //子节点重置
            box.querySelectorAll('sl-select').forEach(se => {
                if (se.getAttribute('data-index') > sei) {
                    se.innerHTML = '';
                    se.value = '';

                    let domItem = document.createElement("sl-menu-item");
                    domItem.value = "";
                    domItem.innerHTML = "（请选择）";
                    se.appendChild(domItem);
                }
            });

            //显示值
            var val1 = [], val2 = [];
            box.querySelectorAll('sl-select').forEach(se => {
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
                var seitem;
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

                        let domItem = document.createElement("sl-menu-item");
                        domItem.value = "";
                        domItem.innerHTML = "（无数据）";
                        senext.appendChild(domItem);
                    } else {
                        requestJson(`${npmsrc}${seitem.id.substring(0, 4)}.json`).then(json => {
                            //根据父节点筛选数据
                            var fdata = json.filter(x => x.pid == sekey);
                            if (fdata.length) {
                                fdata.forEach(item => {
                                    let domItem = document.createElement("sl-menu-item");
                                    domItem.value = item.id;
                                    domItem.innerHTML = `${item.txt} - ${item.id}`;
                                    domItem.setAttribute('data-item', JSON.stringify(item))
                                    senext.appendChild(domItem);
                                });
                            }
                        });
                    }
                } else {
                    requestJson(`${npmsrc}0.json`).then(json => {
                        var fdata = json.filter(x => x.pid == sekey);
                        fdata.forEach(item => {
                            let domItem = document.createElement("sl-menu-item");
                            domItem.value = item.id;
                            domItem.innerHTML = `${item.txt} - ${item.id}`;
                            domItem.setAttribute('data-item', JSON.stringify(item))
                            senext.appendChild(domItem);
                        });
                    });
                }
            }
        });
    }
}

linkage(document.querySelector('.nr-zoning3'), 3)
linkage(document.querySelector('.nr-zoning5'), 5)
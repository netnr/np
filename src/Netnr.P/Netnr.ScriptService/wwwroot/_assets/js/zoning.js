﻿var npmsrc = "https://npm.elemecdn.com/zoningjs@2.2021.0/";

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
    var ses = box.querySelectorAll("select");
    for (var i = 0; i < deep; i++) {
        var sei = ses[i];
        sei.setAttribute('data-index', i);
        sei.options.add(new Option("（请选择）", ""));

        //构建第一级
        var se0 = box.querySelector('select');
        if (i == 0) {
            requestJson(`${npmsrc}0.json`).then(json => {
                var fdata = json.filter(x => x.pid == "0");
                fdata.forEach(item => {
                    var option = new Option(`${item.txt} - ${item.id}`, item.id);
                    option.setAttribute('data-item', JSON.stringify(item))
                    se0.options.add(option);
                });
            });
        }

        //选择联动
        sei.onchange = function () {
            var sekey = this.value,
                sei = Number(this.getAttribute('data-index')),
                senext = box.querySelector(`[data-index="${sei + 1}"]`);

            //子节点重置
            box.querySelectorAll('select').forEach(se => {
                if (se.getAttribute('data-index') > sei) {
                    se.innerHTML = '';
                    se.options.add(new Option("（请选择）", ""));
                }
            });

            //显示值
            var val1 = [], val2 = [];
            box.querySelectorAll('select').forEach(se => {
                if (se.value != "") {
                    var seitem = JSON.parse(se.options[se.selectedIndex].getAttribute('data-item'));
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
                var seitem = JSON.parse(this.options[this.selectedIndex].getAttribute('data-item'));

                if (seitem.deep >= 3) {
                    if (seitem.leaf == 1) {
                        senext.innerHTML = '';
                        senext.options.add(new Option("（无数据）", ""));
                    } else {
                        requestJson(`${npmsrc}${seitem.id.substring(0, 4)}.json`).then(json => {
                            //根据父节点筛选数据
                            var fdata = json.filter(x => x.pid == sekey);
                            if (fdata.length) {
                                fdata.forEach(item => {
                                    var option = new Option(`${item.txt} - ${item.id}`, item.id);
                                    option.setAttribute('data-item', JSON.stringify(item))
                                    senext.options.add(option);
                                });
                            }
                        });
                    }
                } else {
                    requestJson(`${npmsrc}0.json`).then(json => {
                        var fdata = json.filter(x => x.pid == sekey);
                        fdata.forEach(item => {
                            var option = new Option(`${item.txt} - ${item.id}`, item.id);
                            option.setAttribute('data-item', JSON.stringify(item))
                            senext.options.add(option);
                        });
                    });
                }
            }
        }
    }
}

linkage(document.querySelector('.nr3'), 3)
linkage(document.querySelector('.nr5'), 5)
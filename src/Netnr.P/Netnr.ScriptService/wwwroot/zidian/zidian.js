/*
 * zidian
 * 2022-02-24
 */

(function (window) {

    var zidian = {
        config: {
            host: location.origin,
            word: {
                rows: 30,
                source: "dist/word/00.json"
            },
            ci: {
                rows: 600,
                source: "dist/ci/00.json"
            },
            idiom: {
                rows: 150,
                source: "dist/idiom/00.json"
            }
        },
        cache: {
            word: {},
            ci: {},
            idiom: {},
            promise: {}
        },
        //获取KYES
        taskKeys: function (type) {
            return zidian.cache.promise[type] = new Promise(function (resolve) {
                var keys = zidian.cache[type]["00"] || [];
                if (keys.length) {
                    resolve(keys);
                } else {
                    fetch(zidian.config.host + zidian.config[type].source).then(x => x.json()).then(function (res) {
                        zidian.cache[type]["00"] = keys = res;
                        resolve(keys)
                    })
                }
            })
        },
        //获取KEY的索引页
        taskItems: function (type, key) {
            return new Promise(function (resolve) {
                (zidian.cache.promise[type] || zidian.taskKeys(type)).then(function (keys) {
                    var ki = keys.indexOf(key);
                    if (ki >= 0) {
                        var pi = Math.ceil((ki + 1) / zidian.config[type].rows) - 1;
                        var ii = ki - pi * zidian.config[type].rows;
                        var pd = zidian.cache[type][pi] || [];
                        if (pd.length) {
                            resolve(pd[ii]);
                        } else {
                            fetch(zidian.config.host + zidian.config[type].source.replace("00", pi)).then(x => x.json()).then(function (res) {
                                zidian.cache[type][pi] = pd = res;
                                resolve(pd[ii]);
                            })
                        }
                    } else {
                        resolve();
                    }
                })
            })
        },
        //模糊搜索词
        likeCi: function (key) {
            return new Promise(function (resolve) {
                var type = 'ci';
                (zidian.cache.promise[type] || zidian.taskKeys(type)).then(function (keys) {
                    var res = [];
                    keys.forEach(x => {
                        x.indexOf(key) >= 0 && res.push(x);
                    })
                    resolve(res);
                })
            })
        },
        //模糊搜索成语
        likeIdiom: function (key) {
            return new Promise(function (resolve) {
                var type = 'idiom';
                (zidian.cache.promise[type] || zidian.taskKeys(type)).then(function (keys) {
                    var res = [];
                    keys.forEach(x => {
                        x.indexOf(key) >= 0 && res.push(x);
                    })
                    resolve(res);
                })
            })
        },
        //查询字
        equalWord: function (key) {
            return new Promise(function (resolve) {
                zidian.taskItems('word', key).then(resolve)
            })
        },
        //查询词
        equalCi: function (key) {
            return new Promise(function (resolve) {
                zidian.taskItems('ci', key).then(resolve)
            })
        },
        //查询成语
        equalIdiom: function (key) {
            return new Promise(function (resolve) {
                zidian.taskItems('idiom', key).then(resolve)
            })
        }
    }

    window.zidian = zidian;

})(window);
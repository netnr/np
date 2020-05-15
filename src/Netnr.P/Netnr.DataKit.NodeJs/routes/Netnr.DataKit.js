var express = require('express');
var fileto = require('../services/fileto');
var path = require("path");
var md5 = require('md5');
var router = express.Router();

var dk = {

    //数据库类型
    typeDB: ["MySQL", "SQLite", "Oracle", "SQLServer", "PostgreSQL"],

    //接口名称
    interfaceName: ["GetTable", "GetColumn", "SetTableComment", "SetColumnComment", "GetData"],

    /**
     * 判断是 NULL、空字符串
     * @param {any} obj
     */
    isNullOrWhiteSpace: function (obj) {
        if (obj == null || (obj + "").trim() == "") {
            return true;
        }
        return false;
    },

    /**
     * 获取时间
     * @param {any} timezone 时区，默认中国 东8区
     */
    time: function (timezone) {
        timezone = timezone == null ? 8 : timezone;
        return new Date(new Date().valueOf() + timezone * 3600000).toISOString().replace('Z', '').replace('T', ' ');
    },

    /**
     * 去除末尾匹配字符
     * @param {any} str 字符串
     * @param {any} c 匹配字符
     */
    trimEnd: function (str, c) {
        if (!c) { return this; }
        while (true) {
            if (str.substr(str.length - c.length, c.length) != c) {
                break;
            }
            str = str.substr(0, str.length - c.length);
        }
        return str;
    },

    /**
     * 数据库连接字符串解析
     */
    connectionOptions: function () {
        var ops = {
            code: null, //错误码
            msg: null   //错误消息
        }

        try {

            var conn = this.pars.conn;
            var tdb = this.typeDB[Number(this.pars.tdb)];;

            if (conn && conn.length > 5) {
                conn += ';';

                var matchEngine = {
                    regexs: {
                        "Data Source": /Data Source=(.*?);/i,
                        "Server": /Server=(.*?);/i,
                        "Host": /Host=(.*?);/i,
                        "(Host)": /\(Host=(.*?)\)/i,
                        "Port": /Port=(.*?);/i,
                        "(Port)": /\(Port=(.*?)\)/i,
                        "(SERVICE_NAME)": /\(SERVICE_NAME=(.*?)\)/i,
                        "Username": /Username=(.*?);/i,
                        "User Id": /User Id=(.*?);/i,
                        "UserId": /UserId=(.*?);/i,
                        "Uid": /Uid=(.*?);/i,
                        "Password": /Password=(.*?);/i,
                        "Pwd": /Pwd=(.*?);/i,
                        "Database": /Database=(.*?);/i
                    },
                    result: function (rgx) {
                        for (var i = 0; i < rgx.length; i++) {
                            var rr = matchEngine.regexs[rgx[i]].exec(conn);
                            if (rr) {
                                return rr[1]
                            }
                        }
                        return null;
                    }
                }

                switch (tdb) {
                    case "MySQL":
                        {
                            ops = {
                                host: matchEngine.result(["Data Source", "Server", "Host"]),
                                port: matchEngine.result(["Port"]) || 3306,
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                database: matchEngine.result(["Database"])
                            }
                        }
                        break;
                    case "SQLite":
                        {
                            return new Promise(function (resolve, reject) {
                                //下载 SQLite 文件
                                var ds = dk.trimEnd(conn.substring(12), ';');
                                //路径
                                var dspath = path.join(__dirname, '../') + "tmp/";
                                //总天数
                                var totalday = Math.floor(new Date().valueOf() / 1000 / 3600 / 24);
                                //文件名
                                var dsname = totalday + "_" + md5(ds) + ".db";

                                //网络路径
                                if (ds.toLowerCase().indexOf("http") == 0) {
                                    fileto.exists(dspath + dsname).then(x => {
                                        //不存在则下载
                                        if (x == false) {
                                            //删除今天前的文件
                                            fileto.readdir(dspath).then(data => {
                                                data.forEach(f => {
                                                    var day = f.split('_')[0] * 1;
                                                    if (day > 10000 && day < totalday) {
                                                        fileto.delete(dspath + f);
                                                    }
                                                })
                                            })

                                            //下载
                                            fileto.down(ds, dspath, dsname).then(() => {
                                                resolve({
                                                    filename: dspath + dsname,
                                                    database: dsname.split('.')[0]
                                                });
                                            })
                                        } else {
                                            resolve({
                                                filename: dspath + dsname,
                                                database: dsname.split('.')[0]
                                            });
                                        }
                                    })
                                } else {
                                    //本地物理路径
                                    fileto.exists(ds).then(x => {
                                        if (x) {
                                            resolve({
                                                filename: ds,
                                                database: dsname.split('.')[0]
                                            });
                                        } else {
                                            reject({});
                                        }
                                    })
                                }
                            })
                        }
                        break;
                    case "Oracle":
                        {
                            var host = matchEngine.result(["(Host)"]);
                            var port = matchEngine.result(["(Port)"]);
                            port = port ? ":" + port : "";
                            var server_name = matchEngine.result(["(SERVICE_NAME)"]);

                            ops = {
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                connectString: host + port + "/" + server_name
                            }
                        }
                        break;
                    case "SQLServer":
                        {
                            var serverandport = matchEngine.result(["Data Source", "Server"]).split(',');

                            ops = {
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                server: serverandport[0],
                                port: serverandport[1] || 1433,
                                database: matchEngine.result(["Database"]),
                                pool: {
                                    max: 10,
                                    min: 0,
                                    idleTimeoutMillis: 30000
                                }
                            }
                        }
                        break;
                    case "PostgreSQL":
                        {
                            ops = {
                                user: matchEngine.result(["Username", "User Id", "UserId", "Uid"]),
                                password: matchEngine.result(["Password", "Pwd"]),
                                host: matchEngine.result(["Data Source", "Server", "Host"]),
                                port: matchEngine.result(["Port"]) || 5432,
                                database: matchEngine.result(["Database"])
                            }
                        }
                        break;
                }
            } else {
                ops.code = 1;
                ops.msg = "连接字符串似乎有问题？";
            }
        } catch (e) {
            ops.code = -1;
            ops.msg = e;
        }

        return ops;
    },

    //初始化
    init: function () {

        //引入
        dk.typeDB.forEach(d => {
            dk[d] = require('./Netnr.DataKit.' + d);
        });

        //接口
        dk.interfaceName.forEach(iname => {
            router.all('/' + iname, function (req, res, next) {

                //跨域
                res.header('Access-Control-Allow-Origin', '*');
                res.header('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
                res.header('Access-Control-Allow-Headers', 'Accept, Authorization, Cache-Control, Content-Type, DNT, If-Modified-Since, Keep-Alive, Origin, User-Agent, X-Requested-With, Token, x-access-token');

                //返回对象
                var vm = {
                    code: 0,
                    msg: null,
                    data: null,
                    startTime: dk.time(),
                    endTime: null,
                    useTime: null
                };

                //接收参数
                dk.pars = req.method == "POST" ? req.body : req.query;

                //数据库类型
                var tdb = dk.typeDB[Number(dk.pars.tdb)];

                //按数据库类型调用接口对应的方法
                dk[tdb][iname](dk).then(ret => {
                    vm.code = 200;
                    vm.msg = "success";
                    vm.data = ret;
                    vm.endTime = dk.time();
                    vm.useTime = new Date(vm.endTime) - new Date(vm.startTime);

                    //输出结果
                    res.send(vm);
                }).catch(err => {
                    vm.code = -1;
                    vm.msg = JSON.stringify(err);

                    //输出结果
                    res.send(vm);
                })
            });
        })


        module.exports = router;
    }

}

dk.init();
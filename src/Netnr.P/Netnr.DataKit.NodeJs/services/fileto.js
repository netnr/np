var fs = require("fs");
var request = require("request");

module.exports = {

    /**
     * 下载
     * @param {any} url 下载文件链接地址
     * @param {any} dirPath 保存路径
     * @param {any} fileName 保存文件名
     */
    down: function (url, dirPath, fileName) {

        return new Promise(function (resolve, reject) {
            //创建临时文件夹
            if (!fs.existsSync(dirPath)) {
                fs.mkdirSync(dirPath);
            }

            let stream = fs.createWriteStream(dirPath + fileName);
            request(url).pipe(stream).on("close", function (err) {
                if (err) {
                    reject(err)
                } else {
                    resolve(true)
                }
            });
        })
    },

    /**
     * 读取目录（下一级文件、文件夹）
     * @param {any} dirpath 目录路径
     */
    readdir: function (dir) {
        return new Promise(function (resolve, reject) {
            fs.readdir(dir, function (err, data) {
                if (err) {
                    reject(err)
                } else {
                    resolve(data)
                }
            })
        })
    },

    /**
     * 删除文件
     * @param {any} fullpath 文件路径
     */
    delete: function (fullpath) {

        return new Promise(function (resolve, reject) {
            fs.unlink(fullpath, function (err) {
                if (err) {
                    reject(err)
                } else {
                    resolve(true)
                }
            })
        })
    },

    /**
     * 判断文件是否存在
     * @param {any} path 文件路径
     */
    exists: function (path) {

        return new Promise(function (resolve, reject) {
            try {
                fs.exists(path, function (exists) {
                    resolve(exists)
                })
            } catch (e) {
                reject(e);
            }
        })
    }
}
/**
 * bed storage 图床存储 （须支持跨域请求）
 * by netnr
 * */

(function (window) {

    let bs = function () { }

    //配置
    bs.config = {
        //分块大小
        chunkSize: 1024 * 1024 * 4,
        //混合存储大小（超过分块存储）
        mixSize: 1024 * 1024 * 2,
        //存储商（需要支持跨域）
        storageVendor: [],
        //基础图像
        baseImage: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAGklEQVR42mNkmJX4n4ECwDhqwKgBowYMFwMABjQfsXjkfQMAAAAASUVORK5CYII="
    };

    /**
     * 上传
     * @param {any} file
     */
    bs.upload = function (file) {
        return new Promise(function (resolve, reject) {
            let info = bs.newInfo();
            info.name = file.name;
            info.size = file.size;
            info.type = file.type;

            //需要分块
            if (file.size > bs.config.mixSize) {

            } else {
                //转Base64
                bs.fileAsBase64(file).then(b => {
                    info.fileContent = b;

                    let sv = bs.getStorageVendor();
                    sv.upload(bs.buildUploadObject(info, sv.type)).then(res => {
                        resolve(res);
                    }).catch(err => {
                        reject(err);
                    })
                })
            }
        })
    };

    /**
     * 下载
     * @param {any} url
     */
    bs.download = function (url) {
        return new Promise(function (resolve) {
            bs.urlAsBlob(url).then(blob => {
                let blen = bs.base64AsBlob(bs.config.baseImage).size;
                blob = blob.slice(blen, blob.size, "image/png");

                bs.blobAsText(blob).then(info => {
                    info = JSON.parse(info);
                    resolve(info);
                })
            })
        })
    };

    /** 获取上传存储商 */
    bs.getStorageVendor = function () {
        let sv = bs.config.storageVendor[0];

        function (file) {
            var fd = new FormData();
            fd.append('file', file);

            bs.ajax({
                url: sv.url,
                type: "POST",
                data: fd,
                dataType: "json",
                progress: function (p) {
                    console.log(p);
                },
                success: function (res) {
                    console.log(res);
                    res = JSON.parse(res);
                    resolve(res)
                },
                error: function (xhr, status) {
                    console.log(xhr, status);
                    reject(xhr, status);
                }
            })
        };
    };

    //得到一个新的存储商
    bs.newStorageVendor = function () {
        return {
            //名称
            name: null,
            //上传类型
            type: "file",
            //上传大小限制
            maxSize: null,
            //上传接口地址
            uploadUrl: null,
            //上传接口请求方式
            uploadMethod: "POST",
            //上传接口请求方式
            upoloadField: "file",
            //返回结果格式化
            dataType: "json"
        };
    };

    //得到一个新的存储信息
    bs.newInfo = function () {
        return {
            //名称
            name: null,
            //大小
            size: null,
            //类型
            type: null,
            //文件内容 Base64（混合存储时）
            fileContent: null,
            //文件索引（分块存储时）
            fileIndex: [],
            //日志
            logs: [],
            //创建时间
            create: new Date().toISOString(),
            //版本
            version: "0.0.1"
        };
    };

    /**
     * ajax请求
     * @param {JQuery.AjaxSettings} settings 类似于jQuery传参
     *
     * @param {string} url 请求地址
     * @param {string} type 请求方式，默认：GET
     * @param {boolean} async 异步请求，默认：true
     * @param {string} data 发送内容，字符串 | 键值对 | FormData
     * @param {string} contentType 内容编码类型，默认：application/x-www-form-urlencoded
     * @param {string} dataType 返回类型：json/xml/text，默认：text
     * @param {string} headers 消息头，键值对
     * @param {string} timeout 超时，单位：毫秒，默认设置超时
     * @param {string} progress function(p){ }，FormData上传进度回调方法 p：0-100
     * @param {string} success function(data,status,xhr){ }，成功回调方法，data：返回数据，
     * @param {string} error function(xhr,status){ }，错误回调方法
     * @param {string} complete:function(xhr,status){ }，完成回调
     */
    bs.ajax = function (settings) {
        settings.xhr = (window.XMLHttpRequest) ? (new XMLHttpRequest()) : (new ActiveXObject("Microsoft.XMLHTTP"));
        settings.type = (settings.type || "GET").toUpperCase();
        settings.async = settings.async == undefined ? true : settings.async;
        settings.data = settings.data || {};
        settings.headers = settings.headers || {};
        settings.contentType = settings.contentType || "application/x-www-form-urlencoded";
        settings.dataType = (settings.dataType || "text").toLowerCase();

        var darr = [], isfd = false;
        if (typeof settings.data == "object") {
            //FormData
            if (settings.data.__proto__.constructor.name == "FormData") {
                isfd = true;
                settings.type = "POST";
                settings.contentType = false;
                //进度监听
                settings.xhr.upload.onprogress = function (e) {
                    if (e.lengthComputable) {
                        typeof settings.progress == "function" && settings.progress(((e.loaded / e.total) * 100).toFixed(0));
                    }
                };
            } else {
                //键值对序列化
                for (var i in settings.data) {
                    darr.push('&' + encodeURIComponent(i) + '=' + encodeURIComponent(settings.data[i]));
                }
            }
        }

        //GET请求，参数拼接
        if (settings.type == "GET" && darr.length) {
            settings.url += (settings.url.indexOf('?') >= 0 ? '&' : '?') + darr.join('').substr(1);
        }

        var ts = null;
        settings.xhr.onreadystatechange = function () {
            if (settings.xhr.readyState == 4) {
                //成功
                if (settings.xhr.status == 200) {
                    //返回结果
                    var data = settings.xhr.responseText;
                    //成功回调
                    if (typeof settings.success == "function") {
                        switch (settings.dataType) {
                            case 'json':
                                //解析json
                                var pj = true;
                                try {
                                    if (typeof JSON == "function") {
                                        data = JSON.parse(data);
                                    } else {
                                        data = (new Function("return " + data))();
                                    }
                                }
                                catch (e) {
                                    //解析失败
                                    pj = false;
                                    typeof settings.error == "function" && settings.error(settings.xhr, "parsererror");
                                }
                                pj && settings.success.call(settings, data, "success", settings.xhr);
                                break;
                            case 'xml':
                                settings.success.call(settings, settings.xhr.responseXML, "success", settings.xhr);
                                break;
                            default:
                                settings.success.call(settings, data, "success", settings.xhr);
                                break;
                        }
                    }
                }
                else {
                    //错误回调
                    typeof settings.error == "function" && settings.error.call(settings, settings.xhr, ts || settings.xhr.statusText);
                }
                //完成回调
                typeof settings.complete == "function" && settings.complete.call(settings, settings.xhr, ts || settings.xhr.statusText);
            }
        };

        //监听异步超时
        if (settings.async && settings.timeout > 0) {
            setTimeout(function () {
                if (settings.xhr.readyState != 4) {
                    ts = "timeout";
                    //停止请求
                    settings.xhr.abort();
                }
            }, settings.timeout)
        }

        //open
        settings.xhr.open(settings.type, settings.url, settings.async);

        //设置header,非FromData
        if (!isfd) {
            settings.xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            settings.contentType && settings.xhr.setRequestHeader("Content-Type", settings.contentType);
        }
        //追加headers
        for (var i in settings.headers) {
            settings.xhr.setRequestHeader(i, settings.headers[i]);
        }

        //发送前回调
        if (typeof settings.beforeSend == "function") {
            if (settings.beforeSend.call(settings, settings) == false) {
                return false;
            }
        }

        //send
        settings.xhr.send(settings.data);
    };

    /**
     * info 转 节点
     * @param {any} info
     */
    bs.infoAsNode = function (info) {
        if (info.fileContent != null) {
            let blob = bs.base64AsBlob(info.fileContent), vnode;

            if (blob.type.indexOf("image") >= 0) {
                vnode = document.createElement("img");
            }
            if (blob.type.indexOf("audio") >= 0) {
                vnode = document.createElement("audio");
                vnode.controls = true;
            }
            if (blob.type.indexOf("video") >= 0) {
                vnode = document.createElement("video");
                vnode.controls = true;
            }
            if (vnode) {
                vnode.src = bs.objAsUrl(blob);
            } else {
                vnode = document.createElement("a");
                vnode.href = bs.objAsUrl(blob);
                vnode.setAttribute("download", info.name);
                vnode.innerHTML = info.name;
            }
            return vnode;
        }
    }

    /**
     * 构建上传对象
     * @param {any} info
     * @param {any} otype
     */
    bs.buildUploadObject = function (info, otype) {
        switch (otype) {
            case "blob":
                {
                    let ab = [bs.base64AsBlob(bs.config.baseImage), bs.textAsBlob(JSON.stringify(info))];
                    let blob = bs.mergeBlob(ab);
                    return blob;
                }
            case "file":
            default:
                {
                    let ab = [bs.base64AsBlob(bs.config.baseImage), bs.textAsBlob(JSON.stringify(info))];
                    let file = bs.mergeBlob(ab);
                    file.lastModifiedDate = new Date();
                    file.name = info.name;
                    return file;
                }
        }
    }

    /**
     * 文件转Base64
     * @param {any} file
     */
    bs.fileAsBase64 = function (file) {
        return new Promise(function (resolve) {
            var r = new FileReader();
            r.onload = function () {
                resolve(this.result)
            }
            r.readAsDataURL(file);
        });
    };

    /**
     * blob 转 文本
     * @param {any} text
     */
    bs.blobAsText = function (blob) {
        return new Promise(function (resolve) {
            var r = new FileReader();
            r.onload = function () {
                resolve(this.result)
            }
            r.readAsText(blob);
        });
    }

    /**
     * 文本 转 blob
     * @param {any} text
     */
    bs.textAsBlob = function (text) {
        return new Blob([text], { type: 'text/plain' });
    }

    /**
     * base64 转 blob
     * @param {any} code
     */
    bs.base64AsBlob = function (code) {
        let parts = code.split(';base64,');
        let contentType = parts[0].split(':')[1];
        let raw = window.atob(parts[1]);
        let rawLength = raw.length;
        let uInt8Array = new Uint8Array(rawLength);
        for (let i = 0; i < rawLength; ++i) {
            uInt8Array[i] = raw.charCodeAt(i);
        }
        return new Blob([uInt8Array], {
            type: contentType
        });
    };

    /**
     * 请求链接得到Blob对象
     * @param {any} url
     */
    bs.urlAsBlob = function (url) {
        return new Promise(function (resolve, reject) {
            let http = new XMLHttpRequest();
            http.open("GET", url, true);
            http.responseType = "blob";
            http.onload = function () {
                if (this.status == 200 || this.status === 0) {
                    resolve(this.response);
                } else {
                    reject(this);
                }
            };
            http.send();
        });
    };

    /**
     * object 转 URL
     * @param {any} obj
     */
    bs.objAsUrl = function (obj) {
        return URL.createObjectURL(obj);
    };

    /**
     * 分片
     * @param {any} file
     */
    bs.splitFile = function (file) {
        let chunks = [], start = 0, fileSize = file.size;
        while (start < fileSize) {
            let slen = start + bs.con.chunkSize;
            let chunk = file.slice(start, Math.min(slen, fileSize));
            start += bs.con.chunkSize;
            chunks.push(chunk);
        }
        return chunks;
    };

    /**
     * 合并 blob
     * @param {Array} arr 
     * @param {Array} mimeType
     */
    bs.mergeBlob = function (arr, mimeType) {
        return new Blob(arr, { type: mimeType || "image/png" });
    }

    if (true) {
        let sv = bs.newStorageVendor();
        sv.uploadUrl = "https://bit.baidu.com/upload/fileUpload";
        sv.name = "百度（技术学院）";
        sv.maxSize = 1024 * 1024 * 5;

        bs.config.storageVendor.push(sv);
    };

    window.bs = bs;

})(window);
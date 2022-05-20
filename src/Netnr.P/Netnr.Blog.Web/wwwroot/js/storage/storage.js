var page = {
    config: {
        SecretId: "",
        SecretKey: "",
        Bucket: "netnr",
        Region: "",
        Endpoint: "",
        CNAME: "",
        Domain: "",
        Prefix: "",
        Marker: "",
        Delimiter: ""
    },
    vender: {
        name: location.pathname.split("/").pop(),
        config: null,
        client: null,
        key: "",
        column: {},
        gridOptions: null,
        dataList: data => data,
        dataListSort: data => data,
        init: function () {
            page.vender.name = page.vender.name[0].toUpperCase() + page.vender.name.substring(1);

            switch (page.vender.name) {
                case "Qiniu":
                    {
                        Object.assign(page.vender, {
                            key: "key",
                            column: {
                                key: "key",
                                size: "fsize",
                                mimeType: "mimeType",
                                lastModified: "putTime",
                                lastModified_valueFormatter: params => {
                                    if (params.value) {
                                        return new Date(params.value.toString().substring(0, 13) * 1).toLocaleString()
                                    }
                                }
                            },
                            dataList: data => data.Result.items,
                            dataListSort: data => {
                                let list = data.Result.items;
                                list.sort((a, b) => {
                                    if (a.fsize == b.fsize) {
                                        return a.key.localeCompare(b.key)
                                    } else if (a.key.endsWith('/')) {
                                        return -1
                                    } else if (b.key.endsWith('/')) {
                                        return 1;
                                    } else {
                                        return a.key.localeCompare(b.key)
                                    }
                                });
                                return list;
                            }
                        });
                    }
                    break;
                case "Netease":
                    {
                        Object.assign(page.vender, {
                            key: "Key",
                            column: {
                                key: "Key",
                                size: "Size",
                                storageClass: "StorageClass",
                                lastModified: "LastModified",
                                lastModified_valueFormatter: params => {
                                    if (params.value) {
                                        return new Date(params.value.replace(" +", "+")).toLocaleString()
                                    }
                                }
                            },
                            dataList: data => data.ObjectSummarise,
                            dataListSort: data => {
                                let list = data.ObjectSummarise;
                                list.sort((a, b) => {
                                    if (a.Size == b.Size) {
                                        return a.Key.localeCompare(b.Key)
                                    } else if (a.Key.endsWith('/')) {
                                        return -1
                                    } else if (b.Key.endsWith('/')) {
                                        return 1;
                                    } else {
                                        return a.Key.localeCompare(b.Key)
                                    }
                                });
                                return list;
                            }
                        });
                    }
                    break;
                case "Tencent":
                    {
                        Object.assign(page.vender, {
                            key: "Key",
                            column: {
                                key: "Key",
                                size: "Size",
                                storageClass: "StorageClass",
                                lastModified: "LastModified",
                            },
                            dataList: data => data.Contents,
                            dataListSort: data => {
                                let list = data.Contents;
                                list.forEach(x => x.Size = x.Size * 1);
                                list.sort((a, b) => {
                                    if (a.Size == b.Size) {
                                        return a.Key.localeCompare(b.Key)
                                    } else if (a.Size == 0) {
                                        return -1
                                    } else if (b.Size == 0) {
                                        return 1;
                                    } else {
                                        return a.Key.localeCompare(b.Key)
                                    }
                                });
                                return list;
                            }
                        });
                    }
                    break;
            }

            //列
            let columnDefs = [
                {
                    field: page.vender.column.key, flex: 1, minWidth: 200, cellRenderer: params => {
                        if (params.value == null) {
                            return ''
                        } else if (params.value.endsWith('/')) {
                            return params.value
                        } else {
                            return `<a class="text-decoration-none" role="button" data-cmd="download-url">${params.value}</a>`
                        }
                    }
                },
                { field: page.vender.column.size, aggFunc: 'sum', valueFormatter: params => nr.formatByteSize(params.value) },
            ];
            if (page.vender.column.storageClass) {
                columnDefs.push({ field: page.vender.column.storageClass });
            }
            if (page.vender.column.mimeType) {
                columnDefs.push({ field: page.vender.column.mimeType });
            }
            columnDefs.push({ field: page.vender.column.lastModified, valueFormatter: page.vender.column.lastModified_valueFormatter });

            //配置
            page.vender.gridOptions = {
                localeText: ag.localeText, //语言
                defaultColDef: { filter: true, sortable: true, resizable: true, width: 180, },
                suppressRowClickSelection: true,
                rowSelection: 'multiple',
                columnDefs: columnDefs,
                autoGroupColumnDef: {
                    headerName: 'File', flex: 1, minWidth: 200, cellRendererParams: {
                        checkbox: true, suppressCount: true, innerRenderer: class {
                            init(params) {
                                var tempDiv = document.createElement('span');
                                if (params.node.group || (params.data && params.data[page.vender.key].endsWith('/'))) {
                                    tempDiv.innerText = `📂 ${params.value}`;
                                } else {
                                    var lkey = params.value.toLowerCase(), defaultIcon = '📄';
                                    for (const key in page.iconMap) {
                                        if (page.iconMap[key].filter(x => lkey.endsWith(x)).length) {
                                            defaultIcon = key;
                                            break;
                                        }
                                    }
                                    tempDiv.innerText = `${defaultIcon} ${params.value}`;
                                }
                                this.eGui = tempDiv;
                            }

                            getGui() {
                                return this.eGui;
                            }

                            refresh() {
                                return false;
                            }
                        },
                    },
                },
                treeData: true,
                animateRows: true,
                //groupDefaultExpanded: -1,
                getDataPath: function (data) {
                    var paths = data[page.vender.key].split('/');
                    if (paths[paths.length - 1] == "") {
                        paths.pop();
                    }
                    return paths;
                },
                getRowId: event => event.data[page.vender.key],
                getContextMenuItems: () => {
                    var result = [
                        'expandAll',
                        'contractAll',
                        'separator',
                        {
                            name: "Reload", icon: "🔄", action: function () {
                                page.load()
                            }
                        },
                        'autoSizeAll',
                        'resetColumns',
                        'separator',
                        'copy',
                        'copyWithHeaders'
                    ];

                    return result;
                }
            };
        }
    },
    uploader: [], //上传对象
    iconMap: {
        "📚": ".zip .7z .rar .iso .dll .unpkg".split(' '),
        "🎬": ".mp4 .avi .rmvb .wmv .mkv .flv".split(' '),
        "🎨": ".png .jpg .jpeg .gif .webp .ico".split(' '),
        "🎵": ".mp3 .wma .wav .ogg .ape".split(' '),
        "💠": ".exe .msi .apk .jar".split(' '),
        "📝": ".txt .md .doc .docx .xls .xlsx .ppt .pptx .pdf".split(' '),
    },
    init: () => {
        page.vender.name = page.vender.name[0].toUpperCase() + page.vender.name.substring(1);

        page.domGrid = document.querySelector('.nr-grid');
        page.domGrid.classList.add("ag-theme-alpine");

        page.eventBind().then(() => {
            page.load();
        })
    },
    eventBind: () => new Promise(function (resolve, reject) {

        //上传文件
        page.domUploaderFile = document.querySelector('.nr-uploader-file');
        nr.receiveFiles(function (files) {
            page.addFiles(files);
            page.domUploaderFile.value = "";
        }, page.domUploaderFile);

        //前缀
        page.domTxtSearchPrefix = document.querySelector('.nr-txt-search-prefix');
        page.domTxtSearchPrefix.addEventListener('keydown', function (e) {
            if (e.keyCode == 13) {
                page.config.Prefix = page.domTxtSearchPrefix.value;
                page.load();
            }
        }, false);

        //搜索
        page.domTxtSearchKey = document.querySelector('.nr-txt-search-key');
        page.domTxtSearchKey.addEventListener('input', function () {
            page.gridOps.api.setQuickFilter(this.value)
        }, false);

        //桶
        page.domSeBucket = document.querySelector('.nr-se-bucket');
        page.domSeBucket.addEventListener('change', function () {
            page.config.Bucket = this.value;
            page.load();
        }, false);

        //域名
        page.domTxtDomain = document.querySelector('.nr-txt-domain');
        page.domTxtDomain.addEventListener('input', function () {
            page.config.Domain = this.value;

            if (page.vender.name == "Tencent") {
                page.vender.client.options.Domain = this.value;
            }
        }, false);

        fetch('/Storage/API/key').then(resp => resp.json()).then(res => {
            //载入配置、初始化
            page.vender.config = res;
            var obj = page.vender.config[page.vender.name];
            page.config.SecretId = obj.SecretId;
            page.config.SecretKey = obj.SecretKey;
            page.config.Bucket = obj.Bucket1.Name;
            page.config.Region = obj.Bucket1.Region;
            page.config.Endpoint = obj.Bucket1.Endpoint;
            page.config.CNAME = obj.Bucket1.CNAME;
            page.config.Domain = obj.Bucket1.Domain;

            //载入桶
            Object.keys(obj).filter(k => k.startsWith("Bucket")).forEach(k => {
                var item = obj[k];
                var domItem = document.createElement('sl-menu-item');
                domItem.value = item.Name;
                domItem.setAttribute("data-region", item.Region || "");
                domItem.setAttribute("data-endpoint", item.Endpoint || "");
                domItem.setAttribute("data-cname", item.CNAME || "");
                domItem.setAttribute("data-domain", item.Domain || "");

                domItem.innerText = domItem.value;
                page.domSeBucket.appendChild(domItem);
            });
            page.domSeBucket.value = page.config.Bucket; //默认桶
            page.domTxtDomain.value = page.config.Domain; //默认域名

            //服务商参数对应
            page.vender.init();

            // js client
            if (page.vender.name == "Tencent") {
                page.vender.client = new COS(page.config);
                page.vender.client.options.Domain = page.config.Endpoint; //fix only http
            }

            resolve();
        })

        window.addEventListener('resize', function () {
            page.autoSize();
        });

        // 事件
        document.body.addEventListener("click", function (e) {
            var target = e.target, cmd = target.getAttribute("data-cmd");
            switch (cmd) {
                case "new-folder":
                    {
                        var snode = page.gridOps.api.getSelectedNodes(), prefix;
                        if (snode.length > 0) {
                            var node = snode[0];
                            if (node.group) {
                                prefix = ((function (nobj, path) {
                                    path = path || [];
                                    path.push(nobj.key);
                                    if (nobj.parent && nobj.parent.key != null) {
                                        return arguments.callee(nobj.parent, path);
                                    }
                                    return path;
                                })(node)).reverse().join('/') + "/";
                            } else {
                                var paths = node.data[page.vender.key].split('/');
                                if (paths.length > 1) {
                                    paths.pop();
                                    prefix = paths.join('/') + "/";
                                }
                            }
                        }

                        var createObj = prompt(`new (folder: abc/ , file: abc.txt)`, prefix || "");
                        if (createObj) {
                            var objOps = {};
                            objOps[page.vender.key] = createObj;
                            page.setLoading(true);
                            page.putObject(objOps).then(() => {
                                page.load();
                            });
                        }
                    }
                    break;
                case "delete":
                    {
                        var snode = page.gridOps.api.getSelectedNodes();
                        if (snode.length == 0) {
                            alert("Please choose");
                        } else {
                            var pathFolder = [], pathFile = [], pathFileData = [];
                            snode.forEach(node => {
                                if (node.group) {
                                    pathFolder.push(((function (nobj, path) {
                                        path = path || [];
                                        path.push(nobj.key);
                                        if (nobj.parent && nobj.parent.key != null) {
                                            return arguments.callee(nobj.parent, path);
                                        }
                                        return path;
                                    })(node)).reverse().join('/'));
                                } else {
                                    pathFileData.push(node.data);
                                    pathFile.push(node.data[page.vender.key]);
                                }
                            })

                            var paths = ["Are you sure to delete?", "\r\n\r\n", pathFolder.join("\r\n"), "\r\n\r\n", pathFile.join("\r\n")].join("");
                            if (confirm(paths)) {
                                //删除文件
                                if (pathFile.length > 0) {
                                    page.deleteMultipleObject(pathFile).then(() => {
                                        page.gridOps.api.applyTransaction({
                                            remove: pathFileData
                                        });
                                    })
                                }

                                //删除文件夹
                                if (pathFolder.length > 0) {
                                    pathFolder.forEach(path => {
                                        page.getBucket(Object.assign({}, page.config, { Prefix: path })).then(resChild => {
                                            console.log(page.config.Prefix);

                                            let list = page.vender.dataList(resChild);
                                            let delChildKeys = list.map(x => x[page.vender.key]);

                                            page.deleteMultipleObject(delChildKeys).then(() => {
                                                page.gridOps.api.applyTransaction({
                                                    remove: list
                                                });
                                            })
                                        })
                                    });
                                }
                            }
                        }
                    }
                    break;
                case "download-url":
                    {
                        var htm = [`<p class="text-break">${target.innerText}</p>`];
                        if (page.config.Domain != null && page.config.Domain != "") {
                            var urlDomain = `${page.config.Domain}/${target.innerText}`;
                            htm.push(`<sl-input class="mb-2" label="Domain" value="${urlDomain}"></sl-input>
                            <sl-qr-code value="${urlDomain}" fill="orange" background="transparent"></sl-qr-code>`);
                        }
                        if (page.config.CNAME != null && page.config.CNAME != "") {
                            var urlCNAME = `https://${page.config.CNAME}/${target.innerText}`;
                            htm.push(`<sl-input class="mb-2" label="CNAME" value="${urlCNAME}"></sl-input>
                            <sl-qr-code value="${urlCNAME}" fill="orange" background="transparent"></sl-qr-code>`);
                        }

                        page.dialog({
                            title: "Download",
                            body: htm.join("<sl-divider></sl-divider>"),
                        });
                        page.domDialog.querySelectorAll('sl-input').forEach(dom => {
                            dom.addEventListener("input", function () {
                                dom.nextElementSibling.value = dom.value;
                            });
                        });
                    }
                    break;
            }
        })
    }),
    setLoading: (loading) => {
        nr.domBtnNew.loading = loading;
        nr.domBtnDelete.loading = loading;
    },
    getToken: (key) => new Promise(function (resolve, reject) {
        fetch(`/Storage/API/${page.vender.name}/upload-token?` + nr.toQueryString({
            Bucket: page.config.Bucket,
            Endpoint: page.config.Endpoint,
            key: key
        })).then(resp => resp.text()).then(res => {
            resolve(res);
        }).catch(err => {
            reject(err);
        });
    }),
    /**
     * 桶列表
     */
    getBucket: (options) => new Promise((resolve, reject) => {
        switch (page.vender.name) {
            case "Qiniu":
            case "Netease":
                {
                    var ops = Object.assign({
                        Bucket: page.config.Bucket
                    }, options);

                    fetch(`/Storage/API/${page.vender.name}/list?${nr.toQueryString(ops)}`).then(resp => {
                        return resp.json()
                    }).then(data => {
                        resolve(data);
                    }).catch(err => {
                        reject(err);
                    });
                }
                break;
            case "Tencent":
                {
                    page.vender.client.getBucket(Object.assign({
                        Bucket: page.config.Bucket,
                        Region: page.config.Region,
                        Prefix: '',
                        Marker: '',
                        Delimiter: '', // /当前目录 空为深度遍历
                    }, options), function (err, data) {
                        if (err) {
                            reject(err)
                        } else {
                            resolve(data)
                        }
                    });
                }
                break;
        }
    }),
    /**
     * 创建 文件（夹）
     * @param {*} options 
     * @returns 
     */
    putObject: (options) => new Promise((resolve, reject) => {
        switch (page.vender.name) {
            case "Qiniu":
                {
                    page.getToken().then(token => {

                        var fd = new FormData();
                        fd.append('file', null);
                        fd.append("key", options.key);
                        fd.append("token", token);

                        //上传
                        var xhr = new XMLHttpRequest();
                        xhr.open("post", "//upload.qiniup.com", true);
                        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                        xhr.send(fd);
                        xhr.onreadystatechange = function (e) {
                            if (xhr.readyState == 4) {
                                if (xhr.status == 200) {
                                    try {
                                        var json = JSON.parse(xhr.responseText);
                                        resolve(json);
                                    } catch (e) {
                                        reject(e);
                                    }
                                } else {
                                    reject(xhr.statusText);
                                }
                            }
                        }
                    })
                }
                break
            case "Netease":
                {
                    page.uploadFile({
                        key: options.key,
                        file: '',
                        onFileFinish: function () {
                            resolve()
                        }
                    })
                }
                break;
            case "Tencent":
                {
                    page.vender.client.putObject(Object.assign({
                        Bucket: page.config.Bucket,
                        Region: page.config.Region,
                        Key: '',
                        Body: ''
                    }, options), function (err, data) {
                        if (err) {
                            reject(err)
                        } else {
                            resolve(data)
                        }
                    });
                }
                break;
        }
    }),
    /**
     * 上传
     * @param {*} options 
     * @returns 
     */
    uploadFile: (options) => {
        switch (page.vender.name) {
            case "Qiniu":
                {
                    page.getToken().then(token => {

                        var fd = new FormData();
                        fd.append('file', options.file);
                        fd.append("key", options.key);
                        fd.append("token", token);

                        //上传
                        var xhr = new XMLHttpRequest();
                        xhr.upload.onprogress = function (event) {
                            if (event.lengthComputable) {
                                options.onProgress(event);
                            }
                        };

                        xhr.open("post", "//upload.qiniup.com", true);
                        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                        xhr.send(fd);
                        xhr.onreadystatechange = function (e) {
                            if (xhr.readyState == 4) {
                                if (xhr.status == 200) {
                                    try {
                                        var json = JSON.parse(xhr.responseText);
                                        options.onFileFinish(json);
                                    } catch (e) {
                                        options.onFileFinish(e);
                                    }
                                } else {
                                    options.onFileFinish(xhr.statusText);
                                }
                            }
                        }
                    })
                }
                break;
            case "Netease":
                {
                    page.getToken(options.key).then(token => {
                        options.token = token;
                        //上传
                        page.uploadLoop(options);
                    })
                }
                break;
            case "Tencent":
                {
                    var objOps = Object.assign({
                        Bucket: page.config.Bucket,
                        Region: page.config.Region,
                        Key: '',
                        Body: null,
                        SliceSize: 1024 * 1024 * 2,
                        onTaskReady: function (taskId) {
                            console.log(taskId);
                        },
                        onProgress: function (progressData) {           /* 非必须 */
                            console.log(progressData);
                        },
                        onFileFinish: function (err, data, options) {   /* 非必须 */
                            console.log(options.Key + '上传' + (err ? '失败' : '完成'));
                        },
                    }, options);
                    objOps.Key = options.key;
                    objOps.Body = options.file;

                    page.vender.client.uploadFile(objOps);
                }
                break;
        }

    },
    uploadLoop: function (options) {
        options = Object.assign({
            bucket: page.config.Bucket,
            token: "",
            key: "",
            file: null,
            chunk: null,
            offset: 0,
            complete: "true",
            context: "",
            version: "1.0",
            chunkSize: 1024 * 1024 * 2, //切片
            onProgress: function (event) {
                console.debug(event);
            },
            onFileFinish: function (event) {
                console.debug(event);
            }
        }, options);

        //处理分片
        options.chunk = options.file.slice(options.offset, options.offset + options.chunkSize);
        if (options.file.size) {
            options.complete = (options.offset + options.chunkSize) >= options.file.size ? "true" : "false";
        } else {
            options.complete = "true";
        }
        var url = `https://nosup-eastchina1.126.net/${options.bucket}/${options.key}?offset=${options.offset}&complete=${options.complete}&context=${options.context}&version=${options.version}`

        //上传
        var xhr = new XMLHttpRequest();
        xhr.upload.onprogress = function (event) {
            if (event.lengthComputable) {
                options.onProgress({ loaded: options.offset + event.loaded, total: options.file.size || 0 });
            }
        };
        xhr.open("post", url, true);
        xhr.setRequestHeader("x-nos-token", options.token);
        xhr.send(options.chunk);
        xhr.onreadystatechange = function (e) {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    try {
                        var json = JSON.parse(xhr.responseText);
                        options.context = json.context;
                        options.offset = json.offset;
                        if (options.complete == "true") {
                            options.onFileFinish(json);
                        } else {
                            page.uploadLoop(options);
                        }
                    } catch (e) {
                        console.debug(e)
                        alert('上传失败');
                    }
                } else {
                    console.debug(xhr)
                    alert('上传失败');
                }
            }
        }
    },
    /**
     * 删除
     * @param {*} keys 
     * @returns 
     */
    deleteMultipleObject: (keys) => new Promise((resolve, reject) => {
        switch (page.vender.name) {
            case "Qiniu":
            case "Netease":
                {
                    var ops = Object.assign(page.config, {
                        Bucket: page.config.Bucket,
                        keys: keys.join(',')
                    });

                    fetch(`/Storage/API/${page.vender.name}/delete?` + nr.toQueryString(ops)).then(resp => {
                        if (page.vender.name == "Netease") {
                            return { resp }
                        } else {
                            return resp.json()
                        }
                    }).then(data => {
                        resolve(data)
                    })
                }
                break;
            case "Tencent":
                {
                    let objKeys = [];
                    keys.map(key => {
                        var objKey = {};
                        objKey[page.vender.key] = key;
                        objKeys.push(objKey)
                    });

                    page.vender.client.deleteMultipleObject({
                        Bucket: page.config.Bucket,
                        Region: page.config.Region,
                        Objects: objKeys
                    }, function (err, data) {
                        if (err) {
                            reject(err)
                        } else {
                            resolve(data)
                        }
                    });
                }
                break;
        }
    }),
    load: function () {
        page.setLoading(1);
        page.getBucket(page.config).then(data => {
            page.setLoading(false);
            nr.domBtnUploader.classList.remove('invisible');
            page.viewBucket(data);
        })
    },
    viewBucket: (data) => {
        data = page.vender.dataListSort(data);
        Object.assign(page.vender.gridOptions, { rowData: data })

        if (page.gridOps) {
            page.gridOps.api.setRowData(data)
        } else {
            page.gridOps = new agGrid.Grid(page.domGrid, page.vender.gridOptions).gridOptions;
        }
        page.autoSize();
    },
    addFiles: function (files) {
        var srow = page.gridOps.api.getSelectedRows();
        var path = srow.length ? srow[0].Key : "";
        if (!path.endsWith('/')) {
            path = path.substring(0, path.lastIndexOf('/') + 1);
        }

        for (let index = 0; index < files.length; index++) {
            let file = files[index];
            let obj = {
                path: path,
                file: file,
                status: 0,
                progress: 0,
            };

            page.uploadFile({
                key: obj.path + file.name,
                file: obj.file,
                onProgress: function (progressData) {
                    obj.progress = progressData;
                    page.refreshProgress();
                },
                onFileFinish: function () {
                    page.load();
                    page.refreshProgress();
                },
            })
            page.uploader.push(obj);
        }
    },
    refreshProgress: () => {
        let htm = [], isDone = true;
        page.uploader.forEach(item => {
            if (item.progress) {
                let p = ((item.progress.loaded / item.progress.total) * 100).toFixed(0) + '%';
                htm.push(`<div class="progress mb-2" title="${item.file.name}"><div class="progress-bar" role="progressbar" style="width: ${p};">${p}</div></div>`)

                if (item.progress.loaded < item.progress.total) {
                    isDone = false;
                }
            } else {
                htm.push(`<div class="progress mb-2" title="${item.file.name}"><div class="progress-bar" role="progressbar" style="width: 0%;">Ready</div></div>`)
            }
        });
        if (page.uploader.length == 0) {
            isDone = false;
        }

        page.dialog({
            title: isDone ? 'Uploaded' : 'Uploading',
            body: htm.join(''),
        });

        //完成清理、关闭
        if (isDone) {
            setTimeout(() => {
                page.uploader = [];
                page.domDialog.hide()
            }, 1000 * 2);
        }
    },
    lsSave: () => {
        var val = btoa(encodeURIComponent(JSON.stringify({ config: page.config })));
        localStorage.setItem(location.pathname, val)
    },
    lsLoad: () => {
        try {
            var val = localStorage.getItem(location.pathname);
            if (val) {
                var json = JSON.parse(decodeURIComponent(atob(val)));
                Object.assign(page.config, json.config);

                page.domSeBucket.value = page.config.Bucket;
                page.domTxtDomain.value = page.config.Domain;
                page.domTxtSearchPrefix.value = page.config.Prefix;
            }
        } catch (err) {
            console.log(err);
        }
    },
    dialog: (options) => {
        options = Object.assign({
            title: 'Message',
            body: '',
        }, options);

        if (page.domDialog == null) {
            page.domDialog = document.createElement('sl-dialog');
            document.body.appendChild(page.domDialog);
        }
        page.domDialog.label = options.title;
        page.domDialog.innerHTML = `<div>${options.body}</div>`;
        page.domDialog.show();
    },
    autoSize: () => {
        if (page.domGrid) {
            var ch = document.documentElement.clientHeight;
            var vh = ch - page.domGrid.getBoundingClientRect().top - 15;

            page.domGrid.style.height = vh + "px";
        }
    }
}

page.init();
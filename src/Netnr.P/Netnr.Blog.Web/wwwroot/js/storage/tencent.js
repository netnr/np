var nrObj = {
    config: {
        SecretId: "",
        SecretKey: "",
        AppId: "",
        Bucket: "",
        Region: "",
        Prefix: "",
        Marker: "",
        Domain: ""
    },
    uploader: [], //上传对象
    cos: null, //cos对象
    fixDomain: isHandle => {
        if (isHandle) {
            if (nrObj.cos.options.Domain != "" && !nrObj.cos.options.Domain.startsWith(location.protocol)) {
                nrObj.cos.options._Domain = nrObj.cos.options.Domain;
                nrObj.cos.options.Domain = '';
            }
        } else {
            if (nrObj.cos.options._Domain != null) {
                nrObj.cos.options.Domain = nrObj.cos.options._Domain;
            }
        }
    },
    iconMap: {
        "📚": ".zip .7z .rar .iso .dll .unpkg".split(' '),
        "🎬": ".mp4 .avi .rmvb .wmv .mkv .flv".split(' '),
        "🎨": ".png .jpg .jpeg .gif .webp .ico".split(' '),
        "🎵": ".mp3 .wma .wav .ogg .ape".split(' '),
        "💠": ".exe .msi .apk .jar".split(' '),
        "📝": ".txt .md .doc .docx .xls .xlsx .ppt .pptx .pdf".split(' '),
    },
    init: () => new Promise((resolve, reject) => {
        nrObj.domStorage = document.querySelector('.nr-storage');
        nrObj.domStorage.classList.add("ag-theme-alpine");

        //上传文件
        nrObj.domUploaderFile = document.querySelector('.nr-uploader-file');
        nrObj.domUploaderFile.addEventListener('change', function () {
            nrObj.addFiles(this.files);
            this.value = ""
        }, false);
        //上传文件夹
        nrObj.domUploaderFolder = document.querySelector('.nr-uploader-folder');
        nrObj.domUploaderFolder.addEventListener('change', function () {
            nrObj.addFiles(this.files);
            this.value = ""
        }, false);

        //前缀
        nrObj.domTxtSearchPrefix = document.querySelector('.nr-txt-search-prefix');
        nrObj.domBtnSearchPrefix = document.querySelector('.nr-btn-search-prefix');
        nrObj.domBtnSearchPrefix.addEventListener('click', function () {
            nrObj.config.Prefix = nrObj.domTxtSearchPrefix.value;
            nrObj.lsSave();
            nrObj.load();
        }, false);
        nrObj.domTxtSearchPrefix.addEventListener('keydown', function (e) {
            if (e.keyCode == 13) {
                nrObj.domBtnSearchPrefix.click();
            }
        }, false);

        //搜索
        nrObj.domTxtSearchKey = document.querySelector('.nr-txt-search-key');
        nrObj.domTxtSearchKey.addEventListener('input', function () {
            nrObj.gridOpsCos.api.setQuickFilter(this.value)
        }, false);

        //桶
        nrObj.domSeBucket = document.querySelector('.nr-se-bucket');
        nrObj.domSeBucket.addEventListener('change', function () {
            nrObj.config.Bucket = this.value;
            nrObj.cos.options.Bucket = this.value;
            nrObj.lsSave();
            nrObj.load();
        }, false);
        //域名
        nrObj.domTxtDomain = document.querySelector('.nr-txt-domain');
        nrObj.domTxtDomain.addEventListener('input', function () {
            nrObj.config.Domain = this.value;
            nrObj.cos.options.Domain = this.value;
            nrObj.lsSave();
        }, false);

        fetch("/Storage/TencentAPI").then(x => x.json()).then(res => {
            nrObj.lsLoad(); //本地配置

            //载入桶
            res.Bucket.split(',').forEach(item => {
                nrObj.domSeBucket.options.add(new Option(item, `${item}-${res.AppId}`));
            });

            if (nrObj.config.Bucket == "") {
                res.Bucket = nrObj.domSeBucket.value;
            } else {
                res.Bucket = nrObj.config.Bucket;
            }

            //载入配置、初始化
            nrObj.config = Object.assign(nrObj.config, res)
            nrObj.cos = new COS(nrObj.config);

            nrObj.load();
            resolve();
        });

        window.addEventListener('resize', function () {
            nrObj.autoSize();
        });

        document.body.addEventListener("click", function (e) {
            var target = e.target, cmd = target.getAttribute("data-cmd");
            switch (cmd) {
                case "uploader":
                    {
                        nrObj.modal({
                            title: target.innerHTML,
                            body: `
                                <div class="row">
                                    <div class="col-12">
                                        <div class="form-group">
                                            <label>选择文件</label>
                                            <input type="file" class="form-control" id="nr-file-uploader" multiple>
                                        </div>
                                    </div>
                                </div>                                
                            `,
                        })
                    }
                    break;
                case "new-folder":
                    {
                        var srow = nrObj.gridOpsCos.api.getSelectedRows(),
                            path = '';
                        if (srow.length) {
                            path = srow[0].Key
                        }

                        var folder = prompt(`new (prefix: ${path || '/'}, folder: abc/ , file: abc.txt)`);
                        if (folder) {
                            nrObj.putObject({ Key: path + folder }).then(() => {
                                nrObj.load();
                            });
                        }
                    }
                    break;
                case "delete":
                    {
                        var srow = nrObj.gridOpsCos.api.getSelectedRows();
                        if (srow.length == 0) {
                            alert("Please choose");
                        } else {
                            var paths = srow.map(x => x.Key);
                            paths.unshift("\r\n");
                            paths.unshift("Are you sure to delete?")
                            if (confirm(paths.join('\r\n'))) {
                                var delRows1 = srow.filter(x => !x.Key.endsWith('/')),
                                    delRows2 = srow.filter(x => x.Key.endsWith('/')),
                                    delKeys1 = delRows1.map(function (m) { return { Key: m.Key } }),
                                    delKeys2 = delRows2.map(function (m) { return { Key: m.Key } });

                                //删除文件
                                if (delKeys1.length > 0) {
                                    nrObj.deleteMultipleObject(delKeys1).then(() => {
                                        nrObj.gridOpsCos.api.applyTransaction({
                                            remove: delRows1
                                        });
                                    })
                                }

                                //删除文件夹
                                if (delKeys2.length > 0) {
                                    delKeys2.forEach(obj => {
                                        nrObj.getBucket({ Prefix: obj.Key }).then(resChild => {
                                            var delChildKeys = resChild.Contents.map(function (m) { return { Key: m.Key } });
                                            nrObj.deleteMultipleObject(delChildKeys).then(() => {
                                                nrObj.gridOpsCos.api.applyTransaction({
                                                    remove: resChild.Contents
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
                        nrObj.getObjectUrl({ Key: target.innerText }).then(res => {
                            nrObj.modal({
                                title: target.innerText,
                                body: `<textarea class="form-control mb-2">${res.Url}</textarea><a class="btn btn-warning btn-block" href="${res.Url}" target="_blank">Download</a>`,
                            })
                        })
                    }
                    break;
            }
        })
    }),
    /**
     * 桶列表
     */
    getBucket: (options) => new Promise((resolve, reject) => {
        nrObj.fixDomain(1);

        nrObj.cos.getBucket(Object.assign({
            Bucket: nrObj.config.Bucket,
            Region: nrObj.config.Region,
            Prefix: '',
            Marker: '',
            Delimiter: '', // /当前目录 空为深度遍历
        }, options), function (err, data) {
            nrObj.fixDomain(0);

            if (err) {
                console.debug(err)
                reject(err)
            } else {
                console.debug(data)
                resolve(data)
            }
        });
    }),
    /**
     * 创建
     * @param {*} options 
     * @returns 
     */
    putObject: (options) => new Promise((resolve, reject) => {
        nrObj.cos.putObject(Object.assign({
            Bucket: nrObj.config.Bucket,
            Region: nrObj.config.Region,
            Key: '',
            Body: ''
        }, options), function (err, data) {
            if (err) {
                console.debug(err)
                reject(err)
            } else {
                console.debug(data)
                resolve(data)
            }
        });
    }),
    /**
     * 上传
     * @param {*} options 
     * @returns 
     */
    uploadFile: (options) => new Promise((resolve, reject) => {
        nrObj.fixDomain(1);

        nrObj.cos.uploadFile(Object.assign({
            Bucket: nrObj.config.Bucket,
            Region: nrObj.config.Region,
            Key: '',
            Body: null,
            SliceSize: 1024 * 1024 * 5,
            onTaskReady: function (taskId) {
                console.log(taskId);
            },
            onProgress: function (progressData) {           /* 非必须 */
                console.log(JSON.stringify(progressData));
            },
            onFileFinish: function (err, data, options) {   /* 非必须 */
                console.log(options.Key + '上传' + (err ? '失败' : '完成'));
            },
        }, options), function (err, data) {
            nrObj.fixDomain(1);

            if (err) {
                console.debug(err)
                reject(err)
            } else {
                console.debug(data)
                resolve(data)
            }
        });
    }),
    /**
     * 删除
     * @param {*} keys 
     * @returns 
     */
    deleteMultipleObject: (keys) => new Promise((resolve, reject) => {
        nrObj.fixDomain(1);

        nrObj.cos.deleteMultipleObject({
            Bucket: nrObj.config.Bucket,
            Region: nrObj.config.Region,
            Objects: keys
        }, function (err, data) {
            nrObj.fixDomain(1);

            if (err) {
                console.debug(err)
                reject(err)
            } else {
                console.debug(data)
                resolve(data)
            }
        });
    }),
    /**
     * 获取Url
     * @param {*} options 
     * @returns 
     */
    getObjectUrl: (options) => new Promise((resolve, reject) => {
        nrObj.cos.getObjectUrl(Object.assign({
            Bucket: nrObj.config.Bucket,
            Region: nrObj.config.Region,
            Key: '',
            Sign: false
        }, options), function (err, data) {
            if (err) {
                console.debug(err)
                reject(err)
            } else {
                console.debug(data)
                resolve(data)
            }
        });
    }),
    formatByteSize: function (size, keep = 2, rate = 1024) {
        if (Math.abs(size) < rate) {
            return size + ' B';
        }

        const units = rate == 1000 ? ['KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'] : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
        let u = -1;
        const r = 10 ** keep;

        do {
            size /= rate;
            ++u;
        } while (Math.round(Math.abs(size) * r) / r >= rate && u < units.length - 1);

        return size.toFixed(keep) + ' ' + units[u];
    },
    viewBucket: (data) => {
        data.forEach(x => x.Size = x.Size * 1);
        data.sort((a, b) => {
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

        let gridOptions = {
            columnDefs: [
                {
                    field: 'Key', flex: 1, minWidth: 200, cellRenderer: params => {
                        if (params.value.endsWith('/')) {
                            return `<a href="javascript:void(0)" class="text-primary nr-grid-folder">${params.value}</a>`
                        } else {
                            return `<a href="javascript:void(0)" class="text-dark" data-cmd="download-url">${params.value}</a>`
                        }
                    }
                },
                { field: 'Size', aggFunc: 'sum', width: 150, valueFormatter: params => nrObj.formatByteSize(params.value) },
                { field: 'StorageClass', width: 150, },
                { field: 'LastModified', valueFormatter: params => new Date(params.value).toLocaleString() },
            ],
            defaultColDef: {
                filter: true,
                sortable: true,
                resizable: true,
            },
            suppressRowClickSelection: true,
            rowSelection: 'multiple',
            autoGroupColumnDef: {
                headerName: 'File', flex: 1, minWidth: 200,
                cellRendererParams: {
                    checkbox: true,
                    suppressCount: true,
                    innerRenderer: class {
                        init(params) {
                            var tempDiv = document.createElement('span');
                            if (params.node.group || (params.data && params.data.Key.endsWith('/'))) {
                                tempDiv.innerText = `📂 ${params.value}`;
                            } else {
                                var lkey = params.value.toLowerCase(), defaultIcon = '📄';
                                for (const key in nrObj.iconMap) {
                                    if (nrObj.iconMap[key].filter(x => lkey.endsWith(x)).length) {
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
            rowData: data,
            treeData: true,
            animateRows: true,
            //groupDefaultExpanded: -1,
            getDataPath: function (data) {
                var paths = data.Key.split('/');
                if (paths[paths.length - 1] == "") {
                    paths.pop();
                }
                return paths;
            },
            getRowNodeId: data => data.Key,
            getContextMenuItems: (params) => {
                var result = [
                    'expandAll',
                    'contractAll',
                    'separator',
                    {
                        name: "Reload", icon: "🔄", action: function () {
                            nrObj.load()
                        }
                    },
                    'autoSizeAll',
                    'resetColumns',
                    'separator',
                    'copy',
                    'copyWithHeaders'
                ];

                return result;
            },
        };

        if (nrObj.gridOpsCos) {
            nrObj.gridOpsCos.api.setRowData(data)
        } else {
            nrObj.gridOpsCos = new agGrid.Grid(nrObj.domStorage, gridOptions).gridOptions;
        }
        nrObj.autoSize();
    },
    load: function () {
        nrObj.getBucket(nrObj.config).then(data => {
            if (data.statusCode == 200) {
                nrObj.viewBucket(data.Contents);
            }
        })
    },
    addFiles: function (files) {
        var srow = nrObj.gridOpsCos.api.getSelectedRows();
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

            nrObj.uploadFile({
                Key: obj.path + file.name,
                Body: obj.file,
                onTaskReady: function (taskId) {
                    obj.taskId = taskId;
                },
                onProgress: function (progressData) {
                    obj.progress = progressData;
                    nrObj.refreshProgress();
                },
                onFileFinish: function (err, _data, options) {
                    if (err) {
                        obj.status = -1;
                        obj.error = err;
                    } else {
                        obj.status = 1;
                        obj.key = options.Key;
                        var folder = obj.key.substring(0, obj.key.lastIndexOf('/') + 1);

                        //获取当前目录
                        nrObj.getBucket({
                            Prefix: folder,
                            Delimiter: '/',
                        }).then(newRes => {
                            var rows = newRes.Contents.filter(x => x.Key == obj.key);
                            rows[0].Size = rows[0].Size * 1;

                            nrObj.gridOpsCos.api.applyTransaction({
                                add: rows
                            })
                        })
                    }

                    nrObj.refreshProgress();
                },
            })
            nrObj.uploader.push(obj);
        }
    },
    refreshProgress: () => {
        let htm = [], isDone = true;
        nrObj.uploader.forEach(item => {
            if (item.progress) {
                let p = ((item.progress.loaded / item.progress.total) * 100).toFixed(0) + '%';
                htm.push(`<div class="progress mb-2" title="${item.file.name}"><div class="progress-bar" role="progressbar" style="width: ${p};">${p}</div></div>`)
            } else {
                htm.push(`<div class="progress mb-2" title="${item.file.name}"><div class="progress-bar" role="progressbar" style="width: 0%;">Ready</div></div>`)
            }

            if (item.status == 0) {
                isDone = false;
            }
        });
        if (nrObj.uploader.length == 0) {
            isDone = false;
        }

        nrObj.modal({
            title: isDone ? 'Uploaded' : 'Uploading',
            body: htm.join(''),
        });

        //完成清理、关闭
        if (isDone) {
            setTimeout(() => {
                nrObj.uploader = [];
                $(nrObj.domModal).modal('hide')
            }, 1000 * 2);
        }
    },
    lsSave: () => {
        var val = btoa(encodeURIComponent(JSON.stringify({ config: nrObj.config })));
        localStorage.setItem(location.pathname, val)
    },
    lsLoad: () => {
        try {
            var val = localStorage.getItem(location.pathname);
            if (val) {
                var json = JSON.parse(decodeURIComponent(atob(val)));
                nrObj.config = Object.assign(nrObj.config, json.config);

                nrObj.domSeBucket.value = nrObj.config.Bucket;
                nrObj.domTxtDomain.value = nrObj.config.Domain;
                nrObj.domTxtSearchPrefix.value = nrObj.config.Prefix;
            }
        } catch (err) {
            console.log(err);
        }
    },
    modal: (options) => {
        options = Object.assign({
            title: 'Message',
            body: '',
        }, options);

        if (nrObj.domModal == null) {
            nrObj.domModal = document.createElement('div');
            nrObj.domModal.className = 'modal fade';
            nrObj.domModal.setAttribute('data-backdrop', 'static');
            document.body.appendChild(nrObj.domModal);
        }

        nrObj.domModal.innerHTML = `
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">${options.title}</h5>
                        <button type="button" class="close" data-dismiss="modal">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">${options.body}</div>
                </div>
            </div>
        `;

        $(nrObj.domModal).modal('show');
    },
    autoSize: () => {
        if (nrObj.domStorage) {
            var ch = document.documentElement.clientHeight;
            var vh = ch - nrObj.domStorage.getBoundingClientRect().top - 15;

            nrObj.domStorage.style.height = vh + "px";
        }
    }
}

nrObj.init();
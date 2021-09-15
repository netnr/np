/*
 * Netnr File Manager (NFM)
 * 2020-01-24 - 2021-03-02 
 * netnr
 */
(function (window) {

    var nfm = function () { };

    /**配置 */
    nfm.config = {
        //容器
        container: document.querySelector(".nfm"),
        //语言
        locale: "zh-CN",
        //图标
        icon: {
            app: "exe,apk",
            css: "css,less,sass",
            dll: "dll",
            excel: "xlsx,xls",
            file: "",
            folder: "",
            html: "html,htm,mht",
            image: "jpeg,jpg,png,gif,bpm,webp,svg",
            js: "js,ts,json",
            music: "mp3,wav,wma,ape,flac",
            pdf: "pdf",
            ppt: "pptx,ppt",
            text: "text,txt,log,ini",
            video: "mp4,wmv,avi,flv,3gp",
            zip: "zip,rar,7z,gz,tar"
        },
        //图标路径
        iconUri: function (ext) {
            var iconName = 'file';
            for (var i in nfm.config.icon) {
                var exts = nfm.config.icon[i].split(',');
                if (exts.includes(ext)) {
                    iconName = i;
                }
            }
            return `/lib/nfm/icon/${iconName}.svg`;
        }
    };

    /**缓存 */
    nfm.cache = {
        //UI
        ui: {},
        //点击
        click: {
            //最后数据行对象
            lastRow: null
        }
    };

    /**UI */
    nfm.ui = {

        //布局
        layout: function () {
            var html = `
                <div class="row">
                    <div data-box="buttonBox" class="buttonBox col-md-8"></div>
                    <div data-box="searchBox" class="searchBox col-md-4"></div>
                    <div data-box="pathBox" class="pathBox col-md-8"></div>
                    <div data-box="totalBox" class="totalBox col-md-4 d-none d-md-block"></div>                    
                </div>
                <div data-box="tableHeader" class="tableHeader row small"></div>
                <div data-box="tableBody" class="tableBody small"></div>
            `;
            nfm.config.container.innerHTML = html;
            var tags = document.getElementsByTagName('*');
            for (var i = 0; i < tags.length; i++) {
                var tag = tags[i], bn = tag.getAttribute("data-box")
                if (bn) {
                    nfm.cache.ui[bn] = tag;
                }
            }
        },

        /**
         * 按钮
         * @param {any} type 类型（0：empty；1：file；2：folder；3：mix）
         */
        buttonBox: function (type) {
            var buttons = [
                {
                    key: "upload",
                    html: `<div class="btn-group btn-group-sm mb-2 mr-2">
                            <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" data-reference="parent">
                              <span class="sr-only">Toggle Dropdown</span>
                            </button>
                            <div class="dropdown-menu">
                              <a class="dropdown-item" data-cmd="uploadFolder" href="javascript:void(0);">${nfm.locale.get('uploadFolder')}</a>
                            </div>
                            <button type="button" class="btn btn-primary" data-cmd="uploadFile" title="${nfm.locale.get('uploadTip')}">${nfm.locale.get('upload')}</button>
                          </div>`
                },
                {
                    key: "newFolder",
                    html: `<button type="button" class="btn btn-sm mb-2 btn-outline-primary mr-2" data-cmd="newFolder" >${nfm.locale.get('newFolder')}</button>`
                },
                {
                    key: "download",
                    html: `<button type="button" class="btn btn-sm mb-2 btn-outline-primary mr-2" data-cmd="download" >${nfm.locale.get('download')}</button>`
                },
                {
                    key: "delete",
                    html: `<button type="button" class="btn btn-sm mb-2 btn-outline-primary mr-2" data-cmd="delete" >${nfm.locale.get('delete')}</button>`
                },
                {
                    key: "rename",
                    html: `<button type="button" class="btn btn-sm mb-2 btn-outline-primary mr-2" data-cmd="rename" >${nfm.locale.get('rename')}</button>`
                },
                {
                    key: "copy",
                    html: `<button type="button" class="btn btn-sm mb-2 btn-outline-primary mr-2" data-cmd="copy" >${nfm.locale.get('copy')}</button>`
                },
                {
                    key: "move",
                    html: `<button type="button" class="btn btn-sm mb-2 btn-outline-primary mr-2" data-cmd="move" >${nfm.locale.get('move')}</button>`
                }
            ];

            var to = {
                "0": ["upload", "newFolder"],
                "1": ["upload", "newFolder", "download", "delete", "rename", "copy", "move"],
                "2": ["upload", "newFolder", "download", "delete", "rename", "copy", "move"],
                "3": ["upload", "newFolder", "download", "delete", "copy", "move"]
            }

            var htm = [];
            to[type].forEach(key => {
                htm.push(buttons.filter(x => x.key == key)[0].html);
            });

            var cdom = nfm.func.renderDom(htm.join(''));
            nfm.func.appendDom(nfm.cache.ui.buttonBox, cdom);
        },

        //搜素
        searchBox: function () {
            var html = `
                <input class="form-control form-control-sm mb-2" placeholder="`+ nfm.locale.get('searchTip') + `" />
            `;

            var cdom = nfm.func.renderDom(html);
            nfm.func.appendDom(nfm.cache.ui.searchBox, cdom);
        },

        /**
         * 路径
         * @param {any} path 路径字符串
         */
        pathBox: function (path) {
            path = path || "";
            var lis = [];
            path.split('/').forEach(pi => {
                if (pi != "") {
                    lis.push(`<li class="breadcrumb-item"><a href="javascript:void(0)" data-cmd="dirPath" title="${pi}">${pi}</a></li>`)
                }
            });

            var html = `
                <ol class="breadcrumb p-0 m-0 bg-white small" title="${path}">
                    <li class="breadcrumb-item"><a href="javascript:void(0)" data-cmd="dirPath">${nfm.locale.get('allFile')}</a></li>
                    ${lis.join('')}
                </ol>
            `;

            var cdom = nfm.func.renderDom(html);
            nfm.func.appendDom(nfm.cache.ui.pathBox, cdom);
        },

        /**
         * 总记录
         * @param {any} total 记录数
         */
        totalBox: function (total) {
            var html = `<small class="float-right">${nfm.locale.get('totalDispay').replace('{0}', total || 0)}</small>`;
            var cdom = nfm.func.renderDom(html);
            nfm.func.appendDom(nfm.cache.ui.totalBox, cdom);
        },

        /**
         * 表头
         */
        tableHeader: function (viewType) {
            var html;
            if (viewType == "grid") {

            } else {
                var items = [];
                items.push(`
                    <div class="col-md-8 py-1" data-cmd="headerName">
                        <input type="checkbox" data-cmd="checkAll">
                        ${nfm.locale.get('orderName')}
                    </div>
                    <div class="col-md-2 d-none d-md-block py-1" data-cmd="headerSize">${nfm.locale.get('orderSize')}</div>
                    <div class="col-md-2 d-none d-md-block py-1" data-cmd="headerDate">${nfm.locale.get('orderDate')}</div>
                `);

                html = items.join('');
            }

            var cdom = nfm.func.renderDom(html);
            nfm.func.appendDom(nfm.cache.ui.tableHeader, cdom);
        },

        /**
         * 表主体
         * @param {any} list 列表数据
         * @param {any} viewType 视图类型：grid list
         */
        tableBody: function (list, viewType) {
            list = list || [];

            var html;
            if (viewType == "grid") {

            } else {
                var items = [];
                list.forEach(li => {
                    items.push(`
                        <div class="row" data-cmd="colRow">
                            <div class="col-md-8 py-1 text-truncate" data-cmd="colName">
                                <input type="checkbox" data-cmd="checkOne">
                                <img src="${nfm.config.iconUri(li.fileExtension)}" class="list-fileIcon" />
                                <a class="text-muted" href="javascript:void(0)" data-cmd="fileName" title="${li.fileName}">${li.fileName || '-'}</a>
                            </div>
                            <div class="col-md-2 d-none d-md-block py-1 text-muted" data-cmd="colSize">
                                ${li.fileSize || '-'}
                            </div>
                            <div class="col-md-2 d-none d-md-block py-1 text-muted" data-cmd="colDate">
                                ${li.fileModDate || '-'}
                            </div>
                        </div>
                    `);
                });

                html = items.join('');
            }

            var cdom = nfm.func.renderDom(html);
            nfm.func.appendDom(nfm.cache.ui.tableBody, cdom);
        }
    };

    /**方法 */
    nfm.func = {

        /**
         * 渲染为 DOM
         * @param {any} html 字符串
         */
        renderDom: function (html) {
            var el = document.createElement('div');
            el.innerHTML = html;
            return el;
        },

        /**
         * 添加到 DOM
         * @param {any} parentDom
         * @param {any} doms
         */
        appendDom: function (parentDom, doms) {
            while (doms.children.length) {
                parentDom.appendChild(doms.children[0]);
            }
        },

        /**
         * 数据行选择样式
         * @param {any} row
         * @param {any} isSelected
         */
        rowSelectStyle: function (row, isSelected) {
            var ckOnes = row.getElementsByTagName('input');
            for (var i = 0; i < ckOnes.length; i++) {
                var ckOne = ckOnes[i];
                if (ckOne.getAttribute("data-cmd") == "checkOne") {
                    ckOne.checked = isSelected;
                    break;
                }
            }

            if (isSelected) {
                if (!(" " + row.className + " ").includes(" bg-light ")) {
                    row.className += " bg-light";
                }
            } else {
                row.className = " " + row.className.replace(" bg-light", "");
            }
        },

        /**
         * 选中行
         * @param {any} row
         */
        selectedRow: function (row, e) {
            //选中的行
            var srow = [];

            //Shift 多选
            if (nfm.cache.click.lastRow && e && e.shiftKey) {
                var rowAll = nfm.cache.ui.tableBody.children, arow = [];
                for (var i = 0; i < rowAll.length; i++) {
                    arow.push(rowAll[i]);
                }
                var startIndex = arow.indexOf(nfm.cache.click.lastRow),
                    endIndex = arow.indexOf(row);
                if (startIndex > endIndex) {
                    startIndex = endIndex + (endIndex = startIndex) - startIndex;
                }

                for (var i = startIndex; i <= endIndex; i++) {
                    srow.push(rowAll[i]);
                }
            } else {
                srow.push(row);
            }

            //选中
            var rows = row.parentNode.children;
            for (var i = 0; i < rows.length; i++) {
                var rowi = rows[i];
                var isSelected = srow.includes(rowi);
                nfm.func.rowSelectStyle(rowi, isSelected);
            }

            nfm.cache.click.lastRow = row;
        },

        /**处理点击事件 */
        globalClick: function () {
            document.body.addEventListener('click', function (e) {
                var target = e.target;
                var cmd = target.getAttribute('data-cmd');

                if (cmd) {
                    nfm.event.click[cmd] && nfm.event.click[cmd](e);
                }
            }, false);
        },

        /**
         * 获取图标
         * @param {any} fileName 文件名称
         */
        getIcon: function (fileName) {
            var ext = (fileName.split('.').pop() || "").toLowerCase();

            var vm = { name: "file", path: "/lib/nfm/icon/" };

            nfm.config.icon.map(() => {
                console.log(arguments)
            });

            return vm;
        },

        /**自适应大小 */
        resize: function () {
            var winh = document.documentElement.clientHeight;

            //表主体自适应
            var vh = winh - nfm.cache.ui.tableBody.getBoundingClientRect().top;
            nfm.cache.ui.tableBody.style.maxHeight = Math.max(200, vh - 15) + "px";

            //表头自适应
            var sw = nfm.cache.ui.tableBody.offsetWidth - nfm.cache.ui.tableBody.clientWidth;
            if (sw) {
                nfm.cache.ui.tableHeader.style.marginRight = (-15 + sw) + "px";
            }
        },

        /**
         * 随机数
         * @param {any} max 最大值
         */
        random: function (max) {
            return Math.floor(Math.random() * (max + 1))
        }

    };

    /**事件 */
    nfm.event = {

        //单击
        click: {
            /**列表 */
            list: function () {

            },

            /**上传文件 */
            uploadFile: function () {

            },

            /**上传文件夹 */
            uploadFolder: function () {

            },

            /**新建文件夹 */
            newFolder: function () {

            },

            /**下载 */
            download: function () {

            },

            /**删除 */
            delete: function () {

            },

            /**重命名 */
            rename: function () {

            },

            /**复制 */
            copy: function () {

            },

            /**移动 */
            move: function () {

            },

            /**目录定位 */
            dirPath: function () {

            },

            /**全选 */
            checkAll: function (e) {
                var ckAll = e ? e.target : nfm.cache.ui.tableHeader.getElementsByTagName('input')[0];
                var ckOnes = nfm.cache.ui.tableBody.getElementsByTagName('input');
                for (var i = 0; i < ckOnes.length; i++) {
                    var ckOne = ckOnes[i];
                    if (ckOne.getAttribute("data-cmd") == "checkOne") {
                        ckOne.checked = ckAll.checked;

                        //行样式
                        var row = ckOne.parentNode.parentNode;
                        nfm.func.rowSelectStyle(row, ckAll.checked);
                    }
                }
            },

            /**单选 */
            checkOne: function (e, target) {
                var ckAll = nfm.cache.ui.tableHeader.getElementsByTagName('input')[0];
                var ckOne = e ? e.target : target;

                if (!ckOne.checked) {
                    ckAll.checked = false;
                }

                //行样式
                var row = ckOne.parentNode.parentNode;
                nfm.func.rowSelectStyle(row, ckOne.checked);
            },

            /**数据行 */
            colRow: function (e, target) {
                var row = (e ? e.target : target);
                nfm.func.selectedRow(row, true);
            },

            /**名称 */
            colName: function (e, target) {
                var row = (e ? e.target : target).parentNode;
                nfm.func.selectedRow(row, e);
            },

            /**大小 */
            colSize: function (e, target) {
                var row = (e ? e.target : target).parentNode;
                nfm.func.selectedRow(row, e);
            },

            /**日期 */
            colDate: function (e, target) {
                console.log(arguments)
            },

            /**名称 */
            headerName: function (e, target) {
                console.log(arguments)
            },

            /**大小 */
            headerSize: function (e, target) {
                console.log(arguments)
            },

            /**日期 */
            headerDate: function (e, target) {
                console.log(arguments)
            },

            /**名称 */
            fileName: function () {

            }
        }

    };

    /**语言 */
    nfm.locale = {

        "zh-CN": {
            upload: "上传",
            uploadTip: "点击选择文件",
            uploadFile: "上传文件",
            uploadFolder: "上传文件夹",
            newFolder: "新建文件夹",
            download: "下载",
            delete: "删除",
            rename: "重命名",
            copy: "复制",
            move: "移动",
            searchTip: "搜索您的文件",
            searchButton: "搜索",
            orderName: "名称",
            orderSize: "大小",
            orderDate: "修改日期",
            backLevel: "返回上一级",
            allFile: "全部文件",
            totalDispay: "已全部加载，共 {0} 个"
        },

        "en": {
            upload: "Upload",
            uploadTip: "Click to select file",
            uploadFile: "Upload files",
            uploadFolder: "Upload folder",
            newFolder: "New folder",
            download: "Download",
            delete: "Delete",
            rename: "Rename",
            copy: "Copy",
            move: "Move",
            searchTip: "Search your files",
            searchButton: "Search",
            orderName: "Name",
            orderSize: "Size",
            orderDate: "Date",
            backLevel: "Back to previous",
            allFile: "All files",
            totalDispay: "All loaded, a total of {0}"
        },

        /**
         * 获取语言
         * @param {any} key
         */
        get: function (key) {
            return nfm.locale[nfm.config.locale][key];
        }
    };

    /**厂商 */
    nfm.vendor = [
        {
            key: "ufile",
            name: "UCloud 对象存储",
            icon: null,
            interface: {

            }
        }
    ];

    //UCloud
    nfm.initUCloud = function () {

    };

    /**初始化 */
    nfm.init = function () {

        nfm.config.container.onselectstart = function () { return false }

        nfm.ui.layout();
        nfm.ui.buttonBox(1);
        nfm.ui.searchBox();
        nfm.ui.pathBox('/abc/123/kdfkdfkdfkdf/abcabcabcabcabcabcabcabc/123456123456/');
        nfm.ui.totalBox();

        nfm.ui.tableHeader();

        var list = [];
        var exts = Object.values(nfm.config.icon).join(',').split(',');
        for (var i = 0; i < 88; i++) {
            list.push({
                type: ['file', 'folder'][(Math.random() * 1).toFixed(0)],
                fileName: `文件名文件名文件名文件名文件名文件名文件名文件名文件名文件名文件名文件名${nfm.func.random(9999)}`,
                fileExtension: exts[nfm.func.random(exts.length - 1)],
                fileSize: nfm.func.random(999),
                fileModDate: new Date().toLocaleDateString()
            })
        }
        console.log(list)
        nfm.ui.tableBody(list)

        nfm.func.globalClick();

        //自适应
        nfm.func.resize();
        window.addEventListener("resize", function () {
            nfm.func.resize()
        }, false);
    }

    window.nfm = nfm;

})(window);

nfm.init();
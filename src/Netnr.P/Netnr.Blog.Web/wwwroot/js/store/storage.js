(function (window) {

    var stg = function () { };

    /**配置 */
    stg.config = {
        //语言
        locale: "zh-CN",
        //容器
        container: document.getElementsByClassName("stg")[0]
    };

    /**缓存 */
    stg.cache = {
        //UI
        ui: {

        }
    };

    /**UI */
    stg.ui = {

        //布局
        layout: function () {
            var html = `
                <div data-box="buttonBox" class="col-md-8"></div>
                <div data-box="searchBox" class="col-md-4"></div>
                <div data-box="pathBox" class="col-md-8"></div>
                <div data-box="totalBox" class="col-md-4"></div>
                <div data-box="tableBox" class="col-md-12"></div>
            `;
            var cdom = stg.func.renderDom(html).children;
            while (cdom.length) {
                var cdi = cdom[0];
                stg.cache.ui[cdi.getAttribute('data-box')] = cdi;
                stg.config.container.appendChild(cdi)
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
                    html: `<div class="btn-group mr-2">
                            <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" data-reference="parent">
                              <span class="sr-only">Toggle Dropdown</span>
                            </button>
                            <div class="dropdown-menu">
                              <a class="dropdown-item" data-cmd="uploadFolder" href="javascript:void(0);">`+ stg.locale.get('uploadFolder') + `</a>
                            </div>
                            <button type="button" class="btn btn-primary" data-cmd="uploadFile" title="`+ stg.locale.get('uploadTip') + `">` + stg.locale.get('upload') + `</button>
                          </div>`
                },
                {
                    key: "newFolder",
                    html: `<button type="button" class="btn btn-outline-primary mr-2" data-cmd="newFolder" >` + stg.locale.get('newFolder') + `</button>`
                },
                {
                    key: "download",
                    html: `<button type="button" class="btn btn-outline-primary mr-2" data-cmd="download" >` + stg.locale.get('download') + `</button>`
                },
                {
                    key: "delete",
                    html: `<button type="button" class="btn btn-outline-primary mr-2" data-cmd="delete" >` + stg.locale.get('delete') + `</button>`
                },
                {
                    key: "rename",
                    html: `<button type="button" class="btn btn-outline-primary mr-2" data-cmd="rename" >` + stg.locale.get('rename') + `</button>`
                },
                {
                    key: "copy",
                    html: `<button type="button" class="btn btn-outline-primary mr-2" data-cmd="copy" >` + stg.locale.get('copy') + `</button>`
                },
                {
                    key: "move",
                    html: `<button type="button" class="btn btn-outline-primary mr-2" data-cmd="move" >` + stg.locale.get('move') + `</button>`
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

            var cdom = stg.func.renderDom(htm.join(''));
            stg.func.appendDom(stg.cache.ui.buttonBox, cdom);
        },

        //搜素
        searchBox: function () {
            var html = `
                <input class="form-control" placeholder="`+ stg.locale.get('searchTip') + `" />
            `;

            var cdom = stg.func.renderDom(html);
            stg.func.appendDom(stg.cache.ui.searchBox, cdom);
        }
    };

    /**方法 */
    stg.func = {

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

        /**处理点击事件 */
        globalClick: function () {
            $(document).click(function (e) {
                var target = e.target;
                var cmd = target.getAttribute('data-cmd');
                console.log(cmd, target.nodeName, target)
                if (cmd) {
                    stg.event[cmd]();
                }
            });
        }
    };

    /**事件 */
    stg.event = {

        /**上传 */
        uploadFile: function () {

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

        }

    };

    /**语言 */
    stg.locale = {

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
            orderName: "文件名",
            orderSize: "大小",
            orderDate: "修改日期",
            backLevel: "返回上一级",
            allFile: "全部文件",
            totalDispay: "已全部加载，共{0}个"
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
            return stg.locale[stg.config.locale][key];
        }
    };

    /**厂商 */
    stg.vendor = [
        {
            key: "ufile",
            name: "UCloud 对象存储",
            icon: null,
            interface: {

            }
        }
    ];

    /**初始化 */
    stg.init = function () {

        stg.ui.layout();
        stg.ui.buttonBox(1);
        stg.ui.searchBox();

        stg.func.globalClick();
    }

    window.stg = stg;

})(window);

stg.init();
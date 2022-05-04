var nr = {
    init: function () {
        //dom对象
        document.querySelectorAll('*').forEach(node => {
            if (node.classList.value.startsWith('nr-')) {
                var vkey = 'dom';
                node.classList[0].substring(3).split('-').forEach(c => vkey += c.substring(0, 1).toUpperCase() + c.substring(1))
                nr[vkey] = node;
            }
        });


        nr.navbarInit();
        nr.event();

        //ss
        if (document.cookie.includes(".theme=dark") && !document.documentElement.classList.contains("sl-theme-dark")) {
            nr.setTheme('dark')
        }

        nr.ready();
        nr.changeSize();
        window.addEventListener("resize", function () {
            nr.changeSize();
        })

        nr.setThemeGrid();
    },
    navbarInit: function () {
        if (nr.domNavbar) {
            //mobile menu Toggler
            nr.domNavbarToggler.addEventListener('click', function () {
                nr.domNavbarDrawer.style.display = "";
                nr.domNavbarDrawer.show();
            });

            if (nr.domNavbarDropdown) {
                nr.domNavbarDropdown.addEventListener('sl-show', function () {
                    nr.domNavbarDropdown.querySelector('sl-menu').style.display = "";
                });
            }

            //menu item click
            nr.domNavbar.querySelectorAll('sl-menu').forEach(menu => {
                menu.addEventListener('sl-select', function (e) {
                    var item = e.detail.item;

                    //jump
                    var href = item.getAttribute('data-href');
                    var newtab = item.getAttribute('data-target');
                    if (href != null) {
                        if (newtab == "_blank") {
                            window.open(href)
                        } else {
                            location.href = href;
                        }
                    }
                });
            })
        }

        nr.changeTheme();
    },

    event: function () {
        //body click
        document.body.addEventListener('click', function (e) {
            var target = e.target;
            var action = target.getAttribute('data-action');

            switch (action) {
                case "back-to-top":
                    document.documentElement.scrollTo(0, 0)
                    document.body.scrollTo(0, 0)
                    break;
                case "theme":
                    nr.setTheme(nr.isDark() ? "light" : "dark");
                    break;
            }
        }, false)
    },

    keyId: null, //编辑主键

    ready: function () {
        nr.onReady();
    },
    onReady: function () { },

    changeSize: function () {
        var ch = document.documentElement.clientHeight;
        var cw = document.documentElement.clientWidth;
        nr.onChangeSize(ch, cw);
    },
    onChangeSize: function (_ch, _cw) { },

    isDark: function () {
        return document.documentElement.classList.contains('sl-theme-dark')
    },
    setTheme: function (theme) {
        var oldTheme = theme == "dark" ? "light" : "dark";

        document.documentElement.className = document.documentElement.className.replace(oldTheme, theme);
        nr.domNavbar.className = nr.domNavbar.className.replaceAll(oldTheme, theme);

        nr.cookie('.theme', theme, 1000 * 3600 * 24 * 365);

        nr.changeTheme();
    },
    setThemeGrid: function (theme, dom) {
        dom = dom || nr.domGrid;
        if (dom) {
            theme = theme || (nr.isDark() ? "dark" : "light");
            if (theme == "dark") {
                dom.classList.remove("ag-theme-alpine")
                dom.classList.add("ag-theme-alpine-dark");
            } else {
                dom.classList.remove("ag-theme-alpine-dark")
                dom.classList.add("ag-theme-alpine");
            }
        }
    },
    changeTheme: function () {
        if (nr.domNavbar) {
            var chk = nr.domNavbar.querySelector('sl-menu-item[data-action="theme"]');
            if (chk) {
                chk.checked = nr.isDark();
            }
        }

        if (window["monaco"]) {
            if (nr.isDark()) {
                monaco.editor.setTheme('vs-dark');
            } else {
                monaco.editor.setTheme('vs');
            }
        }

        if (nr.nmd) {
            nr.nmd.toggleTheme(nr.isDark() ? "dark" : "light");
        }

        nr.setThemeGrid();

        nr.onChangeTheme();
    },
    onChangeTheme: function () {
        //console.debug(`Is Dark ${nr.isDark()}`)
    },

    toFormData: function (obj) {
        var fd = new FormData();
        for (var key in obj) {
            fd.append(key, obj[key]);
        }
        return fd;
    },
    toQueryString: function (obj) {
        var qs = [];
        for (var key in obj) {
            qs.push(key + "=" + encodeURIComponent(obj[key]));
        }
        return qs.join("&");
    },

    htmlEncode: html => {
        var div = document.createElement("div");
        div.innerText = html;
        return div.innerHTML;
    },
    htmlDecode: html => {
        var div = document.createElement('div');
        div.innerHTML = html;
        return div.innerText;
    },

    alert: function (message, icon = 'bell', duration = 5000) {
        const alert = Object.assign(document.createElement('sl-alert'), {
            closable: true, duration: duration,
            innerHTML: `<sl-icon name="${icon}" slot="icon"></sl-icon>${message}`
        });

        document.body.append(alert);
        return alert.toast();
    },

    dialog: function (message) {
        const dialog = Object.assign(document.createElement('sl-dialog'), {
            label: 'Message',
            innerHTML: `${message}`
        });

        document.body.append(dialog);
        dialog.show();
        return dialog;
    },

    cookie: function (key, value, ms) {
        if (arguments.length == 1) {
            var arr = document.cookie.match(new RegExp("(^| )" + key + "=([^;]*)(;|$)"));
            if (arr != null) {
                return arr[2];
            }
            return null;
        } else {
            var kv = key + "=" + value + ";path=/";
            if (ms) {
                var d = new Date();
                d.setTime(d.getTime() + ms);
                kv += ";expires=" + d.toGMTString();
            }
            document.cookie = kv;
        }
    },

    findScript: function (name) {
        var ds = document.scripts;
        for (let index = 0; index < ds.length; index++) {
            let si = ds[index];
            if (si.src.includes(name)) {
                return si;
            }
        }
    },

    //判断类型
    type: function (obj) {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },

    /**
     * 接收文件
     * @param {*} fn 回调
     * @param {*} fileNode 选择文件控件
     * @param {*} dragNode 拖拽区域，默认全局
     */
    receiveFiles: function (fn, fileNode, dragNode) {
        dragNode = dragNode || document;

        //拖拽
        dragNode.addEventListener('dragover', (event) => {
            if (!(fileNode && fileNode.contains(event.target))) {
                event.preventDefault();
                event.stopPropagation();
            }
        });
        dragNode.addEventListener("drop", (event) => {
            if (!(fileNode && fileNode.contains(event.target))) {
                event.preventDefault();
                var items = event.dataTransfer.items;
                nr.readDataTransferItems(items).then(files => {
                    if (files.length) {
                        fn(files, 'drag');
                    }
                });
            }
        });

        //浏览
        if (fileNode) {
            fileNode.addEventListener("change", function () {
                var files = this.files;
                if (files.length) {
                    fn(files, 'change');
                }
            });
        }

        //粘贴
        document.addEventListener('paste', function (event) {
            var items = event.clipboardData.items, files = [];
            for (let index = 0; index < items.length; index++) {
                var blob = items[index].getAsFile();
                blob && files.push(blob);
            }
            if (files.length) {
                fn(files, 'paste');
            }
        })
    },
    readDataTransferItems: (items) => new Promise((resolve) => {
        var parr = [], list = [];
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var itemEntry = item.webkitGetAsEntry();
            if (itemEntry != null) {
                parr.push(nr.readDataTransferItemEntry(itemEntry));
            } else {
                var file = item.getAsFile();
                if (file) {
                    list.push(file);
                }
            }
        }
        Promise.all(parr).then((arr) => {
            arr.forEach(x => {
                if (x.length) {
                    list = list.concat(x)
                } else {
                    list.push(x)
                }
            })
            resolve(list)
        })
    }),
    readDataTransferItemEntry: (itemEntry, path) => new Promise((resolve) => {
        path = path || "";

        if (itemEntry.isFile) {
            itemEntry.file(file => {
                if (path != "") {
                    file.fullPath = path + file.name; // 兼容路径丢失
                }
                resolve(file)
            })
        } else if (itemEntry.isDirectory) {
            var dirReader = itemEntry.createReader();
            dirReader.readEntries((entries) => {
                var parr = [];
                for (var i = 0; i < entries.length; i++) {
                    parr.push(nr.readDataTransferItemEntry(entries[i], path + itemEntry.name + "/"))
                }
                Promise.all(parr).then((arr) => {
                    var list = [];
                    arr.forEach(x => {
                        if (x.length) {
                            list = list.concat(x)
                        } else {
                            list.push(x)
                        }
                    })
                    resolve(list)
                })
            });
        }
    }),

    /* localStorage */
    ls: {},
    lsInit: function () {
        try {
            var lsv = localStorage.getItem(location.pathname);
            if (lsv != null && lsv != '') {
                nr.ls = JSON.parse(lsv);
            }
        } catch (e) {
            nr.ls = {};
            console.debug("localStorage parse error", e);
        }
    },
    lsArr: function (key) {
        return nr.ls[key] = nr.ls[key] || [];
    },
    lsObj: function (key) {
        return nr.ls[key] = nr.ls[key] || {};
    },
    lsStr: function (key) {
        return nr.ls[key] = nr.ls[key] || "";
    },
    lsSave: function () {
        localStorage.setItem(location.pathname, JSON.stringify(nr.ls));
    },

    /**
     * 下载
     * @param {any} content
     * @param {any} fileName
     */
    download: function (content, fileName) {
        var aTag = document.createElement('a');
        aTag.download = fileName;
        if (content.nodeType == 1) {
            aTag.href = content.toDataURL();
        } else {
            var blob = new Blob([content]);
            aTag.href = URL.createObjectURL(blob);
        }
        document.body.appendChild(aTag);
        aTag.click();
        aTag.remove();
    },

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

        return (size.toFixed(keep) * 1).toString() + ' ' + units[u];
    },
}

window.addEventListener("DOMContentLoaded", function () {
    nr.lsInit();
    nr.init();
}, false);

export { nr }
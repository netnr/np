import { netnrmd } from "./netnrmd";

class netnrmdInit {

    //编辑器容器
    domContainer;
    //编辑器主体
    domEditor;
    //工具栏
    domToolbar;
    //写
    domWrite;
    //显示
    domView;

    //工具条项
    objToolbarIcons = netnrmd.toolbarIcons;
    objWrite;
    objOptions;

    /**
     * 构造
     * @param {*} id 
     * @param {*} ops 
     */
    constructor(id, ops) {
        var that = this;

        ops = Object.assign({
            theme: 'auto', // 主题 light, dark, auto (自动)
            ph: '预览区域', // 预览区域提示
            toc: true, // 是否显示目录
            viewmodel: 2, // 视图：1 输入，2 分屏，3 预览
            fullscreen: 0, // 1 全屏，0 窗口
            fontsize: 16, // 编辑器字体大小
            height: 300, // 高度
            defer: 500, // 延迟解析（毫秒）
            headerIds: function (node, index) { node.id = "toc_" + index }, // 目录ID，true 默认ID
            autosave: true, // 默认有变化自动保存
            storekey: `${location.pathname}_netnrmd_content`, // 自动保存键，一个页面有多个编辑器时需要对应配置
        }, ops);

        //md容器
        this.domContainer = document.querySelector(id);
        this.domContainer.innerHTML = "";

        //渲染前回调
        if (typeof ops.viewbefore == "function") {
            ops.viewbefore.call(this)
        }

        //构建工具条
        var ulhtml = `<ul class="netnrmd-menu">${this.objToolbarIcons.map(x => netnrmd.toolbarBuildIcon(x)).join('\n')}</ul>`;
        this.domToolbar = netnrmd.createDom("div", "netnrmd-toolbar", ulhtml);

        //工具条加持命令响应
        this.domToolbar.firstChild.addEventListener('click', function (e) {
            var target = e.target;
            if (target.nodeName == "LI") {
                var cmdname = target.getAttribute('data-cmd');
                //执行命令
                that.cmd(cmdname);
            }
        }, false);

        //写
        this.domWrite = netnrmd.createDom("div", "netnrmd-write");
        Object.assign(this.domWrite.style, {
            fontSize: ops.fontsize + "px"
        });

        //视图
        this.domView = netnrmd.createDom("div", "netnrmd-view");
        this.domMarkdownBody = netnrmd.createDom("div", "markdown-body"); //内容
        this.domToc = netnrmd.createDom("div", "netnrmd-toc"); //目录
        this.domView.appendChild(this.domMarkdownBody);
        this.domView.appendChild(this.domToc);
        //编辑器
        this.domEditor = netnrmd.createDom("div", "netnrmd");
        this.domEditor.appendChild(this.domToolbar);
        this.domEditor.appendChild(this.domWrite);
        this.domEditor.appendChild(this.domView);

        this.setHeight(ops.height);

        //载入编辑器
        this.domContainer.appendChild(this.domEditor);

        //new ace 
        if (window["netnrmdAce"] != null) {
            //初始化写
            this.objWrite = new netnrmdAce(this.domWrite);

            //编辑器内容变动回调
            this.objWrite.session.on('change', function () {
                if (typeof that.objOptions.input == "function" && that.objOptions.input.call(that) == false) {
                    return false;
                }

                //自动保存
                if (that.objOptions.autosave) {
                    that.setstore();
                }

                //渲染
                that.render();
            });

            //滚动条同步
            this.objWrite.session.on("changeScrollTop", function (th) {
                var perc = th / (that.objWrite.renderer.scrollBar.scrollHeight - that.objWrite.renderer.scrollBar.element.clientHeight);
                that.domView.scrollTop = (that.domView.scrollHeight - that.domView.clientHeight) * perc;
            })

            //按键事件监听
            this.objToolbarIcons.forEach(item => {
                if (item.key) {
                    that.objWrite.commands.addCommand({
                        name: item.cmd,
                        bindKey: { win: item.key, mac: item.key.replace('Ctrl', 'Command') },
                        exec: function () {
                            that.cmd(item.cmd);
                        },
                        readOnly: false,
                        scrollIntoView: "cursor"
                    });
                }
            });
            //header1-6
            [1, 2, 3, 4, 5, 6].forEach(h => {
                that.objWrite.commands.addCommand({
                    name: `header${h}`,
                    bindKey: { win: `Ctrl+Alt+${h}`, mac: `Command+Alt+${h}` },
                    exec: function () {
                        that.cmd(`header${h}`);
                    },
                    readOnly: false,
                    scrollIntoView: "cursor"
                });
            });
        }

        //初始化响应配置
        this.objOptions = ops;

        //视图模式：1输入|2分屏|3预览
        this.toggleView(ops.viewmodel);

        //全屏：1
        this.toggleFullScreen(ops.fullscreen);

        //主题
        this.setTheme(ops.theme);

        //载入本地保存
        if (ops.autosave > 0) {
            this.getstore();
        }

        //添加窗口改变事件
        window.addEventListener('resize', function () {
            //全屏时
            if (that.domEditor.classList.contains("netnrmd-fullscreen")) {
                that.setHeight(document.documentElement.clientHeight);
            } else if (typeof that.objOptions.resize == "function") {
                that.objOptions.resize.call(that, document.documentElement.clientHeight);
            }
        });
        //初始化调用 resize
        if (typeof that.objOptions.resize == "function") {
            that.objOptions.resize.call(that, document.documentElement.clientHeight);
        }

        //浏览器变更主题
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
            if (that.objOptions.theme == 'auto') {
                that.setTheme(that.objOptions.theme);
            }
        });

        this.domContainer['netnrmd'] = this;
    }

    cmd(cmdname) {
        var ops = this.objOptions;

        //执行命令前回调
        if (typeof ops.cmdcallback == "function") {
            if (ops.cmdcallback.call(this, cmdname) == false) {
                return false;
            }
        }

        // 事件
        var itemIcon = this.objToolbarIcons.filter(x => x.cmd == cmdname)[0];
        if (itemIcon && itemIcon.action) {
            itemIcon.action(this);
        }

        var eobj = {
            before: '',
            dv: '',
            after: '',
            addrow: 0,
            //执行公共插入
            isdo: true
        }

        //编辑器选择对象
        var txtRange = this.objWrite.getSelectionRange(); //选中的范围
        var text = this.objWrite.session.getTextRange(txtRange); //选中的内容
        var textBeforeTheCursor = this.objWrite.session.getLine(txtRange.start.row).substring(0, txtRange.start.column).trim(); //选中的前面内容

        switch (cmdname) {
            case "bold":
                eobj.before = '**';
                eobj.dv = '粗体';
                eobj.after = '**';
                break;
            case "italic":
                eobj.before = '_';
                eobj.dv = '斜体';
                eobj.after = '_';
                break;
            case "strikethrough":
                eobj.before = '~~';
                eobj.dv = '删除';
                eobj.after = '~~';
                break;
            case "header":
            case "header1":
            case "header2":
            case "header3":
            case "header4":
            case "header5":
            case "header6":
                {
                    eobj.dv = '标题';
                    eobj.before = ' ';
                    var hn = cmdname.substring(6) * 1;
                    if (hn == 0) {
                        hn = 3;
                    }

                    while (hn-- > 0) {
                        eobj.before = "#" + eobj.before;
                    }

                    if (textBeforeTheCursor != "" && ["-", "+", "*"].includes(textBeforeTheCursor) && !/^\d+.$/.test(textBeforeTheCursor)) {
                        eobj.addrow = 1;
                        eobj.before = "\n" + eobj.before;
                    }
                }
                break;
            case "quote":
                {
                    eobj.before = '> ';
                    if (txtRange.start.column > 0) {
                        eobj.addrow = 1;
                        eobj.before = '\n> ';
                    }
                    eobj.dv = '引用内容';
                }
                break;
            case "list-ol":
                {
                    eobj.before = '1. ';
                    if (txtRange.start.column > 0) {
                        eobj.addrow = 1;
                        eobj.before = '\n1. ';
                    }
                    eobj.dv = '列表文本';
                }
                break;
            case "list-ul":
                {
                    eobj.before = '- ';
                    if (txtRange.start.column > 0) {
                        eobj.addrow = 1;
                        eobj.before = '\n- ';
                    }
                    eobj.dv = '列表文本';
                }
                break;
            case "checked":
                {
                    eobj.before = '- [x] ';
                    if (txtRange.start.column > 0) {
                        eobj.addrow = 1;
                        eobj.before = '\n- [x] ';
                    }
                    eobj.dv = '列表文本';
                }
                break;
            case "unchecked":
                {
                    eobj.before = '- [ ] ';
                    if (txtRange.start.column > 0) {
                        eobj.addrow = 1;
                        eobj.before = '\n- [ ] ';
                    }
                    eobj.dv = '列表文本';
                }
                break;
            case "link":
                eobj.before = '[链接说明](';
                eobj.dv = 'https://';
                eobj.after = ')';
                break;
            case "image":
                eobj.before = '![图片说明](';
                eobj.dv = 'https://';
                eobj.after = ')';
                break;
            case "table":
                {
                    var cols = 'col 1 | col 2 | col 3', hd = '--- | --- | ---', nl = '\r\n';
                    eobj.before = cols + nl + hd + nl + cols + nl + cols + nl + nl;

                    eobj.addrow = 5;
                    if (txtRange.start.column > 0) {
                        eobj.addrow = 7;
                        eobj.before = "\n\n" + eobj.before;
                    }
                }
                break;
            case "code":
                {
                    if (txtRange.start.column == 0) {
                        eobj.addrow = 1;
                        eobj.before = '```\n';
                        eobj.after = '\n```';
                    } else {
                        eobj.before = '`';
                        eobj.after = '`';
                    }
                    eobj.dv = '输入代码';
                }
                break;
            case "line":
                {
                    eobj.before = '---\r\n';
                    if (txtRange.start.column > 0) {
                        eobj.addrow = 1;
                        eobj.before = '\n---\r\n';
                    }
                }
                break;
            default:
                eobj.isdo = false;
                break;
        }

        if (eobj.isdo && cmdname && cmdname != "") {
            if (text.trim() == "") {
                text = eobj.dv;
            }

            this.focus();

            //光标处插入
            this.insert(eobj.before + text + eobj.after);

            //光标选中
            txtRange.start.column += eobj.before.length;
            txtRange.end.column = txtRange.start.column + text.length;
            if (eobj.addrow > 0) {
                txtRange.start.row += eobj.addrow;
                txtRange.end.row += eobj.addrow;
                txtRange.start.column = eobj.before.length - 1;
                if (cmdname == "code") {
                    txtRange.start.column = 0;
                }
                txtRange.end.column += text.length;
            }
            this.objWrite.selection.setRange(txtRange);
        }
    }

    //焦点
    focus() {
        this.objWrite.focus();
    }

    //插入内容
    insert(content) {
        this.objWrite.insert(content);
    }

    //添加按键命令
    addCommand(keys, exec) {
        this.objWrite.commands.addCommand({
            name: keys,
            bindKey: { win: keys, mac: keys.replace("Ctrl", "Command") },
            exec
        });
    }

    //设置高度
    setHeight(height) {
        var weh = (height - (this.domToolbar.style.display == "none" ? 0 : this.domToolbar.offsetHeight)) + "px";
        this.domWrite.style.height = weh;
        this.domView.style.height = weh;
        if (this.objWrite) {
            this.objWrite.resize();
        }
        return this;
    }

    //设置高度（非全屏模式时）
    height(height) {
        if (height != null) {
            this.objOptions.height = height;
            if (!this.objOptions.fullscreen) {
                this.setHeight(height);
            }
            return this;
        } else {
            return this.objOptions.height;
        }
    }

    //全屏切换
    toggleFullScreen(fullscreen) {
        var tt = this.getToolbarTarget('fullscreen');
        this.objOptions.fullscreen = !this.objOptions.fullscreen;
        if (fullscreen != null) {
            this.objOptions.fullscreen = fullscreen;
        }
        if (!this.objOptions.fullscreen) {
            this.domEditor.classList.remove('netnrmd-fullscreen');
            tt.classList.remove('active');
            this.height(this.objOptions.height);
        } else {
            this.domEditor.classList.add('netnrmd-fullscreen');
            tt.classList.add('active');
            this.setHeight(document.documentElement.clientHeight);
        }
        if (this.objWrite) {
            this.objWrite.resize();
        }
    }

    //视图切换
    toggleView(n) {
        if (n == null) {
            n = this.objOptions.viewmodel - 1;
            if (n < 1) {
                n = 3;
            }
        }
        this.objOptions.viewmodel = n;

        this.domWrite.classList.remove("netnrmd-write-w100", "netnrmd-write-hidden");
        this.domView.classList.remove("netnrmd-view-hidden", "netnrmd-view-w100");
        switch (n) {
            case 1:
                this.domWrite.classList.add("netnrmd-write-w100");
                this.domView.classList.add("netnrmd-view-hidden");
                break;
            case 3:
                this.domWrite.classList.add("netnrmd-write-hidden");
                this.domView.classList.add("netnrmd-view-w100");
                break;
        }
        if (this.objWrite) {
            this.objWrite.resize();
        }
    }

    //设置主题
    setTheme(theme) {
        this.objOptions.theme = theme;

        var domIcon = this.getToolbarTarget('theme');
        var itemIcon = netnrmd.toolbarIcons.filter(x => x.cmd == "theme")[0];
        var tooltip = `${itemIcon.title}/${itemIcon.cmd} ${(itemIcon.key ? itemIcon.key : '')}`;
        domIcon.title = `${tooltip} ${theme}`;

        if (theme == 'auto') {
            theme = window.matchMedia("(prefers-color-scheme: dark)").matches ? 'dark' : 'light';
        }

        document.documentElement.classList.remove("netnrmd-dark", "netnrmd-light");
        switch (theme) {
            case "dark":
                if (this.objWrite) {
                    this.objWrite.setTheme("ace/theme/tomorrow_night");
                }
                document.documentElement.classList.add("netnrmd-dark");
                break;
            default:
                if (this.objWrite) {
                    this.objWrite.setTheme("ace/theme/github");
                }
                document.documentElement.classList.add("netnrmd-light");
                break;
        }
    }

    /**
     * 主题切换
     * @param {*} theme 指定，默认循环切换
     */
    toggleTheme(theme) {
        if (theme == null) {
            var themes = ["light", "dark", "auto"];
            var ti = themes.indexOf(this.objOptions.theme) + 1;
            if (ti >= themes.length) {
                ti = 0;
            }
            theme = themes[ti];
        }
        this.setTheme(theme);
    }

    //根据命令获取工具条的对象
    getToolbarTarget(cmd) {
        var target;
        this.domToolbar.querySelectorAll('li').forEach(item => {
            if (item.getAttribute('data-cmd') == cmd) {
                target = item;
            }
        })
        return target;
    }

    //赋值md
    setmd(md) {
        if (this.objWrite) {
            this.objWrite.setValue(md, -1);
        }
        return this;
    }

    //获取md
    getmd() {
        if (this.objWrite) {
            return this.objWrite.getValue()
        }
        return "";
    }

    //呈现html
    sethtml(html) {
        this.domMarkdownBody.innerHTML = html;

        //重写ID
        if (typeof this.objOptions.headerIds == "function") {
            var menus = this.domMarkdownBody.querySelectorAll("h1,h2,h3,h4.h5,h6");
            for (var i = 0; i < menus.length; i++) {
                var item = menus[i];
                this.objOptions.headerIds(item, i);
            }
        }

        return this;
    }

    //获取html
    gethtml() {
        return this.domMarkdownBody.innerHTML;
    }

    //获取目录
    gettoc() {
        return this.domToc.innerHTML;
    }

    //间隙
    spacing() {
        this.setmd(netnrmd.pangu.spacing(this.getmd()));
    }

    //渲染
    render() {
        var that = this;
        clearTimeout(this.objOptions.deferIndex);
        this.objOptions.deferIndex = setTimeout(function () {
            var md = that.getmd();
            if (md == "") {
                //清理html、本地缓存
                that.sethtml('<div class="netnrmd-view-empty">' + that.objOptions.ph + '</div>');
                that.setstore();
            } else {
                that.sethtml(netnrmd.render(md));
                that.toc();
            }
        }, this.objOptions.defer);
    }

    //目录
    toc() {
        if (this.objOptions.toc) {
            netnrmd.tocbot.init({
                tocElement: this.domToc,
                contentElement: this.domView,
                headingSelector: 'h1, h2, h3, h4, h5, h6',
                scrollElement: this.domEditor
            })
        }
    }

    //隐藏
    hide(area) {
        switch (area) {
            case "toolbar":
                this.domToolbar.style.display = "none";
                break;
            default:
                this.domEditor.style.display = "none";
        }
    }

    //显示
    show(area) {
        switch (area) {
            case "toolbar":
                this.domToolbar.style.display = "";
                break;
            default:
                this.domEditor.style.display = "";
        }
    }

    //写入本地保存
    setstore() {
        localStorage[this.objOptions.storekey] = this.getmd();
    }

    //获取本地保存
    getstore() {
        var md = localStorage[this.objOptions.storekey];
        if (md != null) {
            this.setmd(md);
            this.render(); // 初始化时，赋值未触发变更事件
        }
    }

    //保存
    save(format, filename) {
        var that = this;
        var oldvm = this.objOptions.viewmodel;

        format = format.toLowerCase();
        if (format == "markdown") {
            format = "md";
        }
        if (format == "word") {
            format = "docx";
        }
        if (format == "image") {
            format = "png";
        }
        filename = filename || ("nmd." + format);

        switch (format) {
            case "md":
                netnrmd.download(this.getmd(), filename);
                break;
            case "html":
            case "docx":
                {
                    var cssPath;
                    for (var i = 0; i < document.scripts.length; i++) {
                        var domScript = document.scripts[i];
                        if (domScript.src.includes("netnrmd.js")) {
                            cssPath = domScript.src.replace(".js", ".css");
                            break;
                        }
                    }

                    var htmlContent = this.gethtml();
                    fetch(cssPath).then(resp => resp.text()).then(cssStyle => {
                        var html = '<!DOCTYPE html><html><head>'
                            + '<meta name="viewport" content="width=device-width, initial-scale=1.0" />'
                            + '<meta charset="utf-8" /><style type="text/css">' + cssStyle + '</style>'
                            + '</head><body><div class="markdown-body">' + htmlContent + '</div></body></html>';

                        if (format == "html") {
                            netnrmd.download(html, filename);
                        }
                        else if (format == "docx") {
                            netnrmd.readyPackage("htmlDocx", () => {
                                netnrmd.download(htmlDocx.asBlob(html), filename)
                            })
                        }
                    })
                }
                break;
            case "png":
            case "jpg":
                {
                    if (oldvm == 1) {
                        this.toggleView(2);
                    }

                    that.domView.style.padding = 0;
                    that.domMarkdownBody.style.padding = "15px";

                    netnrmd.readyPackage("html2canvas", () => {
                        html2canvas(that.domMarkdownBody, { scale: 1.2, }).then(function (canvas) {
                            netnrmd.download(canvas, filename);

                            that.domView.style.removeProperty("padding");
                            that.domMarkdownBody.style.removeProperty("padding");

                            if (oldvm == 1) {
                                this.toggleView(1);
                            }
                        })
                    })
                }
                break;
            case "pdf":
                {
                    if (oldvm == 1) {
                        this.toggleView(2);
                    }

                    netnrmd.readyPackage("html2pdf", () => {
                        html2pdf(that.domMarkdownBody, {
                            margin: 3, filename: filename, html2canvas: { scale: 1.2 }
                        })

                        if (oldvm == 1) {
                            this.toggleView(1);
                        }
                    })
                }
                break;
            default:
                console.debug("unsupported format");
                break;
        }
    }
}

export { netnrmdInit };
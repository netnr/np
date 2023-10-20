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
     * @param {*} container 
     * @param {*} ops 
     */
    constructor(container, ops) {
        let that = this;

        ops = Object.assign({
            theme: 'light', // 主题 light(default), dark
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
        this.domContainer = typeof container == 'object' ? container : document.querySelector(container);
        this.domContainer.innerHTML = "";

        //渲染前回调
        if (typeof ops.viewbefore == "function") {
            ops.viewbefore.call(this)
        }

        //构建工具条
        let ulhtml = `<ul class="netnrmd-menu">${this.objToolbarIcons.map(x => netnrmd.toolbarBuildIcon(x)).join('\r\n')}</ul>`;
        this.domToolbar = netnrmd.createDom("div", "netnrmd-toolbar", ulhtml);

        //工具条加持命令响应
        this.domToolbar.firstChild.addEventListener('click', function (e) {
            let target = e.target;
            if (target.nodeName == "LI") {
                let cmdname = target.getAttribute('data-cmd');
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

        this._setHeight(ops.height);

        //载入编辑器
        this.domContainer.appendChild(this.domEditor);

        //new editor 
        if (window["monaco"] != null) {
            //初始化写
            this.objWrite = window["monaco"].editor.create(this.domWrite, {
                theme: ops.theme == 'dark' ? 'vs-dark' : 'vs',
                wordWrap: "on", language: 'markdown', fontSize: 17, automaticLayout: true,
                scrollbar: { verticalScrollbarSize: 13, horizontalScrollbarSize: 13 },
                unicodeHighlight: { ambiguousCharacters: false },
                scrollBeyondLastLine: false, minimap: { enabled: false }
            })

            //编辑器内容变动回调
            this.objWrite.onDidChangeModelContent(function () {
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
            this.objWrite.onDidScrollChange(function (sc) {
                let p = sc.scrollTop / (sc.scrollHeight - that.domWrite.clientHeight - 4);
                that.domView.scrollTop = (that.domView.scrollHeight - that.domView.clientHeight) * p;
            });

            //按键事件监听
            this.objToolbarIcons.forEach(item => {
                if (item.key) {
                    that.objWrite.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode[`Key${item.key.split('+').pop()}`], function () {
                        that.cmd(item.cmd);
                    })
                }
            });
            //header1-6
            [1, 2, 3, 4, 5, 6].forEach(h => {
                that.objWrite.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode[`Digit${h}`], function () {
                    that.cmd(`header${h}`);
                });

                that.objWrite.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode[`Numpad${h}`], function () {
                    that.cmd(`header${h}`);
                });
            });
        } else {
            console.debug("monaco is not defined");
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
                that._setHeight(document.documentElement.clientHeight);
            } else if (typeof that.objOptions.resize == "function") {
                that.objOptions.resize.call(that, document.documentElement.clientHeight);
            }
        });
        //初始化调用 resize
        if (typeof that.objOptions.resize == "function") {
            that.objOptions.resize.call(that, document.documentElement.clientHeight);
        }

        this.domContainer['netnrmd'] = this;
    }

    cmd(cmdname) {
        let ops = this.objOptions;

        //执行命令前回调
        if (typeof ops.cmdbefore == "function") {
            if (ops.cmdbefore.call(this, cmdname) == false) {
                return false;
            }
        }

        // 事件
        let itemIcon = this.objToolbarIcons.find(x => x.cmd == cmdname);
        if (itemIcon && itemIcon.action) {
            itemIcon.action(this);
        }

        let eobj = {
            before: '',
            dv: '',
            after: '',
            addrow: 0,
            //执行公共插入
            isdo: true
        }

        //选中的范围
        let txtRange = this.objWrite.getSelection();
        //选中的内容
        let text = this.objWrite.getModel().getValueInRange(txtRange);

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
                    let hn = cmdname.substring(6) * 1;
                    if (hn == 0) {
                        hn = 3;
                    }

                    while (hn-- > 0) {
                        eobj.before = "#" + eobj.before;
                    }

                    if (txtRange.startColumn > 1) {
                        //选中的前面内容
                        let textBefore = this.objWrite.getModel().getLineContent(txtRange.startLineNumber).substring(0, txtRange.startColumn - 1).trim();
                        if (["-", "+", "*"].filter(x => textBefore).length) {
                            eobj.addrow = 1;
                            eobj.before = "\r\n" + eobj.before;
                        }
                    }
                }
                break;
            case "quote":
                {
                    eobj.dv = '引用内容';
                    eobj.before = '> ';
                    if (txtRange.startColumn > 1) {
                        eobj.addrow = 1;
                        eobj.before = '\r\n> ';
                    }
                }
                break;
            case "list-ol":
                {
                    eobj.dv = '列表文本';
                    eobj.before = '1. ';
                    if (txtRange.startColumn > 1) {
                        eobj.addrow = 1;
                        eobj.before = '\r\n1. ';
                    }
                }
                break;
            case "list-ul":
                {
                    eobj.dv = '列表文本';
                    eobj.before = '- ';
                    if (txtRange.startColumn > 1) {
                        eobj.addrow = 1;
                        eobj.before = '\r\n- ';
                    }
                }
                break;
            case "checked":
                {
                    eobj.dv = '列表文本';
                    eobj.before = '- [x] ';
                    if (txtRange.startColumn > 1) {
                        eobj.addrow = 1;
                        eobj.before = '\r\n- [x] ';
                    }
                }
                break;
            case "unchecked":
                {
                    eobj.dv = '列表文本';
                    eobj.before = '- [ ] ';
                    if (txtRange.startColumn > 1) {
                        eobj.addrow = 1;
                        eobj.before = '\r\n- [ ] ';
                    }
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
                    let cols = 'col 1 | col 2 | col 3', hd = '--- | --- | ---', nl = '\r\n';
                    eobj.before = cols + nl + hd + nl + cols + nl + cols + nl + nl;

                    eobj.addrow = 5;
                    if (txtRange.startColumn > 1) {
                        eobj.addrow = 7;
                        eobj.before = "\r\n" + eobj.before;
                    }
                }
                break;
            case "code":
                {
                    if (txtRange.startColumn == 1) {
                        eobj.addrow = 1;
                        eobj.before = '```\r\n';
                        eobj.after = '\r\n```';
                        eobj.dv = '输入代码';
                    } else {
                        //选中的前面内容
                        let textBefore = this.objWrite.getModel().getLineContent(txtRange.startLineNumber).substring(0, txtRange.startColumn - 1);
                        eobj.before = (textBefore.slice(-1) == " " ? "" : " ") + '`';
                        //选中的后面内容
                        let textAfter = this.objWrite.getModel().getLineContent(txtRange.endLineNumber).substring(txtRange.endColumn - 1);
                        eobj.after = '`' + (textAfter.slice(0, 1) == " " ? "" : " ");
                        eobj.dv = '输入关键字';
                    }
                }
                break;
            case "line":
                {
                    eobj.before = '---';
                    if (txtRange.startColumn > 1) {
                        eobj.addrow = 1;
                        eobj.before = '\r\n---';
                    }
                }
                break;
            default:
                eobj.isdo = false;
                break;
        }

        if (eobj.isdo && cmdname && cmdname != "") {
            //未选中内容，插入模版
            if (text == "") {
                text = eobj.dv;

                //光标处插入
                this.insert(eobj.before + text + eobj.after);
            } else {
                //构建选中内容后的光标位置
                let afterRange = new monaco.Range(txtRange.endLineNumber, txtRange.endColumn, txtRange.endLineNumber, txtRange.endColumn);
                this.insert(eobj.after, afterRange);

                //构建选中内容前的光标位置                
                let beforeRange = new monaco.Range(txtRange.startLineNumber, txtRange.startColumn, txtRange.startLineNumber, txtRange.startColumn);
                this.insert(eobj.before, beforeRange);
            }

            this.focus();
            //光标选中
            txtRange.startColumn += eobj.before.length;
            txtRange.endColumn = txtRange.startColumn + text.length;
            if (eobj.addrow > 0) {
                txtRange.startLineNumber += eobj.addrow;
                txtRange.endLineNumber += eobj.addrow;
                txtRange.startColumn = eobj.before.length - 1;
                if (cmdname == "code") {
                    txtRange.startColumn = 1;
                }
                txtRange.endColumn += text.length;
            }
            this.objWrite.setSelection(new monaco.Selection(txtRange.startLineNumber, txtRange.startColumn, txtRange.endLineNumber, txtRange.endColumn));
        }
    }

    //焦点
    focus() {
        this.objWrite.focus();
    }

    /**
     * 插入文本
     * @param {*} text 内容
     * @param {*} range 默认为光标位置，可指定插入位置
     */
    insert(text, range) {
        let model = this.objWrite.getModel();

        //默认为光标位置
        if (range == null) {
            let cursorPosition = this.objWrite.getPosition();
            range = new monaco.Range(cursorPosition.lineNumber, cursorPosition.column, cursorPosition.lineNumber, cursorPosition.column);
        }

        let edits = [{ identifier: { major: 1, minor: 1 }, range: range, text: text, forceMoveMarkers: true }];
        model.pushEditOperations([], edits, function () {
            return null;
        });
    }

    //添加按键命令
    addCommand(keys, exec) {
        if (window["monaco"] != null) {
            //Ctrl+Alt+Shift+S to monaco
            let keybinding = 0;
            keys.split('+').forEach(key => {
                if (key == "Ctrl") {
                    keybinding |= monaco.KeyMod.CtrlCmd;
                } else if (/^[A-Z]$/i.test(key.toUpperCase())) {
                    keybinding |= monaco.KeyCode[`Key${key.toUpperCase()}`];
                } else if (/^[0-9]$/.test(key)) {
                    keybinding |= monaco.KeyCode[`Digit${key}`];
                } else if (key in monaco.KeyMod) {
                    keybinding |= monaco.KeyMod[key];
                } else if (key in monaco.KeyCode) {
                    keybinding |= monaco.KeyCode[key];
                }
            });

            this.objWrite.addCommand(keybinding, exec);
        }
    }

    //设置高度（内部使用）
    _setHeight(height) {
        let weh = (height - (this.domToolbar.style.display == "none" ? 0 : this.domToolbar.offsetHeight)) + "px";
        this.domWrite.style.height = weh;
        this.domView.style.height = weh;
        return this;
    }

    //获取或设置高度（非全屏模式时）
    height(height) {
        if (height != null) {
            this.objOptions.height = height;
            if (!this.objOptions.fullscreen) {
                this._setHeight(height);
            }
            return this;
        } else {
            return this.objOptions.height;
        }
    }

    //全屏切换
    toggleFullScreen(fullscreen) {
        let tt = this.getToolbarTarget('fullscreen');
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
            this._setHeight(document.documentElement.clientHeight);
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
    }

    //设置主题
    setTheme(theme) {
        this.objOptions.theme = theme;

        let domIcon = this.getToolbarTarget('theme');
        let itemIcon = netnrmd.toolbarIcons.find(x => x.cmd == "theme");
        let tooltip = `${itemIcon.title}/${itemIcon.cmd} ${(itemIcon.key ? itemIcon.key : '')}`;
        domIcon.title = `${tooltip} ${theme}`;

        let domHtml = document.documentElement;
        switch (theme) {
            case "dark":
                if (this.objWrite) {
                    monaco.editor.setTheme('vs-dark');
                }
                domHtml.classList.remove("netnrmd-light");
                domHtml.classList.add("netnrmd-dark");
                break;
            default:
                if (this.objWrite) {
                    monaco.editor.setTheme('vs');
                }
                domHtml.classList.remove("netnrmd-dark");
                domHtml.classList.add("netnrmd-light");
                break;
        }
    }

    /**
     * 主题切换
     * @param {*} theme 指定，默认循环切换
     */
    toggleTheme(theme) {
        if (theme == null) {
            theme = this.objOptions.theme == "dark" ? "light" : "dark";
        }
        this.setTheme(theme);
    }

    //根据命令获取工具条的对象
    getToolbarTarget(cmd) {
        let target;
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
            let pos = this.objWrite.getPosition();
            this.objWrite.executeEdits('', [{
                range: this.objWrite.getModel().getFullModelRange(),
                text: md
            }]);
            this.objWrite.setSelection(new monaco.Range(0, 0, 0, 0));
            this.objWrite.setPosition(pos);
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
            let menus = this.domMarkdownBody.querySelectorAll("h1,h2,h3,h4.h5,h6");
            for (let i = 0; i < menus.length; i++) {
                let item = menus[i];
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
        this.focus();
    }

    //渲染
    render() {
        let that = this;
        clearTimeout(this.objOptions.deferIndex);
        this.objOptions.deferIndex = setTimeout(function () {
            let md = that.getmd();
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
        let md = localStorage[this.objOptions.storekey];
        if (md != null) {
            this.setmd(md);
            this.render(); // 初始化时，赋值未触发变更事件
        }
    }

    //保存
    async save(format, filename) {
        let that = this;
        let oldvm = this.objOptions.viewmodel;
        let nmdBg = getComputedStyle(that.domMarkdownBody).getPropertyValue('--nmd-bg').trim();

        format = format.toLowerCase();
        if (format == "markdown") {
            format = "md";
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
                {
                    let htmlContent = this.gethtml();
                    let mdcss = netnrmd.mirrorNPM(netnrmd.tsRely.markdownStyle);
                    let styleContent = await (await fetch(mdcss)).text();

                    let htmls = ['<!DOCTYPE html>',
                        '<html><head>',
                        '<meta name="viewport" content="width=device-width, initial-scale=1.0" /><meta charset="utf-8" />',
                        '<style type="text/css">' + styleContent + '</style>',
                        '</head><body>',
                        '<div class="markdown-body">' + htmlContent + '</div>',
                        '</body></html>'
                    ];
                    let result = htmls.join('\r\n');
                    netnrmd.download(result, filename);
                }
                break;
            case "png":
            case "jpg":
            case "pdf":
                {
                    if (oldvm == 1) {
                        this.toggleView(2);
                    }

                    that.domMarkdownBody.style.padding = "2em 2em 4em";

                    if (window["define"] == null) {
                        await netnrmd.importScript(netnrmd.tsRely.html2canvas);
                        if (format == "pdf") {
                            await netnrmd.importScript(netnrmd.tsRely.jspdf);
                        }
                    } else {
                        await netnrmd.require([netnrmd.tsRely.html2canvas], "html2canvas");
                        if (format == "pdf") {
                            await netnrmd.require([netnrmd.tsRely.jspdf], "jspdf");
                        }
                    }

                    //分批截图
                    let totalHeight = that.domMarkdownBody.scrollHeight;
                    let partsCount = Math.ceil(totalHeight / netnrmd.tsScreenshotHeight);
                    console.debug(`Total height ${totalHeight} , Total ${partsCount} images`);
                    let arrIndex = [];
                    while (partsCount > 0) {
                        arrIndex.unshift(partsCount--);
                    }
                    for (const index of arrIndex) {
                        let y = (index - 1) * netnrmd.tsScreenshotHeight;
                        let height = Math.min(netnrmd.tsScreenshotHeight, totalHeight - y);

                        let canvas = await html2canvas(that.domMarkdownBody, {
                            scale: format == "pdf" ? 3 : 2,
                            backgroundColor: nmdBg,
                            y: y,
                            useCORS: true,
                            logging: false,
                            height: height
                        });

                        let newname = arrIndex.length > 1 ? filename.replace(".", `_${index}.`) : filename;
                        if (format == "pdf") {
                            let a4w = 595.28;
                            let a4h = 841.89;

                            let pageHeight = (canvas.width / a4w) * a4h;
                            let leftHeight = canvas.height;
                            let position = 0;
                            let imgHeight = (a4w / canvas.width) * canvas.height;
                            let pageData = canvas.toDataURL('image/jpeg', 0.92);
                            let pdf = new window["jspdf"].jsPDF('', 'pt', 'a4');
                            if (leftHeight < pageHeight) {
                                pdf.addImage(pageData, 'JPEG', 0, 0, a4w, imgHeight);
                            } else {
                                while (leftHeight > 0) {
                                    pdf.addImage(pageData, 'JPEG', 0, position, a4w, imgHeight);
                                    leftHeight -= pageHeight;
                                    position -= a4h;
                                    if (leftHeight > 0) {
                                        pdf.addPage();
                                    }
                                }
                            }
                            pdf.save(newname);
                        } else {
                            netnrmd.download(canvas, newname);
                        }

                        if (arrIndex.length > 1) {
                            console.debug(`progress ${index}/${arrIndex.length}`);
                        }
                    }
                    that.domMarkdownBody.style.removeProperty("padding");

                    if (oldvm == 1) {
                        this.toggleView(1);
                    }
                }
                break;
            default:
                console.debug("unsupported format");
                break;
        }
    }
}

export { netnrmdInit };
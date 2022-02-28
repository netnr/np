/*                                                                                          *\
 *  Monaco Editor 编辑器 + Marked 解析 + DOMPurify 清洗 + highlight 代码高亮 + pangu 间隙
 *  Author：netnr
 *                                                                                          */
(function (window) {

    var netnrmd = function (id, obj) { return new netnrmd.fn.init(id, obj) }

    netnrmd.fn = netnrmd.prototype = {

        init: function (id, obj) {
            var that = this;
            obj = obj || {};


            //预览提示文字
            obj.ph = netnrmd.dv(obj.ph, "预览区域");

            //Monaco Editor容器
            obj.mebox = document.querySelector(id);
            obj.mebox.innerHTML = "";

            //编辑器父容器
            obj.container = obj.mebox.parentNode;

            //编辑器字体大小
            obj.fontsize = netnrmd.dv(obj.fontsize, 16);

            //解析延迟毫秒
            obj.defer = netnrmd.dv(obj.defer, 500);

            //工具条项
            obj.items = [
                { title: '表情', cmd: 'emoji', svg: '<path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/><path d="M4.285 9.567a.5.5 0 0 1 .683.183A3.498 3.498 0 0 0 8 11.5a3.498 3.498 0 0 0 3.032-1.75.5.5 0 1 1 .866.5A4.498 4.498 0 0 1 8 12.5a4.498 4.498 0 0 1-3.898-2.25.5.5 0 0 1 .183-.683zM7 6.5C7 7.328 6.552 8 6 8s-1-.672-1-1.5S5.448 5 6 5s1 .672 1 1.5zm4 0c0 .828-.448 1.5-1 1.5s-1-.672-1-1.5S9.448 5 10 5s1 .672 1 1.5z"/>' },
                { title: '粗体', cmd: 'bold', key: 'Ctrl+B', svg: '<path d="M8.21 13c2.106 0 3.412-1.087 3.412-2.823 0-1.306-.984-2.283-2.324-2.386v-.055a2.176 2.176 0 0 0 1.852-2.14c0-1.51-1.162-2.46-3.014-2.46H3.843V13H8.21zM5.908 4.674h1.696c.963 0 1.517.451 1.517 1.244 0 .834-.629 1.32-1.73 1.32H5.908V4.673zm0 6.788V8.598h1.73c1.217 0 1.88.492 1.88 1.415 0 .943-.643 1.449-1.832 1.449H5.907z"/>' },
                { title: '斜体', cmd: 'italic', key: 'Ctrl+I', svg: '<path d="M7.991 11.674 9.53 4.455c.123-.595.246-.71 1.347-.807l.11-.52H7.211l-.11.52c1.06.096 1.128.212 1.005.807L6.57 11.674c-.123.595-.246.71-1.346.806l-.11.52h3.774l.11-.52c-1.06-.095-1.129-.211-1.006-.806z"/>' },
                { title: '删除', cmd: 'strikethrough', key: 'Ctrl+D', svg: '<path d="M6.333 5.686c0 .31.083.581.27.814H5.166a2.776 2.776 0 0 1-.099-.76c0-1.627 1.436-2.768 3.48-2.768 1.969 0 3.39 1.175 3.445 2.85h-1.23c-.11-1.08-.964-1.743-2.25-1.743-1.23 0-2.18.602-2.18 1.607zm2.194 7.478c-2.153 0-3.589-1.107-3.705-2.81h1.23c.144 1.06 1.129 1.703 2.544 1.703 1.34 0 2.31-.705 2.31-1.675 0-.827-.547-1.374-1.914-1.675L8.046 8.5H1v-1h14v1h-3.504c.468.437.675.994.675 1.697 0 1.826-1.436 2.967-3.644 2.967z"/>' },
                { title: '标题', cmd: 'header', key: 'Ctrl+H', svg: '<path d="M7.637 13V3.669H6.379V7.62H1.758V3.67H.5V13h1.258V8.728h4.62V13h1.259zm3.625-4.272h1.018c1.142 0 1.935.67 1.949 1.674.013 1.005-.78 1.737-2.01 1.73-1.08-.007-1.853-.588-1.935-1.32H9.108c.069 1.327 1.224 2.386 3.083 2.386 1.935 0 3.343-1.155 3.309-2.789-.027-1.51-1.251-2.16-2.037-2.249v-.068c.704-.123 1.764-.91 1.723-2.229-.035-1.353-1.176-2.4-2.954-2.385-1.873.006-2.857 1.162-2.898 2.358h1.196c.062-.69.711-1.299 1.696-1.299.998 0 1.695.622 1.695 1.525.007.922-.718 1.592-1.695 1.592h-.964v1.074z"/>' },
                { title: '引用', cmd: 'quote', key: 'Ctrl+Q', svg: '<path d="M2.5 3a.5.5 0 0 0 0 1h11a.5.5 0 0 0 0-1h-11zm5 3a.5.5 0 0 0 0 1h6a.5.5 0 0 0 0-1h-6zm0 3a.5.5 0 0 0 0 1h6a.5.5 0 0 0 0-1h-6zm-5 3a.5.5 0 0 0 0 1h11a.5.5 0 0 0 0-1h-11zm.79-5.373c.112-.078.26-.17.444-.275L3.524 6c-.122.074-.272.17-.452.287-.18.117-.35.26-.51.428a2.425 2.425 0 0 0-.398.562c-.11.207-.164.438-.164.692 0 .36.072.65.217.873.144.219.385.328.72.328.215 0 .383-.07.504-.211a.697.697 0 0 0 .188-.463c0-.23-.07-.404-.211-.521-.137-.121-.326-.182-.568-.182h-.282c.024-.203.065-.37.123-.498a1.38 1.38 0 0 1 .252-.37 1.94 1.94 0 0 1 .346-.298zm2.167 0c.113-.078.262-.17.445-.275L5.692 6c-.122.074-.272.17-.452.287-.18.117-.35.26-.51.428a2.425 2.425 0 0 0-.398.562c-.11.207-.164.438-.164.692 0 .36.072.65.217.873.144.219.385.328.72.328.215 0 .383-.07.504-.211a.697.697 0 0 0 .188-.463c0-.23-.07-.404-.211-.521-.137-.121-.326-.182-.568-.182h-.282a1.75 1.75 0 0 1 .118-.492c.058-.13.144-.254.257-.375a1.94 1.94 0 0 1 .346-.3z"/>' },
                { title: '有序列表', cmd: 'list-ol', key: 'Ctrl+O', svg: '<path fill-rule="evenodd" d="M5 11.5a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5z"/><path d="M1.713 11.865v-.474H2c.217 0 .363-.137.363-.317 0-.185-.158-.31-.361-.31-.223 0-.367.152-.373.31h-.59c.016-.467.373-.787.986-.787.588-.002.954.291.957.703a.595.595 0 0 1-.492.594v.033a.615.615 0 0 1 .569.631c.003.533-.502.8-1.051.8-.656 0-1-.37-1.008-.794h.582c.008.178.186.306.422.309.254 0 .424-.145.422-.35-.002-.195-.155-.348-.414-.348h-.3zm-.004-4.699h-.604v-.035c0-.408.295-.844.958-.844.583 0 .96.326.96.756 0 .389-.257.617-.476.848l-.537.572v.03h1.054V9H1.143v-.395l.957-.99c.138-.142.293-.304.293-.508 0-.18-.147-.32-.342-.32a.33.33 0 0 0-.342.338v.041zM2.564 5h-.635V2.924h-.031l-.598.42v-.567l.629-.443h.635V5z"/>' },
                { title: '无序列表', cmd: 'list-ul', key: 'Ctrl+U', svg: '<path fill-rule="evenodd" d="M5 11.5a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm-3 1a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2z"/>' },
                { title: '选中', cmd: 'checked', key: 'Alt+Y', svg: '<path d="M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z"/><path d="M10.97 4.97a.75.75 0 0 1 1.071 1.05l-3.992 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425a.235.235 0 0 1 .02-.022z"/>' },
                { title: '未选中', cmd: 'unchecked', key: 'Alt+N', svg: '<path d="M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z"/>' },
                { title: '链接', cmd: 'link', key: 'Ctrl+L', svg: '<path d="M4.715 6.542 3.343 7.914a3 3 0 1 0 4.243 4.243l1.828-1.829A3 3 0 0 0 8.586 5.5L8 6.086a1.002 1.002 0 0 0-.154.199 2 2 0 0 1 .861 3.337L6.88 11.45a2 2 0 1 1-2.83-2.83l.793-.792a4.018 4.018 0 0 1-.128-1.287z"/><path d="M6.586 4.672A3 3 0 0 0 7.414 9.5l.775-.776a2 2 0 0 1-.896-3.346L9.12 3.55a2 2 0 1 1 2.83 2.83l-.793.792c.112.42.155.855.128 1.287l1.372-1.372a3 3 0 1 0-4.243-4.243L6.586 4.672z"/>' },
                { title: '图片', cmd: 'image', key: 'Ctrl+G', svg: '<path d="M6.002 5.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z"/><path d="M1.5 2A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h13a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-13zm13 1a.5.5 0 0 1 .5.5v6l-3.775-1.947a.5.5 0 0 0-.577.093l-3.71 3.71-2.66-1.772a.5.5 0 0 0-.63.062L1.002 12v.54A.505.505 0 0 1 1 12.5v-9a.5.5 0 0 1 .5-.5h13z"/>' },
                { title: '表格', cmd: 'table', key: 'Alt+T', svg: '<path d="M0 2a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V2zm15 2h-4v3h4V4zm0 4h-4v3h4V8zm0 4h-4v3h3a1 1 0 0 0 1-1v-2zm-5 3v-3H6v3h4zm-5 0v-3H1v2a1 1 0 0 0 1 1h3zm-4-4h4V8H1v3zm0-4h4V4H1v3zm5-3v3h4V4H6zm4 4H6v3h4V8z"/>' },
                { title: '代码', cmd: 'code', key: 'Ctrl+K', svg: '<path d="M10.478 1.647a.5.5 0 1 0-.956-.294l-4 13a.5.5 0 0 0 .956.294l4-13zM4.854 4.146a.5.5 0 0 1 0 .708L1.707 8l3.147 3.146a.5.5 0 0 1-.708.708l-3.5-3.5a.5.5 0 0 1 0-.708l3.5-3.5a.5.5 0 0 1 .708 0zm6.292 0a.5.5 0 0 0 0 .708L14.293 8l-3.147 3.146a.5.5 0 0 0 .708.708l3.5-3.5a.5.5 0 0 0 0-.708l-3.5-3.5a.5.5 0 0 0-.708 0z"/>' },
                { title: '分隔线', cmd: 'line', key: 'Ctrl+R', svg: '<path d="M2 8a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>' },
                { title: '间隙', cmd: 'spacing', key: 'Alt+S', svg: '<path fill-rule="evenodd" d="M14.5 1a.5.5 0 0 0-.5.5v13a.5.5 0 0 0 1 0v-13a.5.5 0 0 0-.5-.5zm-13 0a.5.5 0 0 0-.5.5v13a.5.5 0 0 0 1 0v-13a.5.5 0 0 0-.5-.5z"/><path d="M6 13a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1v10z"/>' },
                { title: '全屏', cmd: 'fullscreen', key: 'Ctrl+M', class: 'float-right', svg: '<path fill-rule="evenodd" d="M5.828 10.172a.5.5 0 0 0-.707 0l-4.096 4.096V11.5a.5.5 0 0 0-1 0v3.975a.5.5 0 0 0 .5.5H4.5a.5.5 0 0 0 0-1H1.732l4.096-4.096a.5.5 0 0 0 0-.707zm4.344 0a.5.5 0 0 1 .707 0l4.096 4.096V11.5a.5.5 0 1 1 1 0v3.975a.5.5 0 0 1-.5.5H11.5a.5.5 0 0 1 0-1h2.768l-4.096-4.096a.5.5 0 0 1 0-.707zm0-4.344a.5.5 0 0 0 .707 0l4.096-4.096V4.5a.5.5 0 1 0 1 0V.525a.5.5 0 0 0-.5-.5H11.5a.5.5 0 0 0 0 1h2.768l-4.096 4.096a.5.5 0 0 0 0 .707zm-4.344 0a.5.5 0 0 1-.707 0L1.025 1.732V4.5a.5.5 0 0 1-1 0V.525a.5.5 0 0 1 .5-.5H4.5a.5.5 0 0 1 0 1H1.732l4.096 4.096a.5.5 0 0 1 0 .707z"/>' },
                { title: '分屏', cmd: 'splitscreen', class: 'float-right', svg: '<path d="M0 3a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V3zm8.5-1v12H14a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H8.5zm-1 0H2a1 1 0 0 0-1 1v10a1 1 0 0 0 1 1h5.5V2z"/>' }
            ];

            //渲染前回调
            if (typeof obj.viewbefore == "function") {
                obj.viewbefore.call(obj)
            }

            //工具条
            var lis = [];
            obj.items.forEach(item => {
                var keytip = item.title + "/" + item.cmd + " " + (item.key ? item.key : '');
                lis.push('<li class="' + (item.class || "") + '" data-cmd="' + item.cmd + '" title="' + keytip + '">');
                lis.push('<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16">' + item.svg + '</svg>');
                lis.push('</li>');
            });

            //构建工具条
            obj.toolbar = netnrmd.createDom("div", "netnrmd-toolbar", '<ul class="netnrmd-menu">' + lis.join('') + '</ul>');

            //工具条加持命令响应
            obj.toolbar.firstChild.addEventListener('click', function (e) {
                var target = e.target;
                if (target.nodeName == "LI") {
                    var cmdname = target.getAttribute('data-cmd');
                    //执行命令
                    netnrmd.cmd(cmdname, that);
                }
            }, false);

            //写
            obj.write = netnrmd.createDom("div", "netnrmd-write");
            obj.write.appendChild(obj.mebox);

            //视图
            obj.view = netnrmd.createDom("div", "markdown-body netnrmd-view");

            //编辑器
            obj.editor = netnrmd.createDom("div", "netnrmd");
            obj.editor.appendChild(obj.toolbar);
            obj.editor.appendChild(obj.write);
            obj.editor.appendChild(obj.view);

            //载入编辑器
            obj.container.appendChild(obj.editor);

            //Monaco Editor对象
            obj.me = monaco.editor.create(obj.mebox, {
                language: 'markdown',
                scrollBeyondLastLine: false,
                automaticLayout: true,
                wordWrap: "on",
                fontSize: obj.fontsize,
                minimap: { enabled: false }
            });

            //编辑器内容变动回调
            obj.me.onDidChangeModelContent(function () {

                if (typeof obj.input == "function" && obj.input.call(that) == false) {
                    return false;
                }

                //自动保存
                if (obj.autosave) {
                    that.setstore();
                }

                //渲染
                that.render();
            });

            //滚动条同步
            obj.me.onDidScrollChange(function (sc) {
                var hratio = sc.scrollTop / (sc.scrollHeight - obj.mebox.clientHeight - 4);
                obj.view.scrollTop = (obj.view.scrollHeight - obj.view.clientHeight) * hratio;
            });

            //按键事件监听
            obj.items.forEach(item => {
                if (item.key) {
                    var ks = item.key.split('+');
                    var km = ks[0] == "Ctrl" ? monaco.KeyMod.CtrlCmd : monaco.KeyMod[ks[0]];
                    var kc = monaco.KeyCode["KEY_" + ks[1]] || monaco.KeyCode["Key" + ks[1]];
                    obj.me.addCommand(km | kc, function () {
                        //执行命令
                        netnrmd.cmd(item.cmd, that);
                    })
                }
            });
            //header1-6
            [1, 2, 3, 4, 5, 6].forEach(h => {
                var kc = monaco.KeyCode["KEY_" + h] || monaco.KeyCode["Key" + h];
                obj.me.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyMod.Shift | kc, function () {
                    netnrmd.cmd("header" + h, that);
                })
            })

            this.obj = obj;

            //初始化响应配置

            //视图模式：1输入|2分屏|3预览
            this.toggleView(obj.viewmodel = netnrmd.dv(obj.viewmodel, 2));

            //高度
            this.height(obj.height = netnrmd.dv(obj.height, 250));

            //全屏：1
            this.toggleFullScreen(obj.fullscreen = netnrmd.dv(obj.fullscreen, false));
            window.addEventListener('resize', function () {
                if (obj.fullscreen) {
                    that.setHeight(document.documentElement.clientHeight);
                }
            }, false);

            //本地保存键
            obj.storekey = netnrmd.dv(obj.storekey, location.pathname + "_netnrmd_markdown");

            //本地自动保存
            obj.autosave = netnrmd.dv(obj.autosave, true);

            //载入本地保存
            if (obj.autosave > 0) {
                this.getstore();
            }

            obj.mebox['netnrmd'] = this;
            return this;
        },

        //获取焦点
        focus: function () {
            this.obj.me.focus();
            return this;
        },

        //设置高度
        setHeight: function (height) {
            var weh = (height - (this.obj.toolbar.style.display == "none" ? 0 : this.obj.toolbar.offsetHeight)) + "px";
            this.obj.write.style.height = weh;
            this.obj.mebox.style.height = weh;
            this.obj.view.style.height = weh;
        },

        //设置高度（非全屏模式时）
        height: function (height) {
            if (height != null) {
                this.obj.height = height;
                if (!this.obj.fullscreen) {
                    this.setHeight(height)
                }
                return this;
            } else {
                return this.obj.height;
            }
        },

        //全屏切换
        toggleFullScreen: function (fullscreen) {
            var obj = this.obj, tit = this.getToolItemTarget('fullscreen');
            obj.fullscreen = !obj.fullscreen;
            if (fullscreen != null) {
                obj.fullscreen = fullscreen;
            }
            if (!obj.fullscreen) {
                obj.editor.classList.remove('netnrmd-fullscreen');
                tit.classList.remove('active');
                this.height(obj.height, true);
            } else {
                obj.editor.classList.add('netnrmd-fullscreen');
                tit.classList.add('active');
                this.setHeight(document.documentElement.clientHeight);
            }
        },

        //分屏切换
        toggleSplitScreen: function (splitscreen) {
            var obj = this.obj;
            obj.splitscreen = !obj.splitscreen;
            if (splitscreen != null) {
                obj.splitscreen = splitscreen;
            }
            if (!obj.splitscreen) {
                this.togglePreview(0);

                obj.write.classList.add('netnrmd-write-w100');
                obj.view.classList.add('netnrmd-view-hidden');
            } else {
                obj.write.classList.remove('netnrmd-write-w100');
                obj.view.classList.remove('netnrmd-view-hidden');
            }
            setTimeout(function () {
                obj.me.layout()
            }, 1)
        },

        //预览切换
        togglePreview: function (preview) {
            var obj = this.obj;
            obj.preview = !obj.preview;
            if (preview != null) {
                obj.preview = preview;
            }
            if (obj.preview) {
                this.toggleSplitScreen(1);
                obj.write.classList.add('netnrmd-write-hidden');
                obj.view.classList.add('netnrmd-view-w100');
            } else {
                obj.write.classList.remove('netnrmd-write-hidden');
                obj.view.classList.remove('netnrmd-view-w100');
            }
            setTimeout(function () {
                obj.me.layout()
            }, 1)
        },

        //视图切换
        toggleView: function (n) {
            if (n == null) {
                n = this.obj.viewmodel - 1;
                if (n < 1) {
                    n = 3;
                }
            }
            this.obj.viewmodel = n;
            switch (n) {
                case 1:
                    this.togglePreview(0);
                    this.toggleSplitScreen(0);
                    break;
                case 2:
                    this.togglePreview(0);
                    this.toggleSplitScreen(1);
                    break;
                case 3:
                    this.toggleSplitScreen(0);
                    this.togglePreview(1);
                    break;
            }
        },

        //添加间隙
        spacing: function () {
            netnrmd.keepSetValue(this.obj.me, netnrmd.spacing(this.obj.me.getValue()));
            return this;
        },

        //根据命令获取工具条的对象
        getToolItemTarget: function (cmd) {
            var target;
            this.obj.toolbar.querySelectorAll('li').forEach(item => {
                if (item.getAttribute('data-cmd') == cmd) {
                    target = item;
                }
            })
            return target;
        },

        //获取光标行前面的内容
        getLineContentPosPrev: function () {
            var ms = this.obj.me.getSelection();
            var lcpp = this.obj.me.getModel().getLineContent(ms.startLineNumber).substring(0, ms.startColumn - 1).trim();
            return lcpp;
        },

        //赋值md
        setmd: function (md) {
            netnrmd.keepSetValue(this.obj.me, md);
            return this;
        },

        //获取md
        getmd: function () {
            return this.obj.me.getValue();
        },

        //呈现html
        sethtml: function (html) {
            this.obj.view.innerHTML = html;
            return this;
        },

        //获取html
        gethtml: function () {
            return this.obj.view.innerHTML;
        },

        //渲染
        render: function () {
            var that = this;
            clearTimeout(that.obj.deferIndex);
            that.obj.deferIndex = setTimeout(function () {
                var md = that.getmd();
                if (md == "") {
                    //清理html、本地缓存
                    that.sethtml('<div class="netnrmd-view-empty">' + that.obj.ph + '</div>');
                    that.setstore();
                } else {
                    that.sethtml(netnrmd.render(md));
                }
            }, that.obj.defer);
        },

        //隐藏
        hide: function (area) {
            switch (area) {
                case "toolbar":
                    this.obj.toolbar.style.display = "none";
                    break;
                default:
                    this.obj.editor.style.display = "none";
            }
        },

        //显示
        show: function (area) {
            switch (area) {
                case "toolbar":
                    this.obj.toolbar.style.display = "";
                    break;
                default:
                    this.obj.editor.style.display = "";
            }
        },

        //写入本地保存
        setstore: function () {
            localStorage[this.obj.storekey] = this.getmd();
        },

        //获取本地保存
        getstore: function () {
            var md = localStorage[this.obj.storekey]
            if (md) {
                this.setmd(md);
                this.render();
            }
        }
    }

    netnrmd.fn.init.prototype = netnrmd.fn;

    /**
     * 创建节点
     * @param {any} domName 节点名称
     * @param {any} className 类样式名
     * @param {any} html
     */
    netnrmd.createDom = function (domName, className, html) {
        var dom = document.createElement(domName);
        if (className != null) {
            dom.className = className;
        }
        if (html != null) {
            dom.innerHTML = html;
        }
        return dom;
    };

    /**
     * 命令
     * @param {any} cmdname 命名名称
     * @param {any} that netnrmd创建的对象
     */
    netnrmd.cmd = function (cmdname, that) {

        var obj = that.obj;

        //执行命令前回调
        if (typeof obj.cmdcallback == "function") {
            if (obj.cmdcallback.call(that, cmdname) == false) {
                return false;
            }
        }

        //允许响应命令
        if (obj.preview && "help,preview,splitscreen,fullscreen".indexOf(cmdname) == -1) {
            return false;
        }

        var ops = {
            me: obj.me,
            cmd: cmdname,
            txt: obj.mebox,
            before: '',
            dv: '',
            after: '',
            //执行公共插入
            isdo: true
        }
        switch (cmdname) {
            case "emoji":
                {
                    if (!that.emojipopup) {
                        //构建弹出内容
                        var htm = [];
                        htm.push("<div class='netnrmd-emoji'>")
                        for (var eh in netnrmd.emoji) {
                            htm.push('<b>' + eh + '</b><ul>');
                            netnrmd.emoji[eh].split(' ').forEach(ei => {
                                htm.push('<li>' + ei + '</li>');
                            })
                            htm.push('</ul>');
                        }
                        htm.push("<div>")
                        //弹出
                        that.emojipopup = netnrmd.popup("表情（按住 Ctrl 连选）", htm.join(''));
                        //选择表情
                        that.emojipopup.addEventListener('click', function (e) {
                            var target = e.target;
                            if (target.nodeName == "LI") {
                                netnrmd.insertAfterText(that.obj.me, target.innerHTML);
                                if (!e.ctrlKey) {
                                    this.style.display = 'none';
                                }
                            }
                        }, false)
                    } else {
                        that.emojipopup.style.display = '';
                    }
                }
                break;
            case "bold":
                ops.before = '**';
                ops.dv = '粗体';
                ops.after = '**';
                break;
            case "italic":
                ops.before = '_';
                ops.dv = '斜体';
                ops.after = '_';
                break;
            case "strikethrough":
                ops.before = '~~';
                ops.dv = '删除';
                ops.after = '~~';
                break;
            case "header":
            case "header1":
            case "header2":
            case "header3":
            case "header4":
            case "header5":
            case "header6":
                {
                    ops.dv = '标题';
                    ops.before = ' ';
                    var hn = cmdname.substring(6) * 1;
                    if (hn == 0) {
                        hn = 3;
                    }

                    while (hn-- > 0) {
                        ops.before = "#" + ops.before;
                    }

                    var lcpp = that.getLineContentPosPrev();
                    if (lcpp != "" && ["-", "+", "*"].indexOf(lcpp) == -1 && !/^\d+.$/.test(lcpp)) {
                        ops.before = "\n" + ops.before;
                    }
                }
                break;
            case "quote":
                ops.before = '> ';
                break;
            case "list-ol":
                {
                    ops.before = '1. ';
                    ops.dv = '列表文本';

                    var lcpp = that.getLineContentPosPrev();
                    if (lcpp != "") {
                        ops.before = "\n" + ops.before;
                    }
                }
                break;
            case "list-ul":
                {
                    ops.before = '- ';
                    ops.dv = '列表文本';

                    var lcpp = that.getLineContentPosPrev();
                    if (lcpp != "") {
                        ops.before = "\n" + ops.before;
                    }
                }
                break;
            case "checked":
                {
                    ops.before = '- [x] ';
                    ops.dv = '列表文本';

                    var lcpp = that.getLineContentPosPrev();
                    if (lcpp != "") {
                        ops.before = "\n" + ops.before;
                    }
                }
                break;
            case "unchecked":
                {
                    ops.before = '- [ ] ';
                    ops.dv = '列表文本';

                    var lcpp = that.getLineContentPosPrev();
                    if (lcpp != "") {
                        ops.before = "\n" + ops.before;
                    }
                }
                break;
            case "link":
                ops.before = '[链接说明](';
                ops.dv = 'https://';
                ops.after = ')';
                break;
            case "image":
                ops.before = '![图片说明](';
                ops.dv = 'https://';
                ops.after = ')';
                break;
            case "table":
                {
                    var cols = ' col 1 | col 2 | col 3 ', hd = ' ---- | ---- | ---- ', nl = '\r\n';
                    ops.before = cols + nl + hd + nl + cols + nl + cols + nl + nl;

                    var lcpp = that.getLineContentPosPrev();
                    if (lcpp != "") {
                        ops.before = "\n\n" + ops.before;
                    }
                }
                break;
            case "code":
                {
                    if (obj.me.getSelection().startColumn == 1) {
                        ops.before = '```\n';
                        ops.after = '\n```';
                    } else {
                        ops.before = '`';
                        ops.after = '`';
                    }
                    ops.dv = '输入代码';
                }
                break;
            case "line":
                {
                    if (obj.me.getSelection().startColumn == 1) {
                        ops.before = '---\r\n';
                    } else {
                        ops.before = '\n---\r\n';
                    }
                }
                break;
            case "spacing":
                ops.isdo = false;
                obj.mebox['netnrmd'].spacing();
                break;
            case "fullscreen":
                ops.isdo = false;
                obj.mebox['netnrmd'].toggleFullScreen();
                break;
            case "splitscreen":
                ops.isdo = false;
                obj.mebox['netnrmd'].toggleView();
                break;
            default:
                ops.isdo = false;
                break;
        }

        if (ops.isdo && ops.cmd && ops.cmd != "") {
            var before = ops.before, dv = ops.dv, after = ops.after;
            var gse = ops.me.getSelection();
            var text = netnrmd.getSelectText(ops.me);
            if (text.join('').trim() == "") {
                text = dv;
            } else {
                text = text.join('\n');
            }

            netnrmd.insertAfterText(ops.me, before + text + after);

            var startPos = gse.startColumn + before.length,
                endPos = startPos + text.length,
                startLine = gse.startLineNumber,
                addline = before.split('\n').length - 1;
            //有换行时选择下一行
            if (addline) {
                startLine += addline;
                startPos = 0;
                endPos = 99;
            }
            netnrmd.setSelectText(ops.me, startLine, startPos, startLine, endPos);

            //编辑器内容变动回调
            var that = ops.txt['netnrmd'], obj = that.obj;
            if (typeof obj.input == "function" && obj.input.call(that) == false) {
                return false;
            }
            that.render();
        }
    };

    /**
     * 默认值
     * @param {any} obj 对象
     * @param {any} v 默认值
     */
    netnrmd.dv = function (obj, v) {
        return (obj == null || obj == undefined) ? v : obj;
    }

    /**
     * 解析Markdown
     * @param {any} md
     */
    netnrmd.render = function (md) {
        return DOMPurify.sanitize(marked.parse(md, {
            headerIds: false,
            highlight: function (str, lang) {
                if (window.hljs) {
                    try {
                        return hljs.getLanguage(lang)
                            ? hljs.highlight(str, { language: lang }).value
                            : hljs.highlightAuto(str).value;
                    } catch (__) { console.log(__) }
                }
                return str;
            }
        }))
    }

    /**
     * 获取选中文字
     * @param {any} me Monaco Editor 对象
     */
    netnrmd.getSelectText = function (me) {
        var gse = me.getSelection(), gm = me.getModel(), rows = [];
        if (gse.startLineNumber == gse.endLineNumber) {
            var row = gm.getLineContent(gse.startLineNumber);
            row = row.substring(gse.startColumn - 1, gse.endColumn - 1);
            rows.push(row)
        } else {
            for (var i = gse.startLineNumber; i <= gse.endLineNumber; i++) {
                var row = gm.getLineContent(i);
                if (i == gse.startLineNumber) {
                    row = row.substring(gse.startColumn - 1);
                }
                if (i == gse.endLineNumber) {
                    row = row.substring(0, gse.endColumn - 1);
                }
                rows.push(row);
            }
        }
        return rows;
    }

    /**
     * 选中特定范围的文本
     * @param {any} me Monaco Editor 对象
     * @param {any} startRow 开始行
     * @param {any} startPos 开始列
     * @param {any} endRow 结束行
     * @param {any} endPos 结束列
     */
    netnrmd.setSelectText = function (me, startRow, startPos, endRow, endPos) {
        me.setSelection(new monaco.Selection(startRow, startPos, endRow, endPos));
        me.focus();
    }

    /**
     * 在光标后插入文本
     * @param {any} me Monaco Editor 对象
     * @param {any} text 文本
     */
    netnrmd.insertAfterText = function (me, text) {
        var gse = me.getSelection();
        var range = new monaco.Range(gse.startLineNumber, gse.startColumn, gse.endLineNumber, gse.endColumn);
        var op = { identifier: { major: 1, minor: 1 }, range: range, text: text, forceMoveMarkers: true };
        me.executeEdits('', [op]);
        me.focus();
    }

    /**
     * 保留赋值（可撤回）
     * @param {any} me
     * @param {any} text
     */
    netnrmd.keepSetValue = function (me, text) {
        var cpos = me.getPosition();
        me.executeEdits('', [{
            range: me.getModel().getFullModelRange(),
            text: text
        }]);
        me.setSelection(new monaco.Range(0, 0, 0, 0));
        me.setPosition(cpos);
    }

    /**
     * 文字、数字、符号、英文添加空格间隙
     * @param {any} text
     */
    netnrmd.spacing = function (text) {
        return pangu.spacing(text);
    }

    /**
     * 弹窗
     * @param {any} title 标题
     * @param {any} content 内容主体
     */
    netnrmd.popup = function (title, content) {
        var pp = document.createElement("div");

        var htm = [];
        htm.push('<div class="np-card">');
        htm.push('<div class="np-header">');
        htm.push('<a class="np-close" href="javascript:void(0);" title="关闭">✖</a>');
        htm.push('<span>' + title + '</span></div>');
        htm.push('<div class="np-body">' + content + '</div>');
        htm.push('</div>');

        var pp = netnrmd.createDom('div', 'netnrmd-popup', htm.join(''));
        document.body.appendChild(pp);
        pp.addEventListener('click', function (e) {
            var target = e.target;
            if (!this.firstChild.contains(target)) {
                pp.style.display = "none";
            }
        }, false);
        pp.querySelector('.np-close').addEventListener('click', function () {
            pp.style.display = "none";
        }, false);

        return pp;
    }

    netnrmd.emoji = {
        "常用": "👍 👎 😄 🎉 😕 💗 🚀 👀",
        "笑脸和情感": "😀 😃 😄 😁 😆 😅 🤣 😂 🙂 🙃 😉 😊 😇 🥰 😍 🤩 😘 😗 ☺️ 😚 😙 😋 😛 😜 🤪 😝 🤑 🤗 🤭 🤫 🤔 🤐 🤨 😐 😑 😶 😏 😒 🙄 😬 🤥 😌 😔 😪 🤤 😴 😷 🤒 🤕 🤢 🤮 🤧 🥵 🥶 🥴 😵 🤯 🤠 🥳 😎 🤓 🧐 😕 😟 🙁 ☹️ 😮 😯 😲 😳 🥺 😦 😧 😨 😰 😥 😢 😭 😱 😖 😣 😞 😓 😩 😫 🥱 😤 😡 😠 🤬 😈 👿 💀 ☠️ 💩 🤡 👹 👺 👻 👽 👾 🤖 😺 😸 😹 😻 😼 😽 🙀 😿 😾 🙈 🙉 🙊 💋 💌 💘 💝 💖 💗 💓 💞 💕 💟 ❣️ 💔 ❤️ 🧡 💛 💚 💙 💜 🤎 🖤 🤍 💯 💢 💥 💫 💦 💨 🕳️ 💣 💬 👁️‍🗨️ 🗨️ 🗯️ 💭 💤",
        "人类和身体": "👋 🤚 🖐️ ✋ 🖖 👌 🤏 ✌️ 🤞 🤟 🤘 🤙 👈 👉 👆 🖕 👇 ☝️ 👍 👎 ✊ 👊 🤛 🤜 👏 🙌 👐 🤲 🤝 🙏 ✍️ 💅 🤳 💪 🦾 🦿 🦵 🦶 👂 🦻 👃 🧠 🦷 🦴 👀 👁️ 👅 👄 👶 🧒 👦 👧 🧑 👱 👨 🧔 👨‍🦰 👨‍🦱 👨‍🦳 👨‍🦲 👩 👩‍🦰 👩‍🦱 👩‍🦳 👩‍🦲 👱‍♀️ 👱‍♂️ 🧓 👴 👵 🙍 🙍‍♂️ 🙍‍♀️ 🙎 🙎‍♂️ 🙎‍♀️ 🙅 🙅‍♂️ 🙅‍♀️ 🙆 🙆‍♂️ 🙆‍♀️ 💁 💁‍♂️ 💁‍♀️ 🙋 🙋‍♂️ 🙋‍♀️ 🧏 🧏‍♂️ 🧏‍♀️ 🙇 🙇‍♂️ 🙇‍♀️ 🤦 🤦‍♂️ 🤦‍♀️ 🤷 🤷‍♂️ 🤷‍♀️ ⚕️ 👨‍⚕️ 👩‍⚕️ 👨‍🎓 👩‍🎓 👨‍🏫 👩‍🏫 👨‍⚖️ 👩‍⚖️ 👨‍🌾 👩‍🌾 👨‍🍳 👩‍🍳 👨‍🔧 👩‍🔧 👨‍🏭 👩‍🏭 👨‍💼 👩‍💼 👨‍🔬 👩‍🔬 👨‍💻 👩‍💻 👨‍🎤 👩‍🎤 👨‍🎨 👩‍🎨 👨‍✈️ 👩‍✈️ 👨‍🚀 👩‍🚀 👨‍🚒 👩‍🚒 👮 👮‍♂️ 👮‍♀️ 🕵️ 🕵️‍♂️ 🕵️‍♀️ 💂 💂‍♂️ 💂‍♀️ 👷 👷‍♂️ 👷‍♀️ 🤴 👸 👳 👳‍♂️ 👳‍♀️ 👲 🧕 🤵 👰 🤰 🤱 👼 🎅 🤶 🦸 🦸‍♂️ 🦸‍♀️ 🦹 🦹‍♂️ 🦹‍♀️ 🧙 🧙‍♂️ 🧙‍♀️ 🧚 🧚‍♂️ 🧚‍♀️ 🧛 🧛‍♂️ 🧛‍♀️ 🧜 🧜‍♂️ 🧜‍♀️ 🧝 🧝‍♂️ 🧝‍♀️ 🧞 🧞‍♂️ 🧞‍♀️ 🧟 🧟‍♂️ 🧟‍♀️ 💆 💆‍♂️ 💆‍♀️ 💇 💇‍♂️ 💇‍♀️ 🚶 🚶‍♂️ 🚶‍♀️ 🧍 🧍‍♂️ 🧍‍♀️ 🧎 🧎‍♂️ 🧎‍♀️ 👨‍🦯 👩‍🦯 👨‍🦼 👩‍🦼 👨‍🦽 👩‍🦽 🏃 🏃‍♂️ 🏃‍♀️ 💃 🕺 🕴️ 👯 👯‍♂️ 👯‍♀️ 🧖 🧖‍♂️ 🧖‍♀️ 🧗 🧗‍♂️ 🧗‍♀️ 🤺 🏇 ⛷️ 🏂 🏌️ 🏌️‍♂️ 🏌️‍♀️ 🏄 🏄‍♂️ 🏄‍♀️ 🚣 🚣‍♂️ 🚣‍♀️ 🏊 🏊‍♂️ 🏊‍♀️ ⛹️ ⛹️‍♂️ ⛹️‍♀️ 🏋️ 🏋️‍♂️ 🏋️‍♀️ 🚴 🚴‍♂️ 🚴‍♀️ 🚵 🚵‍♂️ 🚵‍♀️ 🤸 🤸‍♂️ 🤸‍♀️ 🤼 🤼‍♂️ 🤼‍♀️ 🤽 🤽‍♂️ 🤽‍♀️ 🤾 🤾‍♂️ 🤾‍♀️ 🤹 🤹‍♂️ 🤹‍♀️ 🧘 🧘‍♂️ 🧘‍♀️ 🛀 🛌 🧑‍🤝‍🧑 👭 👫 👬 💏 👩‍❤️‍💋‍👨 👨‍❤️‍💋‍👨 👩‍❤️‍💋‍👩 💑 👩‍❤️‍👨 👨‍❤️‍👨 👩‍❤️‍👩 👪 👨‍👩‍👦 👨‍👩‍👧 👨‍👩‍👧‍👦 👨‍👩‍👦‍👦 👨‍👩‍👧‍👧 👨‍👨‍👦 👨‍👨‍👧 👨‍👨‍👧‍👦 👨‍👨‍👦‍👦 👨‍👨‍👧‍👧 👩‍👩‍👦 👩‍👩‍👧 👩‍👩‍👧‍👦 👩‍👩‍👦‍👦 👩‍👩‍👧‍👧 👨‍👦 👨‍👦‍👦 👨‍👧 👨‍👧‍👦 👨‍👧‍👧 👩‍👦 👩‍👦‍👦 👩‍👧 👩‍👧‍👦 👩‍👧‍👧 🗣️ 👤 👥 👣",
        "动物和自然": "🐵 🐒 🦍 🦧 🐶 🐕 🦮 🐕‍🦺 🐩 🐺 🦊 🦝 🐱 🐈 🦁 🐯 🐅 🐆 🐴 🐎 🦄 🦓 🦌 🐮 🐂 🐃 🐄 🐷 🐖 🐗 🐽 🐏 🐑 🐐 🐪 🐫 🦙 🦒 🐘 🦏 🦛 🐭 🐁 🐀 🐹 🐰 🐇 🐿️ 🦔 🦇 🐻 🐨 🐼 🦥 🦦 🦨 🦘 🦡 🐾 🦃 🐔 🐓 🐣 🐤 🐥 🐦 🐧 🕊️ 🦅 🦆 🦢 🦉 🦩 🦚 🦜 🐊 🐢 🦎 🐍 🐲 🐉 🦕 🦖 🐳 🐋 🐬 🐟 🐠 🐡 🦈 🐙 🐚 🐌 🦋 🐛 🐜 🐝 🐞 🦗 🕷️ 🕸️ 🦂 🦟 🦠 💐 🌸 💮 🏵️ 🌹 🥀 🌺 🌻 🌼 🌷 🌱 🌲 🌳 🌴 🌵 🌾 🌿 ☘️ 🍀 🍁 🍂 🍃",
        "食物和饮料": "🍇 🍈 🍉 🍊 🍋 🍌 🍍 🥭 🍎 🍏 🍐 🍑 🍒 🍓 🥝 🍅 🥥 🥑 🥔 🥕 🌽 🌶️ 🥒 🥬 🥦 🧄 🧅 🍄 🥜 🌰 🍞 🥐 🥖 🥨 🥯 🥞 🧇 🧀 🍖 🍗 🥩 🥓 🍔 🍟 🍕 🌭 🥪 🌮 🌯 🥙 🧆 🥚 🍳 🥘 🍲 🥣 🥗 🍿 🧈 🧂 🥫 🍱 🍘 🍙 🍚 🍛 🍜 🍝 🍠 🍢 🍣 🍤 🍥 🥮 🍡 🥟 🥠 🥡 🦀 🦞 🦐 🦑 🦪 🍦 🍧 🍨 🍩 🍪 🎂 🍰 🧁 🥧 🍫 🍬 🍭 🍮 🍯 🍼 🥛 ☕ 🍵 🍶 🍾 🍷 🍸 🍹 🍺 🍻 🥂 🥃 🥤 🧃 🧉 🧊 🥢 🍽️ 🍴 🥄 🔪 🏺",
        "旅行和地点": "🌍 🌎 🌏 🌐 🗺️ 🗾 🧭 🏔️ ⛰️ 🌋 🗻 🏕️ 🏖️ 🏜️ 🏝️ 🏞️ 🏟️ 🏛️ 🏗️ 🧱 🏘️ 🏚️ 🏠 🏡 🏢 🏣 🏤 🏥 🏦 🏨 🏩 🏪 🏫 🏬 🏭 🏯 🏰 💒 🗼 🗽 ⛪ 🕌 🛕 🕍 ⛩️ 🕋 ⛲ ⛺ 🌁 🌃 🏙️ 🌄 🌅 🌆 🌇 🌉 ♨️ 🎠 🎡 🎢 💈 🎪 🚂 🚃 🚄 🚅 🚆 🚇 🚈 🚉 🚊 🚝 🚞 🚋 🚌 🚍 🚎 🚐 🚑 🚒 🚓 🚔 🚕 🚖 🚗 🚘 🚙 🚚 🚛 🚜 🏎️ 🏍️ 🛵 🦽 🦼 🛺 🚲 🛴 🛹 🚏 🛣️ 🛤️ 🛢️ ⛽ 🚨 🚥 🚦 🛑 🚧 ⚓ ⛵ 🛶 🚤 🛳️ ⛴️ 🛥️ 🚢 ✈️ 🛩️ 🛫 🛬 🪂 💺 🚁 🚟 🚠 🚡 🛰️ 🚀 🛸 🛎️ 🧳 ⌛ ⏳ ⌚ ⏰ ⏱️ ⏲️ 🕰️ 🕛 🕧 🕐 🕜 🕑 🕝 🕒 🕞 🕓 🕟 🕔 🕠 🕕 🕡 🕖 🕢 🕗 🕣 🕘 🕤 🕙 🕥 🕚 🕦 🌑 🌒 🌓 🌔 🌕 🌖 🌗 🌘 🌙 🌚 🌛 🌜 🌡️ ☀️ 🌝 🌞 🪐 ⭐ 🌟 🌠 🌌 ☁️ ⛅ ⛈️ 🌤️ 🌥️ 🌦️ 🌧️ 🌨️ 🌩️ 🌪️ 🌫️ 🌬️ 🌀 🌈 🌂 ☂️ ☔ ⛱️ ⚡ ❄️ ☃️ ⛄ ☄️ 🔥 💧 🌊",
        "活动": "🎃 🎄 🎆 🎇 🧨 ✨ 🎈 🎉 🎊 🎋 🎍 🎎 🎏 🎐 🎑 🧧 🎀 🎁 🎗️ 🎟️ 🎫 🎖️ 🏆 🏅 🥇 🥈 🥉 ⚽ ⚾ 🥎 🏀 🏐 🏈 🏉 🎾 🥏 🎳 🏏 🏑 🏒 🥍 🏓 🏸 🥊 🥋 🥅 ⛳ ⛸️ 🎣 🤿 🎽 🎿 🛷 🥌 🎯 🪀 🪁 🎱 🔮 🧿 🎮 🕹️ 🎰 🎲 🧩 🧸 ♠️ ♥️ ♦️ ♣️ ♟️ 🃏 🀄 🎴 🎭 🖼️ 🎨 🧵 🧶",
        "对象": "👓 🕶️ 🥽 🥼 🦺 👔 👕 👖 🧣 🧤 🧥 🧦 👗 👘 🥻 🩱 🩲 🩳 👙 👚 👛 👜 👝 🛍️ 🎒 👞 👟 🥾 🥿 👠 👡 🩰 👢 👑 👒 🎩 🎓 🧢 ⛑️ 📿 💄 💍 💎 🔇 🔈 🔉 🔊 📢 📣 📯 🔔 🔕 🎼 🎵 🎶 🎙️ 🎚️ 🎛️ 🎤 🎧 📻 🎷 🎸 🎹 🎺 🎻 🪕 🥁 📱 📲 ☎️ 📞 📟 📠 🔋 🔌 💻 🖥️ 🖨️ ⌨️ 🖱️ 🖲️ 💽 💾 💿 📀 🧮 🎥 🎞️ 📽️ 🎬 📺 📷 📸 📹 📼 🔍 🔎 🕯️ 💡 🔦 🏮 🪔 📔 📕 📖 📗 📘 📙 📚 📓 📒 📃 📜 📄 📰 🗞️ 📑 🔖 🏷️ 💰 💴 💵 💶 💷 💸 💳 🧾 💹 ✉️ 📧 📨 📩 📤 📥 📦 📫 📪 📬 📭 📮 🗳️ ✏️ ✒️ 🖋️ 🖊️ 🖌️ 🖍️ 📝 💼 📁 📂 🗂️ 📅 📆 🗒️ 🗓️ 📇 📈 📉 📊 📋 📌 📍 📎 🖇️ 📏 📐 ✂️ 🗃️ 🗄️ 🗑️ 🔒 🔓 🔏 🔐 🔑 🗝️ 🔨 🪓 ⛏️ ⚒️ 🛠️ 🗡️ ⚔️ 🔫 🏹 🛡️ 🔧 🔩 ⚙️ 🗜️ ⚖️ 🦯 🔗 ⛓️ 🧰 🧲 ⚗️ 🧪 🧫 🧬 🔬 🔭 📡 💉 🩸 💊 🩹 🩺 🚪 🛏️ 🛋️ 🪑 🚽 🚿 🛁 🪒 🧴 🧷 🧹 🧺 🧻 🧼 🧽 🧯 🛒 🚬 ⚰️ ⚱️ 🗿",
        "符号": "🏧 🚮 🚰 ♿ 🚹 🚺 🚻 🚼 🚾 🛂 🛃 🛄 🛅 ⚠️ 🚸 ⛔ 🚫 🚳 🚭 🚯 🚱 🚷 📵 🔞 ☢️ ☣️ ⬆️ ↗️ ➡️ ↘️ ⬇️ ↙️ ⬅️ ↖️ ↕️ ↔️ ↩️ ↪️ ⤴️ ⤵️ 🔃 🔄 🔙 🔚 🔛 🔜 🔝 🛐 ⚛️ 🕉️ ✡️ ☸️ ☯️ ✝️ ☦️ ☪️ ☮️ 🕎 🔯 ♈ ♉ ♊ ♋ ♌ ♍ ♎ ♏ ♐ ♑ ♒ ♓ ⛎ 🔀 🔁 🔂 ▶️ ⏩ ⏭️ ⏯️ ◀️ ⏪ ⏮️ 🔼 ⏫ 🔽 ⏬ ⏸️ ⏹️ ⏺️ ⏏️ 🎦 🔅 🔆 📶 📳 📴 ♀️ ♂️ ⚧️ ✖️ ➕ ➖ ➗ ♾️ ‼️ ⁉️ ❓ ❔ ❕ ❗ 〰️ 💱 💲 ⚕️ ♻️ ⚜️ 🔱 📛 🔰 ⭕ ✅ ☑️ ✔️ ❌ ❎ ➰ ➿ 〽️ ✳️ ✴️ ❇️ ©️ ®️ ™️ #️⃣ *️⃣ 0️⃣ 1️⃣ 2️⃣ 3️⃣ 4️⃣ 5️⃣ 6️⃣ 7️⃣ 8️⃣ 9️⃣ 🔟 🔠 🔡 🔢 🔣 🔤 🅰️ 🆎 🅱️ 🆑 🆒 🆓 ℹ️ 🆔 Ⓜ️ 🆕 🆖 🅾️ 🆗 🅿️ 🆘 🆙 🆚 🈁 🈂️ 🈷️ 🈶 🈯 🉐 🈹 🈚 🈲 🉑 🈸 🈴 🈳 ㊗️ ㊙️ 🈺 🈵 🔴 🟠 🟡 🟢 🔵 🟣 🟤 ⚫ ⚪ 🟥 🟧 🟨 🟩 🟦 🟪 🟫 ⬛ ⬜ ◼️ ◻️ ◾ ◽ ▪️ ▫️ 🔶 🔷 🔸 🔹 🔺 🔻 💠 🔘 🔳 🔲",
        "旗帜": "🏁 🚩 🎌 🏴 🏳️ 🏳️‍🌈 🏴‍☠️ 🇦🇨 🇦🇩 🇦🇪 🇦🇫 🇦🇬 🇦🇮 🇦🇱 🇦🇲 🇦🇴 🇦🇶 🇦🇷 🇦🇸 🇦🇹 🇦🇺 🇦🇼 🇦🇽 🇦🇿 🇧🇦 🇧🇧 🇧🇩 🇧🇪 🇧🇫 🇧🇬 🇧🇭 🇧🇮 🇧🇯 🇧🇱 🇧🇲 🇧🇳 🇧🇴 🇧🇶 🇧🇷 🇧🇸 🇧🇹 🇧🇻 🇧🇼 🇧🇾 🇧🇿 🇨🇦 🇨🇨 🇨🇩 🇨🇫 🇨🇬 🇨🇭 🇨🇮 🇨🇰 🇨🇱 🇨🇲 🇨🇳 🇨🇴 🇨🇵 🇨🇷 🇨🇺 🇨🇻 🇨🇼 🇨🇽 🇨🇾 🇨🇿 🇩🇪 🇩🇬 🇩🇯 🇩🇰 🇩🇲 🇩🇴 🇩🇿 🇪🇦 🇪🇨 🇪🇪 🇪🇬 🇪🇭 🇪🇷 🇪🇸 🇪🇹 🇪🇺 🇫🇮 🇫🇯 🇫🇰 🇫🇲 🇫🇴 🇫🇷 🇬🇦 🇬🇧 🇬🇩 🇬🇪 🇬🇫 🇬🇬 🇬🇭 🇬🇮 🇬🇱 🇬🇲 🇬🇳 🇬🇵 🇬🇶 🇬🇷 🇬🇸 🇬🇹 🇬🇺 🇬🇼 🇬🇾 🇭🇰 🇭🇲 🇭🇳 🇭🇷 🇭🇹 🇭🇺 🇮🇨 🇮🇩 🇮🇪 🇮🇱 🇮🇲 🇮🇳 🇮🇴 🇮🇶 🇮🇷 🇮🇸 🇮🇹 🇯🇪 🇯🇲 🇯🇴 🇯🇵 🇰🇪 🇰🇬 🇰🇭 🇰🇮 🇰🇲 🇰🇳 🇰🇵 🇰🇷 🇰🇼 🇰🇾 🇰🇿 🇱🇦 🇱🇧 🇱🇨 🇱🇮 🇱🇰 🇱🇷 🇱🇸 🇱🇹 🇱🇺 🇱🇻 🇱🇾 🇲🇦 🇲🇨 🇲🇩 🇲🇪 🇲🇫 🇲🇬 🇲🇭 🇲🇰 🇲🇱 🇲🇲 🇲🇳 🇲🇴 🇲🇵 🇲🇶 🇲🇷 🇲🇸 🇲🇹 🇲🇺 🇲🇻 🇲🇼 🇲🇽 🇲🇾 🇲🇿 🇳🇦 🇳🇨 🇳🇪 🇳🇫 🇳🇬 🇳🇮 🇳🇱 🇳🇴 🇳🇵 🇳🇷 🇳🇺 🇳🇿 🇴🇲 🇵🇦 🇵🇪 🇵🇫 🇵🇬 🇵🇭 🇵🇰 🇵🇱 🇵🇲 🇵🇳 🇵🇷 🇵🇸 🇵🇹 🇵🇼 🇵🇾 🇶🇦 🇷🇪 🇷🇴 🇷🇸 🇷🇺 🇷🇼 🇸🇦 🇸🇧 🇸🇨 🇸🇩 🇸🇪 🇸🇬 🇸🇭 🇸🇮 🇸🇯 🇸🇰 🇸🇱 🇸🇲 🇸🇳 🇸🇴 🇸🇷 🇸🇸 🇸🇹 🇸🇻 🇸🇽 🇸🇾 🇸🇿 🇹🇦 🇹🇨 🇹🇩 🇹🇫 🇹🇬 🇹🇭 🇹🇯 🇹🇰 🇹🇱 🇹🇲 🇹🇳 🇹🇴 🇹🇷 🇹🇹 🇹🇻 🇹🇼 🇹🇿 🇺🇦 🇺🇬 🇺🇲 🇺🇳 🇺🇸 🇺🇾 🇺🇿 🇻🇦 🇻🇨 🇻🇪 🇻🇬 🇻🇮 🇻🇳 🇻🇺 🇼🇫 🇼🇸 🇽🇰 🇾🇪 🇾🇹 🇿🇦 🇿🇲 🇿🇼 🏴󠁧󠁢󠁥󠁮󠁧󠁿 🏴󠁧󠁢󠁳󠁣󠁴󠁿 🏴󠁧󠁢󠁷󠁬󠁳󠁿"
    }

    window.netnrmd = netnrmd;

})(window);
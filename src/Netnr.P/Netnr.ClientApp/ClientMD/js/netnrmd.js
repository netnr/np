import pangu from 'pangu';
import tocbot from 'tocbot';
import { marked } from 'marked';
import DOMPurify from 'dompurify';
import hljs from 'highlight.js/lib/common';
import { netnrmdInit } from './netnrmdInit';

let netnrmd = {

    init: (id, ops) => new netnrmdInit(id, ops),

    hljs,
    DOMPurify,
    marked,
    pangu,
    tocbot,

    /**
     * 渲染
     * @param {*} md 
     * @returns 
     */
    render: function (md) {
        return DOMPurify.sanitize(marked.parse(md, {
            headerIds: true,
            headerPrefix: 'toc-',
            highlight: function (str, lang) {
                return hljs.getLanguage(lang) ? hljs.highlight(str, { language: lang }).value : hljs.highlightAuto(str).value;
            },
            langPrefix: 'hljs language-',
        }))
    },

    /**
     * 创建节点
     * @param {any} domName 节点名称
     * @param {any} className 类样式名
     * @param {any} html
     */
    createDom: function (domName, className, html) {
        let dom = document.createElement(domName);
        if (className != null) {
            dom.className = className;
        }
        if (html != null) {
            dom.innerHTML = html;
        }
        return dom;
    },

    /**
     * 弹窗
     * @param {any} title 标题
     * @param {any} content 内容主体
     */
    popup: function (title, content) {
        let htm = [];
        htm.push('<div class="np-card">');
        htm.push('<div class="np-header">');
        htm.push('<a class="np-close" href="javascript:void(0);" title="关闭">✖</a>');
        htm.push('<span>' + title + '</span></div>');
        htm.push('<div class="np-body">' + content + '</div>');
        htm.push('</div>');

        let pp = this.createDom('div', 'netnrmd-popup', htm.join(''));
        document.body.appendChild(pp);

        //点击遮罩层隐藏
        pp.addEventListener('click', function (e) {
            let target = e.target;
            if (!this.firstChild.contains(target)) {
                pp.style.display = "none";
            }
        }, false);
        //点击右上角隐藏
        pp.querySelector('.np-close').addEventListener('click', function () {
            pp.style.display = "none";
        }, false);

        return pp;
    },

    /**
     * 构建目录
     * @param {any} ops
     */
    toc: function (ops) {
        ops = Object.assign({
            domMarkdown: document.querySelector('.markdown-body'), //markdown
            headerId: (node, index) => node.id = `toc_${index}`, //重设ID
            tocClass: 'toc', //类
        }, ops)

        let menus = ops.domMarkdown.querySelectorAll("h1,h2,h3,h4,h5,h6");
        for (let i = 0; i < menus.length; i++) {
            let item = menus[i];
            ops.headerId(item, i);
        }

        if (menus.length > 0) {
            //添加 flex 布局，显示右侧目录, 添加目录
            let domMarkdownContainer = ops.domMarkdown.parentElement;
            domMarkdownContainer.classList.add('markdown-container');

            let domToc = document.createElement('div');
            domToc.className = "markdown-toc";
            domMarkdownContainer.appendChild(domToc);

            tocbot.init({
                tocElement: domToc,
                contentElement: ops.domMarkdown,
                headingSelector: 'h1, h2, h3, h4, h5, h6'
            });
        }
    },

    /**
     * 下载
     * @param {*} content 
     * @param {*} file 
     */
    download: function (content, file) {
        let aTag = document.createElement('a');
        aTag.download = file;
        if (content.nodeType == 1) {
            aTag.href = content.toDataURL();
        } else {
            let blob = new Blob([content]);
            aTag.href = URL.createObjectURL(blob);
        }
        document.body.appendChild(aTag);
        aTag.click();
        aTag.remove();
    },

    tsRely: {
        markdownStyle: "https://npmcdn.com/netnrmd@4.0.0/dist/netnrmd.css",
        htmlDocx: "https://npmcdn.com/html-docx-js@0.3.1/dist/html-docx.js",
        html2canvas: "https://npmcdn.com/html2canvas@1.4.1/dist/html2canvas.min.js",
        jspdf: "https://npmcdn.com/jspdf@2.5.1/dist/jspdf.umd.min.js",
        html2PDF: "https://npmcdn.com/jspdf-html2canvas@1.4.9/dist/jspdf-html2canvas.min.js",
    },
    tsLoaded: {},
    /**
     * 引入远程脚本
     * @param {*} src 
     * @param {*} type 
     * @returns 
     */
    importScript: async (src, type) => {
        let pout = netnrmd.tsLoaded[src];
        if (!pout) {
            netnrmd.tsLoaded[src] = pout = new Promise((resolve, reject) => {

                let ds = document.scripts;
                let domSrc;
                for (let index = 0; index < ds.length; index++) {
                    let si = ds[index];
                    if (si.src.includes(src)) {
                        domSrc = si;
                        break;
                    }
                }

                if (domSrc) {
                    resolve();
                } else {
                    let ele = document.createElement("SCRIPT");
                    ele.src = src;
                    ele.type = type || "text/javascript";
                    ele.onerror = function (ex) {
                        reject(ex)
                    }
                    ele.onload = function () {
                        resolve();
                    }

                    document.head.appendChild(ele);
                }
            });
        }
        return pout;
    },

    /**
     * 图标按钮构建
     * @param {*} itemIcon 
     */
    toolbarBuildIcon: function (itemIcon) {
        let tooltip = `${itemIcon.title}/${itemIcon.cmd} ${(itemIcon.key ? itemIcon.key : '')}`;
        let li = `<li class="${itemIcon.class || ''}" data-cmd="${itemIcon.cmd}" title="${tooltip}">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16">${itemIcon.svg}</svg></li>`;
        return li;
    },

    /**
     * 工具条图标
     */
    toolbarIcons: [
        {
            title: '表情', cmd: 'emoji', svg: '<path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/><path d="M4.285 9.567a.5.5 0 0 1 .683.183A3.498 3.498 0 0 0 8 11.5a3.498 3.498 0 0 0 3.032-1.75.5.5 0 1 1 .866.5A4.498 4.498 0 0 1 8 12.5a4.498 4.498 0 0 1-3.898-2.25.5.5 0 0 1 .183-.683zM7 6.5C7 7.328 6.552 8 6 8s-1-.672-1-1.5S5.448 5 6 5s1 .672 1 1.5zm4 0c0 .828-.448 1.5-1 1.5s-1-.672-1-1.5S9.448 5 10 5s1 .672 1 1.5z"/>', action: function (that) {
                if (!that.domEmoji) {
                    //构建弹出内容
                    let htm = [];
                    htm.push("<div class='netnrmd-emoji'>")
                    for (let eh in netnrmd.emoji) {
                        htm.push('<b>' + eh + '</b><ul>');
                        netnrmd.emoji[eh].split(' ').forEach(ei => {
                            htm.push('<li>' + ei + '</li>');
                        })
                        htm.push('</ul>');
                    }
                    htm.push("<div>")

                    //弹出
                    that.domEmoji = netnrmd.popup("表情（按住 Ctrl 连选）", htm.join(''));

                    //选择表情
                    that.domEmoji.addEventListener('click', function (e) {
                        let target = e.target;
                        if (target.nodeName == "LI") {
                            that.insert(target.innerHTML); //插入表情
                            if (!e.ctrlKey) {
                                this.style.display = 'none';
                            }
                        }
                        that.focus();
                    }, false)
                } else {
                    that.domEmoji.style.display = '';
                }
            }
        },
        { title: '粗体', cmd: 'bold', key: 'Ctrl+Alt+B', svg: '<path d="M8.21 13c2.106 0 3.412-1.087 3.412-2.823 0-1.306-.984-2.283-2.324-2.386v-.055a2.176 2.176 0 0 0 1.852-2.14c0-1.51-1.162-2.46-3.014-2.46H3.843V13H8.21zM5.908 4.674h1.696c.963 0 1.517.451 1.517 1.244 0 .834-.629 1.32-1.73 1.32H5.908V4.673zm0 6.788V8.598h1.73c1.217 0 1.88.492 1.88 1.415 0 .943-.643 1.449-1.832 1.449H5.907z"/>' },
        { title: '斜体', cmd: 'italic', key: 'Ctrl+Alt+I', svg: '<path d="M7.991 11.674 9.53 4.455c.123-.595.246-.71 1.347-.807l.11-.52H7.211l-.11.52c1.06.096 1.128.212 1.005.807L6.57 11.674c-.123.595-.246.71-1.346.806l-.11.52h3.774l.11-.52c-1.06-.095-1.129-.211-1.006-.806z"/>' },
        { title: '删除', cmd: 'strikethrough', key: 'Ctrl+Alt+D', svg: '<path d="M6.333 5.686c0 .31.083.581.27.814H5.166a2.776 2.776 0 0 1-.099-.76c0-1.627 1.436-2.768 3.48-2.768 1.969 0 3.39 1.175 3.445 2.85h-1.23c-.11-1.08-.964-1.743-2.25-1.743-1.23 0-2.18.602-2.18 1.607zm2.194 7.478c-2.153 0-3.589-1.107-3.705-2.81h1.23c.144 1.06 1.129 1.703 2.544 1.703 1.34 0 2.31-.705 2.31-1.675 0-.827-.547-1.374-1.914-1.675L8.046 8.5H1v-1h14v1h-3.504c.468.437.675.994.675 1.697 0 1.826-1.436 2.967-3.644 2.967z"/>' },
        { title: '标题', cmd: 'header', key: 'Ctrl+Alt+H', svg: '<path d="M7.637 13V3.669H6.379V7.62H1.758V3.67H.5V13h1.258V8.728h4.62V13h1.259zm3.625-4.272h1.018c1.142 0 1.935.67 1.949 1.674.013 1.005-.78 1.737-2.01 1.73-1.08-.007-1.853-.588-1.935-1.32H9.108c.069 1.327 1.224 2.386 3.083 2.386 1.935 0 3.343-1.155 3.309-2.789-.027-1.51-1.251-2.16-2.037-2.249v-.068c.704-.123 1.764-.91 1.723-2.229-.035-1.353-1.176-2.4-2.954-2.385-1.873.006-2.857 1.162-2.898 2.358h1.196c.062-.69.711-1.299 1.696-1.299.998 0 1.695.622 1.695 1.525.007.922-.718 1.592-1.695 1.592h-.964v1.074z"/>' },
        { title: '引用', cmd: 'quote', key: 'Ctrl+Alt+Q', svg: '<path d="M2.5 3a.5.5 0 0 0 0 1h11a.5.5 0 0 0 0-1h-11zm5 3a.5.5 0 0 0 0 1h6a.5.5 0 0 0 0-1h-6zm0 3a.5.5 0 0 0 0 1h6a.5.5 0 0 0 0-1h-6zm-5 3a.5.5 0 0 0 0 1h11a.5.5 0 0 0 0-1h-11zm.79-5.373c.112-.078.26-.17.444-.275L3.524 6c-.122.074-.272.17-.452.287-.18.117-.35.26-.51.428a2.425 2.425 0 0 0-.398.562c-.11.207-.164.438-.164.692 0 .36.072.65.217.873.144.219.385.328.72.328.215 0 .383-.07.504-.211a.697.697 0 0 0 .188-.463c0-.23-.07-.404-.211-.521-.137-.121-.326-.182-.568-.182h-.282c.024-.203.065-.37.123-.498a1.38 1.38 0 0 1 .252-.37 1.94 1.94 0 0 1 .346-.298zm2.167 0c.113-.078.262-.17.445-.275L5.692 6c-.122.074-.272.17-.452.287-.18.117-.35.26-.51.428a2.425 2.425 0 0 0-.398.562c-.11.207-.164.438-.164.692 0 .36.072.65.217.873.144.219.385.328.72.328.215 0 .383-.07.504-.211a.697.697 0 0 0 .188-.463c0-.23-.07-.404-.211-.521-.137-.121-.326-.182-.568-.182h-.282a1.75 1.75 0 0 1 .118-.492c.058-.13.144-.254.257-.375a1.94 1.94 0 0 1 .346-.3z"/>' },
        { title: '有序列表', cmd: 'list-ol', key: 'Ctrl+Alt+O', svg: '<path fill-rule="evenodd" d="M5 11.5a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5z"/><path d="M1.713 11.865v-.474H2c.217 0 .363-.137.363-.317 0-.185-.158-.31-.361-.31-.223 0-.367.152-.373.31h-.59c.016-.467.373-.787.986-.787.588-.002.954.291.957.703a.595.595 0 0 1-.492.594v.033a.615.615 0 0 1 .569.631c.003.533-.502.8-1.051.8-.656 0-1-.37-1.008-.794h.582c.008.178.186.306.422.309.254 0 .424-.145.422-.35-.002-.195-.155-.348-.414-.348h-.3zm-.004-4.699h-.604v-.035c0-.408.295-.844.958-.844.583 0 .96.326.96.756 0 .389-.257.617-.476.848l-.537.572v.03h1.054V9H1.143v-.395l.957-.99c.138-.142.293-.304.293-.508 0-.18-.147-.32-.342-.32a.33.33 0 0 0-.342.338v.041zM2.564 5h-.635V2.924h-.031l-.598.42v-.567l.629-.443h.635V5z"/>' },
        { title: '无序列表', cmd: 'list-ul', key: 'Ctrl+Alt+U', svg: '<path fill-rule="evenodd" d="M5 11.5a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm-3 1a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2z"/>' },
        { title: '选中', cmd: 'checked', key: 'Ctrl+Alt+Y', svg: '<path d="M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z"/><path d="M10.97 4.97a.75.75 0 0 1 1.071 1.05l-3.992 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425a.235.235 0 0 1 .02-.022z"/>' },
        { title: '未选中', cmd: 'unchecked', key: 'Ctrl+Alt+N', svg: '<path d="M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z"/>' },
        { title: '链接', cmd: 'link', key: 'Ctrl+Alt+L', svg: '<path d="M4.715 6.542 3.343 7.914a3 3 0 1 0 4.243 4.243l1.828-1.829A3 3 0 0 0 8.586 5.5L8 6.086a1.002 1.002 0 0 0-.154.199 2 2 0 0 1 .861 3.337L6.88 11.45a2 2 0 1 1-2.83-2.83l.793-.792a4.018 4.018 0 0 1-.128-1.287z"/><path d="M6.586 4.672A3 3 0 0 0 7.414 9.5l.775-.776a2 2 0 0 1-.896-3.346L9.12 3.55a2 2 0 1 1 2.83 2.83l-.793.792c.112.42.155.855.128 1.287l1.372-1.372a3 3 0 1 0-4.243-4.243L6.586 4.672z"/>' },
        { title: '图片', cmd: 'image', key: 'Ctrl+Alt+G', svg: '<path d="M6.002 5.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z"/><path d="M1.5 2A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h13a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-13zm13 1a.5.5 0 0 1 .5.5v6l-3.775-1.947a.5.5 0 0 0-.577.093l-3.71 3.71-2.66-1.772a.5.5 0 0 0-.63.062L1.002 12v.54A.505.505 0 0 1 1 12.5v-9a.5.5 0 0 1 .5-.5h13z"/>' },
        { title: '表格', cmd: 'table', key: 'Ctrl+Alt+T', svg: '<path d="M0 2a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V2zm15 2h-4v3h4V4zm0 4h-4v3h4V8zm0 4h-4v3h3a1 1 0 0 0 1-1v-2zm-5 3v-3H6v3h4zm-5 0v-3H1v2a1 1 0 0 0 1 1h3zm-4-4h4V8H1v3zm0-4h4V4H1v3zm5-3v3h4V4H6zm4 4H6v3h4V8z"/>' },
        { title: '代码', cmd: 'code', key: 'Ctrl+Alt+K', svg: '<path d="M10.478 1.647a.5.5 0 1 0-.956-.294l-4 13a.5.5 0 0 0 .956.294l4-13zM4.854 4.146a.5.5 0 0 1 0 .708L1.707 8l3.147 3.146a.5.5 0 0 1-.708.708l-3.5-3.5a.5.5 0 0 1 0-.708l3.5-3.5a.5.5 0 0 1 .708 0zm6.292 0a.5.5 0 0 0 0 .708L14.293 8l-3.147 3.146a.5.5 0 0 0 .708.708l3.5-3.5a.5.5 0 0 0 0-.708l-3.5-3.5a.5.5 0 0 0-.708 0z"/>' },
        { title: '分隔线', cmd: 'line', key: 'Ctrl+Alt+R', svg: '<path d="M2 8a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm3 3a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0-3a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>' },
        { title: '间隙', cmd: 'spacing', key: 'Ctrl+Alt+S', svg: '<path fill-rule="evenodd" d="M14.5 1a.5.5 0 0 0-.5.5v13a.5.5 0 0 0 1 0v-13a.5.5 0 0 0-.5-.5zm-13 0a.5.5 0 0 0-.5.5v13a.5.5 0 0 0 1 0v-13a.5.5 0 0 0-.5-.5z"/><path d="M6 13a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1v10z"/>', action: function (that) { that.spacing() } },
        {
            title: '导出', cmd: 'export', svg: '<path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/><path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/>', action: function (that) {
                if (!that.domExport) {
                    //弹出
                    that.domExport = netnrmd.popup("导出", "Markdown Word HTML PNG PDF".split(' ').map(x => `<p style="font-size:1.3em; text-align:center;"><a href="javascript:void(0)">${x}</a></p>`).join(''));
                    that.domExport.querySelector('.np-card').style.maxWidth = "15em";
                    that.domExport.addEventListener('click', function (e) {
                        let target = e.target;
                        if (target.nodeName == "A") {
                            that.save(target.innerHTML);

                            this.style.display = 'none';
                        }
                    }, false)
                }
                that.domExport.style.display = '';
            }
        },
        {
            title: '关于', cmd: 'about', svg: '<path d="m10.277 5.433-4.031.505-.145.67.794.145c.516.123.619.309.505.824L6.101 13.68c-.34 1.578.186 2.32 1.423 2.32.959 0 2.072-.443 2.577-1.052l.155-.732c-.35.31-.866.434-1.206.434-.485 0-.66-.34-.536-.939l1.763-8.278zm.122-3.673a1.76 1.76 0 1 1-3.52 0 1.76 1.76 0 0 1 3.52 0z"/>', action: function (that) {
                if (!that.domAbout) {
                    let html = [
                        '### 编辑器 (NetnrMD)',
                        'Ace 编辑器 + Marked 解析 + DOMPurify 清洗 + highlight 代码高亮 + tocbot 目录 + pangu 间隙  ',
                        '### 源码 (Source)',
                        '<https://github.com/netnr>',
                        '### 键盘快捷键 (Keyboard shortcuts)',
                        '按键 | 说明',
                        '--- | ---',
                        'Ctrl+F	| 搜索/Search',
                        'Ctrl+H	| 替换/Replace',
                    ];
                    that.objToolbarIcons.forEach(x => {
                        if (x.key) {
                            html.push(`${x.key} | ${x.title}/${x.cmd}`);
                            if (x.cmd == "header") {
                                for (let h = 1; h <= 6; h++) {
                                    html.push(`${x.key.replace("H", h)} | ${x.title}/${x.cmd}${h}`);
                                }
                            }
                        }
                    });

                    that.domAbout = netnrmd.popup("关于", netnrmd.render(html.join('\r\n')));
                    that.domAbout.querySelector('.np-body').classList.add('markdown-body');
                }
                that.domAbout.style.display = '';
            }
        },
        { title: '全屏', cmd: 'fullscreen', key: 'Ctrl+Alt+M', class: 'float-right', svg: '<path fill-rule="evenodd" d="M5.828 10.172a.5.5 0 0 0-.707 0l-4.096 4.096V11.5a.5.5 0 0 0-1 0v3.975a.5.5 0 0 0 .5.5H4.5a.5.5 0 0 0 0-1H1.732l4.096-4.096a.5.5 0 0 0 0-.707zm4.344 0a.5.5 0 0 1 .707 0l4.096 4.096V11.5a.5.5 0 1 1 1 0v3.975a.5.5 0 0 1-.5.5H11.5a.5.5 0 0 1 0-1h2.768l-4.096-4.096a.5.5 0 0 1 0-.707zm0-4.344a.5.5 0 0 0 .707 0l4.096-4.096V4.5a.5.5 0 1 0 1 0V.525a.5.5 0 0 0-.5-.5H11.5a.5.5 0 0 0 0 1h2.768l-4.096 4.096a.5.5 0 0 0 0 .707zm-4.344 0a.5.5 0 0 1-.707 0L1.025 1.732V4.5a.5.5 0 0 1-1 0V.525a.5.5 0 0 1 .5-.5H4.5a.5.5 0 0 1 0 1H1.732l4.096 4.096a.5.5 0 0 1 0 .707z"/>', action: function (that) { that.toggleFullScreen() } },
        { title: '分屏', cmd: 'splitscreen', class: 'float-right', svg: '<path d="M0 3a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V3zm8.5-1v12H14a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H8.5zm-1 0H2a1 1 0 0 0-1 1v10a1 1 0 0 0 1 1h5.5V2z"/>', action: function (that) { that.toggleView() } },
        { title: '主题', cmd: 'theme', class: 'float-right', svg: '<path d="M7 8a3.5 3.5 0 0 1 3.5 3.555.5.5 0 0 0 .625.492A1.503 1.503 0 0 1 13 13.5a1.5 1.5 0 0 1-1.5 1.5H3a2 2 0 1 1 .1-3.998.5.5 0 0 0 .509-.375A3.502 3.502 0 0 1 7 8zm4.473 3a4.5 4.5 0 0 0-8.72-.99A3 3 0 0 0 3 16h8.5a2.5 2.5 0 0 0 0-5h-.027z"/><path d="M11.286 1.778a.5.5 0 0 0-.565-.755 4.595 4.595 0 0 0-3.18 5.003 5.46 5.46 0 0 1 1.055.209A3.603 3.603 0 0 1 9.83 2.617a4.593 4.593 0 0 0 4.31 5.744 3.576 3.576 0 0 1-2.241.634c.162.317.295.652.394 1a4.59 4.59 0 0 0 3.624-2.04.5.5 0 0 0-.565-.755 3.593 3.593 0 0 1-4.065-5.422z"/>', action: function (that) { that.toggleTheme() } }
    ],

    /**
     * 表情
     */
    emoji: {
        "常用": "👍 👎 😄 🎉 😕 💗 🚀 👀",
        "笑脸和情感": "😀 😃 😄 😁 😆 😅 🤣 😂 🙂 🙃 😉 😊 😇 🥰 😍 🤩 😘 😗 ☺️ 😚 😙 😋 😛 😜 🤪 😝 🤑 🤗 🤭 🤫 🤔 🤐 🤨 😐 😑 😶 😏 😒 🙄 😬 🤥 😌 😔 😪 🤤 😴 😷 🤒 🤕 🤢 🤮 🤧 🥵 🥶 🥴 😵 🤯 🤠 🥳 😎 🤓 🧐 😕 😟 🙁 ☹️ 😮 😯 😲 😳 🥺 😦 😧 😨 😰 😥 😢 😭 😱 😖 😣 😞 😓 😩 😫 🥱 😤 😡 😠 🤬 😈 👿 💀 ☠️ 💩 🤡 👹 👺 👻 👽 👾 🤖 😺 😸 😹 😻 😼 😽 🙀 😿 😾 🙈 🙉 🙊 💋 💌 💘 💝 💖 💗 💓 💞 💕 💟 ❣️ 💔 ❤️ 🧡 💛 💚 💙 💜 🤎 🖤 🤍 💯 💢 💥 💫 💦 💨 🕳️ 💣 💬 👁️‍🗨️ 🗨️ 🗯️ 💭 💤",
        "人类和身体": "👋 🤚 🖐️ ✋ 🖖 👌 🤏 ✌️ 🤞 🤟 🤘 🤙 👈 👉 👆 🖕 👇 ☝️ 👍 👎 ✊ 👊 🤛 🤜 👏 🙌 👐 🤲 🤝 🙏 ✍️ 💅 🤳 💪 🦾 🦿 🦵 🦶 👂 🦻 👃 🧠 🦷 🦴 👀 👁️ 👅 👄 👶 🧒 👦 👧 🧑 👱 👨 🧔 👨‍🦰 👨‍🦱 👨‍🦳 👨‍🦲 👩 👩‍🦰 👩‍🦱 👩‍🦳 👩‍🦲 👱‍♀️ 👱‍♂️ 🧓 👴 👵 🙍 🙍‍♂️ 🙍‍♀️ 🙎 🙎‍♂️ 🙎‍♀️ 🙅 🙅‍♂️ 🙅‍♀️ 🙆 🙆‍♂️ 🙆‍♀️ 💁 💁‍♂️ 💁‍♀️ 🙋 🙋‍♂️ 🙋‍♀️ 🧏 🧏‍♂️ 🧏‍♀️ 🙇 🙇‍♂️ 🙇‍♀️ 🤦 🤦‍♂️ 🤦‍♀️ 🤷 🤷‍♂️ 🤷‍♀️ ⚕️ 👨‍⚕️ 👩‍⚕️ 👨‍🎓 👩‍🎓 👨‍🏫 👩‍🏫 👨‍⚖️ 👩‍⚖️ 👨‍🌾 👩‍🌾 👨‍🍳 👩‍🍳 👨‍🔧 👩‍🔧 👨‍🏭 👩‍🏭 👨‍💼 👩‍💼 👨‍🔬 👩‍🔬 👨‍💻 👩‍💻 👨‍🎤 👩‍🎤 👨‍🎨 👩‍🎨 👨‍✈️ 👩‍✈️ 👨‍🚀 👩‍🚀 👨‍🚒 👩‍🚒 👮 👮‍♂️ 👮‍♀️ 🕵️ 🕵️‍♂️ 🕵️‍♀️ 💂 💂‍♂️ 💂‍♀️ 👷 👷‍♂️ 👷‍♀️ 🤴 👸 👳 👳‍♂️ 👳‍♀️ 👲 🧕 🤵 👰 🤰 🤱 👼 🎅 🤶 💃 👣",
        "动物和自然": "🐵 🐒 🦍 🦧 🐶 🐕 🦮 🐕‍🦺 🐩 🐺 🦊 🦝 🐱 🐈 🦁 🐯 🐅 🐆 🐴 🐎 🦄 🦓 🦌 🐮 🐂 🐃 🐄 🐷 🐖 🐗 🐽 🐏 🐑 🐐 🐪 🐫 🦙 🦒 🐘 🦏 🦛 🐭 🐁 🐀 🐹 🐰 🐇 🐿️ 🦔 🦇 🐻 🐨 🐼 🦥 🦦 🦨 🦘 🦡 🐾 🦃 🐔 🐓 🐣 🐤 🐥 🐦 🐧 🕊️ 🦅 🦆 🦢 🦉 🦩 🦚 🦜 🐊 🐢 🦎 🐍 🐲 🐉 🦕 🦖 🐳 🐋 🐬 🐟 🐠 🐡 🦈 🐙 🐚 🐌 🦋 🐛 🐜 🐝 🐞 🦗 🕷️ 🕸️ 🦂 🦟 🦠 💐 🌸 💮 🏵️ 🌹 🥀 🌺 🌻 🌼 🌷 🌱 🌲 🌳 🌴 🌵 🌾 🌿 ☘️ 🍀 🍁 🍂 🍃",
        "食物和饮料": "🍇 🍈 🍉 🍊 🍋 🍌 🍍 🥭 🍎 🍏 🍐 🍑 🍒 🍓 🥝 🍅 🥥 🥑 🥔 🥕 🌽 🌶️ 🥒 🥬 🥦 🧄 🧅 🍄 🥜 🌰 🍞 🥐 🥖 🥨 🥯 🥞 🧇 🧀 🍖 🍗 🥩 🥓 🍔 🍟 🍕 🌭 🥪 🌮 🌯 🥙 🧆 🥚 🍳 🥘 🍲 🥣 🥗 🍿 🧈 🧂 🥫 🍱 🍘 🍙 🍚 🍛 🍜 🍝 🍠 🍢 🍣 🍤 🍥 🥮 🍡 🥟 🥠 🥡 🦀 🦞 🦐 🦑 🦪 🍦 🍧 🍨 🍩 🍪 🎂 🍰 🧁 🥧 🍫 🍬 🍭 🍮 🍯 🍼 🥛 ☕ 🍵 🍶 🍾 🍷 🍸 🍹 🍺 🍻 🥂 🥃 🥤 🧃 🧉 🧊 🥢 🍽️ 🍴 🥄 🔪 🏺",
        "旅行和地点": "🌍 🌎 🌏 🌐 🗺️ 🗾 🧭 🏔️ ⛰️ 🌋 🗻 🏕️ 🏖️ 🏜️ 🏝️ 🏞️ 🏟️ 🏛️ 🏗️ 🧱 🏘️ 🏚️ 🏠 🏡 🏢 🏣 🏤 🏥 🏦 🏨 🏩 🏪 🏫 🏬 🏭 🏯 🏰 💒 🗼 🗽 ⛪ 🕌 🛕 🕍 ⛩️ 🕋 ⛲ ⛺ 🌁 🌃 🏙️ 🌄 🌅 🌆 🌇 🌉 ♨️ 🎠 🎡 🎢 💈 🎪 🚂 🚃 🚄 🚅 🚆 🚇 🚈 🚉 🚊 🚝 🚞 🚋 🚌 🚍 🚎 🚐 🚑 🚒 🚓 🚔 🚕 🚖 🚗 🚘 🚙 🚚 🚛 🚜 🏎️ 🏍️ 🛵 🦽 🦼 🛺 🚲 🛴 🛹 🚏 🛣️ 🛤️ 🛢️ ⛽ 🚨 🚥 🚦 🛑 🚧 ⚓ ⛵ 🛶 🚤 🛳️ ⛴️ 🛥️ 🚢 ✈️ 🛩️ 🛫 🛬 🪂 💺 🚁 🚟 🚠 🚡 🛰️ 🚀 🛸 🛎️ 🧳 ⌛ ⏳ ⌚ ⏰ ⏱️ ⏲️ 🕰️ 🕛 🕧 🕐 🕜 🕑 🕝 🕒 🕞 🕓 🕟 🕔 🕠 🕕 🕡 🕖 🕢 🕗 🕣 🕘 🕤 🕙 🕥 🕚 🕦 🌑 🌒 🌓 🌔 🌕 🌖 🌗 🌘 🌙 🌚 🌛 🌜 🌡️ ☀️ 🌝 🌞 🪐 ⭐ 🌟 🌠 🌌 ☁️ ⛅ ⛈️ 🌤️ 🌥️ 🌦️ 🌧️ 🌨️ 🌩️ 🌪️ 🌫️ 🌬️ 🌀 🌈 🌂 ☂️ ☔ ⛱️ ⚡ ❄️ ☃️ ⛄ ☄️ 🔥 💧 🌊",
        "活动": "🎃 🎄 🎆 🎇 🧨 ✨ 🎈 🎉 🎊 🎋 🎍 🎎 🎏 🎐 🎑 🧧 🎀 🎁 🎗️ 🎟️ 🎫 🎖️ 🏆 🏅 🥇 🥈 🥉 ⚽ ⚾ 🥎 🏀 🏐 🏈 🏉 🎾 🥏 🎳 🏏 🏑 🏒 🥍 🏓 🏸 🥊 🥋 🥅 ⛳ ⛸️ 🎣 🤿 🎽 🎿 🛷 🥌 🎯 🪀 🪁 🎱 🔮 🧿 🎮 🕹️ 🎰 🎲 🧩 🧸 ♠️ ♥️ ♦️ ♣️ ♟️ 🃏 🀄 🎴 🎭 🖼️ 🎨 🧵 🧶",
        "对象": "👓 🕶️ 🥽 🥼 🦺 👔 👕 👖 🧣 🧤 🧥 🧦 👗 👘 🥻 🩱 🩲 🩳 👙 👚 👛 👜 👝 🛍️ 🎒 👞 👟 🥾 🥿 👠 👡 🩰 👢 👑 👒 🎩 🎓 🧢 ⛑️ 📿 💄 💍 💎 🔇 🔈 🔉 🔊 📢 📣 📯 🔔 🔕 🎼 🎵 🎶 🎙️ 🎚️ 🎛️ 🎤 🎧 📻 🎷 🎸 🎹 🎺 🎻 🪕 🥁 📱 📲 ☎️ 📞 📟 📠 🔋 🔌 💻 🖥️ 🖨️ ⌨️ 🖱️ 🖲️ 💽 💾 💿 📀 🧮 🎥 🎞️ 📽️ 🎬 📺 📷 📸 📹 📼 🔍 🔎 🕯️ 💡 🔦 🏮 🪔 📔 📕 📖 📗 📘 📙 📚 📓 📒 📃 📜 📄 📰 🗞️ 📑 🔖 🏷️ 💰 💴 💵 💶 💷 💸 💳 🧾 💹 ✉️ 📧 📨 📩 📤 📥 📦 📫 📪 📬 📭 📮 🗳️ ✏️ ✒️ 🖋️ 🖊️ 🖌️ 🖍️ 📝 💼 📁 📂 🗂️ 📅 📆 🗒️ 🗓️ 📇 📈 📉 📊 📋 📌 📍 📎 🖇️ 📏 📐 ✂️ 🗃️ 🗄️ 🗑️ 🔒 🔓 🔏 🔐 🔑 🗝️ 🔨 🪓 ⛏️ ⚒️ 🛠️ 🗡️ ⚔️ 🔫 🏹 🛡️ 🔧 🔩 ⚙️ 🗜️ ⚖️ 🦯 🔗 ⛓️ 🧰 🧲 ⚗️ 🧪 🧫 🧬 🔬 🔭 📡 💉 🩸 💊 🩹 🩺 🚪 🛏️ 🛋️ 🪑 🚽 🚿 🛁 🪒 🧴 🧷 🧹 🧺 🧻 🧼 🧽 🧯 🛒 🚬 ⚰️ ⚱️ 🗿",
        "符号": "🏧 🚮 🚰 ♿ 🚹 🚺 🚻 🚼 🚾 🛂 🛃 🛄 🛅 ⚠️ 🚸 ⛔ 🚫 🚳 🚭 🚯 🚱 🚷 📵 🔞 ☢️ ☣️ ⬆️ ↗️ ➡️ ↘️ ⬇️ ↙️ ⬅️ ↖️ ↕️ ↔️ ↩️ ↪️ ⤴️ ⤵️ 🔃 🔄 🔙 🔚 🔛 🔜 🔝 🛐 ⚛️ 🕉️ ✡️ ☸️ ☯️ ✝️ ☦️ ☪️ ☮️ 🕎 🔯 ♈ ♉ ♊ ♋ ♌ ♍ ♎ ♏ ♐ ♑ ♒ ♓ ⛎ 🔀 🔁 🔂 ▶️ ⏩ ⏭️ ⏯️ ◀️ ⏪ ⏮️ 🔼 ⏫ 🔽 ⏬ ⏸️ ⏹️ ⏺️ ⏏️ 🎦 🔅 🔆 📶 📳 📴 ♀️ ♂️ ⚧️ ✖️ ➕ ➖ ➗ ♾️ ‼️ ⁉️ ❓ ❔ ❕ ❗ 〰️ 💱 💲 ⚕️ ♻️ ⚜️ 🔱 📛 🔰 ⭕ ✅ ☑️ ✔️ ❌ ❎ ➰ ➿ 〽️ ✳️ ✴️ ❇️ ©️ ®️ ™️ #️⃣ *️⃣ 0️⃣ 1️⃣ 2️⃣ 3️⃣ 4️⃣ 5️⃣ 6️⃣ 7️⃣ 8️⃣ 9️⃣ 🔟 🔠 🔡 🔢 🔣 🔤 🅰️ 🆎 🅱️ 🆑 🆒 🆓 ℹ️ 🆔 Ⓜ️ 🆕 🆖 🅾️ 🆗 🅿️ 🆘 🆙 🆚 🈁 🈂️ 🈷️ 🈶 🈯 🉐 🈹 🈚 🈲 🉑 🈸 🈴 🈳 ㊗️ ㊙️ 🈺 🈵 🔴 🟠 🟡 🟢 🔵 🟣 🟤 ⚫ ⚪ 🟥 🟧 🟨 🟩 🟦 🟪 🟫 ⬛ ⬜ ◼️ ◻️ ◾ ◽ ▪️ ▫️ 🔶 🔷 🔸 🔹 🔺 🔻 💠 🔘 🔳 🔲",
        "旗帜": "🏁 🚩 🎌 🏴 🏳️ 🏳️‍🌈 🏴‍☠️"
    }
};

export { netnrmd };
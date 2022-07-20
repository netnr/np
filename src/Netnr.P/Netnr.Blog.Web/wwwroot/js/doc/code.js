window.$docsify = {
    noEmoji: true,
    auto2top: true,
    loadSidebar: true,
    routerMode: 'history',
    markdown: {
        renderer: {
            code: function (str, lang) {
                var ch = [];
                ch.push('<pre><code>');
                try {
                    str = hljs.getLanguage(lang)
                        ? hljs.highlight(str, { language: lang }).value
                        : hljs.highlightAuto(str).value;
                    ch.push(str);
                } catch (__) {
                    ch.push(str);
                }
                ch.push("</code></pre>");
                return ch.join('');
            }
        }
    },
    plugins: [
        function (hook) {
            // 每次开始解析 Markdown 内容时调用
            hook.beforeEach(function (content) {
                var domMain = document.getElementById("main");
                domMain.classList.add("markdown-body");

                return content;
            });

            // 每次路由切换时数据全部加载完成后调用
            hook.doneEach(function () {
                var domMain = document.getElementById("main");
                var domContent = domMain.parentNode;
                var domToc = domContent.querySelector(".toc");
                if (!domToc) {
                    domToc = document.createElement("div");
                    domToc.className = "toc";
                    domContent.appendChild(domToc);
                }
                domToc.innerHTML = "";

                tocbot.init({
                    tocElement: domToc,
                    contentElement: domMain,
                    headingSelector: 'h3, h4, h5, h6'
                });

                var toclink = domToc.querySelectorAll('a');
                if (toclink.length) {
                    toclink.forEach(item => {
                        item.title = item.innerText;
                        item.setAttribute('data-href', item.getAttribute('href'));
                        item.onclick = function () {
                            item.href = location.pathname + "?id=" + decodeURIComponent(item.hash).substring(1);
                            setTimeout(() => {
                                item.href = item.getAttribute('data-href');
                            }, 100);
                        }
                    })
                }

                if (toclink.length < 2 || document.body.scrollHeight - document.documentElement.clientHeight < 200) {
                    domToc.style.transform = "translateY(-100vh)";
                } else {
                    domToc.style.transform = "";
                }
            });
        }
    ]
};

const nd = {
    domAppNav: document.querySelector(".app-nav"),

    init: function () {
        nd.domAppNav.style.display = "";

        //菜单点击处理
        nd.domAppNav.addEventListener("click", function (e) {
            var target = e.target;

            let pns = location.pathname.split('/');
            let id = pns[3];
            let sid = pns[4];

            this.querySelectorAll('a').forEach(item => {
                if (item.contains(target)) {
                    switch (item.title) {
                        case "Edit":
                            if (sid == "") {
                                location.href = `/doc/form/${id}`
                            } else {
                                location.href = `/doc/item/${id}/${sid}`
                            }
                            break;
                        case "Delete":
                            if (sid == "") {
                                alert("请选择子页面删除")
                            } else if (confirm("确定删除？")) {
                                location.href = `/doc/itemdelete/${id}/${sid}`
                            }
                            break;
                        case "Word":
                            {
                                fetch(`/Doc/ToHtml/${id}`).then(resp => resp.text()).then(bodyContent => {
                                    fetch('/app/md/dist/netnrmd.css').then(resp => resp.text()).then(cssContent => {
                                        var html = `<!DOCTYPE html><html><head>
                                            <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
                                            <style type="text/css">${cssContent}</style>
                                        </head><body><div class="markdown-body">${bodyContent}</div></body></html>`;

                                        netnrmd.readyPackage("htmlDocx", () => {
                                            netnrmd.getScript("https://fastly.jsdelivr.net/npm/file-saver@2.0.5/dist/FileSaver.min.js", () => {
                                                saveAs(htmlDocx.asBlob(html), 'doc.docx');
                                            })
                                        })
                                    })
                                })
                            }
                            break;
                        case "Print":
                            {
                                document.body.classList.add('close');
                                var domToc = document.querySelector('.toc');
                                if (domToc) {
                                    domToc.style.visibility = "hidden";
                                }
                                window.print();
                                if (domToc) {
                                    domToc.style.visibility = "";
                                }
                            }
                            break;
                        case "Theme":
                            {
                                var c1 = "light", c2 = "dark", tv = "dark";
                                if (document.cookie.includes(".theme=dark")) {
                                    c1 = "dark";
                                    c2 = "light";
                                    tv = "light";
                                }
                                document.documentElement.className = document.documentElement.className.replaceAll(c1, c2);
                                document.cookie = ".theme=" + tv + ";path=/";
                            }
                            break;
                        default:
                            {
                                let ahref = item.getAttribute("data-href");
                                if (ahref) {
                                    location.href = ahref;
                                }
                            }
                            break;
                    }
                }
            });

            e.stopPropagation();
        });
    }
}

nd.init();
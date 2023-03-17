window.$docsify = {
    noEmoji: true,
    auto2top: true,
    loadSidebar: true,
    routerMode: "history",
    markdown: {
        renderer: {
            code: function (str, lang) {
                let htm = hljs.getLanguage(lang)
                    ? hljs.highlight(str, { language: lang }).value
                    : hljs.highlightAuto(str).value;
                return `<pre><code>${htm}</code></pre>`;
            }
        }
    },
    plugins: [
        function (hook) {
            // 每次开始解析 Markdown 内容时调用
            hook.beforeEach(function (content) {
                let domMain = document.getElementById("main");
                domMain.classList.add("markdown-body");

                return content;
            });

            // 每次路由切换时数据全部加载完成后调用
            hook.doneEach(function () {
                let domMain = document.getElementById("main");
                let domContent = domMain.parentNode;
                let domToc = domContent.querySelector(".toc");
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

                let toclink = domToc.querySelectorAll('a');
                if (toclink.length) {
                    toclink.forEach(item => {
                        item.title = item.innerText;
                        item.dataset.href = item.getAttribute('href');
                        item.onclick = function () {
                            item.href = `${location.pathname}?id=${decodeURIComponent(item.hash).substring(1)}`;

                            setTimeout(() => { item.href = item.dataset.href }, 100);
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

const nrPage = {
    domAppNav: document.querySelector(".app-nav"),

    init: function () {
        nrPage.domAppNav.style.removeProperty('display');

        //菜单点击处理
        nrPage.domAppNav.addEventListener("click", async function (e) {
            let target = e.target;

            let pns = location.pathname.split('/');
            let id = pns[3];
            let sid = pns[4];

            let allLink = this.querySelectorAll('a');

            for (let item of allLink) {
                if (item.contains(target)) {

                    switch (item.title) {
                        case "Edit":
                            if (sid == "") {
                                location.href = `/doc/edit/${id}`
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
                                let htmlContent = await fetch(`/Doc/ToHtml/${id}`).then(resp => resp.text());
                                let styleContent = await fetch('/file/md/netnrmd.css').then(resp => resp.text())

                                let htmls = ['<!DOCTYPE html>',
                                    '<html><head>',
                                    '<meta name="viewport" content="width=device-width, initial-scale=1.0" /><meta charset="utf-8" />',
                                    '<style type="text/css">' + styleContent + '</style>',
                                    '</head><body>',
                                    '<div class="markdown-body">' + htmlContent + '</div>',
                                    '</body></html>'
                                ];
                                let result = htmls.join('\r\n');

                                await nrcDocsify.importScript('https://npmcdn.com/html-docx-js@0.3.1/dist/html-docx.js');
                                await nrcDocsify.importScript("https://npmcdn.com/file-saver@2.0.5/dist/FileSaver.min.js");
                                saveAs(htmlDocx.asBlob(result), 'doc.docx');
                            }
                            break;
                        default:
                            {
                                let ahref = item.dataset.href;
                                if (ahref) {
                                    location.href = ahref;
                                }
                            }
                            break;
                    }
                }
            }

            e.stopPropagation();
        });
    },
}

nrPage.init();
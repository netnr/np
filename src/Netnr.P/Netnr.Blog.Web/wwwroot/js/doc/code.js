localStorage.setItem('DARK_LIGHT_THEME', document.cookie.includes(".theme=dark") ? "dark" : "light");

window.$docsify = {
    noEmoji: true,
    auto2top: true,
    loadSidebar: true,
    routerMode: 'history',
    markdown: {
        renderer: {
            code: function (str, lang) {
                var ch = [];
                ch.push("<pre><code>");
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
            var tocnode = document.querySelector('.toc');
            hook.doneEach(function () {

                tocbot.init({
                    tocSelector: '.toc',
                    contentSelector: '#main',
                    headingSelector: 'h2, h3, h4, h5, h6'
                });

                if (tocnode.children.length) {
                    tocnode.classList.add("nr-toc");
                    tocnode.querySelectorAll('a').forEach(item => {
                        item.title = item.innerText;
                        item.setAttribute('data-href', item.getAttribute('href'));
                        item.onclick = function () {
                            item.href = location.pathname + "?id=" + decodeURIComponent(item.hash).substring(1);
                            setTimeout(() => {
                                item.href = item.getAttribute('data-href');
                            }, 100);
                        }
                    });
                }

                let toclink = tocnode.querySelectorAll('a');
                if (toclink.length < 2 || document.body.scrollHeight - document.documentElement.clientHeight < 200) {
                    tocnode.style.transform = "translateY(-100vh)";
                } else {
                    tocnode.style.transform = "";
                }
            });
        }
    ]
};

const nd = {
    sourceList: [
        "https://npm.elemecdn.com/docsify-darklight-theme@3.2.0/dist/style.min.css",
        "https://npm.elemecdn.com/docsify@4.12.2/lib/docsify.min.js",
        "https://npm.elemecdn.com/@highlightjs/cdn-assets@11.5.0/styles/vs2015.min.css",
        "https://npm.elemecdn.com/@highlightjs/cdn-assets@11.5.0/highlight.min.js",
        "https://npm.elemecdn.com/docsify-darklight-theme@3.2.0/dist/index.min.js"
    ],
    createNode: function (nn, html) {
        let em = document.createElement(nn);
        em.innerHTML = html;
        return em;
    },
    domAppNav: document.querySelector(".app-nav"),

    download: function (content, file) {
        var aTag = document.createElement('a');
        aTag.target = '_blank';
        aTag.download = file;
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

    init: function () {
        let pas = [];
        nd.sourceList.forEach(u => {
            pas.push(fetch(u).then(x => x.text()));
        });

        Promise.all(pas).then(res => {

            var head = document.getElementsByTagName("HEAD")[0];

            for (let i = 0; i < res.length; i++) {
                let suri = nd.sourceList[i];
                let text = res[i];
                if (text.includes("fonts.googleapis.com")) {
                    text = "*" + text.split(';*')[1];
                }
                text = text.replaceAll("cdn.jsdelivr.net/npm/", "npm.elemecdn.com/");
                head.appendChild(nd.createNode(suri.endsWith('.js') ? "SCRIPT" : "STYLE", text));
            }

            nd.domAppNav.style.display = "";
        });

        // style
        let st = document.createElement("STYLE");
        st.innerHTML = `
html {
    --bodyFontSize: 16px !important;
}

.sidebar ul li p.active > a {
    border-right: 2px solid;
    font-weight: 600;
    color: #42b983;
}

.sidebar ul li.active > a {
    font-weight: 400 !important;
}

.markdown-section {
    padding-top: 50px !important;
}

.app-nav li ul {
    color: currentColor;
    background-color: var(--codeBackgroundColor) !important;
}

/*目录*/
@media (min-width: 800px) {
    .nr-toc {
        border: 1px solid #ddd;
        display: block !important;
    }
}

.toc {
    display: none;
    position: fixed;
    z-index: 2;
    right: 2em;
    bottom: 12em;
    max-width: 200px;
    max-height: 35vh;
    overflow-y: auto;
    border-left: none;
    padding: 0 15px 0 0;
    color: currentColor;
    background-color: var(--codeBackgroundColor);
    border-radius: 0 4px 4px 0;
}

.toc-list {
    margin: 0;
    overflow: hidden;
    position: relative;
    padding-left: 15px;
}

    .toc-list li {
        list-style: none;
        line-height: 2rem;
    }

.toc-link {
    display: block;
    color: currentColor;
    overflow: hidden;
    text-decoration: none;
    text-overflow: ellipsis;
    white-space: nowrap;
    height: 100%;
}

.is-collapsible {
    max-height: 1000px;
    overflow: hidden;
    transition: all 300ms ease-in-out;
}

.is-collapsed {
    max-height: 0;
}

.is-active-link {
    font-weight: 700;
}

.toc-link::before {
    background-color: #eee;
    content: " ";
    display: inline-block;
    height: inherit;
    left: 0;
    margin-top: -1px;
    position: absolute;
    width: 3px;
}

.is-active-link::before {
    background-color: orange;
}

.toc::-webkit-scrollbar {
    width: 8px;
    height: 8px;
}

.toc::-webkit-scrollbar-corner {
    background-color: inherit;
}

.toc::-webkit-scrollbar-thumb {
    background: rgba(0,0,0,0.12);
    border-radius: 5px;
    border: 1px solid transparent;
    height: 140px;
    background-clip: content-box;
}
`;
        document.head.appendChild(st);

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
                                fetch(`/Doc/ToHtml/${id}`).then(resp => resp.text()).then(res => {
                                    var html = `<!DOCTYPE html>
                                    <html>
                                        <head>
                                            <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
                                            <style type="text/css"></style>
                                        </head>
                                        <body>
                                            <div class="markdown-body">${res}</div>
                                        </body>
                                    </html>`;

                                    let st = document.createElement("SCRIPT");
                                    st.src = 'https://npm.elemecdn.com/html-docx-js@0.3.1/dist/html-docx.js';
                                    st.onload = function () {
                                        nd.download(htmlDocx.asBlob(html), 'doc.docx');
                                    }
                                    st.onerror = function () {
                                        alert("导出失败");
                                    }
                                    document.head.appendChild(st);
                                })
                            }
                            break;
                        case "Print":
                            {
                                document.body.classList.add('close');
                                window.print();
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
        });
    }
}

nd.init();
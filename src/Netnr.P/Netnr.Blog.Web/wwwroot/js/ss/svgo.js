nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor.getBoundingClientRect().top - 30;
    nr.domEditor.style.height = vh + 'px';
    nr.domCardPreview.style.height = vh + 'px';
}

nr.onReady = function () {

    me.init().then(() => {
        me.editor = me.create(nr.domEditor, {
            language: 'html',
            wordWrap: 'on',
        });

        me.editor.addCommand(monaco.KeyCode.PauseBreak, function () {
            page.preview();
        });

        nr.domBtnDownload.addEventListener('click', function () {
            if (page.svgOut) {
                if (typeof page.svgOut == "string") {
                    nr.download(page.svgOut, "icon.svg");
                } else {
                    if (page.svgOut.length == 1) {
                        nr.download(page.svgOut[0].data, page.svgOut[0].path);
                    } else {
                        var zip = new JSZip();
                        page.svgOut.forEach(item => zip.file(item.path, item.data));
                        zip.generateAsync({ type: "blob" }).then(function (content) {
                            nr.download(content, "icon.zip");
                        });
                    }
                }
            } else {
                nr.alert("请选择或拖拽 SVG 文件后再下载")
            }
        });
    })

    //接收文件
    nr.receiveFiles(function (files) {
        page.readFiles(files);
    }, nr.domTxtFile);
}

var page = {
    svgJson: [],
    svgOut: null,
    readFiles: function (files) {
        var arr = [];
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            if (file.name.toLowerCase().includes(".svg")) {
                arr.push(page.readFile(file));
            }
        }

        Promise.all(arr).then(json => {
            page.svgJson = json;

            nr.domTxtFile.value = "";
            var merge = nr.domSeType.value;

            page.svgOut = page.optimization(json, merge);

            nr.domEditor.classList.remove('invisible');
            nr.domEditor.classList.add('border');
            nr.domCardPreview.classList.remove('invisible');

            var html = [];
            if (merge == "1") {
                html.push(page.svgOut);
                html.push("\n<!--使用-->");

                var vdom = document.createElement('div');
                vdom.innerHTML = page.svgOut;
                vdom.querySelectorAll('symbol').forEach(node => {
                    var id = node.getAttribute('id');
                    html.push(`<svg><use xlink:href="#${node.id}"></use></svg>`);
                });
            } else {
                page.svgOut.forEach(item => {
                    html.push(item.data + "\n");
                });
            }
            me.keepSetValue(me.editor, html.join("\n"));
            page.preview();
        });
    },
    optimization: function (json, merge) {
        var results = [];
        json.forEach(item => {
            var result = svgo.optimize(item.text, {
                multipass: true,
                path: item.path
            });

            result.data = result.data.replace(' class="icon" ', ' ').replace("<defs><style/></defs>", "");

            //合并
            if (merge == 1) {
                result.data = result.data.replace('<svg ', `<symbol id="${result.path.replace(".svg", "")}" `)
                    .replace(' xmlns="http://www.w3.org/2000/svg"', '')
                    .replace(/ width="(\d+)"/, "")
                    .replace(/ height="(\d+)"/, "")
                    .replace(/ width="(\d+.\d+)"/, "")
                    .replace(/ height="(\d+.\d+)"/, "")
                    .replace("</svg>", "</symbol>");
                results.push(result.data);
            } else {
                results.push(result);
            }
        })

        //合并
        if (merge == 1) {
            results.splice(0, 0, '<svg version="1.1" xmlns="http://www.w3.org/2000/svg" style="display:none;">');
            results.push('</svg>');
            results = results.join('');
        }

        return results;
    },
    readFile: function (file) {
        return new Promise((resolve) => {
            var reader = new FileReader();
            reader.onload = function (e) {
                var obj = { path: file.name, text: e.target.result };
                resolve(obj);
            };
            reader.readAsText(file);
        });
    },
    preview: function () {
        var val = me.editor.getValue();
        nr.domCardPreview.innerHTML = `<style>
.nr-card-preview svg {
    height: 4em;
    margin: .8em 0 .8em .8em;
    max-width: 4em;
}

.nr-card-preview svg:hover {
    transition: transform 1.5s;
    transform: rotate(360deg);
}
</style> ${val}`;
    }
};
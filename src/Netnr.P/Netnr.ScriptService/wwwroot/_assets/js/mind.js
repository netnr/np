var body = document.body;
body.setAttribute('ng-app', "MindMap");
body.setAttribute('ng-controller', "MainController");

//完成时
angular.module('MindMap', ['kityminderEditor']).config(function (configProvider) {
    configProvider.set('imageUpload', '/draw/code/upload');
}).controller('MainController', function ($scope) {
    $scope.initEditor = function (editor, minder) {
        window.editor = editor;
        window.minder = minder;

        minder.on('contentchange ', function () {
            localStorage.setItem(km.storageKey, JSON.stringify(minder.exportJson()))
        });

        km.inject();
    };
});

var km = {
    storageKey: "/mind/content",

    inject: function () {
        setTimeout(() => {
            var ul = document.querySelector('.ng-isolate-scope ul');

            var liMenu = document.createElement('li');
            liMenu.innerHTML = `
<div class="dropdown">
    <button class="btn btn-primary btn-sm dropdown-toggle" data-toggle="dropdown">
        菜单 <span class="caret"></span>
    </button>
    <ul class="dropdown-menu" id="mmmenu">
        <li><a data-mc="new" href="javascript:void(0)">新建</a></li>
        <li><a data-mc="export" href="javascript:void(0)">导出</a></li>
        <li><a data-mc="import" href="javascript:void(0)">导入</a></li>
    </ul>
</div>
`
            ul.insertBefore(liMenu, ul.firstElementChild);

            //初始化回调
            km.init();
        }, 200)
    },

    init: function () {
        //菜单
        $('#mmmenu').click(function (e) {
            e = e || window.event;
            var target = e.target || e.srcElement, mc;
            $(this).children().each(function () {
                if (this.contains(target)) {
                    mc = $(this).find('a').attr('data-mc');
                    return false;
                }
            })
            switch (mc) {
                case "new":
                    km.new()
                    break;
                case "export":
                    $('#ehModalExport').modal();
                    break;
                case "import":
                    $('#fileImport').val('');
                    $('#txtImprot').val('');
                    $('#ehModalImport').modal();
                    break;
            }
        });

        //导出
        $('#btnExport').click(function () {
            var ef = $('input[name="efs"]:checked').val();
            if (ef) {
                km.export(ef);
            } else {
                alert("请选择一种导出格式")
            }
        })

        //导入选择
        $('#fileImport').change(function () {
            var file = this.files[0];
            if (file) {
                if (file.size > 1024 * 1024 * 2) {
                    alert('Max size 2MB');
                } else {
                    var r = new FileReader();
                    r.onload = function () {
                        $('#txtImprot').val(this.result)
                    }
                    r.readAsText(file);
                }
            }
        })
        //导入
        $('#btnImport').click(function () {
            var txt = $('#txtImprot').val().trim();
            if (txt == "") {
                alert('导入内容不能为空')
            } else {
                try {
                    var json = JSON.parse(txt);
                    editor.minder.importJson(json);
                    $('#ehModalImport').modal('hide');
                } catch (e) {
                    try {
                        var ismd = 0;
                        $.each(txt.split('\n'), function () {
                            if (this.trim().indexOf('# ') >= 0) {
                                ismd++;
                            }
                        })
                        editor.minder.importData(ismd ? 'markdown' : 'text', txt)
                        $('#ehModalImport').modal('hide');
                    } catch (e) {
                        console.log(e);
                        alert('导入失败');
                    }
                }
            }
        })

        //恢复本地
        try {
            var content = localStorage.getItem(km.storageKey);
            if (content) {
                editor.minder.importJson(JSON.parse(content));
            }
        } catch (e) { console.log(e) }

        //导出格式
        km.exportInit.create();
    },

    //新建
    new: function () {
        if (confirm("确定删除当前记录？")) {
            localStorage.setItem(km.storageKey, "");
            location.reload(false)
        }
    },

    //导出项
    exportInit: {
        format: {
            png: { ext: "png", txt: "PNG 图片文件" },
            text: { ext: "txt", txt: "TXT 文本格式" },
            markdown: { ext: "md", txt: "Markdown 文档" },
            json: { ext: "json", txt: "JSON 字符串" },
            svg: { ext: "svg", txt: "SVG 适量图形" }
        },
        create: function () {
            var htm = [];
            $.each(km.exportInit.format, function (k, v) {
                htm.push('<div class="radio"><label><input type="radio" name="efs" value="' + k + '"> ' + v.txt + '（*.' + v.ext + '）</label></div>');
            });
            $('#mbef').html(htm.join('')).css({ "padding": "10px 30px", "font-size": "1.5rem" });
        }
    },

    export: function (format) {
        editor.minder.exportData(format).then(function (res) {
            var filename = "export." + km.exportInit.format[format].ext;
            if (format == "json") {
                res = JSON.stringify(JSON.parse(res), null, 2);
            }
            var blob;
            switch (format) {
                case "png":
                    blob = km.base64ToBlob(res);
                    break;
                default:
                    if (typeof blob != "object") {
                        blob = new Blob([res]);
                    }
                    break;
            }
            if (blob) {
                km.down(blob, filename);
                $('#ehModalExport').modal('hide');
            } else {
                alert('error')
            }
        }, function (err) {
            console.log(err);
        })
    },
    //下载
    down: function (blob, fileName) {
        var aEle = document.createElement("a");
        aEle.download = fileName;
        aEle.href = URL.createObjectURL(blob);
        document.body.appendChild(aEle);
        aEle.click();
        document.body.removeChild(aEle);
    },
    base64ToBlob(code) {
        let parts = code.split(';base64,');
        let contentType = parts[0].split(':')[1];
        let raw = window.atob(parts[1]);
        let rawLength = raw.length;

        let uInt8Array = new Uint8Array(rawLength);

        for (let i = 0; i < rawLength; ++i) {
            uInt8Array[i] = raw.charCodeAt(i);
        }
        return new Blob([uInt8Array], { type: contentType });
    },
}
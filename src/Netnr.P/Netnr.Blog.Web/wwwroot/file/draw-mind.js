var rowId = location.pathname.split('/').pop();

var body = document.body;
body.setAttribute('ng-app', "MindMap");
body.setAttribute('ng-controller', "MainController");

//完成时
window.addEventListener('DOMContentLoaded', function () {
    angular.module('MindMap', ['kityminderEditor'])
        .config(function (configProvider) {
            configProvider.set('imageUpload', '/draw/MindUpload');
        })
        .controller('MainController', function ($scope) {
            $scope.initEditor = function (editor, minder) {
                window.editor = editor;
                window.minder = minder;

                km.inject();

                $('#LoadingMask').fadeOut(200);
            };
        });
}, false);

var km = {
    RequestActive: null,

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
        <li><a data-mc="save" href="javascript:void(0)">保存 (Ctrl+S)</a></li>
        <li><a data-mc="export" href="javascript:void(0)">导出</a></li>
        <li><a data-mc="import" href="javascript:void(0)">导入</a></li>
        <li class="divider"></li>
        <li><a href="/draw/discover">Discover</a></li>
        <li><a href="/">Netnr</a></li>
    </ul>
</div>
`
            var liTitle = document.createElement('li');
            liTitle.classList.add("hidden-xs")
            liTitle.style.float = "right";
            liTitle.innerHTML = `
<div class="form-inline">
    <div class="form-group">
        <input class="form-control input-sm" id="DrName" value="" placeholder="标题名称" maxlength="30" />
    </div>
    <a class="btn btn-link btn-sm" href="/draw/discover">Discover</a>
    <a class="btn btn-link btn-sm" href="/">Netnr</a>
</div>
`

            ul.insertBefore(liMenu, ul.firstElementChild);
            ul.appendChild(liTitle)

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
                case "save":
                    km.save()
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

        //编辑
        if (rowId != "") {
            $.ajax({
                url: `/draw/EditorOpen/${rowId}`,
                dataType: 'json',
                success: function (result) {
                    if (result.code == 200) {
                        try {
                            document.title = result.data.DrName + " - " + document.title;
                            $('#DrName').val(result.data.DrName);
                            editor.minder.importJson(JSON.parse(result.data.DrContent));
                        } catch (e) { console.log(e) }
                    } else {
                        alert(result.msg);
                    }
                }
            })
        }

        //导出格式
        km.exportInit.create();

        //快捷键注册
        $(window).on('keydown', function (e) {
            e = e || window.event;
            if (e.keyCode == 83 && e.ctrlKey) {
                km.stopDefault(e);
                km.save();
            }
        });
    },

    //阻止浏览器默认行为
    stopDefault: function (e) {
        if (e && e.preventDefault) {
            e.preventDefault()
        } else {
            window.event.returnValue = false
        }
    },

    //保存
    save: function () {
        if (km.RequestActive) {
            console.log('Operating too fast: ' + km.RequestActive);
            return;
        }

        var dn = $('#DrName').val().trim();
        var dc = JSON.stringify(editor.minder.exportJson());
        if (dn == "") {
            alert("请输入标题")
            return
        }

        km.RequestActive = 'save';

        $.ajax({
            url: `/draw/EditorSave/${rowId}`,
            data: {
                DrType: 'mind',
                filename: dn,
                xml: dc
            },
            type: "post",
            dataType: 'json',
            success: function (result) {
                if (result.code == 200) {
                    if (result.data) {
                        location.href = "/draw/code/" + result.data;
                    } else {
                        alert(result.msg);
                    }
                } else {
                    alert(result.msg);
                }
            },
            error: function (e) {
                console.log(e)
                alert('error')
            },
            complete: function () {
                km.RequestActive = null;
            }
        })
    },
    //导出项
    exportInit: {
        format: {
            png: { ext: "png", txt: "PNG 图片文件" },
            text: { ext: "text", txt: "Text 文本格式" },
            markdown: { ext: "markdown", txt: "Markdown 文档" },
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
            var filename = $('#DrName').val().trim();
            if (filename == "") {
                filename = "export";
            }
            filename += "." + km.exportInit.format[format].ext;
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
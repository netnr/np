var ti = {
    tti: new tti(),
    editor: null,
    init: function () {

        //转图片
        $('#btnToImage').click(function () {
            var txt = ti.editor.getValue();
            if (txt != "") {
                ti.showImage(txt);
            }
        });
        $('#txtFile1').change(function () {
            var file = this.files[0];
            if (file) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    ti.editor.setValue(e.target.result);
                    $('#btnToImage')[0].click();
                };
                reader.readAsText(file);
            }
        })

        //接收文件
        ss.receiveFiles(function (files) {
            var file = files[0]
            //解析
            if (file.type.indexOf("image") == 0) {
                ti.showText(URL.createObjectURL(file));
            } else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    ti.editor.setValue(e.target.result);
                    $('#btnToImage')[0].click();
                };
                reader.readAsText(file);
            }
        });

        //转文本
        $('#txtFile2').change(function () {
            var file = this.files[0];
            if (file && file.type.indexOf("image") == 0) {
                ti.showText(URL.createObjectURL(file));
            }
        })
        $('#btnToText').click(function () {
            var uri = $('#txtUri').val();
            if (uri != "") {
                ti.showText(uri);
            }
        });
        $('#btnToTextDemo').click(function () {
            var uri = 'https://img13.360buyimg.com/ddimg/jfs/t1/204958/32/5863/175214/613ab6d5E0e894894/10cf6ad9fc63e04e.png';
            $('#txtUri').val(uri);
            $('#btnToText')[0].click();
        });

        ti.showCode('');
    },
    viewFile: function (file, cb) {
        if (file) {
            if (file.type.indexOf("image") == 0) {
                ti.showText(URL.createObjectURL(file));
                cb && cb();
            } else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    ti.editor.setValue(e.target.result);
                    $('#btnToImage')[0].click();
                    cb && cb();
                };
                reader.readAsText(file);
            }
        }
    },
    showCode: function (code) {
        if (ti.editor) {
            ti.editor.setValue(code);
        } else {
            require(['vs/editor/editor.main'], function () {
                ti.editor = monaco.editor.create(document.getElementById("editor"),
                    ss.meConfig({
                        value: code,
                        wordWrap: "on",
                        language: 'html',
                        minimap: { enabled: false }
                    }))
            });
        }
    },
    showText: function (src) {
        var nv = $('.nrView').removeClass('d-none').children();

        ti.showCode("正在解析为文本，请稍等 ... \r\n非文字转成的图片解析后是乱码 ... \r\n图片越大（亮）解析越慢 ... ");
        nv.first().attr('src', src);
        nv.last().attr('href', src);
        setTimeout(function () {
            try {
                ti.tti.asText(nv.first()[0], function (txt) {
                    ti.showCode(txt)
                });
            } catch (e) {
                console.log(e);
                ti.showCode(e + "");
            }
        }, 1000)
    },
    showImage: function (txt) {
        var $base64 = ti.tti.asImage(txt).toImage();
        var nv = $('.nrView').removeClass('d-none').children();
        nv.first().attr('src', $base64);
        nv.last().attr('href', nv.first().attr('src'));
    }
}

ti.init();
var pg = {
    //压缩配置
    options: {
        width: null,
        height: null,
        quality: 0.7
    },

    init: function () {
        pg.zip = new JSZip();

        //接收文件
        ss.receiveFiles(function (files) {
            pg.addFiles(files);
        }, "#txtFile");

        //更新压缩配置
        $('.nrOptionsOk').click(function (e) {
            var txts = $(this).parent().find('input');

            pg.options = {
                width: parseInt(txts.eq(0).val()),
                height: parseInt(txts.eq(1).val()),
                quality: parseFloat(txts.eq(2).val())
            };
            if (isNaN(pg.options.width) || pg.options.width <= 0) {
                pg.options.width = null
                txts.eq(0).val('')
            }
            if (isNaN(pg.options.height) || pg.options.height <= 0) {
                pg.options.height = null
                txts.eq(1).val('')
            }
            if (isNaN(pg.options.quality) || pg.options.quality < 0 || pg.options.quality > 1) {
                pg.options.quality = 0.7
                txts.eq(1).val(pg.options.quality);
            }
        });

        //重置压缩配置
        $('.nrOptionsReset').click(function (e) {
            if (e && e.stopPropagation) {
                e.stopPropagation()
            } else {
                window.event.cancelBubble = true
            }

            $(this).parent().find('input').each(function () {
                this.value = this.defaultValue;
            });
            pg.options = {
                width: null,
                height: null,
                quality: 0.7
            }
        });
    },

    //添加文件
    addFiles: function (files) {
        var ignorefile = [];
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            if (file.type.indexOf("image/") == 0) {
                lrz(file, pg.options).then(function (rst) {

                    var size1 = ss.sizeOf(rst.origin.size);
                    var size2 = ss.sizeOf(rst.file.size);
                    var ratio = ((rst.origin.size - rst.file.size) / rst.origin.size * 100).toFixed(2) + "%";

                    var htm = [];
                    htm.push('<tr>');
                    htm.push('<td><img src="' + rst.base64 + '" style="max-height:20px;max-width:50px" /> &nbsp; <a href="javascript:void(0)" download="' + rst.origin.name + '" >' + rst.origin.name + '</a> &nbsp; <a href="javascript:void(0)" target="_blank" >查看</a></td>');
                    htm.push('<td>-' + ratio + '（' + size1 + ' ➜ <span class="text-success">' + size2 + '</span>）</td>');
                    htm.push('</tr>');
                    var tr = $(htm.join(''));
                    tr.find('a').attr('href', URL.createObjectURL(rst.file));
                    $('.nrTable').append(tr);

                    pg.zip.file(pg.checkSafeName(rst.origin.name), rst.file, { base64: true });
                    pg.zip.generateAsync({ type: "blob" }).then(function (content) {
                        $('.nrDownAll').attr('href', URL.createObjectURL(content))
                    });
                }).catch(err => {
                    console.log(err);
                })
            } else {
                ignorefile.push(file.name);
            }
        }
        if (ignorefile.length) {
            bs.alert('<textarea class="form-control text-nowrap" rows="10">' + ignorefile.join('\r\n') + '</textarea>');
            bs.obj.alert._dialog.classList.remove('modal-sm');
        }
        $('#txtFile').val('');
        $('.nrDragTip').addClass('d-none');
        $('.nrTable').removeClass('d-none');
    },

    checkSafeName: function (filename) {
        var newfilename = filename, num = 1, index = filename.lastIndexOf("."),
            name = filename.substr(0, index), ext = filename.substr(index);
        while (newfilename in pg.zip.files) {
            newfilename = name + "(" + (num++) + ")" + ext;
        }
        return newfilename;
    }
}

pg.init();
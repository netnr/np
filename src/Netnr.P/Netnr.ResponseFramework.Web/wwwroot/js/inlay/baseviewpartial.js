//完成事件
window.addEventListener('DOMContentLoaded', function () {
    //绑定数据
    setTimeout(z.FormAttrAjax, 0);
}, false);

//新增
z.button("add", function () {
    //重置
    z.FormRequired('reset');
    //清空
    z.FormClear();
});

//编辑
z.button("edit", function () {
    //重置
    z.FormRequired('reset');
    //清空
    z.FormClear();
});

//查看
z.button("see", function () {
    z.FormRequired('reset');
});

//批处理
z.button("batch", function () {
    z.batchButtonSwitch();
});

//关闭批处理
z.button("batch_close", function () {
    z.batchButtonSwitch();
});

//批处理按钮切换
z.batchButtonSwitch = function (isopen) {
    isopen = isopen == null ? !(z.batchControlStatus || false) : isopen;
    z.batchControlStatus = isopen;
    $('#BtnMenu').children().each(function () {
        var that = $(this);
        if (this.nodeName != "BUTTON") {
            that = $(this).children();
        }
        if (this.id.toLowerCase().indexOf("batch_") >= 0) {
            if (isopen) {
                that.removeClass('hidden');
            } else {
                that.addClass('hidden');
            }
        } else {
            if ("m_Query m_Reload m_Full_Screen".indexOf(this.id) == -1) {
                if (isopen) {
                    that.addClass('disabled')
                } else {
                    that.removeClass('disabled')
                }
            }
        }
    });
    if (isopen) {
        $('#m_Batch').addClass('hidden');
    } else {
        $('#m_Batch').removeClass('hidden');
    }
}

//全屏
z.button("full_screen", function () {
    z.FullScreen.iframe();
})

//导出
z.button("export", function () {
    GlobalExport();
});
/**
 * 公共导出
 * @param {any} url 导出的源，可选
 * @param {any} callback 导出回调，回填参数，可选
 */
function GlobalExport(url, callback) {
    var da = {
        //导出标题
        title: document.title,
        //不分页
        pagination: 0,
        //处理类型
        handleType: "export"
    };

    //导出回调，自定义导出事件
    if (typeof ExportCallBack == "function") {
        if (ExportCallBack(da) == false) {
            return false;
        }
    }
    if (typeof callback == "function") {
        if (callback(da) == false) {
            return false;
        }
    }

    var mod = z.Modal();
    mod.title = "<i class='fa fa-level-down green'></i><span>导出</span>";

    var htm = [];
    htm.push('<div class="text-center h2">')
    htm.push('<img src="/images/loading.gif" style="vertical-align:sub;" />');
    htm.push('<h3>正在生成文件，请稍等 . . .</h3>');
    htm.push('</div>');

    mod.content = htm.join('');
    mod.showFooter = false;
    mod.append();
    mod.modal.attr('data-backdrop', 'static');
    mod.modal.find('.modal-header').addClass('hidden');
    mod.modal.on('hidden.bs.modal', function () { $('#' + this.id).remove() });
    mod.show();

    url = url || ('/io/export?tableName=' + z.TableName);
    $.ajax({
        url: url,
        data: da,
        dataType: 'json',
        success: function (data) {
            var mb = mod.modal.find('.modal-body');
            mod.modal.find('.modal-header').removeClass('hidden');
            if (data.code == 200) {
                mb.html('<h2 class="text-center"><a class="btn btn-success btn-lg" href="' + data.data + '" ><i class="fa fa-download"></i> 点击下载文件</a></h2>');
            } else {
                mb.html('<h4 class="text-center red">' + data.msg + '</h4>');
            }
        }
    })
}
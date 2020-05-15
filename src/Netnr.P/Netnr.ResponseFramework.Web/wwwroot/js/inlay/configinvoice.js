//按钮切换
z.buttonViewSwitch = function (btns, isEnable) {
    var child = $('#BtnMenu').children();
    child.each(function () {
        if (!$(this).hasClass('pull-right') && this.nodeName == "BUTTON") {
            var id = this.id.replace("m_", "").toLowerCase();
            $(this)[isEnable ? "addClass" : "removeClass"]('disabled');
            if (btns.indexOf(id) >= 0) {
                $(this)[isEnable ? "removeClass" : "addClass"]('disabled');
            }
        }
    })
}

//根据触发拿到按钮对象
z.buttonForTrigger = function (trigger) {
    var btn = null;
    trigger = trigger || z.btnTrigger;
    $('#BtnMenu').find('button').each(function () {
        if (this.id.toLowerCase() == "m_" + trigger) {
            btn = this;
            return false;
        }
    });
    return btn;
}

//显示隐藏切换
z.shSwitch = function (show, hide) {
    $.each(show, function () {
        $("" + this).removeClass('hidden');
    })
    $.each(hide, function () {
        $("" + this).addClass('hidden');
    })
}

//单据视图切换，表格⇔单据（gridBox样式类对应表格，invoiceBox样式类对应单据）
z.invoiceViewSwitch = function (isopen, index) {
    isopen = isopen == null ? !(z.invoiceControlStatus || false) : isopen;
    z.invoiceControlStatus = isopen;
    index = index || 1;
    if (isopen) {
        z.shSwitch(['.invoiceBox'], ['.gridBox'])
    } else {
        z.shSwitch(['.gridBox'], ['.invoiceBox'])
        //切换成列表时，调整大小
        var gd = window["gd" + index];
        gd && z.GridAuto(gd);
    }
}

//单据表格拓展
z.invoiceGridExtend = {
    //新增一行
    addRow: function (ig) {
        setTimeout(function () {
            var index = ig.func('getRowIndex', ig.func('getSelected'));
            ig.func('insertRow', { index: index + 1, row: {} });
        }, 10)
    },
    //删除一行
    delRow: function (ig) {
        setTimeout(function () {
            var index = ig.func('getRowIndex', ig.func('getSelected'));
            ig.func('deleteRow', index);
            //始终保留一行
            if (!ig.data.length) {
                ig.func('insertRow', { index: 0, row: {} });
            } else {
                z.GridAuto(ig);
            }
        }, 10)
    },
    //必填
    requiredRow: function (ig) {
        var rowDatas = ig.func('getRows'), cols = ig.columns[0], rows = [], lack = [];
        $.each(rowDatas, function (index, item) {
            var isempty = true, lackfield = [];

            for (var key in item) {
                var ik = item[key], hasv = ik != null && ik != "";
                isempty = hasv ? false : isempty;
            }

            if (!isempty) {
                $.each(cols, function (i, col) {
                    if (col.FormRequired) {
                        var ck = item[col.field];
                        if (ck == null || ck == "") {
                            lackfield.push($('<div>' + col.title + '</div>').text());
                            return false;
                        }
                    }
                })

                if (lackfield.length) {
                    lack.push("第<b>" + (index + 1) + "</b>行，" + lackfield.join('、') + " 为必填项");
                } else {
                    rows.push(item)
                }
            }
        })

        return { rows, lack }
    }
}
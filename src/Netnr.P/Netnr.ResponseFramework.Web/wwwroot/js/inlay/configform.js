//跨列
z.DC["dataurl_cf_colspan"] = {
    data: [
        { value: 1, text: "一半排" },
        { value: 2, text: "一整排" }
    ],
    init: function () {
        this.value = 1;
        this.panelHeight = 100;
    }
}
//区域
z.DC["dataurl_cf_colarea"] = {
    data: [],
    init: function () {
        this.value = 1;
        this.panelHeight = 100;
    }
}

//页面表索引
z.TableIndex = $('input[name="hidtableindex"]').first().val();
//页面表名
z.TableName = $('input[name="hidtablename"]').first().val();

z.ConfigForm = {
    CF: null,
    edit: null,
    prefix: "CF",
    modal: "fv_modal_",
    save: "fv_save_",
    formitem: "formitem"
}

//点击表单配置
$('#list_Config_Form').click(function () {
    if (!z.ConfigForm.CF) {
        var newNode = $('#' + z.ConfigForm.modal + z.TableIndex)[0].cloneNode(true), num = 1;
        newNode.id = z.ConfigForm.prefix + z.ConfigForm.modal + z.TableIndex;
        z.ConfigForm.CF = $(newNode);

        //呈现准备
        z.ConfigForm.CF.find('*').each(function () {
            if (this.nodeName == "A" && this.getAttribute('data-toggle') == "tab") {
                this.setAttribute('href', this.href.split('#')[0] + "#" + z.ConfigForm.prefix + this.hash.substring(1));
            }
            var pdid = this.getAttribute('id');
            if (pdid != null) {
                this.setAttribute('data-field', pdid);
                this.setAttribute('id', z.ConfigForm.prefix + pdid);
            }
            if (this.nodeName == "DIV" && this.className == "form-group" && !$(this).parent().hasClass('hidden')) {
                $(this).css('position', 'relative').css('border', '1px solid #ddd').append('<i class="fa fa-edit orange ief"></i><em class="eef">' + num + '</em>');
                this.style["padding"] = "5px 15px";
                $(this).find('label').css('cursor', 'move');
                num++;
            }
            if (this.type && "text,password,checkbox,radio,file,select,textarea".indexOf(this.type) >= 0) {
                this.disabled = true;
                this.style["cursor"] = "move";
                this.style["background-color"] = "white";
            }
        });

        z.ConfigForm.CF.css('user-select', 'none').on('selectstart', function () { return false }).find('div.modal-footer')
            .html('<button id="' + z.ConfigForm.prefix + z.ConfigForm.save + z.TableIndex + '" type="button" class="btn btn-primary"><span class="fa fa-save"></span>&nbsp;保存</button>');

        //呈现
        document.body.appendChild(newNode);
        z.FormTitle({
            id: z.ConfigForm.CF.find('h4.modal-title'),
            icon: 'fa-cog orange',
            title: '表单配置 （ 拖动排序 ）',
            required: false
        });

        //绑定事件
        z.ConfigForm.CF.click(function (e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            //编辑
            if (target.nodeName == "I" && target.className.indexOf('ief') >= 0) {

                var jtt = $(target), sib = jtt.siblings();

                //编辑的col
                z.ConfigForm.editcol = jtt.parent().parent();

                //当前值
                var etitle = sib.first().html(),
                    espan = z.ConfigForm.editcol.hasClass('col-md-12') ? 2 : 1,
                    eorder = sib.last().html();
                z.ConfigForm.earea = 1;
                //所在区域
                z.ConfigForm.CF.find('ul.nav').find('li').each(function (ei) {
                    if ($(this).hasClass('active')) {
                        z.ConfigForm.earea = ei + 1;
                        return false;
                    }
                });

                if (!z.ConfigForm.edit) {
                    //创建模态框
                    var zm = z.Modal();
                    zm.size = 2;
                    zm.showCancel = false;
                    zm.okText = '<span class="fa fa-search"></span>&nbsp;预览';
                    zm.okClick = function () {
                        //预览
                        var eform = z.ConfigForm.edit.modal.find('form');
                        if (z.FormRequired('red', eform)) {
                            var efromArr = eform.serializeArray(), evspan = 1, evarea = 1, evorder;
                            $(efromArr).each(function () {
                                switch (this.name) {
                                    case "title":
                                        z.ConfigForm.editcol.find('label').first().html(this.value);
                                        break;
                                    case "span":
                                        evspan = (this.value || 1) * 6;
                                        z.ConfigForm.editcol[0].className = "col-md-" + evspan + " col-sm-" + evspan;
                                        break;
                                    case "area":
                                        evarea = this.value || 1;
                                        break;
                                    case "order":
                                        evorder = this.value;
                                        break;
                                }
                            });

                            var next_node;
                            //有区域
                            if (z.ConfigForm.Combo_area.data.length) {
                                var tanpanes = z.ConfigForm.CF.find('div.tab-content').children();

                                //切换区域
                                if (z.ConfigForm.earea != evarea) {
                                    tanpanes.eq(evarea - 1).append(z.ConfigForm.editcol);
                                }
                                var ems = tanpanes.eq(evarea - 1).find('em.eef'),
                                    next_node = ems.last().parent().parent()[0];
                                ems.each(function () {
                                    if (this.innerHTML == evorder) {
                                        next_node = $(this).parent().parent()[0];
                                        return false;
                                    }
                                });
                            } else {
                                var ems = z.ConfigForm.CF.find('em.eef'),
                                    next_node = ems.last().parent().parent()[0];
                                ems.each(function () {
                                    if (this.innerHTML == evorder) {
                                        next_node = $(this).parent().parent()[0];
                                        return false;
                                    }
                                });
                            }

                            //交换节点
                            if ($(next_node).prev()[0] == z.ConfigForm.editcol[0]) {
                                z.ConfigForm.editcol.insertAfter(next_node);
                            } else {
                                z.ConfigForm.editcol.insertBefore(next_node);
                            }

                            //计算序号
                            CFalignNum();

                            //隐藏模态框
                            z.ConfigForm.edit.hide();
                        }
                    }
                    zm.append();

                    //填充表单
                    var htm = [];
                    htm.push('<form class="form-horizontal formui"><div class="row">');
                    var formitem = {
                        title: "标题",
                        span: "宽度",
                        area: "区域",
                        order: "顺序"
                    }
                    for (var i in formitem) {
                        htm.push('<div class="col-xs-12"><div class="form-group"><label class="col-xs-4 control-label required" >' + formitem[i] + '</label>');
                        htm.push('<div class="col-xs-8" style="padding:0;"><input class="form-control" id="' + z.ConfigForm.prefix + z.ConfigForm.formitem + i + '" name="' + i + '" /></div></div></div>');
                    }
                    htm.push('</form></div>');
                    zm.modal.attr('data-backdrop', 'static').find('div.modal-body').html(htm.join(''));

                    //初始化时的输入框
                    var cfeinput = zm.modal.find('input');

                    //宽度
                    var cf_cb1 = z.Combo();
                    cf_cb1.id = cfeinput.eq(1);
                    cf_cb1.data = [
                        { value: 1, text: "一半排" },
                        { value: 2, text: "一整排" }
                    ];
                    cf_cb1.value = 1;
                    cf_cb1.panelHeight = 100;
                    cf_cb1.editable = false;
                    cf_cb1.bind();
                    //区域
                    var cf_cb2 = z.Combo();
                    cf_cb2.id = cfeinput.eq(2);
                    cf_cb2.data = [];
                    z.ConfigForm.CF.find('ul.nav').find('a').each(function (ri) {
                        cf_cb2.data.push({ value: ri + 1, text: $.trim(this.innerHTML) });
                    });
                    if (!cf_cb2.data.length) {
                        sib.first().removeClass('required');
                        cfeinput.eq(2).parents('div.col-xs-12').addClass('hidden');
                    }
                    cf_cb2.panelHeight = 100;
                    cf_cb2.editable = false;
                    cf_cb2.bind();

                    //记录Combo对象
                    z.ConfigForm.Combo_span = cf_cb1;
                    z.ConfigForm.Combo_area = cf_cb2;

                    //设置模态框标题
                    z.FormTitle({
                        id: zm.modal.find('h4.modal-title'),
                        icon: 'fa-cog orange',
                        title: '显示设置',
                        required: false
                    });

                    //记录模态框
                    z.ConfigForm.edit = zm;
                }

                //重置样式
                z.FormRequired('reset', z.ConfigForm.edit.modal.find('form'));

                //赋值
                var etxtprefix = '#' + z.ConfigForm.prefix + z.ConfigForm.formitem;
                $(etxtprefix + "title").val(etitle);
                $(etxtprefix + "order").val(eorder);
                z.ConfigForm.Combo_span.func('setValue', espan);
                z.ConfigForm.Combo_area.func('setValue', z.ConfigForm.earea);

                //显示模态框
                z.ConfigForm.edit.show();
            }
        }).mousedown(function (e) {
            //拖拽
            e = e || window.event;
            var target = e.target || e.srcElement, sNode,
                dfg = $(this).find('div.form-group'),
                sX = e.clientX, sY = e.clientY;
            if (target.nodeName == "I" && target.className.indexOf('ief') >= 0) { return false; }
            dfg.each(function () {
                if (this.contains(target)) {
                    sNode = $(this).parent();
                    return false;
                }
            });
            $(z.ConfigForm.shadowNode).remove();
            if (sNode) {
                sX -= sNode[0].offsetLeft;
                sY -= sNode[0].offsetTop;
                z.ConfigForm.shadowNode = sNode[0].cloneNode(true);
                var shadowNode = $(z.ConfigForm.shadowNode), upEnd;
                shadowNode.css({
                    position: "absolute",
                    "z-index": 6666,
                    "display": "none"
                });
                shadowNode.children().css({
                    'margin-bottom': 0,
                    opacity: .8,
                    border: '2px dashed #FF892A'
                });
                shadowNode.appendTo(sNode.parent()[0]);
                //用子元素移动事件监听 获取松开鼠标所在节点
                this.onmousemove = this.onmouseover = function (e) {
                    if (upEnd) {
                        e = e || window.event;
                        var target = e.target || e.srcElement, eNode;
                        dfg.each(function () {
                            if (this.contains(target)) {
                                eNode = $(this).parent();
                                return false;
                            }
                        });
                        if (eNode && sNode[0] != eNode[0]) {
                            if (eNode.prev()[0] == sNode[0]) {
                                sNode.insertAfter(eNode);
                            } else {
                                sNode.insertBefore(eNode);
                            }
                            CFalignNum();
                        }
                        this.onmouseover = null;
                        this.onmousemove = null;
                    }
                }
                document.onmousemove = function (e) {
                    e = e || window.event;
                    var x = e.clientX, y = e.clientY;
                    shadowNode.css('left', x - sX).css('top', y - sY).css('display', 'block');
                }
                document.onmouseup = function (e) {
                    upEnd = 1;
                    this.onmousemove = null;
                    this.onmouseup = null;
                    this.releaseCapture && this.releaseCapture();
                }
                this.setCapture && this.setCapture();
            }
        }).mouseup(function () { $(z.ConfigForm.shadowNode).remove() });

        //保存
        $('#' + z.ConfigForm.prefix + z.ConfigForm.save + z.TableIndex).click(function () {
            var data = CFgetJson();
            var that = this;
            that.disabled = true;
            $.ajax({
                url: '/Inlay/SaveConfigForm',
                type: 'post',
                dataType: 'json',
                data: {
                    tablename: z.TableName,
                    rows: JSON.stringify(data)
                },
                success: function (data) {
                    if (data.code == 200) {
                        z.ConfigForm.CF.modal('hide');
                        art('操作成功，刷新生效，是否刷新？', function () {
                            location.reload(false)
                        });
                    } else {
                        art(data.msg);
                    }
                },
                error: function () {
                    art('error');
                },
                complete: function () {
                    that.disabled = false;
                }
            })
        });
    }

    //显示模态框
    z.ConfigForm.CF.modal();
});

//重新序号
function CFalignNum() {
    var sn = 1;
    z.ConfigForm.CF.find('em.eef').each(function () {
        if (!$(this).parent().parent().hasClass('hidden')) {
            $(this).html(sn);
            sn++;
        }
    });
}

//获取表单配置
function CFgetJson() {
    var data = [];
    //有区域
    if (z.ConfigForm.CF.find('ul.nav').length) {
        z.ConfigForm.CF.find('div.tab-pane').each(function (i, panel) {
            $(panel).children().each(function () {
                if (this.className.indexOf('hidden') == -1) {
                    var item = {};
                    item.field = $(this).find('input,textarea').first().attr('data-field');
                    item.title = $(this).find('label').first().html();
                    item.span = this.className.indexOf('col-md-6') >= 0 ? 1 : 2;
                    item.area = i + 1;
                    data.push(item);
                }
            });
        });
    } else {
        z.ConfigForm.CF.find('form').children().children().each(function () {
            if (this.className.indexOf('hidden') == -1) {
                var item = {};
                item.field = $(this).find('input,textarea').first().attr('data-field');
                item.title = $(this).find('label').first().html();
                item.span = this.className.indexOf('col-md-6') >= 0 ? 1 : 2;
                item.area = 1;
                data.push(item);
            }
        });
    }
    return data;
}
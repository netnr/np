/**
 * 默认查询关系符
 */
z.defaultQueryRelation = "Contains";

//查询可选关系符
z.QueryRelationData = function () {
    return [
        { value: "Contains", text: "包含" },
        { value: "Equal", text: "等于" },
        { value: "GreaterThanOrEqual", text: "大于等于" },
        { value: "LessThanOrEqual", text: "小于等于" },
        { value: "BetweenAnd", text: "两值之间" }
    ]
}

/**
 * 关系符
 */
z.DC["dataurl_cq_relation"] = {
    data: z.QueryRelationData(),
    init: function (cb) {
        this.formatter = function (row) {
            if (row.value == "Clear") {
                return "<span class='red'>重置条件</span>";
            }
            return row.text;
        };
        //显示时，展示可选查询关系符
        this.onShowPanel = function () {
            var rowData = z.queryin.grid.func('getSelected');
            var cv = $(this).combobox('getValue');
            var qrd = z.QueryRelationData();
            cb.data = [];
            $.each(rowData.relationList, function (i, v) {
                $.each(qrd, function () {
                    if (this.value == v) {
                        cb.data.push(this);
                    }
                })
            })
            cb.data.push({ value: "Clear", text: "" });

            var cbp = $(this).combobox('panel');
            cbp.height(cbp.children().first().outerHeight() * cb.data.length);

            $(this).combobox(cb);
            $(this).combobox('setValue', cv);
        };
        this.onClick = function (record) {
            var ei = z.queryin.grid.ei;
            setTimeout(function () {
                //结束编辑
                if (z.queryin.grid.ei != null) {
                    z.queryin.grid.func('endEdit', z.queryin.grid.ei);
                    z.queryin.grid.ei = null;
                }
            }, 50);
            if (record.value == "Clear") {
                setTimeout(function () {
                    var rowData = z.queryin.grid.func('getSelected');
                    rowData.relation = rowData.relationList[0];
                    rowData.value1 = null;
                    rowData.value2 = null;
                    z.queryin.grid.func('updateRow', { index: ei, row: rowData });
                    z.queryin.grid.func('refreshRow', ei);
                }, 80);
            }
        };
        this.onSelect = function (record) {
            if (record.value != "BetweenAnd") {
                var rowData = z.queryin.grid.func('getSelected');
                rowData.value2 = null;
                z.queryin.grid.func('updateRow', { index: z.queryin.grid.ei, row: rowData });
            }
        }
    }
}

/**
 * 根据表配置数据转换为查询面板的查询条件项
 * @param {any} data 表配置信息
 */
z.GridQueryDataConvert = function (data) {
    var arrdata = [];
    $.each(data, function () {
        if (this.ColQuery == 1) {
            var rl = this.ColRelation.split(',');
            arrdata.push({
                FormType: this.FormType || "text",
                FormUrl: this.FormUrl,
                field: this.field || this.ColField,
                relation: rl[0],
                relationList: rl,
                title: this.title || this.ColTitle
            });
        }
    });
    return arrdata;
}


/**
 * 创建查询标记
 * @param gd z.Grid 对象
 */
z.GridQueryMark = function (gd) {
    var gkm = ++z.index;
    z.DC.querykeymap = z.DC.querykeymap || {};
    z.DC.querykeymap[gkm] = gd;

    //构建查询列，默认列配置
    if (!gd.queryData) {
        //默认取列配置第一行
        if (gd.columns) {
            var cols = gd.frozenColumns[0].concat(gd.columns[0]);
            gd.queryData = z.GridQueryDataConvert(cols);
        }
    }

    var hasqd = {};
    $.each(gd.queryData, function () {
        hasqd[this.field] = 1;
    })

    var tds = gd.func('getPanel').find('tr.datagrid-header-row').children();
    tds.each(function () {
        var field = $(this).attr('field');
        if (field != null && field in hasqd) {
            var fp = $(this).children().css('position', 'relative');
            if (!fp.find('.datagrid-filter-icon').length) {
                $('<span class="datagrid-filter-icon fa fa-filter" data-gkm="' + gkm + '" data-field="' + field + '"></span>').appendTo(fp).click(function (e) {
                    z.stopEvent(e);
                    z.DC.querykeymap[this.getAttribute('data-gkm')].QueryOpen(this.getAttribute('data-field'));
                })
            }
        }
    });
}

/**
 * 查询条件值格式化
 * @param {any} value 值
 * @param {any} row 行数据
 * @param {any} index 索引
 * @param {any} gd z.Grid 对象
 */
z.GridQueryValue12 = function (value, row, index, gd) {
    if (row.relation == "Clear") {
        return "";
    }
    switch (row.FormType) {
        case "password":
            if (value != undefined) {
                var c = [], len = value.length;
                while (len) {
                    c.push('★');
                    len -= 1;
                }
                return c.join('');
            }
            break;
        case "checkbox":
            if (value != undefined) {
                var icon = value == "1" ? "fa-check-square-o" : "fa-square-o";
                return '<span class="fa ' + icon + ' fa-2x text-muted"></span>';
            }
            break;
        case "combobox":
            try {
                var node = findTreeNode(z.DC[String(row.FormUrl).toLowerCase()].data, value, "value");
                if (node) {
                    return node.text;
                }
            } catch (e) { }
            break;
        case "combotree":
            try {
                var node = findTreeNode(z.DC[String(row.FormUrl).toLowerCase()].data, value, "id");
                if (node) {
                    return node.text;
                }
            } catch (e) { }
            break;
        case "modal":
            try {
                var funcb = eval("col_custom_query_" + gd.data[index].field.toLowerCase());
                if (typeof funcb == "function") {
                    return funcb(value, row, index);
                }
            } catch (e) { }
            break;
    }
    return value;
}

/**
 * 建立查询面板
 * @param {any} gd z.Grid 对象
 */
z.GridQueryBuild = function (gd) {
    if (!gd.query) {
        gd.query = {};
        var gqm = new z.Modal();
        gqm.title = '<i class="fa fa-search blue"></i><span>查询条件</span>';
        gqm.size = 3;
        gqm.showCancel = false;
        gqm.okText = '<span class="fa fa-search"></span>&nbsp;确定';
        gd.query.id = "GridQuery_" + z.index;
        gqm.content = '<div id="P' + gd.query.id + '"><div class="loadingimg" id="' + gd.query.id + '"></div></div>';
        //查询确定
        gqm.okClick = function () {
            var errs = [];
            $(gd.data).each(function () {
                if (this.relation == "BetweenAnd") {
                    if (this.value1 == undefined || this.value1 == "" || this.value2 == undefined || this.value2 == "") {
                        errs.push(this.title);
                    }
                }
            });
            if (errs.length) {
                var mo = art('<div style="font-size:initial;">' + errs.join('</br>') + '</div>');
                mo.modal.find('h4.modal-title').html('<b class="red">请输入值范围</b>');
            } else {
                //查询确定事件，return fales 阻止关闭查询面板
                if (typeof gd.QueryOk == "function") {
                    if (gd.QueryOk() != false) {
                        gqm.modal.modal('hide')
                    }
                } else {
                    //默认获取查询条件，请求第一页
                    gd.onBeforeLoad = function (row, param) {
                        var sq = z.GridQueryWhere(gd);
                        param.wheres = sq.stringify();
                    }
                    gd.pageNumber = 1;
                    gd.load();
                    gqm.modal.modal('hide');
                }
            }
        }

        gqm.append();
        gqm.clearid = "fq_clear_" + z.index;
        $('<button id="' + gqm.clearid + '" class="btn btn-default red mr15"><span class="fa fa-remove"></span>&nbsp;清空条件</button>').insertBefore(gqm.modal.find('div.modal-footer').children().first());

        //构建查询表格
        var gq = z.Grid();
        gq.id = "#" + gd.query.id;
        gq.autosizePid = "#P" + gd.query.id;
        gq.autosize = "p";
        gq.fitColumns = true;
        gq.rownumbers = false;
        gq.striped = true;
        gq.pagination = false;
        gq.queryMark = false;
        gq.onClickCell = function (index, field, value) {
            setTimeout(function () {
                var row, allowedit = true;
                if ("value1,value2".indexOf(field) >= 0) {
                    row = gq.func('getSelected');
                    if (row.relation == null || row.relation == "") {
                        allowedit = false;
                    }
                    if (field == "value2" && row.relation != "BetweenAnd") {
                        allowedit = false;
                    }
                }
                if (allowedit) {
                    z.GridEditor(gq, index, field, row)
                } else {
                    //结束编辑
                    if (gq.ei != null) {
                        gq.func('endEdit', gq.ei);
                        gq.ei = null;
                    }
                }
            }, 10);
        }
        gq.columns = [[
            { field: "field", title: "键", width: 100, halign: "center", hidden: true },
            { field: "title", title: "条件名称", width: 90, halign: "center" },
            {
                field: "relation", title: "关系符", width: 70, halign: "center", FormType: "combobox", FormUrl: "dataurl_cq_relation", formatter: function (value) {
                    if (value == "Clear") {
                        return "";
                    }
                    $(z.DC["dataurl_cq_relation"].data).each(function () {
                        if (this.value == value) {
                            value = this.text;
                            return false;
                        }
                    });
                    return value;
                }
            },
            {
                field: "value1", title: "值1", width: 120, halign: "center", FormType: "text", formatter: function (value, row, index) {
                    return z.GridQueryValue12(value, row, index, gq);
                }
            },
            {
                field: "value2", title: "值2", width: 80, halign: "center", FormType: "text", formatter: function (value, row, index) {
                    return z.GridQueryValue12(value, row, index, gq);
                }
            }
        ]];

        //如果是字符串，则发起同步请求列配置
        if (typeof gd.queryData == "string") {
            var sq = z.SqlQuery();
            sq.wheres.push({ field: "TableName", relation: "Equal", value: gd.queryData });
            $.ajax({
                url: "/Inlay/QueryConfigTable",
                data: {
                    wheres: sq.stringify(),
                    sort: "ColOrder"
                },
                async: false,
                dataType: 'json',
                success: function (data) {
                    gd.queryData = z.GridQueryDataConvert(data.data);
                },
                error: function () {
                    gd.queryData = [];
                    art('加载查询面板出错');
                }
            })
        }
        gq.data = gd.queryData;

        //载入加载提示
        z.GridLoading(gq);

        //模态框显示时
        gqm.modal.on('shown.bs.modal', function () {
            if (gq.isinit) {
                gq.bind();

                z.GridEditorBlank(gq);
                $(window).resize(function () {
                    AutoHeightGrid(gq);
                });
            }
            AutoHeightGrid(gq);
        });

        //清空条件
        $('#' + gqm.clearid).click(function () {
            $(gq.data).each(function (i) {
                this.relation = this.relationList[0];
                this.value1 = null;
                this.value2 = null;
            });
            gq.bind();
        });

        gd.query.md = gqm;
        gd.query.grid = gq;
    }
}

/**
 * 建立查询面板
 */
z.Grid.fn.QueryBuild = function () {
    z.GridQueryBuild(this)
}

/**
 * 找到树形Tree的key=value的节点
 * @param {any} data
 * @param {any} value
 * @param {any} key
 */
function findTreeNode(data, value, key) {
    key = key || "value";
    var i = 0, len = data.length;
    for (; i < len; i++) {
        var node = data[i], child = node.children;
        if (node[key] == value) {
            return node;
        }
        if (child) {
            node = arguments.callee(child, value, key);
            if (node != undefined) {
                return node;
            }
        }
    }
}

/**
 * 调整高度自适应
 * @param {any} gd z.Grid 对象
 */
function AutoHeightGrid(gd) {
    gd.func('resize', {
        width: $(gd.autosizePid).width(),
        height: $(window).height() - 160
    });
}

/**
 * 获取查询条件
 * @param {any} gd z.Grid 对象
 */
z.GridQueryWhere = function (gd) {
    var items = [], sq = z.SqlQuery();
    try {
        $(gd.query.grid.data).each(function (index, obj) {
            if (obj.value1 != null && $.trim(obj.value1) != "") {
                var item = sq.item();

                item.field = obj.field;
                item.relation = obj.relation;

                if (obj.relation == "BetweenAnd") {
                    if (obj.value1 != "" && obj.value2 != "") {
                        item.value = [obj.value1, obj.value2];
                        items.push(item);
                    }
                } else {
                    if (obj.FormType == "checkbox") {
                        item.value = obj.value1 == "1" ? "1" : "0";
                        items.push(item);
                    } else {
                        if (obj.value1 != "") {
                            item.value = obj.value1;
                            items.push(item);
                        }
                    }
                }
            }
        });
    } catch (e) { }
    sq.wheres = items;
    return sq;
}

/** 
 * 获取查询条件
 */
z.Grid.fn.QueryWhere = function () {
    return z.GridQueryWhere(this);
}

/**
 * 打开查询、定位
 * @param {any} gd z.Grid 对象
 * @param {any} field 定位某个查询字段名，可选
 */
z.GridQueryOpen = function (gd, field) {
    z.GridQueryBuild(gd);

    var modal = gd.query.md.modal;
    modal[0].mfield = field;
    //定位
    if (!modal.sbm) {
        modal.sbm = true;
        modal.on('shown.bs.modal', function () {
            var field = this.mfield;
            if (field) {
                var grid = gd.query.grid;
                $.each(grid.data, function (i) {
                    if (this.field == field) {
                        grid.func('scrollTo', i);
                        grid.func('selectRow', i);
                        z.GridEditor(grid, i, "value1", this);
                        return false;
                    }
                });
            }
        });
    }

    gd.query.md.show();
    document.activeElement.blur();
    //指向当前查询面板
    z.queryin = gd.query;
}

/**
 * 打开查询、定位
 * @param {any} field 定位某个查询字段名，可选
 */
z.Grid.fn.QueryOpen = function (field) {
    return z.GridQueryOpen(this, field);
}

//快捷键
$(document).keydown(function (e) {
    e = e || window.event;
    var key = e.keyCode || e.which || e.charCode;
    switch (key) {
        case 13:
            {
                //查询面板 回车搜索
                if (z.queryin && z.queryin.md.modal.hasClass('in')) {
                    //结束编辑
                    if (z.queryin.grid.ei != null) {
                        z.queryin.grid.func('endEdit', z.queryin.grid.ei);
                        z.queryin.grid.ei = null;
                    }
                    z.queryin.md.okClick();
                }
            }
            break;
    }

});
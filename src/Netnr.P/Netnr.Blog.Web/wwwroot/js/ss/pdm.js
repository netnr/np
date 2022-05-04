var page = {
    dataArray: [],
    dataItem: function () {
        return {
            txt: null,
            xml: null,
            json: null
        }
    },
    grid: null,
    gbox: document.querySelector('.nr-grid'),

    tip: function (isShow) {
        ss.loading(isShow);
    },

    gridData: function () {

        var data = [];
        //文件

        for (var d1 = 0; d1 < page.dataArray.length; d1++) {
            var id1 = page.dataArray[d1].json;

            var PowerDesignerTarget = id1.PowerDesigner.Target;
            var PowerDesignerVersion = id1.PowerDesigner.version;
            var ModelCode = id1.Model.Code;

            var Users = "Users";
            var Tables = "Tables";

            //表
            for (var d2 = 0; d2 < id1.Tables.length; d2++) {
                var id2 = id1.Tables[d2];

                var TableName = id2.Name;
                var TableCode = id2.Code;
                var TableComment = id2.Comment;

                //列
                for (var d3 = 0; d3 < id2.Columns.length; d3++) {
                    var id3 = id2.Columns[d3];

                    var ColumnName = id3.Name;
                    var ColumnCode = id3.Code;
                    var ColumnComment = id3.Comment;
                    var ColumnDataType = id3.DataType;
                    var ColumnLength = id3.Length;
                    var ColumnFieldOrder = id3.FieldOrder;
                    var ColumnPrimaryKey = id3.PrimaryKey;
                    var ColumnClusterObject = id3.ClusterObject;
                    var ColumnMandatory = id3.Mandatory;
                    var ColumnDefaultValue = id3.DefaultValue;

                    var obj = {
                        PowerDesignerTarget, PowerDesignerVersion, ModelCode,
                        Users, Tables,
                        TableName, TableCode, TableComment,
                        ColumnName, ColumnCode, ColumnComment, ColumnDataType, ColumnLength, ColumnFieldOrder, ColumnPrimaryKey, ColumnClusterObject, ColumnMandatory, ColumnDefaultValue
                    };

                    data.push(obj);
                }
            }
        }

        return data;
    },

    view: function (data) {
        if (page.grid) {
            page.grid.api.setRowData(page.grid.rowData = page.gridData())
            return;
        }

        var gridOptions = ag.optionDef({
            columnDefs: [
                { field: "ModelCode", headerName: "Model", rowGroup: true, hide: true },
                { field: "Users", headerName: "Users", hide: true },
                { field: "Tables", headerName: "Tables", hide: true },
                { field: "TableName", headerName: "Table Name", rowGroup: true, hide: true },
                { field: "TableCode", headerName: "Code", hide: true },
                { field: "TableComment", headerName: "Table Comment", width: 300, tooltipField: "TableComment", hide: true },
                { field: "ColumnName", headerName: "Name", hide: true },
                { field: "ColumnCode", headerName: "Code" },
                { field: "ColumnComment", headerName: "Comment", width: 300, tooltipField: "ColumnComment" },
                { field: "ColumnDataType", headerName: "Data Type" },
                { field: "ColumnLength", headerName: "Length", width: 120 },
                {
                    field: "ColumnPrimaryKey", headerName: "P", width: 150, cellRenderer: function (item) {
                        if (item.value) {
                            var vr = `<b title="Primary Key">🔑</b>`;
                            if (item.data.ColumnClusterObject) {
                                vr += `<b class="ml-2" title="Cluster Object">📏</b>`;
                            }
                            return vr;
                        }
                    }
                },
                {
                    field: "ColumnMandatory", headerName: "M", width: 120, cellRenderer: function (item) {
                        return item.value ? `<b title="NOT NULL">✍</b>` : '';
                    }
                },
                {
                    field: "ColumnDefaultValue", headerName: "Default Value", cellRenderer: function (item) {
                        return item.value;
                    }
                }
            ],
            autoGroupColumnDef: ag.autoGroupColumnDef({
                menuTabs: ['generalMenuTab', 'columnsMenuTab'],
                field: 'ColumnName', width: 550, resizable: true, cellRendererParams: {
                    suppressCount: true, suppressEnterExpand: true, suppressDoubleClickExpand: true,
                    innerRenderer: function (item) {
                        var grow = page.grid.rowData.filter(x => x[item.node.field] == item.value);
                        var row = grow[0];
                        switch (item.node.field) {
                            case "ModelCode":
                                {
                                    let subg = page.groupBy(grow, gi => gi.TableName);
                                    return `<b>${item.value}</b> (${subg.length}) <sl-badge variant="success" class="mx-2">${row.PowerDesignerTarget}</sl-badge> <sl-badge variant="warning">v${row.PowerDesignerVersion}</sl-badge>`
                                }
                            case "TableName":
                                {
                                    let subg = page.groupBy(grow, gi => gi.ColumnCode);
                                    return `<b>${row.TableCode}</b> (${subg.length}) <sl-badge>${row.TableComment || ""}</sl-badge>`
                                }
                            default:
                                return `<b>${item.value}</b>`
                        }
                    }
                }
            }),
            rowData: data,
        })

        page.grid = new agGrid.Grid(page.gbox, gridOptions).gridOptions;

        //搜索
        nr.domTxtFilter.addEventListener("input", function () {
            page.grid.api.setQuickFilter(this.value.trim());
        });

        //展开节点
        setTimeout(function () {
            page.grid.api.forEachNode(function (node) {
                if (node.group && node.level == 0) {
                    node.setExpanded(true);
                }
            });
        }, 200)

        page.resize();
    },

    openUrl: function (url) {
        if (url == null) {
            url = location.hash.substr(1);
        }
        if (url.length < 4) {
            return;
        }
        nr.domBtnDemo.loading = true;
        page.tip(true);

        ss.fetch({
            url: url,
        }).then(res => {
            location.hash = "";

            page.dataArray = page.dataArray.concat(page.parse(res));
            page.view(page.gridData());

            page.tip(false);
            nr.domBtnDemo.loading = false;
        }).catch(ex => {
            console.debug(ex);
            page.tip(false);
            nr.domBtnDemo.loading = false;
        })
    },

    groupBy: function (array, f) {
        var groups = {};
        array.forEach(o => {
            var key = f(o);
            groups[key] = groups[key] || [];
            groups[key].push(o);
        });
        return Object.keys(groups);
    },

    resize: function () {
        var ch = document.documentElement.clientHeight;
        var vh = ch - page.gbox.getBoundingClientRect().top - 30;
        page.gbox.style.height = vh + "px";
    },

    parse: function (txt) {
        var di = page.dataItem();
        di.xml = new DOMParser().parseFromString(di.txt = txt, "text/xml");
        di.json = {
            PowerDesigner: page.getPowerDesigner(di),
            Model: page.getModel(di),
            Users: page.getUsers(di),
            TargetModels: page.getTargetModels(di),
            Tables: page.getTables(di)
        };
        return di;
    },

    visualDate: function (u) {
        return new Date(u * 1000 + 8 * 3600 * 1000).toISOString().replace("T", " ").replace("Z", "");
    },

    getKeyObj: function (kp) {
        var i = 0, ko = page.dataArray;
        do {
            ko = ko[kp[i++]];
        } while (i < kp.length);
        return ko
    },

    getTagName: function (di, kk, dom) {
        var tn = dom ? dom : [di.xml], kks = kk.split(',');

        for (var i = 0; i < kks.length; i++) {
            var k = kks[i];
            if (tn && tn.length == null) {
                tn = tn.getElementsByTagName(k);
            } else if (tn && tn.length) {
                tn = tn[0].getElementsByTagName(k);
            }
        }

        return tn;
    },

    getNameValue: function (di, kk, dom) {
        var nd = page.getTagName(di, kk, dom);
        if (nd && nd[0]) {
            return nd[0].innerHTML;
        }
        return null;
    },

    getAttr: function (kk, dom) {
        if (dom) {
            return dom.getAttribute(kk);
        }
        return null;
    },

    getPowerDesigner: function (di) {
        var vt = di.txt.substr(di.txt.indexOf("<?PowerDesigner"), Math.min(5000, di.txt.indexOf("<o:RootObject")));
        var obj = {
            AppLocale: /AppLocale="(\w+)"/.exec(vt)[1],
            Name: /Name="([\w,\., ]+)"/.exec(vt)[1],
            Target: /Target="([\w,\., ]+)"/.exec(vt)[1],
            version: /version="([\d,\.]+)"/.exec(vt)[1]
        }

        return obj;
    },

    getModel: function (di) {
        var mo = page.getTagName(di, "o:RootObject,c:Children,o:Model");
        var obj = {
            Name: page.getNameValue(di, "a:Name", mo),
            Code: page.getNameValue(di, "a:Code", mo),
            CreationDate: page.visualDate(page.getNameValue(di, "a:CreationDate", mo)),
            Creator: page.getNameValue(di, "a:Creator", mo),
            ModificationDate: page.visualDate(page.getNameValue(di, "a:ModificationDate", mo)),
            Modifier: page.getNameValue(di, "a:Modifier", mo)
        }

        return obj;
    },

    getUsers: function (di) {
        var nn = page.getTagName(di, "o:RootObject,c:Children,o:Model,c:Users,o:User");
        var obj = {
            Name: page.getNameValue(di, "a:Name", nn),
            Code: page.getNameValue(di, "a:Code", nn),
            CreationDate: page.visualDate(page.getNameValue(di, "a:CreationDate", nn)),
            Creator: page.getNameValue(di, "a:Creator", nn),
            ModificationDate: page.visualDate(page.getNameValue(di, "a:ModificationDate", nn)),
            Modifier: page.getNameValue(di, "a:Modifier", nn),
            Stereotype: page.getNameValue(di, "a:Stereotype", nn)
        }

        return obj;
    },

    getTargetModels: function (di) {
        var nn = page.getTagName(di, "o:RootObject,c:Children,o:Model,c:TargetModels,o:TargetModel");
        var obj = {
            Name: page.getNameValue(di, "a:Name", nn),
            Code: page.getNameValue(di, "a:Code", nn),
            CreationDate: page.visualDate(page.getNameValue(di, "a:CreationDate", nn)),
            ModificationDate: page.visualDate(page.getNameValue(di, "a:ModificationDate", nn))
        }

        return obj;
    },

    getTables: function (di) {
        var nn = page.getTagName(di, "o:RootObject,c:Children,o:Model,c:Tables,o:Table"), tables = [];

        for (var i = 0; i < nn.length; i++) {
            var nt = nn[i], table = {
                Id: page.getAttr("Id", nt),
                Name: page.getNameValue(di, "a:Name", nt),
                Code: page.getNameValue(di, "a:Code", nt),
                CreationDate: page.visualDate(page.getNameValue(di, "a:CreationDate", nt)),
                Creator: page.getNameValue(di, "a:Creator", nt),
                ModificationDate: page.visualDate(page.getNameValue(di, "a:ModificationDate", nt)),
                Modifier: page.getNameValue(di, "a:Modifier", nt),
                Comment: page.getNameValue(di, "a:Comment", nt),
                PhysicalOptions: page.getNameValue(di, "a:PhysicalOptions", nt),
                Columns: [],
                Keys: [],
                Indexes: [],
                //主键 Keys id
                PrimaryKey: null,
                //聚集索引 Index id
                ClusterObject: null
            }

            //Columns
            var nc = page.getTagName(di, "c:Columns,o:Column", nt);
            for (var j = 0; j < nc.length; j++) {
                var cj = nc[j], column = {
                    Id: page.getAttr("Id", cj),
                    Name: page.getNameValue(di, "a:Name", cj),
                    Code: page.getNameValue(di, "a:Code", cj),
                    CreationDate: page.visualDate(page.getNameValue(di, "a:CreationDate", cj)),
                    Creator: page.getNameValue(di, "a:Creator", cj),
                    ModificationDate: page.visualDate(page.getNameValue(di, "a:ModificationDate", cj)),
                    Modifier: page.getNameValue(di, "a:Modifier", cj),
                    Comment: page.getNameValue(di, "a:Comment", cj),
                    DefaultValue: page.getNameValue(di, "a:DefaultValue", cj),
                    DataType: page.getNameValue(di, "a:DataType", cj),
                    Length: page.getNameValue(di, "a:Length", cj),
                    FieldOrder: (j + 1),
                    PrimaryKey: null,
                    ClusterObject: null,
                    Mandatory: page.getNameValue(di, "a:Column.Mandatory", cj),
                    ExtendedAttributesText: page.getNameValue(di, "a:ExtendedAttributesText", cj),
                }
                table.Columns.push(column);
            }

            //Keys
            var nk = page.getTagName(di, "c:Keys,o:Key", nt);
            for (var j = 0; j < nk.length; j++) {
                var kj = nk[j], key = {
                    Id: page.getAttr("Id", kj),
                    Code: page.getNameValue(di, "a:Code", kj),
                    CreationDate: page.visualDate(page.getNameValue(di, "a:CreationDate", kj)),
                    Creator: page.getNameValue(di, "a:Creator", kj),
                    ModificationDate: page.getNameValue(di, "a:ModificationDate", kj),
                    Modifier: page.getNameValue(di, "a:Modifier", kj),
                    PhysicalOptions: page.getNameValue(di, "a:PhysicalOptions", kj),
                    ConstraintName: page.getNameValue(di, "a:ConstraintName", kj),
                    //Columns id
                    Columns: []
                };

                //Columns id
                var nkc = page.getTagName(di, "c:Key.Columns,o:Column", kj);
                for (var m = 0; m < nkc.length; m++) {
                    var id = page.getAttr("Ref", nkc[m]);
                    key.Columns.push(id);
                }

                table.Keys.push(key);
            }

            //Indexes
            var ni = page.getTagName(di, "c:Indexes,o:Index", nt);
            for (var j = 0; j < ni.length; j++) {
                var ij = ni[j], index = {
                    Id: page.getAttr("Id", ij),
                    Name: page.getNameValue(di, "a:Name", ij),
                    Code: page.getNameValue(di, "a:Code", ij),
                    CreationDate: page.visualDate(page.getNameValue(di, "a:CreationDate", ij)),
                    Creator: page.getNameValue(di, "a:Creator", ij),
                    ModificationDate: page.visualDate(page.getNameValue(di, "a:ModificationDate", ij)),
                    Modifier: page.getNameValue(di, "a:Modifier", ij),
                    PhysicalOptions: page.getNameValue(di, "a:PhysicalOptions", ij),
                    Unique: page.getNameValue(di, "a:Unique", ij),
                    //Columns id
                    Columns: []
                };

                //Columns id
                var niic = page.getTagName(di, "c:IndexColumns,o:IndexColumn", ij);
                for (var m = 0; m < niic.length; m++) {
                    var id = page.getAttr("Ref", page.getTagName(di, "c:Column,o:Column", niic[m])[0]);
                    index.Columns.push(id);
                }

                table.Indexes.push(index);
            }

            //主键
            var hasPk = page.getTagName(di, "c:PrimaryKey,o:Key", nt);
            if (hasPk.length) {
                table.PrimaryKey = page.getAttr("Ref", hasPk[0]);
                var colid = table.Keys.filter(x => x.Id == table.PrimaryKey)[0].Columns;
                table.Columns.forEach(c => {
                    var pki = colid.indexOf(c.Id);
                    c.PrimaryKey = pki == -1 ? null : pki + 1;
                });
            }

            //聚集索引
            var hasCo = page.getTagName(di, "c:ClusterObject,o:Index", nt);
            if (hasCo.length) {
                table.ClusterObject = page.getAttr("Ref", hasCo[0]);
                var colid = table.Indexes.filter(x => x.Id == table.ClusterObject)[0].Columns;
                table.Columns.forEach(c => {
                    var pki = colid.indexOf(c.Id);
                    c.ClusterObject = pki == -1 ? null : pki + 1;
                });
            }

            tables.push(table);
        }

        return tables;
    }
}

nr.onReady = function () {
    window.addEventListener('resize', function () {
        page.resize()
    });
    window.addEventListener('hashchange', function () {
        page.openUrl();
    });
    page.openUrl();

    nr.domBtnDemo.addEventListener('click', function () {
        location.hash = "https://s1.netnr.eu.org/2021/07/02/125049306.pdm";
    });

    nr.receiveFiles(function (files) {
        page.tip(true);

        var arr = [];
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            if (file.name.split('.').pop().toLowerCase() == "pdm") {

                var pi = new Promise(function (resolve, reject) {
                    var fr = new FileReader();
                    fr.onload = function (e) {
                        try {
                            var di = page.parse(e.target.result);
                            resolve(di);
                        } catch (e) {
                            console.log(e)
                            reject(e)
                        }
                    }
                    fr.readAsText(file);
                });

                arr.push(pi);
            }
        }

        Promise.all(arr).then(res => {
            page.dataArray = page.dataArray.concat(res);
            page.view(page.gridData());

            page.tip(false);
        })

        nr.domTxtFile.value = "";
    }, nr.domTxtFile);
}
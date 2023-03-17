let pdm = {
    dataArray: [],
    dataItem: function () {
        return {
            txt: null,
            xml: null,
            json: null
        }
    },

    gridData: function () {

        var data = [];
        //文件

        for (var d1 = 0; d1 < pdm.dataArray.length; d1++) {
            var id1 = pdm.dataArray[d1].json;

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

    groupBy: function (array, f) {
        var groups = {};
        array.forEach(o => {
            var key = f(o);
            groups[key] = groups[key] || [];
            groups[key].push(o);
        });
        return Object.keys(groups);
    },

    parse: function (txt) {
        var di = pdm.dataItem();
        di.xml = new DOMParser().parseFromString(di.txt = txt, "text/xml");
        di.json = {
            PowerDesigner: pdm.getPowerDesigner(di),
            Model: pdm.getModel(di),
            Users: pdm.getUsers(di),
            TargetModels: pdm.getTargetModels(di),
            Tables: pdm.getTables(di)
        };
        return di;
    },

    visualDate: function (u) {
        return new Date(u * 1000 + 8 * 3600 * 1000).toISOString().replace("T", " ").replace("Z", "");
    },

    getKeyObj: function (kp) {
        var i = 0, ko = pdm.dataArray;
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
        var nd = pdm.getTagName(di, kk, dom);
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
        var mo = pdm.getTagName(di, "o:RootObject,c:Children,o:Model");
        var obj = {
            Name: pdm.getNameValue(di, "a:Name", mo),
            Code: pdm.getNameValue(di, "a:Code", mo),
            CreationDate: pdm.visualDate(pdm.getNameValue(di, "a:CreationDate", mo)),
            Creator: pdm.getNameValue(di, "a:Creator", mo),
            ModificationDate: pdm.visualDate(pdm.getNameValue(di, "a:ModificationDate", mo)),
            Modifier: pdm.getNameValue(di, "a:Modifier", mo)
        }

        return obj;
    },

    getUsers: function (di) {
        var nn = pdm.getTagName(di, "o:RootObject,c:Children,o:Model,c:Users,o:User");
        var obj = {
            Name: pdm.getNameValue(di, "a:Name", nn),
            Code: pdm.getNameValue(di, "a:Code", nn),
            CreationDate: pdm.visualDate(pdm.getNameValue(di, "a:CreationDate", nn)),
            Creator: pdm.getNameValue(di, "a:Creator", nn),
            ModificationDate: pdm.visualDate(pdm.getNameValue(di, "a:ModificationDate", nn)),
            Modifier: pdm.getNameValue(di, "a:Modifier", nn),
            Stereotype: pdm.getNameValue(di, "a:Stereotype", nn)
        }

        return obj;
    },

    getTargetModels: function (di) {
        var nn = pdm.getTagName(di, "o:RootObject,c:Children,o:Model,c:TargetModels,o:TargetModel");
        var obj = {
            Name: pdm.getNameValue(di, "a:Name", nn),
            Code: pdm.getNameValue(di, "a:Code", nn),
            CreationDate: pdm.visualDate(pdm.getNameValue(di, "a:CreationDate", nn)),
            ModificationDate: pdm.visualDate(pdm.getNameValue(di, "a:ModificationDate", nn))
        }

        return obj;
    },

    getTables: function (di) {
        var nn = pdm.getTagName(di, "o:RootObject,c:Children,o:Model,c:Tables,o:Table"), tables = [];

        for (var i = 0; i < nn.length; i++) {
            var nt = nn[i], table = {
                Id: pdm.getAttr("Id", nt),
                Name: pdm.getNameValue(di, "a:Name", nt),
                Code: pdm.getNameValue(di, "a:Code", nt),
                CreationDate: pdm.visualDate(pdm.getNameValue(di, "a:CreationDate", nt)),
                Creator: pdm.getNameValue(di, "a:Creator", nt),
                ModificationDate: pdm.visualDate(pdm.getNameValue(di, "a:ModificationDate", nt)),
                Modifier: pdm.getNameValue(di, "a:Modifier", nt),
                Comment: pdm.getNameValue(di, "a:Comment", nt),
                PhysicalOptions: pdm.getNameValue(di, "a:PhysicalOptions", nt),
                Columns: [],
                Keys: [],
                Indexes: [],
                //主键 Keys id
                PrimaryKey: null,
                //聚集索引 Index id
                ClusterObject: null
            }

            //Columns
            var nc = pdm.getTagName(di, "c:Columns,o:Column", nt);
            for (var j = 0; j < nc.length; j++) {
                var cj = nc[j], column = {
                    Id: pdm.getAttr("Id", cj),
                    Name: pdm.getNameValue(di, "a:Name", cj),
                    Code: pdm.getNameValue(di, "a:Code", cj),
                    CreationDate: pdm.visualDate(pdm.getNameValue(di, "a:CreationDate", cj)),
                    Creator: pdm.getNameValue(di, "a:Creator", cj),
                    ModificationDate: pdm.visualDate(pdm.getNameValue(di, "a:ModificationDate", cj)),
                    Modifier: pdm.getNameValue(di, "a:Modifier", cj),
                    Comment: pdm.getNameValue(di, "a:Comment", cj),
                    DefaultValue: pdm.getNameValue(di, "a:DefaultValue", cj),
                    DataType: pdm.getNameValue(di, "a:DataType", cj),
                    Length: pdm.getNameValue(di, "a:Length", cj),
                    FieldOrder: (j + 1),
                    PrimaryKey: null,
                    ClusterObject: null,
                    Mandatory: pdm.getNameValue(di, "a:Column.Mandatory", cj),
                    ExtendedAttributesText: pdm.getNameValue(di, "a:ExtendedAttributesText", cj),
                }
                table.Columns.push(column);
            }

            //Keys
            var nk = pdm.getTagName(di, "c:Keys,o:Key", nt);
            for (var j = 0; j < nk.length; j++) {
                var kj = nk[j], key = {
                    Id: pdm.getAttr("Id", kj),
                    Code: pdm.getNameValue(di, "a:Code", kj),
                    CreationDate: pdm.visualDate(pdm.getNameValue(di, "a:CreationDate", kj)),
                    Creator: pdm.getNameValue(di, "a:Creator", kj),
                    ModificationDate: pdm.getNameValue(di, "a:ModificationDate", kj),
                    Modifier: pdm.getNameValue(di, "a:Modifier", kj),
                    PhysicalOptions: pdm.getNameValue(di, "a:PhysicalOptions", kj),
                    ConstraintName: pdm.getNameValue(di, "a:ConstraintName", kj),
                    //Columns id
                    Columns: []
                };

                //Columns id
                var nkc = pdm.getTagName(di, "c:Key.Columns,o:Column", kj);
                for (var m = 0; m < nkc.length; m++) {
                    var id = pdm.getAttr("Ref", nkc[m]);
                    key.Columns.push(id);
                }

                table.Keys.push(key);
            }

            //Indexes
            var ni = pdm.getTagName(di, "c:Indexes,o:Index", nt);
            for (var j = 0; j < ni.length; j++) {
                var ij = ni[j], index = {
                    Id: pdm.getAttr("Id", ij),
                    Name: pdm.getNameValue(di, "a:Name", ij),
                    Code: pdm.getNameValue(di, "a:Code", ij),
                    CreationDate: pdm.visualDate(pdm.getNameValue(di, "a:CreationDate", ij)),
                    Creator: pdm.getNameValue(di, "a:Creator", ij),
                    ModificationDate: pdm.visualDate(pdm.getNameValue(di, "a:ModificationDate", ij)),
                    Modifier: pdm.getNameValue(di, "a:Modifier", ij),
                    PhysicalOptions: pdm.getNameValue(di, "a:PhysicalOptions", ij),
                    Unique: pdm.getNameValue(di, "a:Unique", ij),
                    //Columns id
                    Columns: []
                };

                //Columns id
                var niic = pdm.getTagName(di, "c:IndexColumns,o:IndexColumn", ij);
                for (var m = 0; m < niic.length; m++) {
                    var id = pdm.getAttr("Ref", pdm.getTagName(di, "c:Column,o:Column", niic[m])[0]);
                    index.Columns.push(id);
                }

                table.Indexes.push(index);
            }

            //主键
            var hasPk = pdm.getTagName(di, "c:PrimaryKey,o:Key", nt);
            if (hasPk.length) {
                table.PrimaryKey = pdm.getAttr("Ref", hasPk[0]);
                var colid = table.Keys.filter(x => x.Id == table.PrimaryKey)[0].Columns;
                table.Columns.forEach(c => {
                    var pki = colid.indexOf(c.Id);
                    c.PrimaryKey = pki == -1 ? null : pki + 1;
                });
            }

            //聚集索引
            var hasCo = pdm.getTagName(di, "c:ClusterObject,o:Index", nt);
            if (hasCo.length) {
                table.ClusterObject = pdm.getAttr("Ref", hasCo[0]);
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
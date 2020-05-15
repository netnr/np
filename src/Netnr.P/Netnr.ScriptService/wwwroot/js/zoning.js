//var npmsrc = "https://unpkg.com/zoningjs@2.0.19/";
//国内镜像
var npmsrc = "https://code.bdstatic.com/npm/zoningjs@2.0.19/";

var zoning4 = {
    json: {},
    load: function (callback) {
        $.getJSON(npmsrc + "zoning-4.json", null, function (data) {
            zoning4.json = data;
            callback();
        });
    },
    change: function (ses, index) {
        var se1 = ses[index++];
        if (index < ses.length) {
            var se2 = ses[index], sd = zoning4.json[se1.value];
            se2.options.length = 0;
            se2.options.add(new Option("（请选择）", ""));
            if (se1.value != "" && sd) {
                for (var i = 0; i < sd.length; i++) {
                    var sdi = sd[i];
                    se2.options.add(new Option(sdi.text, sdi.id));
                }
            }
            if (index + 1 < ses.length) {
                zoning4.change(ses, index);
            }
        }
    },
    getvalues: function (ses) {
        var codes = [];
        ses.each(function () {
            codes.push(this.value || "-");
        });
        return codes;
    },
    init: function (ses) {
        ses.each(function (index) {
            $(this).attr('data-index', index);
            this.options.add(new Option("（请选择）", ""));
            if (!index) {
                var idb = zoning4.json["0"];
                for (var i = 0; i < idb.length; i++) {
                    var sdi = idb[i];
                    this.options.add(new Option(sdi.text, sdi.id));
                }
            }
        }).change(function () {
            zoning4.change(ses, $(this).attr('data-index'));

            $('#zoningcode4').html(zoning4.getvalues(ses).join('</br>'));
        })
    }
}
zoning4.load(function () {
    zoning4.init($('#zoning4').find('select'))
});

var zoning5 = {
    list: function (pid, callback) {
        var pidaspath = pid > 0 ? (pid.substring(0, 2) + "/" + pid) : 0;
        $.getJSON(npmsrc + pidaspath + ".json", null, callback)
    },
    change: function (ses, index) {
        var se1 = ses[index++];
        if (index < ses.length) {
            var se2 = ses[index];
            se2.options.length = 0;
            se2.options.add(new Option("（请选择）", ""));
            if (se1.value != "") {
                zoning5.list(se1.value, function (res) {
                    for (var i = 0; i < res.length; i++) {
                        var rei = res[i];
                        se2.options.add(new Option(rei.text, rei.id));
                    }
                })
            }
            if (index + 1 < ses.length) {
                zoning5.change(ses, index);
            }
        }
    },
    getvalues: function (ses) {
        var codes = [];
        ses.each(function () {
            codes.push(this.value || "-");
        });
        return codes;
    },
    init: function (ses) {
        ses.each(function (index) {
            $(this).attr('data-index', index);
            this.options.add(new Option("（请选择）", ""));
            if (!index) {
                var that = this;
                zoning5.list("0", function (res) {
                    for (var i = 0; i < res.length; i++) {
                        var rei = res[i];
                        that.options.add(new Option(rei.text, rei.id));
                    }
                })
            }
        }).change(function () {
            zoning5.change(ses, $(this).attr('data-index'));

            $('#zoningcode5').html(zoning5.getvalues(ses).join(' / '));
        })
    }
}

zoning5.init($('#zoning5').find('select'));
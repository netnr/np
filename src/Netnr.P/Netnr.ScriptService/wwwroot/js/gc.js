var gc = {
    tabs: [],
    resize: function () {
        $.each(gc.tabs, function () {
            $(this).bootstrapTable('resetView', {
                height: $(window).height() - $('#PGrid').offset().top - 15
            });
        });
    },
    load: function () {
        var pg = $('#PGrid');
        var gcjson = JSON.parse($('#source').html());

        var tobj = {};
        $.each(gcjson, function () {
            var trow = tobj[this.category] || [];
            trow.push(this);
            tobj[this.category] = trow;
        });

        var ti = 0;
        for (var i in tobj) {
            var tb = pg.children().eq(ti++).children();
            gc.tabs.push(tb);
            tb.bootstrapTable({
                classes: 'table table-sm table-bordered',
                data: tobj[i]
            })
        }

        $(window).resize(gc.resize);
        gc.resize();
    }
}

gc.load();

//垃圾分类数据来源：https://github.com/alexayan/garbage-classification-data
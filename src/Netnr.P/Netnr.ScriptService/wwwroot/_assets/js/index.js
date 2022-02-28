function search() {
    var group = $('.nr-group').val(), keywords = $('.nr-search').val();

    keywords = keywords.trim().toLowerCase();
    var fb = $('.navbox');
    fb.find('a').each(function () {
        var col = $(this).parent().parent().parent();
        if (keywords != "") {
            if ($(this).text().toLowerCase().indexOf(keywords) >= 0 || this.href.toLowerCase().indexOf(keywords) >= 0) {
                col.removeClass('d-none');
            } else {
                col.addClass('d-none');
            }
        } else {
            col.removeClass('d-none');
        }
    });
    fb.children().each(function () {
        var cols = $(this).children();
        if (group != "") {
            if (cols.first().text().trim() == group) {
                $(this).removeClass('d-none');
            } else {
                $(this).addClass('d-none');
            }
        } else {
            $(this).removeClass('d-none');
            if (cols.filter('.d-none').length + 1 >= cols.length) {
                cols.first().addClass('d-none');
            } else {
                cols.first().removeClass('d-none');
            }
        }
    });
}

function savels() {
    ss.ls["ss-search"] = $('.nr-search').val();
    ss.ls["ss-group"] = $('.nr-group').val();
    ss.lsSave();
}

var lssearch = ss.lsStr("ss-search") || "",
    lsgroup = ss.lsStr("ss-group") || "";

$('.nr-group').change(function () {
    savels();
    search();
}).find('option').each(function () {
    if (this.value == lsgroup) {
        $('.nr-group').val(this.value);
        return false;
    }
});

$('.nr-search').on('input', function () {
    savels();
    search();
}).val(lssearch);

$(document).dblclick(function () {
    $('.nr-search').val('');
    $('.nr-group').val('');
    search();
});

$('.navbox').find('a').each(function () {
    //兼容本地生成的静态页面
    var na = this;
    if (na.href.indexOf("//localhost") >= 0 && na.href.indexOf("/home/") == -1 && !na.href.endsWith('/')) {
        na.href += ".html";
    }
});
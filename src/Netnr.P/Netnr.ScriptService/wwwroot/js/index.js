function search(group, keywords) {
    keywords = keywords.trim().toLowerCase();
    var fb = $('#favoritebox');
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
$('#seGroup').change(function () {
    localStorage["favorite-group"] = this.value;
    search($('#seGroup').val(), $('#txtSearch').val())
});
$('#btnSearch').click(function () {
    localStorage["favorite-group"] = this.value;
    search($('#seGroup').val(), localStorage["favorite-search"] = $('#txtSearch').val())
});
$('#txtSearch').on('input', function () {
    search($('#seGroup').val(), localStorage["favorite-search"] = $('#txtSearch').val())
});

$(document).dblclick(function () {
    $('#txtSearch').val('');
    $('#seGroup').val('');
    $('#btnSearch')[0].click();
});

$('#txtSearch').val(localStorage["favorite-search"] || "");
var defaultGroup = localStorage["favorite-group"] || "";
$('#seGroup').find('option').each(function () {
    if (this.value == defaultGroup) {
        $('#seGroup').val(this.value);
        $('#btnSearch')[0].click();
        return false;
    }
});

$('#favoritebox').find('img').each(function () {
    var that = this;
    this.onerror = function () { this.src = '/favicon.svg'; this.onerror = null; }
    var ci = new Image();
    ci.onload = function () { that.src = this.src; }
    ci.src = that.getAttribute('data-icon');

    //本地环境兼容
    var na = that.parentNode;
    if (na.href.indexOf("//localhost") >= 0 && na.href.indexOf("/home/") == -1) {
        na.href += ".html";
    }
});
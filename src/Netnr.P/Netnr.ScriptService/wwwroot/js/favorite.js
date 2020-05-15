function search(group, keywords) {
    keywords = keywords.trim().toLowerCase();
    var fb = $('#favoritebox');
    fb.find('a').each(function () {
        if (keywords != "") {
            if (this.innerHTML.toLowerCase().indexOf(keywords) >= 0 || this.title.toLowerCase().indexOf(keywords) >= 0 || this.href.toLowerCase().indexOf(keywords) >= 0) {
                $(this).parent().removeClass('hidden');
            } else {
                $(this).parent().addClass('hidden');
            }
        } else {
            $(this).parent().removeClass('hidden');
        }
    });
    fb.children().each(function () {
        if (group != "") {
            if ($(this).attr('data-group') == group) {
                $(this).removeClass('hidden');
            } else {
                $(this).addClass('hidden');
            }
        } else {
            $(this).removeClass('hidden');
        }
        var ga = $(this).find('div.mba');
        if (ga.length == ga.filter('.hidden').length) {
            $(this).addClass('hidden');
        }
    });
}
$('#seGroup').change(function () {
    localStorage["favorite-group"] = this.value;
    search($('#seGroup').val(), $('#txtSearch').val())
});
$('#btnSearch').click(function () {
    search($('#seGroup').val(), localStorage["favorite-search"] = $('#txtSearch').val())
});
$('#txtSearch').on('input', function () {    ;
    search($('#seGroup').val(), localStorage["favorite-search"] = $('#txtSearch').val())
});
$('#favoritebox').find('a').each(function () {
    var hs = this.href.split('/');
    hs.length = 3;
    var img = $(this).find('img');
    img[0].onerror = function () { this.src = '/images/net.svg'; this.onerror = null; }
    if (img.attr('data-icon') != "") {
        img[0].src = img.attr('data-icon');
    } else {
        if (this.href.indexOf('http://') >= 0) {
            img[0].src = '/images/net.svg';
        } else {
            img[0].src = hs.join('/') + "/favicon.ico";
        }
    }
});

$('#txtSearch').val(localStorage["favorite-search"] || "");
var defaultGroup = localStorage["favorite-group"] || "";
if (defaultGroup != "") {
    $('#seGroup').find('option').each(function () {
        if (this.value == defaultGroup) {
            $('#seGroup').val(this.value);
            $('#btnSearch')[0].click();
            return false;
        }
    });
}
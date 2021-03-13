$(function () {
    $('#icon-carousel').carousel({ interval: 5000 });
    $('#main-nav').onePageNav({ currentClass: 'active', });

    if (navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i)) {
        $('.fa-hover a').each(function (i, e) {
            $(this).removeClass("col-xs-11");
        });
    } else {
        $('.fa-hover').each(function (i, e) {
            var tempEle = $(this);
            tempEle.append("<span class='clipboard text-muted' style='line-height: 32px;'><i class='fa fa-clipboard'></i></span>");
            tempEle.children('.clipboard').zclip({
                path: "./assets/libs/zclip/zclip_1.1.2_ZeroClipboard.swf",
                copy: function () {//复制内容
                    return "fa-" + tempEle.text().substr(12).replace(" (alias)", "");
                },
                afterCopy: function () {//复制成功
                    toastr.warning('已成功复制标签，请直接粘贴');
                }
            });
        });
    }
});
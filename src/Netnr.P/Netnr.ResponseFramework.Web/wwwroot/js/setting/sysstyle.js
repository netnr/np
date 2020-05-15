var rf = parent.rf;
try {
    var fs = rf.ls(rf.key.fontsize),
        ff = rf.ls(rf.key.fontfamily),
        bs = rf.ls(rf.key.btntype);
    if (fs) {
        $('#seFontSize').val(fs);
        document.body.style["font-size"] = fs;
    }
    if (ff) {
        $('#seFontFamily').val(ff);
        document.body.style["font-family"] = ff;
    }
    if (bs == 1) {
        $('#seButtonStyle').val(bs);
    }
} catch (e) { }

document.getElementById('seFontSize').onchange = function () {
    document.body.style["font-size"] = this.value;
}
document.getElementById('seFontFamily').onchange = function () {
    document.body.style["font-family"] = this.value;
}

document.getElementById('btnSaveStyle').onclick = function () {
    try {
        rf.ls(rf.key.fontsize, $('#seFontSize').val());
        rf.ls(rf.key.fontfamily, $('#seFontFamily').val());
        rf.ls(rf.key.btntype, $('#seButtonStyle').val());
        art('保存成功，是否整个网页刷新？', function () {
            top.location.reload(false);
        });
    } catch (e) {
        art('您的浏览器版本过低，不支持该功能');
    }
}
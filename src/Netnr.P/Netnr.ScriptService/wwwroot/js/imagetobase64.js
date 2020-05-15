$('#txtFile').change(function () {
    if (this.files.length) {
        var file = this.files[0];
        if (!/image\/\w+/.test(file.type)) {
            jz.msg("请确保文件为图像类型");
            return false;
        }
        var r = new FileReader();
        r.onload = function (e) {
            $('#txtBase64').val(this.result)
            $('#labSize').html("图片大小：" + Math.round(this.result.length / 1024 * 1000) / 1000 + " KB");
        }
        r.readAsDataURL(file);
    }
});

$('#btnBase64ToImage').click(function () {
    $('#viewBase64').html('<img src="' + $('#txtBase64').val() + '" style="max-width:100%" />');
});

$(window).on('load', function () {
    if (typeof (FileReader) === 'undefined') {
        jz.alert("你的浏览器不支持 FileReader <br />请使用现代浏览器操作！");
        $('#txtFile')[0].disabled = true;
    }
})
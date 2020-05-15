$('#btnUpwd').click(function () {
    if (z.FormRequired('red', '#fv_upwd', true)) {
        var fv = $('#fv_upwd'); txts = fv.find('input');
        if (txts.eq(1).val() != txts.eq(2).val()) {
            art('两次输入的密码不一致');
        } else if (txts.eq(1).val().length < 5) {
            art('密码长度至少 5 位');
        } else {
            $.ajax({
                url: '/account/updatenewpassword',
                type: 'post',
                dataType: 'json',
                data: fv.serialize(),
                success: function (data) {
                    txts.val('');
                    art(data.msg);
                }
            })
        }
    }
    return false;
});
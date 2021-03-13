//非嵌套显示
top != self && (top.location = self.location);

//低版本跳转
try { eval("() => 1") } catch (e) { top.location = "/home/ub" }

//http=>https
if (location.hostname != "localhost" && location.protocol == "http:") {
    location = location.href.replace("http://", "https://");
}

//登录
function loginValid() {

    $('.submit')[0].disabled = true;

    $.ajax({
        url: "/Account/LoginValidation?" + new Date().valueOf(),
        type: "POST",
        data: $('form').serialize(),
        dataType: 'json',
        success: function (data) {
            if (data.code == 200) {
                window.location.href = data.data;
            }
            else {
                $('form')[0].reset();
                $("#img_captcha")[0].click();
                alert(data.msg);
            }
        },
        error: function () {
            $("#img_captcha")[0].click();
            alert('网络错误');
        },
        complete: function () {
            $('.submit')[0].disabled = false;
        }
    })

    return false;
}

//刷新验证码
document.getElementById('img_captcha').onclick = function () {
    this.src = "/account/captcha?" + new Date().valueOf();
};

//自适应高度
$(window).on('load resize', function () {
    var rc = $('.rightcard');
    rc.height(Math.max($('.leftcard').outerHeight(), rc.children().outerHeight(), $(window).height()) - 1);
});
function loadOSinfo() {
    $.ajax({
        url: "/Tool/QueryServerInfo?__nolog=true",
        dataType: 'json',
        success: function (data, _status, xhr) {
            if (data.code == 200) {
                var nl = '<br />';
                data = data.data.trim().split('\r\n\r\n').join(nl);

                var htm = [];
                xhr.getAllResponseHeaders().replace(/server: (.*)/, function () {
                    htm.push(' 🌺 服务： ' + arguments[1] + nl);
                })
                htm.push(data);
                $('#divAs').html(htm.join('')).css('line-height', '2.5');
            } else {
                $('#divAs').html('<h4 class="text-center text-danger">获取服务器信息异常</h4>');
            }

            //自动刷新
            setTimeout(loadOSinfo, 1000 * 10);
        },
        error: function () {
            $('#divAs').html('<h4 class="text-center text-danger">获取服务器信息异常</h4>');

            //自动刷新
            setTimeout(loadOSinfo, 1000 * 10);
        }
    });
}

loadOSinfo();
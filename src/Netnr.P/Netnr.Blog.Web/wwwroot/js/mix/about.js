function loadOSinfo() {
    var fd = new FormData();
    fd.append('__nolog', 'true');
    fd.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);

    var server = "";
    fetch('/Mix/AboutServerStatus', {
        method: 'POST',
        body: fd
    }).then(resp => {
        nr.resp = resp;
        server = resp.headers.get('server');
        return resp.json();
    }).then(res => {
        if (res.code == 200) {
            nr.domSystemStatus.innerHTML = ` Duration: ${nr.domHidDuration.value} Days\n\nServer: ${server}${res.data}`;
            nr.domSystemStatus.style.whiteSpace = 'pre-line';
        } else {
            nr.domSystemStatus.innerHTML = '<h4 class="text-danger">获取服务器信息异常</h4>';
        }

        //自动刷新
        setTimeout(loadOSinfo, 1000 * 10);
    }).catch(ex => {
        //自动刷新
        setTimeout(loadOSinfo, 1000 * 10);
    })
}

loadOSinfo();
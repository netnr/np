nr.onReady = function () {
    nr.domTxtQuery.value = page.ua;
    page.parse(page.ua);

    nr.domTxtQuery.addEventListener('input', function () {
        var val = nr.domTxtQuery.value.trim();
        if (val == "") {
            page.parse(page.ua);
        } else {
            page.parse(val);
        }
    })
}
var page = {
    ua: navigator.userAgent,
    parse: function (ua) {
        var uaobj = new DeviceDetector().parse(ua);
        nr.domCardResult.innerHTML = `<sl-textarea label="解析结果" rows="20"></sl-textarea>`;
        nr.domCardResult.querySelector('sl-textarea').value = JSON.stringify(uaobj, null, 2);
    }
}
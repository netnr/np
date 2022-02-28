var pg = {
    ua: navigator.userAgent,
    init: function () {
        document.querySelector('.nr-ua').addEventListener('input', function () {
            if (this.value == "") {
                pg.parse(pg.ua);
            } else {
                pg.parse(this.value);
            }
        }, false);

        document.querySelector('.nr-ua').value = pg.ua;
        pg.parse(pg.ua);
    },
    parse: function (ua) {
        var uaobj = new DeviceDetector().parse(ua);
        if (uaobj.bot) {
            document.querySelector('.nr-result').innerHTML = `<div class="col-md-12"><pre class="bg-dark rounded text-white fs-5 m-0 p-3">${JSON.stringify(uaobj.bot, null, 2)}</pre></div>`;
        } else {
            var uc = uaobj.client,
                uo = uaobj.os,
                ud = uaobj.device,
                ci = `${uc.name} ${uc.version}`,
                ei = `${uc.engine} ${uc.engineVersion}`,
                oi = `${uo.name} ${uo.version} ${uo.platform}`,
                di = `${ud.brand} ${ud.model}`;

            document.querySelector('.nr-result').innerHTML = `
            <div class="col-md-auto"><div class="border rounded fs-5 p-3 mb-3">浏览器：${ci}</div></div>
            <div class="col-md-auto"><div class="border rounded fs-5 p-3 mb-3">内核：${ei}</div></div>
            <div class="col-md-auto"><div class="border rounded fs-5 p-3 mb-3">系统：${oi}</div></div>
            <div class="col-md-auto"><div class="border rounded fs-5 p-3 mb-3">设备：${di}</div></div>
            `;
        }
    }
}

pg.init();
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
        document.querySelector('.nr-result').innerHTML = `<div class="col-md-12"><pre class="bg-dark rounded text-white fs-5 m-0 p-3">${JSON.stringify(uaobj, null, 2)}</pre></div>`;
    }
}

pg.init();
var vd = {
    video: document.querySelector('video'),
    init: function () {
        fetch('/_assets/file/m3u8.txt').then(x => x.text()).then(res => {
            var ds = res.split('#EXTINF:-1'), sehtm = [];
            sehtm.push('<option value="">（选择源）</option>')
            ds.forEach(item => {
                var matchm3u8 = /http(.*).m3u8/i.exec(item);
                var namem3u8 = /,(.*)/.exec(item)

                if (matchm3u8 && namem3u8) {
                    sehtm.push('<option value="' + matchm3u8[0] + '">' + namem3u8[1] + '</option>')
                }
            })

            document.querySelector('.nr-select-m3u8').innerHTML = sehtm.join('');
        })

        document.querySelector('.nr-text-m3u8').addEventListener('input', function () {
            if (this.value.toLowerCase().indexOf('.m3u8') >= 0) {
                vd.play(this.value, 1);
            }
        }, false);

        document.querySelector('.nr-select-m3u8').addEventListener('change', function () {
            if (this.value != "") {
                vd.play(this.value, true);
            }
        }, false);

        vd.autoSize();
        window.onresize = vd.autoSize;
    },
    play: function (m3u8, isplay) {
        if (Hls.isSupported()) {
            var hls = new Hls();
            hls.loadSource(m3u8);
            hls.attachMedia(vd.video);
            hls.on(Hls.Events.MANIFEST_PARSED, function () {
                isplay && vd.video.play();
            });
        }
        else if (vd.video.canPlayType('application/vnd.apple.mpegurl')) {
            vd.video.src = m3u8;
            vd.video.addEventListener('loadedmetadata', function () {
                isplay && vd.video.load();
            });
        } else {
            vd.video.parentNode.innerHTML = "您的浏览器不支持，请换Chrome浏览器、Firefox浏览器";
        }
    },
    autoSize: function () {
        var vh = document.documentElement.clientHeight - vd.video.getBoundingClientRect().top - 10;
        vd.video.style.maxHeight = vh + "px";
    }
}

vd.init();
var vd = {
    m3u8: function () { return document.getElementById('seM3u8').value },
    video: document.getElementById('video'),

    init: function () {
        $.ajax({
            url: "/lib/m3u/tv.m3u",
            success: function (data) {
                var ds = data.split('#EXTINF:-1'), sehtm = [];
                ds.forEach(d => {
                    var matchm3u8 = /http(.*).m3u8/i.exec(d);
                    var namem3u8 = /,(.*)/.exec(d)

                    if (matchm3u8 && namem3u8) {
                        sehtm.push('<option value="' + matchm3u8[0] + '">' + namem3u8[1] + '</option>')
                    }
                })

                $('#seM3u8').html(sehtm.join(''));

                vd.play(vd.m3u8(), 1);
            }
        })

        document.getElementById('txtM3u8').oninput = function () {
            if (this.value.toLowerCase().indexOf('.m3u8') >= 0) {
                vd.play(this.value, 1);
            }
        }

        document.getElementById('seM3u8').onchange = function () {
            vd.play(this.value, 1);
        }

        vd.autoSize();
        window.onresize = vd.autoSize;
    },
    play(m3u8, isplay) {
        if (Hls.isSupported()) {
            var hls = new Hls();
            hls.loadSource(m3u8);
            hls.attachMedia(video);
            hls.on(Hls.Events.MANIFEST_PARSED, function () {
                isplay && video.play();
            });
        }
        // hls.js is not supported on platforms that do not have Media Source Extensions (MSE) enabled.
        // When the browser has built-in HLS support (check using `canPlayType`), we can provide an HLS manifest (i.e. .m3u8 URL) directly to the video element through the `src` property.
        // This is using the built-in support of the plain video element, without using hls.js.
        // Note: it would be more normal to wait on the 'canplay' event below however on Safari (where you are most likely to find built-in HLS support) the video.src URL must be on the user-driven
        // white-list before a 'canplay' event will be emitted; the last video event that can be reliably listened-for when the URL is not on the white-list is 'loadedmetadata'.
        else if (video.canPlayType('application/vnd.apple.mpegurl')) {
            video.src = m3u8;
            video.addEventListener('loadedmetadata', function () {
                isplay && video.play();
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
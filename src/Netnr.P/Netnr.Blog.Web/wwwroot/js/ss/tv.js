nr.onChangeSize = function (ch) {
    var vh = ch - page.video.getBoundingClientRect().top - 30;
    page.video.style.height = vh + "px";
}

nr.onReady = function () {
    fetch('/file/ss/m3u8.txt').then(x => x.text()).then(res => {
        res.split('#EXTINF:-1').forEach(item => {
            var matchm3u8 = /http(.*).m3u8/i.exec(item);
            var namem3u8 = /,(.*)/.exec(item)

            if (matchm3u8 && namem3u8) {
                var domItem = document.createElement('sl-menu-item');
                domItem.value = matchm3u8[0];
                domItem.innerHTML = namem3u8[1];
                nr.domSeM3u8.appendChild(domItem);
            }
        })
    })

    nr.domTxtM3u8.addEventListener('input', function () {
        page.play(this.value, true);
    }, false);

    nr.domSeM3u8.addEventListener('sl-change', function () {
        console.log(decodeURIComponent(this.value));
        if (this.value != "") {
            page.play(decodeURIComponent(this.value), true);
        }
    }, false);
}

var page = {
    video: document.querySelector('video'),
    play: function (m3u8, isplay) {
        page.video.classList.remove('invisible');

        if (Hls.isSupported()) {
            var hls = new Hls();
            hls.loadSource(m3u8);
            hls.attachMedia(page.video);
            hls.on(Hls.Events.MANIFEST_PARSED, function () {
                isplay && page.video.play();
            });
        }
        else if (page.video.canPlayType('application/vnd.apple.mpegurl')) {
            page.video.src = m3u8;
            page.video.addEventListener('loadedmetadata', function () {
                isplay && page.video.load();
            });
        } else {
            page.video.parentNode.innerHTML = "您的浏览器不支持，请换Chrome浏览器、Firefox浏览器";
        }
    }
}
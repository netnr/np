import { nrVary } from "../../nrVary";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/tv",

    init: async () => {
        let resp = await fetch('/file/data-m3u8.txt');
        let result = await resp.text();
        result.split('#EXTINF:-1').forEach(item => {
            let matchm3u8 = /http(.*).m3u8/i.exec(item);
            let namem3u8 = /,(.*)/.exec(item)

            if (matchm3u8 && namem3u8) {
                let domItem = document.createElement('option');
                domItem.value = matchm3u8[0];
                domItem.innerHTML = namem3u8[1];
                nrVary.domSeM3u8.appendChild(domItem);
            }
        })

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domTxtM3u8.addEventListener('input', async function () {
            await nrPage.play(this.value, true)
        });

        nrVary.domSeM3u8.addEventListener('input', async function () {
            if (this.value != "") {
                await nrPage.play(decodeURIComponent(this.value), true);
            }
        });
    },

    play: async (m3u8, isplay) => {
        nrVary.domVideo.classList.remove('invisible');
        await nrcRely.remote('hls.js')

        if (Hls.isSupported()) {
            let hls = new Hls();
            hls.loadSource(m3u8);
            hls.attachMedia(nrVary.domVideo);
            hls.on(Hls.Events.MANIFEST_PARSED, function () {
                isplay && nrVary.domVideo.play();
            });
        }
        else if (nrVary.domVideo.canPlayType('application/vnd.apple.mpegurl')) {
            nrVary.domVideo.src = m3u8;
            nrVary.domVideo.addEventListener('loadedmetadata', function () {
                isplay && nrVary.domVideo.load();
            });
        } else {
            nrVary.domVideo.parentNode.innerHTML = "您的浏览器不支持，请换Chrome浏览器、Firefox浏览器";
        }
    }
}

export { nrPage };
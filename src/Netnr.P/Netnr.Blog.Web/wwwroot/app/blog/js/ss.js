/* ScriptService */
var ss = {
    apiServer: "https://www.netnr.eu.org",

    fetch: (obj, hostIndex) => new Promise((resolve, reject) => {
        var hosts = ["https://cors.eu.org/", "https://cors.zme.ink/", "https://www.netnr.eu.org/api/v1/Proxy?url="];
        var url = encodeURIComponent(obj.url);
        var encoding = obj.encoding || "utf-8"; //GBK
        delete obj.url;
        delete obj.encoding;

        if (hostIndex != null) {
            url = hosts[hostIndex] + url;
            fetch(url, obj).then(resp => resp.blob()).then(blob => {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var result = e.target.result;
                    resolve(result);
                }
                reader.readAsText(blob, encoding)
            }).catch(reject);
        } else {
            upstream(hosts, function (fast) {
                url = fast + url;
                fetch(url, obj).then(resp => resp.blob()).then(blob => {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        var result = e.target.result;
                        resolve(result);
                    }
                    reader.readAsText(blob, encoding)
                }).catch(reject);
            }, 1);
        }
    }),

    loading: function (isLoading) {
        var slaToggler = nr.domNavbarToggler.querySelector("sl-animation");
        var slaSS = nr.domSlaSs;

        if (isLoading) {
            slaToggler.duration = slaSS.duration = 1500;
            slaToggler.keyframes = slaSS.keyframes = [
                {
                    offset: 0,
                    easing: 'cubic-bezier(0.250, 0.460, 0.450, 0.940)',
                    fillMode: 'both',
                    transformOrigin: 'center center',
                    transform: 'rotate(0)'
                },
                {
                    offset: 1,
                    easing: 'cubic-bezier(0.250, 0.460, 0.450, 0.940)',
                    fillMode: 'both',
                    transformOrigin: 'center center',
                    transform: 'rotate(360deg)'
                }
            ];
        } else {
            slaSS.duration = 10000;
            slaToggler.keyframes = slaSS.keyframes = [];
            slaToggler.cancel();
        }
    }
}

export { ss }
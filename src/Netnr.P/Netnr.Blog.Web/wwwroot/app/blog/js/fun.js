/*
 * netnr
 * 2022-04-09
 */

window.addEventListener("DOMContentLoaded", function () {
    if (console) {
        var outs = [], domain = "https://www.netnr.com", fi = function () { return { msg: "", style: "" } };

        var oi = fi();
        oi.msg = "NET牛人";
        oi.style = "padding:10px 40px 10px;line-height:50px;background:url('" + domain + "/favicon.svg') no-repeat;background-size:15% 100%;font-size:1.8rem;color:orange";
        outs.push(oi);

        oi = fi();
        oi.msg = domain;
        oi.style = "background-image:-webkit-gradient( linear, left top, right top, color-stop(0, #f22), color-stop(0.15, #f2f), color-stop(0.3, #22f), color-stop(0.45, #2ff), color-stop(0.6, #25e),color-stop(0.75, #4f2), color-stop(0.9, #f2f), color-stop(1, #f22) );color:transparent;-webkit-background-clip: text;font-size:1.5em;"
        outs.push(oi);

        oi = fi();
        var vls = [
            { name: "GitHub", link: "https://github.com/netnr" }
        ];
        for (var i = 0; i < vls.length; i++) {
            var vi = vls[i];
            oi.msg += "\r\n" + vi.name + "：\r\n" + vi.link + "\r\n"
        }
        outs.push(oi);

        if (!("ActiveXObject" in window)) {
            outs.map(function (x) {
                console.log("%c" + x.msg, x.style);
            });
        }

        //耗时
        if (window.performance) {
            window.funsi = setInterval(function () {
                var t = performance.timing;
                if (t.loadEventEnd) {
                    console.log(JSON.stringify({
                        load: t.loadEventEnd - t.navigationStart,
                        ready: t.domComplete - t.responseEnd,
                        request: t.responseEnd - t.requestStart
                    }))
                    clearInterval(window.funsi);
                }
            }, 10)
        }
    }

    let day = (new Date(new Date().valueOf() + 8 * 3600000)).toISOString().substring(5, 10);
    switch (day) {
        case "04-05":
            {
                var des = document.documentElement.style;
                des["filter"] = "progid: DXImageTransform.Microsoft.BasicImage(grayscale = 1)";
                des["-webkit-filter"] = "grayscale(100%)";
            }
            break;
    }

}, false);
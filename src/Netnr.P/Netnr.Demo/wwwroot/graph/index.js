var graph = {
    domGraph: document.querySelector('.nr-graph'),
    init: () => {
        graph.viewCaptcha();
        graph.viewResize();
        graph.viewWatermark();
    },

    viewCaptcha: () => {
        var domTitle = document.createElement("H2");
        domTitle.innerHTML = "Captcha 验证码";
        graph.domGraph.appendChild(domTitle);

        var items = [
            { name: "MagickNET", pathname: "/GraphDemo/MagickNET/Captcha" },
            { name: "SixLaborsImageSharpDrawing", pathname: "/GraphDemo/SixLaborsImageSharpDrawing/Captcha" },
            { name: "SkiaSharp", pathname: "/GraphDemo/SkiaSharp/Captcha" },
            { name: "SystemDrawingCommon", pathname: "/GraphDemo/SystemDrawingCommon/Captcha" },
            { name: "NetVips", pathname: "/GraphDemo/NetVips/Captcha" }
        ].map(x => `<div style="margin:1em"><img src="${x.pathname}" /><br/><b>${x.name}</b></div>`);

        var domCard = document.createElement("div");
        domCard.innerHTML = `
<a target="_blank" href="/Graph/CaptchaTest/1">Loop 1</a> &nbsp;
<a target="_blank" href="/Graph/CaptchaTest/10">Loop 10</a> &nbsp;
<a target="_blank" href="/Graph/CaptchaTest/66">Loop 66</a>

<div style="display:flex;flex-wrap:wrap;text-align:center">${items}</div>
`;
        domCard.onclick = function (e) {
            var target = e.target;
            if (target.nodeName == "IMG") {
                target.src = `${new URL(target.src).pathname}?${Math.random()}`;
            }
        }

        graph.domGraph.appendChild(domCard);
    },

    viewResize: () => {
        var domTitle = document.createElement("H2");
        domTitle.innerHTML = "Resize 缩略图";
        graph.domGraph.appendChild(domTitle);

        var items = [
            { name: "SystemDrawingCommon", pathname: "/GraphDemo/SystemDrawingCommon/Resize" },
            { name: "SkiaSharp", pathname: "/GraphDemo/SkiaSharp/Resize" }
        ].map(x => `<div style="margin:1em"><img src="${x.pathname}" /><br/><b>${x.name}</b></div>`);

        var domCard = document.createElement("div");
        domCard.innerHTML = `
<div style="display:flex;flex-wrap:wrap;text-align:center">
    <div style="margin:1em">
        <img src="/images/netnr_avatar.jpg" /><br/><b>原图</b>
    </div>
    ${items}
</div>

`;

        graph.domGraph.appendChild(domCard);
    },

    viewWatermark: () => {
        var domTitle = document.createElement("H2");
        domTitle.innerHTML = "Watermark 水印";
        graph.domGraph.appendChild(domTitle);

        var items = [
            { name: "SystemDrawingCommon", pathname: "/GraphDemo/SystemDrawingCommon/Watermark" },
            { name: "SkiaSharp", pathname: "/GraphDemo/SkiaSharp/Watermark" },
            { name: "MagickNET", pathname: "/GraphDemo/MagickNET/Watermark" },
        ].map(x => `<div style="margin:1em"><img src="${x.pathname}" /><br/><b>${x.name}</b></div>`);

        var domCard = document.createElement("div");
        domCard.innerHTML = `
<div style="display:flex;flex-wrap:wrap;text-align:center">
    <div style="margin:1em">
        <img src="/images/netnr_avatar.jpg" /><br/><b>原图</b>
    </div>
    ${items}
</div>

`;

        graph.domGraph.appendChild(domCard);
    }
};

graph.init();
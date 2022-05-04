/**
 * 2021-12-15
 * by netnr
 */
const oConsole = Object.assign({
    isHide: new URLSearchParams(location.search).get("pure") == "1",
    oGui: null,
    oTool: null,
    init: () => {

        let st = document.createElement("STYLE");
        st.innerHTML = `
.nr-tool {
    position: fixed;
    z-index: 999;
    bottom: 1em;
    right: 1em;
    opacity: .5;
    filter: alpha(opacity=50)
}
.nr-tool a {
    color: #777;
    margin: 0 .3em;
    text-decoration: none;
}

.nr-console {
    position: fixed;
    top: 30%;
    right: 1.5em;
    bottom: 3em;
    zoom: 1;
    z-index: 999;
    display: none;
    padding: 0;
    opacity: 0.9;
    width: 40em;
    max-width: 60vw;
    overflow: hidden;
    overflow-y: auto;
    border-radius: 0.3em;
    border: 1px solid #cccccc;
    background-color: rgb(240, 240, 240);
}
.nr-console pre {
    margin: 0;
    padding: 8px;
    font-size: .9em;
    min-height: 12px;
    white-space: pre-wrap;
    border: none;
    border-bottom: 1px solid #cccccc;
}
.nr-console pre:last-child {
    border-top: none;
}
`;
        document.head.appendChild(st);

        let code = location.pathname.split("/").pop();
        let tdom = document.createElement("div");
        tdom.className = "nr-tool"
        tdom.innerHTML = `<a href='javascript:void(0);' title="Toggle Console">👁</a>
        <a href='/run/code/${code}/edit' title="Editor">📝</a>
        <a href='/run/code/${code}?pure=1' title="Close">✖</a>
        `;
        oConsole.oTool = tdom;
        document.body.appendChild(tdom);

        let odom = document.createElement('div');
        odom.classList.add('nr-console');
        document.body.appendChild(odom);
        oConsole.oGui = odom;

        //switch
        tdom.querySelector('a').addEventListener('click', function () {
            oConsole.oGui.style.display = getComputedStyle(oConsole.oGui).getPropertyValue("display") == "none" ? "block" : "none";
        }, false);
    },
    type: function (obj) {
        var tv = {}.toString.call(obj);
        return tv.split(' ')[1].replace(']', '');
    },
    write: function () {
        var fn = this;
        var item = document.createElement('pre'), len = arguments.length, oarr = [];
        for (var i = 0; i < len; i++) {
            try {
                var aval = arguments[i], atype = oConsole.type(aval);
                if (['Object', 'Array'].includes(atype)) {
                    if (fn == "table" && atype == "Array" && typeof aval[0] == "object") {
                        var ivs = [], iks = Object.keys(aval[0]), lines = [];
                        ivs.push(iks.join('\t|\t'))
                        iks.forEach(() => lines.push('---'));
                        ivs.push(lines.join(' | '))
                        aval.forEach(avi => {
                            ivs.push(Object.values(avi).join('\t|\t'))
                        })
                        oarr.push(ivs.join('\n'));
                        item.style.whiteSpace = "nowrap"
                    } else {
                        oarr.push(JSON.stringify(aval))
                    }
                } else if (atype.endsWith("Element")) {
                    var nodeName = aval.nodeName.toLowerCase(), attrs = [], nodeChild = "";
                    for (var a = 0; a < aval.attributes.length; a++) {
                        var attr = aval.attributes[a];
                        attrs.push(`${attr.name}="${attr.value}"`);
                    }
                    if (attrs.length) {
                        attrs = ` ${attrs.join(' ')}`;
                    }
                    if (aval.children.length) {
                        nodeChild = "..."
                    }
                    oarr.push(`<${nodeName}${attrs}>${nodeChild}</${nodeName}>`)
                } else {
                    oarr.push(aval.toString())
                }
            } catch (_) { }
        }
        var oval = oarr.join(' ');
        if (oval.length > 999) {
            oval = oval.substring(0, 999) + " ...";
        }
        item.innerText = oval;

        var echild = oConsole.oGui.children;
        if (echild.length == 0) {
            oConsole.oGui.style.display = "block";
        }
        if (echild.length > 999) {
            for (var i = 0; i < 333; i++) {
                echild[i].remove()
            }
        }

        if (fn == "clear") {
            oConsole.oGui.innerHTML = '';
        }
        oConsole.oGui.appendChild(item);
        oConsole.oGui.scrollBy(0, 99999);
    }
}, console);

if (!oConsole.isHide) {
    oConsole.init();

    for (let fn in oConsole) {
        console[fn] = function () {
            oConsole[fn].apply(this, arguments)
            oConsole.write.apply(fn, arguments)
        }
    }
}
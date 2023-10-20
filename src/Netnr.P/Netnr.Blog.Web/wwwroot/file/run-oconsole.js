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
    opacity: .6;
    filter: alpha(opacity=60)
}
.nr-tool a {
    color: #777;
    margin: 0 .3em;
    text-decoration: none;
}

.nr-console {
    color-scheme: dark;

    position: fixed;
    top: 50%;
    right: 1.5em;
    bottom: 3em;
    zoom: 1;
    z-index: 999;
    display: none;
    padding: 0;
    opacity: 0.9;
    width: 35em;
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
        tdom.innerHTML = `<a href='javascript:void(0);' title="Toggle Console">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                <path d="M16 8s-3-5.5-8-5.5S0 8 0 8s3 5.5 8 5.5S16 8 16 8zM1.173 8a13.133 13.133 0 0 1 1.66-2.043C4.12 4.668 5.88 3.5 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.133 13.133 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755C11.879 11.332 10.119 12.5 8 12.5c-2.12 0-3.879-1.168-5.168-2.457A13.134 13.134 0 0 1 1.172 8z"/>
                <path d="M8 5.5a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5zM4.5 8a3.5 3.5 0 1 1 7 0 3.5 3.5 0 0 1-7 0z"/>
            </svg>
        </a>
        <a href='/run/code/${code}?pure=1' title="Hide Console">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                <path d="M13.359 11.238C15.06 9.72 16 8 16 8s-3-5.5-8-5.5a7.028 7.028 0 0 0-2.79.588l.77.771A5.944 5.944 0 0 1 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.134 13.134 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755-.165.165-.337.328-.517.486l.708.709z"/>
                <path d="M11.297 9.176a3.5 3.5 0 0 0-4.474-4.474l.823.823a2.5 2.5 0 0 1 2.829 2.829l.822.822zm-2.943 1.299.822.822a3.5 3.5 0 0 1-4.474-4.474l.823.823a2.5 2.5 0 0 0 2.829 2.829z"/>
                <path d="M3.35 5.47c-.18.16-.353.322-.518.487A13.134 13.134 0 0 0 1.172 8l.195.288c.335.48.83 1.12 1.465 1.755C4.121 11.332 5.881 12.5 8 12.5c.716 0 1.39-.133 2.02-.36l.77.772A7.029 7.029 0 0 1 8 13.5C3 13.5 0 8 0 8s.939-1.721 2.641-3.238l.708.709zm10.296 8.884-12-12 .708-.708 12 12-.708.708z"/>
            </svg>
        </a>
        <a href='/run/edit/${code}' title="Editor">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                <path d="M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z"/>
                <path fill-rule="evenodd" d="M1 13.5A1.5 1.5 0 0 0 2.5 15h11a1.5 1.5 0 0 0 1.5-1.5v-6a.5.5 0 0 0-1 0v6a.5.5 0 0 1-.5.5h-11a.5.5 0 0 1-.5-.5v-11a.5.5 0 0 1 .5-.5H9a.5.5 0 0 0 0-1H2.5A1.5 1.5 0 0 0 1 2.5v11z"/>
            </svg>
        </a>
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
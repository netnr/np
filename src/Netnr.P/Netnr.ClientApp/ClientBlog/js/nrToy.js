/*
 * netnr
 * 2022-09-01
 */

let nrToy = {
    init: async () => {

        //console
        [{
            msg: "https://www.netnr.com",
            css: "background-image:-webkit-gradient( linear, left top, right top, color-stop(0, #f22), color-stop(0.15, #f2f), color-stop(0.3, #22f), color-stop(0.45, #2ff), color-stop(0.6, #25e),color-stop(0.75, #4f2), color-stop(0.9, #f2f), color-stop(1, #f22) );color:transparent;-webkit-background-clip: text;font-size:1.5em;"
        },
        { msg: "https://github.com/netnr", css: "" }].forEach(item => {
            console.log("%c" + item.msg, item.css);
        })

        //special day
        let day = (new Date(new Date().valueOf() + 8 * 3600000)).toISOString().substring(5, 10);
        switch (day) {
            case "04-05":
                {
                    let des = document.documentElement.style;
                    des["filter"] = "progid: DXImageTransform.Microsoft.BasicImage(grayscale = 1)";
                    des["-webkit-filter"] = "grayscale(100%)";
                }
                break;
        }
    },
}

export { nrToy }
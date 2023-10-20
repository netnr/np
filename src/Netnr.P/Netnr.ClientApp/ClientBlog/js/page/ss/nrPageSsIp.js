import { nrcBase } from "../../../../frame/nrcBase";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/ip",

    init: async () => {

        try {
            window["ipCallback"] = function (json) {
                console.debug(json)
                nrVary.domTxtTaobao.value = json.ip;
            }
            await nrcBase.importScript('https://www.taobao.com/help/getip.php');
        } catch (ex) {
            nrVary.domTxtTaobao.placeholder = "网络错误";
        }

        try {
            await nrcBase.importScript('https://vv.video.qq.com/checktime?otype=json');
            nrVary.domTxtQq.value = QZOutputJson.ip;
        } catch (ex) {
            nrVary.domTxtQq.placeholder = "网络错误";
        }

        try {
            let resp = await fetch('https://js.org/cdn-cgi/trace');
            let result = await resp.text();
            let ip = result.split('\n').find(x => x.startsWith('ip=')).substring(3);
            nrVary.domTxtCloudflare.value = ip;
        } catch (ex) {
            nrVary.domTxtCloudflare.placeholder = "网络错误";
        }
    },
}

export { nrPage };
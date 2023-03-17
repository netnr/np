import { nrcBase } from "../../../../frame/nrcBase";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: '/mix/about',

    init: async () => {
        await nrPage.reqSystemStatus();
    },

    reqSystemStatus: async () => {

        let fd = new FormData();
        fd.append('__nolog', 'true');
        fd.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);

        let vm = await nrcBase.fetch('/Mix/AboutServerStatus', { method: "POST", body: fd });

        if (vm.error || vm.resp.ok == false) {
            nrVary.domSystemStatus.innerHTML = '<h4 class="text-danger">获取服务器信息异常</h4>';
        } else if (vm.result && vm.result.code == 200) {
            let server = vm.resp.headers.get("server");
            nrVary.domSystemStatus.innerHTML = ` Duration: ${nrVary.domHidDuration.value} Days\n\nServer: ${server}${vm.result.data}`;
            nrVary.domSystemStatus.style.whiteSpace = 'pre-line';
        }

        //自动刷新
        setTimeout(nrPage.reqSystemStatus, 1000 * 10);
    }
}

export { nrPage };
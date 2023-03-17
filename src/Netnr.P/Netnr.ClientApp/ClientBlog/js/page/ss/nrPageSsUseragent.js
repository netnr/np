import { nrcBase } from "../../../../frame/nrcBase";
import { nrcRely } from "../../../../frame/nrcRely";
import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/useragent",

    init: async () => {
        nrVary.domTxtQuery.value = navigator.userAgent;

        nrVary.domTxtQuery.addEventListener('input', async function () {
            let val = nrVary.domTxtQuery.value.trim();
            await nrPage.parse(val || navigator.userAgent);
        });
        nrcBase.dispatchEvent('input', nrVary.domTxtQuery);
    },

    dd: null,
    parse: async (ua) => {
        await nrcRely.remote('bowser.js');
        let parsedResult = bowser.getParser(ua).parsedResult;

        nrVary.domCardResult.innerHTML = `<label class="form-label">解析结果</label><textarea class="form-control" rows="18"></textarea>`;
        nrVary.domCardResult.querySelector('textarea').value = JSON.stringify(parsedResult, null, 2);
    }
}

export { nrPage };
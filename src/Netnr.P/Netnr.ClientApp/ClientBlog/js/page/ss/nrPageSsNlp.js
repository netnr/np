import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/nlp",

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        if (Intl.Segmenter) {
            nrVary.domBtnLocal.classList.remove('d-none');

            nrVary.domBtnLocal.addEventListener('click', function () {
                let val = nrVary.domTxtContent.value.trim();

                let segm = new Intl.Segmenter('cn', { granularity: 'word' });
                let vals = Array.from(segm.segment(val));
                nrVary.domTxtResult.value = JSON.stringify(vals.map(x => x.segment), null, 2);
            });
        }

        nrVary.domBtnQuery.addEventListener('click', async function () {
            let val = nrVary.domTxtContent.value.trim();
            if (val == "") {
                nrVary.domTxtResult.value = "";
            } else {
                nrApp.setLoading(this);

                let fd = new FormData();
                fd.append('content', val);

                let url = 'https://netnr.zme.ink/api/v1/Analysis'
                let result = await nrWeb.reqServer(url, { method: 'POST', body: fd });
                nrApp.setLoading(this, true);

                if (result.code == 200) {
                    nrVary.domTxtResult.value = JSON.stringify(result.data, null, 2);
                } else {
                    nrVary.alert(result.msg);
                }
            }
        });
    },
}

export { nrPage };
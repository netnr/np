import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

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
                nrVary.domTxtResult.value = vals.map(x => x.segment).join('    ');
            });
        }

        nrVary.domBtnQuery.addEventListener('click', async function () {
            let val = nrVary.domTxtContent.value.trim();
            if (val == "") {
                nrVary.domTxtResult.value = "";
            } else {
                if (!nrPage.segmentit) {
                    nrApp.setLoading(this);

                    await nrcRely.remote('segmentit.js');
                    nrPage.segmentit = Segmentit.useDefault(new Segmentit.Segment());

                    nrApp.setLoading(this, true);
                }
                let result = nrPage.segmentit.doSegment(val);
                console.debug(result)
                nrVary.domTxtResult.value = result.map(x => x.w).join('    ');
            }
        });
    },
}

export { nrPage };
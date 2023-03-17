import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrVary } from "../../nrVary";
import { nrWeb } from "../../nrWeb";

let nrPage = {
    pathname: '/admin/keyvalues',

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        //Grab
        nrVary.domBtnRun1.addEventListener('click', async function () {
            if (nrVary.domTxt1.value.trim() != "") {
                await nrPage.run('Grab', nrVary.domTxt1.value.split('\n').join())
            }
        })

        //Synonym
        nrVary.domBtnRun2.addEventListener('click', async function () {
            if (nrVary.domTxt2.value.trim() != "") {
                await nrPage.run('Synonym', nrVary.domTxt2.value.split('\n').join())
            }
        })

        //Tag
        nrVary.domBtnRun3.addEventListener('click', async function () {
            await nrPage.run('Tag', nrVary.domTxt3.value.split('\n').join())
        })
    },

    run: async (cmd, keys) => {
        nrApp.setLoading(nrVary.domBtnRun1);
        nrApp.setLoading(nrVary.domBtnRun2);
        nrApp.setLoading(nrVary.domBtnRun3);

        let url = `/Admin/KeyVal/${cmd}?keys=${encodeURIComponent(keys)}`;
        let result = await nrWeb.reqServer(url);

        nrApp.setLoading(nrVary.domBtnRun1, true);
        nrApp.setLoading(nrVary.domBtnRun2, true);
        nrApp.setLoading(nrVary.domBtnRun3, true);

        nrVary.domTxtResult.value = result.log.join('\n');
    }
}

export { nrPage };
import { nrcUpstream } from "../../../../frame/nrcUpstream";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/lottery",

    init: async () => {
        await nrPage.viewCard();

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domSeType.addEventListener('input', async function () {
            await nrPage.viewCard();
        });
    },

    viewCard: async () => {
        nrVary.domSeType.disabled = true;
        nrVary.domSpinner.classList.remove('d-none');

        try {

            let url = `http://cp.zgzcw.com/lottery/hisnumber.action?lotteryId=${nrVary.domSeType.value}&issueLen=36`;
            let res = await nrcUpstream.fetch(url);
            res = JSON.parse(res);

            if (res.length) {
                let htm = [];
                res.forEach(item => {
                    let codes = (item.lotteryNumber || item.tryoutNumber).split('+');
                    let code1 = codes[0].split(',');
                    let spans1 = "";
                    let spans2 = '';

                    for (let k = 0; k < code1.length; k++) {
                        spans1 += '<b class="border border-danger me-1 rounded-circle text-danger text-center d-inline-block" style="min-width:1.6em;">' + code1[k] + '</b>';
                    }

                    if (codes[1] != undefined) {
                        let code2 = codes[1].split(',');
                        for (let u = 0; u < code2.length; u++) {
                            spans2 += '<b class="border border-primary me-1 rounded-circle text-primary text-center d-inline-block" style="min-width:1.6em">' + code2[u] + '</b>';
                        }
                    }

                    htm.push(`<div class="col-xxl-2 col my-4">
                    <div class="text-nowrap">
                        <b class="h5 me-2">${item.lotteryExpect}</b>
                        <span>${(new Date(item.ernieDate)).toISOString().substring(0, 10)}</span>
                    </div>
                    <div class="text-nowrap">${spans1}${spans2}</div>
                    </div>`);
                })

                nrVary.domCardResult.innerHTML = htm.join('');
            }

        } catch (ex) {
            nrApp.logError(ex, '网络错误');
        }

        nrVary.domSeType.disabled = false;
        nrVary.domSpinner.classList.add('d-none');
    },
}

export { nrPage };
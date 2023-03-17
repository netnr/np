import { nrVary } from "../../nrVary";

let nrPage = {
    pathname: "/ss/bmi",

    init: async () => {
        [nrVary.domTxtNumber1, nrVary.domTxtNumber2].forEach(dom => {
            dom.addEventListener('input', () => nrPage.bmi())
        })
    },

    bmi: () => {
        let weight = nrVary.domTxtNumber1.value * 1;
        let height = nrVary.domTxtNumber2.value * 1;

        let bmi = (weight / Math.pow(height / 100, 2)).toFixed(2);
        let remark = "", color = "";
        if (bmi < 18.5) {
            remark = "低重";
            color = "neutral";
        } else if (bmi < 24) {
            remark = "正常";
            color = "success";
        } else if (bmi < 28) {
            remark = "超重";
            color = "warning";
        } else {
            remark = "肥胖";
            color = "danger";
        }

        nrVary.domTxtNumber3.value = bmi + " " + remark;
        nrVary.domTxtNumber3.style.color = `var(--bs-${color})`;
    }
}

export { nrPage };
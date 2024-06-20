import { nrcBase } from "../../../../frame/nrcBase";
import { nrVary } from "../../nrVary";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: "/ss/identicon",

    init: async () => {
        await nrcRely.remote('identicon');

        nrPage.build();

        nrPage.bindEvent();
    },

    bindEvent: () => {
        nrVary.domTxtSign.addEventListener('input', () => nrPage.build());
        nrVary.domTxtSize.addEventListener('input', () => nrPage.build());

        document.body.addEventListener('click', async function (event) {
            let action = event.target.dataset.action;
            let domCardView = (action || "").includes("1") ? nrVary.domCardView1 : nrVary.domCardView2;
            switch (action) {
                case "svg1":
                case "svg2":
                    nrcBase.downloadText(domCardView.innerHTML, "identicon.svg");
                    break;
                case "image1":
                case "image2":
                    {
                        await nrcRely.remote("html2canvas.js");

                        domCardView.classList.toggle('border');
                        let canvas = await html2canvas(domCardView, { backgroundColor: null });
                        domCardView.classList.toggle('border');

                        nrcBase.downloadCanvas(canvas, "identicon.png");
                    }
                    break;
            }
        });
    },

    build: () => {
        clearTimeout(nrPage.defer);
        nrPage.defer = setTimeout(async () => {
            let value = nrVary.domTxtSign.value;
            let size = nrVary.domTxtSize.value * 1 || 320;

            let svg1 = iisvg({ value, size });
            nrVary.domCardView1.innerHTML = '';
            nrVary.domCardView1.classList.add('border');
            nrVary.domCardView1.appendChild(svg1);
            nrVary.domCardView1.parentNode.classList.remove('d-none');

            let svg2 = jdenticon.toSvg(value, size);
            nrVary.domCardView2.innerHTML = svg2;
            nrVary.domCardView2.classList.add('border');
            nrVary.domCardView2.parentNode.classList.remove('d-none');
        }, 200)
    }
}

export { nrPage };
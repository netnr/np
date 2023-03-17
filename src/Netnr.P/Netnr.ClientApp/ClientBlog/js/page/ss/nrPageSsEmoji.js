import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcBase } from "../../../../frame/nrcBase";

let nrPage = {
    pathname: "/ss/emoji",

    init: async () => {
        document.body.addEventListener("click", async (event) => {
            let target = event.target;
            if (target.nodeName == "LI") {
                nrcBase.clipboard(target.innerText);
                nrApp.toast('Copy successfully !')
            }
        });
    },
}

export { nrPage };
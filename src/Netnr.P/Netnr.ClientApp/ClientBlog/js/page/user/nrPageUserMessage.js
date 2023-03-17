import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: '/user/message',

    init: async () => {
        await nrcRely.remote("netnrmd.css");
    },

}

export { nrPage };
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: '/user/id/*',

    init: async () => {
        if (nrVary.domHidIsme.value == "1") {
            nrVary.domBtnEditSay.classList.remove('d-none');

            nrPage.bindEvent();
        }
    },

    bindEvent: () => {
        //编辑 say
        nrVary.domBtnEditSay.addEventListener('click', function () {
            nrVary.domCardSay1.classList.add('d-none');
            nrVary.domCardSay2.classList.remove('d-none');

            nrVary.domTxtSay.value = nrVary.domCardSay1.innerText;
        })

        //保存 say
        nrVary.domBtnSaveSay.addEventListener('click', async function () {
            let fd = new FormData();
            fd.append('UserSay', nrVary.domTxtSay.value);

            nrApp.setLoading(nrVary.domBtnSaveSay);
            let result = await nrWeb.reqServer('/User/UpdateUserSay', { method: "POST", body: fd });
            nrApp.setLoading(nrVary.domBtnSaveSay, true);

            if (result.code == 200) {
                nrVary.domCardSay1.innerText = nrVary.domTxtSay.value;
                nrVary.domCardSay1.classList.remove('d-none');
                nrVary.domCardSay2.classList.add('d-none');
            } else {
                nrApp.alert(result.msg);
            }
        })

        //取消 say
        nrVary.domBtnCancelSay.addEventListener('click', function () {
            nrVary.domCardSay1.classList.remove('d-none');
            nrVary.domCardSay2.classList.add('d-none');
        })

        //编辑 avatar
        nrVary.domImgAvatar.addEventListener('click', function () {
            nrPage.viewModalAvatar();
        })

        //获取 avatar
        nrVary.domBtnGetAvatar.addEventListener('click', async function () {
            if (nrVary.domTxtEmail.value.trim() == "") {
                nrApp.toast("请输入邮箱");
            } else {
                nrApp.setLoading(nrVary.domBtnGetAvatar);

                let img = new Image();
                img.onload = function () {
                    nrVary.domBtnSaveAvatar.disabled = false;
                    nrApp.setLoading(nrVary.domBtnGetAvatar, true);

                    nrVary.domImgPreviewAvatar.src = img.src;
                }
                img.onerror = function () {
                    nrVary.domBtnSaveAvatar.disabled = false;
                    nrApp.setLoading(nrVary.domBtnGetAvatar, true);

                    nrApp.toast("获取头像失败");
                }

                await nrcRely.remote('md5.js');
                img.src = nrVary.domHidGravatarUrl.value + md5(nrVary.domTxtEmail.value.trim()) + "?s=400";
            }
        })

        //保存 avatar
        nrVary.domBtnSaveAvatar.addEventListener('click', async function () {
            let fd = new FormData();
            fd.append('type', 'link');
            fd.append('source', nrVary.domImgPreviewAvatar.src);

            nrApp.setLoading(nrVary.domBtnSaveAvatar);
            let result = await nrWeb.reqServer('/User/UpdateUserAvatar', { method: "POST", body: fd });
            nrApp.setLoading(nrVary.domBtnSaveAvatar, true);

            if (result.code == 200) {
                nrVary.domImgAvatar.src = nrVary.domImgPreviewAvatar.src;
                nrPage.modalAvatar.hide();
            } else {
                nrApp.alert(result.msg);
            }
        })
    },

    viewModalAvatar: () => {
        if (nrPage.modalAvatar == null) {
            nrPage.modalAvatar = new bootstrap.Modal(nrVary.domModalAvatar, { keyboard: false, });
        }
        nrPage.modalAvatar.show();
    }
}

export { nrPage };
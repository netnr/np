import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: '/user/setting',

    init: async () => {
        nrPage.bindEvent();
    },

    bindEvent: () => {
        //保存个人信息
        nrVary.domFormInfo.addEventListener('submit', async function (e) {
            e.preventDefault();

            nrApp.setLoading(nrVary.domBtnSaveInfo);
            let fd = nrcBase.fromFormToFormData(this);
            let result = await nrWeb.reqServer('/User/SaveUserInfo', { method: "POST", body: fd });
            nrApp.setLoading(nrVary.domBtnSaveInfo, true);

            if (result.code == 200) {
                nrApp.toast('修改成功');
                await nrcBase.sleep(1000);
                location.reload(false)
            } else {
                nrApp.alert(result.msg);
            }
        });

        //修改密码
        nrVary.domFormPwd.addEventListener('submit', async function (e) {
            e.preventDefault();

            let fd = nrcBase.fromFormToFormData(this);
            if (fd.get("newPassword") != fd.get("newPassword2")) {
                nrApp.alert('两次输入的密码不一致')
            } else {
                nrApp.setLoading(nrVary.domBtnSavePwd);
                let result = await nrWeb.reqServer('/User/UpdatePassword', { method: "POST", body: fd })
                nrApp.setLoading(nrVary.domBtnSavePwd, true);

                if (result.code == 200) {
                    await nrcBase.sleep(2000);
                    location.href = "/account/login";
                } else {
                    nrApp.alert(result.msg);
                }
            }
        });
    },
}

export { nrPage };
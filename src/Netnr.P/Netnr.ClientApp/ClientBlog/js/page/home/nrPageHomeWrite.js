import { nrcBase } from "../../../../frame/nrcBase";
import { nrWeb } from "../../nrWeb";
import { nrVary } from "../../nrVary";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";
import { nrcRely } from "../../../../frame/nrcRely";

let nrPage = {
    pathname: '/home/write',

    init: async () => {
        //标签
        let result = await nrWeb.reqServer('/Home/TagSelect');
        if (result.code == 200 && result.data) {
            await nrcRely.remote('choices');

            nrVary.domSeTags.multiple = true;
            nrVary.tag1 = new Choices(nrVary.domSeTags, {
                allowHTML: true,
                maxItemCount: 3,
                removeItemButton: true,
                noResultsText: '咣',
                noChoicesText: '（空）',
                itemSelectText: '点击选择',
                maxItemText: (maxItemCount) => `最多选择 ${maxItemCount} 项`,
            });
            nrVary.tag1.dropdown.element.classList.add('rounded');
            nrVary.tag1.containerInner.element.classList.add('rounded');
            nrVary.tag1.setChoices(() => {
                return result.data.map(item => ({ value: item.TagId, label: item.TagName }))
            })
        }

        //markdown 编辑器
        await nrcRely.remote("netnrmdEditor");
        await nrcRely.remote("netnrmd");
        nrApp.tsMd = netnrmd.init('.nrg-editor', {
            theme: nrcBase.isDark() ? "dark" : "light"
        });

        //快捷键
        nrApp.tsMd.addCommand("Ctrl+S", () => nrVary.domBtnSave.click());

        //保存
        nrVary.domBtnSave.addEventListener("click", async function () {
            let obj = {
                UwId: 0,
                UwTitle: nrVary.domTxtTitle.value,
                UwCategory: 0,
                UwContent: nrApp.tsMd.gethtml(),
                UwContentMd: nrApp.tsMd.getmd(),
                TagIds: nrVary.tag1.getValue(true).join()
            }

            let errMsg = [];
            if (obj.UwTitle == "") {
                errMsg.push("请输入 标题");
            }
            if (obj.TagIds == "") {
                errMsg.push("请选择 标签");
            }
            if (obj.UwContentMd.length < 20) {
                errMsg.push("多写一点内容哦");
            }

            if (errMsg.length > 0) {
                nrApp.alert(errMsg.join('<hr/>'));
            } else {
                nrApp.setLoading(nrVary.domBtnSave);

                let fd = nrcBase.fromKeyToFormData(obj);

                let result = await nrWeb.reqServer('/Home/WriteSave', { method: 'POST', body: fd });
                nrApp.setLoading(nrVary.domBtnSave, true);

                if (result.code == 200) {
                    nrApp.toast('保存成功');
                    nrApp.tsMd.setmd('');
                    location.href = `/home/list/${result.data}`;
                } else {
                    nrApp.alert(result.msg);
                }
            }
        });
    },

    event_resize: (ch) => {
        if (nrApp.tsMd) {
            let vh = ch - nrApp.tsMd.domEditor.getBoundingClientRect().top - 40;
            nrApp.tsMd.height(Math.max(200, vh));
        }
    }
}

export { nrPage };
import { nrEditor } from "../../../../frame/nrEditor";
import { nrcFile } from "../../../../frame/nrcFile";
import { nrVary } from "../../nrVary";
import { nrcBase } from "../../../../frame/nrcBase";
import { nrApp } from "../../../../frame/Bootstrap/nrApp";

let nrPage = {
    pathname: "/ss/diff",

    init: async () => {
        //隐藏菜单
        if (nrPage.isHideMenu) {
            document.querySelector('nav').classList.add('d-none');
        }

        //编辑器
        nrVary.domEditor.innerHTML = nrApp.tsLoadingHtml;
        await nrEditor.init();
        nrVary.domEditor.innerHTML = '';

        nrApp.tsEditor = monaco.editor.createDiffEditor(nrVary.domEditor, nrEditor.config({
            originalEditable: true
        }));
        nrApp.tsEditor.setModel({
            original: monaco.editor.createModel("just some text\n\nHello World\n\nSome more text", "plaintext"),
            modified: monaco.editor.createModel("just some Text\n\nHello World\n\nSome more changes", "plaintext")
        });

        nrVary.domEditor.classList.add('border');
        nrcBase.setHeightFromBottom(nrVary.domEditor);

        nrPage.bindEvent();
    },

    isHideMenu: location.hash == "#hide-menu",

    bindEvent: () => {
        //显示模式
        nrVary.domSeDiff.addEventListener('input', function () {
            nrApp.tsEditor.updateOptions({
                renderSideBySide: this.value != 1
            });
        });

        nrVary.domBtnNext.addEventListener('click', function () {
            if (nrApp.tsEditor) {
                nrApp.tsEditor.accessibleDiffViewerNext()
            }
        });

        //通信
        window.addEventListener("message", function (event) {
            try {
                let jsonData = JSON.parse(event.data);
                if (jsonData.origin != null && jsonData.vary != null) {
                    nrApp.tsEditor.getOriginalEditor().setValue(jsonData.origin);
                    nrApp.tsEditor.getModifiedEditor().setValue(jsonData.vary);
                }
            } catch (ex) {
                console.error(ex);
            }
        });

        //接收文件
        nrcFile.init((files) => {
            nrApp.alert(`<div class="text-center">
<button class="btn btn-lg mx-2 btn-primary" data-action="left">左边 Left</button>
<button class="btn btn-lg mx-2 btn-danger" data-action="right">右边 Right</button></div>`);

            let writeEditor = async function (event) {
                let action = event.target.dataset.action;
                if (["left", "right"].includes(action)) {
                    let text = await nrcFile.reader(files[0]);
                    if (action == "left") {
                        nrApp.tsEditor.getOriginalEditor().setValue(text);
                    } else {
                        nrApp.tsEditor.getModifiedEditor().setValue(text);
                    }
                    nrApp.domAlert["alert"].hide();
                    nrApp.domAlert.removeEventListener('click', writeEditor);
                }
            }
            nrApp.domAlert.addEventListener('click', writeEditor)

        }, nrVary.domTxtFile);
    },
}

export { nrPage };
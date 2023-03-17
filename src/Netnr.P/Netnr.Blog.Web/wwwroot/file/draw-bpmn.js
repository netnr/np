var rowId = location.pathname.split('/').pop();

var page = {
    domTxtTitle: document.querySelector(".nrg-txt-title"),
    init: function () {
        page.domBPMN = document.createElement("div");
        page.domBPMN.classList.add("nrg-bpmn");
        document.body.appendChild(page.domBPMN);

        page.bpmnModeler = new BpmnJS({
            container: page.domBPMN,
            keyboard: {
                bindTo: window
            }
        });

        page.load();

        //Ctrl+S
        document.addEventListener('keydown', function (e) {
            if (e.keyCode == 83 && e.ctrlKey) {
                page.save();

                e.preventDefault();
                e.stopPropagation();
            }
        })

        //事件
        document.querySelector('.bjs-tool').addEventListener('click', function (e) {
            var target = e.target, action = target.getAttribute('data-action');
            switch (action) {
                case "save":
                    {
                        page.save();
                    }
                    break;
                case "import":
                    {
                        page.import(prompt("bpmnXML"));
                    }
                    break;
                case "export":
                    {
                        page.export().then(xml => {
                            prompt("export", xml)
                        })
                    }
                    break;
                case "svg":
                    {
                        var canvas = page.bpmnModeler.get('canvas');
                        console.debug(canvas._svg)
                        prompt("export", canvas._svg.outerHTML)
                    }
                    break;
            }
        });

        document.getElementById('LoadingMask').style.display = "none";
    },
    load: () => {
        //新建
        page.bpmnModeler.createDiagram();

        fetch(`/draw/EditorOpen/${rowId}`).then(resp => resp.json()).then(result => {
            if (result.code == 200) {
                page.domTxtTitle.value = result.data.DrName;
                document.title = result.data.DrName + " - " + document.title;
                page.import(result.data.DrContent);
            } else {
                alert(result.msg);
            }
        })
    },
    save: () => {
        if (page.RequestActive == null) {
            page.RequestActive = "save";
            var title = page.domTxtTitle.value;
            if (title.trim() == "") {
                alert("请输入标题");
                return;
            }

            page.export().then(xml => {

                var fd = new FormData();
                fd.append("DrType", "bpmn");
                fd.append("filename", title);
                fd.append("xml", xml);

                fetch(`/draw/EditorSave/${rowId}`, {
                    method: 'POST',
                    body: fd
                }).then(resp => resp.json()).then(result => {
                    page.RequestActive = null;
                    if (result.code == 200 && result.data) {
                        location.href = "/draw/code/" + result.data;
                    } else {
                        alert(result.msg);
                    }
                }).catch(ex => {
                    console.debug(ex);
                    page.RequestActive = null;
                })
            })
        }
    },
    import: (bpmnXML) => {
        if (bpmnXML != null && bpmnXML != "") {
            try {
                page.bpmnModeler.importXML(bpmnXML).then(() => {
                    var canvas = page.bpmnModeler.get('canvas');

                    // zoom to fit full viewport
                    canvas.zoom('fit-viewport');
                })
            } catch (err) {
                console.error(err);
                alert('import error');
            }
        }
    },
    export: () => new Promise((resolve) => {
        page.bpmnModeler.saveXML({ format: true }).then(res => resolve(res.xml))
    })
};

page.init();
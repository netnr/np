var page = {
    vid: document.getElementById("vid").value,
    domTxtTitle: document.querySelector(".nr-txt-title"),
    init: function () {
        page.domBPMN = document.createElement("div");
        page.domBPMN.classList.add("nr-bpmn");
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
        })
    },
    load: () => {
        //新建
        page.bpmnModeler.createDiagram();

        if (page.vid != "") {
            fetch('/draw/code/open/' + page.vid).then(resp => resp.json()).then(res => {
                page.domTxtTitle.value = res.data.DrName;
                document.title = res.data.DrName + " - " + document.title;
                page.import(res.data.DrContent);
            })
        }
    },
    save: () => {
        if (page.vid != "" && page.RequestActive == null) {
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

                fetch('/draw/code/save/' + page.vid, {
                    method: 'POST',
                    body: fd
                }).then(resp => resp.json()).then(res => {
                    page.RequestActive = null;
                    if (res.code == 200 && res.data) {
                        sessionStorage.removeItem(page.storageKey);
                        location.href = "/draw/code/" + res.data;
                    } else {
                        alert(res.msg);
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
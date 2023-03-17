let nrPage = {
    storageKey: "/bpmn/content",
    init: function () {
        nrPage.domBPMN = document.createElement("div");
        nrPage.domBPMN.classList.add("nrg-bpmn");
        document.body.appendChild(nrPage.domBPMN);

        nrPage.bpmnModeler = new BpmnJS({
            container: nrPage.domBPMN,
            keyboard: {
                bindTo: window
            }
        });

        //恢复
        let bpmnXML = localStorage.getItem(nrPage.storageKey);
        if (bpmnXML) {
            nrPage.import(bpmnXML);
        } else {
            //新建
            nrPage.bpmnModeler.createDiagram();
        }

        //变更监听
        nrPage.bpmnModeler.get('eventBus').on('elements.changed', function () {
            clearTimeout(nrPage.defer1);
            nrPage.defer1 = setTimeout(() => {
                nrPage.export().then(xml => {
                    localStorage.setItem(nrPage.storageKey, xml);
                })
            }, 1000);
        });

        //事件
        document.querySelector('.bjs-tool').addEventListener('click', function (e) {
            var target = e.target, action = target.getAttribute('data-action');
            switch (action) {
                case "new":
                    {
                        if (confirm("确定新建？")) {
                            nrPage.bpmnModeler.createDiagram();
                        }
                    }
                    break;
                case "import":
                    {
                        nrPage.import(prompt("bpmnXML"));
                    }
                    break;
                case "export":
                    {
                        nrPage.export().then(xml => {
                            prompt("export", xml)
                        })
                    }
                    break;
                case "svg":
                    {
                        var canvas = nrPage.bpmnModeler.get('canvas');
                        console.debug(canvas._svg)
                        prompt("export", canvas._svg.outerHTML)
                    }
                    break;
            }
        })

        document.getElementById('LoadingMask').style.display = "none";
    },
    import: (bpmnXML) => {
        if (bpmnXML != null && bpmnXML != "") {
            try {
                nrPage.bpmnModeler.importXML(bpmnXML).then(() => {
                    var canvas = nrPage.bpmnModeler.get('canvas');

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
        nrPage.bpmnModeler.saveXML({ format: true }).then(res => resolve(res.xml))
    })
};

nrPage.init();
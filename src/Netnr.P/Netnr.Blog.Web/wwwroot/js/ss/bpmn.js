var page = {
    storageKey: "/bpmn/content",
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

        //恢复
        let bpmnXML = localStorage.getItem(page.storageKey);
        if (bpmnXML) {
            page.import(bpmnXML);
        } else {
            //新建
            page.bpmnModeler.createDiagram();
        }

        //变更监听
        page.bpmnModeler.get('eventBus').on('elements.changed', function () {
            clearTimeout(page.defer1);
            page.defer1 = setTimeout(() => {
                page.export().then(xml => {
                    localStorage.setItem(page.storageKey, xml);
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
                            page.bpmnModeler.createDiagram();
                        }
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
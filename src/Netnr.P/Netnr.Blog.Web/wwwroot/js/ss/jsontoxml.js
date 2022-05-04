nr.onChangeSize = function (ch) {
    var vh = ch - nr.domEditor1.getBoundingClientRect().top - 30;
    nr.domEditor1.style.height = vh + "px";
    nr.domEditor2.style.height = vh + "px";
}

var editor1, editor2;

nr.onReady = function () {
    ss.loading(true);

    me.init().then(() => {

        editor1 = me.create(nr.domEditor1, {
            value: JSON.stringify({
                "site": {
                    "title": "NET牛人",
                    "domain": "https://www.netnr.com",
                    "mirror": "https://www.netnr.eu.org",
                    "createtime": "2014.01.01"
                },
                "about": {
                    "name": "netnr",
                    "sex": "男",
                    "injob": "2012.03.01",
                    "live": "中国重庆",
                    "mail": "netnr@netnr.com",
                    "git": [
                        {
                            "name": "github",
                            "url": "https://github.com/netnr"
                        },
                        {
                            "name": "gitee",
                            "url": "https://gitee.com/netnr"
                        }
                    ]
                },
                "updaet": "2022.04.21",
                "version": "v.1.0.0"
            }, null, 2),
            language: 'json',
            scrollBeyondLastLine: false
        });

        editor2 = me.create(nr.domEditor2, {
            value: "",
            language: 'xml',
            scrollBeyondLastLine: false
        });

        nr.domEditor1.classList.add('border');
        nr.domEditor2.classList.add('border');
        nr.domCardBox.classList.remove('invisible');
        ss.loading(false);

        //接收文件
        nr.receiveFiles(function (files) {
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function (e) {
                me.keepSetValue(editor1, e.target.result);
            };
            reader.readAsText(file);
        });

        //json to xml
        nr.domBtnBuild1.addEventListener('click', function () {
            var jsonText = editor1.getValue().trim();
            if (jsonText == "") {
                nr.alert('JSON 不能为空');
            } else {
                try {
                    var jsonObj = JSON.parse(jsonText);
                    var xml = new FastXmlParser.XMLBuilder().build(jsonObj);
                    me.keepSetValue(editor2, xml);
                    me.formatter(editor2);
                } catch (ex) {
                    console.debug(ex);
                    nr.alert('转换错误');
                }
            }
        });

        //xml to json
        nr.domBtnBuild2.addEventListener('click', function () {
            var xmlText = editor2.getValue().trim();
            if (xmlText == "") {
                nr.alert('XML 不能为空');
            } else {
                try {
                    var jsonObj = new FastXmlParser.XMLParser().parse(xmlText);
                    var json = JSON.stringify(jsonObj, null, 2);
                    me.keepSetValue(editor1, json);
                } catch (ex) {
                    console.debug(ex);
                    nr.alert('转换错误');
                }
            }
        });
    })
}
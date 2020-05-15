var gd1 = z.Grid();
gd1.url = "/Admin/QueryLog";
gd1.pageSize = 100;
gd1.sortName = "LogCreateTime";
gd1.sortOrder = "desc";
gd1.columns = [[
    { title: "账号", field: "LogUid", width: 160 },
    { title: "昵称", field: "LogNickname", width: 120 },
    {
        title: "动作", field: "LogAction", width: 150, formatter: function (value) {
            try {
                return decodeURIComponent(value)
            } catch (e) {
                return value;
            }
        }
    },
    { title: "内容", field: "LogContent", width: 150 },
    {
        title: "链接", field: "LogUrl", width: 200, formatter: function (value) {
            try {
                return decodeURIComponent(value)
            } catch (e) {
                return value;
            }
        }
    },
    { title: "IP", field: "LogIp", width: 160 },
    { title: "归属地", field: "LogArea", width: 240 },
    { title: "引荐", field: "LogReferer", width: 300 },
    {
        title: "时间", field: "LogCreateTime", width: 200, formatter: function (value) {
            value = new Date((value - 621355968000000000) / 10000).toISOString().replace("T", " ").replace("Z", "");
            return value;
        }
    },
    { title: "浏览器", field: "LogBrowserName", width: 180 },
    { title: "操作系统", field: "LogSystemName", width: 120 },
    { title: "UA", field: "LogUserAgent", width: 180 },
    {
        title: "组", field: "LogGroup", width: 60, formatter: function (value) {
            switch (value) {
                case "1":
                    value = '用户';
                    break;
                case "2":
                    value = '爬虫';
                    break;
            }
            return value;
        }
    },
    {
        title: "级别", field: "LogLevel", width: 60, formatter: function (value) {
            switch (value) {
                case "F": value = "Fatal"; break;
                case "E": value = "Error"; break;
                case "W": value = "Warn"; break;
                case "I": value = "Info"; break;
                case "D": value = "Debug"; break;
                case "A": value = "All"; break;
            }
            return value;
        }
    },
    { title: "备注", field: "LogRemark", width: 100 }
]];
gd1.onDblClickRow = function () {
    var msg = JSON.stringify(gd1.func("getSelected"), null, 4);
    $.each(gd1.columns[0], function () {
        msg = msg.replace('"' + this.field + '"', '"' + this.title + '"')
    });

    jz.popup({
        title: "查看明细",
        content: "<pre class='h6' style='white-space:pre-wrap'>" + msg + "</pre>",
        drag: 1,
        footer: false,
        blank: 1,
        mask: .5
    });
}

gd1.load();
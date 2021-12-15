var ndkVary = {

    version: '0.1.0',
    theme: "auto", //主题 可选 auto、dark、light
    themeGet: () => {
        if (ndkVary.theme == "auto") {
            return window.matchMedia('(prefers-color-scheme: dark)').matches ? "dark" : "light";
        }
        return ndkVary.theme;
    },
    parameterConfig: {
        autoFilterDatabaseNumber: {
            "zh-CN": "数据库名超过自动过滤",
            "en-US": "Auto filter database number",
            type: "number",
            value: 40
        },
        selectDataLimit: {
            "zh-CN": "查询数据默认限制行数",
            "en-US": "Select data default limit",
            type: "number",
            value: 100
        },
        buildSqlWithQuote: {
            "zh-CN": "生成 SQL 带符号",
            "en-US": "Build SQL with quote",
            type: "boolean",
            list: [
                {
                    "zh-CN": "带符号",
                    "en-US": "With quote",
                    val: true
                },
                {
                    "zh-CN": "不带",
                    "en-US": "Without quote",
                    val: false
                }
            ],
            value: true
        },
        buildSqlWithComment: {
            "zh-CN": "生成 SQL 带注释",
            "en-US": "Build SQL with comment",
            type: "boolean",
            list: [
                {
                    "zh-CN": "带注释",
                    "en-US": "With comment",
                    val: true
                },
                {
                    "zh-CN": "不带",
                    "en-US": "Without comment",
                    val: false
                }
            ],
            value: true
        },
        dataSqlBulkInsert: {
            "zh-CN": "数据 SQL 批量插入",
            "en-US": "Data SQL bulk insert",
            type: "number",
            value: 10
        },
        dataSqlWithAutoIncrement: {
            "zh-CN": "数据 SQL 带自增列",
            "en-US": "Data SQL with auto increment",
            type: "boolean",
            list: [
                {
                    "zh-CN": "带自增列",
                    "en-US": "With auto increment",
                    val: true
                },
                {
                    "zh-CN": "不带自增列",
                    "en-US": "Without auto increment",
                    val: false
                }
            ],
            value: true
        },
        editorFontSize: {
            "zh-CN": "编辑器字体大小",
            "en-US": "Editor font size",
            type: "number",
            value: 18
        },
        editorLineNumbers: {
            "zh-CN": "编辑器行号",
            "en-US": "Editor line numbers",
            type: "select",
            list: [
                {
                    "zh-CN": "显示",
                    "en-US": "Show",
                    val: 'on'
                },
                {
                    "zh-CN": "不显示",
                    "en-US": "Hide",
                    val: 'off'
                }
            ],
            value: 'on'
        },
        editorWordWrap: {
            "zh-CN": "编辑器换行",
            "en-US": "Editor word wrap",
            type: "select",
            list: [
                {
                    "zh-CN": "自动换行",
                    "en-US": "Auto wrap",
                    val: 'on'
                },
                {
                    "zh-CN": "不换行",
                    "en-US": "No wrap",
                    val: 'off'
                }
            ],
            value: 'off'
        },
        gridDataShowLength: {
            "zh-CN": "表格数据显示截断",
            "en-US": "Grid data show length",
            type: "number",
            value: 200
        }
    },

    //数据库类型
    typeDB: ["SQLite", "MySQL", "MariaDB", "Oracle", "SQLServer", "PostgreSQL"],
    iconDB: type => ["🖤", "💚", "🤎", "💗", "🧡", "💙"][ndkVary.typeDB.indexOf(type)], //对应图标

    typeEnv: ["Development", "Test", "Production"], //环境类型
    colorEnv: env => {
        switch (env) {
            case "Test": return 'var(--sl-color-primary-600)';
            case "Production": return 'var(--sl-color-danger-600)';
        }
    },
    iconEnv: env => ["⚫", "🔵", "🔴"][ndkVary.typeEnv.indexOf(env)], //环境图标

    /**
     * svg 图标
     * @param {*} name 
     * @param {*} style 
     * @returns 
     */
    iconSvg: (name, style) => `<svg class="${style || ""}" aria-hidden="true"><use xlink:href="#${name.toLowerCase()}"></use></svg>`,

    icons: {
        menu: "Ⓜ",
        quick: "📌",
        id: "🆔",
        connType: "💞",
        connOrder: "🚩",
        connGroup: "👪",
        connEnv: "⚫",
        connConn: "🔗",
        connDatabase: "🎒",
        connTable: "📚",
        connColumn: "📙",
        loading: "🔄",
        ok: "🆗",
        generate: "🎲",
        ctrl: "🔧",
        copy: "👯",
        name: "📛",
        key: "🔑",
        edit: "✍",
        incr: "➚",
        comment: "📝",
        add: "➕",
        remove: "❌",
        success: "✔",
        info: "🔔",
        data: "🧮",
        cog: "⚙",
        parameter: "🛠",
        server: "🌎",
        io: "♻",
        clipboard: "📋",
        save: "💾",
        full: "💯",
        date: "🕓",
        cut: "✂",
        loading: "🛑",
    },
    /**
     * 获取 icon
     * @param {any} icon
     */
    iconGrid: icon => `<span class="ag-icon ag-icon-${icon}"></span>`,

    //连接模板
    resConnTemplate: {
        SQLite: "Data Source=<文件物理路径，或网络路径，后台自动下载文件>",
        MySQL: "Server=<IP>;Port=3306;Uid=<USER>;Pwd=<PWD>;Database=<DBNAME>;",
        MariaDB: "Server=<IP>;Port=3306;Uid=<USER>;Pwd=<PWD>;Database=<DBNAME>;",
        Oracle: "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<IP>)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=<USER>;Password=<PWD>;",
        SQLServer: "Server=<IP>,1433;User Id=<USER>;Password=<PWD>;Database=<DBNAME>;TrustServerCertificate=True;",
        PostgreSQL: "Server=<IP>;Port=5432;User Id=<USER>;Password=<PWD>;Database=<DBNAME>;"
    },
    //连接示例
    resConnDemo: [
        { id: 10001, type: "SQLite", alias: "SQLite:netnrf", group: "demo", order: 1, env: "Test", conn: "Data Source=https://s1.netnr.eu.org/2020/05/22/2037505934.db" },
        { id: 10002, type: "MySQL", alias: "Heroku-JawsDB:ustf345c1n0wkaow", group: "demo", order: 2, env: "Test", conn: "Server=c8u4r7fp8i8qaniw.chr7pe7iynqr.eu-west-1.rds.amazonaws.com;Port=3306;Uid=fyxnmvubyl01t2k9;Pwd=ai7a4eg3c31scfcm;Database=ustf345c1n0wkaow;" },
        { id: 10003, type: "MariaDB", alias: "Heroku-JawsDB:gvx25hgtxzfr2lia", group: "demo", order: 3, env: "Test", conn: "Server=eporqep6b4b8ql12.chr7pe7iynqr.eu-west-1.rds.amazonaws.com;Port=3306;Uid=hydfd5qr08d3akt9;Pwd=tk53sieop5ua97pv;Database=gvx25hgtxzfr2lia;" },
        { id: 10004, type: "SQLServer", alias: "SOMEE-mssql:netnr-kit", group: "demo", order: 5, env: "Test", conn: "Server=198.37.116.112,1433;User Id=netnr_SQLLogin_1;Password=o2y9vrbjac;Database=netnr-kit;TrustServerCertificate=True;" },
        { id: 10005, type: "PostgreSQL", alias: "Heroku-pgsql:d7mhfq80unm96q", group: "demo", order: 6, env: "Test", conn: "Server=ec2-54-74-35-87.eu-west-1.compute.amazonaws.com;Port=5432;User Id=psphnovbbmsgtj;Password=7554b25380195aa5755a24c7f6e1f9f94f3de3dcef9c345c7e93ae8b07699ace;Database=d7mhfq80unm96q;SslMode=Require;Trust Server Certificate=true;" }
    ],
    //服务
    resServer: [
        { host: "https://www.netnr.eu.org/api/v1", remark: "基于 Heroku 构建" },
        { host: location.origin, remark: "当前" }
    ],

    apiServer: location.origin, //接口服务
    apiHeaders: null, //接口头部参数（如：{ Authorization: "token" }）

    apiGetDatabaseName: "/DK/GetDatabaseName",
    apiGetDatabaseInfo: "/DK/GetDatabase",
    apiGetTable: "/DK/GetTable",
    apiGetColumn: "/DK/GetColumn",
    apiSetTableComment: "/DK/SetTableComment",
    apiSetColumnComment: "/DK/SetColumnComment",
    apiExecuteSql: "/DK/ExecuteSql",
    apiGetData: "/DK/GetData",
    apiGetDEI: "/DK/GetDEI",

    envConnsChanged: false, //连接变化
};

//参数默认值
for (const key in ndkVary.parameterConfig) {
    ndkVary.parameterConfig[key].defaultValue = ndkVary.parameterConfig[key].value;
}

export { ndkVary }
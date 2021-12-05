var ndkVary = {

    version: '0.1.0',
    theme: "light", //主题 可选 dark
    parameterConfig: {
        autoFilterDatabaseNumber: {
            label: "数据库名超过自动过滤",
            type: "number",
            value: 40
        },
        selectDataLimit: {
            label: "查询数据默认限制行数",
            type: "number",
            value: 200
        },
        buildSqlWithQuote: {
            label: "生成 SQL 带符号",
            type: "boolean",
            list: [
                { txt: "带符号", val: true },
                { txt: "不带", val: false }
            ],
            value: true
        },
        buildSqlWithComment: {
            label: "生成 SQL 带注释",
            type: "boolean",
            list: [
                { txt: "带注释", val: true },
                { txt: "不带", val: false }
            ],
            value: true
        },
        editorFontSize: {
            label: "编辑器字体大小",
            type: "number",
            value: 18
        },
        editorLineNumbers: {
            label: "编辑器行号",
            type: "select",
            list: [
                { txt: "显示", val: 'on' },
                { txt: "不显示", val: 'off' }
            ],
            value: 'on'
        },
        editorWordWrap: {
            label: "编辑器换行",
            type: "select",
            list: [
                { txt: "自动换行", val: 'on' },
                { txt: "不换行", val: 'off' }
            ],
            value: 'on'
        },
        gridDataShowLength: {
            label: "表格数据显示截断",
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
        clipboard: "📋",
        date: "🕓",
        cut: "✂",
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
};

export { ndkVary }
import { ndkI18n } from "./ndkI18n";

// 变量
var ndkVary = {
    version: "1.1.3", // 版本 2022-09
    name: "NDK (Netnr.DataKit)",

    theme: "auto", //主题 可选 auto、dark、light
    themeGet: () => {
        if (ndkVary.theme == "auto") {
            return window.matchMedia('(prefers-color-scheme: dark)').matches ? "dark" : "light";
        }
        return ndkVary.theme;
    },
    // 主题
    resTheme: [
        { key: 'auto', icon: 'square-half' }, // 默认值，保持首位，其它地方有引用
        { key: 'dark', icon: 'moon-fill' },
        { key: 'light', icon: 'sun' },
    ],

    //数据库类型
    typeDB: ["SQLite", "MySQL", "MariaDB", "Oracle", "SQLServer", "PostgreSQL"],

    typeEnv: ["dev", "prod"], //环境类型
    colorEnv: env => `var(--sl-color-${env == "prod" ? "danger" : "primary"}-600)`,

    /**
     * SVG 图标
     * @param {*} iconName 图标名
     * @param {*} content 内容
     * @param {*} options
     * @returns 
     */
    iconSvg: (iconName, content, options) => {
        content = content ?? '';

        options = Object.assign({
            className: "",
            slot: null,
            library: 'default'
        }, options);
        options.slot = options.slot ? `slot="${options.slot}"` : '';

        return `<sl-icon library="${options.library}" class="${options.className}" name="${iconName.toLowerCase()}" ${options.slot}></sl-icon> ${content}`
    },
    /**
     * 获取 icon
     * @param {any} icon
     */
    iconGrid: icon => `<span class="ag-icon ag-icon-${icon}"></span>`,

    //表情
    emoji: {
        menu: "Ⓜ",
        quick: "📌",
        id: "🆔",
        loading: "🛑",
        comment: "📝",
        ok: "🆗",
        key: "🔑",
        incr: "👆",
        star: "⭐",
        remove: "❌",
        success: "✔",
        cog: "⚙",
        parameter: "🛠",
        server: "🌎",
        io: "♻",
        clipboard: "📋",
        save: "💾",
        cut: "✂",
        see: "👁",
        wait: "⏳",
        package: "📦",
        link: "🔗",
        bucket: "🛢"
    },

    //连接模板
    resConnTemplate: {
        SQLite: "Data Source={文件物理路径，或网络路径，后台自动下载文件}",
        MySQL: "Server={IP};Port=3306;Uid={USER};Pwd={PWD};Database={DBNAME};",
        MariaDB: "Server={IP};Port=3306;Uid={USER};Pwd={PWD};Database={DBNAME};",
        Oracle: "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={IP})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id={USER};Password={PWD};",
        SQLServer: "Server={IP},1433;User Id={USER};Password={PWD};Database={DBNAME};TrustServerCertificate=True;",
        PostgreSQL: "Server={IP};Port=5432;User Id={USER};Password={PWD};Database={DBNAME};"
    },
    //连接示例
    resConnDemo: [
        { id: 10001, type: "SQLite", alias: "netnrf", group: "demo", order: 1, env: "dev", conn: "Data Source=https://gs.zme.ink/2020/05/22/2037505934.db" },
        { id: 10002, type: "MySQL", alias: "jjrrk4fMaf", group: "demo", order: 2, env: "dev", conn: "Server=remotemysql.com;Port=3306;Uid=jjrrk4fMaf;Pwd=Fb7Kx8ICY6;Database=jjrrk4fMaf;" },
        { id: 10005, type: "PostgreSQL", alias: "eiueluhc", group: "demo", order: 3, env: "dev", conn: "Server=satao.db.elephantsql.com;Port=5432;User Id=eiueluhc;Password=IbiZfVBcqLilS58RkaWNDG6j007Td0ml;Database=eiueluhc;SslMode=Require;Trust Server Certificate=true;" }
    ],
    //参数配置
    parameterConfig: {
        autoFilterDatabaseNumber: { type: "number", value: 40 },
        selectDataLimit: { type: "number", value: 100 },
        buildSqlWithQuote: {
            type: "boolean",
            list: [{ val: true }, { val: false }],
            value: true
        },
        buildSqlWithComment: {
            type: "boolean",
            list: [{ val: true }, { val: false }],
            value: true
        },
        dataSqlBulkInsert: { type: "number", value: 10 },
        dataSqlWithAutoIncrement: {
            type: "boolean",
            list: [{ val: true }, { val: false }],
            value: true
        },
        editorFontSize: { type: "number", value: 18 },
        editorLineNumbers: {
            type: "select",
            list: [{ val: 'on' }, { val: 'off' }],
            value: 'on'
        },
        editorWordWrap: {
            type: "select",
            list: [{ val: 'on' }, { val: 'off' }],
            value: 'off'
        },
        gridDataShowLength: { type: "number", value: 200 },
        maxSqlHistory: { type: "number", value: 5000 },
        readFileEncoding: {
            type: "select",
            list: [{ val: 'GBK' }, { val: 'utf-8' }],
            value: 'GBK'
        },
    },
    //接口服务列表
    resApiServer: [
        { key: location.origin, remark: ndkI18n.lg.current },
        { key: "https://netnr.zme.ink/api/v1", remark: "online" }
    ],

    apiServer: location.origin, //指定接口服务

    apiServiceStatus: "/DK/ServiceStatus", // 服务状态检测 200
    apiGetDatabaseName: "/DK/GetDatabaseName",
    apiGetDatabaseInfo: "/DK/GetDatabase",
    apiGetTable: "/DK/GetTable",
    apiGetColumn: "/DK/GetColumn",
    apiSetTableComment: "/DK/SetTableComment",
    apiSetColumnComment: "/DK/SetColumnComment",
    apiGetTableDDL: "/DK/GetTableDDL",
    apiExecuteSql: "/DK/ExecuteSql",

    defer: {}, //延迟对象
    envConnsChanged: false, //连接变化

    fileObject: null, //文件对象
    fileContent: null, //文件内容
    pasteContent: null, //粘贴内容
};

//参数默认值
for (const key in ndkVary.parameterConfig) {
    ndkVary.parameterConfig[key].defaultValue = ndkVary.parameterConfig[key].value;
}

export { ndkVary }
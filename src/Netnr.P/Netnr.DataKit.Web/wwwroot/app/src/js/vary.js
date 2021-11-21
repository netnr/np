var vary = {

    version: '0.1.0',
    theme: "light", //主题 可选 dark
    config: {
        autoFilterDatabaseNumber: 40, //数据库名超过自动过滤
        selectDataLimit: 200, //查询数据默认限制行数
    },

    //数据库类型
    typeDB: ["SQLite", "MySQL", "MariaDB", "Oracle", "SQLServer", "PostgreSQL"],
    iconDB: type => ["🖤", "💚", "🤎", "💗", "🧡", "💙"][vary.typeDB.indexOf(type)], //对应图标
    typeEnv: ["Development", "Test", "Production"], //环境类型
    iconEnv: env => ["⚪", "🔵", "🔴"][vary.typeEnv.indexOf(env)], //环境图标
    icons: {
        id: "🆔",
        connType: "💞",
        connOrder: "🚩",
        connGroup: "👪",
        connEnv: "⚪",
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
        info: "🔔",
        data: "🧮",
        cog: "⚙",
        clipboard:"📋"
    },
    /**
     * 获取 icon
     * @param {any} icon
     */
    iconGrid: function (icon) { return `<span class="ag-icon ag-icon-${icon}"></span>` },

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
        { id: 10002, type: "MySQL", alias: "Heroku JawsDB:ustf345c1n0wkaow", group: "demo", order: 2, env: "Test", conn: "Server=c8u4r7fp8i8qaniw.chr7pe7iynqr.eu-west-1.rds.amazonaws.com;Port=3306;Uid=fyxnmvubyl01t2k9;Pwd=ai7a4eg3c31scfcm;Database=ustf345c1n0wkaow;" },
        { id: 10003, type: "MariaDB", alias: "Heroku JawsDB:gvx25hgtxzfr2lia", group: "demo", order: 3, env: "Test", conn: "Server=eporqep6b4b8ql12.chr7pe7iynqr.eu-west-1.rds.amazonaws.com;Port=3306;Uid=hydfd5qr08d3akt9;Pwd=tk53sieop5ua97pv;Database=gvx25hgtxzfr2lia;" },
        { id: 10004, type: "SQLServer", alias: "SOMEE MSSQL:netnr-kit", group: "demo", order: 5, env: "Test", conn: "Server=198.37.116.112,1433;User Id=netnr_SQLLogin_1;Password=o2y9vrbjac;Database=netnr-kit;TrustServerCertificate=True;" },
        { id: 10005, type: "PostgreSQL", alias: "Heroku PostgreSQL:d7mhfq80unm96q", group: "demo", order: 6, env: "Test", conn: "Server=ec2-54-74-35-87.eu-west-1.compute.amazonaws.com;Port=5432;User Id=psphnovbbmsgtj;Password=7554b25380195aa5755a24c7f6e1f9f94f3de3dcef9c345c7e93ae8b07699ace;Database=d7mhfq80unm96q;SslMode=Require;Trust Server Certificate=true;" }
    ],
    //服务
    resServer: [
        { host: "https://www.netnr.eu.org/api/v1", remark: "基于 Heroku 构建" },
        { host: location.origin, remark: "当前" }
    ],

    _apiServer: location.origin,
    apiServer: "https://localhost:5001", //连接服务
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

window.addEventListener("DOMContentLoaded", function () {

    //dom对象
    document.querySelectorAll('*').forEach(node => {
        if (node.classList.value.startsWith('nr-')) {
            var vkey = 'dom';
            node.classList[0].substring(3).split('-').forEach(c => vkey += c.substring(0, 1).toUpperCase() + c.substring(1))
            vary[vkey] = node;
        }
    })

}, false);

export { vary }
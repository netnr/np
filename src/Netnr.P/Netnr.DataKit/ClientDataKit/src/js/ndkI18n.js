// 语言切换
var ndkI18n = {
    language: "auto", // 语言
    // 语言项
    resLanguage: [
        { key: 'zh-CN', icon: 'flag' },
        { key: 'en-US', icon: 'flag' },
        { key: 'auto', icon: 'flag' }
    ],
    languageGet: () => {
        if (ndkI18n.language == "auto") {
            if (navigator.language in ndkI18n.languages) {
                return navigator.language;
            }
            return "en-US";
        }
        return ndkI18n.language;
    },

    /**
     * 获取当前语言
     * @param {*} key 
     * @returns 
     */
    get: key => ndkI18n.languages[ndkI18n.languageGet()][key] || key,

    // 获取语言
    lg: {},

    // 语言列表
    languages: {
        "zh-CN": {
            //公共
            menu: "菜单",
            setting: "设置",
            quick: "快捷",
            note: "笔记",
            link: "链接",
            search: "搜索",
            export: "导出",
            import: "导入",
            exportData: "导出数据",
            importData: "导入数据",
            delete: "删除",
            refresh: "刷新",
            confirm: "确认",
            close: "关闭",
            cancel: "取消",
            save: "保存",
            default: "默认",
            current: "当前",
            about: "关于",
            version: "版本",
            notify: "通知",
            message: "消息",
            fullScreenSwitch: "全屏切换",
            wrapSwitch: "换行切换",
            createTime: "创建时间",
            updateTime: "更新时间",
            group: "分组",
            expandAll: "全部展开",
            collapseAll: "全部收起",
            generatingScript: "正在生成脚本",
            pleaseChoose: "请选择",
            run: "运行",
            debug: "调试",
            test: "测试",
            copyTitle: "复制标题",
            see: "查看",

            //提示
            done: "已完成!",
            reloadDone: "刷新页面才能看到效果!",
            success: "成功!",
            contentNotEmpty: "内容不能为空!",
            selectAnItem: "请选择项!",
            selectConn: "请选择连接!",
            selectColumn: "请选择列!",
            selectDataRows: "请选择数据行!",
            inProgress: "正在进行中...",
            onlyTextFile: "只能是文本文件!",
            unsupported: "不支持!",
            serverError: "服务错误!",
            copiedToClipboard: "已复制到剪贴板!",

            //主题
            themeKey_light: "主题-浅色",
            themeKey_dark: "主题-深色",
            themeKey_auto: "主题-自动",

            //语言
            languageKey_zh_CN: "简体中文",
            languageKey_en_US: "English",
            languageKey_auto: "语言-自动",

            //设置
            setTitle: "设置",
            setServerTitle: "服务设置",
            setServerPlaceholder: "服务地址",
            setParameterConfigTitle: "参数设置",

            //参数项
            parameterKey_autoFilterDatabaseNumber: "数据库名超过自动过滤",
            parameterKey_selectDataLimit: "查询数据默认限制行数",
            parameterKey_buildSqlWithQuote: "生成 SQL 带符号",
            parameterKey_buildSqlWithQuote_true: "带符号",
            parameterKey_buildSqlWithQuote_false: "不带",
            parameterKey_buildSqlWithComment: "生成 SQL 带注释",
            parameterKey_buildSqlWithComment_true: "带注释",
            parameterKey_buildSqlWithComment_false: "不带",
            parameterKey_dataSqlBulkInsert: "数据 SQL 批量插入",
            parameterKey_dataSqlWithAutoIncrement: "数据 SQL 带自增列",
            parameterKey_dataSqlWithAutoIncrement_true: "带自增列",
            parameterKey_dataSqlWithAutoIncrement_false: "不带自增列",
            parameterKey_editorFontSize: "编辑器字体大小",
            parameterKey_editorLineNumbers: "编辑器行号",
            parameterKey_editorLineNumbers_on: "显示",
            parameterKey_editorLineNumbers_off: "不显示",
            parameterKey_editorWordWrap: "编辑器换行",
            parameterKey_editorWordWrap_on: "自动换行",
            parameterKey_editorWordWrap_off: "不换行",
            parameterKey_gridDataShowLength: "表格数据显示截断",
            parameterKey_maxSqlHistory: "SQL 历史记录",
            parameterKey_readFileEncoding: "读取文件编码",
            parameterKey_readFileEncoding_GBK: "GBK",
            parameterKey_readFileEncoding_utf_8: "utf-8",

            //导入导出
            setExportTitle: "配置导入导出",
            setExportSave: "导出配置",
            setExportSaveClipboard: "导出配置（剪贴板）",
            setImportPlaceholder: "拖拽配置文件到此处 或 粘贴配置文件内容",
            setImportSave: "导入配置",
            setImportSaveClipboard: "导入配置（剪贴板）",

            //存储键
            storageKey_conns: "连接",
            storageKey_steps: "步骤",
            storageKey_errors: "错误",
            storageKey_notes: "笔记",
            storageKey_historys: "历史",

            //查看
            dbEnvInfo: "环境信息",
            dbVarInfo: "变量信息",
            dbStatusInfo: "状态信息",
            dbDataType: "数据类型",
            dbUser: "用户",
            dbRole: "角色",
            dbSession: "会话",
            dbLock: "锁",
            dbView: "视图",
            dbIndex: "索引",
            dbTrigger: "触发器",

            dbParamsInfo: "参数信息",
            package: "包",
            generateCode: "生成代码",
            generateFaker: "生成假数据",

            //选项卡（结构树）连接
            tab1Conns: "连接",
            tab1Database: "库",
            tab1Table: "表",
            tab1Column: "列",

            //选项卡（结构树）连接表格
            connAlias: "连接名",
            connEnv: "环境",
            connGroup: "分组",
            connType: "类型",
            connOrder: "排序",
            connConnection: "连接字符串",
            connControl: "操作",

            //选项卡（结构树）连接右键菜单
            newQuery: "新建查询",
            openConn: "打开连接",
            createConn: "新建连接",
            editConn: "编辑连接",
            copyConn: "复制连接",
            deleteConn: "删除连接",
            confirmDelete: "确认删除",

            //选项卡（结构树）数据库表格
            databaseInfo: "数据库信息",
            dbName: "库名",
            dbOwner: "所有者",
            dbSpace: "表空间",
            dbCharSet: "字符集",
            dbCollation: "排序规则",
            dbDataSize: "数据大小",
            dbLogSize: "日志大小",
            dbIndexSize: "索引大小",
            dbDataPath: "库路径",
            dbLogPath: "日志路径",

            //选项卡（结构树）数据库右键菜单
            openDatabase: "打开库",
            createDatabase: "创建库 (SQL)",
            dropDatabase: "删除库 (SQL)",
            executeSqlFile: "执行 SQL 文件",

            //选项卡（结构树）表
            tableName: "表名",
            tableComment: "表注释",
            tableRows: "行数",
            tableDataSize: "数据大小",
            tableIndexSize: "索引大小",
            tableCharSet: "字符集",
            tableCollation: "排序规则",
            schemaName: "模式",
            tableOwner: "所属",
            tableSpace: "空间",
            tableCatagory: "类别",

            //选项卡（结构树）表右键菜单
            tableDesign: "表设计",
            tableData: "表数据",
            tableGenerateSQL: "生成 SQL",

            //选项卡（结构树）列
            columnName: "列名",
            columnComment: "列注释",
            columnTypeLength: "类型及长度",
            columnType: "类型",
            columnLength: "长度",
            columnScale: "精度",
            columnOrder: "列序",
            columnPrimary: "主键",
            columnAutoIncrement: "自增",
            columnDefault: "默认值",
            columnIsNullable: "必填",

            //选项卡（右侧主体）功能按钮
            sqlButtonExecute: "执行选中或全部脚本",
            sqlButtonFormat: "格式化脚本",
            sqlButtonNote: "笔记",
            sqlButtonHistory: "执行 SQL 历史记录",

            //选项卡（SQL执行结果）执行结果
            dataGenerateSQL: "复制为 SQL",

            //查看 base64
            viewBase64ToFile: "转文件",
            viewBase64Decode: "转文本",
            viewBase64Detect: "检测 MIME",
        },

        "en-US": {
            // 公共
            menu: "Menu",
            setting: "Setting",
            quick: "Quick",
            note: "Note",
            link: "Link",
            search: "Search",
            export: "Export",
            import: "Import",
            exportData: "Export Data",
            importData: "Import Data",
            delete: "Delete",
            refresh: "Refresh",
            confirm: "Confirm",
            close: "Close",
            cancel: "Cancel",
            save: "Save",
            default: "Default",
            current: "Current",
            about: "About",
            version: "Version",
            notify: "Notify",
            message: "Message",
            fullScreenSwitch: "Full Screen Switch",
            wrapSwitch: "Wrap Switch",
            createTime: "Create Time",
            updateTime: "Update Time",
            group: "Group",
            expandAll: "Expand All",
            collapseAll: "Collapse All",
            generatingScript: "Generating Script",
            pleaseChoose: "Please Choose",
            run: "Run",
            debug: "Debug",
            test: "Test",
            copyTitle: "Copy Title",
            see: "See",

            // 提示
            done: "Done!",
            reloadDone: "Reload page to see the effect!",
            success: "Success!",
            contentNotEmpty: "Content can not be empty!",
            selectAnItem: "Select an item!",
            selectConn: "Please select connection!",
            selectColumn: "Please select column!",
            selectDataRows: "Select data rows!",
            inProgress: "In progress...",
            onlyTextFile: "Only text file!",
            unsupported: "Unsupported!",
            serverError: "Service Error!",
            copiedToClipboard: "Copied to clipboard!",

            //主题
            themeKey_light: "Theme Light",
            themeKey_dark: "Theme Dark",
            themeKey_auto: "Theme Auto",

            //语言
            languageKey_zh_CN: "简体中文",
            languageKey_en_US: "English",
            languageKey_auto: "Language Auto",

            //设置
            setTitle: "Setting",
            setServerTitle: "Server setting",
            setServerPlaceholder: "Server host",
            setParameterConfigTitle: "Parameter configuration",

            //参数项
            parameterKey_autoFilterDatabaseNumber: "Auto filter database number",
            parameterKey_selectDataLimit: "Select data default limit",
            parameterKey_buildSqlWithQuote: "Build SQL with quote",
            parameterKey_buildSqlWithQuote_true: "With quote",
            parameterKey_buildSqlWithQuote_false: "Without quote",
            parameterKey_buildSqlWithComment: "Build SQL with comment",
            parameterKey_buildSqlWithComment_true: "With comment",
            parameterKey_buildSqlWithComment_false: "Without comment",
            parameterKey_dataSqlBulkInsert: "Data SQL bulk insert",
            parameterKey_dataSqlWithAutoIncrement: "Data SQL with auto increment",
            parameterKey_dataSqlWithAutoIncrement_true: "With auto increment",
            parameterKey_dataSqlWithAutoIncrement_false: "Without auto increment",
            parameterKey_editorFontSize: "Editor font size",
            parameterKey_editorLineNumbers: "Editor line numbers",
            parameterKey_editorLineNumbers_on: "Show",
            parameterKey_editorLineNumbers_off: "Hide",
            parameterKey_editorWordWrap: "Editor word wrap",
            parameterKey_editorWordWrap_on: "Auto wrap",
            parameterKey_editorWordWrap_off: "No wrap",
            parameterKey_gridDataShowLength: "Grid data show length",
            parameterKey_maxSqlHistory: "Max SQL history",
            parameterKey_readFileEncoding: "Read file encoding",
            parameterKey_readFileEncoding_GBK: "GBK",
            parameterKey_readFileEncoding_utf_8: "utf-8",

            //导入导出
            setExportTitle: "Configuration import and export",
            setExportSave: "Export configuration",
            setExportSaveClipboard: "Export configuration (clipboard)",
            setImportPlaceholder: "Paste configuration content",
            setImportSave: "Import configuration",
            setImportSaveClipboard: "Import configuration (clipboard)",

            //存储键
            storageKey_conns: "conns",
            storageKey_steps: "steps",
            storageKey_errors: "errors",
            storageKey_notes: "notes",
            storageKey_historys: "historys",

            //快捷
            dbEnvInfo: "Env Info",
            dbVarInfo: "Var Info",
            dbStatusInfo: "Status Info",
            dbDataType: "Type of data",
            dbUser: "User",
            dbRole: "Role",
            dbSession: "Session",
            dbLock: "Lock",
            dbView: "View",
            dbIndex: "Index",
            dbTrigger: "Trigger",

            dbParamsInfo: "Params Info",
            package: "Package",
            generateCode: "Generate Code",
            generateFaker: "Generate Faker",

            //选项卡（结构树）连接
            tab1Conns: "Connection",
            tab1Database: "Database",
            tab1Table: "Table",
            tab1Column: "Column",

            //选项卡（结构树）连接表格
            connAlias: "Alias",
            connEnv: "Env",
            connGroup: "Group",
            connType: "Type",
            connOrder: "Order",
            connConnection: "Connection String",
            connControl: "Control",

            //选项卡（结构树）连接右键菜单
            newQuery: "New Query",
            openConn: "Open Conn",
            createConn: "Create Conn",
            editConn: "Edit Conn",
            copyConn: "Copy Conn",
            deleteConn: "Delete Conn",
            confirmDelete: "Confirm Delete",

            //选项卡（结构树）数据库表格
            databaseInfo: "Database Info",
            dbName: "Database Name",
            dbOwner: "Owner",
            dbSpace: "Space",
            dbCharSet: "CharSet",
            dbCollation: "Collation",
            dbDataSize: "Data Size",
            dbLogSize: "Log Size",
            dbIndexSize: "Index Size",
            dbDataPath: "Data Path",
            dbLogPath: "Log Path",

            //选项卡（结构树）数据库右键菜单
            openDatabase: "Open Database",
            createDatabase: "Create Database (SQL)",
            dropDatabase: "Drop Database (SQL)",
            executeSqlFile: "Execute SQL File",

            //选项卡（结构树）表
            tableName: "Table Name",
            tableComment: "Table Comment",
            tableRows: "Rows",
            tableDataSize: "Data Size",
            tableIndexSize: "Index Size",
            tableCharSet: "CharSet",
            tableCollation: "Collation",
            schemaName: "Schema",
            tableOwner: "Owner",
            tableSpace: "Space",
            tableCatagory: "Catagory",

            //选项卡（结构树）表右键菜单
            tableDesign: "Table Design",
            tableData: "Table Data",
            tableGenerateSQL: "Generate SQL",

            //选项卡（结构树）列
            columnName: "Column Name",
            columnComment: "Column Comment",
            columnTypeLength: "Type and Length",
            columnType: "Type",
            columnLength: "Length",
            columnScale: "Scale",
            columnOrder: "Order",
            columnPrimary: "Primary Key",
            columnAutoIncrement: "Auto Increment",
            columnDefault: "Default",
            columnIsNullable: "Required",

            //选项卡（右侧主体）功能按钮
            sqlButtonExecute: "Execute Selected or All Script",
            sqlButtonFormat: "Format Script",
            sqlButtonNote: "Note",
            sqlButtonHistory: "Execute SQL History",

            //选项卡（SQL执行结果）执行结果
            dataGenerateSQL: "Copy as SQL",

            //查看 base64
            viewBase64ToFile: "To File",
            viewBase64Decode: "Decode",
            viewBase64Detect: "Detect MIME",
        }
    },
}

// 语言对象
let elang = [];
for (const key in ndkI18n.languages["zh-CN"]) {
    elang.push(`get ${key}() { return ndkI18n.get('${key}') }`);
}
eval(`ndkI18n.lg = {${elang.join(',')}}`);

export { ndkI18n }
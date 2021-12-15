var ndkI18n = {
    language: "auto", // 语言，可选值：auto, zh-CN, en-US
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
            search: "搜索",
            export: "导出",
            import: "导入",
            refresh: "刷新",
            confirm: "确认",
            cancel: "取消",
            save: "保存",
            default: "默认",
            current: "当前",
            fullScreenSwitch: "全屏切换",
            wrapSwitch: "不换行/换行",
            createTime: "创建时间",
            updateTime: "更新时间",
            group: "分组",
            expandAll: "全部展开",
            collapseAll: "全部收起",
            generatingScript: "正在生成脚本",

            //提示
            done: "已完成！",
            reloadDone: "刷新页面才能看到效果！",
            success: "成功！",
            contentNotEmpty: "内容不能为空！",
            selectDataRows: "请选择数据行！",
            inProgress: "正在进行中...",

            //主题
            themeLight: "主题-浅色",
            themeDark: "主题-深色",
            themeAuto: "主题-自动",

            //语言
            languageZHCN: "简体中文",
            languageENUS: "English",
            languageAuto: "语言-自动",

            //设置
            setTitle: "设置",
            setServerTitle: "服务设置",
            setServerPlaceholder: "服务地址",

            setParameterConfigTitle: "参数设置",

            setExportTitle: "配置导入导出",
            setExportSave: "导出配置",
            setExportSaveClipboard: "导出配置到剪贴板",
            setImportPlaceholder: "粘贴配置内容",
            setImportButton: "导入配置并刷新",

            //选项卡1连接
            tab1Conns: "连接",
            tab1Database: "库",
            tab1Table: "表",
            tab1Column: "列",

            //选项卡1连接表格
            connAlias: "连接名",
            connEnv: "环境",
            connGroup: "分组",
            connType: "类型",
            connOrder: "排序",
            connConnection: "连接字符串",
            connControl: "操作",

            //选项卡1连接右键菜单
            newQuery: "新建查询",
            openConn: "打开连接",
            createConn: "新建连接",
            demoConn: "示例连接",
            editConn: "编辑连接",
            copyConn: "复制连接",
            deleteConn: "删除连接",
            confirmDelete: "确认删除",

            //选项卡1数据库表格
            databaseInfo: "数据库信息",
            dbName: "库名",
            dbCatagory: "类别",
            dbOwner: "所有者",
            dbSpace: "表空间",
            dbCharSet: "字符集",
            dbCollate: "排序规则",
            dbDataSize: "数据大小",
            dbLogSize: "日志大小",
            dbIndexSize: "索引大小",
            dbDataPath: "库路径",
            dbLogPath: "日志路径",

            //选项卡1数据库右键菜单
            openDatabase: "打开库",

            //选项卡1表
            tableName: "表名",
            tableComment: "表注释",
            tableRows: "行数",
            tableDataSize: "数据大小",
            tableIndexSize: "索引大小",
            tableCharSet: "字符集",
            tableCollate: "排序规则",
            tableSchema: "架构",
            tableCatagory: "类别",

            //选项卡1表右键菜单
            tableDesign: "表设计",
            tableData: "表数据",
            tableGenerateSQL: "生成 SQL",

            //选项卡1列
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

            //选项卡2功能按钮
            sqlButtonExecute: "执行选中或全部脚本",
            sqlButtonFormat: "格式化脚本",
            sqlButtonNote: "笔记",

            //选项卡3执行结果
            dataGenerateSQL: "生成 SQL",
        },

        "en-US": {
            // 公共
            menu: "Menu",
            setting: "Setting",
            quick: "Quick",
            note: "Note",
            search: "Search",
            export: "Export",
            import: "Import",
            refresh: "Refresh",
            confirm: "Confirm",
            cancel: "Cancel",
            save: "Save",
            default: "Default",
            current: "Current",
            fullScreenSwitch: "Full Screen Switch",
            wrapSwitch: "Wrap Switch",
            createTime: "Create Time",
            updateTime: "Update Time",
            group: "Group",
            expandAll: "Expand All",
            collapseAll: "Collapse All",
            generatingScript: "Generating Script",

            // 提示
            done: "Done!",
            reloadDone: "Reload page to see the effect!",
            success: "Success!",
            contentNotEmpty: "Content can not be empty!",
            selectDataRows: "Select data rows!",
            inProgress: "In progress...",

            //主题
            themeLight: "Theme Light",
            themeDark: "Theme Dark",
            themeAuto: "Theme Auto",

            //语言
            languageZHCN: "简体中文",
            languageENUS: "English",
            languageAuto: "Language Auto",

            //设置
            setTitle: "Setting",
            setServerTitle: "Server setting",
            setServerPlaceholder: "Server host",

            setParameterConfigTitle: "Parameter configuration",

            setExportTitle: "Configuration import and export",
            setExportSave: "Export configuration",
            setExportSaveClipboard: "Export configuration to clipboard",
            setImportPlaceholder: "Paste configuration content",
            setImportButton: "Import configuration and refresh",

            //选项卡1连接
            tab1Conns: "Connection",
            tab1Database: "Database",
            tab1Table: "Table",
            tab1Column: "Column",

            //选项卡1连接表格
            connAlias: "Alias",
            connEnv: "Env",
            connGroup: "Group",
            connType: "Type",
            connOrder: "Order",
            connConnection: "Connection String",
            connControl: "Control",

            //选项卡1连接右键菜单
            newQuery: "New Query",
            openConn: "Open Conn",
            createConn: "Create Conn",
            demoConn: "Demo Conn",
            editConn: "Edit Conn",
            copyConn: "Copy Conn",
            deleteConn: "Delete Conn",
            confirmDelete: "Confirm Delete",

            //选项卡1数据库表格
            databaseInfo: "Database Info",
            dbName: "Database Name",
            dbCatagory: "Catagory",
            dbOwner: "Owner",
            dbSpace: "Space",
            dbCharSet: "CharSet",
            dbCollate: "Collate",
            dbDataSize: "Data Size",
            dbLogSize: "Log Size",
            dbIndexSize: "Index Size",
            dbDataPath: "Data Path",
            dbLogPath: "Log Path",

            //选项卡1数据库右键菜单
            openDatabase: "Open Database",

            //选项卡1表
            tableName: "Table Name",
            tableComment: "Table Comment",
            tableRows: "Rows",
            tableDataSize: "Data Size",
            tableIndexSize: "Index Size",
            tableCharSet: "CharSet",
            tableCollate: "Collate",
            tableSchema: "Schema",
            tableCatagory: "Catagory",

            //选项卡1表右键菜单
            tableDesign: "Table Design",
            tableData: "Table Data",
            tableGenerateSQL: "Generate SQL",

            //选项卡1列
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

            //选项卡2功能按钮
            sqlButtonExecute: "Execute Selected or All Script",
            sqlButtonFormat: "Format Script",
            sqlButtonNote: "Note",

            //选项卡3执行结果
            dataGenerateSQL: "Generate SQL",
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
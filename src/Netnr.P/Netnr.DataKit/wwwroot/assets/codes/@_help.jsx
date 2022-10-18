{
    files: [{
        fullName: `help.txt`,
        content: [
            '生成代码构建脚本位于 assets/codes/',
            '格式为 {language}_{name}.jsx 例如 csharp_model.jsx',
            '新增或删除 jsx 文件后执行 build.bat 或 build.sh 重建 list.txt 列表',
            '后缀为 jsx 与 React 无关，如果为 js 后缀打包需要额外处理'
        ].join('\r\n')
    }]
} 
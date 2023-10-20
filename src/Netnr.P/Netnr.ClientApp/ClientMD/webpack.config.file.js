const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = (_env, argv) => {
    console.debug(argv);

    const releaseRoot = path.join(__dirname, "../../Netnr.Blog.Web/wwwroot/file/md");
    console.debug(`✨ Release: ${releaseRoot}`);

    let config = {
        mode: 'production',
        entry: {
            netnrmd: path.join(__dirname, './index.js'),
            // 打包后实践，部分页面使用 await nrcRely.remote("netnrmdEditor") 引入出错
            // 使用线上方式代替 await nrEditor.init()
            // monaco: path.join(__dirname, './monaco.js'),
        },
        output: {
            path: releaseRoot,
            clean: true, //删除
        },
        module: {
            rules: [
                { test: /\.css$/i, use: [MiniCssExtractPlugin.loader, 'css-loader'] },
                { test: /\.svg$/, loader: 'svg-sprite-loader' }
            ],
        },
        optimization: {
            minimize: true,
            minimizer: [
                new CssMinimizerPlugin(),
                new TerserPlugin({ extractComments: false, })
            ]
        },
        plugins: [
            new MiniCssExtractPlugin({
                filename: "[name].css",
                experimentalUseImportModule: true,
            }),
            new CopyPlugin({
                patterns: [
                    {
                        from: path.join(__dirname, './README.md'),
                        to: path.resolve(releaseRoot)
                    },
                    {
                        from: path.join(__dirname, './CHANGELOG.md'),
                        to: path.resolve(releaseRoot)
                    }
                ]
            })
        ]
    };

    return config;
}
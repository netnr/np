const path = require('path');
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = (_env, argv) => {
    console.debug(argv);

    const releaseRoot = path.join(__dirname, "../../Netnr.Blog.Web/wwwroot/file/docsify-better");
    console.debug(`✨ Release: ${releaseRoot}`);

    let isDev = argv.mode == 'development';
    let config = {
        mode: isDev ? 'development' : 'production',
        entry: {
            "docsify-better": path.join(__dirname, './index.js'),
        },
        output: {
            filename: "[name].js",
            path: path.resolve(releaseRoot),
            clean: false, //删除
        },
        module: {
            rules: [
                { test: /\.css$/i, use: [MiniCssExtractPlugin.loader, 'css-loader'] },
                { test: /\.svg$/, loader: 'svg-sprite-loader' }
            ],
        },
        optimization: {
            minimize: !isDev,
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
        ]
    };

    return config;
}
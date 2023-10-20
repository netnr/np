const path = require('path');
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = (_env, argv) => {
    console.debug(argv);

    const releaseRoot = path.join(__dirname, "../../Netnr.Blog.Web/wwwroot/dist");
    console.debug(`✨ Release: ${releaseRoot}`);

    let isDev = argv.mode == 'development';
    let config = {
        mode: isDev ? 'development' : 'production',
        entry: {
            main: path.join(__dirname, './index.js'),
        },
        output: {
            filename: '[name].js',
            chunkFilename: '[name].[contenthash].js',
            path: path.resolve(releaseRoot),
            clean: true, //删除
        },
        // Set devServer
        devServer: {
            static: { directory: __dirname }, port: 775,
            server: require('../file/client.json').server,
            historyApiFallback: true, //history
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
            })
        ]
    };

    return config;
}
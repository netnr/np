const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const WorkboxPlugin = require('workbox-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const releaseRoot = path.join(__dirname, "dist");

let config = {
    entry: path.join(__dirname, './indexWeb.js'),
    output: {
        filename: "[name].[contenthash].js",
        path: path.resolve(releaseRoot),
        clean: false, //删除
    },
    // Set devServer
    devServer: {
        static: { directory: __dirname }, port: 783,
        historyApiFallback: true, //history
    },
    module: {
        rules: [
            { test: /\.css$/i, use: [MiniCssExtractPlugin.loader, 'css-loader'] },
            { test: /\.svg$/, loader: 'svg-sprite-loader' }
        ],
    },
    optimization: {
        minimize: false,
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
        new HtmlWebpackPlugin({
            filename: 'index.html',
            template: path.join(__dirname, './file/index.html'),
            inject: "body",
        }),
        new CopyPlugin({
            patterns: [
                {
                    from: path.join(__dirname, '../file/favicon.ico'),
                    to: path.resolve(releaseRoot)
                },
                {
                    from: path.join(__dirname, './README.md'),
                    to: path.resolve(releaseRoot)
                }
            ]
        })
    ]
};

module.exports = (env, argv) => {
    console.debug(env, argv);
    console.debug(`Release: ${releaseRoot}`);

    if (argv.mode == 'development') {
        config.mode = 'development';
        config.optimization.minimize = false;
    } else {
        config.mode = 'production';
        config.optimization.minimize = true;

        config.plugins.push(new WorkboxPlugin.GenerateSW({
            swDest: '/sw.js', clientsClaim: true, skipWaiting: true,
            maximumFileSizeToCacheInBytes: 1024 * 1024 * 10
        }))
    }

    return config;
}
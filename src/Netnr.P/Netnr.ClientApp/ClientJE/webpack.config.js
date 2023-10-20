const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const WorkboxPlugin = require('workbox-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = (_env, argv) => {
    console.debug(argv);

    const releaseRoot = path.join(__dirname, "dist");
    console.debug(`✨ Release: ${releaseRoot}`);

    let isDev = argv.mode == 'development';
    let config = {
        mode: isDev ? 'development' : 'production',
        entry: path.join(__dirname, './index.js'),
        output: {
            filename: "[name]-[contenthash].js",
            path: path.resolve(releaseRoot),
            clean: true, //删除
        },
        // Set devServer
        devServer: {
            static: { directory: __dirname }, port: 781,
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
            new HtmlWebpackPlugin({
                filename: 'index.html',
                template: path.join(__dirname, './file/index.html'),
                inject: "body",
            }),
            new MiniCssExtractPlugin({
                filename: "[name]-[contenthash].css",
                experimentalUseImportModule: true,
            }),
            new CopyPlugin({
                patterns: [
                    {
                        from: path.join(__dirname, '../file/favicon.ico'),
                        to: path.resolve(releaseRoot)
                    }
                ]
            }),
        ]
    };

    if (!isDev) {
        config.plugins.push(new WorkboxPlugin.GenerateSW({
            swDest: '/sw.js', clientsClaim: true, skipWaiting: true,
            maximumFileSizeToCacheInBytes: 1024 * 1024 * 10
        }))
    }

    return config;
}
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
        entry: path.join(__dirname, './indexWeb.js'),
        output: {
            filename: "[name].[contenthash].js",
            path: path.resolve(releaseRoot),
            clean: true, //删除
        },
        // Set devServer
        devServer: {
            static: { directory: __dirname }, port: 783,
            server: require('../file/client.json').server,
            historyApiFallback: true, //history
        },
        module: {
            rules: [
                { test: /\.css$/i, use: [MiniCssExtractPlugin.loader, 'css-loader'] },
                { test: /\.svg$/, loader: 'svg-sprite-loader' },
                {
                    test: /\.(?:js|mjs|cjs)$/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            plugins: ['@babel/plugin-transform-runtime'],
                            presets: [['@babel/preset-env', { targets: { chrome: "78" } }]],
                            cacheDirectory: true,
                        }
                    }
                }
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

    if (!isDev) {
        config.plugins.push(new WorkboxPlugin.GenerateSW({
            swDest: '/sw.js', clientsClaim: true, skipWaiting: true,
            maximumFileSizeToCacheInBytes: 1024 * 1024 * 10
        }))
    }

    return config;
}
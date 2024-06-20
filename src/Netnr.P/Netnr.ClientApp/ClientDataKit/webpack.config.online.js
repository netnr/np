const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = (_env, argv) => {
    console.debug(argv);

    const releaseRoot = path.join(__dirname, "../../Netnr.DataKit/wwwroot");
    console.debug(`✨ Release: ${releaseRoot}`);

    let isDev = argv.mode == 'development';
    let config = {
        mode: isDev ? 'development' : 'production',
        entry: path.join(__dirname, './index.js'),
        output: {
            filename: "[name].[contenthash].js",
            path: path.resolve(releaseRoot),
            clean: true, //删除
        },
        // Set devServer
        devServer: {
            static: { directory: __dirname }, port: 777,
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
            new HtmlWebpackPlugin({
                filename: 'index.html',
                template: path.join(__dirname, './file/index_online.html'),
                inject: "body",
            }),
            new MiniCssExtractPlugin({
                filename: "[name].[contenthash].css",
                experimentalUseImportModule: true,
            }),
            new CopyPlugin({
                patterns: [
                    {
                        from: path.join(__dirname, '../file/favicon.ico'),
                        to: path.resolve(releaseRoot)
                    },
                    {
                        from: path.join(__dirname, './file/assets'),
                        to: path.resolve(releaseRoot, 'assets')
                    }
                ]
            })
        ]
    };

    return config;
}

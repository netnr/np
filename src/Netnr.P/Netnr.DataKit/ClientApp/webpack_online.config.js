const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const releaseRoot = __dirname.replace("ClientApp", "wwwroot");

var config = {
    entry: './src/js/index_online.js',
    output: {
        filename: "[name].[contenthash].js",
        path: path.resolve(releaseRoot),
        clean: true
    },
    // Set devServer
    devServer: {
        static: {
            directory: path.join(__dirname, './src'),
        },
        port: 8002,
    },
    module: {
        // Bundle styles
        rules: [
            {
                test: /\.css$/i,
                use: [MiniCssExtractPlugin.loader, 'css-loader']
            },
            {
                test: /\.svg$/,
                loader: 'svg-sprite-loader'
            }
        ],
    },
    optimization: {
        minimize: false,
        minimizer: [
            new CssMinimizerPlugin(),
            new TerserPlugin({
                extractComments: false,
            })
        ]
    },
    plugins: [
        new HtmlWebpackPlugin({
            filename: 'index.html',
            template: 'src/index_online.html',
            inject: "body",
        }),
        new MiniCssExtractPlugin({
            filename: "[name].[contenthash].css",
            experimentalUseImportModule: true,
        }),
        new CopyPlugin({
            patterns: [
                {
                    from: path.join(__dirname, './src/favicon.ico'),
                    to: path.resolve(releaseRoot)
                },
                // Copy custom icons to dist/shoelace
                {
                    from: path.resolve(__dirname, './src/icons'),
                    to: path.resolve(releaseRoot, 'images/icons')
                },
            ]
        })
    ]
};

module.exports = (env, argv) => {
    console.log(env, argv);

    if (argv.mode == 'development') {
        config.mode = 'development';
        config.optimization.minimize = false;
    } else {
        config.mode = 'production';
        config.optimization.minimize = true;
    }
    return config;
}
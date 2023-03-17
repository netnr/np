const path = require('path');
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const releaseRoot = path.join(__dirname, "../../Netnr.Blog.Web/wwwroot/dist");

let config = {
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
    }

    return config;
}
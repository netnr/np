const path = require('path');
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

var config = {
    entry: {
        'device-detector-js': './libs/device-detector-js.js',
        'fast-xml-parser': './libs/fast-xml-parser.js',
        'js-yaml': './libs/js-yaml.js',
    },
    output: {
        filename: "[name].min.js",
        path: path.join(__dirname, './dist'),
        clean: true
    },
    module: {
        // Bundle styles
        rules: [
            { test: /\.css$/i, use: [MiniCssExtractPlugin.loader, 'css-loader'] },
            { test: /\.svg$/, loader: 'svg-sprite-loader' },
        ]
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
        new MiniCssExtractPlugin({
            filename: "[name].min.css",
            experimentalUseImportModule: true,
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
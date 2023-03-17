const path = require('path');
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const releaseRoot = path.join(__dirname, "../../Netnr.Blog.Web/wwwroot/file/docsify-better");

let config = {
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
    ]
};

module.exports = (env, argv) => {
    console.debug(env, argv);
    console.debug(`Release: ${releaseRoot}`);

    if (argv.mode == 'development') {
        config.mode = 'development';
    } else {
        config.mode = 'production';
    }

    return config;
}
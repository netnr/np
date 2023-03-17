const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const releaseRoot = path.join(__dirname, "../../Netnr.Blog.Web/wwwroot/file/md");

let config = {
    entry: {
        netnrmd: path.join(__dirname, './index.js'),
        ace: path.join(__dirname, './indexAce.js'),
        "netnrmd.bundle": path.join(__dirname, './indexBundle.js'),
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
        new CopyPlugin({
            patterns: [
                {
                    from: path.join(__dirname, './README.md'),
                    to: path.resolve(releaseRoot)
                },
                {
                    from: path.join(__dirname, './CHANGELOG.md'),
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
    } else {
        config.mode = 'production';
    }

    return config;
}
const path = require('path');
const TerserPlugin = require("terser-webpack-plugin");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const releaseRoot = path.join(__dirname, './dist');

var config = {
    entry: {
        ace: './src/js/indexAce.js',
        netnrmd: './src/js/index.js'
    },
    output: {
        filename: "[name].js",
        path: path.resolve(releaseRoot),
        clean: true
    },
    // Set devServer
    devServer: {
        static: {
            directory: path.join(__dirname, './src'),
        },
        port: 8003,
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
        new MiniCssExtractPlugin({
            filename: "[name].css",
            experimentalUseImportModule: true,
        })
    ]
};

module.exports = (env, argv) => {
    console.log(env, argv);

    if (argv.mode == 'development') {
        config.mode = 'development';
        config.optimization.minimize = false;

        config.plugins.push(new HtmlWebpackPlugin({
            filename: 'index.html',
            template: 'src/index.html',
            inject: "head",
        }));
    } else {
        config.mode = 'production';
        config.optimization.minimize = true;
    }
    return config;
}
const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
//const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

module.exports = {
    mode: 'production',
    entry: './src/js/index.js',
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, 'dist')
    },
    // Set devServer
    devServer: {
        static: {
            directory: path.join(__dirname, './src'),
        },
        port: 45
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
    plugins: [
        new HtmlWebpackPlugin({
            filename: 'index.html',
            template: 'src/index.html'
        }),
        new MiniCssExtractPlugin({
            filename: 'bundle.css'
        }),
        new CopyPlugin({
            patterns: [
                {
                    from: path.join(__dirname, './src/favicon.ico'),
                    to: path.resolve(__dirname, 'dist')
                }
            ]
        }),
        // new CopyPlugin({
        //     patterns: [
        //         // Copy Shoelace assets to dist/shoelace
        //         {
        //             from: path.resolve(__dirname, 'node_modules/@shoelace-style/shoelace/dist/assets'),
        //             to: path.resolve(__dirname, 'dist/shoelace/assets')
        //         }
        //     ]
        // }),
        //new MonacoWebpackPlugin()
    ]
};

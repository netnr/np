const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const releaseRoot = __dirname.replace("ClientApp", "wwwroot");

var config = {
    entry: './src/js/index.js',
    output: {
        filename: "[name].[contenthash].js",
        path: path.resolve(releaseRoot),
        publicPath: '/',
        clean: true
    },
    // Set devServer
    devServer: {
        static: {
            directory: path.join(__dirname, './src'),
        },
        port: 9933,
        historyApiFallback: true,
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
            template: 'src/index.html',
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

                // Copy Shoelace assets to dist/shoelace
                {
                    from: path.resolve(__dirname, 'node_modules/@shoelace-style/shoelace/dist/assets'),
                    to: path.resolve(releaseRoot, 'shoelace/assets')
                },
                // Copy custom icons to dist/shoelace
                // {
                //     from: path.resolve(__dirname, './src/icons'),
                //     to: path.resolve(releaseRoot, 'shoelace/assets/icons')
                // },
                {
                    from: path.resolve(__dirname, 'node_modules/@shoelace-style/shoelace/dist/themes'),
                    to: path.resolve(releaseRoot, 'shoelace/themes')
                },
            ]
        }),
        // new MonacoWebpackPlugin({
        //     filename: 'monaco.[name].[contenthash].js',
        //     languages: ['typescript', 'javascript', 'css', 'html', 'xml', 'csharp', 'java', 'php', 'json', 'sql', 'mysql', 'pgsql', 'markdown'],
        // })
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
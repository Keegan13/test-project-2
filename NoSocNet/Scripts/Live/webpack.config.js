"use strict";
var path = require("path");

module.exports = {
    entry: path.resolve(__dirname, "./src/index.ts"),
    output: {
        path: path.resolve(__dirname, './../../wwwroot/js'),
        publicPath: 'live/',
        filename: 'live.js'
    },
    optimization: {
        minimize: false
    }
    ,
    module: {
        rules: [
            {
                test: /\.(ts)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        babelrc: false,
                        presets: ["@babel/typescript", "@babel/env"],
                        plugins: ["@babel/plugin-proposal-class-properties"]
                    }
                }
            }
        ]
    },
    resolve: {
        modules: [path.resolve(__dirname, "./src"), 'node_modules'],
        extensions: ['.ts', '.js'],
    }
};
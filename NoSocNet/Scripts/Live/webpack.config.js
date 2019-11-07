﻿"use strict";
var path = require("path");

module.exports = {
    entry: path.resolve(__dirname, "./src/index.ts"),
    output: {
        path: path.resolve(__dirname, './../../wwwroot/live'),
        publicPath: 'live/',
        filename: '[name].js'
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
                        presets: ["@babel/typescript", "@babel/env"]
                    }
                }
            }
        ]
    },
};
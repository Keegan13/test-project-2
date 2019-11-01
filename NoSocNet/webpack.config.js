var path = require('path');

module.exports = {
    mode: "development",
    devtool: "source-map",
    entry: {
        main: path.join(process.cwd(), 'Scripts/ChatClient/src/index.tsx'),
    },
    output: {
        path: path.resolve(process.cwd(), 'wwwroot/dist'),
        publicPath: 'dist/',
        filename: '[name].js'
    },
    module: {
        rules: [
            {
                test: /\.(ts|tsx)$/,
                exclude: /node_modules/,
                use: 'ts-loader'
            },
            // {
            //     enforce: "pre",
            //     test: /\.js$/,
            //     loader: "source-map-loader"
            // }
        ]
    },
    resolve: {
        modules: ['main', 'node_modules'],
        extensions: ['.ts', '.tsx', ".js", ".jsx"],
    }
}
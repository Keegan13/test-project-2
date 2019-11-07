var path = require('path');



module.exports = (env, args) => {
    const isDev = (args && args.mode === 'development') || (env && env.mode === 'development');

    return ({
        devtool: "source-map",
        entry: {
            main: path.join(process.cwd(), 'Scripts/ChatClient/src/index.tsx'),
        },
        optimization: {
            minimize: !isDev
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
                    use: {
                        loader: 'babel-loader',
                        options: {
                            babelrc: false,
                            presets: ["@babel/react", "@babel/typescript", "@babel/env"],
                            plugins: ["react-hot-loader/babel"]
                        }
                    }
                }
            ]
        },
        resolve: {
            modules: ["main", 'node_modules'],
            extensions: ['.ts', '.tsx', ".js", ".jsx"],
        }
    })
};
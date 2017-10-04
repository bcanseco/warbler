const path                 = require("path");
const glob                 = require("glob-all");
const webpack              = require("webpack");
const ExtractTextPlugin    = require("extract-text-webpack-plugin");
const CopyWebpackPlugin    = require("copy-webpack-plugin");
const bundleOutputDir      = "./wwwroot/dist";

module.exports = (env) => {
  const isDevBuild = !(env && env.prod);
  const extractLess = new ExtractTextPlugin({ filename: "[name].css" });

  const entry = () => {
    const jsx = glob
      .sync("./Components/*/index.jsx")
      .reduce((accumulator, filePath) => {
        accumulator[filePath.split("/")[2]] = filePath;
        return accumulator;
      }, {});

    const other = glob
      .sync("./Graphics/**/*.@(png|jpg|jpeg|gif|svg|ico)")
      .reduce((accumulator, filePath) => {
        accumulator[filePath.replace(/^.*[\\\/]/, "")] = filePath;
        return accumulator;
      }, {"shared-styles.dist": "./Components/site-shared.less"});

    return Object.assign(jsx, other);
  }

  return [{
    stats: { modules: false },
    entry: entry(),
    resolve: { extensions: [".js", ".jsx"] },
    output: {
      path: path.join(__dirname, bundleOutputDir),
      filename: "[name].js",
      publicPath: "dist/"
    },
    module: {
      rules: [
        { test: /\.jsx$/, exclude: /(node_modules)/, use: { loader: "babel-loader" } },
        { test: /\.less$/, exclude: [/(node_modules)/, path.resolve(__dirname, "./Components/site-shared.less")], use: [{ loader: "style-loader" }, { loader: "css-loader" }, { loader: "less-loader" }] },
        { test: /site-shared\.less$/, exclude: /(node_modules)/, use: extractLess.extract({ use: [{ loader: "css-loader" }, { loader: "less-loader" }] }) },
        { test: /\.(png|jpg|jpeg|gif|svg|ico)$/, exclude: /(node_modules)/, use: "file-loader?name=[name].[ext]" }
      ]
    },
    plugins: [
      extractLess,
      new webpack.DllReferencePlugin({
        context: __dirname,
        manifest: require("./wwwroot/dist/vendor-manifest.json")
      }),
      new CopyWebpackPlugin([{ from: "./Components/site-shared.js", to: "shared-scripts.dist.js" }])
    ].concat(isDevBuild ? [
      // Plugins that apply in development builds only
      new webpack.SourceMapDevToolPlugin({
        filename: "[file].map", // Remove this line if you prefer inline source maps
        moduleFilenameTemplate: path.relative(bundleOutputDir, "[resourcePath]") // Point sourcemap entries to the original file locations on disk
      })
    ] : [
      // Plugins that apply in production builds only
      new webpack.optimize.UglifyJsPlugin()
    ])
  }];
};

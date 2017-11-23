const path = require("path");
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = (env) => {
  const extractCSS = new ExtractTextPlugin("vendor.css");
  const isDevBuild = !(env && env.prod);
  return [{
    stats: { modules: false },
    resolve: {
      extensions: [".js"]
    },
    module: {
      rules: [
        { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: "url-loader?limit=100000" },
        { test: /\.css(\?|$)/, use: extractCSS.extract([isDevBuild ? "css-loader" : "css-loader?minimize"]) }
      ]
    },
    entry: {
      vendor: [
        // Required by ASP.NET Identity account pages
        "jquery",
        "jquery-validation",
        "jquery-validation-unobtrusive",
        // Bootstrap & its dependencies
        "tether",
        "popper.js",
        "bootstrap",
        // Everything else
        "daemonite-material",
        "@aspnet/signalr-client",
        "isomorphic-fetch",
        "react",
        "react-dom",
        "react-google-maps",
        "react-simple-popover"
      ].concat([
        "font-awesome/css/font-awesome.css",
        "bootstrap/dist/css/bootstrap.css",
        "daemonite-material/css/material.css"
      ])
    },
    output: {
      path: path.join(__dirname, "wwwroot", "dist"),
      publicPath: "dist/",
      filename: "[name].js",
      library: "[name]_[hash]"
    },
    plugins: [
      extractCSS,
      new webpack.ProvidePlugin({
        tether: 'tether',
        Tether: 'tether',
        'window.Tether': 'tether',
        $: "jquery",
        jQuery: "jquery",
        "window.jQuery": "jquery",
      }),
      new webpack.DllPlugin({
        path: path.join(__dirname, "wwwroot", "dist", "[name]-manifest.json"),
        name: "[name]_[hash]"
      }),
      new webpack.DefinePlugin({
        'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
      })
    ].concat(isDevBuild ? [] : [
      new webpack.optimize.UglifyJsPlugin()
    ])
  }];
}; 

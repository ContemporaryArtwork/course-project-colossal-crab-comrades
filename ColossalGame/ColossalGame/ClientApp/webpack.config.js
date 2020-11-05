const path = require('path');
const webpack = require('webpack');
const HtmlWebpackPlugin = require("html-webpack-plugin");

/*
 * SplitChunksPlugin is enabled by default and replaced
 * deprecated CommonsChunkPlugin. It automatically identifies modules which
 * should be splitted of chunk by heuristics using module duplication count and
 * module category (i. e. node_modules). And splits the chunks…
 *
 * It is safe to remove "splitChunks" from the generated configuration
 * and was added as an educational example.
 *
 * https://webpack.js.org/plugins/split-chunks-plugin/
 *
 */

/*
 * We've enabled TerserPlugin for you! This minifies your app
 * in order to load faster and run less javascript.
 *
 * https://github.com/webpack-contrib/terser-webpack-plugin
 *
 */

const TerserPlugin = require('terser-webpack-plugin');

module.exports = {
	mode: process.env.ASPNETCORE_ENVIRONMENT == "Development" ? "development" : "production",
	entry: './src/index.tsx', //used to be index.ts. index.ts might still be the correct choice.
	plugins: [new webpack.ProgressPlugin(),

		new HtmlWebpackPlugin({
			template: path.resolve(__dirname, "public", "index.html"),
		}),
	

	
	],
	output: {
		path: path.resolve(__dirname, "build"),
		filename: "bundle.js",
		sourceMapFilename: "bundle.js.map"
	  },
	devtool: "source-map",
	devServer: {
		contentBase: path.resolve(__dirname, "build"),
		port:3000
	},
	module: {
		rules: [
			{
				test: /\.(ts|js)x?$/,
				exclude: /node_modules/,
				use: {
				  loader: "babel-loader",
				  options: {
					presets: [
					  ["@babel/preset-env",
					{
						targets: {
							esmodules: true,
						},
					},
					],
					  "@babel/preset-react",
					  "@babel/preset-typescript",
					],
					"plugins": [
						"@babel/proposal-class-properties",
						"@babel/proposal-object-rest-spread"
					]
				  },
				},
			  },
			  {
				test: /\.(ts|tsx)$/,
				loader: 'ts-loader',
				include: [path.resolve(__dirname, 'src')],
				exclude: [/node_modules/],
				options: {
					compilerOptions: {
						"noEmit": false
                    },
				},
			},
			{
				test: /.css$/,

				use: [
					{
						loader: 'style-loader',
					},
					{
						loader: 'css-loader',

						options: {
							sourceMap: true,
						},
					},
				],
			},
			//Added for Phaser images
			{
				test: /\.(png|jpe?g|gif)$/i,
				use: [
					{
						loader: 'file-loader',
					},
				],
			},
			{
				test: /\.mp4$/,
				use: 'file-loader?name=videos/[name].[ext]',
		 },
		],
	},

	resolve: {
		extensions: ['.tsx', '.ts', '.js'],
	},

	optimization: {
		minimizer: [new TerserPlugin()],

		splitChunks: {
			cacheGroups: {
				vendors: {
					priority: -10,
					test: /[\\/]node_modules[\\/]/,
				},
			},

			chunks: 'async',
			minChunks: 1,
			minSize: 30000,
			name: true,
		},
	},
};
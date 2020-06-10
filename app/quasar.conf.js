/* eslint-disable */
// Configuration for your app
// https://quasar.dev/quasar-cli/quasar-conf-js
// const HtmlWebpackPlugin = require('@quasar/app/node_modules/html-webpack-plugin')
// const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const fs = require('fs');
const glob = require('glob');
const appConfig = require('./app.config');
const { BundleAnalyzerPlugin } = require('webpack-bundle-analyzer');

module.exports = function (ctx) {
  return {
    // app boot file (/src/boot)
    // --> boot files are part of "main.js"
    // https://quasar.dev/quasar-cli/cli-documentation/boot-files
    boot: [
      'i18n',
      'axios',
      'blake2b',
      'keypair',
      'base32',
      'idle',
      'auth',
      'createPdf',
      'crypto',
      'web3',
      'server/payment',
      'server/user',
      'server/timestamp',
      'notify-defaults',
    ],

    // https://quasar.dev/quasar-cli/quasar-conf-js#Property%3A-css
    css: [
      'app.scss',
    ],

    // https://github.com/quasarframework/quasar/tree/dev/extras
    extras: [
      // 'ionicons-v4',
      // 'mdi-v4',
      'fontawesome-v5',
      // 'eva-icons',
      // 'themify',
      // 'roboto-font-latin-ext', // this or either 'roboto-font', NEVER both!

      'roboto-font', // optional, you are not bound to it
      'material-icons', // optional, you are not bound to it
    ],

    // https://quasar.dev/quasar-cli/quasar-conf-js#Property%3A-framework
    framework: {
      // iconSet: 'ionicons-v4', // Quasar icon set
      // lang: 'de', // Quasar language pack

      // Possible values for "all":
      // * 'auto' - Auto-import needed Quasar components & directives
      //            (slightly higher compile time; next to minimum bundle size; most convenient)
      // * false  - Manually specify what to import
      //            (fastest compile time; minimum bundle size; most tedious)
      // * true   - Import everything from Quasar
      //            (not treeshaking Quasar; biggest bundle size; convenient)
      all: 'auto',

      components: [],
      directives: [],

      // Quasar plugins
      plugins: ['Loading','Notify'],
    },
    // https://quasar.dev/quasar-cli/cli-documentation/supporting-ie
    supportIE: true,
    // https://quasar.dev/quasar-cli/quasar-conf-js#Property%3A-build
    build: {
      scopeHoisting: true,
      env: appConfig(ctx.dev, process.env.test, process.env.prod),
      //vueRouterMode: 'history',
      // showProgress: false,
      // gzip: true,
      // analyze: true,
      // preloadChunks: false,
      // extractCSS: false,

      // this is a configuration passed on
      // to the underlying Webpack
      devtool: 'source-map',
  
      /**
       * 
       *   (api, { quasarConf }) => ?Promise
       */
      afterBuild({ quasarConf }) {
        let appHost = 'http://localhost:6420';
        let b2cloginPage = 'https://timeproof.b2clogin.com';
        if(ctx.dev){
          appHost = 'http://localhost:6420';
          b2cloginPage = 'https://timeproof.b2clogin.com';
        }else if(process.env.test){
          appHost = 'https://timescribewebteststorage.z35.web.core.windows.net';
          b2cloginPage = 'https://timeproof.b2clogin.com';
        }else if(process.env.prod){
          appHost = 'https://timescribewebstorageprod.z16.web.core.windows.net';
          b2cloginPage = 'https://timescribe.b2clogin.com';
        }

        const fileArr = [
          './dist/spa/statics/login.html',
          './dist/spa/statics/css/login.css',
          './dist/spa/statics/signup.html',
          './dist/spa/statics/css/signup.css',
          './dist/spa/statics/forgotPassword.html',
          './dist/spa/statics/css/forgotPassword.css',
          './dist/spa/statics/editProfile.html',
          './dist/spa/statics/css/editProfile.css'
        ];

        fileArr.forEach(async (file) => {
          fs.readFile(file, 'utf8', async (err, data) => {
            if (err) {
              return console.log(err);
            }

            var result = data.replace(/{appHost}|{b2cloginPage}/g, function (m) {
              return {
                  '{appHost}': appHost,
                  '{b2cloginPage}': b2cloginPage
              }[m];
            });
          
            fs.writeFile(file, result, 'utf8', function (err) {
               if (err) return console.log(err);
            });
          });
        });
      },

      // https://quasar.dev/quasar-cli/cli-documentation/handling-webpack
      extendWebpack(cfg) {
        // const ruleIndex = cfg.module.rules.findIndex(rule => rule.test.toString() === '/\.css$/')

        // cfg.module.rules[ruleIndex].oneOf.unshift({
        //     test: /login\.css$/,
        //     use: [
        //       {
        //         loader: MiniCssExtractPlugin.loader,
        //         options: {
        //           publicPath: './src/statics/css',
        //         },
        //       },
        //       'css-loader',
        //     ],
        //   },   
        // )

        cfg.module.rules.push(
          {
            enforce: 'pre',
            test: /\.(js|vue)$/,
            loader: 'eslint-loader',
            exclude: /node_modules/,
            options: {
              formatter: require('eslint').CLIEngine.getFormatter('stylish'),
            },
          },
          // {
          //   enforce: 'pre',
          //   test: /\.(js)$/,
          //   loader: 'babel-loader',
          //   exclude: /node_modules/,
          // },
        );

        // Output an html file for the page
        
        cfg.plugins.push(
          new BundleAnalyzerPlugin({
            analyzerMode: 'static',
            openAnalyzer: false,
            reportFilename: 'bundle-size.html'
          })
        );
        // cfg.plugins.push(
        //   new HtmlWebpackPlugin({
        //     template: './src/loginSignup/login.html',
        //     filename: `static/login.html`,
        //     chunks: ['login']
        //   }),
        //   new HtmlWebpackPlugin({
        //     template: './src/loginSignup/signup.html',
        //     filename: `static/signup.html`,
        //     chunks: ['signup']
        //   }),
          // new MiniCssExtractPlugin({
          //   // Options similar to the same options in webpackOptions.output
          //   // both options are optional
          //   filename: 'static/css/[name].css',
          //   chunkFilename: 'static/css/[name].css',
          // }),
        // )
          
      }
    },

    // https://quasar.dev/quasar-cli/quasar-conf-js#Property%3A-devServer
    devServer: {
      // https: true,
      port: 6420,
      open: true, // opens browser window automatically
    },

    // animations: 'all', // --- includes all animations
    // https://quasar.dev/options/animations
    animations: [],

    // https://quasar.dev/quasar-cli/developing-ssr/configuring-ssr
    ssr: {
      pwa: false,
    },

    // https://quasar.dev/quasar-cli/developing-pwa/configuring-pwa
    pwa: {
      // workboxPluginMode: 'InjectManifest',
      // workboxOptions: {}, // only for NON InjectManifest
      manifest: {
        // name: 'Upgraded Giggle',
        // short_name: 'Upgraded Giggle',
        // description: 'Document signing app',
        display: 'standalone',
        orientation: 'portrait',
        background_color: '#ffffff',
        theme_color: '#027be3',
        icons: [
          {
            src: 'statics/icons/icon-128x128.png',
            sizes: '128x128',
            type: 'image/png',
          },
          {
            src: 'statics/icons/icon-192x192.png',
            sizes: '192x192',
            type: 'image/png',
          },
          {
            src: 'statics/icons/icon-256x256.png',
            sizes: '256x256',
            type: 'image/png',
          },
          {
            src: 'statics/icons/icon-384x384.png',
            sizes: '384x384',
            type: 'image/png',
          },
          {
            src: 'statics/icons/icon-512x512.png',
            sizes: '512x512',
            type: 'image/png',
          },
        ],
      },
    },

    // https://quasar.dev/quasar-cli/developing-cordova-apps/configuring-cordova
    cordova: {
      // id: 'org.upgradedgiggle.atlascity.app',
      // noIosLegacyBuildFlag: true, // uncomment only if you know what you are doing
    },

    // https://quasar.dev/quasar-cli/developing-electron-apps/configuring-electron
    electron: {
      // bundler: 'builder', // or 'packager'

      extendWebpack(cfg) {
        // do something with Electron main process Webpack cfg
        // chainWebpack also available besides this extendWebpack
      },

      packager: {
        // https://github.com/electron-userland/electron-packager/blob/master/docs/api.md#options

        // OS X / Mac App Store
        // appBundleId: '',
        // appCategoryType: '',
        // osxSign: '',
        // protocol: 'myapp://path',

        // Windows only
        // win32metadata: { ... }
      },

      builder: {
        // https://www.electron.build/configuration/configuration

        // appId: 'upgraded-giggle'
      },
    },
  };
};

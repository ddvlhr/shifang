const { defineConfig } = require('@vue/cli-service')
const { resolve } = require('node:path')
module.exports = defineConfig({
  transpileDependencies: true,
  productionSourceMap: false,
  publicPath: '/',
  outputDir: 'ShiFang.Web',
  lintOnSave: false,
  devServer: {
    host: '0.0.0.0',
    port: 8234,
    https: false,
    open: false,
    hot: true,
    client: {
      overlay: {
        warnings: true
      }
    }
  },
  pluginOptions: {
    windicss: {}
  },
  chainWebpack: (config) => {
    config.plugin('html').tap((args) => {
      args[0].title = '数据采集与分析系统'
      return args
    })

    config.module.rule('svg').exclude.add(resolve('src/assets/icons')).end()

    config.module
      .rule('icons')
      .test(/\.svg$/)
      .include.add(resolve('src/assets/icons'))
      .end()
      .use('svg-sprite-loader')
      .loader('svg-sprite-loader')
      .options({
        symbolId: 'icon-[name]'
      })
  }
})

/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-07-28 16:24:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-29 00:40:46
 * @FilePath: /frontend/src/main.js
 * @Description: 主入口
 */
// 初始化样式
import 'normalize.css'
// 导入主样式表
import '@/styles/index.scss'
import 'windicss'
import Vue from 'vue'
import App from './App.vue'
// 引入 svg-icon 组件和 svg 图标
import '@/assets/icons'
import router from './router'
import store from './store'
import Api from './api'
import NProgress from 'nprogress'
import 'bootstrap/dist/css/bootstrap.css'
import Print from 'vue-print-nb'
// 引入 element-ui 插件
import { setupElementUI } from '@/plugins/element-ui'
// 引入 vue-ele-table 插件
import { setupEleTable } from '@/plugins/vue-ele-table'
// 引入 vue-ele-form 插件
import { setupEleForm } from '@/plugins/vue-ele-form'

// 引入 router-tab 插件
import { setupRouterTab } from '@/plugins/router-tab'

import { setupCustomComponents } from '@/plugins/custom-components'

import { setupVueEcharts } from '@/plugins/vue-echarts'

import 'windi.css'
import './permission'

Vue.prototype.$api = Api
Vue.prototype.$np = NProgress

setupCustomComponents()

Vue.config.productionTip = false
Vue.use(Print)
setupElementUI()
setupEleForm()
setupEleTable()
setupRouterTab()
setupVueEcharts()

let vueThis = new Vue({
  router,
  store,
  render: (h) => h(App)
}).$mount('#app')

export default vueThis

/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-26 22:25:39
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-26 22:25:55
 * @FilePath: /frontend/src/plugins/router-tab/index.js
 * @Description: 
 */
import Vue from 'vue'
import RouterTab from 'vue-router-tab'
import 'vue-router-tab/dist/lib/vue-router-tab.css'

export const setupRouterTab = () => {
  Vue.use(RouterTab)
}

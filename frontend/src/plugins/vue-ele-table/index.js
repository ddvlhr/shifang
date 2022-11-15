/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-26 20:44:05
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-10-26 20:50:16
 * @FilePath: /frontend/src/plugins/vue-ele-table/index.js
 * @Description: vue-ele-table 插件
 */
import Vue from 'vue'
import EleTable from 'vue-ele-table'

export const setupEleTable = () => {
  Vue.use(EleTable, {
    defaultSize: 10,
    paramsKey: {
      page: 'pageNum',
      size: 'pageSize'
    }
  })
}

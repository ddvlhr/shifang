/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 20:29:50
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-01 11:11:37
 * @FilePath: /frontend/src/plugins/custom-components/index.js
 * @Description: 自定义组件插件
 */
import Vue from 'vue'
import FunctionButton from '@/components/FunctionButton'
import TreeTable from 'vue-table-with-tree-grid'
import QuerySelect from '@/components/QuerySelect'
import QueryInput from '@/components/QueryInput'

export const setupCustomComponents = () => {
  Vue.component('function-button', FunctionButton)
  Vue.component('query-select', QuerySelect)
  Vue.component('tree-table', TreeTable)
  Vue.component('query-input', QueryInput)
}

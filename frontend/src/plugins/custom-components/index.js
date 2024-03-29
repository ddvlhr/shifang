/*
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-28 20:29:50
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-25 02:39:26
 * @FilePath: /frontend/src/plugins/custom-components/index.js
 * @Description: 自定义组件插件
 */
import Vue from 'vue'
import FunctionButton from '@/components/FunctionButton'
import TreeTable from 'vue-table-with-tree-grid'
import QuerySelect from '@/components/QuerySelect'
import QueryInput from '@/components/QueryInput'
import QueryDatePicker from '@/components/QueryDatePicker'
import MeasureDataDialog from '@/components/MeasureDataDialog'
import StatisticDialog from '@/components/StatisticDialog'
import FullScreen from '@/components/FullScreen'
import MetricalPushData from '@/components/MetricalPushData'
import ManualMetricalPushData from '@/components/ManualMetricalPushData'
import OriginDataStatistic from '@/components/OriginDataStatistic'

export const setupCustomComponents = () => {
  Vue.component('function-button', FunctionButton)
  Vue.component('query-select', QuerySelect)
  Vue.component('tree-table', TreeTable)
  Vue.component('query-input', QueryInput)
  Vue.component('query-date-picker', QueryDatePicker)
  Vue.component('measure-data-dialog', MeasureDataDialog)
  Vue.component('statistic-dialog', StatisticDialog)
  Vue.component('full-screen', FullScreen)
  Vue.component('metrical-push-data', MetricalPushData)
  Vue.component('manual-metrical-push-data', ManualMetricalPushData)
  Vue.component('origin-data-statistic', OriginDataStatistic)
}

import Vue from 'vue'
import Echarts from 'vue-echarts'
import 'echarts'

Vue.component('v-chart', Echarts)

export const setupVueEcharts = () => {
  Vue.use(Echarts)
}

<template>
  <el-collapse v-if="haveData" accordion>
    <el-collapse-item
      v-for="(item, index) in dataList"
      :key="index"
      :title="item.specificationName"
    >
      <template slot="title">
        <div style="width: 100%">
          <el-row
            :gutter="20"
            style="height: 100%; line-height: 100%; width: 100%"
          >
            <el-col :span="4">牌号: {{ item.specificationName }}</el-col>
            <el-col :span="4">平均值: {{ item.mean }} mmWG</el-col>
            <el-col :span="4">总数: {{ item.total }}</el-col>
            <el-col :span="4">合格数: {{ item.qualityCount }}</el-col>
            <el-col :span="4">合格率: {{ item.rate }}</el-col>
          </el-row>
        </div>
      </template>
      <div>
        <div style="margin: 10px 0">
          <el-radio-group v-model="chartType" @input="changeChartType">
            <el-radio-button label="柱状图"></el-radio-button>
            <el-radio-button label="折线图"></el-radio-button>
            <el-radio-button label="饼图"></el-radio-button>
          </el-radio-group>
        </div>
        <v-chart
          class="chart"
          :class="'sp' + item.specificationId"
          :option="option"
          autoresize
        />
      </div>
    </el-collapse-item>
  </el-collapse>
  <el-empty description="暂无数据" v-else />
</template>

<script>
import * as echarts from 'echarts'
import VChart, { THEME_KEY } from 'vue-echarts'
export default {
  components: {
    VChart
  },
  provide: {
    [THEME_KEY]: 'light'
  },
  props: {
    dataList: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      chartType: '柱状图',
      option: {
        title: {
          text: '数据图表'
        },
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'cross',
            label: {
              backgroupColor: '#6a7985'
            }
          }
        },
        toolbox: {
          show: true,
          feature: {
            mark: { show: true },
            dataView: { show: true, readonly: false },
            restore: { show: true },
            saveAsImage: { show: true }
          }
        },
        legend: {
          show: false,
          data: []
        },
        xAxis: {
          type: 'category',
          data: []
        },
        yAxis: {
          max: 100,
          min: 0
        },
        series: [
          {
            name: '压降',
            type: 'bar',
            data: [],
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
              }
            },
            markLine: {}
          }
        ]
      }
    }
  },
  computed: {
    haveData() {
      return this.dataList.length > 0
    }
  },
  watch: {
    dataList(newVal) {
      if (newVal.length > 0) {
        setTimeout(() => {
          this.updateCharts()
        }, 1000)
      }
    }
  },
  created() {},
  methods: {
    changeChartType() {
      this.updateCharts()
    },
    updateCharts() {
      const chartType =
        this.chartType === '柱状图'
          ? 'bar'
          : this.chartType === '折线图'
          ? 'line'
          : 'pie'
      this.dataList.forEach((item) => {
        const rule = item.rule
        const lower = parseFloat(rule.lower)
        const upper = parseFloat(rule.upper)
        const standard = parseFloat(rule.standard)
        const dom = document.querySelector(`.sp${item.specificationId}`)
        const chart = echarts.getInstanceByDom(dom)
        const tempOption = JSON.parse(JSON.stringify(this.option))
        const xAxis = []

        const data = item.list
        tempOption.title.text = '压降'
        tempOption.series[0].name = '压降'
        if (chartType !== 'pie') {
          tempOption.legend.data.unshift('压降')
          if (data.length > 0) {
            tempOption.series[0].data = data
          }
          let min = Math.min(...data)
          let max = Math.max(...data)

          min = min < lower ? min : lower
          max = max > upper ? max : upper
          tempOption.yAxis.min = (min * 0.99).toFixed(2)
          tempOption.yAxis.max = (max * 1.01).toFixed(2)
          for (var i = 1; i <= data.length; i++) {
            xAxis.push(i)
          }
          const itemStyle = {
            color: function (value) {
              if (value.data > upper) {
                return 'rgba(255, 0, 0, .8)'
              } else if (value.data < lower) {
                return 'rgba(21, 98, 203, .8)'
              } else {
                return 'rgba(52, 212, 65, .8)'
              }
            }
          }
          tempOption.series[0].itemStyle = itemStyle
        }

        tempOption.series[0].type = chartType
        if (this.chartType === '柱状图') {
          const visualMap = {
            top: 10,
            right: 'center',
            orient: 'horizontal',
            precision: 3,
            pieces: [
              {
                gt: upper,
                color: 'rgba(255, 0, 0, .8)'
              },
              {
                lte: upper,
                gte: lower,
                color: 'rgba(52, 212, 65, .8)'
              },
              {
                lt: lower,
                color: 'rgba(21, 98, 203, .8)'
              }
            ]
          }
          tempOption.visualMap = visualMap
        }
        const markLine = {
          data: [
            {
              name: '上限值',
              lineStyle: {
                type: 'max',
                color: 'rgba(255, 0, 0, .8)'
              },
              yAxis: upper
            },
            {
              name: '标准值',
              lineStyle: {
                type: 'medium',
                color: 'rgba(52, 212, 65, .8)'
              },
              yAxis: standard
            },
            {
              name: '下限值',
              lineStyle: {
                type: 'min',
                color: 'rgba(21, 98, 203, .8)'
              },
              yAxis: lower
            }
          ]
        }
        if (chartType !== 'pie') {
          tempOption.series[0].markLine = markLine
          tempOption.xAxis.data = xAxis
        } else {
          tempOption.series[0].radius = '50%'
          tempOption.series[0].data = [
            { value: item.qualityCount, name: '合格数' },
            { value: item.total - item.qualityCount, name: '不合格数' }
          ]
          tempOption.legend = {
            orient: 'horizontal',
            left: 'center',
            data: ['合格数', '不合格数'],
            show: true
          }
          tempOption.tooltip = {
            trigger: 'item'
          }
          tempOption.xAxis = null
          tempOption.yAxis = null
          chart.clear()
        }

        chart.setOption(tempOption)
      })
    }
  }
}
</script>

<style lang="scss" scoped>
.chart {
  width: 100% !important;
  height: 440px;
}
</style>

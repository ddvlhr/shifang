<template>
  <div class="main-container">
    <function-button @search="search" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="4">
          <query-select
            v-model="specificationIds"
            :options="specificationOptions"
            :multiple="true"
            placeholder="请选择牌号"
          />
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.turnId"
            :options="turnOptions"
            placeholder="请选择班次"
          />
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.machineId"
            :options="machineOptions"
            placeholder="请选择机台"
          />
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="excludeDate"
            type="dates"
            placeholder="数据日期筛选"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.statisticType"
            :options="statisticTypeOptions"
            placeholder="请选择统计值"
          />
        </el-col>
      </el-row>
      <el-row :gutter="10" style="margin-top: 15px">
        <el-col :span="6" :offset="0">
          <el-date-picker
            v-model="dateRange"
            @change="getDateRange"
            type="daterange"
            size="normal"
            range-separator="-"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-col>
      </el-row>
      <div
        v-loading="chartLoading"
        :style="chartLoading ? 'height: 300px;' : ''"
      >
        <div
          style="width: 100%; min-height: 600px; margin-top: 15px"
          id="weightScatterPlotArea"
        ></div>
        <div
          style="width: 100%; min-height: 600px; margin-top: 15px"
          id="circleScatterPlotArea"
        ></div>
        <div
          style="
            width: 100%;
            min-height: 600px;
            margin-top: 15px;
            border: 1px solid red;
          "
          id="ovalScatterPlotArea"
        ></div>
        <div
          style="width: 100%; min-height: 600px; margin-top: 15px"
          id="lengthScatterPlotArea"
        ></div>
        <div
          style="width: 100%; min-height: 600px; margin-top: 15px"
          id="resistanceScatterPlotArea"
        ></div>
        <div
          style="width: 100%; min-height: 600px; margin-top: 15px"
          id="hardnessScatterPlotArea"
        ></div>
      </div>
    </el-card>
  </div>
</template>

<script>
import * as echarts from 'echarts'
import { getAverage, reloadCurrentRoute } from '@/utils/utils'
export default {
  components: { QuerySelect },
  data() {
    return {
      queryInfo: {
        specificationId: '',
        turnId: '',
        machineId: '',
        beginDate: '',
        endDate: '',
        excludeDate: '',
        statisticType: ''
      },
      specificationIds: [],
      specificationOptions: [],
      turnOptions: [],
      machineOptions: [],
      statisticTypeOptions: [],
      dateRange: [],
      chartLoading: false,
      excludeDate: [],
      options: {
        title: {
          text: '滤棒统计信息',
          left: 'center'
        },
        yAxis: {},
        xAxis: {},
        tooltip: {
          position: 'top'
        },
        legend: {
          orient: 'vertical',
          left: 0,
          top: 'center'
        },
        toolbox: {
          feature: {
            dataView: { show: true, readonly: true },
            restore: { show: true },
            saveAsImage: { show: true }
          }
        },
        dataZoom: [
          {
            show: true,
            realtime: true,
            start: 0,
            end: 100
          },
          {
            type: 'inside',
            realtime: true,
            start: 0,
            end: 100
          }
        ],
        series: []
      }
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getOptions()
  },
  methods: {
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项信息失败: ' + res.meta.message)
      }
      this.specificationOptions = res.data.specifications
      this.turnOptions = res.data.turns
      this.machineOptions = res.data.machineModels
      this.statisticTypeOptions = res.data.filterStatisticPlotTypes
    },
    getDateRange() {
      if (this.dateRange == null || Object.keys(this.dateRange).length === 0) {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      } else {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      }
    },
    getSeries(name, data, type) {
      const absList = []
      data.forEach((item) => {
        var tempData = Math.abs(Number(item[1]))
        absList.push(tempData)
      })
      const avg = getAverage(absList)
      const meanDis = [
        {
          name: '平均值',
          yAxis: avg.toFixed(3)
        }
      ]
      const subtext = `${name}: ${avg.toFixed(3)}`
      const temp = {
        name: name,
        symbolSize: 15,
        encode: { tooltip: [0, 1] },
        data: data,
        type: 'scatter',
        markLine: {
          data:
            type.text === '绝偏'
              ? meanDis
              : [{ type: 'average', name: '平均值' }],
          precision: 3
        }
      }
      return [subtext, temp]
    },
    async search() {
      if (this.queryInfo.statisticType === '') {
        return this.$message.error('请选择统计指标')
      }

      if (this.excludeDate.length > 0) {
        this.queryInfo.excludeDate = this.excludeDate.join(',')
      }
      const weightDom = document.querySelector('#weightScatterPlotArea')
      const weightChart = echarts.init(weightDom)
      weightChart.clear()
      const circleDom = document.querySelector('#circleScatterPlotArea')
      const circleChart = echarts.init(circleDom)
      circleChart.clear()
      const ovalDom = document.querySelector('#ovalScatterPlotArea')
      const ovalChart = echarts.init(ovalDom)
      ovalChart.clear()
      const lengthDom = document.querySelector('#lengthScatterPlotArea')
      const lengthChart = echarts.init(lengthDom)
      lengthChart.clear()
      const resistanceDom = document.querySelector('#resistanceScatterPlotArea')
      const resistanceChart = echarts.init(resistanceDom)
      resistanceChart.clear()
      const hardnessDom = document.querySelector('#hardnessScatterPlotArea')
      const hardnessChart = echarts.init(hardnessDom)
      hardnessChart.clear()
      this.queryInfo.specificationId = this.specificationIds.join(',')
      this.chartLoading = true
      const { data: res } = await this.$api.getFilterStatisticScatterPlot(
        this.queryInfo
      )
      const type = this.statisticTypeOptions.find(
        (e) => e.value === this.queryInfo.statisticType
      )
      this.options.xAxis.data = res.data.ids
      if (res.data.weight.length > 0) {
        const weightOption = this.options
        const serieses = []
        const subtextArr = []
        res.data.weight.forEach((item) => {
          const series = this.getSeries(item.specificationName, item.data, type)
          subtextArr.push(series[0])
          serieses.push(series[1])
        })
        weightOption.series = serieses
        weightOption.title.text = `滤棒重量 ${type.text} 值散点图`
        weightOption.title.subtext = subtextArr.join(' ')
        console.log(weightOption)
        weightChart.setOption(weightOption)
      }

      if (res.data.circle.length > 0) {
        const circleOption = this.options
        const serieses = []
        const subtextArr = []
        res.data.circle.forEach((item) => {
          const series = this.getSeries(item.specificationName, item.data, type)
          subtextArr.push(series[0])
          serieses.push(series[1])
        })
        circleOption.series = serieses
        circleOption.title.text = `滤棒圆周 ${type.text} 值散点图`
        circleOption.title.subtext = subtextArr.join(' ')
        circleChart.setOption(circleOption)
      }

      if (res.data.oval.length > 0) {
        const ovalOption = this.options
        const serieses = []
        const subtextArr = []
        res.data.oval.forEach((item) => {
          const series = this.getSeries(item.specificationName, item.data, type)
          subtextArr.push(series[0])
          serieses.push(series[1])
        })
        ovalOption.series = serieses
        ovalOption.title.text = `滤棒圆度 ${type.text} 值散点图`
        ovalOption.title.subtext = subtextArr.join(' ')
        ovalChart.setOption(ovalOption)
      }

      if (res.data.length.length > 0) {
        const lengthOption = this.options
        const serieses = []
        const subtextArr = []
        res.data.length.forEach((item) => {
          const series = this.getSeries(item.specificationName, item.data, type)
          subtextArr.push(series[0])
          serieses.push(series[1])
        })
        lengthOption.series = serieses
        lengthOption.title.text = `滤棒长度 ${type.text} 值散点图`
        lengthOption.title.subtext = subtextArr.join(' ')
        lengthChart.setOption(lengthOption)
      }

      if (res.data.resistance.length > 0) {
        const resistanceOption = this.options
        const serieses = []
        const subtextArr = []
        res.data.resistance.forEach((item) => {
          const series = this.getSeries(item.specificationName, item.data, type)
          subtextArr.push(series[0])
          serieses.push(series[1])
        })
        resistanceOption.series = serieses
        resistanceOption.title.text = `滤棒压降 ${type.text} 值散点图`
        resistanceOption.title.subtext = subtextArr.join(' ')
        resistanceChart.setOption(resistanceOption)
      }

      if (res.data.hardness.length > 0) {
        const hardnessOption = this.options
        const serieses = []
        const subtextArr = []
        res.data.hardness.forEach((item) => {
          const series = this.getSeries(item.specificationName, item.data, type)
          subtextArr.push(series[0])
          serieses.push(series[1])
        })
        hardnessOption.series = serieses
        hardnessOption.title.text = `滤棒硬度 ${type.text} 值散点图`
        hardnessOption.title.subtext = subtextArr.join(' ')
        hardnessChart.setOption(hardnessOption)
      }
      this.chartLoading = false
    }
  }
}
</script>

<style scoped>
.el-date-editor {
  width: 100%;
}
</style>

<template>
  <div class="main-container">
    <function-button @search="search" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6">
          <query-select
            :options="specificationTypeOptions"
            v-model="queryInfo.specificationTypeId"
            placeholder="牌号类型筛选"
          />
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            value-format="yyyy-MM-dd"
            range-separator="至"
            @change="getDateRange"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          >
          </el-date-picker>
        </el-col>
        <el-col :span="4">
          <el-input v-model="yMin" placeholder="y轴坐标最小值" >
             <template slot="prepend"> y轴坐标最小值</template>
          </el-input>
        </el-col>
      </el-row>
      <div id="chart" style="width: 100%; min-height: 600px; margin-top: 15px;"></div>
    </el-card>
  </div>
</template>

<script>
import * as echarts from 'echarts'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    const that = this
    console.log(this.$store.getters.getProductWaterYMin)
    return {
      queryInfo: {
        specificationTypeId: '',
        beginDate: '',
        endDate: ''
      },
      yMin: that.$store.getters.getProductWaterYMin,
      dateRange: [],
      specificationTypeOptions: [],
      options: {
        title: {
          text: '成品水分统计',
          left: 'center'
        },
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'cross',
            label: {
              backgroundColor: '#6a7958'
            }
          }
        },
        toolbox: {
          feature: {
            dataView: { show: true, readonly: true },
            restore: { show: true },
            saveAsImage: { show: true }
          }
        },
        legend: {
          top: 30,
          textStyle: {
            fontSize: 16
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
        xAxis: {
          type: 'category',
          axisTick: {
            alignWithLabel: true
          }
        },
        yAxis: {
          type: 'value',
          min: function(value) {
            return that.yMin
          },
          max: function(value) {
            return value.max + 0.5
          }
        },
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
      this.specificationTypeOptions = res.data.specificationTypes
    },
    getDateRange() {
      if (this.dateRange != null) {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      } else {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      }
    },
    async search() {
      if (this.queryInfo.specificationTypeId === '') {
        return this.$message.error('请选择牌号类型')
      }
      const chart = echarts.init(document.querySelector('#chart'))
      chart.clear()
      const { data: res } = await this.$api.getProductWaterStatisticInfo(this.queryInfo)
      if (res.meta.code !== 0) {
        return this.$message.error('获取成品水分统计分析信息失败: ' + res.meta.message)
      }
      if (res.data.productWaterInfos.length === 0 || res.data.timeX.length === 0) {
        return this.$message.info('没有找到对应的数据')
      }
      this.$store.commit('setProductWaterYMin', this.yMin)
      const legend = []
      const series = []
      res.data.productWaterInfos.forEach(item => {
        series.push({
          name: item.specificationName,
          type: 'line',
          data: item.waterInfos
        })
        legend.push(item.specificationName)
      })
      this.options.legend.data = legend
      this.options.xAxis.data = res.data.timeX
      this.options.series = series
      this.options.title.subtext = res.data.rates.join('')
      console.log(this.options)
      chart.setOption(this.options)
    }
  }
}
</script>

<style scoped>
.el-date-editor {
  width: 100%;
}
</style>

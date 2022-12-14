<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-31 09:56:45
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-02 15:54:58
 * @FilePath: /frontend/src/views/Dashboard.vue
 * @Description: Dashboard
-->
<template>
  <div class="main-container">
    <el-row :gutter="20">
      <el-col :span="18" :offset="0">
        <el-card shadow="never" :body-style="{ padding: '20px' }">
          <div slot="header">
            <span>历史测量数据</span>
            <el-button
              style="float: right; padding: 3px 0"
              type="text"
              @click="getMetricalDataInfo(3)"
              >年度</el-button
            >
            <el-button
              style="float: right; padding: 3px 5px"
              type="text"
              @click="getMetricalDataInfo(2)"
              >月度</el-button
            >
            <el-button
              style="float: right; padding: 3px 5px"
              type="text"
              @click="getMetricalDataInfo(1)"
              >周</el-button
            >
          </div>
          <v-chart class="chart" :option="option" />
        </el-card>
      </el-col>
      <el-col :span="6" :offset="0">
        <el-row :gutter="20">
          <el-col :span="24" :offset="0">
            <el-card shadow="never" :body-style="{ padding: '20px' }">
              <div slot="header">
                <span>系统信息</span>
              </div>
              <div class="text-center">
                <!-- <p>系统版本: {{ serverInfo.Version }}</p> -->
                <p>CPU使用率: {{ serverInfo.CpuUsage }}</p>
                <p>系统内存: {{ serverInfo.TotalRam }}</p>
                <el-row :gutter="20">
                  <el-col :span="12" :offset="0">
                    <el-progress
                      type="dashboard"
                      :percentage="serverInfo.CpuRate"
                      :color="colors"
                      :stroke-width="12"
                    >
                    </el-progress>
                    <p>CPU占用率</p>
                  </el-col>
                  <el-col :span="12" :offset="0">
                    <el-progress
                      type="dashboard"
                      :percentage="serverInfo.RamRate"
                      :color="colors"
                      :stroke-width="12"
                    >
                    </el-progress>
                    <p>内存占用率</p></el-col
                  >
                </el-row>
              </div>
            </el-card>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="24" :offset="0">
            <el-card shadow="never" :body-style="{ padding: '20px' }">
              <div slot="header">
                <span>LocalStorage 使用存储空间</span>
              </div>
              {{ localStorageSize }} / 5101 KB
            </el-card>
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="24" :offset="0">
            <el-card shadow="never" :body-style="{ padding: '20px' }">
              <div slot="header">
                <span>SessionStorage 存储使用空间</span>
              </div>
              {{ sessionStorageSize }} / 5101 KB
            </el-card></el-col
          >
        </el-row>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import VChart, { THEME_KEY } from 'vue-echarts'
export default {
  components: {
    VChart
  },
  provide: {
    [THEME_KEY]: 'light'
  },
  data() {
    return {
      colors: [
        { color: '#5cb87a', percentage: 20 },
        { color: '#1989fa', percentage: 40 },
        { color: '#6f7ad3', percentage: 60 },
        { color: '#e6a23c', percentage: 80 },
        { color: '#f56c6c', percentage: 100 }
      ],
      option: {
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'cross'
          }
        },
        legend: {
          data: ['测量组数', '测量总数']
        },
        calculable: true,
        xAxis: [
          {
            type: 'category',
            axisTick: {
              alignWithLabel: true
            },
            data: []
          }
        ],
        yAxis: [
          {
            type: 'value',
            name: '测量组数',
            position: 'left',
            alignTicks: true
          },
          {
            type: 'value',
            name: '测量总数',
            position: 'right',
            alignTicks: true
          }
        ],
        series: [
          {
            name: '测量组数',
            type: 'bar',
            data: []
          },
          {
            name: '测量总数',
            type: 'line',
            yAxisIndex: 1,
            data: []
          }
        ]
      }
    }
  },
  created() {
    this.$store.dispatch('app/setSystemCacheSize')
    this.getMetricalDataInfo(3)
  },
  computed: {
    localStorageSize() {
      return this.$store.state.app.localStorageSize
    },
    sessionStorageSize() {
      return this.$store.state.app.sessionStorageSize
    },
    serverInfo() {
      return this.$store.state.app.serverInfo
    }
  },
  methods: {
    async getMetricalDataInfo(type) {
      const { data: res } = await this.$api.getMetricalDataInfo(type)
      if (res.meta.code !== 0) {
        return this.$message.error('获取测量数据信息失败: ' + res.meta.message)
      }
      const times = res.data.map((item) => {
        let fix = ' 时'
        switch (type) {
          case 1:
            fix = ' 时'
            break
          case 2:
            fix = ' 日'
            break
          case 3:
            fix = ' 月'
            break
        }

        return item.name + fix
      })
      const groupTotalList = res.data.map((item) => item.groupTotal)
      const dataTotalList = res.data.map((item) => item.dataTotal)
      this.option.xAxis[0].data = times
      this.option.series[0].data = groupTotalList
      this.option.series[1].data = dataTotalList
    }
  }
}
</script>

<style lang="scss" scoped>
#message {
  overflow-y: auto;
  text-align: left;
  border: #42b983 solid 1px;
  height: 500px;
}
.chart {
  width: 100% !important;
  height: 600px;
}
</style>

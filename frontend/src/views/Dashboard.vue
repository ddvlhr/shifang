<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-10-31 09:56:45
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-10-10 08:37:03
 * @FilePath: \frontend\src\views\Dashboard.vue
 * @Description: Dashboard
-->
<template>
  <div class="main-container">
    <el-tabs type="card">
      <el-tab-pane label="历史测量数据">
        <el-card shadow="never" :body-style="{ padding: '20px' }">
          <div slot="header">
            <el-button
              style="padding: 3px 0px"
              type="text"
              @click="getMetricalDataInfo(3)"
              >年度</el-button
            >
            <el-button
              style="padding: 3px 0px"
              type="text"
              @click="getMetricalDataInfo(2)"
              >月度</el-button
            >
            <el-button
              style="padding: 3px 0px"
              type="text"
              @click="getMetricalDataInfo(1)"
              >周度</el-button
            >
          </div>
          <v-chart class="chart" :option="option" autoresize />
        </el-card>
      </el-tab-pane>
      <el-tab-pane label="测量数据推送">
        <el-card
          id="metricalPushData"
          shadow="never"
          :body-style="{ padding: '20px' }"
        >
          <div slot="header">
            <el-row :gutter="20">
              <el-col :span="6" :offset="0">
                <query-select
                  v-model="machineId"
                  :options="machineOptions"
                  placeholder="请选择机台"
                />
              </el-col>
              <el-col :span="4">
                <el-button
                  type="primary"
                  @click="metricalDataPush"
                  v-if="pushDataState"
                  >开启推送</el-button
                >
                <el-button type="danger" @click="stopMetricalDataPush" v-else
                  >停止推送</el-button
                >
              </el-col>
              <el-col :span="4" :offset="10">
                <el-button
                  icon="el-icon-full-screen"
                  type="primary"
                  @click="metricalPushDataFullscreen"
                  class="items-center"
                  >全屏/退出全屏</el-button
                >
              </el-col>
            </el-row>
            <el-row class="mt-3">
              <el-col :span="8" style="vertical-align: middle">
                <el-tag type="dark">
                  当前推送数据测量时间:
                  {{
                    metricalPushData.length > 0
                      ? metricalPushData[0].testTime
                      : ''
                  }}
                </el-tag>
              </el-col>
            </el-row>
          </div>
          <metrical-push-data :showChart="true" :tabCharts="false" />
        </el-card>
      </el-tab-pane>
      <el-tab-pane label="车间测量数据推送">
        <el-card
          id="workshopMetricalPushData"
          shadow="never"
          :body-style="{ padding: '20px' }"
          style="max-height: 1080px; overflow-y: auto"
        >
          <div slot="header">
            <el-row :gutter="10">
              <el-col :span="6" :offset="0">
                <query-select
                  v-model="workShopId"
                  :options="workshopOptions"
                  placeholder="请选择手工车间"
                />
              </el-col>
              <el-col :span="4">
                <el-button
                  type="primary"
                  @click="workshopMetricalDataPush"
                  v-if="workshopPushDataState"
                  >开启推送</el-button
                >
                <el-button
                  type="danger"
                  @click="stopWorkshopMetricalDataPush"
                  v-else
                  >停止推送</el-button
                >
              </el-col>
              <el-col :span="4" :offset="10">
                <el-button
                  icon="el-icon-full-screen"
                  type="primary"
                  @click="workshopMetricalPushDataFullscreen"
                  class="items-center"
                  >全屏/退出全屏</el-button
                >
              </el-col>
            </el-row>
            <el-row class="mt-3">
              <el-col :span="8" style="vertical-align: middle">
                <el-tag type="dark">
                  当前推送数据测量时间:
                  {{
                    workshopMetricalPushData.length > 0
                      ? workshopMetricalPushData[0].testTime
                      : ''
                  }}
                </el-tag>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="16" style="height: 570px; overflow-y: auto">
                <!--                推送统计信息组件还未更改为 选择车间推送-->
                <manual-metrical-push-data
                  :showChart="true"
                  :tabCharts="false"
                />
              </el-col>
              <el-col :span="8">
                <el-table height="570" :data="manualCheckerInfo" border>
                  <el-table-column type="index" label="#"></el-table-column>
                  <el-table-column label="工号" prop="no"></el-table-column>
                  <el-table-column label="数量" prop="count"></el-table-column>
                </el-table>
              </el-col>
            </el-row>
          </div>
          <div>
            <el-button
              style="padding: 3px 0px"
              type="text"
              @click="getManualMetricalDataInfo(1)"
              >天</el-button
            >
            <el-button
              style="padding: 3px 0px"
              type="text"
              @click="getManualMetricalDataInfo(2)"
              >周</el-button
            >
            <el-button
              style="padding: 3px 0px"
              type="text"
              @click="getManualMetricalDataInfo(3)"
              >月</el-button
            >
          </div>
          <v-chart class="chart" :option="workshopOption" autoresize />
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import VChart, { THEME_KEY } from 'vue-echarts'
import sr from '@/utils/signalR'
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
      },
      workshopOption: {
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
      },
      workshopOptions: [],
      workShopId: '',
      machineOptions: [],
      machineId: '',
      tableDesc: {},
      columnsDesc: {
        number: {
          text: '数量'
        },
        userName: {
          text: '检验员'
        }
      },
      manualCheckerInfo: [],
      queryInfo: {
        specificationId: '',
        turnId: '',
        machineId: '',
        pageNum: 1,
        pageSize: 10
      },
      dataList: []
    }
  },
  created() {
    this.$store.dispatch('app/setSystemCacheSize')
    this.getMetricalDataInfo(2)
    // this.loadDataList()
    this.getOptions()
    this.getHandicraftWorkshop()
    this.machineId = this.$store.state.user.metricalPushDataMachine
    this.workShopId = this.$store.state.user.metricalPushDataWorkShop
  },
  mounted() {
    this.initSignalR()
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
    },
    metricalPushData() {
      return this.$store.state.user.metricalPushData
    },
    workshopMetricalPushData() {
      return this.$store.state.user.workshopMetricalData
    },
    pushDataState() {
      return this.$store.state.user.metricalPushDataState === false
    },
    workshopPushDataState() {
      return this.$store.state.user.workshopMetricalDataState === false
    }
  },
  watch: {
    workshopMetricalPushData(newVal) {
      if (newVal.length > 0) {
        this.getManualMetricalDataInfo(3)
        this.getManualCheckerInfos()
      }
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
    },
    async getManualMetricalDataInfo(type) {
      const { data: res } = await this.$api.getManualMetricalDataInfo(
        type,
        this.workShopId
      )
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
            fix = ' 日'
            break
        }

        return item.name + fix
      })
      const groupTotalList = res.data.map((item) => item.groupTotal)
      const dataTotalList = res.data.map((item) => item.dataTotal)
      this.workshopOption.xAxis[0].data = times
      this.workshopOption.series[0].data = groupTotalList
      this.workshopOption.series[1].data = dataTotalList
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败: ' + res.meta.message)
      }
      // this.workshopOptions = res.data.workShops
      this.machineOptions = res.data.machines
    },
    async getHandicraftWorkshop() {
      const { data: res } = await this.$api.getHandicraftWorkshop()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败：' + res.meta.message)
      }

      console.log(res.data)
      const options = res.data.map((item) => {
        return { text: item.name, value: item.letters }
      })
      this.workshopOptions = options
      console.log(this.workshopOptions)
    },
    initSignalR() {
      const metricalPushDataState = this.$store.state.user.metricalPushDataState
      const workshopPushDataState =
        this.$store.state.user.workshopMetricalDataState
      if (!sr.connection || sr.connection.state === 'Disconnected') {
        sr.init(
          this.$utils.getCurrentApiUrl(process.env.NODE_ENV === 'development') +
            '/ServerHub',
          this.$store.state.user.userInfo,
          0
        )
      }
      if (metricalPushDataState && sr.connection.state === 'Connected') {
        const connectionId = sr.connection.connectionId
        const machineId = this.$store.state.user.metricalPushDataMachine
        sr.send('LoginPushData', [connectionId, machineId])
      } else if (workshopPushDataState && sr.connection.state === 'Connected') {
        const connectionId = sr.connection.connectionId
        const workshopName = this.$store.state.user.metricalPushDataWorkShop
        sr.send('LoginManualPushData', [connectionId, workshopName])
      } else {
        setTimeout(() => {
          this.initSignalR()
        }, 500)
      }
    },
    metricalDataPush() {
      if (!this.machineId) {
        return this.$message.error('请选择机台再开启推送')
      }
      this.$store.dispatch('user/clearMetricalPushData')
      // 初始化SignalR, 在登录和刷新页面时调用, 判断是否需要初始化, 防止重复初始化
      if (!sr.connection || sr.connection.state === 'Disconnected') {
        sr.init(
          this.$utils.getCurrentApiUrl(process.env.NODE_ENV === 'development') +
            '/ServerHub',
          this.$store.state.user.userInfo,
          this.machineId
        )
      } else {
        const connectionId = sr.connection.connectionId
        sr.send('LoginPushData', [connectionId, this.machineId])
        this.$store.dispatch('user/setMetricalPushDataState', true)
        this.$store.dispatch('user/setMetricalPushDataMachine', this.machineId)
      }
    },
    // 车间推送
    workshopMetricalDataPush() {
      if (!this.workShopId) {
        return this.$message.error('请选择车间再开启推送')
      }
      this.getManualMetricalDataInfo(3)
      this.getManualCheckerInfos()
      this.$store.dispatch('user/clearWorkshopMetricalPushData')
      // 初始化SignalR, 在登录和刷新页面时调用, 判断是否需要初始化, 防止重复初始化
      if (!sr.connection || sr.connection.state === 'Disconnected') {
        sr.init(
          this.$utils.getCurrentApiUrl(process.env.NODE_ENV === 'development') +
            '/ServerHub',
          this.$store.state.user.userInfo,
          this.machineId
        )
      } else {
        const connectionId = sr.connection.connectionId
        sr.send('LoginManualPushData', [connectionId, this.workShopId])
        this.$store.dispatch('user/setWorkshopMetricalPushDataState', true)
        this.$store.dispatch(
          'user/setMetricalPushDataWorkshop',
          this.workShopId
        )
      }
    },
    async getManualCheckerInfos() {
      const { data: res } = await this.$api.getManualCheckerInfos(
        this.workShopId
      )
      if (res.meta.code !== 0) {
        return this.$message.error('获取信息失败：' + res.meta.message)
      }

      const list = res.data.sort((a, b) => {
        return b.count - a.count
      })
      this.manualCheckerInfo = list
    },
    compareNumbers(a, b) {
      return a - b
    },
    stopMetricalDataPush() {
      sr.send('LogoutPushData', [sr.connection.connectionId])
      this.$store.dispatch('user/setMetricalPushDataState', false)
    },
    stopWorkshopMetricalDataPush() {
      sr.send('LogoutPushData', [sr.connection.connectionId])
      this.$store.dispatch('user/setWorkshopMetricalPushDataState', false)
    },
    metricalPushDataFullscreen() {
      this.$utils.handleFullscreen('#metricalPushData')
    },
    workshopMetricalPushDataFullscreen() {
      this.$utils.handleFullscreen('#workshopMetricalPushData')
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

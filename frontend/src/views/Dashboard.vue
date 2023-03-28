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
      <el-tab-pane label="测量数据推送" style="height: calc(100vh - 200px)">
        <el-card shadow="never" :body-style="{ padding: '20px' }">
          <div slot="header">
            <el-row :gutter="20">
              <el-col :span="6">
                <query-select
                  :options="specificationOptions"
                  placeholder="请选择牌号"
                />
              </el-col>
              <el-col :span="6" :offset="0">
                <query-select :options="turnOptions" placeholder="请选择班次" />
              </el-col>
              <el-col :span="6" :offset="0">
                <query-select
                  :options="machineOptions"
                  placeholder="请选择机台"
                />
              </el-col>
              <el-col :span="6">
                <el-button type="primary">开启推送</el-button>
              </el-col>
            </el-row>
          </div>
          <el-row :gutter="20" style="height: 100%">
            <el-col :span="14" style="height: 100%">
              <el-table :data="dataList">
                <el-table-column
                  label="测量时间"
                  prop="beginTime"
                ></el-table-column>
                <el-table-column
                  label="牌号名称"
                  prop="specificationName"
                ></el-table-column>
                <el-table-column label="班次" prop="turnName"></el-table-column>
                <el-table-column
                  label="机台"
                  prop="machineName"
                ></el-table-column>
                <el-table-column
                  label="测量类型"
                  prop="measureTypeName"
                ></el-table-column>
              </el-table>
            </el-col>
            <el-col :span="10" style="height: 100%"></el-col>
          </el-row>
        </el-card>
      </el-tab-pane>
    </el-tabs>
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
      },
      specificationOptions: [],
      turnOptions: [],
      machineOptions: [],
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
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败: ' + res.meta.message)
      }
      this.specificationOptions = res.data.specifications
      this.turnOptions = res.data.turns
      this.machineOptions = res.data.machines
    },
    loadDataList() {
      let clear = false
      setInterval(async () => {
        // if (this.dataList.length > 10) {
        //   clear = true
        // }
        const { data: res } = await this.$api.getMetricalData(this.queryInfo)
        this.dataList = res.data.list
        // if (this.dataList.length > 10) {
        //   this.dataList.pop()
        //   // clear = false
        // }
        // this.dataList.unshift({
        //   beginTime: this.$utils.getCurrentTime(),
        //   specificationName: '牌号名称',
        //   turnName: '班次',
        //   machineName: '机台',
        //   measureTypeName: '测量类型'
        // })
      }, 5000)
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

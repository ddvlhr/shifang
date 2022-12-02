<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-24 15:21:40
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 21:58:51
 * @FilePath: /frontend/src/views/statistics/ManualTestStatistics.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <function-button @search="search" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6" :offset="0">
            <query-select
              :options="specificationOptions"
              v-model="queryInfo.specificationId"
              placeholder="牌号筛选"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-date-picker v-model="daterange" picker-type="daterange" />
          </el-col>
        </el-row>
      </div>
      <div class="statistic">
        <el-table :data="tableInfo" border>
          <el-table-column
            label="序号"
            type="index"
            width="200"
          ></el-table-column>
          <el-table-column label="判定" prop="result"></el-table-column>
          <el-table-column label="数量" prop="count"></el-table-column>
        </el-table>
        <v-chart class="chart" :option="option" />
      </div>
    </el-card>
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
      queryInfo: {
        begin: '',
        end: '',
        specificationId: ''
      },
      daterange: [],
      specificationOptions: [],
      teamOptions: [],
      turnOptions: [],
      machineOptions: [],
      tableInfo: [],
      option: {
        title: {
          text: '手工检验情况',
          left: 'center'
        },
        tooltip: {
          trigger: 'item',
          formatter: '{a} <br/>{b}: {c} ({d}%)'
        },
        toolbox: {
          show: true,
          feature: {
            mark: { show: true },
            dataView: { show: true, readOnly: false },
            restore: { show: true },
            saveAsImage: { show: true }
          }
        },
        legend: {
          top: '10%',
          left: 'center',
          data: ['优质品', '一等品', '不合格品']
        },
        series: [
          {
            name: '手工检验情况',
            type: 'pie',
            radius: '50%',
            label: {
              show: true,
              formatter: '{b}: {c} ({d}%)'
            },
            emphasis: {
              label: {
                show: true,
                fontSize: '30',
                fontWeight: 'bold'
              },
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
              }
            },
            data: []
          }
        ]
      }
    }
  },
  created() {
    this.getOptions()
  },
  watch: {
    daterange(val) {
      if (val !== null) {
        this.queryInfo.begin = val[0]
        this.queryInfo.end = val[1]
      } else {
        this.queryInfo.begin = ''
        this.queryInfo.end = ''
      }
    }
  },
  methods: {
    async getOptions() {
      Promise.all([
        this.$api.getSpecificationOptions(),
        this.$api.getTeamOptions(),
        this.$api.getTurnOptions(),
        this.$api.getMachineOptions()
      ]).then((res) => {
        this.specificationOptions = res[0].data.data
        this.teamOptions = res[1].data.data
        this.turnOptions = res[2].data.data
        this.machineOptions = res[3].data.data
      })
    },
    async search() {
      const { data: res } = await this.$api.searchManualTestStatistic(
        this.queryInfo
      )
      if (res.meta.code !== 0) {
        return this.$message.error('查询失败: ' + res.meta.message)
      }
      this.tableInfo = res.data.tableInfo
      let seriesData = res.data.tableInfo.map((item) => {
        return {
          value: item.count,
          name: item.result
        }
      })
      this.option.series[0].data = seriesData
      console.log(res.data)
    }
  }
}
</script>

<style lang="scss" scoped>
.chart {
  height: 400px;
  margin-top: 20px;
}
</style>

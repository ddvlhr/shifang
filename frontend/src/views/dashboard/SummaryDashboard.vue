<!--
 * @Author: thx 354874258@qq.com
 * @Date: 2023-11-30 16:18:24
 * @LastEditors: thx 354874258@qq.com
 * @LastEditTime: 2023-12-22 11:47:25
 * @FilePath: \frontend\src\views\dashboard\SummaryDashboard.vue
 * @Description: 汇总看板
-->
<template>
  <div class="main-container">
    <el-tabs v-model="tabSelect">
      <el-tab-pane label="卷包" name="machine">
        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <el-row :gutter="10">
              <el-col :span="8">
                <el-checkbox-group
                  v-model="selectedMachines"
                  @change="machineChange"
                >
                  <el-checkbox-button
                    v-for="mac in machines"
                    :label="mac.value"
                    :key="mac.text"
                    >{{ mac.text }}</el-checkbox-button
                  >
                </el-checkbox-group>
              </el-col>
            </el-row>
          </div>
          <div
            v-for="mac in newestGroupIds"
            :key="mac.value"
            style="margin-top: 20px"
          >
            <metrical-push-data
              :groupId="mac.value"
              :showChart="true"
              :tabCharts="false"
            />
          </div>
        </el-card>
      </el-tab-pane>
      <el-tab-pane label="手工" name="manual">
        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <el-row :gutter="10">
              <el-col :span="4" :offset="0">
                <query-select
                  v-model="queryInfo.turnIds"
                  placeholder="班次筛选"
                  :options="turnOptions"
                  :multiple="true"
                  @change="getSpecificationsByTeams"
                />
              </el-col>
              <el-col :span="4">
                <query-select
                  v-model="queryInfo.specificationId"
                  placeholder="牌号筛选"
                  :multiple="true"
                  :options="specificationOptions"
                />
              </el-col>
              <el-col :span="6" :offset="0">
                <query-date-picker
                  v-model="daterange"
                  picker-type="daterange"
                />
              </el-col>
              <el-col :span="4">
                <el-button type="primary" @click="query">查询</el-button>
              </el-col>
            </el-row>
          </div>
          <div>
            <el-row :gutter="10">
              <el-col :span="16">
                <el-table
                  :data="manualDataTableInfo"
                  border
                  ref="manualDataTableInfoRef"
                  height="600"
                  @mouseenter.native="autoScroll(true)"
                  @mouseleave.native="autoScroll()"
                >
                  <el-table-column
                    align="center"
                    prop="specificationName"
                    label="牌号"
                  />
                  <el-table-column
                    align="center"
                    prop="mean"
                    :label="statisticItemShowStr.mean + '(mmWG)'"
                  />
                  <el-table-column
                    align="center"
                    prop="max"
                    :label="statisticItemShowStr.max + '(mmWG)'"
                  />
                  <el-table-column
                    align="center"
                    prop="min"
                    :label="statisticItemShowStr.min + '(mmWG)'"
                  />
                  <el-table-column
                    align="center"
                    prop="sd"
                    :label="statisticItemShowStr.sd"
                  />
                  <el-table-column
                    align="center"
                    prop="cpk"
                    :label="statisticItemShowStr.cpk"
                  />
                  <el-table-column
                    align="center"
                    prop="offset"
                    :label="statisticItemShowStr.offs"
                  />
                  <el-table-column
                    align="center"
                    prop="total"
                    :label="statisticItemShowStr.total"
                  />
                  <el-table-column
                    align="center"
                    prop="quality"
                    :label="statisticItemShowStr.qualified"
                  />
                  <el-table-column
                    align="center"
                    prop="qualityInfo"
                    :label="statisticItemShowStr.goodQualifiedRate"
                  />
                  <el-table-column
                    align="center"
                    prop="rate"
                    :label="statisticItemShowStr.qualifiedRate"
                  />
                </el-table>
                <el-divider></el-divider>
                <v-chart
                  class="chart quality-pie"
                  style="min-height: 340px"
                  :option="pieOption"
                  autoresize
                />
              </el-col>
              <el-col :span="8">
                <h3 class="text-center">卷制情况</h3>
                <div style="min-height: 900px">
                  <v-chart
                    class="chart checker-chart"
                    style="height: 780px"
                    autoresize
                  />
                </div>
              </el-col>
            </el-row>
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import * as echarts from 'echarts'
import VChart, { THEME_KEY } from 'vue-echarts'
export default {
  components: {
    VChart
  },
  provide: {
    [THEME_KEY]: 'dark'
  },
  data() {
    return {
      departmentSelect: '卷包',
      tabSelect: 'manual',
      machines: [],
      workshops: [],
      newestGroupIds: [],
      selectedMachines: [],
      daterange: [],
      fullSpecificationOptions: [],
      specificationOptions: [],
      turnOptions: [],
      workShopOptions: [],
      scrolltimer: '',
      settings: this.$store.state.app.settings,
      statisticItemShowStr: this.$store.state.app.settings.statisticItemShowStr,
      queryInfo: {
        begin: '',
        end: '',
        specificationId: [],
        turnIds: [],
        workshop: ''
      },
      manualDataTableInfo: [],
      pieOption: {
        title: {
          text: '吸阻合格占比情况',
          left: 'center'
        },
        tooltip: {
          trigger: 'item'
        },
        legend: {
          orient: 'vertical',
          left: 'left'
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
        series: [
          {
            name: '吸阻合格占比情况',
            type: 'pie',
            radius: '50%',
            data: [],
            label: {
              show: true,
              formatter: function (params) {
                const item = params.data
                return (
                  item.name +
                  ':' +
                  item.resistanceMean +
                  '(' +
                  item.qualifiedRate +
                  ' / ' +
                  item.goodQualifiedRate +
                  ')'
                )
              }
            },
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
              }
            }
          }
        ]
      },
      dataMaxOption: {
        dataset: {
          source: []
        },
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'shadow'
          }
        },
        xAxis: {
          max: 'dataMax'
        },
        yAxis: {
          name: '工号',
          nameLocation: 'start',
          type: 'category',
          data: []
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
        series: [
          {
            name: '合格',
            type: 'bar',
            stack: 'total',
            label: {
              show: true
            },
            itemStyle: {
              color: 'rgba(52, 212, 65, .8)'
            },
            emphasis: {
              focus: 'series'
            },
            data: []
          },
          {
            name: '下超',
            type: 'bar',
            stack: 'total',
            label: {
              show: true
            },
            itemStyle: {
              color: 'rgba(21, 98, 203, .8)'
            },
            emphasis: {
              focus: 'series'
            },
            data: []
          },
          {
            name: '上超',
            type: 'bar',
            stack: 'total',
            label: {
              show: true
            },
            itemStyle: {
              color: 'rgba(255, 0, 0, .8)'
            },
            emphasis: {
              focus: 'series'
            },
            data: []
          },
          {
            name: '统计',
            type: 'bar',
            stack: 'total',
            label: {
              normal: {
                show: true,
                position: 'right',
                formatter: function (params) {
                  const item = params.item
                  return item[5]
                },
                fontSize: 16,
                textStyle: {
                  color: '#FFF'
                }
              }
            },
            data: []
          }
        ],
        legend: {
          show: true
        }
      }
    }
  },
  created() {
    this.getOptions()
    this.getHandicraftWorkshop()
  },
  methods: {
    async getOptions() {
      const { data: res } = await this.$api.getOptions(true)
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败: ' + res.meta.message)
      }
      this.machines = res.data.machines
      this.specificationOptions = res.data.specifications
      this.fullSpecificationOptions = res.data.specifications
      this.turnOptions = res.data.turns
    },
    async getHandicraftWorkshop() {
      const { data: res } = await this.$api.getHandicraftWorkshop()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败：' + res.meta.message)
      }

      const options = res.data.map((item) => {
        return { text: item.name, value: item.letters }
      })
      this.workshops = options
    },
    changeDepartment() {
      this.selectedMachines = []
    },
    async machineChange() {
      const machines = this.selectedMachines.map((item) => {
        return item.toString()
      })
      const params = {
        machines,
        isMachine: this.departmentSelect === '卷包'
      }
      const { data: res } = await this.$api.getNewestGroupIds(params)
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败：' + res.meta.message)
      }
      this.newestGroupIds = res.data
    },
    async query() {
      if (this.queryInfo.turnIds.length === 0) {
        return this.$message.error('请选择班组')
      }

      if (this.daterange.length > 0) {
        this.queryInfo.begin = this.daterange[0]
        this.queryInfo.end = this.daterange[1]
      }

      const loading = this.$loading({
        lock: true,
        text: '正在查询',
        spinner: 'el-icon-loading'
      })
      const { data: res } = await this.$api.getManualSummaryInfo(this.queryInfo)
      if (res.meta.code !== 0) {
        return this.$message.error('获取数据失败：' + res.meta.message)
      }

      this.manualDataTableInfo = res.data.tableInfo
      const dom = document.querySelector('.quality-pie')
      let chart = echarts.getInstanceByDom(dom)
      if (chart == null) {
        chart = echarts.init(dom)
      }
      const tempOption = JSON.parse(JSON.stringify(this.pieOption))
      const pieChartInfo = res.data.pieChartInfo
      tempOption.series[0].data = pieChartInfo
      chart.setOption(tempOption)

      const checkInfo = res.data.checkerInfo
      const yList = []
      const seriesData = []
      const lessData = []
      const qualityData = []
      const moreData = []
      const statsticData = []
      const staData = []
      checkInfo.forEach((item) => {
        yList.push(item.no)
        seriesData.push(item.count)
        lessData.push(item.lessCount)
        qualityData.push(item.qualityCount)
        moreData.push(item.moreCount)
        staData.push(0)
        statsticData.push([
          item.no,
          item.count,
          item.lessCount,
          item.qualityCount,
          item.moreCount,
          item.qualifiedRate,
          item.goodRate
        ])
      })

      const checkDom = document.querySelector('.checker-chart')
      let checkerChart = echarts.getInstanceByDom(checkDom)
      if (checkerChart == null) {
        checkerChart = echarts.init(dom)
      }
      const tempCheckerOption = this.dataMaxOption
      tempCheckerOption.dataset.source = statsticData
      tempCheckerOption.yAxis.data = yList
      tempCheckerOption.series[1].data = lessData
      tempCheckerOption.series[0].data = qualityData
      tempCheckerOption.series[2].data = moreData
      tempCheckerOption.series[3].data = staData
      tempCheckerOption.series[1].name = this.statisticItemShowStr.lowCnt
      tempCheckerOption.series[0].name = this.statisticItemShowStr.qualified
      tempCheckerOption.series[2].name = this.statisticItemShowStr.highCnt
      if (yList.length > 20) {
        const dataZoom = [
          {
            type: 'slider',
            show: true,
            yAxisIndex: [0],
            start: 0,
            end: 15
          }
        ]
        tempCheckerOption.dataZoom = dataZoom
      }
      checkerChart.setOption(tempCheckerOption)
      this.autoScroll(false)
      loading.close()
    },
    async getSpecificationsByTeams() {
      const queryInfo = {
        begin: this.queryInfo.begin,
        end: this.queryInfo.end,
        turnIds: this.queryInfo.turnIds,
        isManual: true
      }

      const { data: res } = await this.$api.getSpecificationsByTeams(queryInfo)
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项失败: ' + res.meta.message)
      }

      if (res.data.length > 0) {
        this.specificationOptions = res.data
      } else {
        this.specificationOptions = this.fullSpecificationOptions
      }
    },
    // 设置自动滚动
    autoScroll(stop) {
      const table = this.$refs.manualDataTableInfoRef // 拿到表格中承载数据的div元素
      const divData = table.$refs.bodyWrapper // 拿到元素后，对元素进行定时增加距离顶部距离，实现滚动效果 (此配置为每150毫秒移动1像素)
      if (stop) {
        // 再通过事件监听，监听到 组件销毁 后，再执行关闭计时器。
        window.clearInterval(this.scrolltimer)
      } else {
        this.scrolltimer = window.setInterval(() => {
          // 元素自增距离顶部1像素
          divData.scrollTop += 1
          // 判断元素是否滚动到底部 (可视高度+距离顶部=整个高度)
          if (
            divData.clientHeight + divData.scrollTop ===
            divData.scrollHeight
          ) {
            // 重置table距离顶部距离
            divData.scrollTop = 0
            // 重置table距离顶部距离。 值= (滚动到底部时，距离顶部的大小) - 整个高度/2
            // divData.scrollTop = divData.scrollTop - divData.scrollHeight / 2
          }
        }, 100) // 滚动速度
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.chart {
  width: 100% !important;
  height: 100%;
}

.el-tag {
  margin-top: 15px;
}
</style>

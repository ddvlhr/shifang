<template>
  <div
    id="manualMetricalPush"
    style="background-color: #100c2a; max-height: 1080px"
  >
    <el-row :gutter="10">
      <el-col :span="6" v-if="metricalPushData.length > 0">
        <!-- <el-alert
          :title="'当前数据推送时间: ' + metricalPushData[0].testTime"
          type="success"
          :closable="false"
          effect="dark"
        /> -->
        <el-alert type="success" :closable="false" effect="dark">
          <span>{{ info.specificationName }}</span>
          <el-divider direction="vertical"></el-divider>
          <span>{{ info.beginTime }}</span>
          <el-divider direction="vertical"></el-divider>
          <span>{{ info.instance }}</span>
          <el-divider direction="vertical"></el-divider>
          <span>{{ info.workShopName }}</span>
        </el-alert>
      </el-col>
      <el-col :span="4" style="vertical-align: middle">
        <el-button
          icon="el-icon-full-screen"
          type="primary"
          @click="metricalPushDataFullscreen"
          class="items-center"
          >全屏/退出全屏</el-button
        >
      </el-col>
    </el-row>
    <el-row :gutter="10" style="max-height: 1040px">
      <el-col :span="13">
        <el-row style="height: 400px">
          <el-col>
            <el-tabs
              v-model="tabSelected"
              tab-position="top"
              class="metricalPushData"
            >
              <el-tab-pane label="统计信息" name="statistic">
                <el-table
                  :data="statisticDataInfo"
                  border
                  highlight-current-row
                  height="340"
                >
                  <el-table-column
                    :label="col.text"
                    :prop="col.key"
                    v-for="col in statisticColumns"
                    :key="col.key + groupId"
                    align="center"
                  >
                    <template slot-scope="scope">
                      <p v-if="scope.row[col.key]?.high === 0">
                        {{ scope.row[col.key]?.text }}
                      </p>
                      <p
                        v-else
                        :class="
                          scope.row[col.key]?.text > scope.row[col.key]?.high
                            ? 'text-red-600'
                            : scope.row[col.key]?.text < scope.row[col.key]?.low
                            ? 'text-blue-600'
                            : 'text-green-600'
                        "
                      >
                        {{
                          scope.row[col.key]?.text === 0
                            ? ''
                            : scope.row[col.key]?.text
                        }}
                      </p>
                    </template>
                  </el-table-column>
                </el-table>
              </el-tab-pane>
              <el-tab-pane label="原始数据" name="origin">
                <el-table
                  :data="dataInfo"
                  border
                  highlight-current-row=""
                  height="340"
                >
                  <el-table-column
                    label="#"
                    type="index"
                    align="center"
                  ></el-table-column>
                  <el-table-column
                    v-for="col in columns"
                    :label="col.text"
                    :prop="col.key"
                    :key="col.key"
                    align="center"
                  >
                    <template slot-scope="scope">
                      <p v-if="scope.row[col.key]?.high === 0">
                        {{ scope.row[col.key]?.text }}
                      </p>
                      <p
                        v-else
                        :class="
                          scope.row[col.key]?.text > scope.row[col.key]?.high
                            ? 'text-red-600'
                            : scope.row[col.key]?.text < scope.row[col.key]?.low
                            ? 'text-blue-600'
                            : 'text-green-600'
                        "
                      >
                        {{
                          scope.row[col.key]?.text === 0
                            ? ''
                            : scope.row[col.key]?.text
                        }}
                      </p>
                    </template>
                  </el-table-column>
                </el-table>
              </el-tab-pane>
              <el-tab-pane v-if="showChart" label="数据图表" name="chart">
                <el-tabs
                  tab-position="top"
                  v-model="chartTabSelected"
                  v-if="tabCharts"
                >
                  <el-tab-pane
                    v-for="col in measureColumns"
                    :label="col.text"
                    :name="col.key"
                    :key="col.key"
                  >
                    <v-chart
                      class="chart"
                      :class="[col.key + groupId, col.key]"
                      :option="option"
                      autoresize
                    />
                  </el-tab-pane>
                </el-tabs>
                <div v-else>
                  <div style="margin: 10px 0">
                    <el-radio-group
                      v-model="chartType"
                      @input="changeChartType"
                    >
                      <el-radio-button label="柱状图"></el-radio-button>
                      <el-radio-button label="折线图"></el-radio-button>
                    </el-radio-group>
                  </div>
                  <el-row :gutter="10">
                    <el-col
                      v-for="col in measureColumns"
                      :key="col.key"
                      :span="measureColumns.length < 2 ? 24 : 12"
                    >
                      <v-chart
                        style="height: 360px"
                        class="chart"
                        :class="[col.key + groupId, col.key]"
                        :option="option"
                        autoresize
                      />
                    </el-col>
                  </el-row>
                </div>
              </el-tab-pane>
            </el-tabs>
          </el-col>
        </el-row>
        <!-- <el-divider></el-divider> -->
        <el-row style="max-height: 640px; margin: 10px 0 10px 0">
          <el-col>
            <el-table
              :data="manualDataTableInfo"
              max-height="300"
              @mouseenter.native="tableAutoScroll(true)"
              @mouseleave.native="tableAutoScroll()"
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
            </el-table>
            <v-chart
              class="chart quality-pie"
              :class="'quality-pie-' + groupId"
              style="height: 340px"
              :option="pieOption"
              autoresize
            />
          </el-col>
        </el-row>
      </el-col>
      <el-col :span="10">
        <v-chart
          class="chart checker-chart"
          :class="'checker-chart-' + groupId"
          style="height: 1040px"
          autoresize
        />
      </el-col>
    </el-row>
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
  props: {
    showChart: {
      type: Boolean,
      default: false
    },
    tabCharts: {
      type: Boolean,
      default: true
    },
    workShopId: {
      type: String,
      default: ''
    },
    groupId: {
      type: Number,
      default: 0
    },
    autoScroll: {
      type: Boolean,
      default: true
    }
  },
  data() {
    const that = this
    return {
      tabSelected: 'statistic',
      chartTabSelected: '',
      statisticKey: 123,
      rules: [],
      columns: {},
      info: {},
      measureColumns: [],
      statisticColumns: {},
      dataInfo: [],
      statisticDataInfo: [],
      chartDataInfo: [],
      chartMarkLineInfo: [],
      manualDataTableInfo: [],
      tableSelected: 'table',
      settings: this.$store.state.app.settings,
      statisticItemShowStr: this.$store.state.app.settings.statisticItemShowStr,
      checkerStatisticData: [],
      chartType: '柱状图',
      timer: {},
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
            name: '重量',
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
      },
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
                distance: 20,
                formatter: function (params) {
                  const item = that.checkerStatisticData[params.dataIndex]
                  return item[5] + '% / ' + item[6] + '%'
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
  computed: {
    metricalPushData() {
      return this.$store.state.user.workshopMetricalData
    }
  },
  watch: {
    metricalPushData(newVal) {
      if (newVal.length > 0) {
        this.getStatisticInfo(newVal[0].groupId)
        this.getManualTableInfo(this.workShopId)
        this.getManualCheckerInfos(this.workShopId)
        setTimeout(() => {
          this.updateCharts()
        }, 1000)
      }
    },
    workShopId(newVal) {
      this.getManualTableInfo()
      this.getManualCheckerInfos()
    }
  },
  mounted() {
    if (this.groupId > 0) {
      this.getStatisticInfo(this.groupId)
      setTimeout(() => {
        this.updateCharts()
      }, 1000)
    }
  },
  methods: {
    async getStatisticInfo(groupId) {
      const { data: res } = await this.$api.getMetricalDataStatisticInfo(
        groupId,
        true
      )
      const columns = res.data.columns
      if (this.groupId > 0) {
        const workshop = res.data.instance.charAt(0)
        this.getManualTableInfo(workshop)
        this.getManualCheckerInfos(workshop)
      }
      this.chartDataInfo = res.data.chartDataInfo
      this.rules = res.data.standard
      const tempColumns = []
      const tempMeasureColumns = []
      for (const key in columns) {
        if (Object.hasOwnProperty.call(columns, key)) {
          const element = columns[key]
          tempColumns.push({ key, text: element.text })
          if (key !== 'testTime') {
            tempMeasureColumns.push(key)
            this.measureColumns.push({ key, text: element.text, init: false })
          }
        }
      }
      this.chartTabSelected = tempMeasureColumns[0]
      this.columns = tempColumns
      const statisticColumns = res.data.statisticColumns
      const tempStatisticColumns = [{ key: 'itemName', text: '' }]
      for (const key in statisticColumns) {
        if (Object.hasOwnProperty.call(statisticColumns, key)) {
          const element = statisticColumns[key]
          tempStatisticColumns.push({ key, text: element.text })
        }
      }

      this.statisticColumns = tempStatisticColumns
      this.dataInfo = res.data.dataInfo
      this.statisticDataInfo = res.data.statisticDataInfo
      this.chartMarkLineInfo = res.data.chartMarkLineInfo
      this.info = res.data
    },
    async getManualTableInfo(workshop) {
      console.log(this.workShopId)
      const { data: res } = await this.$api.getManualTableInfo(workshop)
      if (res.meta.code !== 0) {
        return this.$message.error('获取信息失败：' + res.meta.message)
      }

      this.manualDataTableInfo = res.data.tableInfo
      const clsName =
        this.groupId > 0 ? `.quality-pie-${this.groupId}` : '.quality-pie'
      const dom = document.querySelector(clsName)
      let chart = echarts.getInstanceByDom(dom)
      if (chart == null) {
        chart = echarts.init(dom)
      }
      const tempOption = JSON.parse(JSON.stringify(this.pieOption))
      const pieChartInfo = res.data.pieChartInfo
      for (var item in pieChartInfo) {
        if (Object.hasOwnProperty.call(pieChartInfo, item)) {
          const element = pieChartInfo[item]
          tempOption.series[0].data.push({
            name: item,
            value: element
          })
        }
      }
      chart.setOption(tempOption)
    },
    changeChartType() {
      this.updateCharts()
    },
    updateCharts() {
      const chartDataInfo = this.chartDataInfo
      const chartMarkLineInfo = this.chartMarkLineInfo
      const rules = JSON.parse(JSON.stringify(this.rules))
      this.measureColumns.forEach((item) => {
        const clsName =
          this.groupId > 0 ? `.${item.key}${this.groupId}` : `.${item.key}`
        const dom = document.querySelector(clsName)
        console.log(dom)
        const chart = echarts.getInstanceByDom(dom)
        const tempOption = JSON.parse(JSON.stringify(this.option))
        const columnyKey = item.key.slice(1)
        const xAxis = []

        const data = chartDataInfo[columnyKey]
        const rule = rules[columnyKey]
        const markLineInfo = chartMarkLineInfo[columnyKey]
        tempOption.title.text = item.text
        tempOption.series[0].name = item.text
        tempOption.legend.data.unshift(item.text)
        if (data.length > 0) {
          tempOption.series[0].data = data
        }
        let min = Math.min(...data)
        let max = Math.max(...data)
        min = min < rule.lower ? min : rule.lower
        max = max > rule.upper ? max : rule.upper
        tempOption.yAxis.min = (min * 0.99).toFixed(2)
        tempOption.yAxis.max = (max * 1.01).toFixed(2)
        for (var i = 1; i <= data.length; i++) {
          xAxis.push(i)
        }
        const itemStyle = {
          color: function (value) {
            if (value.data > rule.upper) {
              return 'rgba(255, 0, 0, .8)'
            } else if (value.data < rule.lower) {
              return 'rgba(21, 98, 203, .8)'
            } else {
              return 'rgba(52, 212, 65, .8)'
            }
          }
        }
        tempOption.series[0].itemStyle = itemStyle
        tempOption.series[0].type = this.chartType === '柱状图' ? 'bar' : 'line'
        if (this.chartType === '柱状图') {
          const visualMap = {
            top: 10,
            right: 'center',
            orient: 'horizontal',
            precision: 3,
            pieces: [
              {
                gt: rule.upper,
                color: 'rgba(255, 0, 0, .8)'
              },
              {
                lte: rule.upper,
                gte: rule.lower,
                color: 'rgba(52, 212, 65, .8)'
              },
              {
                lt: rule.lower,
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
                color: 'rgba(255, 0, 0, .8)',
                width: 3
              },
              yAxis: rule.upper,
              label: { formatter: '↑数量: ' + markLineInfo.upper }
            },
            {
              name: '标准值',
              lineStyle: {
                type: 'medium',
                color: 'rgba(52, 212, 65, .8)',
                width: 3
              },
              yAxis: rule.standard,
              label: { formatter: '↕数量: ' + markLineInfo.standard }
            },
            {
              name: '下限值',
              lineStyle: {
                type: 'min',
                color: 'rgba(21, 98, 203, .8)',
                width: 3
              },
              yAxis: rule.lower,
              label: { formatter: '↓数量: ' + markLineInfo.lower }
            }
          ]
        }
        tempOption.series[0].markLine = markLine

        tempOption.xAxis.data = xAxis
        chart.setOption(tempOption)
      })
    },
    async getManualCheckerInfos(workshop) {
      const { data: res } = await this.$api.getManualCheckerInfos(workshop)
      if (res.meta.code !== 0) {
        return this.$message.error('获取信息失败：' + res.meta.message)
      }

      clearInterval(this.timer)
      const yList = []
      const seriesData = []
      const lessData = []
      const qualityData = []
      const moreData = []
      const staData = []
      const statisticData = []
      res.data.forEach((item) => {
        yList.push(item.no)
        seriesData.push(item.count)
        lessData.push(item.lessCount)
        qualityData.push(item.qualityCount)
        moreData.push(item.moreCount)
        staData.push(0)
        statisticData.push([
          item.no,
          item.count,
          item.lessCount,
          item.qualityCount,
          item.moreCount,
          item.qualifiedRate,
          item.goodRate
        ])
      })

      const clsName =
        this.groupId > 0 ? `.checker-chart-${this.groupId}` : '.checker-chart'
      const dom = document.querySelector(clsName)
      let chart = echarts.getInstanceByDom(dom)
      if (chart == null) {
        chart = echarts.init(dom)
      }
      console.log(this.dataMaxOption)
      const tempOption = this.dataMaxOption
      tempOption.yAxis.data = yList
      this.checkerStatisticData = statisticData
      tempOption.series[1].data = lessData
      tempOption.series[0].data = qualityData
      tempOption.series[2].data = moreData
      tempOption.series[3].data = staData
      tempOption.series[1].name = this.statisticItemShowStr.lowCnt
      tempOption.series[0].name = this.statisticItemShowStr.qualified
      tempOption.series[2].name = this.statisticItemShowStr.highCnt
      const dataZoom = [
        {
          type: 'slider',
          show: true,
          yAxisIndex: [0],
          start: 0,
          end: 10
        }
      ]
      tempOption.dataZoom = dataZoom
      chart.setOption(tempOption)
      if (this.autoScroll) {
        this.timer = setInterval(() => {
          if (tempOption.dataZoom[0].end === yList.length - 1) {
            tempOption.dataZoom[0].end = 10
            tempOption.dataZoom[0].start = 0
          } else {
            tempOption.dataZoom[0].end += 1
            tempOption.dataZoom[0].start += 1
          }
          chart.setOption(tempOption)
        }, 2000)
      }
    },
    metricalPushDataFullscreen() {
      const isFull = this.$utils.handleFullscreen('#manualMetricalPush')
      if (!isFull) {
        this.$utils.switchMode('dark')
      } else {
        this.$utils.switchMode('light')
      }
    },
    // 设置自动滚动
    tableAutoScroll(stop) {
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

.manualMetricalPush {
  max-height: 1080px;
  overflow-y: auto;
}
.el-divider__vertical {
  height: 100% !important;
}

#manualMetricalPush {
  background: #fff;
  padding-top: 20px;
  padding-left: 20px;
  padding-bottom: 20px;
  padding-right: 0;
}

.el-alert__description {
  margin: 0 !important;
  padding: 2.5px 0 2.5px 0 !important;
}
</style>

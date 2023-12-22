<template>
  <div id="metricalDataPush" style="background-color: #100c2a">
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
    <div style="widht: 100%">
      <el-tabs
        v-model="tabSelected"
        tab-position="top"
        class="metricalPushData"
      >
        <el-tab-pane label="统计信息" name="statistic">
          <el-table :data="statisticDataInfo" border highlight-current-row>
            <el-table-column
              :label="col.text"
              :prop="col.key"
              v-for="col in statisticColumns"
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
        <el-tab-pane label="原始数据" name="origin">
          <el-table
            :data="dataInfo"
            border
            highlight-current-row=""
            height="500"
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
                :class="col.key"
                :option="option"
                autoresize
              />
            </el-tab-pane>
          </el-tabs>
          <div v-else>
            <div style="margin: 10px 0">
              <el-radio-group v-model="chartType" @input="changeChartType">
                <el-radio-button label="柱状图"></el-radio-button>
                <el-radio-button label="折线图"></el-radio-button>
              </el-radio-group>
            </div>
            <el-row :gutter="10">
              <el-col v-for="col in measureColumns" :key="col.key" :span="12">
                <v-chart
                  class="chart grid-chart"
                  :class="col.key + groupId"
                  :option="option"
                  autoresize
                />
              </el-col>
              <el-col :span="24">
                <h2 style="text-align: center; margin-bottom: 20px">
                  当日该机台该牌号统计数据
                </h2>
                <el-table
                  :data="dayStatisticDataInfo"
                  border
                  highlight-current-row
                >
                  <el-table-column
                    :label="col.text"
                    :prop="col.key"
                    v-for="col in statisticColumns"
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
              </el-col>
            </el-row>
          </div>
        </el-tab-pane>
      </el-tabs>
    </div>
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
      default: false,
      required: true
    },
    tabCharts: {
      type: Boolean,
      default: true,
      required: true
    },
    groupId: {
      type: Number,
      default: 0
    }
  },
  data() {
    return {
      tabSelected: 'statistic',
      chartTabSelected: '',
      statisticKey: 123,
      rules: [],
      columns: {},
      info: {},
      measureColumns: [],
      statisticColumns: {},
      fullScreen: false,
      dataInfo: [],
      statisticDataInfo: [],
      dayStatisticDataInfo: [],
      chartDataInfo: [],
      chartMarkLineInfo: [],
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
      }
    }
  },
  computed: {
    metricalPushData() {
      return this.$store.state.user.metricalPushData
    }
  },
  watch: {
    metricalPushData(newVal) {
      if (newVal.length > 0) {
        this.getStatisticInfo(newVal[0].groupId)
        setTimeout(() => {
          this.updateCharts()
        }, 1000)
      }
    }
  },
  mounted() {
    this.getStatisticInfo(this.groupId)
    setTimeout(() => {
      this.updateCharts()
    }, 1000)
  },
  methods: {
    async getStatisticInfo(groupId) {
      const { data: res } = await this.$api.getMetricalDataStatisticInfo(
        groupId,
        true
      )
      const columns = res.data.columns
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
            if (this.measureColumns.find((c) => c.key === key)) {
              continue
            }
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
      this.dayStatisticDataInfo = res.data.dayStatisticDataInfo
      this.chartMarkLineInfo = res.data.chartMarkLineInfo
      this.info = res.data
    },
    changeChartType() {
      this.updateCharts()
    },
    updateCharts() {
      const chartDataInfo = this.chartDataInfo
      const chartMarkLineInfo = this.chartMarkLineInfo
      const rules = JSON.parse(JSON.stringify(this.rules))
      this.measureColumns.forEach((item) => {
        const dom = document.querySelector(
          `#pane-chart .${item.key}${this.groupId}`
        )
        const chart = echarts.getInstanceByDom(dom)
        const tempOption = JSON.parse(JSON.stringify(this.option))
        const columnyKey = item.key.slice(1)
        const xAxis = []

        const data = chartDataInfo[columnyKey]
        const markLineInfo = chartMarkLineInfo[columnyKey]
        const rule = rules[columnyKey]
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
                color: '#FF0000',
                width: 3
              },
              yAxis: rule.upper,
              label: { formatter: '↑数量: ' + markLineInfo.upper }
            },
            {
              name: '标准值',
              lineStyle: {
                type: 'medium',
                color: '#00FF00',
                width: 3
              },
              yAxis: rule.standard,
              label: { formatter: '↕数量: ' + markLineInfo.standard }
            },
            {
              name: '下限值',
              lineStyle: {
                type: 'min',
                color: '#0066FF',
                width: 3
              },
              yAxis: rule.lower,
              label: { formatter: '↓数量: ' + markLineInfo.lower }
            }
          ]
        }

        if (rule.qualityUpper !== rule.standard) {
          markLine.data.push({
            name: '上预警',
            lineStyle: {
              type: 'max',
              color: '#FF9900',
              width: 3
            },
            yAxis: rule.qualityUpper,
            label: { formatter: '↑数量: ' + markLineInfo.qualityUpper }
          })
        }

        if (rule.qualityLower !== rule.standard) {
          markLine.data.push({
            name: '下预警',
            lineStyle: {
              type: 'min',
              color: '#00CCFF',
              width: 3
            },
            yAxis: rule.qualityLower,
            label: { formatter: '↓数量: ' + markLineInfo.qualityLower }
          })
        }
        tempOption.series[0].markLine = markLine

        tempOption.xAxis.data = xAxis
        chart.setOption(tempOption)
      })
    },
    metricalPushDataFullscreen() {
      const isFull = this.$utils.handleFullscreen('#metricalDataPush')
      if (!isFull) {
        this.$utils.switchMode('dark')
      } else {
        this.$utils.switchMode('light')
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.chart {
  width: 100% !important;
  height: 440px;
}

.grid-chart {
  height: 300px;
}

.metricalPushData {
  max-height: 680px;
  overflow-y: auto;
}

#metricalDataPush {
  background: #fff;
  padding-top: 20px;
  padding-left: 20px;
  padding-bottom: 20px;
  padding-right: 0;
}

.el-alert .el-alert__description {
  margin: 0 !important;
  padding: 2.5px 0 2.5px 0 !important;
}
</style>

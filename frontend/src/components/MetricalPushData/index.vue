<template>
  <el-tabs v-model="tabSelected" tab-position="top">
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
                scope.row[col.key]?.text === 0 ? '' : scope.row[col.key]?.text
              }}
            </p>
          </template>
        </el-table-column>
      </el-table>
    </el-tab-pane>
    <el-tab-pane label="原始数据" name="origin">
      <el-table :data="dataInfo" border highlight-current-row="">
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
                scope.row[col.key]?.text === 0 ? '' : scope.row[col.key]?.text
              }}
            </p>
          </template>
        </el-table-column>
      </el-table>
    </el-tab-pane>
    <el-tab-pane v-if="showChart" label="数据图表" name="chart">
      <el-tabs tab-position="top">
        <el-tab-pane
          v-for="col in measureColumns"
          :label="col.text"
          :name="col.key"
          :key="col.key"
        >
          <v-chart />
        </el-tab-pane>
      </el-tabs>
    </el-tab-pane>
  </el-tabs>
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
  props: {
    showChart: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      tabSelected: 'statistic',
      statisticKey: 123,
      columns: {},
      measureColumns: [],
      statisticColumns: {},
      dataInfo: [],
      statisticDataInfo: []
    }
  },
  computed: {
    metricalPushData() {
      return this.$store.state.user.metricalPushData
    }
  },
  watch: {
    metricalPushData(newVal) {
      this.getStatisticInfo(newVal[0].groupId)
    }
  },
  methods: {
    async getStatisticInfo(groupId) {
      const { data: res } = await this.$api.getMetricalDataStatisticInfo(
        groupId
      )
      const columns = res.data.columns
      const tempColumns = []
      for (const key in columns) {
        if (Object.hasOwnProperty.call(columns, key)) {
          const element = columns[key]
          tempColumns.push({ key, text: element.text })
          if (key !== 'testTime') {
            this.measureColumns.push({ key, text: element.text, init: false })
          }
        }
      }
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
    }
  }
}
</script>

<style lang="scss" scoped></style>

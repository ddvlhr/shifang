<template>
  <el-dialog
    title="统计信息"
    :visible.sync="dialogVisible"
    width="90%"
    @close="handleClosed"
  >
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
                    ? 'text-sky-400'
                    : 'text-lime-500'
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
                    ? 'text-sky-400'
                    : 'text-lime-500'
                "
              >
                {{
                  scope.row[col.key]?.text === 0 ? '' : scope.row[col.key]?.text
                }}
              </p>
              <!-- <p
                v-else-if="
                  scope.row[col.key]?.high &&
                  scope.row[col.key]?.text > scope.row[col.key]?.high
                "
                class="text-red-600"
              >
                {{ scope.row[col.key]?.text }}
              </p>
              <p
                v-else-if="
                  scope.row[col.key]?.high &&
                  scope.row[col.key]?.text < scope.row[col.key]?.low
                "
                class="text-blue-600"
              >
                {{ scope.row[col.key]?.text }}
              </p>
              <p v-else class="text-green-600">
                {{ scope.row[col.key]?.text }}
              </p> -->
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
    </el-tabs>
    <span slot="footer" class="dialog-footer">
      <el-button @click="dialogVisible = false">取 消</el-button>
    </span>
  </el-dialog>
</template>

<script>
export default {
  props: {
    visible: {
      type: Boolean,
      default: false
    },
    groupId: {
      type: Number,
      requried: true
    }
  },
  data() {
    return {
      dialogVisible: false,
      tabSelected: 'statistic',
      statisticKey: 123,
      columns: {},
      statisticColumns: {},
      dataInfo: [],
      statisticDataInfo: []
    }
  },
  computed: {},
  watch: {
    visible(val) {
      if (val) {
        this.getStatisticInfo()
        this.dialogVisible = true
      }
    }
  },
  methods: {
    async getStatisticInfo() {
      const { data: res } = await this.$api.getMetricalDataStatisticInfo(
        this.groupId
      )
      console.log(res.data)
      const columns = res.data.columns
      const tempColumns = []
      for (const key in columns) {
        if (Object.hasOwnProperty.call(columns, key)) {
          const element = columns[key]
          tempColumns.push({ key, text: element.text })
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
    },
    handleClosed() {
      this.$emit('update:visible', false)
    }
  }
}
</script>

<style lang="scss" scoped></style>

<template>
  <div class="main-container">
    <function-button @addAppearance="addAppearance" @download="download" />
    <el-card>
      <el-row :gutter="10">
        <el-col :span="6" :offset="0">
          <query-select
            v-model="queryInfo.specificationId"
            :options="specificationOptions"
            @change="query"
            @clear="query"
            placeholder="牌号筛选"
          />
        </el-col>
        <el-col :span="6" :offset="0">
          <query-select
            v-model="queryInfo.specificationTypeId"
            :options="specificationTypeOptions"
            @change="query"
            @clear="query"
            placeholder="牌号类型筛选"
          />
        </el-col>
        <el-col :span="4" :offset="0">
          <query-select
            v-model="queryInfo.turnId"
            :options="turnOptions"
            @change="query"
            @clear="query"
            placeholder="班次筛选"
          />
        </el-col>
        <el-col :span="4" :offset="0">
          <query-select
            v-model="queryInfo.machineModelId"
            :options="machineModelOptions"
            @change="query"
            @clear="query"
            placeholder="请选择机台"
            :clearable="true"
          />
        </el-col>
      </el-row>
      <el-row :gutter="10" style="margin-top: 15px">
        <el-col :span="6" :offset="0">
          <el-date-picker
            v-model="dateRange"
            @change="getDateRange"
            type="daterange"
            size="normal"
            range-separator="-"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-col>
        <el-col :span="4">
          <query-select
            v-model="queryInfo.state"
            placeholder="报表状态筛选"
            :options="reportRetList"
            @change="query"
          />
        </el-col>
      </el-row>
      <ele-table
        :columns-desc="columnsDesc"
        :is-show-index="true"
        :request-fn="getProductReports"
        :is-show-right-delete="false"
        :is-show-top-delete="false"
        :rightButtons="rightButtons"
        ref="table"
      ></ele-table>
    </el-card>
    <ele-form-dialog
      v-model="formData"
      width="70%"
      :form-desc="formDesc"
      title="编辑成品检验报表外观缺陷"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @close="handleClosed"
    ></ele-form-dialog>
    <el-dialog
      title="报表详情"
      :visible.sync="statisticDialogVisible"
      width="80%"
    >
      <el-tabs v-model="tabSelected" tab-position="top">
        <el-tab-pane label="统计信息" name="statistic">
          <el-tag>牌号: {{ statisticInfo.specificationName }}</el-tag>
          <el-tag>班次: {{ statisticInfo.turnName }}</el-tag>
          <el-tag>机型名称: {{ statisticInfo.machineModelName }}</el-tag>
          <el-tag>得分: {{ 100 - statisticInfo.points }}</el-tag>
          <el-tag>物理判定: {{ statisticInfo.phyRet }}</el-tag>
          <el-tag>综合判定: {{ statisticInfo.finalRet }}</el-tag>
          <ele-table
            :key="statisticKey"
            :columns-desc="statisticColumnsDesc"
            :is-show-index="false"
            :is-show-right-delete="false"
            :is-show-top-delete="false"
            :is-show-pagination="false"
            :is-show-selection="false"
            :request-fn="getStatisticData"
          ></ele-table>
          <el-card v-if="statisticInfo.phyRetDes !== null">
            <div slot="header" class="clearfix">
              <span>测量指标不合格项</span>
            </div>
            <el-tag
              type="danger"
              v-for="(item, index) in statisticInfo.phyRetDes"
              :key="index"
              >{{ item }}</el-tag
            >
          </el-card>
          <el-card v-if="statisticInfo.appearances !== null">
            <div slot="header" class="clearfix">
              <span>外观缺陷</span>
            </div>
            <el-tag
              type="danger"
              v-for="(item, index) in statisticInfo.appearances"
              :key="index"
              >{{ item }}</el-tag
            >
          </el-card>
        </el-tab-pane>
        <el-tab-pane label="原始数据" name="origin">
          <ele-table
            :key="statisticKey"
            :columns-desc="originColumnsDesc"
            :request-fn="getOriginDataInfo"
            :is-show-selection="false"
            :is-show-top-delete="false"
            :is-show-right-delete="false"
            ref="originTableRef"
          ></ele-table>
        </el-tab-pane>
        <el-tab-pane label="水分数据" name="water">
          <ele-table
            :key="statisticKey"
            :columns-desc="waterColumnsDesc"
            :request-fn="getWaterDataInfo"
            :is-show-selection="false"
            :is-show-top-delete="false"
            :is-show-right-delete="false"
            ref="waterTableRef"
          />
        </el-tab-pane>
      </el-tabs>
      <span slot="footer" class="dialog-footer">
        <el-button @click="statisticDialogVisible = false">关 闭</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { reportRetList } from '@/assets/js/constant'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    return {
      specificationOptions: [],
      specificationTypeOptions: [],
      turnOptions: [],
      machineModelOptions: [],
      appearanceIndicatorOptions: [],
      statisticDialogVisible: false,
      statisticColumnsDesc: {},
      originColumnsDesc: {},
      originDataInfo: [],
      statisticDataInfo: [],
      statisticInfo: {
        specificationName: ''
      },
      reportRetList: reportRetList,
      queryInfo: {
        specificationId: '',
        specificationTypeId: '',
        turnId: '',
        machineModelId: '',
        testDate: '',
        beginDate: '',
        endDate: '',
        query: '',
        state: ''
      },
      dateRange: [],
      tabSelected: 'statistic',
      statisticKey: 0,
      waterDataInfo: [],
      waterColumnsDesc: {
        testDateTime: {
          text: '测量时间'
        },
        before: {
          text: '烘前重(g)'
        },
        after: {
          text: '烘后重(g)'
        },
        water: {
          text: '水分(%)'
        }
      },
      rightButtons: [
        {
          text: '统计信息',
          attrs: {
            type: 'primary'
          },
          click: async (id, index, row) => {
            const { data: res } = await this.$api.getTestReportStatistics(id)
            if (res.meta.code !== 0) {
              return this.$message.error(
                '获取统计信息失败: ' + res.meta.message
              )
            }
            this.statisticInfo = res.data
            this.statisticColumnsDesc = JSON.parse(res.data.statisticColumns)
            this.originColumnsDesc = JSON.parse(res.data.originColumns)
            this.originDataInfo = JSON.parse(res.data.originData)
            this.statisticDataInfo = JSON.parse(res.data.statisticData)
            this.waterDataInfo = res.data.waterInfos
            this.statisticKey = id
            this.statisticDialogVisible = true
          }
        }
      ],
      columnsDesc: {
        beginTime: {
          text: '测量时间'
        },
        productDate: {
          text: '生产日期'
        },
        specificationName: {
          text: '牌号名称'
        },
        turnName: {
          text: '班次名称'
        },
        machineModelName: {
          text: '机台名称'
        },
        finalRet: {
          text: '是否合格',
          options: reportRetList,
          type: 'status'
        },
        remark: {
          text: '备注'
        },
        phyScore: {
          text: '物理得分'
        },
        userName: {
          text: '操作员'
        }
      },
      formData: {},
      formDesc: {
        appearances: {
          label: '外观缺陷',
          type: 'table-editor',
          attrs: {
            columns: [
              {
                prop: 'dbId',
                label: 'ID',
                default: 0
              },
              {
                prop: 'indicatorId',
                label: '外观指标',
                width: 200,
                content: {
                  type: 'el-select',
                  attrs: {
                    filterable: 'filterable'
                  },
                  options: [],
                  on: {
                    change(val, row, col) {
                      console.log('123', val, row, col)
                    }
                  }
                }
              },
              {
                prop: 'frequency',
                label: '频次',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              },
              {
                prop: 'subScore',
                label: '扣分',
                content: {
                  type: 'el-input',
                  attrs: {
                    type: 'number'
                  }
                }
              }
            ],
            newColumnValue: {
              dbId: 0
            }
          }
        }
      },
      dialogFormVisible: false,
      selectedReport: {},
      formError: false
    }
  },
  created() {
    // 根据 router-tab 当前选中的页面重新设置当前路由
    reloadCurrentRoute(this.$tabs, this.$store)
    this.getOptions()
  },
  methods: {
    query() {
      const page = this.$refs.table.page
      const size = this.$refs.table.size
      this.getProductReports({ page, size })
      this.$refs.table.getData()
    },
    async getOptions() {
      const { data: res } = await this.$api.getOptions()
      if (res.meta.code !== 0) {
        return this.$message.error('获取选项信息失败')
      }
      this.specificationOptions = res.data.specifications
      this.specificationTypeOptions = res.data.specificationTypes
      this.turnOptions = res.data.turns
      this.machineModelOptions = res.data.machineModels
      this.appearanceIndicatorOptions = res.data.appearanceIndicators
    },
    async getProductReports(params) {
      const { data: res } = await this.$api.getTestReports(
        Object.assign(this.queryInfo, params)
      )
      console.log(res)
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取物测检验报表数据失败: ' + res.meta.message
        )
      }
      return res.data
    },
    getDateRange() {
      if (this.dateRange.length === 0) {
        this.queryInfo.beginDate = ''
        this.queryInfo.endDate = ''
      } else {
        this.queryInfo.beginDate = this.dateRange[0]
        this.queryInfo.endDate = this.dateRange[1]
      }
      this.query()
    },
    async addAppearance() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要添加外观缺陷的数据')
      }

      if (selectedData.length > 1) {
        this.$message.info('多选时将会默认对第一组数据进行操作')
      }
      this.selectedReport = selectedData[0]
      const { data: res } = await this.$api.getTestReportAppearance(
        selectedData[0].id
      )
      this.formData = res.data
      this.formDesc.appearances.attrs.columns[1].content.options =
        this.appearanceIndicatorOptions
      this.dialogFormVisible = true
    },
    getStatisticData() {
      return this.statisticDataInfo
    },
    getOriginDataInfo() {
      return this.originDataInfo
    },
    getWaterDataInfo() {
      return this.waterDataInfo
    },
    async handleSubmit(data) {
      this.formError = false
      const { data: res } = await this.$api.uploadTestReportAppearance(data)
      if (res.meta.code !== 0) {
        this.formError = true
        return this.$message.error('提交失败: ' + res.meta.message)
      }
    },
    async download() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要导出的数据')
      }
      const specificationNames = []
      const turnNames = []
      const ids = []
      selectedData.forEach((item) => {
        specificationNames.push(item.specificationName)
        turnNames.push(item.turnName)
        ids.push(item.id)
      })
      const notSameSpecifications = [...new Set(specificationNames)]
      const notSameTurns = [...new Set(turnNames)]
      if (notSameSpecifications.length > 1) {
        return this.$message.error('请选择牌号相同的数据')
      }

      if (notSameTurns.length > 1) {
        return this.$message.error('请选择班次相同的数据')
      }

      const res = await this.$api.downloadTestReports(ids)
      console.log('begin', res)
      const { data, headers } = res
      console.log('end', data, headers)
      const fileName = headers['content-disposition']
        .split(';')[1]
        .split('filename=')[1]

      const blob = new Blob([data], { type: headers['content-type'] })
      // const blob = new Blob(res)
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('测试检验报表' + fileName)
      dom.style.display = 'none'
      document.body.appendChild(dom)
      dom.click()
      dom.parentNode.removeChild(dom)
      window.URL.revokeObjectURL(url)
      this.$notify({
        title: '导出提示',
        message: '导出成功',
        type: 'success'
      })
    },
    handleSuccess() {
      if (!this.formError) {
        this.query()
        this.formData = {}
        this.dialogFormVisible = false
        this.$message.success('提交成功')
      }
    },
    handleClosed() {
      this.$refs.table.selectedData = []
      this.$refs.table.reset()
    }
  }
}
</script>

<style scoped>
.el-tag {
  margin-right: 15px;
}
.el-date-editor {
  width: 100%;
}
</style>

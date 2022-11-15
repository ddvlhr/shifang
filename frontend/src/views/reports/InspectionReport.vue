<template>
  <div class="main-container">
    <function-button
      @addAppearance="addAppearance"
      @download="download"
      @add="add"
    />
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
            placeholder="请选择机型"
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
        :request-fn="getInspectionReports"
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
      title="编辑巡检检验报表外观缺陷"
      :request-fn="handleSubmit"
      :visible.sync="dialogFormVisible"
      @request-success="handleSuccess"
      @close="handleClosed"
    ></ele-form-dialog>
    <el-dialog
      title="报表详情"
      :visible.sync="statisticDialogVisible"
      width="80%"
      @close="handleStatisticClosed"
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
      </el-tabs>

      <span slot="footer" class="dialog-footer">
        <el-button @click="statisticDialogVisible = false">关 闭</el-button>
      </span>
    </el-dialog>
    <ele-form-dialog
      v-model="reportFormData"
      width="70%"
      :form-desc="reportFormDesc"
      title="添加巡检检验报表"
      :visible.sync="reportDialogFormVisible"
      :request-fn="addReport"
      @request-success="handleReportSuccess"
    ></ele-form-dialog>
  </div>
</template>

<script>
import { reportRetList, finalRetList } from '@/assets/js/constant'
import { reloadCurrentRoute } from '@/utils/utils'
export default {
  data() {
    const self = this
    return {
      specificationOptions: [],
      specificationTypeOptions: [],
      turnOptions: [],
      machineModelOptions: [],
      appearanceIndicatorOptions: [],
      selectedDateValue: [],
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
      statisticDialogVisible: false,
      tabSelected: 'statistic',
      statisticColumnsDesc: {},
      originColumnsDesc: {},
      originDataInfo: [],
      statisticKey: '',
      statisticDataInfo: [],
      statisticInfo: {
        specificationName: ''
      },
      reportFormData: {},
      reportFormError: false,
      reportFormDesc: {
        testTime: {
          type: 'datetimerange',
          label: '测量时间',
          attrs: {
            valueFormat: 'yyyy-MM-dd HH:mm:ss'
          },
          required: true
        },
        productionTime: {
          type: 'date',
          label: '生产日期',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          }
        },
        deliverTime: {
          type: 'date',
          label: '发货日期',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          }
        },
        specificationId: {
          type: 'select',
          label: '牌号',
          options: async (data) => {
            return await this.specificationOptions
          },
          required: true
        },
        turnId: {
          type: 'select',
          label: '班次',
          options: async (data) => {
            return await this.turnOptions
          },
          required: true
        },
        machineModelId: {
          type: 'select',
          label: '机型',
          options: async (data) => {
            return await this.machineModelOptions
          },
          required: true
        },
        instance: {
          type: 'input',
          label: '仪器名称'
        }
      },
      reportDialogFormVisible: false,
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
      rightButtons: [
        {
          text: '统计信息',
          attrs: {
            type: 'primary'
          },
          click: async (id, index, row) => {
            const { data: res } = await self.$api.getInspectionStatistic(id)
            if (res.meta.code !== 0) {
              return self.$message.error(
                '获取统计信息失败: ' + res.meta.message
              )
            }
            this.statisticInfo = res.data
            this.statisticColumnsDesc = JSON.parse(res.data.statisticColumns)
            this.originColumnsDesc = JSON.parse(res.data.originColumns)
            this.originDataInfo = JSON.parse(res.data.originData)
            this.statisticDataInfo = JSON.parse(res.data.statisticData)
            this.statisticKey = id
            this.statisticDialogVisible = true
          }
        }
      ],
      formData: {},
      formDesc: {
        finalRet: {
          label: '报表状态',
          type: 'radio',
          options: finalRetList
        },
        total: {
          label: '得分',
          type: 'text'
        },
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
      this.getInspectionReports({ page, size })
      this.$refs.table.getData()
    },
    clearStatisticInfo() {
      this.statisticInfo = {}
      this.statisticColumnsDesc = {}
      this.originColumnsDesc = {}
      this.originDataInfo = []
      this.statisticDataInfo = []
    },
    handleStatisticClosed() {
      console.log('statisticDialogClose.')
      this.statisticInfo = {}
      this.statisticColumnsDesc = {}
      this.originColumnsDesc = {}
      this.originDataInfo = []
      this.statisticDataInfo = []
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
      console.log(res.data)
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
    async getInspectionReports(params) {
      const { data: res } = await this.$api.getInspectionReports(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取物测检验报表数据失败: ' + res.meta.message
        )
      }
      return res.data
    },
    async addAppearance() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要添加外观缺陷的数据')
      }

      if (selectedData.length > 1) {
        this.$message.info('多选时将会默认对第一组数据进行操作')
      }
      console.log(selectedData[0])
      this.selectedReport = selectedData[0]
      const { data: res } = await this.$api.getInspectionAppearances(
        selectedData[0].id
      )
      this.formData = res.data
      this.formDesc.appearances.attrs.columns[1].content.options =
        this.appearanceIndicatorOptions
      this.dialogFormVisible = true
    },
    async handleSubmit(data) {
      this.formError = false
      const { data: res } = await this.$api.updateInspectionReportAppearance(
        data
      )
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
      const res = await this.$api.downloadInspectionReport(ids)
      const { data, headers } = res
      console.log(headers)
      const fileName = headers['content-disposition']
        .split(';')[1]
        .split('filename=')[1]
      const blob = new Blob([data], { type: headers['content-type'] })
      const dom = document.createElement('a')
      const url = window.URL.createObjectURL(blob)
      dom.href = url
      dom.download = decodeURI('巡检检验报表' + fileName)
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
    add() {
      console.log(this.specificationOptions)
      this.reportFormDesc.specificationId.options = this.specificationOptions
      this.reportDialogFormVisible = true
    },
    async addReport(data) {
      this.reportFormError = false
      const { data: res } = await this.$api.addInspectionReport(data)
      if (res.meta.code !== 0) {
        this.reportFormError = true
        return this.$message.error('添加报表失败: ' + res.meta.message)
      }
    },
    handleReportSuccess() {
      if (!this.reportFormError) {
        this.query()
        this.reportFormData = {}
        this.reportDialogFormVisible = false
        this.$message.success('添加成功')
      }
    },
    getStatisticData() {
      return this.statisticDataInfo
    },
    getOriginDataInfo() {
      return this.originDataInfo
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

<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-16 22:49:34
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 15:42:08
 * @FilePath: /frontend/src/views/reports/ProcessInspectionReport.vue
 * @Description:
-->
<template>
  <div class="main-container">
    <function-button @add="add" @remove="remove" @download="download" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6">
            <query-select
              :options="specificationOptions"
              v-model="queryInfo.specificationId"
              placeholder="牌号筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-select
              :options="turnOptions"
              v-model="queryInfo.turnId"
              placeholder="班次筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-select
              :options="machineOptions"
              v-model="queryInfo.machineId"
              placeholder="机台筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6" :offset="0">
            <query-select
              :options="qualityResultOptions"
              v-model="queryInfo.state"
              placeholder="判定结果筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="6" :offset="0">
            <el-date-picker
              v-model="dateRange"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              @change="query"
              @clear="query"
            >
            </el-date-picker>
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnsDesc"
        :is-show-index="true"
        :right-buttons="rightButtons"
        :request-fn="getProcessInspectionReports"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        width="80%"
        v-model="formData"
        :form-desc="formDesc"
        :form-error="formError"
        :rules="rules"
        :title="dialogTitle"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @closed="handleClosed"
      >
      </ele-form-dialog>
    </el-card>
  </div>
</template>

<script>
export default {
  data() {
    return {
      queryInfo: {
        specificationId: '',
        turnId: '',
        machineId: '',
        query: '',
        result: '',
        begin: '',
        end: ''
      },
      specificationOptions: [],
      turnOptions: [],
      machineOptions: [],
      defectOptions: [],
      qualityResultOptions: this.$store.state.app.dicts.qualityResult,
      dateRange: [],
      userOptions: [],
      rightButtons: [],
      columnsDesc: {
        time: {
          text: '时间'
        },
        specificationName: {
          text: '牌号'
        },
        turnName: {
          text: '班次'
        },
        machineName: {
          text: '机台'
        },
        operatorName: {
          text: '操作员'
        },
        score: {
          text: '扣分值'
        },
        result: {
          text: '判定结果',
          type: 'status',
          options: this.$store.state.app.dicts.qualityTypeResult
        }
      },
      dialogTitle: '新增',
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        time: {
          label: '检测时间',
          type: 'date',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          },
          default: new Date()
        },
        specificationId: {
          label: '牌号',
          type: 'select',
          options: async () => {
            return await this.specificationOptions
          }
        },
        turnId: {
          label: '班次',
          type: 'select',
          options: async () => {
            return await this.turnOptions
          }
        },
        machineId: {
          label: '机台',
          type: 'select',
          options: async () => {
            return await this.machineOptions
          }
        },
        operatorName: {
          label: '操作员',
          type: 'select',
          options: async () => {
            return await this.userOptions
          },
          prop: { text: 'text', value: 'text' },
          default: this.$store.state.user.userInfo.nickName
        },
        inspector: {
          label: '检验员',
          type: 'select',
          options: async () => {
            return await this.userOptions
          },
          prop: { text: 'text', value: 'text' },
          default: this.$store.state.user.userInfo.nickName
        },
        weightUpper: {
          label: '质量偏大支数',
          type: 'number'
        },
        weightLower: {
          label: '质量偏小支数',
          type: 'number'
        },
        resistanceUpper: {
          label: '吸阻偏大支数',
          type: 'number'
        },
        resistanceLower: {
          label: '吸阻偏小支数',
          type: 'number'
        },
        otherIndicators: {
          label: '其他指标',
          type: 'textarea'
        },
        otherCount: {
          label: '其他指标支数',
          type: 'number'
        },
        defects: {
          label: '缺陷',
          type: 'table-editor',
          attrs: {
            columns: [
              {
                label: '#',
                width: 50,
                type: 'index'
              },
              {
                label: '缺陷',
                prop: 'defectId',
                content: {
                  type: 'el-select',
                  options: [],
                  attrs: {
                    filterable: true
                  }
                }
              },
              {
                label: '数量',
                prop: 'count',
                content: {
                  type: 'el-input-number',
                  default: 1
                }
              }
            ]
          }
        },
        batchUnqualified: {
          label: '批不合格项',
          type: 'textarea'
        },
        remark: {
          label: '备注',
          type: 'textarea'
        }
      },
      rules: {
        time: [
          { required: true, message: '请选择检测时间', trigger: 'change' }
        ],
        specificationId: [
          { required: true, message: '请选择牌号', trigger: 'change' }
        ],
        turnId: [{ required: true, message: '请选择班次', trigger: 'change' }],
        machineId: [
          { required: true, message: '请选择机台', trigger: 'change' }
        ],
        operatorName: [
          { required: true, message: '请输入操作员', trigger: 'blur' }
        ],
        inspector: [
          { required: true, message: '请输入检验员', trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.getOptions()
    this.setRightButtons()
  },
  methods: {
    async setRightButtons() {
      this.rightButtons = await this.$utils.initRightButtons(this)
    },
    async getOptions() {
      Promise.all([
        this.$api.getSpecificationOptions(),
        this.$api.getTurnOptions(),
        this.$api.getMachineOptions(),
        this.$api.getDefectOptions(),
        this.$api.getUserOptions()
      ]).then((res) => {
        this.specificationOptions = res[0].data.data
        this.turnOptions = res[1].data.data
        this.machineOptions = res[2].data.data
        this.defectOptions = res[3].data.data
        this.userOptions = res[4].data.data
      })
    },
    query() {
      if (this.dateRange !== null) {
        this.queryInfo.begin = this.dateRange[0]
        this.queryInfo.end = this.dateRange[1]
      } else {
        this.queryInfo.begin = ''
        this.queryInfo.end = ''
      }
      this.$utils.queryTable(this, this.getProcessInspectionReports)
    },
    async getProcessInspectionReports(params) {
      const { data: res } = await this.$api.getWrapProcessInspectionReports(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      return res.data
    },
    add() {
      this.dialogTitle = '新增'
      this.dialogFormVisible = true
    },
    edit(data, that) {
      that.formData = data
      that.dialogTitle = '编辑'
      that.dialogFormVisible = true
      that.formDesc.defects.attrs.columns[1].content.options =
        that.defectOptions
    },
    async remove() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.warning('请选择要删除的数据')
      }

      const confirm = await this.$confirm(
        '此操作将永久删除该数据, 是否继续?',
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      )

      if (confirm === 'confirm') {
        const ids = selectedData.map((item) => item.id)
        const { data: res } =
          await this.$api.removeWrapProcessInspectionReports(ids)
        if (res.meta.code !== 0) {
          return this.$message.error(res.meta.message)
        }

        this.query()
        this.$message.success(res.meta.message)
      }
    },
    download() {},
    async handleSubmit(data) {
      const { data: res } = await this.$api.editWrapProcessInspectionReport(
        data
      )
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }

      this.query()
      this.dialogFormVisible = false
      return this.$message.success(res.meta.message)
    },
    handleClosed() {
      this.formData = {
        time: new Date()
      }
      this.formError = {}
    }
  }
}
</script>

<style lang="scss" scoped></style>

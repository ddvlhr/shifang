<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-09 14:00:16
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 15:43:07
 * @FilePath: /frontend/src/views/reports/QualityInspectionReport.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <function-button @add="add" @remove="remove" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6">
            <query-input v-model="queryInfo.query" />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="specificationOptions"
              v-model="queryInfo.specificationId"
              placeholder="牌号筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="teamOptions"
              v-model="queryInfo.teamId"
              placeholder="班组筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="turnOptions"
              v-model="queryInfo.turnId"
              placeholder="班次筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="6">
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
          <el-col :span="6">
            <query-select
              :options="volumePickUpOptions"
              v-model="queryInfo.volumePickUpId"
              placeholder="卷接机筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="packagingMachineOptions"
              v-model="queryInfo.packagingMachineId"
              placeholder="包装机筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="qualityResult"
              v-model="queryInfo.result"
              placeholder="状态筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnDesc"
        :is-show-index="true"
        :right-buttons="rightButtons"
        :request-fn="getQualityInspectionReports"
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
        :title="isEdit ? '编辑' : '新增'"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @closed="handleClosed"
      >
      </ele-form-dialog>
    </el-card>
  </div>
</template>

<script>
import { initRightButtons, queryTable } from '@/utils'
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        result: '',
        specificationId: '',
        teamId: '',
        turnId: '',
        volumePickUpId: '',
        packagingMachineId: '',
        beginTime: '',
        endTime: ''
      },
      dateRange: [],
      qualityResult: this.$store.state.app.dicts.qualityResult,
      rightButtons: [],
      specificationOptions: [],
      teamOptions: [],
      turnOptions: [],
      volumePickUpOptions: [],
      packagingMachineOptions: [],
      defectOptions: [],
      userOptions: [],
      columnDesc: {
        time: {
          text: '日期'
        },
        specificationName: {
          text: '牌号'
        },
        teamName: {
          text: '班组'
        },
        turnName: {
          text: '班次'
        },
        volumePickUpName: {
          text: '卷接机'
        },
        packagingMachineName: {
          text: '包装机'
        },
        count: {
          text: '专检次数'
        },
        result: {
          text: '质量等级',
          type: 'status',
          options: this.$store.state.app.dicts.qualityTypeResult
        }
      },
      isEdit: false,
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        time: {
          label: '时间',
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
          },
          attrs: {
            filterable: true
          }
        },
        teamId: {
          label: '班组',
          type: 'select',
          options: async () => {
            return await this.teamOptions
          },
          attrs: {
            filterable: true
          }
        },
        turnId: {
          label: '班次',
          type: 'select',
          options: async () => {
            return await this.turnOptions
          },
          attrs: {
            filterable: true
          }
        },
        volumePickUpId: {
          label: '卷接机',
          type: 'select',
          options: async () => {
            return await this.volumePickUpOptions
          },
          attrs: {
            filterable: true
          }
        },
        packagingMachineId: {
          label: '包装机',
          type: 'select',
          options: async () => {
            return await this.packagingMachineOptions
          },
          attrs: {
            filterable: true
          }
        },
        count: {
          label: '专检次数',
          type: 'input'
        },
        orderNo: {
          label: '烟丝批号',
          type: 'input'
        },
        inspector: {
          label: '操作员',
          type: 'select',
          options: async () => {
            return await this.userOptions
          },
          attrs: {
            filterable: true
          },
          prop: { text: 'text', value: 'valueStr' }
        },
        volumePickUpOperator: {
          label: '卷接机操作员',
          type: 'select',
          options: async () => {
            return await this.userOptions
          },
          attrs: {
            filterable: true
          },
          prop: { text: 'text', value: 'valueStr' }
        },
        packagingMachineOperator: {
          label: '包装机操作员',
          type: 'select',
          options: async () => {
            return await this.userOptions
          },
          attrs: {
            filterable: true
          },
          prop: { text: 'text', value: 'valueStr' }
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
        }
      },
      rules: {
        specificationId: [
          { required: true, message: '请选择牌号', trigger: 'blur' }
        ],
        teamId: [{ required: true, message: '请选择班组', trigger: 'blur' }],
        turnId: [{ required: true, message: '请选择班次', trigger: 'blur' }],
        volumePickUpId: [
          { required: true, message: '请选择卷接机', trigger: 'blur' }
        ],
        packagingMachineId: [
          { required: true, message: '请选择包装机', trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.setRightButtons()
    this.getOptions()
  },
  methods: {
    query() {
      if (this.dateRange !== null) {
        this.queryInfo.beginTime = this.dateRange[0]
        this.queryInfo.endTime = this.dateRange[1]
      } else {
        this.queryInfo.beginTime = ''
        this.queryInfo.endTime = ''
      }
      queryTable(this, this.getQualityInspectionReports)
    },
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    async getOptions() {
      Promise.all([
        this.$api.getSpecificationOptions(),
        this.$api.getTeamOptions(),
        this.$api.getTurnOptions(),
        this.$api.getVolumePickUpOptions(),
        this.$api.getPackagingMachineOptions(),
        this.$api.getUserOptions(),
        this.$api.getDefectOptions()
      ]).then((res) => {
        this.specificationOptions = res[0].data.data
        this.teamOptions = res[1].data.data
        this.turnOptions = res[2].data.data
        this.volumePickUpOptions = res[3].data.data
        this.packagingMachineOptions = res[4].data.data
        this.userOptions = res[5].data.data
        this.defectOptions = res[6].data.data
      })
    },
    async getQualityInspectionReports(params) {
      const { data: res } = await this.$api.getWrapQualityInspectionReports(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取卷包质量报表列表失败: ' + res.meta.message
        )
      }
      return res.data
    },
    add() {
      this.isEdit = false
      this.dialogFormVisible = true
    },
    edit(data, that) {
      that.formData = data
      that.isEdit = true
      that.dialogFormVisible = true
      that.formDesc.defects.attrs.columns[1].content.options =
        that.defectOptions
    },
    async remove() {
      const selectedData = this.$refs.table.selectedData
      if (selectedData.length === 0) {
        return this.$message.error('请选择需要删除的数据')
      }
      const confirm = await this.$confirm(
        '此操作将永久删除选择的数据, 是否继续?',
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      )

      if (confirm === 'confirm') {
        const ids = selectedData.map((item) => {
          return item.id
        })
        const { data: res } =
          await this.$api.removeWrapQualityInspectionReports(ids)
        if (res.meta.code !== 0) {
          return this.$message.error(res.meta.message)
        }
        this.query()
        this.$message.success(res.meta.message)
      }
    },
    async handleSubmit(data) {
      const { data: res } = await this.$api.editWrapQualityInspectionReport(
        data
      )
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      this.query()
      this.dialogFormVisible = false
      this.$message.success(res.meta.message)
    },
    handleClosed() {
      this.formData = { time: new Date() }
    }
  }
}
</script>

<style lang="scss" scoped></style>

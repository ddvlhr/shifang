<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-07 13:40:50
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 15:41:41
 * @FilePath: /frontend/src/views/reports/ManualInspectionReport.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <function-button @add="add" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="8">
            <query-input
              v-model="queryInfo.query"
              @click="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="8">
            <query-select
              :options="specificationOptions"
              v-model="queryInfo.specificationId"
              placeholder="牌号筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
          <el-col :span="8">
            <query-select
              :options="qualityResult"
              v-model="queryInfo.result"
              placeholder="状态筛选"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="8" :offset="0">
            <el-date-picker
              v-model="dateRange"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              @change="query"
              @clear="query"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnDesc"
        :is-show-index="true"
        :request-fn="getReports"
        :right-buttons="rightButtons"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        width="80%"
        v-model="formData"
        :form-desc="formDesc"
        :form-error="formError"
        :title="isEdit ? '编辑' : '新增'"
        :request-fn="handleSubmit"
        :visible.sync="dialogFormVisible"
        @request-success="handleSuccess"
        @closed="handleClosed"
      >
      </ele-form-dialog>
    </el-card>
  </div>
</template>

<script>
import { queryTable, initRightButtons } from '@/utils'
export default {
  data() {
    return {
      queryInfo: {
        specificationId: '',
        count: '',
        result: '',
        begin: '',
        end: ''
      },
      dateRange: [],
      specificationOptions: [],
      defectOptions: [],
      rightButtons: [],
      qualityResult: this.$store.state.app.dicts.qualityResult,
      columnDesc: {
        time: {
          text: '时间'
        },
        specificationName: {
          text: '牌号'
        },
        operation: {
          text: '操作工'
        },
        count: {
          text: '数量'
        },
        result: {
          text: '判定结果',
          type: 'status',
          options: this.$store.state.app.dicts.qualityTypeResult
        }
      },
      isEdit: false,
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        specificationId: {
          label: '牌号',
          type: 'select',
          attrs: {
            filterable: true
          },
          options: async () => {
            return await this.specificationOptions
          }
        },
        time: {
          label: '时间',
          type: 'date',
          attrs: {
            valueFormat: 'yyyy-MM-dd'
          },
          default: new Date()
        },
        operation: {
          label: '操作员',
          type: 'input',
          default: this.$store.state.user.userInfo.nickName
        },
        count: {
          label: '数量',
          type: 'number'
        },
        defectInfo: {
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
      }
    }
  },
  created() {
    this.getOptions()
    this.setRightButtons()
  },
  methods: {
    async getOptions() {
      Promise.all([
        this.$api.getSpecificationOptions(),
        this.$api.getDefectOptions()
      ]).then((res) => {
        this.specificationOptions = res[0].data.data
        this.defectOptions = res[1].data.data
      })
    },
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    query() {
      if (this.dateRange !== null) {
        this.queryInfo.begin = this.dateRange[0]
        this.queryInfo.end = this.dateRange[1]
      } else {
        this.queryInfo.begin = ''
        this.queryInfo.end = ''
      }
      queryTable(this, this.getReports)
    },
    async getReports(params) {
      const { data: res } = await this.$api.getManualInspectionReports(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }
      return res.data
    },
    add() {
      this.isEdit = false
      this.dialogFormVisible = true
      this.formDesc.defectInfo.attrs.columns[1].content.options =
        this.defectOptions
    },
    edit(data, that) {
      that.isEdit = true
      that.formData = data
      that.dialogFormVisible = true
      that.formDesc.defectInfo.attrs.columns[1].content.options =
        that.defectOptions
    },
    async handleSubmit(data) {
      // return console.log(data)
      const { data: res } = await this.$api.editManualInspectionReport(data)
      if (res.meta.code !== 0) {
        return this.$message.error(res.meta.message)
      }

      this.query()
      this.dialogFormVisible = false
      this.$message.success(res.meta.message)
    },
    handleSuccess() {},
    handleClosed() {
      this.formData = {
        time: new Date(),
        operation: this.$store.state.user.userInfo.nickName
      }
    }
  }
}
</script>

<style lang="scss" scoped></style>

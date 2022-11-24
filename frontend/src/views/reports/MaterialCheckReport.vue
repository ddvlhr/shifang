<!--
 * @Author: ddvlhr 354874258@qq.com
 * @Date: 2022-11-02 16:51:59
 * @LastEditors: ddvlhr 354874258@qq.com
 * @LastEditTime: 2022-11-24 15:40:56
 * @FilePath: /frontend/src/views/reports/MaterialCheckReport.vue
 * @Description: 
-->
<template>
  <div class="main-container">
    <function-button @add="add" />
    <el-card shadow="never">
      <div slot="header">
        <el-row :gutter="20">
          <el-col :span="6">
            <query-select
              :options="specificationOptions"
              v-model="queryInfo.specificationId"
              placeholder="牌号筛选"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="teamOptions"
              v-model="queryInfo.teamId"
              placeholder="班组筛选"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="turnOptions"
              v-model="queryInfo.turnId"
              placeholder="班次筛选"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="machineOptions"
              v-model="queryInfo.machineId"
              placeholder="机台筛选"
            />
          </el-col>
        </el-row>
        <el-row :gutter="20" class="mt-3">
          <el-col :span="6">
            <query-select
              :options="qualified"
              v-model="queryInfo.qualified"
              placeholder="判定状态筛选"
            />
          </el-col>
          <el-col :span="6">
            <query-select
              :options="materialCheckStatus"
              v-model="queryInfo.state"
              placeholder="流程状态筛选"
            />
          </el-col>
        </el-row>
      </div>

      <ele-table
        :columns-desc="columnDesc"
        :is-show-index="true"
        :right-buttons="rightButtons"
        :request-fn="getMaterialCheckReports"
        :is-show-top-delete="false"
        :is-show-right-delete="false"
        ref="table"
      ></ele-table>
      <ele-form-dialog
        v-model="formData"
        :form-desc="formDesc"
        :form-error="formError"
        title="物检申检报表"
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
        state: '',
        specificationId: '',
        teamId: '',
        turnId: '',
        machineId: '',
        measureTypeId: '',
        qualified: ''
      },
      qualified: this.$store.state.app.dicts.qualified,
      materialCheckStatus: this.$store.state.app.dicts.materialCheckStatus,
      specificationOptions: [],
      teamOptions: [],
      turnOptions: [],
      machineOptions: [],
      measureTypeOptions: [],
      rightButtons: [],
      columnDesc: {
        testDate: {
          text: '检测时间'
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
        machineName: {
          text: '机台'
        },
        measureTypeName: {
          text: '测量类型'
        },
        qualified: {
          text: '合格状态',
          type: 'status',
          options: this.$store.state.app.dicts.qualifiedTypes
        },
        materialCheckStatus: {
          text: '报表状态',
          type: 'status',
          options: this.$store.state.app.dicts.materialCheckStatusTypes
        }
      },
      dialogFormVisible: false,
      formData: {},
      formError: {},
      formDesc: {
        testDate: {
          label: '检测日期',
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
        teamId: {
          label: '班组',
          type: 'select',
          options: async () => {
            return await this.teamOptions
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
        measureTypeId: {
          label: '测量类型',
          type: 'select',
          options: async () => {
            return await this.measureTypeOptions
          }
        },
        orginator: {
          label: '发起人',
          type: 'text',
          default: this.$store.state.user.userInfo.nickName,
          attrs: {
            readonly: true
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
    query() {
      queryTable(this, this.getMaterialCheckReports)
    },
    async setRightButtons() {
      this.rightButtons = await initRightButtons(this)
    },
    async getOptions() {
      Promise.all([
        this.$api.getSpecificationOptions(),
        this.$api.getTeamOptions(),
        this.$api.getTurnOptions(),
        this.$api.getMachineOptions(),
        this.$api.getMeasureTypeOptions()
      ]).then((res) => {
        this.specificationOptions = res[0].data.data
        this.teamOptions = res[1].data.data
        this.turnOptions = res[2].data.data
        this.machineOptions = res[3].data.data
        this.measureTypeOptions = res[4].data.data
      })
    },
    async getMaterialCheckReports(params) {
      const { data: res } = await this.$api.getMaterialCheckReports(
        Object.assign(this.queryInfo, params)
      )
      if (res.meta.code !== 0) {
        return this.$message.error(
          '获取物资申检报表列表失败: ' + res.meta.message
        )
      }
      return res.data
    },
    add() {
      this.dialogFormVisible = true
    },
    edit(data, that) {
      that.formData = data
      that.dialogFormVisible = true
    },
    async handleSubmit(data) {
      const { data: res } = await this.$api.editMaterialCheckReport(data)
      if (res.meta.code !== 0) {
        return this.$message.error('提交失败: ' + res.meta.message)
      }
      this.query()
      this.dialogFormVisible = false
      this.$message.success('提交成功')
    },
    handleClosed() {
      this.formData = { testDate: new Date() }
    }
  }
}
</script>

<style lang="scss" scoped></style>
